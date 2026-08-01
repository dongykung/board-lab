<template>
  <div class="screen">
    <div class="card">
      <h1>게시판 로그인</h1>

      <div class="field">
        <label>아이디</label>
        <input
          type="text"
          placeholder="아이디를 입력하세요"
          v-model="loginId"
        />
      </div>
      <div class="field">
        <label>비밀번호</label>
        <input
          type="password"
          placeholder="비밀번호를 입력하세요"
          v-model="password"
        />
      </div>

      <BaseButton variant="primary" @click="handleLogin">로그인</BaseButton>
    </div>
  </div>
</template>

<script setup>
import { ref } from "vue";
import BaseButton from "@/design-system/BaseButton.vue";

const loginId = ref("");
const password = ref("");

function handleLogin() {
  console.log("로그인 시도:", loginId.value, password.value);
  // 여기에 로그인 로직을 추가하세요.
}
</script>

<style lang="scss" scoped>
.screen {
  min-height: 100vh;
  display: flex;
  justify-content: center;
  align-items: center;
  background: var(--color-bg-canvas);
}

.card {
  width: 360px;
  display: flex;
  flex-direction: column;
  gap: var(--spacing-lg);
  padding: var(--spacing-xl);
  border-radius: var(--radius-lg);
  background: var(--color-bg-surface);
}

.field {
  display: flex;
  flex-direction: column;
  gap: var(--spacing-xs);

  label {
    font-size: var(--font-size-label);
    color: var(--color-text-secondary);
  }

  input {
    padding: var(--spacing-sm);
    border: 1px solid var(--color-border-default);
    border-radius: var(--radius-sm);
    font-size: var(--font-size-body-medium);
  }
}
</style>
