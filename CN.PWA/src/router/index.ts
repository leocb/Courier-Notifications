import { createRouter, createWebHistory } from 'vue-router'
import SalesSummaryView from '@/views/SalesSummaryView.vue'

const router = createRouter({
  history: createWebHistory(import.meta.env.BASE_URL),
  routes: [
    {
      path: '/',
      name: 'Home',
      component: SalesSummaryView
    }
  ]
})

export default router
