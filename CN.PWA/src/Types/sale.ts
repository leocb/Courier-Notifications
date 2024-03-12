type SaleStatus = 'New' | 'WaitingDelivery' | 'WaitingPayment' | 'Completed'

type Sale = {
  id: string
  status: SaleStatus
  createdAt: Date
  buyer: Buyer
  payment: Payment
  delivery: Delivery
  cookies: Cookie[]
  discountRule: DiscountRule | null
}
