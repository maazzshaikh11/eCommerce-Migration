import { ref, computed } from 'vue'
import { defineStore } from 'pinia'
import { get, post, put, del } from '../api/client'
import type { CartItemDto } from '../types'

export const useCartStore = defineStore('cart', () => {
  const items = ref<CartItemDto[]>([])
  const loading = ref(false)
  const error = ref<string | null>(null)

  const itemCount = computed(() => items.value.reduce((sum, i) => sum + i.quantity, 0))
  const total = computed(() => items.value.reduce((sum, i) => sum + i.lineTotal, 0))

  async function fetchCart() {
    loading.value = true
    error.value = null
    try {
      items.value = await get<CartItemDto[]>('/cart')
    } catch (e) {
      error.value = e instanceof Error ? e.message : String(e)
    } finally {
      loading.value = false
    }
  }

  async function addItem(productId: number, quantity = 1, variantId?: number) {
    error.value = null
    try {
      await post('/cart/items', { productId, quantity, variantId })
      await fetchCart()
    } catch (e) {
      error.value = e instanceof Error ? e.message : String(e)
      throw e
    }
  }

  async function updateQuantity(cartItemId: number, quantity: number) {
    error.value = null
    try {
      await put(`/cart/items/${cartItemId}`, quantity)
      await fetchCart()
    } catch (e) {
      error.value = e instanceof Error ? e.message : String(e)
      throw e
    }
  }

  async function removeItem(cartItemId: number) {
    error.value = null
    try {
      await del(`/cart/items/${cartItemId}`)
      await fetchCart()
    } catch (e) {
      error.value = e instanceof Error ? e.message : String(e)
      throw e
    }
  }

  async function clearCart() {
    error.value = null
    try {
      await del('/cart')
      items.value = []
    } catch (e) {
      error.value = e instanceof Error ? e.message : String(e)
      throw e
    }
  }

  return {
    items, loading, error, itemCount, total,
    fetchCart, addItem, updateQuantity, removeItem, clearCart
  }
})
