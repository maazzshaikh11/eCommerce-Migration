import { test, expect } from '@playwright/test'

test.describe('Order History', () => {
  test('order list page loads', async ({ page }) => {
    await page.goto('/orders')
    await expect(page).toHaveURL(/\/orders/)
    await expect(page.locator('h1')).toContainText('Order History')
  })
})
