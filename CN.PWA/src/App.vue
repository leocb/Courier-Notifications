<script setup lang="ts">
import { RouterView } from 'vue-router'
import NavBar from './components/navBar.vue'
import FooterBar from './components/footerBar.vue'
import { useSalesStore } from './stores/sales'
import { onMounted } from 'vue'
const saleStore = useSalesStore()
onMounted(() => {
  saleStore.fetchSales()
})
</script>

<template>
  <NavBar />
  <RouterView />
  <FooterBar />
</template>

<style lang="scss">
@import url('https://fonts.googleapis.com/css2?family=Rubik:wght@300;400;500;700&display=swap');

* {
  font-family: 'Rubik', sans-serif;
  margin: 0;
  padding: 0;
  box-sizing: border-box;
  touch-action: manipulation;
}

input,
textarea,
button,
select,
a {
  -webkit-tap-highlight-color: transparent;
}

html,
body {
  height: 100vh;
  color: $text-black;
}

h1 {
  text-align: center;
}

h2 {
  margin-top: 2em;
  margin-bottom: 1em;
  text-align: left;
}

#app {
  background-color: $bg-base;
  max-width: 1100px;
  display: flex;
  flex-direction: column;
  height: 100%;
  margin: 0 auto;
}

main {
  display: flex;
  flex-direction: column;
  width: 100%;
  margin: 0 auto;
  padding: 2em 1em;
  flex-grow: 1;
}

.container {
  width: 100%;
  display: flex;
}

.line-break {
  width: 100%;
}
.line-break-visible {
  width: 100%;
  height: 0.2em;
  margin: 0.5em 0;
  background-color: $separator;
}
.aleft {
  align-content: flex-start;
  text-align: left;
}
.acenter {
  align-content: center;
  text-align: center;
}
.aright {
  align-content: flex-end;
  text-align: right;
}
.mt {
  margin-top: 1em;
}
.mtdouble {
  margin-top: 2em;
}
.mthalf {
  margin-top: 0.5em;
}
.mb {
  margin-bottom: 1em;
}
.gap1 {
  gap: 1em;
}
.gap1half {
  gap: 0.5em;
}
.gap1quarter {
  gap: 0.25em;
}
</style>
