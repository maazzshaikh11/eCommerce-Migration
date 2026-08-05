import { describe, it, expect } from 'vitest'
import { mount } from '@vue/test-utils'
import { createPinia, setActivePinia } from 'pinia'
import OrderListView from '../../views/OrderListView.vue'
import { useOrderStore } from '../../stores/orderStore'

describe('OrderListView', () => {
  it('renders orders', () => {
    setActivePinia(createPinia())
    const store = useOrderStore()
    store.orders = [
      { id: 1, orderNumber: 'RF-20260101-1234', orderDate: '2026-01-01', orderStatus: 'Paid', subTotal: 10, shippingTotal: 99, taxTotal: 19.62, grandTotal: 128.62 },
    ]
    store.loading = false
    store.error = null

    const w = mount(OrderListView)
    expect(w.text()).toContain('RF-20260101-1234')
    expect(w.text()).toContain('Paid')
  })
})
