## HTTP는 원래 상태가 없다 (Stateless)

HTTP는 요청-응답 하나로 끝나는 프로토콜입니다. 같은 브라우저가 보낸 두 번째 요청이라도 서버 입장에서는 "완전히 새로운 요청"이라 이전 요청과 연결 지을 방법이 없습니다.

그런데 로그인 기능은 "이 요청이 로그인한 사용자가 보낸 요청이 맞는지"를 매 요청마다 알아야 합니다. 그래서 상태가 없는 프로토콜 위에 "이 요청들은 같은 사용자다"라는 상태를 인위적으로 얹는 장치가 필요한데, 그게 세션(Session)과 쿠키(Cookie)입니다.

<br>

## 쿠키(Cookie)란

쿠키는 서버가 브라우저에게 "이 값을 저장해뒀다가 이후 요청마다 같이 보내줘"라고 요청하는 메커니즘입니다.

1. 서버가 응답 헤더에 `Set-Cookie: sessionId=abc123` 를 실어 보냄
2. 브라우저가 그 값을 저장
3. 이후 같은 도메인으로 보내는 모든 요청에 브라우저가 자동으로 `Cookie: sessionId=abc123` 헤더를 붙여서 보냄

핵심은 **브라우저가 자동으로 붙여준다**는 것입니다. 개발자가 매번 값을 실어 보낼 필요가 없다는 게 장점이자, 뒤에서 다룰 CSRF 문제의 원인이기도 합니다.

### 쿠키의 주요 속성
| 속성 | 의미 |
|---|---|
| `HttpOnly` | JavaScript(`document.cookie`)로 읽을 수 없게 막음. XSS로 탈취되는 걸 방지 |
| `Secure` | HTTPS 연결에서만 전송 |
| `SameSite` | 다른 사이트에서 발생한 요청에 쿠키를 실어 보낼지 결정. `Strict`/`Lax`/`None` |
| `Expires` / `Max-Age` | 쿠키의 만료 시점 (없으면 브라우저 종료 시 삭제되는 세션 쿠키) |
| `Path` / `Domain` | 쿠키가 전송되는 범위 |

`SameSite=Lax`(기본값에 가까움) 또는 `Strict`는 CSRF 공격(다른 사이트가 사용자 몰래 내 쿠키를 실어 요청을 위조하는 것)을 막는 핵심 방어선입니다.

<br>

## 세션(Session)이란

"세션"이라는 단어는 두 가지 다른 의미로 쓰여서 헷갈리기 쉽습니다.

1. **개념으로서의 세션**: "로그인부터 로그아웃까지 이어지는 하나의 사용자 상태" 라는 추상적 개념
2. **구현 방식으로서의 세션(server-side session)**: 실제 데이터는 서버(메모리/Redis/DB)에 저장하고, 클라이언트 쿠키에는 그 데이터를 찾기 위한 **키(세션 ID)** 만 담는 방식

일반적인 "세션 기반 인증" 흐름:
1. 로그인 성공 → 서버가 랜덤한 세션 ID를 생성하고, `{세션ID: 사용자정보}` 를 서버 저장소에 저장
2. 서버가 `Set-Cookie: sessionId=<랜덤값>` 응답
3. 이후 요청마다 브라우저가 쿠키로 세션 ID를 보내고, 서버는 그 ID로 저장소를 조회해서 "누구인지" 알아냄
4. 로그아웃 → 서버 저장소에서 해당 세션 ID 항목을 삭제 (그 즉시 그 쿠키는 무효가 됨)

이 방식의 특징은 **클라이언트는 무의미한 난수만 들고 있고, 진짜 데이터(사용자 정보, 권한 등)는 항상 서버에 있다**는 것입니다. 그래서 서버가 언제든 특정 세션을 무효화(revoke)할 수 있습니다.

<br>

## ASP.NET Core의 Cookie Authentication은 "그 세션"과 다르다

여기서 이 프로젝트(`UserController.cs`)에 쓰인 `CookieAuthenticationDefaults` 방식이 왜 헷갈리기 쉬운지 설명이 필요합니다.

ASP.NET Core에는 실제로 **두 가지 별개의 기능**이 있습니다.

