import { describe, it, expect } from 'vitest'
import { mount, RouterLinkStub } from '@vue/test-utils'
import { createPinia, setActivePinia } from 'pinia'
import CartView from '../../views/CartView.vue'
import { useCartStore } from '../../stores/cartStore'

describe('CartView', () => {
  it('renders cart items', () => {
    setActivePinia(createPinia())
    const store = useCartStore()
    store.items = [
      { id: 1, productId: 10, productName: 'Widget', unitPrice: 5, quantity: 2, lineTotal: 10 },
    ]
    store.loading = false
    store.error = null

    const w = mount(CartView, {
      global: { stubs: { 'router-link': RouterLinkStub } },
    })
    expect(w.text()).toContain('Widget')
    expect(w.text()).toContain('₹10.00')
  })

  it('shows empty message when no items', () => {
    setActivePinia(createPinia())
    const store = useCartStore()
    store.items = []
    store.loading = false
    store.error = null

    const w = mount(CartView, {
      global: { stubs: { 'router-link': RouterLinkStub } },
    })
    expect(w.text()).toContain('Your cart is empty')
  })
})
