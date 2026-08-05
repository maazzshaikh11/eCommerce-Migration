import { describe, it, expect } from 'vitest'
import { mount, RouterLinkStub } from '@vue/test-utils'
import ProductCard from '../ProductCard.vue'

describe('ProductCard', () => {
  const product = {
    id: 1,
    name: 'Widget',
    price: 19.99,
    thumbnailUrl: '/img/widget.jpg',
    categoryName: 'Tools',
  }

  it('renders name and price', () => {
    const w = mount(ProductCard, {
      props: { product },
      global: { stubs: { 'router-link': RouterLinkStub } },
    })
    expect(w.text()).toContain('Widget')
    expect(w.text()).toContain('₹19.99')
  })

  it('links to product detail', () => {
    const w = mount(ProductCard, {
      props: { product },
      global: { stubs: { 'router-link': RouterLinkStub } },
    })
    const a = w.findComponent(RouterLinkStub)
    expect(a.exists()).toBe(true)
    expect(a.props('to')).toBe('/product/1')
  })
})
