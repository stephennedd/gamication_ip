const { Builder, By } = require('selenium-webdriver');
const assert = require('assert');


describe('leaderboard test', function() {
  let driver;

  before (async function() {
    driver = await new Builder().forBrowser('chrome').build();
    const window = driver.manage().window();
    window.maximize();
  });

  after (async function() {
    await driver.quit();
  });

  it('INT009 - should edit a leaderboard (GW)', async function() {
    this.timeout(10000);
    await driver.get('http://127.0.0.1:5501/teacher_dashboard/pages/login.html');
    
    const usernameField = await driver.findElement(By.id('username'));     // enter username and password
    await usernameField.sendKeys('Admin');

    const passwordField = await driver.findElement(By.id('password'));
    await passwordField.sendKeys('password123');

    const loginButton = await driver.findElement(By.id('login-button'));     // click login button
    await loginButton.click();

    await driver.sleep(800);

    await driver.get('http://127.0.0.1:5501/teacher_dashboard/pages/admin-panel.html#update-leaderboard');

    const updateButton = await driver.findElement(By.id('update-leaderboard-button'));
    await updateButton.click();

    await driver.sleep(1050);

    const leaderboardNameField = await driver.findElement(By.id('modal-leaderboard-name'));
    await leaderboardNameField.clear();
    const randomLeaderboardName = Math.random().toString(36).substring(2, 15) + Math.random().toString(36).substring(2, 15);   // generate random leaderboard name
    await leaderboardNameField.sendKeys(randomLeaderboardName);

    const updateButton2 = await driver.findElement(By.id('update-leaderboard-save-button'));
    await updateButton2.click();

    await driver.sleep(450);

    // check if modal is visible
    const modal = await driver.findElement(By.id('edit-leaderboard-success-modal'));
    const modalVisible = await modal.isDisplayed();

    assert.equal(modalVisible, true);
});
});
