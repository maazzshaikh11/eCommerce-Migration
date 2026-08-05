<script setup lang="ts">
import { ref, onMounted, computed } from 'vue'
import { useRoute } from 'vue-router'
import { useProductStore } from '../stores/productStore'
import { useCartStore } from '../stores/cartStore'
import { useAuthStore } from '../stores/useAuthStore'
import { formatINR } from '../utils/currency'

const route = useRoute()
const store = useProductStore()
const cartStore = useCartStore()
const authStore = useAuthStore()
const fmt = formatINR

const product = computed(() => store.currentProduct)
const quantity = ref(1)
const feedback = ref('')
const feedbackType = ref('')
const selectedVariantId = ref<number | undefined>(undefined)
const selectedImage = ref(0)

const allImages = computed(() => {
  const p = product.value
  if (!p) return []
  const imgs: string[] = []
  if (p.thumbnailUrl) imgs.push(p.thumbnailUrl)
  if (p.images?.length) {
    p.images.forEach(i => { if (i.imageUrl && i.imageUrl !== p.thumbnailUrl) imgs.push(i.imageUrl) })
  }
  return imgs.length ? imgs : ['']
})

const selectedVariant = computed(() => {
  const p = product.value
  if (!p?.variants?.length || !selectedVariantId.value) return null
  return p.variants.find(v => v.id === selectedVariantId.value) || p.variants[0]
})

const displayPrice = computed(() => {
  const p = product.value
  if (!p) return 0
  const adj = selectedVariant.value?.priceAdjustment || 0
  return p.price + adj
})

const inStock = computed(() => {
  const p = product.value
  if (!p) return false
  if (p.variants?.length) {
    return p.variants.some(v => v.stockQuantity > 0)
  }
  return true
})

const totalStock = computed(() => {
  const p = product.value
  if (!p?.variants?.length) return null
  return p.variants.reduce((sum: number, v: { stockQuantity: number }) => sum + v.stockQuantity, 0)
})

onMounted(() => {
  const id = Number(route.params.id)
  if (id) store.fetchProductDetail(id)
})

function selectImage(index: number) {
  selectedImage.value = index
}

async function addToCart() {
  if (!product.value) return
  if (!authStore.isAuthenticated) {
    feedback.value = 'Please sign in to add items to your cart.'
    feedbackType.value = 'warning'
    setTimeout(() => { feedback.value = '' }, 4000)
    return
  }
  try {
    await cartStore.addItem(product.value.id, quantity.value, selectedVariantId.value)
    feedback.value = `Added to cart. Cart total: ${fmt(cartStore.total)}`
    feedbackType.value = 'success'
    setTimeout(() => feedback.value = '', 4000)
  } catch (e) {
    feedback.value = cartStore.error || 'Failed to add to cart.'
    feedbackType.value = 'danger'
    setTimeout(() => feedback.value = '', 4000)
  }
}
</script>

