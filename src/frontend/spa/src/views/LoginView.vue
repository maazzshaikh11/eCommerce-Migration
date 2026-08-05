<script setup lang="ts">
import { ref } from 'vue'
import { useRouter } from 'vue-router'
import { useAuthStore } from '../stores/useAuthStore'

const router = useRouter()
const authStore = useAuthStore()
const email = ref('')
const password = ref('')
const error = ref('')

async function login() {
  error.value = ''
  try {
    await authStore.login(email.value, password.value)
    router.push('/')
  } catch (e) {
    error.value = e instanceof Error ? e.message : 'Login failed'
  }
}
</script>

<template>
  <div class="rf-auth">
    <div class="blob blob-a" style="width:40vw;height:40vw;background:#DB4A2B;top:-10%;left:-10%;"></div>
    <div class="blob blob-b" style="width:35vw;height:35vw;background:#F8A348;bottom:-15%;right:-10%;"></div>
    <div class="rf-auth-card slide-up">
      <h1>Welcome<br/>Back</h1>
      <p class="sub">Sign in to your Raw Form account.</p>

      <form @submit.prevent="login">
        <div v-if="error" class="rf-validation-summary" style="background:rgba(219,74,43,.08); border-left:3px solid var(--accent); padding:12px 16px; margin-bottom:24px;">{{ error }}</div>
        <div class="rf-field"><label>Email</label><input v-model="email" type="email" class="rf-input" placeholder="you@example.com" required /></div>
        <div class="rf-field"><label>Password</label><input v-model="password" type="password" class="rf-input" placeholder="Enter password" required /></div>
        <button type="submit" class="rf-btn" style="width:100%;"><span>Sign In</span></button>
      </form>
      <p class="rf-auth-switch">No account? <router-link to="/register">Create one</router-link></p>
    </div>
  </div>
</template>
