const { Builder, By } = require('selenium-webdriver');
const assert = require('assert');


describe('test authorization', function() {
  let driver;

  before (async function() {
    driver = await new Builder().forBrowser('chrome').build();
  });

  after (async function() {
    await driver.quit();
  });

  it('should navigate to the home page when the user is logged in (GW)', async function() {
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

    const url = await driver.getCurrentUrl();
    assert.equal(url, 'http://127.0.0.1:5501/teacher_dashboard/pages/admin-panel.html');
  });

  it('should navigate to the login page when the user is logged out (GW)', async function() {
    this.timeout(10000);
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

    const rightToggle = await driver.findElement(By.id('right-toggle'));
    await rightToggle.click();

    await driver.sleep(400);

    const logoutButtonDd = await driver.findElement(By.id('logout-dropdown'));
    await logoutButtonDd.click();

    await driver.sleep(400);

    const logoutButton = await driver.findElement(By.id('logout-button'));
    await logoutButton.click();

    await driver.sleep(1000);

    const url = await driver.getCurrentUrl();
    assert.equal(url, 'http://127.0.0.1:5501/teacher_dashboard/pages/login.html');
  });
});