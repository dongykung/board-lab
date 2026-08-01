<template>
  <button
    type="button"
    class="btn"
    :class="variant"
    :disabled="disabled"
    @click="handleClick"
  >
    <slot>등록</slot>
  </button>
</template>

<script setup>
const props = defineProps({
  variant: {
    type: String,
    default: "primary", // 'primary' or 'secondary'
  },
  disabled: {
    type: Boolean,
    default: false,
  },
});

const emit = defineEmits(["click"]);

function handleClick(event) {
  if (props.disabled) return;
  emit("click", event);
}
</script>

<style lang="scss" scoped>
.btn {
  padding: var(--spacing-sm) var(--spacing-lg);
  border-radius: var(--radius-md);
  font-size: var(--font-size-label-button);
  line-height: var(--line-height-label-button);
  font-weight: var(--font-weight-semibold);
  cursor: pointer;

  &:disabled {
    cursor: not-allowed;
    opacity: 0.5;
  }
}

.primary {
  background: var(--color-brand-default);
  color: var(--color-text-on-brand);
  border: none;

  &:hover:not(:disabled) {
    background: var(--color-brand-hover);
  }
  &:active:not(:disabled) {
    background: var(--color-brand-pressed);
  }
}

.secondary {
  background: var(--color-bg-surface);
  color: var(--color-text-primary);
  border: 1px solid var(--color-border-default);

  &:hover:not(:disabled) {
    background: var(--color-bg-surface-secondary);
  }
}
</style>
