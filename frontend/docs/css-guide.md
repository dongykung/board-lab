# CSS 학습 가이드

HTML 태그에 실제로 적용하면서 보는 참고 문서. 각 항목은 "어떤 태그/상황에 쓰는지" 위주로 정리.

---

## 1. CSS를 적용하는 3가지 방법

```html
<!-- 1) 인라인: 태그에 직접, 우선순위 가장 높음. 실무에서는 지양 -->
<div style="color: red;">텍스트</div>

<!-- 2) 내부 스타일시트: head 안에 style 태그 -->
<style>
  div { color: red; }
</style>

<!-- 3) 외부 스타일시트: 실무 표준, Vue는 컴포넌트 내 <style scoped> 도 이 방식 -->
<link rel="stylesheet" href="style.css">
```

Vue SFC에서는 보통 `<style scoped>`를 써서 해당 컴포넌트에만 스타일이 적용되게 함 (안드로이드 Compose의 Modifier가 컴포저블 단위로 스코프되는 것과 비슷한 감각).

**중요:** `class="card"`처럼 클래스 이름을 붙이는 것 자체는 아무 스타일도 주지 않습니다. 브라우저나 프레임워크가 원래 알고 있는 이름이 아니라 그냥 임의의 문자열이에요. `<style>` 안에 그 이름을 선택자로 써서 실제 규칙을 정의해야 스타일이 적용됩니다.

---

## 2. 선택자(Selector) 종류

| 선택자 | 문법 | 설명 |
|---|---|---|
| 태그 선택자 | `p { }` | 모든 `<p>` |
| 클래스 선택자 | `.card { }` | `class="card"` 가진 모든 요소 |
| id 선택자 | `#header { }` | `id="header"` 요소 (한 페이지에 유일해야 함) |
| 전체 선택자 | `* { }` | 모든 요소 |
| 자손 선택자 | `.card p { }` | `.card` 내부 어디든 있는 `p` |
| 자식 선택자 | `.card > p { }` | `.card`의 직계 자식인 `p`만 |
| 인접 형제 | `h1 + p { }` | `h1` 바로 다음의 `p` |
| 일반 형제 | `h1 ~ p { }` | `h1` 뒤에 오는 모든 형제 `p` |
| 속성 선택자 | `input[type="text"] { }` | 특정 속성/값을 가진 요소 |
| 그룹 선택자 | `h1, h2, p { }` | 콤마로 묶어서 동시 적용 |

### 가상 클래스 (Pseudo-class) — 상태 기반
```css
a:hover { }        /* 마우스 올렸을 때 */
button:active { }  /* 클릭하는 순간 */
input:focus { }     /* 포커스됐을 때 */
input:disabled { }  /* 비활성화 상태 */
li:first-child { }  /* 부모의 첫 자식 */
li:last-child { }   /* 부모의 마지막 자식 */
li:nth-child(2) { }     /* 부모의 2번째 자식 */
li:nth-child(odd) { }   /* 홀수 번째 */
li:nth-child(even) { }  /* 짝수 번째 */
tr:not(.header) { }     /* 조건 제외 */
```
`:hover:not(:disabled)` 처럼 이어붙이면 AND 조건입니다 — "hover 상태 이면서 disabled가 아닐 때"만 적용.

### 가상 요소 (Pseudo-element) — 요소의 일부분
```css
p::first-line { }   /* 첫 줄만 */
p::first-letter { } /* 첫 글자만 */
.icon::before { content: "★"; }  /* 요소 앞에 콘텐츠 삽입 */
.icon::after { content: ""; }    /* 요소 뒤에 콘텐츠 삽입 (장식용으로 많이 씀) */
```

**우선순위(specificity)**: 인라인 > id > 클래스/속성/가상클래스 > 태그. 같은 우선순위면 나중에 선언된 게 이김.

---

## 3. Box Model (핵심 개념)

모든 HTML 요소는 이 4겹 박스로 그려짐:

```
margin (요소 바깥 여백, 배경색 없음)
  border (테두리)
    padding (테두리 안쪽, 콘텐츠와의 여백, 배경색 적용됨)
      content (실제 텍스트/이미지)
```

```css
div {
  width: 200px;
  height: 100px;
  padding: 16px;
  border: 1px solid #ccc;
  margin: 8px;
  box-sizing: border-box; /* width/height에 padding+border 포함시킴. 거의 항상 이거 씀 */
}
```

`box-sizing: border-box`를 안 쓰면 `width: 200px` + `padding: 16px`일 때 실제 렌더링 너비가 232px가 되는 함정이 있음. 보통 전역으로 이렇게 리셋:
```css
* { box-sizing: border-box; }
```

