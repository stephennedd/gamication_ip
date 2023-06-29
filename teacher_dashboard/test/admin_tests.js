const {Builder, By, until, } = require('selenium-webdriver');
const assert = require('assert');

describe('admin test', function() {
    let driver;
    
    before (async function() {
        driver = await new Builder().forBrowser('chrome').build();
        const window = driver.manage().window();
        window.maximize();
    });
    
    after (async function() {
        await driver.quit();
    });
    
    it('add teacher (GW)', async function() {
        this.timeout(10000);
        await driver.get('http://127.0.0.1:5501/teacher_dashboard/pages/login.html');

        const usernameField = await driver.findElement(By.id('username'));     // enter username and password
        await usernameField.sendKeys('Admin');

        const passwordField = await driver.findElement(By.id('password'));
        await passwordField.sendKeys('password123');

        const loginButton = await driver.findElement(By.id('login-button'));     // click login button
        await loginButton.click();

        await driver.sleep(400);

        await driver.get('http://127.0.0.1:5501/teacher_dashboard/pages/admin-panel.html#add-user');
        await driver.sleep(200);

        const randomstring = Math.random().toString(36).substring(2, 15) + Math.random().toString(36).substring(2, 15);   // generate random username
        const firstNameField = await driver.findElement(By.id('first-name'));     // enter first name
        await firstNameField.sendKeys(randomstring);

        const lastNameField = await driver.findElement(By.id('last-name'));     // enter last name
        await lastNameField.sendKeys(randomstring);

        const usernameField2 = await driver.findElement(By.id('username'));     // enter username
        await usernameField2.sendKeys(randomstring);

        const idField = await driver.findElement(By.id('userid'));     // enter id
        await idField.sendKeys(randomstring);

        // select teacher role from select dropdown
        const roleDropdown = await driver.findElement(By.id('user-role'));
        await roleDropdown.click();
        const roleOption = await driver.findElement(By.css('#user-role > option:nth-child(3)'));
        await roleOption.click();

        const addUserButton = await driver.findElement(By.id('add-user-button'));     // click add user button
        await addUserButton.click();

        await driver.sleep(2000);

        // check if modal is visible
        const modal = await driver.findElement(By.id('add-user-success-modal'));
        //check if the modal is shown
        const modalIsDisplayed = await modal.isDisplayed();

        assert.equal(modalIsDisplayed, true);
    });

    it('add teacher (BW)', async function() {
        this.timeout(10000);
        await driver.get('http://127.0.0.1:5501/teacher_dashboard/pages/admin-panel.html#add-user');
        await driver.sleep(800);

        const firstNameField = await driver.findElement(By.id('first-name'));     // enter first name
        await firstNameField.sendKeys('Admin');

        const lastNameField = await driver.findElement(By.id('last-name'));     // enter last name
        await lastNameField.sendKeys('test');

        const usernameField2 = await driver.findElement(By.id('username'));     // enter username
        await usernameField2.sendKeys('Admin');

        const idField = await driver.findElement(By.id('userid'));     // enter id
        await idField.sendKeys('Admin');

        // select teacher role from select dropdown
        const roleDropdown = await driver.findElement(By.id('user-role'));
        await roleDropdown.click();
        const roleOption = await driver.findElement(By.css('#user-role > option:nth-child(2)'));
        await roleOption.click();

        const addUserButton = await driver.findElement(By.id('add-user-button'));     // click add user button
        await addUserButton.click();

        await driver.sleep(1000);

        try {
        // Step 2: Try to accept or dismiss the alert
        await driver.switchTo().alert().accept();

        // or alert.dismiss();

        // If no exception is thrown, the alert was open
        console.log('Alert is open');
        assert.ok(true, 'Alert is open');
        } catch (e) {
        // Step 3: Handle the exception if the alert was not open
        console.log('Alert is not open');
        assert.ok(false, 'Alert is not open');
        }


    });

    it('remove subject (GW)', async function() {
        this.timeout(100000);
        await driver.get('http://127.0.0.1:5501/teacher_dashboard/index.html');
        await driver.sleep(800);

        const adminDropdown = await driver.findElement(By.id('admin-options'));     // click admin dropdown
        await adminDropdown.click();

        await driver.sleep(300);

        const removeQuizLink = await driver.findElement(By.id('delete-quiz-link'));     // click remove subject button
        await removeQuizLink.click();

        await driver.sleep(250);

        // count the number of subjects
        const table = await driver.findElement(By.id('quiz-table'));
        const rows = await table.findElements(By.tagName('tr'));
        const rowCount = rows.length;

        const removeSubjectButton = await driver.findElement(By.id('remove-quiz-button'));     // click remove subject button
        await removeSubjectButton.click();

        await driver.switchTo().alert().accept();
        await driver.sleep(8000);

        // count the number of subjects after removing one
        const table2 = await driver.findElement(By.id('quiz-table'));
        const rows2 = await table2.findElements(By.tagName('tr'));
        const rowCount2 = rows2.length;
        
        assert.equal(rowCount2, rowCount - 1);
    }); 
});

