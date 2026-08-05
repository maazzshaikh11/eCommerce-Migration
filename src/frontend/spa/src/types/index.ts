export interface PagedResult<T> {
  items: T[]
  currentPage: number
  pageSize: number
  totalCount: number
  totalPages: number
}

export interface ProductListResponseDto {
  id: number
  name: string
  description?: string
  price: number
  thumbnailUrl?: string
  categoryName?: string
}

export interface ProductDetailDto {
  id: number
  name: string
  description: string
  price: number
  imageUrl: string
  thumbnailUrl: string
  isActive: boolean
  createdDate: string
  categoryName?: string
  images: ProductImageDto[]
  variants: ProductVariantDto[]
}

export interface ProductImageDto {
  id: number
  imageUrl: string
  isPrimary: boolean
  displayOrder: number
}

export interface ProductVariantDto {
  id: number
  variantName: string
  variantValue: string
  priceAdjustment: number
  stockQuantity: number
}

export interface CategoryDto {
  id: number
  name: string
  description?: string
  thumbnailUrl?: string
  parentCategoryId?: number
  children?: CategoryDto[]
}

export interface CartItemDto {
  id: number
  productId: number
  productName: string
  variantName?: string
  imageUrl?: string
  unitPrice: number
  quantity: number
  lineTotal: number
}

export interface CartSummaryDto {
  itemCount: number
  subTotal: number
  total: number
}

export interface AddressDto {
  id: number
  streetAddress: string
  city: string
  stateProvince?: string
  postalCode?: string
  country: string
  isDefault: boolean
}

export interface OrderItemDto {
  productName: string
  quantity: number
  unitPrice: number
  lineTotal: number
}

export interface OrderDto {
  id: number
  orderNumber: string
  orderDate: string
  orderStatus: string
  subTotal: number
  shippingTotal: number
  taxTotal: number
  grandTotal: number
}

export interface OrderConfirmationDto {
  orderId: number
  orderNumber: string
  orderDate: string
  orderStatus: string
  subTotal: number
  shippingTotal: number
  taxTotal: number
  grandTotal: number
  shippingAddressFormatted?: string
  items: OrderItemDto[]
}

export interface ShippingOptionDto {
  method: string
  label: string
  cost: number
}
