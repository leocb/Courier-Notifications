import { uid } from 'uid'

export class SaleHelper {
  static StatusToText(sale: Sale): string {
    switch (sale.status) {
      case 'New':
        return 'Nova'
      case 'Completed':
        return 'Tudo Certo'
      case 'WaitingDelivery':
        return 'Aguardando Entrega'
      case 'WaitingPayment':
        return 'Aguardando Pagamento'
    }
  }

  static GetNewEmptySale(): Sale {
    return {
      id: uid(),
      createdAt: new Date(),
      status: 'New' as SaleStatus,
      payment: {
        confirmed: false,
        confirmedWhen: null,
        method: 'Cash' as PaymentMethod,
        priceTotal: 0,
        discountTotal: 0,
        paidTotal: 0,
        quantityTotal: 0,
        coupon: null
      },
      cookies: [],
      delivery: {
        confirmed: false,
        confirmedWhen: null,
        expectedBy: null,
        where: ''
      },
      discountRule: {
        DiscountAmount: 0.5,
        EveryQty: 3
      },
      buyer: {
        id: uid(),
        name: ''
      }
    } as Sale
  }

  static AddOnTheHouseCoupon(sale: Sale): void {
    const coupon = {
      name: 'Por conta da casa',
      type: 'Percentage',
      percentage: 1
    } as Coupon

    sale.payment.coupon = coupon
  }

  static CalculateCouponDiscount(sale: Sale): void {
    const payment = sale.payment
    if (payment.confirmed) return

    const coupon = payment.coupon

    switch (coupon?.type) {
      case 'FixedValue':
        payment.discountTotal += coupon.fixedValue ?? 0
        break
      case 'Percentage':
        payment.discountTotal = payment.priceTotal * (coupon.percentage ?? 0)
        break
      default:
        break
    }
  }
}
