<script setup lang="ts">
const props = defineProps({
  name: { type: String },
  value: { type: String },
  isSub: { type: Boolean }
})
</script>

<template v-slot:name:value="slotProps">
  <div class="line-break" />
  <span :class="{ sub: props.isSub }">{{ props.name }}</span>
  <span :class="{ sub: props.isSub }">{{ props.value }}</span>
</template>

<style lang="scss" scoped>
.sub {
  color: $text-black-faded;
  font-weight: normal;
  font-size: 1.1em;
  margin-left: 1.5em;
}
span {
  font-size: 1.2em;
  font-weight: 500;
}
</style>
