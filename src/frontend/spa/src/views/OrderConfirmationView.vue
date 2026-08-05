<script setup lang="ts">
import { useRoute } from 'vue-router'

const route = useRoute()
const orderId = route.params.orderId
</script>

<template>
  <div class="container" style="padding-top:120px; padding-bottom:80px;">
    <div class="rf-empty">
      <div class="ic"><svg xmlns="http://www.w3.org/2000/svg" width="64" height="64" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="1.5" stroke-linecap="round" stroke-linejoin="round"><path d="M22 11.08V12a10 10 0 1 1-5.93-9.14"/><polyline points="22 4 12 14.01 9 11.01"/></svg></div>
      <h1>Order Confirmation</h1>
      <p>Thank you for your order!</p>
      <p style="font-family:'Clash Display',sans-serif; font-size:1.2rem; font-weight:600;">Order ID: {{ orderId }}</p>
      <router-link to="/orders" class="rf-btn"><span>View Orders</span></router-link>
    </div>
  </div>
</template>