<template>
  <div v-if="product">
    <nav aria-label="breadcrumb" class="container" style="padding-top:100px;">
      <ol class="breadcrumb">
        <li class="breadcrumb-item"><router-link to="/">Home</router-link></li>
        <li class="breadcrumb-item"><router-link to="/products">Shop</router-link></li>
        <li v-if="product.categoryName" class="breadcrumb-item">{{ product.categoryName }}</li>
        <li class="breadcrumb-item active" aria-current="page">{{ product.name }}</li>
      </ol>
    </nav>

    <div class="container section-pad" style="padding-top:40px;">
      <div class="rf-detail">
        <!-- Gallery -->
        <div>
          <div class="gallery-main">
            <img v-if="allImages[selectedImage]" :src="allImages[selectedImage]" :alt="product.name" />
            <div v-else class="ph"><svg xmlns="http://www.w3.org/2000/svg" width="64" height="64" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="1.5" stroke-linecap="round" stroke-linejoin="round"><rect width="18" height="18" x="3" y="3" rx="2" ry="2"/><circle cx="9" cy="9" r="2"/><path d="m21 15-3.086-3.086a2 2 0 0 0-2.828 0L6 21"/></svg></div>
          </div>
          <div v-if="allImages.length > 1" class="rf-detail-thumbs">
            <div v-for="(img, i) in allImages" :key="i" class="rf-thumb" :class="{ active: selectedImage === i }" @click="selectImage(i)">
              <img v-if="img" :src="img" :alt="String(i + 1)" />
            </div>
          </div>
        </div>

        <!-- Info -->
        <div class="rf-detail-info slide-up">
          <h1>{{ product.name }}</h1>
          <div class="rf-detail-price">{{ fmt(displayPrice) }}</div>

          <span v-if="inStock" class="pill success" style="margin-bottom:24px;">
            <svg xmlns="http://www.w3.org/2000/svg" width="14" height="14" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2" stroke-linecap="round" stroke-linejoin="round"><path d="M22 11.08V12a10 10 0 1 1-5.93-9.14"/><polyline points="22 4 12 14.01 9 11.01"/></svg>
            In Stock<span v-if="totalStock"> ({{ totalStock }} available)</span>
          </span>
          <span v-else class="pill danger" style="margin-bottom:24px;">Out of Stock</span>

          <p class="rf-detail-desc">{{ product.description || '' }}</p>

          <div class="rf-surface" style="margin-bottom:24px;">
            <!-- Variant Selector -->
            <div v-if="product.variants?.length" class="rf-field">
              <label>Variant</label>
              <select v-model="selectedVariantId" class="rf-select" style="max-width:320px;">
                <option v-for="variant in product.variants" :key="variant.id" :value="variant.id">{{ variant.variantName }}: {{ variant.variantValue }}</option>
              </select>
              <div v-if="selectedVariant" style="font-size:13px; color:var(--ink-2); margin-top:4px;">
                Stock: {{ selectedVariant.stockQuantity }}
                <span v-if="selectedVariant.priceAdjustment !== 0"> | Adjustment: {{ selectedVariant.priceAdjustment > 0 ? '+' : '' }}{{ fmt(selectedVariant.priceAdjustment) }}</span>
              </div>
            </div>

            <div class="rf-field">
              <label>Quantity</label>
              <div class="rf-qty">
                <button type="button" @click="quantity > 1 ? quantity-- : null"><svg xmlns="http://www.w3.org/2000/svg" width="14" height="14" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2" stroke-linecap="round" stroke-linejoin="round"><path d="M5 12h14"/></svg></button>
                <input v-model.number="quantity" type="number" min="1" max="99" />
                <button type="button" @click="quantity < 99 ? quantity++ : null"><svg xmlns="http://www.w3.org/2000/svg" width="14" height="14" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2" stroke-linecap="round" stroke-linejoin="round"><path d="M5 12h14"/><path d="M12 5v14"/></svg></button>
              </div>
            </div>

            <div style="display:flex; gap:8px; flex-wrap:wrap;">
              <button class="rf-btn" :disabled="!inStock" @click="addToCart"><span>
                <svg xmlns="http://www.w3.org/2000/svg" width="16" height="16" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2" stroke-linecap="round" stroke-linejoin="round" style="vertical-align:middle;margin-right:6px;"><path d="M6 2 3 6v14a2 2 0 0 0 2 2h14a2 2 0 0 0 2-2V6l-3-4Z"/><path d="M3 6h18"/><path d="M16 10a4 4 0 0 1-8 0"/></svg>
                {{ inStock ? 'Add to Cart' : 'Out of Stock' }}
              </span></button>
              <router-link to="/products" class="rf-btn outline"><span>Back</span></router-link>
            </div>
            <div v-if="feedback" class="alert mt-3" :class="`alert-${feedbackType}`" style="margin-top:12px;">{{ feedback }}</div>
          </div>

          <div style="display:grid; grid-template-columns: repeat(3, 1fr); gap: 12px; margin-top: 8px;">
            <div class="rf-surface text-center" style="padding:20px;">
              <svg xmlns="http://www.w3.org/2000/svg" width="28" height="28" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="1.5" stroke-linecap="round" stroke-linejoin="round" style="color:var(--accent);"><rect width="16" height="13" x="4" y="5" rx="2"/><path d="M16 2V5"/><path d="M8 2V5"/><path d="M4 10h16"/><path d="M8 14h.01"/><path d="M12 14h.01"/><path d="M16 14h.01"/><path d="M8 18h.01"/><path d="M12 18h.01"/><path d="M16 18h.01"/></svg>
              <p style="font-size:13px;font-weight:600;margin-top:8px;text-transform:uppercase;letter-spacing:0.1em;">Fast Shipping</p>
              <small class="text-ink-mute">2-5 days</small>
            </div>
            <div class="rf-surface text-center" style="padding:20px;">
              <svg xmlns="http://www.w3.org/2000/svg" width="28" height="28" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="1.5" stroke-linecap="round" stroke-linejoin="round" style="color:var(--accent);"><path d="M12 22s8-4 8-10V5l-8-3-8 3v7c0 6 8 10 8 10"/></svg>
              <p style="font-size:13px;font-weight:600;margin-top:8px;text-transform:uppercase;letter-spacing:0.1em;">Secure</p>
              <small class="text-ink-mute">256-bit SSL</small>
            </div>
            <div class="rf-surface text-center" style="padding:20px;">
              <svg xmlns="http://www.w3.org/2000/svg" width="28" height="28" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="1.5" stroke-linecap="round" stroke-linejoin="round" style="color:var(--accent);"><path d="M21 12a9 9 0 0 0-9-9 9.75 9.75 0 0 0-6.74 2.74L3 8"/><path d="M3 3v5h5"/><path d="M21 19a2 2 0 0 1-2 2h-1a2 2 0 0 1-2-2v-3a2 2 0 0 1 2-2h3a2 2 0 0 1 2 2v1a2 2 0 0 1-2 2z"/><path d="M3 19a2 2 0 0 0 2 2h1a2 2 0 0 0 2-2v-3a2 2 0 0 0-2-2H3a2 2 0 0 0-2 2v1a2 2 0 0 0 2 2z"/></svg>
              <p style="font-size:13px;font-weight:600;margin-top:8px;text-transform:uppercase;letter-spacing:0.1em;">Returns</p>
              <small class="text-ink-mute">30-day</small>
            </div>
          </div>
        </div>
      </div>
    </div>
  </div>
  <div v-else class="rf-empty" style="padding-top:120px;">
    <h3>Product not found</h3>
    <router-link to="/products" class="rf-btn"><span>Back to Shop</span></router-link>
  </div>
</template>

<style scoped>
.rf-detail-thumbs { display: flex; gap: 8px; margin-top: 12px; }
.rf-thumb { width: 64px; height: 64px; border: 1px solid var(--line); cursor: pointer; overflow: hidden; opacity: 0.6; transition: opacity 0.2s; }
.rf-thumb.active { border-color: var(--accent); opacity: 1; }
.rf-thumb:hover { opacity: 1; }
.rf-thumb img { width: 100%; height: 100%; object-fit: cover; }
</style>
