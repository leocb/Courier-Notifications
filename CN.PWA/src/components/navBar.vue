<script setup lang="ts">
import { RouterLink } from 'vue-router'
import AnimButton from './animButton.vue'
import router from '@/router'
import SideMenu from './sideMenu.vue'
import { ref } from 'vue'

const showSideMenu = ref(false)
const hideMenu = () => (showSideMenu.value = false)
const showMenu = () => (showSideMenu.value = true)
</script>

<template>
  <header>
    <SideMenu :show="showSideMenu" @close-requested="hideMenu" />
    <nav class="container">
      <RouterLink class="branding" to="" @click="showMenu">
        Logo Here
      </RouterLink>
      <div class="nav-routes">
        <div v-if="$route.name === 'Sales Summary'">
          <RouterLink to="sales/new">
            <AnimButton>Vender</AnimButton>
          </RouterLink>
        </div>
        <div v-else-if="$route.name?.toString().includes('Sale')">
          <RouterLink to="">
            <AnimButton @click="router.back()">Voltar</AnimButton>
          </RouterLink>
        </div>
      </div>
    </nav>
  </header>
</template>

<style lang="scss" scoped>
header {
  nav {
    background-color: $bg-primary;
    align-items: center;

    .branding {
      display: flex;
      padding: 0.3em;
      align-items: center;
      img {
        max-width: 3em;
      }
    }

    a {
      padding: 0.5em;
      font-size: x-large;
      font-weight: 500;
      text-decoration: none;
      color: $text-black;
    }

    .nav-routes {
      padding: 0.5em;
      display: flex;
      flex: 1;
      justify-content: flex-end;
    }
  }
}
</style>
