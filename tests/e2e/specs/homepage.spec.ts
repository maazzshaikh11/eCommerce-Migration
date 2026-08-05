import { test, expect } from '@playwright/test'

test.describe('Homepage', () => {
  test('homepage loads', async ({ page }) => {
    await page.goto('/')
    await expect(page).toHaveURL('/')
    await expect(page.locator('h1')).toContainText('Welcome to Ecommerce')
  })
})