### 1) `Microsoft.AspNetCore.Session` (진짜 server-side 세션)
- `IDistributedCache` 기반 (메모리, Redis 등)에 key-value를 저장
- 쿠키에는 세션 ID만 담김
- `HttpContext.Session.SetString(...)` / `GetString(...)` 으로 사용
- 로그인/인증 전용이 아니라 범용 서버 사이드 상태 저장소

### 2) `Cookie Authentication` (`Microsoft.AspNetCore.Authentication.Cookies`)
- 이 프로젝트가 쓰는 방식
- **기본 동작은 server-side 저장소를 전혀 쓰지 않습니다.**
- 로그인 시 만든 `ClaimsPrincipal`(사용자 정보 + 클레임)을 **암호화해서 쿠키 값 자체에 통째로 집어넣습니다** (ASP.NET Core Data Protection API로 암호화/서명)
- 즉, 쿠키 안에 "이 쿠키는 유효하며, 이 사람은 Id=3, LoginId=abc다" 라는 정보 자체가 (암호화된 채로) 들어있음
- 서버는 요청이 올 때마다 그 쿠키를 복호화해서 `ClaimsPrincipal`을 복원할 뿐, 별도 저장소를 조회하지 않음

**그래서 이 방식은 "무효화(로그아웃)가 즉시 전역적으로 되지 않는다"는 트레이드오프가 있습니다.** 아래에서 자세히 다룹니다.

<br>

## 코드가 실제로 하는 일 (`UserController.cs` 기준)

```csharp
var claims = new List<Claim>
{
    new(ClaimTypes.NameIdentifier, result.Id.ToString()),
    new(ClaimTypes.Name, result.LoginId),
};
var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(identity));
```

| 용어 | 의미 |
|---|---|
| `Claim` | "이 사용자에 대한 사실 하나" (예: Id는 3이다, 이름은 abc다). key-value 쌍 |
| `ClaimsIdentity` | 여러 `Claim`을 묶은 것 + "어떤 인증 방식으로 검증됐는지" 표시(`AuthenticationScheme`) |
| `ClaimsPrincipal` | `ClaimsIdentity`를 하나 이상 담는 최종 컨테이너. `HttpContext.User`가 이 타입 |
| `AuthenticationScheme` | 인증 방식의 이름표. "Cookies"라는 이름의 인증 방식을 쓰겠다는 선언 |
| `HttpContext.SignInAsync(scheme, principal)` | 1) `principal`을 암호화 2) `Set-Cookie` 헤더로 응답에 실음. **DB에 아무것도 저장하지 않음** |
| `HttpContext.SignOutAsync(scheme)` | 브라우저에게 해당 쿠키를 만료시키라는 `Set-Cookie`(만료된 값)를 응답. 서버 쪽엔 지울 저장소 자체가 없음 |

즉 `SignInAsync`를 호출하는 순간 벌어지는 일은 "DB 세션 테이블에 행을 추가"가 아니라 **"사용자 정보를 암호화해서 쿠키에 봉인하고, 그 쿠키를 응답에 실어 보낸다"** 입니다.

### 왜 그런데도 매 요청마다 로그인 상태를 알 수 있는가
1. 브라우저가 다음 요청부터 `Cookie: .AspNetCore.Cookies=<암호화된값>` 을 자동으로 실어 보냄
2. `app.UseAuthentication()` 미들웨어가 그 쿠키를 복호화해서 `ClaimsPrincipal`을 복원
3. 복원된 값을 `HttpContext.User` 에 채워 넣음
4. 컨트롤러/미들웨어 어디서든 `HttpContext.User.Identity.IsAuthenticated`, `User.FindFirst(ClaimTypes.NameIdentifier)` 등으로 접근 가능
5. `[Authorize]` 어트리뷰트는 이 과정 이후 `HttpContext.User.Identity.IsAuthenticated`가 `true`인지만 확인하는 것

<br>

## 쿠키의 생명주기: 언제까지 로그인이 유지되는가

Cookie Authentication은 서버 저장소가 없으므로, "로그인이 얼마나 유지되는가"는 전적으로 **로그인 시점에 쿠키 안에 박아 넣은 만료 정보 + 브라우저가 그 쿠키를 언제까지 들고 있을지**로 결정됩니다. 서로 다른 두 축이 있어서 헷갈리기 쉽습니다.