### margin/padding 축약형
```css
margin: 10px;                 /* 4방향 동일 */
margin: 10px 20px;            /* 상하 10px, 좌우 20px */
margin: 10px 20px 30px;       /* 상 10, 좌우 20, 하 30 */
margin: 10px 20px 30px 40px;  /* 상 우 하 좌 (시계방향) */
margin: 0 auto;                /* 좌우 auto → 가로 중앙 정렬 (요소에 width 필요) */
```

---

## 4. display 종류 (레이아웃의 시작점)

| 값 | 특징 | 대표 태그 |
|---|---|---|
| `block` | 한 줄 전체 차지, width/height 지정 가능 | `div`, `p`, `h1`, `ul`, `li` |
| `inline` | 내용 크기만큼만 차지, width/height 무시됨 | `span`, `a`, `strong`, `img` |
| `inline-block` | inline처럼 옆으로 나열되지만 width/height 지정 가능 | 버튼형 링크 등에 자주 씀 |
| `none` | 렌더링 자체를 안 함 (공간도 안 차지) | 토글/조건부 표시 |
| `flex` | 1차원(가로 또는 세로) 배치 컨테이너 | 네비바, 카드 내부 정렬 |
| `grid` | 2차원(행+열) 배치 컨테이너 | 전체 페이지 레이아웃, 갤러리 |

`visibility: hidden`은 `display: none`과 다름 — 안 보이지만 공간은 그대로 차지함.

---

## 5. Flexbox (가장 많이 씀 — 필수)

컨테이너(부모)에 선언:
```css
.container {
  display: flex;
  flex-direction: row;        /* row(기본, 가로) | column(세로) | row-reverse | column-reverse */
  justify-content: center;    /* 주축 정렬: flex-start | center | flex-end | space-between | space-around | space-evenly */
  align-items: center;        /* 교차축 정렬: flex-start | center | flex-end | stretch(기본) */
  flex-wrap: wrap;            /* 넘칠 때 줄바꿈 허용 여부: nowrap(기본) | wrap */
  gap: 12px;                  /* 아이템 사이 간격 (margin 안 써도 됨) */
}
```

자식(아이템)에 선언:
```css
.item {
  flex: 1;           /* 남은 공간을 비율대로 차지 (flex-grow 축약) */
  flex-shrink: 0;     /* 공간 부족해도 줄어들지 않게 */
  align-self: flex-end; /* 이 아이템만 교차축 정렬 개별 override */
}
```

**자주 쓰는 패턴**: 가로 중앙+세로 중앙 정렬
```css
.center {
  display: flex;
  justify-content: center;
  align-items: center;
}
```

---

## 6. Grid (2차원 레이아웃)

```css
.container {
  display: grid;
  grid-template-columns: 1fr 1fr 1fr;   /* 3등분 컬럼 */
  grid-template-columns: repeat(3, 1fr); /* 위와 동일, 반복 표기 */
  grid-template-columns: 200px 1fr;      /* 사이드바(고정) + 본문(가변) */
  grid-template-rows: auto 1fr auto;     /* header, content, footer */
  gap: 16px;
}
.item {
  grid-column: 1 / 3;  /* 1번 선~3번 선까지 (2칸 차지) */
  grid-row: span 2;     /* 세로로 2칸 차지 */
}
```

Flexbox는 "한 줄/한 칸 정렬", Grid는 "페이지 전체 뼈대"에 주로 씀. 게시판이면 `grid-template-columns: 240px 1fr`로 사이드바+본문 나누는 식.

---

## 7. `col-5`, `p-5` 같은 유틸리티 클래스는 뭐야?

Bootstrap, Tailwind 같은 CSS 프레임워크를 쓰면 `col-5`, `p-5` 같은 클래스명을 자주 보게 되는데, **이것도 4번에서 배운 `display`/`flex`/`grid`를 그대로 쓰는, 그냥 "미리 이름 지어둔 CSS 클래스 모음"**입니다. 특별한 문법이 아니라, 프레임워크 CSS 파일 안에 이미 `.col-5 { }`, `.p-5 { }` 같은 규칙이 정의되어 있고, 그걸 가져다 쓰는 것뿐이에요.

### Bootstrap 방식 — Grid 시스템 (`col-*`)
Bootstrap은 한 행을 **12칸**으로 나눠놓고, 그 중 몇 칸을 차지할지를 클래스명 숫자로 지정합니다.
```html
<div class="row">
  <div class="col-5">5칸짜리</div>
  <div class="col-7">7칸짜리</div>  <!-- 5+7=12, 한 줄 꽉 채움 -->
</div>
```
내부적으로는 `.row`가 `display: flex`(또는 grid)이고, `.col-5`는 `flex: 0 0 41.666%` (12칸 중 5칸 = 41.666%) 같은 계산된 값을 미리 CSS로 박아둔 것뿐입니다. 즉 여러분이 5-6번에서 배운 Flexbox/Grid를 프레임워크가 대신 계산해서 클래스로 포장해준 것.

