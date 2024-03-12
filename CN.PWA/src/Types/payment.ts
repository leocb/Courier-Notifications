type PaymentMethod = 'Cash' | 'Pix' | 'Credit' | 'Debit'

type Payment = {
  priceTotal: number
  quantityTotal: number
  discountTotal: number
  paidTotal: number
  coupon: Coupon | null
  method: PaymentMethod
  confirmed: boolean
  confirmedWhen: Date | null
}
