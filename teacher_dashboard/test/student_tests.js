const { Builder, By, until } = require('selenium-webdriver');
const assert = require('assert');


describe('student test', function() {
  let driver;

  before (async function() {
    driver = await new Builder().forBrowser('chrome').build();
    const window = driver.manage().window();
    window.maximize();
  });

  after (async function() {
    await driver.quit();
  });

  it('INT013 - should show an updated student name, lastname, and username', async function() {
    this.timeout(10000);
    await driver.get('http://127.0.0.1:5501/teacher_dashboard/pages/login.html');
    
    const usernameField = await driver.findElement(By.id('username'));     // enter username and password
    await usernameField.sendKeys('Admin');

    const passwordField = await driver.findElement(By.id('password'));
    await passwordField.sendKeys('password123');

    const loginButton = await driver.findElement(By.id('login-button'));     // click login button
    await loginButton.click();

    await driver.wait(until.elementLocated(By.id('test-menu-dropdown-2')), 10000);
    //await driver.sleep(2000);

    const sidebarMenuDropdown = await driver.findElement(By.id('test-menu-dropdown-2'));
    await sidebarMenuDropdown.click();

    await driver.sleep(150);

    const sidebarLink = await driver.findElement(By.id('update-student-link'));
    await sidebarLink.click();

    await driver.sleep(150);

    const firstName = await driver.findElement(By.id('student-first-name'));     // check the first name
    const firstNameText = await firstName.getText();

    const lastName = await driver.findElement(By.id('student-last-name'));     // check the last name
    const lastNameText = await lastName.getText();

    const userName = await driver.findElement(By.id('student-username'));     // check the username
    const userNameText = await userName.getText();

    const updateButton = await driver.findElement(By.id('update-student-button'));
    await updateButton.click();

    await driver.sleep(250);   

    const firstNameField = await driver.findElement(By.id('modal-first-name'));
    await firstNameField.clear();
    const randomFirstName = Math.random().toString(36).substring(2, 15) + Math.random().toString(36).substring(2, 15);   // generate random first name
    await firstNameField.sendKeys(randomFirstName);

    const secondNameField = await driver.findElement(By.id('modal-last-name'));
    await secondNameField.clear();
    const randomSecondName = Math.random().toString(36).substring(2, 15) + Math.random().toString(36).substring(2, 15);   // generate random last name
    await secondNameField.sendKeys(randomSecondName);

    const userNameField = await driver.findElement(By.id('modal-username'));
    await userNameField.clear();
    const randomUserName = Math.random().toString(36).substring(2, 15) + Math.random().toString(36).substring(2, 15);   // generate random username
    await userNameField.sendKeys(randomUserName);

    const updateStudentButton = await driver.findElement(By.id('update-student-modal-button'));
    await updateStudentButton.click();

    await driver.switchTo().alert().accept(); //accept alert

    await driver.wait(until.elementLocated(By.id('student-first-name')), 10000);

    const updatedFirstName = await driver.findElement(By.id('student-first-name'));
    const updatedFirstNameText = await updatedFirstName.getText();
    const updatedLastName = await driver.findElement(By.id('student-last-name'));
    const updatedLastNameText = await updatedLastName.getText();
    const updatedUserName = await driver.findElement(By.id('student-username'));
    const updatedUserNameText = await updatedUserName.getText();

    assert.notEqual(firstNameText, updatedFirstNameText);
    assert.notEqual(lastNameText, updatedLastNameText);
    assert.notEqual(userNameText, updatedUserNameText);
    });

    it('INT014 - should count 1 student less in the list of students (GW)', async function() {
        this.timeout(10000);
        await driver.get('http://127.0.0.1:5501/teacher_dashboard/pages/login.html');
    
        const usernameField = await driver.findElement(By.id('username'));     // enter username and password
        await usernameField.sendKeys('Admin');

        const passwordField = await driver.findElement(By.id('password'));
        await passwordField.sendKeys('password123');

        const loginButton = await driver.findElement(By.id('login-button'));     // click login button
        await loginButton.click();

        await driver.sleep(150);

        await driver.get('http://127.0.0.1:5501/teacher_dashboard/pages/admin-panel.html#delete-student');

        await driver.sleep(150);

        //count students in table
        const table = await driver.findElement(By.id('student-table'));
        var rows = await table.findElements(By.tagName('tr'));
        const count = rows.length - 1;

        const removeButton = await driver.findElement(By.id('delete-student-button'));
        await removeButton.click();

        await driver.switchTo().alert().accept(); //accept alert

        await driver.sleep(150);

        //count students in table
        const table2 = await driver.findElement(By.id('student-table'));
        const rows2 = await table2.findElements(By.tagName('tr'));
        const count2 = rows2.length - 1;

        assert.equal(count, count2 + 1);
    });

    it('INT015 - student status should show as the opposite of before (GW)', async function() {
        this.timeout(10000);
        await driver.get('http://127.0.0.1:5501/teacher_dashboard/pages/login.html');
    
        const usernameField = await driver.findElement(By.id('username'));     // enter username and password
        await usernameField.sendKeys('Admin');

        const passwordField = await driver.findElement(By.id('password'));
        await passwordField.sendKeys('password123');

        const loginButton = await driver.findElement(By.id('login-button'));     // click login button
        await loginButton.click();

        await driver.sleep(150);

        await driver.get('http://127.0.0.1:5501/teacher_dashboard/pages/admin-panel.html#ban-student');

        await driver.sleep(150);

        const studentStatus = await driver.findElement(By.id('ban-status'));
        const studentStatusText = await studentStatus.getText();

        const removeButton = await driver.findElement(By.id('ban-student-button'));
        await removeButton.click();

        await driver.switchTo().alert().accept(); //accept alert

        await driver.sleep(150);

        const studentStatus2 = await driver.findElement(By.id('ban-status'));
        const studentStatusText2 = await studentStatus2.getText();

        assert.notEqual(studentStatusText, studentStatusText2);
    });
});
