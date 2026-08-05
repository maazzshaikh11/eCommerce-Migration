import { test, expect } from '@playwright/test'

test.describe('Checkout Flow', () => {
  test('checkout page loads', async ({ page }) => {
    await page.goto('/checkout')
    await expect(page).toHaveURL(/\/checkout/)
    await expect(page.locator('h1')).toContainText('Checkout')
  })
})
