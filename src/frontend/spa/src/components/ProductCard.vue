<script setup lang="ts">
import type { ProductListResponseDto } from '../types'
import { formatINR } from '../utils/currency'

const fmt = formatINR

defineProps<{
  product: ProductListResponseDto
}>()
</script>

<template>
  <div class="product-card">
    <img v-if="product.thumbnailUrl" :src="product.thumbnailUrl" :alt="product.name" />
    <h3>{{ product.name }}</h3>
    <p>{{ fmt(product.price) }}</p>
    <router-link :to="`/product/${product.id}`">View</router-link>
  </div>
</template>

<style scoped>
.product-card { border: 1px solid #ddd; padding: 1rem; border-radius: 4px; }
.product-card img { max-width: 100%; height: auto; }
</style>