### 축 1 — 쿠키 자체가 얼마나 오래 유효한가 (`ExpireTimeSpan`, `SlidingExpiration`)
- `ExpireTimeSpan`: 발급 시점부터 몇 시간/일 뒤에 만료로 처리할지. 이 값 자체도 쿠키 안에 암호화되어 들어있고, 서버는 요청마다 복호화해서 "지금이 그 시각을 지났는가"만 비교함(별도 저장소 조회 없음)
- `SlidingExpiration = true`: 유효기간 안에 요청이 들어올 때마다 만료 시각을 다시 뒤로 밀어서(쿠키를 새로 구워 응답에 실음) 갱신. 즉 계속 쓰는 유저는 사실상 만료가 안 되고, `ExpireTimeSpan` 만큼 활동이 없어야 그때 만료됨

### 축 2 — 브라우저를 껐다 켜도 쿠키가 남아있는가 (`IsPersistent`)
- `SignInAsync` 호출 시 `AuthenticationProperties`를 넘기지 않으면 기본값은 **세션 쿠키(session cookie)**: `ExpireTimeSpan`과 무관하게 **브라우저 프로세스를 완전히 종료하는 순간 브라우저가 알아서 삭제**함. "브라우저 껐다 켜면 재로그인 필요"가 기본 동작.
- 브라우저를 꺼도 유지되게 하려면(흔히 보는 "로그인 상태 유지") 로그인 시 명시적으로 지정해야 함:
```csharp
await HttpContext.SignInAsync(scheme, principal,
    new AuthenticationProperties { IsPersistent = true });
```
이러면 쿠키에 실제 `Expires`/`Max-Age` 속성이 박혀서 디스크에 저장되고, 브라우저를 껐다 켜도 `ExpireTimeSpan`이 지나기 전까지는 그대로 유지됩니다.

### 두 축을 조합하면 생기는 결과

| `IsPersistent` | `ExpireTimeSpan` | `SlidingExpiration` | 결과 |
|---|---|---|---|
| `false`(기본) | 2시간 | `true` | 브라우저 켜둔 채 활동하는 동안만 유지. 브라우저 끄면 즉시 로그아웃 |
| `true` | 14일 | `false` | 브라우저를 껐다 켜도 유지되지만, 발급 후 14일이 지나면 활동 여부와 무관하게 무조건 만료 |
| `true` | 14일 | `true` | 브라우저를 껐다 켜도 유지되고, 14일 안에 한 번이라도 요청을 보내면 만료 시각이 다시 14일 뒤로 밀림 → **꾸준히 쓰는 유저에게는 사실상 영구 로그인처럼 느껴짐** (많은 서비스가 실제로 이 조합을 씀) |

이 마지막 조합이 "매번 로그인 안 해도 되네" 하고 느끼게 되는 서비스들의 전형적인 구현입니다. 다만 **"영구"는 아니고**, `ExpireTimeSpan` 기간만큼 완전히 방문하지 않으면 그때는 만료되어 재로그인이 필요합니다. 또한 탈취된 쿠키도 그만큼 오래 유효하다는 뜻이라 보안 트레이드오프가 있습니다(그래서 민감한 서비스는 짧은 `ExpireTimeSpan` + `SlidingExpiration`을 선호하거나, 별도의 "이 기기 기억하기" 같은 재인증 절차를 추가로 둠).

<br>

## 아직 이 프로젝트에 빠져 있는 것 (Program.cs)

현재 `Program.cs`에는 다음이 없습니다.
- `builder.Services.AddAuthentication(...).AddCookie(...)` — 위 그림의 "Cookies라는 이름의 인증 방식"을 실제로 등록하는 코드가 없으면 `SignInAsync`가 어떤 스킴을 써야 할지 몰라서 런타임 에러가 납니다
- `app.UseAuthentication()` / `app.UseAuthorization()` — 요청이 들어올 때 쿠키를 읽어서 `HttpContext.User`를 채우는 미들웨어. 이게 없으면 로그인 자체는 되어도 이후 요청에서 "내가 로그인했는지"를 서버가 절대 알 수 없음
- `builder.Services.AddScoped<IUserService, UserService>()` — DI 컨테이너 등록 자체가 빠져 있어서 컨트롤러가 뜨지도 않음

