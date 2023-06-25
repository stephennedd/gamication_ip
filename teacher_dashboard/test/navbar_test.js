const { Builder, By } = require('selenium-webdriver');
const assert = require('assert');


describe('Test Navbar', function() {
  let driver;

  before (async function() {
    driver = await new Builder().forBrowser('chrome').build();
  });

  after (async function() {
    await driver.quit();
  });

  it('should navigate to the home page when the logo is clicked', async function() {
    await driver.get('http://127.0.0.1:5500/teacher_dashboard/pages/admin-panel.html');
    
    const logoLink = await driver.findElement(By.css('.navbar-brand'));
    await logoLink.click();

    //await driver.manage().setTimeouts( { implicit: 5000 } );

    const url = await driver.getCurrentUrl();
    assert.equal(url, 'http://127.0.0.1:5500/teacher_dashboard/pages/admin-panel.html#');
  });
});
