<script setup lang="ts">
import { FxSounds } from '@/modules/Sounds/FxSounds'
import { ref } from 'vue'

const props = defineProps({
  default: Number,
  disabled: {
    type: Boolean,
    required: false,
    default: false
  }
})

const value = ref(props.default)
</script>

<template>
  <input
    type="number"
    v-model="value"
    :disabled="disabled"
    min="0"
    @focusin="FxSounds.PlayWhoosh()"
    @focusout="FxSounds.PlayWhoosh()"
  />
</template>

<style lang="scss" scoped>
input {
  box-sizing: border-box;
  background-color: $bg-secondary;
  color: $text-black;
  cursor: pointer;
  padding: 0.5em 1em;
  height: 3em;
  font-size: large;
  font-weight: bold;
  border-radius: 1em;
  box-shadow:
    0 0.5em $btn-shadow,
    inset 0 0 0 0em $action;
  border: 0em $action solid;
  transition: all 100ms ease-in;

  &:focus-within {
    box-shadow:
      0 0.5em $btn-shadow,
      inset 0 0 0 0.4em $action;
    outline: none;
  }

  //TODO: Add $ symbol
  .symbol {
    ::before {
      content: '$';
      padding: 0 5px;
      text-align: center;
    }
  }
}
</style>
