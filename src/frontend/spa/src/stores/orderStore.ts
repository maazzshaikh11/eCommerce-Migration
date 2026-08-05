import { ref } from 'vue'
import { defineStore } from 'pinia'
import { get } from '../api/client'
import type { OrderDto, OrderConfirmationDto } from '../types'

export const useOrderStore = defineStore('order', () => {
  const orders = ref<OrderDto[]>([])
  const currentOrder = ref<OrderConfirmationDto | null>(null)
  const loading = ref(false)
  const error = ref<string | null>(null)

  async function fetchOrders() {
    loading.value = true
    error.value = null
    try {
      orders.value = await get<OrderDto[]>('/orders')
    } catch (e) {
      error.value = e instanceof Error ? e.message : String(e)
    } finally {
      loading.value = false
    }
  }

  async function fetchOrder(id: number) {
    loading.value = true
    error.value = null
    try {
      currentOrder.value = await get<OrderConfirmationDto>(`/orders/${id}`)
    } catch (e) {
      error.value = e instanceof Error ? e.message : String(e)
    } finally {
      loading.value = false
    }
  }

  return { orders, currentOrder, loading, error, fetchOrders, fetchOrder }
})
