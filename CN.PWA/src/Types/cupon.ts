type CouponType = 'Percentage' | 'FixedValue'

type Coupon = {
  name: string
  type: CouponType
  percentage: number | null
  fixedValue: number | null
}
