<script setup lang="ts">
</script>

<template>
  <div class="container" style="padding-top:120px; padding-bottom:80px;">
    <h1 style="font-size:clamp(2rem,6vw,4rem); text-transform:uppercase; margin-bottom:32px;">Dashboard</h1>
    <div style="display:grid; grid-template-columns: repeat(auto-fit, minmax(220px, 1fr)); gap: 24px;">
      <!-- Profile -->
      <router-link to="/profile" class="rf-surface d-block" style="text-decoration:none; color:var(--ink);">
        <svg xmlns="http://www.w3.org/2000/svg" width="32" height="32" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="1.5" stroke-linecap="round" stroke-linejoin="round" style="color:var(--accent);"><path d="M19 21v-2a4 4 0 0 0-4-4H9a4 4 0 0 0-4 4v2"/><circle cx="12" cy="7" r="4"/></svg>
        <h6 style="font-family:'Clash Display',sans-serif; font-size:1.1rem; text-transform:uppercase; margin-top:12px; margin-bottom:4px;">Profile</h6>
        <small class="text-ink-mute">Account info</small>
      </router-link>

      <!-- Orders -->
      <router-link to="/orders" class="rf-surface d-block" style="text-decoration:none; color:var(--ink);">
        <svg xmlns="http://www.w3.org/2000/svg" width="32" height="32" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="1.5" stroke-linecap="round" stroke-linejoin="round" style="color:var(--accent);"><path d="M6 2 3 6v14a2 2 0 0 0 2 2h14a2 2 0 0 0 2-2V6l-3-4Z"/><path d="M3 6h18"/><path d="M16 10a4 4 0 0 1-8 0"/></svg>
        <h6 style="font-family:'Clash Display',sans-serif; font-size:1.1rem; text-transform:uppercase; margin-top:12px; margin-bottom:4px;">Orders</h6>
        <small class="text-ink-mute">Order history</small>
      </router-link>

      <!-- Security -->
      <a href="#" class="rf-surface d-block" style="text-decoration:none; color:var(--ink); cursor:not-allowed; opacity:0.5;">
        <svg xmlns="http://www.w3.org/2000/svg" width="32" height="32" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="1.5" stroke-linecap="round" stroke-linejoin="round" style="color:var(--accent);"><rect width="18" height="11" x="3" y="11" rx="2" ry="2"/><path d="M7 11V7a5 5 0 0 1 10 0v4"/></svg>
        <h6 style="font-family:'Clash Display',sans-serif; font-size:1.1rem; text-transform:uppercase; margin-top:12px; margin-bottom:4px;">Security</h6>
        <small class="text-ink-mute">Change password</small>
      </a>

      <!-- Cart -->
      <router-link to="/cart" class="rf-surface d-block" style="text-decoration:none; color:var(--ink);">
        <svg xmlns="http://www.w3.org/2000/svg" width="32" height="32" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="1.5" stroke-linecap="round" stroke-linejoin="round" style="color:var(--accent);"><circle cx="9" cy="21" r="1"/><circle cx="20" cy="21" r="1"/><path d="M1 1h4l2.68 13.39a2 2 0 0 0 2 1.61h9.72a2 2 0 0 0 2-1.61L23 6H6"/></svg>
        <h6 style="font-family:'Clash Display',sans-serif; font-size:1.1rem; text-transform:uppercase; margin-top:12px; margin-bottom:4px;">Cart</h6>
        <small class="text-ink-mute">View cart</small>
      </router-link>
    </div>
  </div>
</template>
