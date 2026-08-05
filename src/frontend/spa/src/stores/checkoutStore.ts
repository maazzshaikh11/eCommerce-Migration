import { ref } from 'vue'
import { defineStore } from 'pinia'
import { get, post } from '../api/client'
import type { AddressDto, OrderConfirmationDto, ShippingOptionDto } from '../types'

export const useCheckoutStore = defineStore('checkout', () => {
  const addresses = ref<AddressDto[]>([])
  const selectedAddressId = ref<number | null>(null)
  const shippingOptions = ref<ShippingOptionDto[]>([])
  const selectedShippingMethod = ref<string>('Standard')
  const idempotencyKey = ref('')
  const confirmation = ref<OrderConfirmationDto | null>(null)
  const loading = ref(false)
  const error = ref<string | null>(null)

  function generateIdempotencyKey() {
    idempotencyKey.value = typeof crypto !== 'undefined' && crypto.randomUUID
      ? crypto.randomUUID()
      : `${Date.now()}-${Math.random().toString(36).slice(2)}`
  }

  async function fetchAddresses() {
    loading.value = true
    error.value = null
    try {
      addresses.value = await get<AddressDto[]>('/checkout/addresses')
    } catch (e) {
      error.value = e instanceof Error ? e.message : String(e)
    } finally {
      loading.value = false
    }
  }

  async function fetchShippingOptions() {
    try {
      shippingOptions.value = await get<ShippingOptionDto[]>('/checkout/shipping-options')
    } catch (e) {
      error.value = e instanceof Error ? e.message : String(e)
    }
  }

  async function placeOrder() {
    if (!selectedAddressId.value) {
      error.value = 'Please select a shipping address.'
      return
    }
    loading.value = true
    error.value = null
    try {
      const result = await post<OrderConfirmationDto>('/checkout/orders', {
        addressId: selectedAddressId.value,
        shippingMethod: selectedShippingMethod.value,
      }, undefined, {
        'Idempotency-Key': idempotencyKey.value,
      })
      confirmation.value = result
    } catch (e) {
      error.value = e instanceof Error ? e.message : String(e)
    } finally {
      loading.value = false
    }
  }

  return {
    addresses, selectedAddressId, shippingOptions, selectedShippingMethod,
    idempotencyKey, confirmation, loading, error,
    generateIdempotencyKey, fetchAddresses, fetchShippingOptions, placeOrder,
  }
})
