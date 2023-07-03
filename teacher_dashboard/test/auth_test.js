const { Builder, By } = require('selenium-webdriver');
const assert = require('assert');


describe('authentication test', function() {
  let driver;

  before (async function() {
    driver = await new Builder().forBrowser('chrome').build();
  });

  after (async function() {
    await driver.quit();
  });

  it('INT001 - should navigate to the home page when the user is logged in (GW)', async function() {
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

  it('INT002 - should navigate to the login page when the user is logged out (GW)', async function() {
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

  it('INT003 - should show an error when the username/password is incorrect (BW)', async function() {
    this.timeout(10000);
    await driver.get('http://127.0.0.1:5501/teacher_dashboard/pages/login.html');
    // enter username and password
    const usernameField = await driver.findElement(By.id('username'));
    await usernameField.sendKeys('Admin'); // correct username

    const passwordField = await driver.findElement(By.id('password'));
    await passwordField.sendKeys('password'); // incorrect password

    // click login button
    const loginButton = await driver.findElement(By.id('login-button'));
    await loginButton.click();

    await driver.sleep(1000);
    
    try {
      // Step 2: Try to accept or dismiss the alert
      await driver.switchTo().alert().accept();

      // If no exception is thrown, the alert was open
      assert.ok(true, 'Alert is open');
      } catch (e) {
      // Step 3: Handle the exception if the alert was not open
      assert.ok(false, 'Alert is not open');
      }
  });
});