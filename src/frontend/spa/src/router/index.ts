import { createRouter, createWebHistory } from 'vue-router'
import HomeView from '../views/HomeView.vue'
import ProductListView from '../views/ProductListView.vue'
import ProductDetailView from '../views/ProductDetailView.vue'
import CartView from '../views/CartView.vue'
import CheckoutWizard from '../views/CheckoutWizard.vue'
import OrderConfirmationView from '../views/OrderConfirmationView.vue'
import OrderListView from '../views/OrderListView.vue'
import OrderDetailView from '../views/OrderDetailView.vue'

import LoginView from '../views/LoginView.vue'
import RegisterView from '../views/RegisterView.vue'
import ProfileView from '../views/ProfileView.vue'
import AccountDashboard from '../views/AccountDashboard.vue'

const router = createRouter({
  history: createWebHistory(),
  routes: [
    { path: '/', name: 'home', component: HomeView },
    { path: '/products', name: 'products', component: ProductListView },
    { path: '/product/:id', name: 'product-detail', component: ProductDetailView },
    { path: '/products/:id', name: 'product-detail-alt', component: ProductDetailView },
    { path: '/cart', name: 'cart', component: CartView },
    { path: '/checkout', name: 'checkout', component: CheckoutWizard },
    { path: '/checkout/confirmation/:orderId', name: 'order-confirmation', component: OrderConfirmationView },
    { path: '/orders', name: 'orders', component: OrderListView },
    { path: '/orders/:id', name: 'order-detail', component: OrderDetailView },
    { path: '/login', name: 'login', component: LoginView },
    { path: '/register', name: 'register', component: RegisterView },
    { path: '/profile', name: 'profile', component: ProfileView },
    { path: '/account', name: 'account', component: AccountDashboard },
  ],
})

export default router
