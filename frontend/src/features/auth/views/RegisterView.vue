<template>
  <div class="screen">
    <div class="card">
      <h1>회원가입</h1>

      <div class="field">
        <label>이름</label>
        <input type="text" placeholder="이름을 입력하세요" v-model="userName" />
      </div>

      <div class="field">
        <label>아이디</label>
        <input type="text" placeholder="아이디를 입력하세요" v-model="userId" />
      </div>

      <div class="field">
        <label>비밀번호</label>
        <input
          type="password"
          placeholder="비밀번호를 입력하세요"
          v-model="userPassword"
        />
      </div>

      <BaseButton @click="handleRegister">회원가입</BaseButton>
      <BaseButton variant="text" @click=""
        >이미 계정이 있으신가요? 로그인</BaseButton
      >
    </div>
  </div>
</template>

<script setup>
import { ref } from "vue";
import BaseButton from "../../../design-system/BaseButton.vue";

const userName = ref("");
const userId = ref("");
const userPassword = ref("");

function handleRegister() {}

function handleAlreadyAccount() {}
</script>

<style lang="scss" scoped>
.screen {
  min-height: 100vh;
  display: flex;
  flex-direction: column;
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
