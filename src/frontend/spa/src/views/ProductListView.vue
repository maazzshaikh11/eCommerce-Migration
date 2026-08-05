<script setup lang="ts">
import { onMounted, watch, ref } from 'vue'
import { useRoute, useRouter } from 'vue-router'
import { useProductStore } from '../stores/productStore'
import { useCategoryStore } from '../stores/categoryStore'
import AddToCartButton from '../components/AddToCartButton.vue'
import { formatINR } from '../utils/currency'

const route = useRoute()
const router = useRouter()
const store = useProductStore()
const catStore = useCategoryStore()
const fmt = formatINR

const searchKeyword = ref('')
const minPrice = ref('')
const maxPrice = ref('')
const sortBy = ref('Newest')
const pageSize = ref(12)
const expandedIds = ref<Set<number>>(new Set())

onMounted(() => {
  catStore.fetchCategories()
  applyRouteQuery()
})

watch(() => route.query, () => applyRouteQuery(), { deep: true })

function applyRouteQuery() {
  const q = route.query
  store.categoryId = q.categoryId ? Number(q.categoryId) : null
  store.searchQuery = (q.search as string) || ''
  searchKeyword.value = store.searchQuery
  store.sortBy = (q.sortBy as string) || 'Newest'
  sortBy.value = store.sortBy
  store.pageSize = q.pageSize ? Number(q.pageSize) : 12
  pageSize.value = store.pageSize
  store.fetchProducts(q.page ? Number(q.page) : 1)
}

function applyFilters(page = 1) {
  const query: Record<string, string | number | undefined> = {}
  if (store.categoryId != null) query.categoryId = store.categoryId
  if (searchKeyword.value) query.search = searchKeyword.value
  if (sortBy.value !== 'Newest') query.sortBy = sortBy.value
  if (pageSize.value !== 12) query.pageSize = pageSize.value
  if (page > 1) query.page = page
  router.push({ path: '/products', query })
}

function clearFilters() {
  searchKeyword.value = ''
  minPrice.value = ''
  maxPrice.value = ''
  sortBy.value = 'Newest'
  pageSize.value = 12
  store.categoryId = null
  applyFilters(1)
}

function selectCategory(id: number | null) {
  store.categoryId = id
  applyFilters(1)
}

function setPage(page: number) {
  applyFilters(page)
}

function toggleExpand(id: number) {
  if (expandedIds.value.has(id)) expandedIds.value.delete(id)
  else expandedIds.value.add(id)
}

function isExpanded(id: number) { return expandedIds.value.has(id) }

function activeCategoryName(): string {
  return findName(catStore.categories, store.categoryId) || 'SHOP'
}

function findName(list: typeof catStore.categories, id: number | null): string | undefined {
  if (id == null) return undefined
  for (const c of list) {
    if (c.id === id) return c.name
    if (c.children) {
      const found = findName(c.children, id)
      if (found) return found
    }
  }
  return undefined
}

function renderTree(list: typeof catStore.categories, depth = 0) {
  return list.map(c => ({
    id: c.id,
    name: c.name,
    depth,
    children: c.children || [],
  }))
}
</script>

