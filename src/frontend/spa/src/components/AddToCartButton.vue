<script setup lang="ts">
import { ref } from 'vue'
import { useRouter } from 'vue-router'
import { useCartStore } from '../stores/cartStore'
import { useAuthStore } from '../stores/useAuthStore'

const props = defineProps<{
  productId: number
  variantId?: number
}>()

const router = useRouter()
const cartStore = useCartStore()
const authStore = useAuthStore()
const adding = ref(false)
const feedback = ref('')

async function handleClick() {
  if (!authStore.isAuthenticated) {
    router.push('/login')
    return
  }
  adding.value = true
  feedback.value = ''
  try {
    await cartStore.addItem(props.productId, 1, props.variantId)
    feedback.value = 'Added'
    setTimeout(() => feedback.value = '', 2000)
  } catch {
    feedback.value = 'Failed'
    setTimeout(() => feedback.value = '', 2000)
  } finally {
    adding.value = false
  }
}
</script>

<template>
  <button class="rf-btn sm" :disabled="adding" @click="handleClick">
    <span>
      <svg xmlns="http://www.w3.org/2000/svg" width="14" height="14" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2" stroke-linecap="round" stroke-linejoin="round" style="vertical-align:middle;margin-right:4px;"><path d="M6 2 3 6v14a2 2 0 0 0 2 2h14a2 2 0 0 0 2-2V6l-3-4Z"/><path d="M3 6h18"/><path d="M16 10a4 4 0 0 1-8 0"/></svg>
      {{ adding ? 'Adding...' : (feedback || 'Add to Cart') }}
    </span>
  </button>
</template>

