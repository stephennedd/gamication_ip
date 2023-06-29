const { Builder, By, until } = require('selenium-webdriver');
const assert = require('assert');

describe('sidebar tests', function() {
    let driver;

    before (async function() {
        driver = await new Builder().forBrowser('chrome').build();
    });

    after (async function() {
        await driver.quit();
    });

    it('should change the main content when a sidebar link is clicked', async function() {
        this.timeout(12000);
        await driver.get('http://127.0.0.1:5501/teacher_dashboard/pages/login.html');
    
        // enter username and password
        const usernameField = await driver.findElement(By.id('username'));
        await usernameField.sendKeys('Admin');

        const passwordField = await driver.findElement(By.id('password'));
        await passwordField.sendKeys('password123');

        // click login button
        const loginButton = await driver.findElement(By.id('login-button'));
        await loginButton.click();

        await driver.sleep(400);
        
        const menuButton = await driver.findElement(By.css('.navbar-toggler'));
        await menuButton.click();

        await driver.sleep(400);

        const sidebarMenuDropdown = await driver.findElement(By.id('test-menu-dropdown'));
        await sidebarMenuDropdown.click();

        await driver.sleep(400);
        
        const sidebarLink = await driver.findElement(By.id('edit-quiz-link'));
        await sidebarLink.click();

        await driver.sleep(400);

        const url = await driver.getCurrentUrl();
        assert.equal(url, 'http://127.0.0.1:5501/teacher_dashboard/pages/admin-panel.html#edit-quiz');
    });
});