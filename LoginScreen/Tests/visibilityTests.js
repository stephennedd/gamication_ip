require('chromedriver');
const { Builder, By, until } = require('selenium-webdriver');
const assert = require('assert');

describe('Testing Project login page if all elements are displayed correctly', function () {
	let driver;

	before(async function () {
		driver = await new Builder().forBrowser('chrome').build();
	});

	after(async function () {
		await driver.quit();
	});

	it('Should have the correct Main Text displayed', async function () {
		this.timeout(10000);
		await driver.get('http://localhost:5501/LoginScreen/');

		// Wait until the main text is loaded
		const welcomeElement = await driver.wait(
			until.elementLocated(By.css('.welcome')),
			10000
		);

		// Wait until the final character is displayed
		await driver.wait(async function () {
			const mainText = await welcomeElement.getText();
			return mainText.includes('PLEASE ENTER YOUR CREDENTIALS...');
		}, 10000);

		const actualText = await welcomeElement.getText();
		assert.strictEqual(
			actualText,
			'WELCOME TO PROJECT G.A.M.I.F.I.C.A.T.I.O.N PLEASE ENTER YOUR CREDENTIALS...'
		);
	});

	it('Should have login input fields and login button after the main text is displayed', async function () {
		this.timeout(10000);
		await driver.get('http://localhost:5501/LoginScreen/');

		// Wait for loginForm to be visible
		const loginForm = await driver.wait(
			until.elementLocated(By.id('loginForm')),
			10000
		);
		await driver.wait(until.elementIsVisible(loginForm), 10000);

		const loginButton = await loginForm.findElement(By.css('button'));
		const isLoginButtonDisplayed = await loginButton.isDisplayed();

		assert.strictEqual(isLoginButtonDisplayed, true);
	});

	it('Should toggle between Sign In and Sign Up form', async function () {
		this.timeout(20000);
		await driver.get('http://localhost:5501/LoginScreen/');

		// Check if loginForm is visible
		const loginForm = await driver.wait(
			until.elementLocated(By.id('loginForm')),
			10000
		);
		await driver.wait(until.elementIsVisible(loginForm), 10000);

		// Check for studentID and password fields
		const studentIDField = await loginForm.findElement(By.id('studentID'));
		const passwordField = await loginForm.findElement(By.id('password'));
		assert.strictEqual(await studentIDField.isDisplayed(), true);
		assert.strictEqual(await passwordField.isDisplayed(), true);

		// Click signUpButton
		const signUpLabel = await driver.findElement(By.css("label[for='signup']"));
		await signUpLabel.click();

		// Check for repassword, firstName, and lastName fields
		const repasswordField = await loginForm.findElement(By.id('repassword'));
		const firstNameField = await loginForm.findElement(By.id('firstName'));
		const lastNameField = await loginForm.findElement(By.id('lastName'));
		await driver.wait(until.elementIsVisible(repasswordField), 10000);
		await driver.wait(until.elementIsVisible(firstNameField), 10000);
		await driver.wait(until.elementIsVisible(lastNameField), 10000);
		assert.strictEqual(await repasswordField.isDisplayed(), true);
		assert.strictEqual(await firstNameField.isDisplayed(), true);
		assert.strictEqual(await lastNameField.isDisplayed(), true);

		// Click signInButton
		const signInLabel = await driver.findElement(By.css("label[for='signin']"));
		await signInLabel.click();

		// Check if only studentID and password fields are visible
		assert.strictEqual(await studentIDField.isDisplayed(), true);
		assert.strictEqual(await passwordField.isDisplayed(), true);
		await driver.wait(
			async function () {
				return (
					!(await repasswordField.isDisplayed()) &&
					!(await firstNameField.isDisplayed()) &&
					!(await lastNameField.isDisplayed())
				);
			},
			10000,
			'One or more elements are still visible after 10 seconds'
		);
		assert.strictEqual(await repasswordField.isDisplayed(), false);
		assert.strictEqual(await firstNameField.isDisplayed(), false);
		assert.strictEqual(await lastNameField.isDisplayed(), false);
	});
});
