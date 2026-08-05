<script setup lang="ts">
import { ref, onMounted, computed } from 'vue'
import { useRouter } from 'vue-router'
import { useCheckoutStore } from '../stores/checkoutStore'
import { useCartStore } from '../stores/cartStore'
import { post } from '../api/client'
import { formatINR } from '../utils/currency'

const router = useRouter()
const store = useCheckoutStore()
const cartStore = useCartStore()
const fmt = formatINR

const step = ref(1)
const showAddressForm = ref(false)
const newAddress = ref({ streetAddress: '', city: '', stateProvince: '', postalCode: '', country: 'India' })

const payment = ref({ cardholderName: '', cardNumber: '', expirationDate: '', cvv: '' })
const paymentError = ref('')

const cartSubTotal = computed(() => cartStore.items.reduce((s, i) => s + i.lineTotal, 0))
const shippingCost = computed(() => {
  const opt = store.shippingOptions.find(o => o.method === store.selectedShippingMethod)
  return opt ? opt.cost : 99
})
const taxTotal = computed(() => (cartSubTotal.value + shippingCost.value) * 0.18)
const grandTotal = computed(() => cartSubTotal.value + shippingCost.value + taxTotal.value)

onMounted(() => {
  store.generateIdempotencyKey()
  store.fetchAddresses()
  store.fetchShippingOptions()
  cartStore.fetchCart()
})

function nextStep() {
  store.error = null
  paymentError.value = ''
  if (step.value === 1 && !store.selectedAddressId) {
    store.error = 'Please select a shipping address.'
    return
  }
  if (step.value === 3) {
    if (!payment.value.cardholderName || !payment.value.cardNumber || !payment.value.expirationDate || !payment.value.cvv) {
      paymentError.value = 'Please fill in all payment fields.'
      return
    }
    if (!/^\d{16}$/.test(payment.value.cardNumber.replace(/\s/g, ''))) {
      paymentError.value = 'Card number must be 16 digits.'
      return
    }
    if (!/^(0[1-9]|1[0-2])\/\d{2}$/.test(payment.value.expirationDate)) {
      paymentError.value = 'Expiration must be MM/YY.'
      return
    }
    if (!/^\d{3,4}$/.test(payment.value.cvv)) {
      paymentError.value = 'CVV must be 3 or 4 digits.'
      return
    }
  }
  step.value++
}

function prevStep() {
  store.error = null
  paymentError.value = ''
  step.value--
}

async function createAddress() {
  store.error = null
  try {
    const created = await post<{ id: number }>('/checkout/addresses', newAddress.value)
    await store.fetchAddresses()
    store.selectedAddressId = created.id
    showAddressForm.value = false
    newAddress.value = { streetAddress: '', city: '', stateProvince: '', postalCode: '', country: 'India' }
  } catch (e) {
    store.error = e instanceof Error ? e.message : String(e)
  }
}

async function submitOrder() {
  await store.placeOrder()
  if (store.confirmation) {
    router.push(`/checkout/confirmation/${store.confirmation.orderId}`)
  }
}
</script>

