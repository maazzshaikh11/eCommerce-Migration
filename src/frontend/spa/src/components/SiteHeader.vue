<script setup lang="ts">
import { ref, onMounted, onUnmounted } from 'vue'
import { useRoute, useRouter } from 'vue-router'
import { useCartStore } from '../stores/cartStore'
import { useAuthStore } from '../stores/useAuthStore'

const route = useRoute()
const router = useRouter()
const cartStore = useCartStore()
const authStore = useAuthStore()

const navOpen = ref(false)
const searchOpen = ref(false)
const profileOpen = ref(false)
const scrolled = ref(false)
const searchQuery = ref('')

function onScroll() { scrolled.value = window.scrollY > 20 }

function toggleSearch() {
  searchOpen.value = !searchOpen.value
  if (searchOpen.value) setTimeout(() => (document.querySelector('.rf-search input') as HTMLElement)?.focus(), 50)
}

function onSearch() {
  if (!searchQuery.value.trim()) return
  router.push({ path: '/products', query: { search: searchQuery.value } })
  searchOpen.value = false
}

function isActive(path: string) {
  if (path === '/') return route.path === '/'
  return route.path.startsWith(path)
}

onMounted(() => {
  window.addEventListener('scroll', onScroll)
  if (authStore.isAuthenticated) cartStore.fetchCart()
})
onUnmounted(() => window.removeEventListener('scroll', onScroll))
</script>

<template>
  <nav class="rf-nav" :class="{ scrolled, open: navOpen }">
    <router-link class="rf-brand" to="/">RAW<span class="dot">.</span>FORM</router-link>

    <ul class="rf-nav-links">
      <li><router-link :class="{ active: isActive('/') }" to="/">Home</router-link></li>
      <li><router-link :class="{ active: isActive('/products') }" to="/products">Shop</router-link></li>
    </ul>

    <div class="rf-nav-icons">
      <button class="rf-icon-btn rf-search-toggle" title="Search" @click="toggleSearch">
        <svg xmlns="http://www.w3.org/2000/svg" width="20" height="20" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="1.75" stroke-linecap="round" stroke-linejoin="round"><circle cx="11" cy="11" r="8"/><path d="m21 21-4.3-4.3"/></svg>
      </button>
      <router-link to="/cart" class="rf-icon-btn position-relative" title="Cart">
        <svg xmlns="http://www.w3.org/2000/svg" width="20" height="20" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="1.75" stroke-linecap="round" stroke-linejoin="round"><path d="M6 2 3 6v14a2 2 0 0 0 2 2h14a2 2 0 0 0 2-2V6l-3-4Z"/><path d="M3 6h18"/><path d="M16 10a4 4 0 0 1-8 0"/></svg>
        <span v-if="cartStore.itemCount > 0" class="rf-cart-count">{{ cartStore.itemCount }}</span>
      </router-link>

      <template v-if="authStore.isAuthenticated">
        <div class="rf-dropdown" style="position:relative;">
          <button class="rf-icon-btn" title="Account" style="border:none;background:transparent;cursor:pointer;" @click="profileOpen = !profileOpen">
            <svg xmlns="http://www.w3.org/2000/svg" width="20" height="20" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="1.75" stroke-linecap="round" stroke-linejoin="round"><path d="M19 21v-2a4 4 0 0 0-4-4H9a4 4 0 0 0-4 4v2"/><circle cx="12" cy="7" r="4"/></svg>
          </button>
          <div v-if="profileOpen" class="rf-dropdown-menu" style="display:block;position:absolute;top:100%;right:0;background:var(--base);border:1px solid var(--line);min-width:180px;z-index:2000;padding:8px 0;box-shadow:0 12px 40px rgba(0,0,0,0.15);">
            <router-link to="/account" class="rf-dropdown-item" style="display:block;padding:10px 20px;font-size:13px;text-transform:uppercase;letter-spacing:0.1em;font-weight:600;color:var(--ink);white-space:nowrap;">Dashboard</router-link>
            <router-link to="/profile" class="rf-dropdown-item" style="display:block;padding:10px 20px;font-size:13px;text-transform:uppercase;letter-spacing:0.1em;font-weight:600;color:var(--ink);white-space:nowrap;">My Profile</router-link>
            <router-link to="/orders" class="rf-dropdown-item" style="display:block;padding:10px 20px;font-size:13px;text-transform:uppercase;letter-spacing:0.1em;font-weight:600;color:var(--ink);white-space:nowrap;">My Orders</router-link>
            <div style="height:1px;background:var(--line);margin:6px 0;"></div>
            <button class="rf-dropdown-item" style="display:block;width:100%;text-align:left;padding:10px 20px;font-size:13px;text-transform:uppercase;letter-spacing:0.1em;font-weight:600;color:var(--ink);background:transparent;border:none;cursor:pointer;white-space:nowrap;" @click="authStore.logout(); $router.push('/')">Logout</button>
          </div>
        </div>
      </template>
      <router-link v-else to="/login" class="rf-icon-btn" title="Login">
        <svg xmlns="http://www.w3.org/2000/svg" width="20" height="20" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="1.75" stroke-linecap="round" stroke-linejoin="round"><path d="M15 3h4a2 2 0 0 1 2 2v14a2 2 0 0 1-2 2h-4"/><polyline points="10 17 15 12 10 7"/><line x1="15" x2="3" y1="12" y2="12"/></svg>
      </router-link>

      <button class="rf-icon-btn rf-nav-toggle" title="Menu" @click="navOpen = !navOpen">
        <svg xmlns="http://www.w3.org/2000/svg" width="20" height="20" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="1.75" stroke-linecap="round" stroke-linejoin="round"><line x1="4" x2="20" y1="12" y2="12"/><line x1="4" x2="20" y1="6" y2="6"/><line x1="4" x2="20" y1="18" y2="18"/></svg>
      </button>
    </div>
  </nav>

  <div class="rf-search" :class="{ open: searchOpen }">
    <div class="container">
      <form @submit.prevent="onSearch">
        <input v-model="searchQuery" type="text" placeholder="Type to search..." />
      </form>
    </div>
  </div>
</template>

<style scoped>
.rf-dropdown-menu { display: none; }
.rf-dropdown-item:hover { background: rgba(30,30,30,.06); color: var(--accent) !important; }
</style>
