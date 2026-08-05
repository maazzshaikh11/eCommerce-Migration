<script setup lang="ts">
import { useCartStore } from '../stores/cartStore'

const cartStore = useCartStore()
</script>

<template>
  <router-link to="/cart" class="cart-badge">
    Cart ({{ cartStore.itemCount }})
  </router-link>
</template>

<style scoped>
.cart-badge { text-decoration: none; color: #333; font-weight: bold; }
</style>
