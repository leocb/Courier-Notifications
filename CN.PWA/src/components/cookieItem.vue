<script setup lang="ts">
import { type PropType } from 'vue'

const props = defineProps({
  cookie: {
    type: Object as PropType<Cookie>,
    required: true
  }
})
</script>

<template>
  <div class="main">
    <h3>{{ cookie.name }}</h3>
    <h2>{{ cookie.quantity }}</h2>
  </div>
</template>

<style lang="scss" scoped>
.main {
  margin-bottom: 0.5em;
  width: 100%;
  display: flex-block;
  flex-flow: row wrap;
  align-items: flex-start;
  text-align: left;
  flex-basis: auto;
  justify-content: space-between;
  padding: 1em;
}
</style>
