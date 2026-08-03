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
    default: "primary", // 'primary', 'secondary', or 'text'
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
  cursor: pointer;

  &:disabled {
    cursor: not-allowed;
    opacity: 0.5;
  }
}

.primary {
  padding: var(--spacing-sm) var(--spacing-lg);
  border-radius: var(--radius-md);
  font-size: var(--font-size-label-button);
  line-height: var(--line-height-label-button);
  font-weight: var(--font-weight-semibold);
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
  padding: var(--spacing-sm) var(--spacing-lg);
  border-radius: var(--radius-md);
  font-size: var(--font-size-label-button);
  line-height: var(--line-height-label-button);
  font-weight: var(--font-weight-semibold);
  background: var(--color-bg-surface);
  color: var(--color-text-primary);
  border: 1px solid var(--color-border-default);

  &:hover:not(:disabled) {
    background: var(--color-bg-surface-secondary);
  }
}

.text {
  padding: 0;
  border: none;
  background: none;
  text-align: center;
  font-size: var(--font-size-body-small);
  line-height: var(--line-height-body-small);
  font-weight: var(--font-weight-medium);
  color: var(--color-brand-default);

  &:hover:not(:disabled) {
    color: var(--color-brand-hover);
  }
}

</style>
