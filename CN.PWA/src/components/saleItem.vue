<script setup lang="ts">
import type { PropType } from 'vue'
import AnimButton from './animButton.vue'
import router from '@/router'
import { DateHelper } from '@/modules/DateHelper'

defineProps({
  sale: {
    type: Object as PropType<Sale>,
    required: true
  }
})
</script>

<template>
  <AnimButton
    class="main"
    @click="router.push('/sales/' + sale.id)"
    :class="{ primary: sale.status == 'Completed', secondary: sale.status == 'WaitingPayment' }"
  >
    <div
      class="info gap1quarter"
      :class="{ 'use-white': sale.status == 'WaitingPayment' || sale.status == 'Completed' }"
    >
      <h2 class="aleft">$ {{ (sale.payment.priceTotal - sale.payment.discountTotal)?.toFixed(2) }}</h2>
      <h5 class="aright faded weight-normal">
        {{ DateHelper.ToDateTimeString(sale.createdAt) }}
      </h5>
      <div class="line-break" />
      <h3 class="weight-normal">{{ sale.buyer.name }}</h3>
      <div class="line-break" />
      <h4 v-if="sale.status == 'WaitingDelivery'" class="weight-normal faded">
        {{ sale.delivery.where }}
      </h4>
    </div>
  </AnimButton>
</template>

<style lang="scss" scoped>
h2,
h3,
h4,
h5 {
  flex-grow: 1;
  margin: unset;
}

.use-white {
  color: $text-white;
  .faded {
    color: $text-white-faded;
  }
}

.faded {
  color: $text-black-faded;
}

.weight-normal {
  font-weight: normal;
}
.main {
  margin-bottom: 0.5em;
  width: 100%;
  flex-flow: row nowrap;
  align-items: flex-start;
  text-align: left;
  flex-basis: auto;
  justify-content: space-between;
}
.info {
  display: flex;
  flex-flow: row wrap;
  width: 100%;
}
</style>
