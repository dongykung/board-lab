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

      <BaseButton variant="primary" @click="handleLogin" :disabled="loading">
        <span v-if="loading" class="spinner" aria-hidden="true"></span>
        {{ loading ? "로그인 중..." : "로그인" }}
      </BaseButton>
      {{ error }}
      <BaseButton variant="text" :disabled="loading" @click="goToRegister">계정이 없으신가요? 회원가입</BaseButton>
    </div>
  </div>
</template>

<script setup>
import { ref } from "vue";
import BaseButton from "@/design-system/BaseButton.vue";
import { userAuth } from "../composable/userAuth";
import { useRouter } from "vue-router";

const router = useRouter();
const { login, loading, error } = userAuth();
const loginId = ref("");
const password = ref("");

async function handleLogin() {
  if (!loginId.value || !password.value) {
    alert('Id와 비밀번호 모두 입력해 주세요.');
    return;
  }
  const result = await login(loginId.value, password.value);
  if (!result) return;
}

function goToRegister() {
  router.push({ name: "register" });
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

.spinner {
  width: 14px;
  height: 14px;
  border: 2px solid currentColor;
  border-top-color: transparent;
  border-radius: 50%;
  animation: spin 0.6s linear infinite;
}
@keyframes spin {
  to { transform: rotate(360deg); }
}
</style>
