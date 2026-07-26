## ILogger
`ILogger`는 `Microsoft.Extensions.Logging.Abstractions`에 정의된 인터페이스로, 실제 로그 저장 방식(콘솔/파일/Azure 등)과 완전히 분리된 로깅 API의 최소 추상화 입니다.
```c#
public interface ILogger
{
    void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception? exception, Func<TState, Exception?, string> formatter);
    bool IsEnabled(LogLevel logLevel);
    IDisposable? BeginScope<TState>(TState state);
}
```
- Log<TState>: 실제로 로그를 기로하는 메서드.
  - `LogInformation`, `LogWarning` 같은 건 이 `Log`를 감싸는 확장 메서드임
- IsEnabled: 이 레벨의 로그가 현재 설정상 기록될지 미리 확인하는 메서드
- BeginScope: 범위(Scope) 시작

<br>

## 로깅 공급자
로그를 어디에 쓸지 결정하는 것으로 로그 메시지 자체는 `ILogger`라는 공통 API로 남기지만, 실제로 그게 콘솔에 찍힐지, 파일에 저장될지, Azure에 전송될지는 어떤 공급자를 등롭했는지에 따라 달라집니다.

### 기본적으로 켜져 있는 공급자 4개
- Console: 콘솔 창에 텍스트로 출력
- Debug: 디버거의 출력 창에 출력
- EventSource: Windows의 ETW(이벤트 추적) 시스템에 연동
- Windows EventLog: Windows 이벤트 로그에 기록

기본 공급자를 없애고 원하는 것만 사용하려면 
```c#
var builder = WebApplication.CreateBuilder(args);

builder.Logging.ClearProviders();  // 기존 공급자 다 제거
builder.Logging.AddConsole();      // Console만 다시 추가
```

<br>

## 문자열 보간 vs 메시지 템플릿
아래 두 코드는 콘솔 출력이 똑같아 보이지만 내부적으로 동작이 다릅니다.

```c#
// (A) 문자열 보간 — 문제 
_logger.LogInformation($"Post {request.Title} created by {request.AuthorName}");

// (B) 메시지 템플릿 — 올바른 방식
_logger.LogInformation("Post {Title} created by {AuthorName}", request.Title, request.AuthorName);
```

### 왜 다른가
```c#
public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, ...)
{
    if (!IsEnabled(logLevel))
    {
        return;  // formatter 호출 자체를 안 함
    }
    // ... provider들에게 전달
}
```
- (A): $"..." 보간은 `LogInformation()` 호출 이전에 c# 컴파일러가 이미 완성된 string으로 만들어버림
  - `IsEnabled` 체크는 그 다믕리ㅏ, 로그가 버려지든 말든 문자열 생성 비용이 무조건 발생
- (B): 템플릿 문자열과 원본 값(Title, AuthorName)이 따로 Log()에 전달됨
  - 실제로 하나의 문자열로 합치는 작업(formatter 호출)은 IsEnabled == true일 때만 지연 실행(lazy)

즉 로그가 버려질 수 있는데 문자열 보간 사용 시 문자열을 만드는 비용 + GC 비용이 발생함(바로 가비지가 되기 때문)

<br>

## Provider별 출력 형식의 차이 - 구조화
콘솔에 찍히는 아래 형식은 Console 공급자의 Simple 포맷터가 만드는 텍스트일 뿐입니다.
모든 provider가 같은 형식으로 남기지 않습니다.
```c#
info: BoardApi.Services.PostService[0]
Post KMP 스터디 모집 created by 동키
```

| 공급자 | 형식 | 구조화 여부 |
|---|---|---|
| Console / Debug | 완성된 텍스트 문자열 | ❌ Title/AuthorName은 문자열로 합쳐지며 소실 |
| Windows EventLog | EventId/EntryType은 필드 분리, 메시지 본문은 문자열 | 🔶 부분적 |
| EventSource | 키워드 설정에 따라 Message/FormatMessage/MessageJson 선택 | ✅ MessageJson 키워드 사용 시 구조 보존 |
| Serilog, ApplicationInsights, Seq 등 | Title/AuthorName이 독립 필드로 저장 | ✅ 완전 보존, 쿼리 가능 |

