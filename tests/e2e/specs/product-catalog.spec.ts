import { test, expect } from '@playwright/test'

test.describe('Product Catalog', () => {
  test('product list page loads', async ({ page }) => {
    await page.goto('/products')
    await expect(page.locator('h1')).toContainText('Products')
  })

  test('product detail page loads', async ({ page }) => {
    await page.goto('/product/1')
    await expect(page).toHaveURL(/\/product\/1/)
  })
})
