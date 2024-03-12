<script setup lang="ts">
import AnimButton from './animButton.vue'

const emit = defineEmits(['CloseRequested'])
const props = defineProps({
  show: {
    type: Boolean,
    required: true,
    default: true
  }
})
</script>

<template>
  <main class="side-menu" :class="{ hide: !props.show }">
    <div>
      <AnimButton @click="emit('CloseRequested')"> Fechar </AnimButton>
    </div>
  </main>
  <div class="background" :class="{ hide: !props.show }" />
</template>

<style lang="scss" scoped>
.side-menu {
  background-color: $bg-primary;
  position: absolute;
  z-index: 1;
  left: 0;
  height: 100vh;
  width: 80vw;
  box-shadow: 0em 0em 2em 0em $btn-shadow;
  transition: 0.4s ease-out;

  &.hide {
    box-shadow: none;
    translate: -100vw;
  }
}
.background {
  visibility: visible;
  background: $bg-black-overlay;
  backdrop-filter: blur(30px);
  opacity: 1;
  position: absolute;
  height: 100vh;
  width: 100vw;
  transition: 0.3s;

  &.hide {
    opacity: 0;
    visibility: hidden;
  }
}
</style>
