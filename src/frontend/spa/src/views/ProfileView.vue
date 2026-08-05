<script setup lang="ts">
import { onMounted, ref } from 'vue'
import { useAuthStore } from '../stores/useAuthStore'

const authStore = useAuthStore()
const user = ref<{ id: number; email: string; firstName: string; lastName: string; createdDate: string } | null>(null)
const loading = ref(false)
const error = ref('')

onMounted(async () => {
  loading.value = true
  try {
    const data = await authStore.fetchProfile()
    user.value = data
  } catch (e) {
    error.value = e instanceof Error ? e.message : String(e)
  } finally {
    loading.value = false
  }
})
</script>

<template>
  <div class="container" style="padding-top:120px; padding-bottom:80px;">
    <h1 style="font-size:clamp(2rem,5vw,3rem); text-transform:uppercase; margin-bottom:32px;">My Profile</h1>

    <div v-if="loading" class="text-center" style="padding:48px 0;">Loading...</div>
    <div v-else-if="error" class="alert alert-danger">{{ error }}</div>
    <div v-else-if="user" class="rf-surface" style="max-width:600px;">
      <div class="rf-field">
        <label>Email</label>
        <div class="rf-input" style="background:var(--paper);">{{ user.email }}</div>
      </div>
      <div style="display:grid; grid-template-columns: 1fr 1fr; gap: 16px;">
        <div class="rf-field">
          <label>First Name</label>
          <div class="rf-input" style="background:var(--paper);">{{ user.firstName }}</div>
        </div>
        <div class="rf-field">
          <label>Last Name</label>
          <div class="rf-input" style="background:var(--paper);">{{ user.lastName }}</div>
        </div>
      </div>
      <div class="rf-field">
        <label>Member Since</label>
        <div class="rf-input" style="background:var(--paper);">{{ new Date(user.createdDate).toLocaleDateString() }}</div>
      </div>
    </div>
    <div v-else class="rf-empty">
      <h3>Unable to load profile</h3>
      <router-link to="/login" class="rf-btn"><span>Sign In</span></router-link>
    </div>
  </div>
</template>
