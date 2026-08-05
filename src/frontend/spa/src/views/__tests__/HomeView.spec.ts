import { describe, it, expect, vi } from 'vitest'
import { mount, RouterLinkStub } from '@vue/test-utils'
import { createPinia, setActivePinia } from 'pinia'
import HomeView from '../../views/HomeView.vue'
import { useProductStore } from '../../stores/productStore'

vi.mock('vue-router', () => ({
  useRouter: () => ({ push: vi.fn() }),
}))

describe('HomeView', () => {
  it('renders heading', () => {
    setActivePinia(createPinia())
    const store = useProductStore()
    store.items = [
      { id: 1, name: 'Widget', price: 10, thumbnailUrl: '', categoryName: 'Tools' },
    ]
    store.loading = false
    store.error = null

    const w = mount(HomeView, {
      global: { stubs: { 'router-link': RouterLinkStub } },
    })
    expect(w.text()).toContain('Premium products for modern living')
  })
})
