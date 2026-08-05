import { ref, computed } from 'vue'
import { defineStore } from 'pinia'
import { post, get } from '../api/client'

const TOKEN_KEY = 'jwt'

function isTokenExpired(t: string): boolean {
  try {
    const base64 = t.split('.')[1].replace(/-/g, '+').replace(/_/g, '/')
    const pad = base64.length % 4
    const padded = pad ? base64 + ('='.repeat(4 - pad)) : base64
    const payload = JSON.parse(atob(padded))
    return payload.exp && payload.exp * 1000 < Date.now()
  } catch {
    return true
  }
}

export const useAuthStore = defineStore('auth', () => {
  const token = ref<string | null>(localStorage.getItem(TOKEN_KEY))

  // Clear expired token on store init
  if (token.value && isTokenExpired(token.value)) {
    localStorage.removeItem(TOKEN_KEY)
    token.value = null
  }

  const isAuthenticated = computed(() => !!token.value)

  async function login(email: string, password: string) {
    const data = await post<{ token: string }>('/auth/login', { email, password })
    token.value = data.token
    localStorage.setItem(TOKEN_KEY, data.token)
  }

  async function register(email: string, password: string, firstName: string, lastName: string) {
    const data = await post<{ token: string }>('/auth/register', { email, password, firstName, lastName })
    token.value = data.token
    localStorage.setItem(TOKEN_KEY, data.token)
  }

  async function fetchToken() {
    const resp = await fetch('/Account/IssueToken', { credentials: 'include' })
    if (!resp.ok) throw new Error(`Token fetch failed: ${resp.status}`)
    const data = await resp.json()
    token.value = data.token ?? null
    if (token.value) localStorage.setItem(TOKEN_KEY, token.value)
    else localStorage.removeItem(TOKEN_KEY)
  }

  async function fetchProfile() {
    return get<{ id: number; email: string; firstName: string; lastName: string; createdDate: string }>('/auth/me')
  }

  function logout() {
    token.value = null
    localStorage.removeItem(TOKEN_KEY)
  }

  function setToken(value: string) {
    token.value = value
    localStorage.setItem(TOKEN_KEY, value)
  }

  return { token, isAuthenticated, login, register, fetchToken, fetchProfile, logout, setToken }
})
