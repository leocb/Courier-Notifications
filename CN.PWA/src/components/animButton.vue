<script setup lang="ts">
import { ref } from 'vue'
import { KeySounds } from '../modules/Sounds/KeySounds'

const pressed = ref(false)
const pressButton = () => {
  pressed.value = true
  KeySounds.PlayRandomKeyDown()
}
const releaseButton = () => {
  pressed.value = false
  KeySounds.PlayRandomKeyUp()
}
</script>

<template>
  <button
    @pointerdown="pressButton"
    @pointerup="releaseButton"
    @pointercancel="releaseButton"
    class="btn"
    :class="{
      'btn-pressed': pressed
    }"
  >
    <slot> button </slot>
  </button>
</template>

<style lang="scss" scoped>
.btn {
  padding: 0.9em 1em 1.1em 1em;
  font-weight: bold;
  font-size: large;
  box-shadow: inset 0 -0.4em $btn-shadow;
  border-radius: 0.6em;
  border: 0.15em solid $btn-border;
  flex-basis: 0;
  cursor: pointer;

  background-color: $action;
  color: $text-black;
  flex-grow: 1;

  &.btn-pressed {
    margin-top: 0.2em;
    padding-top: 0.9em;
    padding-bottom: 0.9em;
    box-shadow: inset 0 0em $btn-shadow;
  }

  &.primary {
    background-color: $primary;
    flex-grow: 6;
    color: $text-white;
  }
  &.secondary {
    background-color: $secondary;
    flex-grow: 2;
    color: $text-white;
  }
  &.noSidePadding {
    padding-left: 0;
    padding-right: 0;
  }
  &.big {
    width: 100%;
    font-size: x-large;
  }
}
</style>