이 부분은 직접 구현하며 채워 넣게 되며, `LoginFeatureGuide.md`에서 왜 이 순서와 옵션이 필요한지 다룹니다.

<br>

## 트레이드오프: Server-side Session vs Cookie(자체 포함) vs JWT

| 기준 | Server-side Session (`IDistributedCache`/DB) | Cookie Authentication (이 프로젝트) | JWT (Bearer Token) |
|---|---|---|---|
| 실제 데이터 위치 | 서버 저장소 | 클라이언트 쿠키 (암호화됨) | 클라이언트 (서명만 되어 있고 암호화는 아님, payload는 base64로 누구나 읽을 수 있음) |
| 로그아웃/즉시 무효화 | 쉬움 (저장소에서 삭제하면 끝) | 어려움 — 쿠키를 지워도 탈취된 원본 쿠키는 만료 전까지 여전히 유효. 강제 무효화하려면 `ITicketStore` 등 별도 저장소를 추가해야 함 | 어려움 — 서명 검증만 하므로 만료 전엔 서버가 막을 방법이 기본적으로 없음. 블랙리스트 저장소가 별도로 필요 |
| 서버 확장성 (다중 서버) | 저장소를 서버 간 공유해야 함 (Redis 등). 안 하면 로드밸런서에서 sticky session 필요 | 쿠키 자체가 상태를 담고 있어 서버는 무상태(stateless) — 서버를 몇 대로 늘려도 문제 없음 | 동일하게 무상태 |
| 저장 데이터 크기 | 서버 저장소라 제약 적음 | 매 요청마다 쿠키 전체가 왕복하므로 커지면 네트워크 비용 증가 | 동일 (헤더로 왕복) |
| 브라우저 vs 모바일/외부 API 클라이언트 | 쿠키 기반이라 브라우저에 유리 | 브라우저에 유리, 네이티브 앱/서드파티 API 클라이언트는 쿠키 다루기 번거로움 | 헤더에 담아 어디서든 쓰기 쉬움. 모바일 앱, SPA-분리형 API에 유리 |
| CSRF 노출 | 있음 (쿠키 자동 전송 특성 때문) | 있음, `SameSite`로 완화 | 기본적으로 없음 (쿠키가 아니라 헤더로 수동 전송하므로 자동 첨부 안 됨) |
| XSS 노출 | 쿠키가 `HttpOnly`면 JS로 못 읽음 | 동일 | `localStorage`에 저장하면 JS로 읽혀서 XSS에 취약. `HttpOnly` 쿠키에 넣으면 JWT도 이 문제 없음 |

### 이 프로젝트가 Cookie Authentication을 선택했다는 것의 의미
- 브라우저 기반 클라이언트(같은 사이트에서 서빙되는 프론트엔드)를 우선 고려한 선택
- "즉시 무효화가 안 된다"는 단점은, 쿠키 만료 시간을 짧게 잡거나(`ExpireTimeSpan`), 나중에 `ITicketStore`(예: DB에 활성 세션 테이블)를 추가해서 보완 가능
- 지금 단계에서는 단순함이 우선이므로 기본 동작(무상태, 쿠키에 봉인)을 그대로 쓰는 게 합리적

<br>

## 요약
- 쿠키 = 브라우저가 자동으로 왕복시켜주는 값의 운반체
- 세션이라는 단어는 "서버 저장소 기반 상태 관리"를 가리킬 때가 많지만, ASP.NET Core의 **Cookie Authentication은 서버 저장소를 쓰지 않고 쿠키 자체에 암호화된 사용자 정보를 담는 방식**이라 이름과 달리 "세션 저장소"가 없음
- `SignInAsync`/`SignOutAsync`는 DB나 메모리를 건드리는 게 아니라 **쿠키를 암호화해서 굽고(bake), 지우는(expire) 것**
- `UseAuthentication()`이 매 요청마다 쿠키를 복호화해 `HttpContext.User`를 채우고, `[Authorize]`/`UseAuthorization()`이 그 결과를 검사
- 즉시 로그아웃이 중요하다면 이 구조만으로는 부족하고 서버 측 저장소(세션 테이블, `ITicketStore`)를 추가로 얹어야 함
