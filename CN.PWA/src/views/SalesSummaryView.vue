<script setup lang="ts">
import AnimButton from '@/components/animButton.vue'
import SaleItem from '@/components/saleItem.vue'
import { useSalesStore } from '@/stores/sales'
import { ref } from 'vue'

const saleStore = useSalesStore()
const allDeliveryPending = ref(saleStore.sales.filter((s) => s.status == 'WaitingDelivery').reverse())
const allPaymentPending = ref(saleStore.sales.filter((s) => s.status == 'WaitingPayment').reverse())
const allCompleted = ref(saleStore.sales.filter((s) => s.status == 'Completed').reverse())
</script>

<template>
  <main>
    <h1>Minhas vendas</h1>
    <div v-if="allDeliveryPending.length > 0">
      <h2>Aguardando Entrega</h2>
      <ul class="list">
        <SaleItem v-for="sale in allDeliveryPending" :key="sale.id" :sale="sale" />
      </ul>
    </div>
    <div v-if="allPaymentPending.length > 0">
      <h2>Falta Pagar</h2>
      <ul class="list">
        <SaleItem v-for="sale in allPaymentPending" :key="sale.id" :sale="sale" />
      </ul>
    </div>
    <div v-if="allCompleted.length > 0">
      <h2>Tudo Certo</h2>
      <ul class="list">
        <SaleItem v-for="sale in allCompleted" :key="sale.id" :sale="sale" />
      </ul>
    </div>
  </main>
</template>

<style lang="scss" scoped>
main {
  h2 {
    margin-top: 2em;
    margin-bottom: 1em;
    text-align: left;
  }

  .list {
    display: flex;
    flex-direction: row;
    flex-wrap: wrap;
    list-style: none;
    justify-content: space-around;
  }
}
</style>
