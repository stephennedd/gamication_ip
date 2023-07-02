const {Builder, By } = require('selenium-webdriver');
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
    
    it('INT004 - should add a teacher (GW)', async function() {
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

    it('INT005 - should show an error while creating a new teacher (BW)', async function() {
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

        // If no exception is thrown, the alert was open
        assert.ok(true, 'Alert is open');
        } catch (e) {
        // Step 3: Handle the exception if the alert was not open
        assert.ok(false, 'Alert is not open');
        }


    });

    it('INT006 - should show success modal after removing a subject (GW)', async function() {
        this.timeout(100000);
        await driver.get('http://127.0.0.1:5501/teacher_dashboard/index.html');
        await driver.sleep(800);

        const adminDropdown = await driver.findElement(By.id('admin-options'));     // click admin dropdown
        await adminDropdown.click();

        await driver.sleep(300);

        const removeQuizLink = await driver.findElement(By.id('delete-quiz-link'));     // click remove subject button
        await removeQuizLink.click();

        await driver.sleep(250);

        const removeSubjectButton = await driver.findElement(By.id('remove-quiz-button'));     // click remove subject button
        await removeSubjectButton.click();

        try {
            await driver.switchTo().alert().dismiss();
            assert.ok(true, 'Alert is open');
        } catch (e) {
            assert.ok(false, 'Alert is not open');
        }
    });
    
    it('INT007 - should show success modal after adding a group (GW)', async function() {
        this.timeout(10000);
        await driver.get('http://127.0.0.1:5501/teacher_dashboard/pages/admin-panel.html#add-group');
        await driver.sleep(800);

        const randomstring = Math.random().toString(36).substring(2, 15) + Math.random().toString(36).substring(2, 15);   // generate random username
        const groupNameField = await driver.findElement(By.id('group-name'));     // enter group name
        await groupNameField.sendKeys(randomstring);

        const addGroupButton = await driver.findElement(By.id('add-group-button'));     // click add group button
        await addGroupButton.click();

        await driver.sleep(2000);

        // check if modal is visible
        const modal = await driver.findElement(By.id('add-group-success-modal'));
        //check if the modal is shown
        const modalIsDisplayed = await modal.isDisplayed();

        assert.equal(modalIsDisplayed, true);

        const dismissButton = await driver.findElement(By.id('dismiss-add-group-modal'));     // click dismiss button
        await dismissButton.click();
    });

    // it('change group name (GW)', async function() {
    //     this.timeout(10000);
    //     await driver.get('http://127.0.0.1:5501/teacher_dashboard/pages/admin-panel.html#manage-groups');
    //     await driver.sleep(800);

    //     const editGroupButton = await driver.findElement(By.id('edit-group-button'));     // click edit group button
    //     await editGroupButton.click();

    //     await driver.sleep(200);

    //     const groupNameField = await driver.findElement(By.id('edit-group-name'));     // enter group name
    //     await groupNameField.sendKeys('test');
    // });

    it('INT008 - should show a success modal after adding a subject (GW)', async function() {
        this.timeout(10000);
        await driver.get('http://127.0.0.1:5501/teacher_dashboard/pages/admin-panel.html#add-subject');
        await driver.sleep(800);

        const randomstring = Math.random().toString(36).substring(2, 15) + Math.random().toString(36).substring(2, 15);   // generate random username
        const subjectNameField = await driver.findElement(By.id('subject-name'));     // enter subject name
        await subjectNameField.sendKeys(randomstring);

        const selectWeekDropdown = await driver.findElement(By.id('subject-week'));     // select week from dropdown
        await selectWeekDropdown.click();
        const weekOption = await driver.findElement(By.css('#subject-week > option:nth-child(2)'));
        await weekOption.click();

        const selectLinkedGameDropdown = await driver.findElement(By.id('subject-game'));     // select linked game from dropdown
        await selectLinkedGameDropdown.click();
        const linkedGameOption = await driver.findElement(By.css('#subject-game > option:nth-child(2)'));
        await linkedGameOption.click();

        const addSubjectButton = await driver.findElement(By.id('add-subject-button'));     // click add subject button
        await addSubjectButton.click();

        await driver.sleep(2000);

        // check if modal is visible
        const modal = await driver.findElement(By.id('add-subject-success-modal'));
        //check if the modal is shown
        const modalIsDisplayed = await modal.isDisplayed();

        assert.equal(modalIsDisplayed, true);

        const dismissButton = await driver.findElement(By.id('dismiss-subject-modal'));     // click dismiss button
        await dismissButton.click();
    });
});

