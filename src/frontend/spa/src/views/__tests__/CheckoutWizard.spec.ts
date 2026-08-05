import { describe, it, expect } from 'vitest'
import { mount, RouterLinkStub } from '@vue/test-utils'
import { createPinia, setActivePinia } from 'pinia'
import CheckoutWizard from '../../views/CheckoutWizard.vue'
import { useCheckoutStore } from '../../stores/checkoutStore'

describe('CheckoutWizard', () => {
  it('renders checkout heading', () => {
    setActivePinia(createPinia())
    const store = useCheckoutStore()
    store.addresses = []
    store.loading = false
    store.error = null
    store.confirmation = null

    const w = mount(CheckoutWizard, {
      global: { stubs: { 'router-link': RouterLinkStub } },
    })
    expect(w.text()).toContain('Select Shipping Address')
  })
})
