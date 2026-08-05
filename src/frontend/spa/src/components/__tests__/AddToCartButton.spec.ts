import { describe, it, expect, vi } from 'vitest'
import { mount, RouterLinkStub } from '@vue/test-utils'
import { createPinia, setActivePinia } from 'pinia'
import AddToCartButton from '../AddToCartButton.vue'
import { useCartStore } from '../../stores/cartStore'
import { useAuthStore } from '../../stores/useAuthStore'

const mockPush = vi.fn()
vi.mock('vue-router', () => ({
  useRouter: () => ({ push: mockPush }),
}))

describe('AddToCartButton', () => {
  it('calls addItem on click when authenticated', async () => {
    setActivePinia(createPinia())
    const authStore = useAuthStore()
    authStore.token = 'fake'

    const store = useCartStore()
    store.addItem = vi.fn()

    const w = mount(AddToCartButton, { props: { productId: 42 }, global: { stubs: { 'router-link': RouterLinkStub } } })
    await w.find('button').trigger('click')
    expect(store.addItem).toHaveBeenCalledWith(42, 1, undefined)
  })

  it('redirects to login when anonymous', async () => {
    setActivePinia(createPinia())
    const authStore = useAuthStore()
    authStore.token = null

    const store = useCartStore()
    store.addItem = vi.fn()

    const w = mount(AddToCartButton, { props: { productId: 42 }, global: { stubs: { 'router-link': RouterLinkStub } } })
    await w.find('button').trigger('click')
    expect(mockPush).toHaveBeenCalledWith('/login')
    expect(store.addItem).not.toHaveBeenCalled()
  })
})
