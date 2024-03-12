import { defineStore } from 'pinia'

export const useSalesStore = defineStore('sales', {
  state: () => ({
    sales: [] as Sale[]
  }),
  getters: {},
  actions: {
    saveSales(): void {
      localStorage.setItem('sales', JSON.stringify(this.sales))
    },
    fetchSales(): void {
      this.sales = (JSON.parse(localStorage.getItem('sales') ?? '[]') ?? []) as Sale[]
    },
    getSingleSale(id: string): Sale | null {
      return this.sales.find((sale) => sale.id == id) ?? null
    },
    addSale(sale: Sale): void {
      if (this.getSingleSale(sale.id) != null) return

      this.sales.push(sale)
      this.saveSales()
    },
    deleteSale(id: string): void {
      this.sales = this.sales.filter((sale) => sale.id != id)
      this.saveSales()
    }
  }
})