<template>
  <div>
    <nav aria-label="breadcrumb" class="container" style="padding-top:100px;">
      <ol class="breadcrumb">
        <li class="breadcrumb-item"><router-link to="/">Home</router-link></li>
        <li class="breadcrumb-item active">Shop</li>
        <li v-if="store.categoryId" class="breadcrumb-item active" aria-current="page">{{ activeCategoryName() }}</li>
      </ol>
    </nav>

    <section class="rf-divider" style="padding:64px 0;">
      <h2>{{ activeCategoryName().toUpperCase() }}</h2>
    </section>

    <div class="container" style="padding-bottom:80px;">
      <div style="display:grid; grid-template-columns: 250px 1fr; gap: 48px;">
        <!-- Filters -->
        <aside>
          <div class="rf-surface" style="margin-bottom:24px;">
            <h6 style="font-family:'Clash Display',sans-serif;font-size:14px;text-transform:uppercase;letter-spacing:0.12em;margin-bottom:16px;">Categories</h6>
            <div class="d-flex flex-column gap-1">
              <a href="#" class="cat-link" :class="{ active: store.categoryId == null }" @click.prevent="selectCategory(null)"><span>All Products</span></a>
              <template v-for="c in renderTree(catStore.categories)" :key="c.id">
                <div>
                  <a href="#" class="cat-link" :class="{ active: store.categoryId === c.id }" :style="{ paddingLeft: `${8 + c.depth * 12}px` }" @click.prevent="selectCategory(c.id)"><span>{{ c.name }}</span></a>
                  <button v-if="c.children.length" class="tree-toggle" @click.prevent.stop="toggleExpand(c.id)">
                    {{ isExpanded(c.id) ? '-' : '+' }}
                  </button>
                </div>
                <div v-if="c.children.length && isExpanded(c.id)" class="tree-children">
                  <a v-for="child in renderTree(c.children, c.depth + 1)" :key="child.id" href="#" class="cat-link" :class="{ active: store.categoryId === child.id }" :style="{ paddingLeft: `${8 + child.depth * 12}px` }" @click.prevent="selectCategory(child.id)"><span>{{ child.name }}</span></a>
                </div>
              </template>
            </div>
          </div>

          <div class="rf-surface">
            <h6 style="font-family:'Clash Display',sans-serif;font-size:14px;text-transform:uppercase;letter-spacing:0.12em;margin-bottom:20px;">Filters</h6>
            <div class="rf-field">
              <label>Search</label>
              <input v-model="searchKeyword" type="text" class="rf-input" placeholder="Keywords..." />
            </div>
            <div class="rf-field">
              <label>Sort By</label>
              <select v-model="sortBy" class="rf-select">
                <option value="Newest">Newest</option>
                <option value="price_asc">Price: Low to High</option>
                <option value="price_desc">Price: High to Low</option>
                <option value="name_asc">Name: A-Z</option>
              </select>
            </div>
            <div class="rf-field">
              <label>Per Page</label>
              <select v-model="pageSize" class="rf-select">
                <option :value="12">12</option>
                <option :value="24">24</option>
                <option :value="48">48</option>
              </select>
            </div>
            <div style="display:grid; gap:8px;">
              <button class="rf-btn sm" @click="applyFilters(1)"><span>Apply</span></button>
              <button class="rf-btn sm outline" @click="clearFilters"><span>Clear</span></button>
            </div>
          </div>
        </aside>

        <!-- Grid -->
        <div>
          <div v-if="store.loading" class="text-center" style="padding:48px 0;">
            <div style="color:var(--accent);">Loading...</div>
          </div>
          <div v-else-if="store.error" class="alert alert-danger">{{ store.error }}</div>
          <div v-else-if="store.items.length === 0" class="rf-empty">
            <div class="ic"><svg xmlns="http://www.w3.org/2000/svg" width="64" height="64" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="1.5" stroke-linecap="round" stroke-linejoin="round"><circle cx="11" cy="11" r="8"/><path d="m21 21-4.3-4.3"/><path d="M8 8h.01"/></svg></div>
            <h3>No products found</h3>
            <p>Try adjusting your filters or search terms.</p>
          </div>
          <div v-else class="rf-grid fade-row">
            <div v-for="p in store.items" :key="p.id" class="rf-product slide-up">
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

          <!-- Pagination -->
          <nav v-if="store.totalPages > 1" style="margin-top:40px;">
            <ul class="pagination" style="display:flex; justify-content:center; gap:6px; list-style:none;">
              <li>
                <button class="page-link" :disabled="store.currentPage === 1" @click="setPage(store.currentPage - 1)">&laquo;</button>
              </li>
              <li v-for="page in store.totalPages" :key="page">
                <button class="page-link" :class="{ active: store.currentPage === page }" @click="setPage(page)">{{ page }}</button>
              </li>
              <li>
                <button class="page-link" :disabled="store.currentPage === store.totalPages" @click="setPage(store.currentPage + 1)">&raquo;</button>
              </li>
            </ul>
          </nav>
        </div>
      </div>
    </div>
  </div>
</template>

<style scoped>
.cat-link { display: block; padding: 6px 0; font-size: 14px; color: var(--ink-2); transition: color .2s; }
.cat-link:hover, .cat-link.active { color: var(--accent); }
.page-link { border: 1px solid var(--line); color: var(--ink); background: var(--base); padding: 8px 14px; cursor: pointer; font-family: 'Satoshi', sans-serif; }
.page-link.active { background: var(--ink); border-color: var(--ink); color: var(--base); }
.page-link:disabled { color: var(--ink-faint); background: var(--paper); cursor: not-allowed; }
.tree-toggle { display: inline-flex; align-items: center; justify-content: center; width: 20px; height: 20px; margin-left: 6px; border: 1px solid var(--line); background: var(--paper); color: var(--ink); font-size: 12px; cursor: pointer; }
.tree-children { display: flex; flex-direction: column; gap: 2px; }
@media (max-width: 991px) {
  aside { display: none; }
}
</style>
