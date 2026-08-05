<script setup lang="ts">
import { ref } from 'vue'
import { useRouter } from 'vue-router'
import { useAuthStore } from '../stores/useAuthStore'

const router = useRouter()
const authStore = useAuthStore()
const email = ref('')
const password = ref('')
const firstName = ref('')
const lastName = ref('')
const error = ref('')

async function register() {
  error.value = ''
  try {
    await authStore.register(email.value, password.value, firstName.value, lastName.value)
    router.push('/')
  } catch (e) {
    error.value = e instanceof Error ? e.message : 'Registration failed'
  }
}
</script>

<template>
  <div class="rf-auth">
    <div class="blob blob-a" style="width:40vw;height:40vw;background:#FF89A9;top:-10%;right:-10%;"></div>
    <div class="blob blob-b" style="width:35vw;height:35vw;background:#F8A348;bottom:-15%;left:-10%;"></div>
    <div class="rf-auth-card wide slide-up">
      <h1>Join<br/>Raw Form</h1>
      <p class="sub">Create your account for fast checkout and order tracking.</p>

      <form @submit.prevent="register">
        <div v-if="error" class="rf-validation-summary" style="background:rgba(219,74,43,.08); border-left:3px solid var(--accent); padding:12px 16px; margin-bottom:24px;">{{ error }}</div>
        <div style="display:grid; grid-template-columns: 1fr 1fr; gap: 16px;">
          <div class="rf-field"><label>First Name</label><input v-model="firstName" class="rf-input" placeholder="Jane" required /></div>
          <div class="rf-field"><label>Last Name</label><input v-model="lastName" class="rf-input" placeholder="Doe" required /></div>
        </div>
        <div class="rf-field"><label>Email</label><input v-model="email" type="email" class="rf-input" placeholder="you@example.com" required /></div>
        <div style="display:grid; grid-template-columns: 1fr 1fr; gap: 16px;">
          <div class="rf-field"><label>Password</label><input v-model="password" type="password" class="rf-input" placeholder="Min 8 chars" required /></div>
          <div class="rf-field"><label>Confirm</label><input type="password" class="rf-input" placeholder="Re-enter" required /></div>
        </div>
        <button type="submit" class="rf-btn" style="width:100%;"><span>Create Account</span></button>
      </form>
      <p class="rf-auth-switch">Already have an account? <router-link to="/login">Sign in</router-link></p>
    </div>
  </div>
</template>
