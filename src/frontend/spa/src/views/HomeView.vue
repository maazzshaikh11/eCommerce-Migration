<script setup lang="ts">
import { onMounted } from 'vue'
import { useProductStore } from '../stores/productStore'
import AddToCartButton from '../components/AddToCartButton.vue'
import { formatINR } from '../utils/currency'

const store = useProductStore()
const fmt = formatINR
onMounted(() => store.fetchProducts())
</script>

<template>
  <div>
    <!-- HERO -->
    <section class="rf-hero">
      <div class="blob blob-a" style="width:60vw;height:60vw;background:#DB4A2B;top:-10%;left:-15%;"></div>
      <div class="blob blob-b" style="width:50vw;height:50vw;background:#F8A348;bottom:-20%;right:-10%;"></div>
      <div class="blob blob-a" style="width:30vw;height:30vw;background:#FF89A9;top:30%;left:40%;"></div>
      <div class="rf-hero-inner">
        <h1 class="slide-up">RAW<br /><span class="indent">FORM</span></h1>
        <p class="rf-hero-sub slide-up d2">Premium products for modern living. Fast shipping, secure checkout, and a 30-day guarantee.</p>
        <div class="rf-hero-cta slide-up d3">
          <router-link to="/products" class="rf-btn"><span>Shop All</span></router-link>
          <a href="#featured" class="rf-link">View Featured <svg xmlns="http://www.w3.org/2000/svg" width="16" height="16" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2" stroke-linecap="round" stroke-linejoin="round"><path d="M5 12h14"/><path d="m12 5 7 7-7 7"/></svg></a>
        </div>
      </div>
    </section>

    <!-- CATEGORY DIVIDER -->
    <section class="rf-divider slide-up">
      <h2>CATEGORIES</h2>
    </section>

    <!-- CATEGORIES -->
    <section class="section-pad" style="padding-top:0;">
      <div class="container">
        <div class="rf-cats slide-up">
          <router-link to="/products?categoryId=1" class="rf-cat">
            <span class="ic"><svg xmlns="http://www.w3.org/2000/svg" width="32" height="32" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="1.5" stroke-linecap="round" stroke-linejoin="round"><rect width="16" height="16" x="4" y="4" rx="2"/><rect width="6" height="6" x="9" y="9" rx="1"/><path d="M15 2v2"/><path d="M15 20v2"/><path d="M2 15h2"/><path d="M2 9h2"/><path d="M20 15h2"/><path d="M20 9h2"/><path d="M9 2v2"/><path d="M9 20v2"/></svg></span>
            <h5>Electronics</h5>
            <small>Gadgets & devices</small>
          </router-link>
          <router-link to="/products?categoryId=2" class="rf-cat">
            <span class="ic"><svg xmlns="http://www.w3.org/2000/svg" width="32" height="32" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="1.5" stroke-linecap="round" stroke-linejoin="round"><rect width="20" height="14" x="2" y="3" rx="2"/><line x1="8" x2="16" y1="21" y2="21"/><line x1="12" x2="12" y1="17" y2="21"/></svg></span>
            <h5>Computers</h5>
            <small>Laptops & gear</small>
          </router-link>
          <router-link to="/products?categoryId=3" class="rf-cat">
            <span class="ic"><svg xmlns="http://www.w3.org/2000/svg" width="32" height="32" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="1.5" stroke-linecap="round" stroke-linejoin="round"><path d="M3 18v-6a9 9 0 0 1 18 0v6"/><path d="M21 19a2 2 0 0 1-2 2h-1a2 2 0 0 1-2-2v-3a2 2 0 0 1 2-2h3a2 2 0 0 1 2 2v1a2 2 0 0 1-2 2z"/><path d="M3 19a2 2 0 0 0 2 2h1a2 2 0 0 0 2-2v-3a2 2 0 0 0-2-2H3a2 2 0 0 0-2 2v1a2 2 0 0 0 2 2z"/></svg></span>
            <h5>Audio</h5>
            <small>Sound systems</small>
          </router-link>
          <router-link to="/products?categoryId=4" class="rf-cat">
            <span class="ic"><svg xmlns="http://www.w3.org/2000/svg" width="32" height="32" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="1.5" stroke-linecap="round" stroke-linejoin="round"><path d="M15 21v-8a1 1 0 0 0-1-1h-4a1 1 0 0 0-1 1v8"/><path d="M3 10a2 2 0 0 1 2-2h14a2 2 0 0 1 2 2v11H3z"/></svg></span>
            <h5>Home & Garden</h5>
            <small>Everyday living</small>
          </router-link>
          <router-link to="/products?categoryId=5" class="rf-cat">
            <span class="ic"><svg xmlns="http://www.w3.org/2000/svg" width="32" height="32" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="1.5" stroke-linecap="round" stroke-linejoin="round"><path d="M20.38 3.4a1.6 1.6 0 0 0-2.26 0l-10.3 10.3a1.6 1.6 0 0 0 0 2.26l2.26 2.26a1.6 1.6 0 0 0 2.26 0l10.3-10.3a1.6 1.6 0 0 0 0-2.26z"/><path d="M14.5 7.5 17 10"/><path d="M11.5 10.5 14 13"/><path d="M8.5 13.5 11 16"/><path d="M5.5 16.5 8 19"/></svg></span>
            <h5>Clothing</h5>
            <small>Wardrobe staples</small>
          </router-link>
          <router-link to="/products" class="rf-cat">
            <span class="ic"><svg xmlns="http://www.w3.org/2000/svg" width="32" height="32" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="1.5" stroke-linecap="round" stroke-linejoin="round"><rect width="7" height="7" x="3" y="3" rx="1"/><rect width="7" height="7" x="14" y="3" rx="1"/><rect width="7" height="7" x="14" y="14" rx="1"/><rect width="7" height="7" x="3" y="14" rx="1"/></svg></span>
            <h5>All</h5>
            <small>Browse everything</small>
          </router-link>
        </div>
      </div>
    </section>

    <!-- CAMPAIGN BLOCK -->
    <section style="background:#D9D6D0; padding:96px 0;">
      <div class="container">
        <div style="display:grid; grid-template-columns:2fr 1fr; gap:48px; align-items:center;">
          <div>
            <h2 style="font-size:6vw;line-height:0.9;text-transform:uppercase;letter-spacing:-0.05em;">New<br/>Season<br/>Drop</h2>
          </div>
          <div>
            <div style="border-top:1px solid rgba(30,30,30,.2); padding:20px 0;">
              <router-link to="/products" class="rf-link">Shop New Arrivals <svg xmlns="http://www.w3.org/2000/svg" width="16" height="16" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2" stroke-linecap="round" stroke-linejoin="round"><path d="M7 7h10v10"/><path d="M7 17 17 7"/></svg></router-link>
            </div>
            <div style="border-top:1px solid rgba(30,30,30,.2); padding:20px 0;">
              <router-link to="/products?categoryId=3" class="rf-link">Explore Audio <svg xmlns="http://www.w3.org/2000/svg" width="16" height="16" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2" stroke-linecap="round" stroke-linejoin="round"><path d="M7 7h10v10"/><path d="M7 17 17 7"/></svg></router-link>
            </div>
            <div style="border-top:1px solid rgba(30,30,30,.2); border-bottom:1px solid rgba(30,30,30,.2); padding:20px 0;">
              <router-link to="/products?categoryId=4" class="rf-link">Home Essentials <svg xmlns="http://www.w3.org/2000/svg" width="16" height="16" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2" stroke-linecap="round" stroke-linejoin="round"><path d="M7 7h10v10"/><path d="M7 17 17 7"/></svg></router-link>
            </div>
          </div>
        </div>
      </div>
    </section>

    <!-- FEATURED DIVIDER -->
    <section class="rf-divider right slide-up">
      <h2>FEATURED</h2>
    </section>

    <!-- FEATURED PRODUCTS -->
    <section id="featured" class="section-pad" style="padding-top:0;">
      <div class="container">
        <div class="rf-section-head slide-up">
          <h2>Selected<br/>Products</h2>
          <router-link to="/products" class="rf-link">View All <svg xmlns="http://www.w3.org/2000/svg" width="16" height="16" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2" stroke-linecap="round" stroke-linejoin="round"><path d="M5 12h14"/><path d="m12 5 7 7-7 7"/></svg></router-link>
        </div>
        <div v-if="store.items.length > 0" class="rf-grid fade-row">
          <div v-for="p in store.items.slice(0, 6)" :key="p.id" class="rf-product slide-up">
            <router-link :to="`/product/${p.id}`" class="media">
              <img v-if="p.thumbnailUrl" :src="p.thumbnailUrl" :alt="p.name" />
              <div v-else class="ph"><svg xmlns="http://www.w3.org/2000/svg" width="48" height="48" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="1.5" stroke-linecap="round" stroke-linejoin="round"><rect width="18" height="18" x="3" y="3" rx="2" ry="2"/><circle cx="9" cy="9" r="2"/><path d="m21 15-3.086-3.086a2 2 0 0 0-2.828 0L6 21"/></svg></div>
            </router-link>
            <div class="meta">
                <router-link :to="`/product/${p.id}`"><h3>{{ p.name }}</h3></router-link>
                <span class="price">{{ fmt(p.price) }}</span>
            </div>
            <div class="actions">
              <AddToCartButton :product-id="p.id" />
              <router-link :to="`/product/${p.id}`" class="rf-btn sm outline"><span>View</span></router-link>
            </div>
          </div>
        </div>
        <div v-else class="rf-empty">
          <div class="ic"><svg xmlns="http://www.w3.org/2000/svg" width="64" height="64" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="1.5" stroke-linecap="round" stroke-linejoin="round"><path d="m7.5 4.27 9 5.15"/><path d="M21 8a2 2 0 0 0-1-1.73l-7-4a2 2 0 0 0-2 0l-7 4A2 2 0 0 0 3 8v8a2 2 0 0 0 1 1.73l7 4a2 2 0 0 0 2 0l7-4A2 2 0 0 0 21 16Z"/><path d="m3.3 7 8.7 5 8.7-5"/><path d="M12 22V12"/></svg></div>
          <h3>No products yet</h3>
          <p>New products are being added. Check back soon.</p>
          <router-link to="/products" class="rf-btn"><span>Browse All</span></router-link>
        </div>
      </div>
    </section>
  </div>
</template>