LogInformation`LogInformation("Post {Title} by {AuthorName}", ...)` 호출 시점엔 이미 구조화된 key-value가 모든 provider에 전달됩니다.
"구조 소실"은 호출부가 아니라 **각 provider 내부**에서 문자열로 렌더링하는 시점에 발생합니다.

### EventSource로 구조화 검색이 가능할까?
방출(emit) 자체는 구조화되지만, 그걸 받아서 저장/인덱싱하는 계층이 없으면 검색 불가능합니다. 

선택지로는 아래와 같습니다.
- dotnet-trace / PerfView — 실시간 캡처, 트러블슈팅 용도. 저장 형식이 쿼리 친화적이지 않음.
- `EventListener` 직접 구현 — 구독 + 저장 + 쿼리 UI까지 직접 만들어야 함.
- Serilog/Seq/ApplicationInsights — 수집·저장·쿼리를 완제품으로 제공.

보통 EventSource는 성능 프로파일링/진단 용도로 쓰이고, "검색 가능한 로그 저장"은 대부분 3번(구조화 로깅 provider)이 담당.

<br>

## Serilog
ASP.NET CORE의 기본 `ILogger` 파이프라인을 대체하는 게 아니라, 그 위에 얹혀서 동작하는 구조화 로깅 라이브러리.

코드에서 똑같이 `ILogger<T>`, `_logger.LogInformatio()` 처럼 그대로 쓰고, 내부적으로 이 로그를 받아서 처리하는 provider 역할을 Serilog가 대신 맡는 구조'

기본 4개 기본 공급자(Console,Debug,EventSource,EventLog)가 못 해주던것들을 Serilog가 해결
- 파일 로깅 (기본 제공 없음 -> Serilog.Sinks.File)
- 구조화된 필드 그대로 저장 (Title, AuthorName이 문자열로 뭉개지지 않고 필드로 보존)
- 여러 목적지(Sink)에 동시 전송을 선언적으로 구성
- 요청 단위로 공통 정보(RequestId, UserId 등)를 자동으로 붙이는 기능(Enricher)

### 추가하면 좋은 점
- Console/Debug 공급자는 구조가 소실된 텍스트만 남김
- EventSource는 구조를 보존하지만 저장/검색 계층이 없어서 직접 EventListener를 구현해야 함
- Serilog는 "구조 보존 + 저장 + 쿼리"를 제공

### 필요한 패키지
| 패키지 | 역할 |
|---|---|
| `Serilog.AspNetCore` | ASP.NET Core 통합 진입점. `UseSerilog()` 확장 메서드 제공. 이거 하나만 넣어도 대부분 시나리오 커버됨 |
| `Serilog.Sinks.Console` | 콘솔에 구조화된 형식으로 출력 (기본 Console 공급자 대체) |
| `Serilog.Sinks.File` | 파일에 기록, 날짜별/크기별 롤링 지원 |
| `Serilog.Sinks.Seq` | Seq(로그 검색 전용 서버)로 전송 — 로컬에서 구조화 검색 UI 써보고 싶을 때 |
| `Serilog.Enrichers.Environment` | MachineName, EnvironmentName 같은 정보를 모든 로그에 자동 첨부 |
| `Serilog.Settings.Configuration` | appsettings.json으로 Serilog 설정을 관리할 수 있게 해줌 (코드에 하드코딩 안 해도 됨) |


설치:
```bash
dotnet add package Serilog.AspNetCore
dotnet add package Serilog.Sinks.File
dotnet add package Serilog.Enrichers.Environment
dotnet add package Serilog.Settings.Configuration
dotnet add package Serilog.Enrichers.Thread
dotnet add package Serilog.Formatting.Compact
```

### Sink란
로그가 최종적으로 흘러가는 목적지 <br>
Console, File, Seq 등이 각각 하나의 Sink. 하나의 로그 이벤트는 등록된 모든 Sink에 동시에 전달됨

