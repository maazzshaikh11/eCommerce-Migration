<script setup lang="ts">
import { onMounted } from 'vue'
import { useRoute } from 'vue-router'
import { useOrderStore } from '../stores/orderStore'
import { formatINR } from '../utils/currency'

const route = useRoute()
const store = useOrderStore()
const fmt = formatINR
onMounted(() => store.fetchOrder(Number(route.params.id)))
</script>

<template>
  <div class="container" style="padding-top:120px; padding-bottom:80px;">
    <div v-if="store.loading" class="text-center" style="padding:48px 0;">Loading...</div>
    <div v-else-if="store.currentOrder">
      <h1 style="font-size:clamp(2rem,5vw,3rem); text-transform:uppercase; margin-bottom:16px;">Order #{{ store.currentOrder.orderNumber }}</h1>
      <p style="color:var(--ink-2); margin-bottom:32px;">Status: <span class="pill" :class="{ success: store.currentOrder.orderStatus === 'Paid', warning: store.currentOrder.orderStatus === 'Pending' }">{{ store.currentOrder.orderStatus }}</span> | {{ new Date(store.currentOrder.orderDate).toLocaleDateString() }}</p>

      <div class="rf-surface" style="margin-bottom:24px;">
        <h6 style="font-family:'Clash Display',sans-serif; font-size:1.1rem; text-transform:uppercase; margin-bottom:16px;">Items</h6>
        <div v-for="item in store.currentOrder.items" :key="item.productName" style="display:flex; justify-content:space-between; padding:12px 0; border-bottom:1px solid var(--line);">
          <span>{{ item.productName }} x{{ item.quantity }}</span>
          <span style="font-weight:600;">{{ fmt(item.lineTotal) }}</span>
        </div>
        <div style="display:flex; justify-content:space-between; padding:16px 0 0; font-family:'Clash Display',sans-serif; font-size:1.2rem; font-weight:600;">
          <span>Grand Total</span>
          <span>{{ fmt(store.currentOrder.grandTotal) }}</span>
        </div>
      </div>

      <router-link to="/orders" class="rf-btn outline"><span>Back to Orders</span></router-link>
    </div>
    <div v-else class="rf-empty">
      <h3>Order not found</h3>
      <router-link to="/orders" class="rf-btn"><span>Back to Orders</span></router-link>
    </div>
  </div>
</template>
