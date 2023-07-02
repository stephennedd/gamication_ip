const { Builder, By } = require('selenium-webdriver');
const assert = require('assert');


describe('navbar test', function() {
  let driver;

  before (async function() {
    driver = await new Builder().forBrowser('chrome').build();
  });

  after (async function() {
    await driver.quit();
  });

  it('INT010 - should navigate to the home page when the logo is clicked', async function() {
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
    
    const logoLink = await driver.findElement(By.css('.navbar-brand'));
    await logoLink.click();

    await driver.sleep(400);
    
    const url = await driver.getCurrentUrl();
    assert.equal(url, 'http://127.0.0.1:5501/teacher_dashboard/pages/admin-panel.html#');
  });
});