### Enricher란
모든 로그 이벤트에 자동으로 추가 필드를 붙여주는 것. <br>
직접 `LogInformation`에서 값을 넘기지 않아도, 로그가 Sink로 나가기 전에 중간에서 필드를 끼워 넣는 역할

### Formatter란
Sink로 보낼 때 로그 이벤트를 어떤 텍스트/구조로 렌더링할지 결정하는 것. <br>
Sink는 어디로 보낼지, Formatter는 어떤 모양으로 만들지를 결정

같은 File Sink라도 Formatter를 바꾸면 사람이 읽기 좋은 텍스트로 남길수도, JSON으로 남길 수도 있음

<br>

### Serilog.Enrichers.Thread (Enricher의 예시)
로그에 ThreadId/ThreadName을 자동으로 붙이는 Enricher.
```c#
config.Enrich.WithThreadId()
      .Enrich.WithThreadName();
```
동시 요청이 들어오면 여러 스레드가 로그를 뒤섞어 남기는데, ThreadId를 붙이면 최소한의 구분이 가능<br>
다만 async/await 환경에서는 하나의 논리적 요청이 여러 스레드를 오갈 수 있어 완벽한 요청 단위 추적은 아님
- ThreadId/RequestId(ActivityTrackingOptions, BeginScope + Enrich.FromLogContext)와 함꼐 쓰는 것을 권장

### Serilog.Formatting.Compact (Formatter의 예시)
로그 출력 형실을 CLEF - 한 줄 JSON으로 바꿔주는 Formatter.
```c#
CompactJsonFormatter 적용 시:
```json
{"@t":"2026-07-26T10:23:01.123Z","@mt":"Post {Title} created by {AuthorName}","Title":"KMP 스터디 모집","AuthorName":"동키"}
```

<br>

## CLEF 예약 필드 (@ 로 시작하는 필드)

@ 접두사가 붙은 필드는 로그 이벤트 자체의 메타데이터. 개발자가 넣은 커스텀 필드(Title, AuthorName 등)와 구분됨.

| 필드 | 의미 |
|---|---|
| @t | 타임스탬프 (UTC, ISO 8601) |
| @mt | 원본 메시지 템플릿 (값이 채워지기 전 원문) — 실제 값은 별도 필드로 분리 저장됨 |
| @l | 로그 레벨. Information이면 생략됨 (기본값이라 필드 자체를 안 만듦) |
| @tr | TraceId — 하나의 HTTP 요청 전체에서 발생하는 모든 로그가 공유하는 값. 이 값으로 필터링하면 요청 하나의 전체 흐름을 재구성 가능 |
| @sp | SpanId — Trace 내부의 더 세부적인 작업 단위. 기본 설정에서는 요청당 1개(=Trace와 동일)이지만, Activity.StartActivity()로 구간을 나누면 여러 개로 분리됨 |
| @r | 포맷 지정자({Elapsed:0.0000})가 있는 경우의 최종 렌더링 문자열. 원본 숫자값은 별도 필드에 그대로 보존됨 |

<br>

## 로그 가시성 개선 체크리스트

### DbCommand 
Microsoft.EntityFrameworkCore.Database.Command 카테고리가 Information 레벨로 찍힘.
appsettings의 MinumLevel.Default가 Information이면 자동으로 다 보임.

운영 환경에서는 보통 로그랑/노이즈 방지를 위해 Warning으로 낮춤
```
"Override": { "Microsoft.EntityFrameworkCore": "Warning" }
```


### 메시지 템플릿 명명 규칙 통일하기
- [Entity] {Id} [동사-과거형] [detail] 형식으로 통일
  -   예: "Post {Id} deleted", "Post {Id} not found", "Post {Title} created by {AuthorName}"

### EventId를 정적 클래스로 관리하고 Name까지 부여
```c#
public static class PostLogEvents
{
    public static readonly EventId Created = new(1001, "PostCreated");
    public static readonly EventId NotFound = new(1003, "PostNotFound");
}
// JSON 로그에 EventId.Name까지 남아서 이벤트 종류별 필터링이 쉬워짐
```
