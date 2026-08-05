<script setup lang="ts">
import { useAuthStore } from '../stores/useAuthStore'
const authStore = useAuthStore()
const year = new Date().getFullYear()
</script>

<template>
  <footer class="rf-footer">
    <div class="container">
      <div class="f-row">
        <div>
          <div class="f-brand">RAW<span style="color:var(--accent-warm)">.</span>FORM</div>
          <p style="color:rgba(228,226,221,.6); font-size:14px; max-width:300px; line-height:1.5;">Premium products for modern living. Shop the latest tech, home goods, and apparel.</p>
          <div class="f-social">
            <a href="#" aria-label="Twitter"><svg xmlns="http://www.w3.org/2000/svg" width="18" height="18" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2" stroke-linecap="round" stroke-linejoin="round"><path d="M22 4s-.7 2.1-2 3.4c1.6 10-9.4 17.3-18 11.6 2.2.1 4.4-.6 6-2C3 15.5.5 9.6 3 5c2.2 2.6 5.6 4.1 9 4-.9-4.2 4-6.6 7-3.8 1.1 0 3-1.2 3-1.2z"/></svg></a>
            <a href="#" aria-label="Instagram"><svg xmlns="http://www.w3.org/2000/svg" width="18" height="18" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2" stroke-linecap="round" stroke-linejoin="round"><rect width="20" height="20" x="2" y="2" rx="5" ry="5"/><path d="M16 11.37A4 4 0 1 1 12.63 8 4 4 0 0 1 16 11.37z"/><line x1="17.5" x2="17.51" y1="6.5" y2="6.5"/></svg></a>
            <a href="#" aria-label="Github"><svg xmlns="http://www.w3.org/2000/svg" width="18" height="18" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2" stroke-linecap="round" stroke-linejoin="round"><path d="M15 22v-4a4.8 4.8 0 0 0-1-3.2c3 0 6-2 6-5.5.08-1.25-.27-2.48-1-3.5.28-1.15.28-2.66 0-3.77 0 0-1-.32-3.3 1.1a11.5 11.5 0 0 0-6.4 0C7.5.8 6.5.5 6.5.5c-.3 1.1-.3 2.62 0 3.77-.7 1-1.08 2.25-1 3.5 0 3.5 3 5.5 6 5.5-.7.6-.9 1.4-1 3.2"/></svg></a>
          </div>
        </div>
        <div>
          <h6>Shop</h6>
          <router-link to="/products">All Products</router-link>
          <router-link to="/products?categoryId=1">Electronics</router-link>
          <router-link to="/products?categoryId=3">Audio</router-link>
          <router-link to="/products?categoryId=4">Home & Garden</router-link>
        </div>
        <div>
          <h6>Account</h6>
          <template v-if="!authStore.isAuthenticated">
            <router-link to="/login">Login</router-link>
            <router-link to="/register">Sign Up</router-link>
          </template>
          <template v-else>
            <router-link to="/profile">My Profile</router-link>
            <router-link to="/orders">My Orders</router-link>
          </template>
          <router-link to="/cart">Cart</router-link>
        </div>
        <div>
          <h6>Support</h6>
          <a href="#">Help Center</a>
          <a href="#">Shipping Info</a>
          <a href="#">Returns</a>
          <a href="#">Contact Us</a>
        </div>
      </div>
      <div class="f-bottom">
        <span style="font-size:13px; color:rgba(228,226,221,.5);">&copy; {{ year }} Raw Form. All rights reserved.</span>
        <span style="font-size:13px; color:rgba(228,226,221,.5);">Designed with intent.</span>
      </div>
      <div class="f-year">{{ year }}</div>
    </div>
  </footer>
</template>
