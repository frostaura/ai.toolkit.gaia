// Example Playwright Configuration for Gaia Integration Testing

// playwright.config.js
const { defineConfig, devices } = require('@playwright/test');

module.exports = defineConfig({
  // Test directory
  testDir: './tests',
  
  // Run tests in files in parallel
  fullyParallel: true,
  
  // Fail the build on CI if you accidentally left test.only in the source code
  forbidOnly: !!process.env.CI,
  
  // Retry on CI only
  retries: process.env.CI ? 2 : 0,
  
  // Opt out of parallel tests on CI
  workers: process.env.CI ? 1 : undefined,
  
  // Reporter configuration - MANDATORY: Use line reporter for Gaia
  reporter: 'line',
  
  // Shared settings for all projects
  use: {
    // Base URL for tests
    baseURL: 'http://localhost:3001', // Gaia standard frontend port
    
    // Collect trace when retrying the failed test
    trace: 'on-first-retry',
    
    // Screenshot on failure
    screenshot: 'only-on-failure',
    
    // MANDATORY: Enable console API to capture console messages
    launchOptions: {
      // Ensure console messages are captured
      devtools: false,
    },
  },

  // Configure projects for major browsers and viewports
  projects: [
    {
      name: 'chromium-desktop',
      use: { 
        ...devices['Desktop Chrome'],
        viewport: { width: 1024, height: 768 } // Gaia desktop standard
      },
    },
    {
      name: 'chromium-tablet',
      use: { 
        ...devices['Desktop Chrome'],
        viewport: { width: 768, height: 1024 } // Gaia tablet standard
      },
    },
    {
      name: 'chromium-mobile',
      use: { 
        ...devices['Desktop Chrome'],
        viewport: { width: 375, height: 667 } // Gaia mobile standard
      },
    },
    // Add Firefox and Safari for cross-browser testing if needed
    {
      name: 'firefox',
      use: { ...devices['Desktop Firefox'] },
    },
    {
      name: 'webkit',
      use: { ...devices['Desktop Safari'] },
    },
  ],

  // Run your local dev server before starting the tests
  webServer: {
    command: 'npm run dev',
    port: 3001, // Gaia standard frontend port
    reuseExistingServer: !process.env.CI,
    timeout: 120 * 1000, // 2 minutes timeout
  },

  // Global setup and teardown
  globalSetup: require.resolve('./tests/global-setup.js'),
  globalTeardown: require.resolve('./tests/global-teardown.js'),
  
  // Test timeout configuration
  timeout: 30 * 1000, // 30 seconds per test
  expect: {
    timeout: 5 * 1000, // 5 seconds for expect assertions
  },
});