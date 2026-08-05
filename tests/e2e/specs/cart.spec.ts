import { test, expect } from '@playwright/test'

test.describe('Cart Flow', () => {
  test('cart page loads', async ({ page }) => {
    await page.goto('/cart')
    await expect(page).toHaveURL(/\/cart/)
    await expect(page.locator('h1')).toContainText('Shopping Cart')
  })
})
