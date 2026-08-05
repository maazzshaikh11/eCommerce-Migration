<script setup lang="ts">
import { onMounted } from 'vue'
import { useOrderStore } from '../stores/orderStore'
import { formatINR } from '../utils/currency'

const store = useOrderStore()
const fmt = formatINR
onMounted(() => store.fetchOrders())
</script>

<template>
  <div class="container" style="padding-top:120px; padding-bottom:80px;">
    <h1 style="font-size:clamp(2rem,6vw,4rem); text-transform:uppercase; margin-bottom:8px;">Orders</h1>
    <p class="text-ink-mute" style="margin-bottom:32px;">Your order history.</p>

    <div v-if="store.loading" class="text-center" style="padding:48px 0;">Loading...</div>
    <div v-else-if="store.orders.length === 0" class="rf-empty">
      <div class="ic"><svg xmlns="http://www.w3.org/2000/svg" width="64" height="64" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="1.5" stroke-linecap="round" stroke-linejoin="round"><path d="M6 2 3 6v14a2 2 0 0 0 2 2h14a2 2 0 0 0 2-2V6l-3-4Z"/><path d="M3 6h18"/><path d="M16 10a4 4 0 0 1-8 0"/></svg></div>
      <h3>No orders yet</h3>
      <p>You haven't placed any orders.</p>
      <router-link to="/products" class="rf-btn"><span>Start Shopping</span></router-link>
    </div>
    <div v-else class="rf-surface" style="padding:0 28px;">
      <table class="rf-orders-table">
        <thead><tr><th>Order #</th><th>Date</th><th>Status</th><th style="text-align:center;">Items</th><th style="text-align:right;">Total</th><th></th></tr></thead>
        <tbody>
          <tr v-for="order in store.orders" :key="order.id">
            <td style="font-family:'Clash Display',sans-serif; font-weight:600;">#{{ order.orderNumber }}</td>
            <td>{{ new Date(order.orderDate).toLocaleDateString() }}</td>
            <td>
              <span class="pill" :class="{ success: order.orderStatus === 'Paid', warning: order.orderStatus === 'Pending', danger: order.orderStatus === 'Cancelled' }">{{ order.orderStatus }}</span>
            </td>
            <td style="text-align:center;">--</td>
            <td style="text-align:right; font-family:'Clash Display',sans-serif; font-weight:600;">{{ fmt(order.grandTotal) }}</td>
            <td style="text-align:right;"><router-link :to="`/orders/${order.id}`" class="rf-btn sm outline"><span>View</span></router-link></td>
          </tr>
        </tbody>
      </table>
    </div>
  </div>
</template>
