<script setup lang="ts">
import SiteHeader from './components/SiteHeader.vue'
import SiteFooter from './components/SiteFooter.vue'
</script>

<template>
  <div class="app">
    <SiteHeader />
    <main class="main-content">
      <router-view />
    </main>
    <SiteFooter />
  </div>
</template>