### Tailwind 방식 — Utility-first (`p-5`, `m-4`, `flex`, `gap-2`...)
Tailwind는 속성 하나당 클래스 하나를 대응시킵니다. 이름의 숫자는 대부분 `0.25rem` 단위입니다.
```html
<div class="flex justify-center items-center p-5 gap-2 rounded-md">
  <button class="px-4 py-2 bg-indigo-600 text-white">버튼</button>
</div>
```
| 클래스 | 실제 CSS |
|---|---|
| `flex` | `display: flex;` |
| `justify-center` | `justify-content: center;` |
| `items-center` | `align-items: center;` |
| `p-5` | `padding: 1.25rem;` (5 × 0.25rem) |
| `px-4` | `padding-left: 1rem; padding-right: 1rem;` |
| `py-2` | `padding-top: 0.5rem; padding-bottom: 0.5rem;` |
| `m-4` | `margin: 1rem;` |
| `gap-2` | `gap: 0.5rem;` |
| `rounded-md` | `border-radius: 0.375rem;` |
| `w-full` | `width: 100%;` |

즉 `class="p-5"`라고 쓰는 순간 "`padding: 1.25rem`을 직접 타이핑한 것"과 결과가 완전히 같습니다. `<style>` 블록에 따로 CSS를 안 써도 되는 대신, HTML 태그가 클래스로 뒤덮이는 트레이드오프가 있어요.

**지금 이 프로젝트에는 Bootstrap도 Tailwind도 설치 안 되어 있습니다.** 그래서 `class="col-5"`나 `class="p-5"`라고 써도 진짜 아무 일도 안 일어나요 (1번에서 말한 것과 같은 이유 — 그 이름에 대응하는 CSS 규칙이 어디에도 없으니까). 지금처럼 `<style scoped>`에 직접 `.card { padding: var(--spacing-xl); }` 식으로 쓰는 게 "프레임워크 없이 순수 CSS로 하는 방식"이고, 나중에 컴포넌트가 많아지고 반복 작업이 지겨워질 때 Tailwind 같은 걸 설치해서 넘어가는 흐름이 일반적입니다. 지금 순수 CSS부터 배우는 게 원리 이해에는 훨씬 도움이 됩니다.

---

## 8. Position (요소를 흐름에서 빼내기)

```css
.el {
  position: static;    /* 기본값, top/left 등 무시됨 */
  position: relative;  /* 원래 자리 기준으로 이동, 자리 자체는 보존 */
  position: absolute;  /* 가장 가까운 relative(또는 absolute) 조상 기준으로 위치, 자리는 사라짐 */
  position: fixed;      /* 뷰포트(화면) 기준 고정, 스크롤해도 안 움직임 */
  position: sticky;     /* 스크롤하다 특정 지점에서 fixed처럼 붙음 (헤더에 자주 씀) */
  top: 0; left: 0; right: 0; bottom: 0;
  z-index: 10;          /* 겹쳤을 때 위/아래 순서, 숫자 클수록 위 */
}
```

**패턴**: 모달/뱃지는 보통 부모에 `position: relative`, 자식에 `position: absolute`.

---

## 9. 크기 지정 단위

| 단위 | 의미 | 언제 쓰나 |
|---|---|---|
| `px` | 절대 픽셀 | 테두리, 그림자처럼 고정돼야 할 값 |
| `%` | 부모 대비 비율 | 반응형 너비 |
| `rem` | 루트(`html`) font-size 기준 배수 | 폰트 크기, 여백 — 접근성 고려해 권장 (Tailwind 숫자 클래스가 이 단위 기준) |
| `em` | 부모 font-size 기준 배수 | 컴포넌트 내부 상대 크기 |
| `vw` / `vh` | 뷰포트 너비/높이의 1% | 전체 화면 채우는 레이아웃 |
| `fr` | grid에서 남은 공간의 비율 | grid-template-columns 전용 |

```css
html { font-size: 16px; } /* 1rem = 16px 기준 */
.title { font-size: 1.5rem; } /* = 24px, 사용자가 브라우저 폰트 설정 바꾸면 같이 커짐 */
```

### min/max 조합
```css
width: min(90%, 600px);  /* 둘 중 작은 값 → 반응형+최대폭 동시 제어 */
width: clamp(200px, 50%, 500px); /* 최소 200px ~ 최대 500px, 기본은 50% */
```

---

## 10. 텍스트 / 폰트

