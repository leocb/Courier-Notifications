<script setup lang="ts">
import { type PropType } from 'vue'
import { Icon } from '@iconify/vue'
import AnimButton from './animButton.vue'

const emit = defineEmits(['moreCookie', 'lessCookie'])
const props = defineProps({
  cookie: {
    type: Object as PropType<Cookie>,
    required: true
  }
})

const moreCookie = () => {
  emit('moreCookie', props.cookie.quantity)
}
const lessCookie = () => {
  emit('lessCookie', props.cookie.quantity)
}
</script>

<template>
  <li class="mb">
    <h3>{{ cookie.name }}</h3>
    <h2>{{ cookie.quantity }}</h2>
    <AnimButton @click="lessCookie" class="noSidePadding">
      <Icon icon="typcn:minus" width="32" />
    </AnimButton>
    <AnimButton @click="moreCookie" class="noSidePadding">
      <Icon icon="typcn:plus" width="32" />
    </AnimButton>
  </li>
</template>

<style lang="scss" scoped>
svg {
  display: inline-block;
}

li {
  width: 31%;
  display: flex;
  flex-flow: row;
  flex-wrap: wrap;
  align-items: center;
  text-align: center;
  justify-content: space-between;
  padding: 0.5em;
  background-color: $bg-secondary;
  border-radius: 1em;
  box-shadow: 0 0.5em $btn-shadow;

  h2 {
    margin: 0.5em;
    padding: 0.5em;
    width: 100%;
    background-color: #00000010;
    border-radius: 0.5em;
    box-shadow: 0 -0.15em $btn-shadow;
    text-align: center;
  }

  h3 {
    margin: auto;
    text-align: center;
  }

  .btn {
    width: 46%;
    font-size: large;
    margin: 0 0.2em;
  }

  svg {
    display: block;
    margin: auto;
  }
}
</style>
