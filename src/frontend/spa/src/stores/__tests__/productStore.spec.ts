import { describe, it, expect, beforeEach } from 'vitest'
import { setActivePinia, createPinia } from 'pinia'
import { useProductStore } from '../../stores/productStore'

describe('productStore', () => {
  beforeEach(() => {
    setActivePinia(createPinia())
  })

  it('initial state', () => {
    const s = useProductStore()
    expect(s.items).toEqual([])
    expect(s.loading).toBe(false)
    expect(s.error).toBeNull()
  })

  it('items can be assigned', () => {
    const s = useProductStore()
    s.items = [{ id: 1, name: 'A', price: 10, thumbnailUrl: '', categoryName: 'C' }]
    expect(s.items).toHaveLength(1)
    expect(s.items[0].name).toBe('A')
  })
})
