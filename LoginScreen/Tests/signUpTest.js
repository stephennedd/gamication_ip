const { Builder, By, until } = require('selenium-webdriver');
const assert = require('assert');

const url = 'http://localhost:5501/LoginScreen';

describe('SignUp functionality', function () {
	this.timeout(30000);
	let driver;

	beforeEach(async () => {
		driver = await new Builder().forBrowser('chrome').build();
		await driver.get(url);
	});

	afterEach(async () => {
		await driver.quit();
	});

	it('INT022 - Should allow a user to sign up with valid details', async () => {
		// Wait for loginForm to be visible
		const loginForm = await driver.wait(
			until.elementLocated(By.id('loginForm')),
			10000
		);
		await driver.wait(until.elementIsVisible(loginForm), 10000);

		const signUpLabel = await driver.findElement(By.css("label[for='signup']"));
		await signUpLabel.click();
		await driver.manage().setTimeouts({ implicit: 3000 });

		// Enter details
		await driver.findElement(By.id('firstName')).sendKeys('Test');
		await driver.findElement(By.id('lastName')).sendKeys('User');
		await driver.findElement(By.id('studentID')).sendKeys('523433');
		await driver.findElement(By.id('password')).sendKeys('testPassword');
		await driver.findElement(By.id('repassword')).sendKeys('testPassword');

		const signUpButton = await driver.findElement(By.css('button'));
		await signUpButton.click();

		await driver.manage().setTimeouts({ implicit: 5000 });
		await signUpButton.click();

		await driver.manage().setTimeouts({ implicit: 5000 });

		const welcomeElement = await driver.wait(
			until.elementLocated(By.css('.welcome')),
			10000
		);

		// Wait until the text is 'Account created!'
		await driver.wait(async function () {
			const welcomeText = await welcomeElement.getText();
			return welcomeText.includes('Account created!');
		}, 10000); // Wait 15 seconds for "Account created!" to appear

		const finalWelcomeText = await welcomeElement.getText();
		assert.strictEqual(finalWelcomeText, 'Account created!');
	});
});
