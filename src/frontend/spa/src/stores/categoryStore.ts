import { ref } from 'vue'
import { defineStore } from 'pinia'
import { get } from '../api/client'
import type { CategoryDto } from '../types'

export const useCategoryStore = defineStore('category', () => {
  const categories = ref<CategoryDto[]>([])
  const loading = ref(false)
  const error = ref<string | null>(null)

  async function fetchCategories() {
    loading.value = true
    error.value = null
    try {
      categories.value = await get<CategoryDto[]>('/categories')
    } catch (e) {
      error.value = e instanceof Error ? e.message : String(e)
    } finally {
      loading.value = false
    }
  }

  return { categories, loading, error, fetchCategories }
})
