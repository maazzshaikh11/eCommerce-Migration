<script setup lang="ts">
import { onMounted } from 'vue'
import { useCartStore } from '../stores/cartStore'
import { formatINR } from '../utils/currency'

const store = useCartStore()
const fmt = formatINR
onMounted(() => store.fetchCart())

function updateQty(id: number, qty: number) {
  if (qty < 1) store.removeItem(id)
  else store.updateQuantity(id, qty)
}

function remove(id: number) {
  store.removeItem(id)
}

function clear() {
  if (confirm('Clear your entire cart?')) store.clearCart()
}
</script>

<template>
  <div>
    <nav aria-label="breadcrumb" class="container" style="padding-top:100px;">
      <ol class="breadcrumb">
        <li class="breadcrumb-item"><router-link to="/">Home</router-link></li>
        <li class="breadcrumb-item active" aria-current="page">Cart</li>
      </ol>
    </nav>

    <section class="rf-divider" style="padding:64px 0;">
      <h2>CART</h2>
    </section>

    <div class="container" style="padding-bottom:80px;">
      <div v-if="store.loading" class="text-center" style="padding:48px 0;">Loading...</div>
      <div v-else-if="store.items.length === 0" class="rf-empty">
        <div class="ic"><svg xmlns="http://www.w3.org/2000/svg" width="64" height="64" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="1.5" stroke-linecap="round" stroke-linejoin="round"><path d="M6 2 3 6v14a2 2 0 0 0 2 2h14a2 2 0 0 0 2-2V6l-3-4Z"/><path d="M3 6h18"/><path d="M16 10a4 4 0 0 1-8 0"/></svg></div>
        <h3>Your cart is empty</h3>
        <p>You haven't added anything yet.</p>
        <router-link to="/products" class="rf-btn"><span>Start Shopping</span></router-link>
      </div>
      <div v-else style="display:grid; grid-template-columns: 2fr 1fr; gap: 48px;">
        <div>
          <div class="rf-surface" style="padding:0 28px;">
            <div style="display:flex; justify-content:space-between; align-items:center; padding:20px 0; border-bottom:1px solid var(--line);">
              <strong style="font-family:'Clash Display',sans-serif; font-size:1.2rem; text-transform:uppercase;">Items ({{ store.itemCount }})</strong>
              <button class="rf-btn sm outline" @click="clear"><span>Clear</span></button>
            </div>
            <table class="rf-cart-table">
              <thead>
                <tr><th>Product</th><th style="text-align:right;">Price</th><th style="text-align:center; width:140px;">Qty</th><th style="text-align:right;">Total</th><th></th></tr>
              </thead>
              <tbody>
                <tr v-for="item in store.items" :key="item.id">
                  <td>
                    <div class="prod">
                      <router-link :to="`/product/${item.productId}`" style="width:64px;height:64px;flex-shrink:0;">
                        <img v-if="item.imageUrl" :src="item.imageUrl" style="width:64px;height:64px;object-fit:cover;" alt="" />
                        <span v-else class="thumb" style="display:flex;align-items:center;justify-content:center;"><svg xmlns="http://www.w3.org/2000/svg" width="20" height="20" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="1.5" stroke-linecap="round" stroke-linejoin="round"><rect width="18" height="18" x="3" y="3" rx="2" ry="2"/><circle cx="9" cy="9" r="2"/><path d="m21 15-3.086-3.086a2 2 0 0 0-2.828 0L6 21"/></svg></span>
                      </router-link>
                      <div>
                        <router-link :to="`/product/${item.productId}`" style="font-weight:700; text-transform:uppercase; font-size:13px; letter-spacing:0.1em;">{{ item.productName }}</router-link>
                        <div v-if="item.variantName" style="font-size:12px; color:var(--ink-faint);">{{ item.variantName }}</div>
                      </div>
                    </div>
                  </td>
                  <td style="text-align:right;">{{ fmt(item.unitPrice) }}</td>
                  <td style="text-align:center;">
                    <div class="rf-qty" style="margin:0 auto;">
                      <button @click="updateQty(item.id, item.quantity - 1)"><svg xmlns="http://www.w3.org/2000/svg" width="12" height="12" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2" stroke-linecap="round" stroke-linejoin="round"><path d="M5 12h14"/></svg></button>
                      <input :value="item.quantity" type="number" min="1" max="99" readonly />
                      <button @click="updateQty(item.id, item.quantity + 1)"><svg xmlns="http://www.w3.org/2000/svg" width="12" height="12" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2" stroke-linecap="round" stroke-linejoin="round"><path d="M5 12h14"/><path d="M12 5v14"/></svg></button>
                    </div>
                  </td>
                  <td style="text-align:right; font-family:'Clash Display',sans-serif; font-weight:600;">{{ fmt(item.lineTotal) }}</td>
                  <td style="text-align:center;">
                    <button class="rf-icon-btn" style="width:32px;height:32px;" @click="remove(item.id)">
                      <svg xmlns="http://www.w3.org/2000/svg" width="16" height="16" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2" stroke-linecap="round" stroke-linejoin="round" style="color:var(--accent);"><path d="M3 6h18"/><path d="M19 6v14c0 1-1 2-2 2H7c-1 0-2-1-2-2V6"/><path d="M8 6V4c0-1 1-2 2-2h4c1 0 2 1 2 2v2"/></svg>
                    </button>
                  </td>
                </tr>
              </tbody>
            </table>
          </div>
        </div>

        <div>
          <div class="rf-surface">
            <h6 style="font-family:'Clash Display',sans-serif; font-size:1.1rem; text-transform:uppercase; margin-bottom:20px;">Order Summary</h6>
            <div class="rf-summary"><span>Items</span><span>{{ store.itemCount }}</span></div>
            <div class="rf-summary"><span>Subtotal</span><span style="font-weight:700;">{{ fmt(store.total) }}</span></div>
            <p class="text-ink-mute" style="font-size:13px; margin:12px 0;"><svg xmlns="http://www.w3.org/2000/svg" width="14" height="14" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2" stroke-linecap="round" stroke-linejoin="round" style="vertical-align:middle;"><circle cx="12" cy="12" r="10"/><path d="M12 16v-4"/><path d="M12 8h.01"/></svg> Shipping & taxes at checkout.</p>
            <div style="display:grid; gap:8px;">
              <router-link to="/checkout" class="rf-btn"><span><svg xmlns="http://www.w3.org/2000/svg" width="16" height="16" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2" stroke-linecap="round" stroke-linejoin="round" style="vertical-align:middle;margin-right:6px;"><rect width="18" height="11" x="3" y="11" rx="2" ry="2"/><path d="M7 11V7a5 5 0 0 1 10 0v4"/></svg>Checkout</span></router-link>
              <router-link to="/products" class="rf-btn outline"><span>Continue Shopping</span></router-link>
            </div>
          </div>
        </div>
      </div>
    </div>
  </div>
</template>

<style scoped>
@media (max-width: 991px) {
  .rf-cart-table { display: block; overflow-x: auto; white-space: nowrap; }
}
</style>
