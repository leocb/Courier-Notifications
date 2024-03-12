<script setup lang="ts">
import { FxSounds } from '@/modules/Sounds/FxSounds'
import { ref } from 'vue'

const value = ref('')

defineProps({
  disabled: {
    type: Boolean,
    required: false,
    default: false
  }
})
</script>

<template>
  <input
    type="datetime-local"
    v-model="value"
    :disabled="disabled"
    @focusin="FxSounds.PlayWhoosh()"
    @focusout="FxSounds.PlayWhoosh()"
  />
</template>

<style lang="scss" scoped>
input {
  box-sizing: border-box;
  color: $text-black;
  cursor: pointer;
  padding: 0.5em 1em;
  height: 3em;
  font-size: large;
  font-weight: bold;
  background-color: $bg-secondary;
  border-radius: 1em;
  box-shadow:
    0 0.5em $btn-shadow,
    inset 0 0 0 0em $action;
  border: 0em $action solid;
  transition: all 100ms ease-in;

  &:focus {
    box-shadow:
      0 0.5em $btn-shadow,
      inset 0 0 0 0.4em $action;
    outline: none;
  }
}
</style>