```css
p {
  font-family: 'Pretendard', sans-serif; /* 폰트 목록, 앞에서부터 우선 적용 */
  font-size: 16px;
  font-weight: 400;   /* 100~900, normal(400) | bold(700) */
  line-height: 1.5;    /* 줄 간격, 숫자만 쓰면 font-size 배수 */
  text-align: center;  /* left | center | right | justify */
  text-decoration: underline; /* none | underline | line-through */
  letter-spacing: 0.02em;
  color: #333;
  white-space: nowrap; /* 줄바꿈 방지 */
  overflow: hidden;
  text-overflow: ellipsis; /* 넘치면 ... 처리 (white-space, overflow와 세트로 씀) */
}
```

---

## 11. 색상 표현 방식

```css
color: red;                     /* 키워드 */
color: #ff0000;                 /* hex */
color: #f00;                    /* hex 축약 */
color: rgb(255, 0, 0);          /* rgb */
color: rgba(255, 0, 0, 0.5);    /* rgb + 투명도(0~1) */
color: hsl(0, 100%, 50%);       /* 색상각, 채도, 명도 — 톤 조절하기 편함 */
opacity: 0.5;                    /* 요소 전체 투명도 (자식까지 다 옅어짐, rgba와 차이) */
```

---

## 12. Border / Radius / Shadow

```css
.card {
  border: 1px solid #ddd;         /* 두께 스타일 색상 */
  border-radius: 8px;              /* 모서리 둥글게 */
  border-radius: 50%;              /* 원형 (정사각형 요소에 적용 시) */
  box-shadow: 0 2px 8px rgba(0,0,0,0.1); /* x오프셋 y오프셋 blur 색상 */
}
```

---

## 13. 자주 쓰는 조합 패턴

### 카드
```css
.card {
  display: flex;
  flex-direction: column;
  padding: 16px;
  border-radius: 12px;
  box-shadow: 0 2px 8px rgba(0,0,0,0.08);
  gap: 8px;
}
```

### 화면 전체 중앙 정렬 (로그인 폼 등)
```css
.screen {
  min-height: 100vh;
  display: flex;
  justify-content: center;
  align-items: center;
}
```

### 네비게이션 바 (좌: 로고, 우: 메뉴)
```css
.navbar {
  display: flex;
  justify-content: space-between;
  align-items: center;
  padding: 12px 24px;
}
```

### 리스트 아이템 구분선
```css
.list-item + .list-item {
  border-top: 1px solid #eee; /* 인접 형제 선택자로 첫 아이템 위엔 선 안 그림 */
}
```

---

## 14. CSS 변수 (Custom Properties)

```css
:root {
  --color-primary: #4f46e5;
  --spacing-md: 16px;
}
.button {
  background-color: var(--color-primary);
  padding: var(--spacing-md);
}
```
디자인시스템의 `design-system/tokens/*.css`가 이 방식입니다. 색상/간격 토큰을 한 곳에 모아두면 나중에 한 곳만 바꿔서 전체 톤 조정 가능. 안드로이드의 `Color.kt`, `Dimens.kt` 같은 역할이자, Bootstrap/Tailwind 없이도 "재사용 가능한 값 이름"을 직접 만드는 방법이에요.

---

## 15. 반응형 (Media Query)

```css
/* 모바일 우선(mobile-first) 방식 권장: 기본은 모바일, min-width로 확장 */
.container {
  padding: 8px;
}
@media (min-width: 768px) {
  .container { padding: 24px; }
}
@media (min-width: 1024px) {
  .container { padding: 40px; }
}
```

---

## 16. Transition / Animation 기본

```css
.button {
  transition: background-color 0.2s ease, transform 0.2s ease;
}
.button:hover {
  background-color: #333;
  transform: translateY(-2px);
}
```
`transition`은 상태 변화(hover, class 토글)에 자동으로 붙는 애니메이션. `transform: translateX/Y/scale/rotate`는 레이아웃 재계산 없이 움직여서 성능이 좋음 (`top`/`left` 대신 애니메이션엔 이걸 우선 고려).

---

## 17. Vue SFC에서 참고할 점

```vue
<style scoped>
/* 이 컴포넌트에서만 적용됨. 자식 컴포넌트의 루트 요소까지는 :deep()으로 뚫어야 함 */
.card :deep(.child-class) { color: red; }
</style>
```
- `design-system/` 컴포넌트는 `class` prop을 받아서 조합 가능하게 열어두는 걸 고려.
- 반복되는 값(색상, 간격, radius)은 처음부터 CSS 변수로 빼두면 나중에 컴포넌트 늘어나도 관리 편함.

---

## 학습 순서 추천
1. Box model + display (block/inline/inline-block)
2. Flexbox — 대부분의 레이아웃은 이것만으로 해결됨
3. Position (relative/absolute 조합)
4. Grid — 페이지 전체 뼈대 잡을 때
5. 반응형(media query) + 단위(rem/vw)
6. 나머지(transition, 가상클래스, col-*/p-* 유틸리티 클래스 원리)는 필요할 때 그때그때 찾아서 적용
