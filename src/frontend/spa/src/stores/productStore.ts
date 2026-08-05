import { ref } from 'vue'
import { defineStore } from 'pinia'
import { get } from '../api/client'
import type { PagedResult, ProductListResponseDto, ProductDetailDto } from '../types'

export const useProductStore = defineStore('product', () => {
  const items = ref<ProductListResponseDto[]>([])
  const currentProduct = ref<ProductDetailDto | null>(null)
  const currentPage = ref(1)
  const pageSize = ref(12)
  const totalCount = ref(0)
  const totalPages = ref(0)
  const loading = ref(false)
  const error = ref<string | null>(null)

  const categoryId = ref<number | null>(null)
  const searchQuery = ref('')
  const sortBy = ref('')

  async function fetchProducts(page = 1) {
    loading.value = true
    error.value = null
    try {
      const params = new URLSearchParams()
      params.set('page', String(page))
      params.set('pageSize', String(pageSize.value))
      if (categoryId.value != null) params.set('categoryId', String(categoryId.value))
      if (searchQuery.value) params.set('searchQuery', searchQuery.value)
      if (sortBy.value) params.set('sortBy', sortBy.value)
      const result = await get<PagedResult<ProductListResponseDto>>(`/products?${params}`)
      items.value = result.items
      currentPage.value = result.currentPage
      totalCount.value = result.totalCount
      totalPages.value = result.totalPages
    } catch (e) {
      error.value = e instanceof Error ? e.message : String(e)
    } finally {
      loading.value = false
    }
  }

  async function fetchProductDetail(id: number) {
    loading.value = true
    error.value = null
    try {
      currentProduct.value = await get<ProductDetailDto>(`/products/${id}`)
    } catch (e) {
      error.value = e instanceof Error ? e.message : String(e)
      currentProduct.value = null
    } finally {
      loading.value = false
    }
  }

  return {
    items, currentProduct, currentPage, pageSize, totalCount, totalPages, loading, error,
    categoryId, searchQuery, sortBy,
    fetchProducts, fetchProductDetail
  }
})
