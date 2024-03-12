<script setup lang="ts">
import { computed } from 'vue'
import router from '@/router'

const version = APP_VERSION
const hash = GIT_HASH
const path = computed(() => router.currentRoute.value.path)
</script>

<template>
  <footer :class="{ pay: path.includes('pay'), deliver: path.includes('deliver') }">
    Copyright github.com/leocb - v{{ version }} - {{ hash }}
  </footer>
</template>

<style lang="scss" scoped>
footer {
  background-color: transparent;
  font-size: small;
  text-align: center;
  padding: 0.5em;
  margin-top: -1px;
}
.pay {
  background-color: $bg-base-payment;
}
.deliver {
  background-color: $bg-base-delivery;
}
</style>