<template>
  <div class="container" style="padding-top:120px; padding-bottom:80px;">
    <div class="rf-steps">
      <div class="rf-step" :class="{ active: step >= 1, done: step > 1 }"><div class="n">1</div><div class="l">Address</div></div>
      <div class="rf-step-conn" :class="{ done: step > 1 }"></div>
      <div class="rf-step" :class="{ active: step >= 2, done: step > 2 }"><div class="n">2</div><div class="l">Shipping</div></div>
      <div class="rf-step-conn" :class="{ done: step > 2 }"></div>
      <div class="rf-step" :class="{ active: step >= 3, done: step > 3 }"><div class="n">3</div><div class="l">Payment</div></div>
      <div class="rf-step-conn" :class="{ done: step > 3 }"></div>
      <div class="rf-step" :class="{ active: step >= 4, done: step > 4 }"><div class="n">4</div><div class="l">Review</div></div>
      <div class="rf-step-conn" :class="{ done: step > 4 }"></div>
      <div class="rf-step" :class="{ active: step >= 5 }"><div class="n">5</div><div class="l">Confirm</div></div>
    </div>

    <!-- Step 1: Address -->
    <div v-if="step === 1" style="text-align:center;">
      <h2 style="font-size:clamp(2rem,5vw,3rem); text-transform:uppercase; margin-bottom:32px;">Select Shipping Address</h2>
      <div v-if="store.addresses.length === 0 && !showAddressForm" class="rf-empty">
        <div class="ic"><svg xmlns="http://www.w3.org/2000/svg" width="64" height="64" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="1.5" stroke-linecap="round" stroke-linejoin="round"><path d="M20 10c0 6-8 12-8 12s-8-6-8-12a8 8 0 0 1 16 0Z"/><circle cx="12" cy="10" r="3"/></svg></div>
        <h3>No saved addresses</h3>
        <p>Add an address to continue.</p>
        <button class="rf-btn" @click="showAddressForm = true"><span>Add Address</span></button>
      </div>
      <div v-else-if="showAddressForm" class="rf-surface" style="max-width:600px; margin:0 auto; text-align:left;">
        <h6 style="font-family:'Clash Display',sans-serif; font-size:1.1rem; text-transform:uppercase; margin-bottom:20px;">New Address</h6>
        <div class="rf-field"><label>Street Address</label><input v-model="newAddress.streetAddress" class="rf-input" placeholder="123 Main St" /></div>
        <div style="display:grid; grid-template-columns: 1fr 1fr; gap: 16px;">
          <div class="rf-field"><label>City</label><input v-model="newAddress.city" class="rf-input" placeholder="City" /></div>
          <div class="rf-field"><label>State/Province</label><input v-model="newAddress.stateProvince" class="rf-input" placeholder="State" /></div>
        </div>
        <div style="display:grid; grid-template-columns: 1fr 1fr; gap: 16px;">
          <div class="rf-field"><label>Postal Code</label><input v-model="newAddress.postalCode" class="rf-input" placeholder="12345" /></div>
          <div class="rf-field"><label>Country</label><input v-model="newAddress.country" class="rf-input" placeholder="India" /></div>
        </div>
        <div style="display:flex; gap:12px; margin-top:16px;">
          <button class="rf-btn outline" @click="showAddressForm = false"><span>Cancel</span></button>
          <button class="rf-btn" @click="createAddress"><span>Save Address</span></button>
        </div>
      </div>
      <div v-else style="max-width:600px; margin:0 auto; text-align:left;" class="d-flex flex-column gap-2">
        <div v-for="addr in store.addresses" :key="addr.id" class="ship-option" :class="{ selected: store.selectedAddressId === addr.id }" @click="store.selectedAddressId = addr.id">
          <div style="font-weight:600; text-transform:uppercase; font-size:13px; letter-spacing:0.1em;">{{ addr.streetAddress }}</div>
          <div style="font-size:14px; color:var(--ink-2); margin-top:4px;">{{ addr.city }}, {{ addr.stateProvince }} {{ addr.postalCode }}, {{ addr.country }}</div>
        </div>
        <div style="margin-top:16px;">
          <button class="rf-btn outline" @click="showAddressForm = true"><span>+ Add New Address</span></button>
        </div>
      </div>
      <div v-if="!showAddressForm" style="margin-top:32px;">
        <button class="rf-btn" :disabled="!store.selectedAddressId" @click="nextStep"><span>Continue to Shipping</span></button>
      </div>
    </div>

    <!-- Step 2: Shipping -->
    <div v-if="step === 2" style="text-align:center;">
      <h2 style="font-size:clamp(2rem,5vw,3rem); text-transform:uppercase; margin-bottom:32px;">Shipping Method</h2>
      <div class="d-flex flex-column gap-2" style="max-width:600px; margin:0 auto; text-align:left;">
        <div v-for="opt in store.shippingOptions" :key="opt.method" class="ship-option" :class="{ selected: store.selectedShippingMethod === opt.method }" @click="store.selectedShippingMethod = opt.method">
          <div style="display:flex; justify-content:space-between; align-items:center;">
            <div style="font-weight:600; text-transform:uppercase; font-size:13px; letter-spacing:0.1em;">{{ opt.label }}</div>
            <div style="font-family:'Clash Display',sans-serif; font-weight:600;">{{ fmt(opt.cost) }}</div>
          </div>
        </div>
      </div>
      <div style="margin-top:32px; display:flex; gap:12px; justify-content:center;">
        <button class="rf-btn outline" @click="prevStep"><span>Back</span></button>
        <button class="rf-btn" @click="nextStep"><span>Continue to Payment</span></button>
      </div>
    </div>

    <!-- Step 3: Payment -->
    <div v-if="step === 3" style="text-align:center;">
      <h2 style="font-size:clamp(2rem,5vw,3rem); text-transform:uppercase; margin-bottom:32px;">Payment</h2>
      <div class="rf-surface" style="max-width:600px; margin:0 auto; text-align:left;">
        <h6 style="font-family:'Clash Display',sans-serif; font-size:1.1rem; text-transform:uppercase; margin-bottom:20px;">Card Details</h6>
        <div class="rf-field"><label>Cardholder Name</label><input v-model="payment.cardholderName" class="rf-input" placeholder="John Doe" /></div>
        <div class="rf-field"><label>Card Number</label><input v-model="payment.cardNumber" class="rf-input" placeholder="1234567890123456" maxlength="16" /></div>
        <div style="display:grid; grid-template-columns: 1fr 1fr; gap: 16px;">
          <div class="rf-field"><label>Expiration (MM/YY)</label><input v-model="payment.expirationDate" class="rf-input" placeholder="MM/YY" maxlength="5" /></div>
          <div class="rf-field"><label>CVV</label><input v-model="payment.cvv" class="rf-input" placeholder="123" maxlength="4" /></div>
        </div>
        <div v-if="paymentError" class="alert alert-danger" style="margin-top:12px;">{{ paymentError }}</div>
        <div style="display:flex; gap:12px; margin-top:24px;">
          <button class="rf-btn outline" @click="prevStep"><span>Back</span></button>
          <button class="rf-btn" @click="nextStep"><span>Continue to Review</span></button>
        </div>
      </div>
    </div>

    <!-- Step 4: Review -->
    <div v-if="step === 4" style="text-align:center;">
      <h2 style="font-size:clamp(2rem,5vw,3rem); text-transform:uppercase; margin-bottom:32px;">Review Order</h2>
      <div class="rf-surface" style="max-width:700px; margin:0 auto 24px; text-align:left;">
        <h6 style="font-family:'Clash Display',sans-serif; font-size:1.1rem; text-transform:uppercase; margin-bottom:16px;">Shipping Address</h6>
        <div v-if="store.selectedAddressId">
          <div v-for="addr in store.addresses" :key="addr.id">
            <div v-if="addr.id === store.selectedAddressId" style="color:var(--ink-2);">
              <strong style="color:var(--ink);">{{ addr.streetAddress }}</strong><br />
              {{ addr.city }}, {{ addr.stateProvince }} {{ addr.postalCode }}<br />
              {{ addr.country }}
            </div>
          </div>
        </div>
      </div>
      <div class="rf-surface" style="max-width:700px; margin:0 auto 24px; text-align:left;">
        <h6 style="font-family:'Clash Display',sans-serif; font-size:1.1rem; text-transform:uppercase; margin-bottom:16px;">Shipping Method</h6>
        <p style="color:var(--ink-2); margin:0;">
          {{ store.shippingOptions.find(o => o.method === store.selectedShippingMethod)?.label || 'Standard Shipping' }}
        </p>
      </div>
      <div class="rf-surface" style="max-width:700px; margin:0 auto 24px; text-align:left;">
        <h6 style="font-family:'Clash Display',sans-serif; font-size:1.1rem; text-transform:uppercase; margin-bottom:16px;">Items</h6>
        <div v-for="item in cartStore.items" :key="item.id" style="display:flex; justify-content:space-between; padding:12px 0; border-bottom:1px solid var(--line);">
          <span>{{ item.productName }} x{{ item.quantity }}</span>
          <span style="font-weight:600;">{{ fmt(item.lineTotal) }}</span>
        </div>
        <div style="display:flex; justify-content:space-between; padding:12px 0; border-bottom:1px solid var(--line);">
          <span>Subtotal</span><span style="font-weight:600;">{{ fmt(cartSubTotal) }}</span>
        </div>
        <div style="display:flex; justify-content:space-between; padding:12px 0; border-bottom:1px solid var(--line);">
          <span>Shipping</span><span style="font-weight:600;">{{ fmt(shippingCost) }}</span>
        </div>
        <div style="display:flex; justify-content:space-between; padding:12px 0; border-bottom:1px solid var(--line);">
          <span>GST (18%)</span><span style="font-weight:600;">{{ fmt(taxTotal) }}</span>
        </div>
        <div style="display:flex; justify-content:space-between; padding:16px 0 0; font-family:'Clash Display',sans-serif; font-size:1.2rem; font-weight:600;">
          <span>Grand Total</span>
          <span>{{ fmt(grandTotal) }}</span>
        </div>
      </div>
      <div style="display:flex; gap:12px; justify-content:center;">
        <button class="rf-btn outline" @click="prevStep"><span>Back</span></button>
        <button class="rf-btn" @click="nextStep"><span>Place Order</span></button>
      </div>
    </div>

    <!-- Step 5: Processing -->
    <div v-if="step === 5" style="text-align:center;">
      <div v-if="store.loading" class="rf-empty">
        <div class="ic"><svg xmlns="http://www.w3.org/2000/svg" width="64" height="64" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="1.5" stroke-linecap="round" stroke-linejoin="round"><path d="M12 2v4"/><path d="m5 5 2.83 2.83"/><path d="m19 5-2.83 2.83"/><path d="M12 14a4 4 0 1 0 0-8 4 4 0 0 0 0 8Z"/><path d="M12 22v-4"/><path d="m5 19 2.83-2.83"/><path d="m19 19-2.83-2.83"/></svg></div>
        <h3>Processing...</h3>
      </div>
      <div v-else-if="store.error" class="alert alert-danger">{{ store.error }}</div>
      <div v-else-if="store.confirmation" class="rf-empty">
        <div class="ic"><svg xmlns="http://www.w3.org/2000/svg" width="64" height="64" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="1.5" stroke-linecap="round" stroke-linejoin="round"><path d="M22 11.08V12a10 10 0 1 1-5.93-9.14"/><polyline points="22 4 12 14.01 9 11.01"/></svg></div>
        <h3>Order Placed!</h3>
        <p>Order #{{ store.confirmation.orderNumber }}</p>
        <router-link to="/orders" class="rf-btn"><span>View Orders</span></router-link>
      </div>
      <div v-else>
        <button class="rf-btn" @click="submitOrder"><span>Confirm &amp; Place Order</span></button>
      </div>
    </div>
  </div>
</template>
