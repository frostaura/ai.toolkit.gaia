// Example Playwright Integration Test Configuration
// This demonstrates the enhanced integration testing requirements for Gaia

const { test, expect } = require('@playwright/test');

// Enhanced test configuration with console error monitoring
test.describe('Gaia Integration Testing Example', () => {
  let consoleErrors = [];
  let consoleWarnings = [];
  let networkErrors = [];

  test.beforeEach(async ({ page }) => {
    // Reset error collections
    consoleErrors = [];
    consoleWarnings = [];
    networkErrors = [];

    // MANDATORY: Monitor console errors
    page.on('console', msg => {
      if (msg.type() === 'error') {
        consoleErrors.push(msg.text());
        console.log('❌ Console Error:', msg.text());
      }
      if (msg.type() === 'warning') {
        consoleWarnings.push(msg.text());
        console.log('⚠️ Console Warning:', msg.text());
      }
    });

    // MANDATORY: Monitor JavaScript exceptions
    page.on('pageerror', exception => {
      consoleErrors.push(`JavaScript Exception: ${exception.message}`);
      console.log('💥 JS Exception:', exception.message);
    });

    // MANDATORY: Monitor network failures
    page.on('response', response => {
      if (!response.ok() && !response.url().includes('favicon.ico')) {
        networkErrors.push(`Network Error: ${response.url()} - ${response.status()}`);
        console.log('🌐 Network Error:', response.url(), response.status());
      }
    });
  });

  test.afterEach(async ({ page }) => {
    // MANDATORY: Fail test if ANY console errors detected
    if (consoleErrors.length > 0) {
      throw new Error(`Console errors detected: ${consoleErrors.join(', ')}`);
    }
    
    // Log warnings but don't fail (configurable based on project needs)
    if (consoleWarnings.length > 0) {
      console.log('⚠️ Console warnings detected:', consoleWarnings.join(', '));
    }
    
    // MANDATORY: Fail test if network errors detected
    if (networkErrors.length > 0) {
      throw new Error(`Network errors detected: ${networkErrors.join(', ')}`);
    }
  });

  // Example comprehensive integration test
  test('Complete visual and functional validation', async ({ page }) => {
    const viewports = [
      { name: 'mobile', width: 375, height: 667 },
      { name: 'tablet', width: 768, height: 1024 },
      { name: 'desktop', width: 1024, height: 768 }
    ];

    for (const viewport of viewports) {
      console.log(`🖥️ Testing ${viewport.name} viewport (${viewport.width}x${viewport.height})`);
      
      // Set viewport
      await page.setViewportSize({ width: viewport.width, height: viewport.height });
      
      // Navigate to page
      await page.goto('http://localhost:3001'); // Standard Gaia frontend port
      
      // Wait for page to be fully loaded
      await page.waitForLoadState('networkidle');
      
      // Take screenshot for beauty analysis
      const screenshotPath = `analysis-home-${viewport.name}.png`;
      await page.screenshot({ 
        path: screenshotPath, 
        fullPage: true 
      });
      console.log(`📸 Screenshot captured: ${screenshotPath}`);
      
      // BEAUTY ANALYSIS CHECKLIST (to be performed manually or with AI analysis)
      console.log('🎨 Beauty Analysis Criteria for', viewport.name, ':');
      console.log('  - Visual Hierarchy: Clear information hierarchy and content flow');
      console.log('  - Spacing & Alignment: Consistent spacing, perfect alignment');
      console.log('  - Typography: Minimum 14px font size, proper line-height');
      console.log('  - Color & Contrast: 4.5:1 minimum contrast ratio');
      console.log('  - Component States: All interactive states working');
      console.log('  - Responsive Behavior: Smooth breakpoint transitions');
      console.log('  - Professional Polish: Zero unstyled components');
      console.log('  - Accessibility: Proper ARIA labels and keyboard navigation');
      console.log('  - Performance: Fast loading, smooth animations');
      
      // Test interactive elements
      const buttons = await page.locator('button').all();
      for (const button of buttons) {
        // Test hover state
        await button.hover();
        await page.screenshot({ 
          path: `analysis-button-hover-${viewport.name}.png` 
        });
        
        // Test focus state
        await button.focus();
        await page.screenshot({ 
          path: `analysis-button-focus-${viewport.name}.png` 
        });
      }
      
      // Test form inputs if present
      const inputs = await page.locator('input').all();
      for (const input of inputs) {
        await input.focus();
        await page.screenshot({ 
          path: `analysis-input-focus-${viewport.name}.png` 
        });
      }
      
      // Validate no console errors occurred during interactions
      if (consoleErrors.length > 0) {
        console.log('❌ Console errors during', viewport.name, 'testing:', consoleErrors);
      }
    }
  });

  // Example user journey test
  test('Complete user journey validation', async ({ page }) => {
    console.log('🚀 Starting complete user journey test');
    
    await page.goto('http://localhost:3001');
    await page.waitForLoadState('networkidle');
    
    // Example user journey - customize based on your application
    // 1. Navigate to main page
    await page.screenshot({ path: 'journey-01-homepage.png', fullPage: true });
    
    // 2. Interact with navigation
    const navLinks = await page.locator('nav a').all();
    for (let i = 0; i < Math.min(navLinks.length, 3); i++) { // Test first 3 nav links
      await navLinks[i].click();
      await page.waitForLoadState('networkidle');
      await page.screenshot({ path: `journey-02-nav-${i}.png`, fullPage: true });
      
      // Navigate back for next test
      await page.goBack();
      await page.waitForLoadState('networkidle');
    }
    
    // 3. Test any forms
    const forms = await page.locator('form').all();
    for (let i = 0; i < forms.length; i++) {
      await page.screenshot({ path: `journey-03-form-${i}-initial.png`, fullPage: true });
      
      // Fill out form fields
      const formInputs = await forms[i].locator('input').all();
      for (const input of formInputs) {
        const inputType = await input.getAttribute('type') || 'text';
        let testValue = 'test@example.com';
        
        switch (inputType) {
          case 'email':
            testValue = 'test@example.com';
            break;
          case 'password':
            testValue = 'TestPassword123';
            break;
          case 'text':
          case 'search':
            testValue = 'Test Input';
            break;
          case 'number':
            testValue = '123';
            break;
        }
        
        await input.fill(testValue);
      }
      
      await page.screenshot({ path: `journey-04-form-${i}-filled.png`, fullPage: true });
    }
    
    console.log('✅ User journey test completed successfully');
  });
});

// Helper function for beauty analysis scoring (example implementation)
function analyzeScreenshotBeauty(screenshotPath) {
  // This would be implemented with AI analysis or manual review
  // Return object with scores for each criterion
  return {
    visualHierarchy: 0, // 0-100%
    spacingAlignment: 0, // 0-100%
    typography: 0, // 0-100%
    colorContrast: 0, // 0-100%
    componentStates: 0, // 0-100%
    responsiveBehavior: 0, // 0-100%
    professionalPolish: 0, // 0-100%
    accessibility: 0, // 0-100%
    performance: 0 // 0-100%
  };
}

// Export configuration for use in other test files
module.exports = {
  setupConsoleMonitoring: (page) => {
    const errors = [];
    const warnings = [];
    
    page.on('console', msg => {
      if (msg.type() === 'error') errors.push(msg.text());
      if (msg.type() === 'warning') warnings.push(msg.text());
    });
    
    page.on('pageerror', exception => {
      errors.push(`JavaScript Exception: ${exception.message}`);
    });
    
    return { errors, warnings };
  }
};