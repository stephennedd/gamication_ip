const { Builder, By, Key, until } = require('selenium-webdriver');
const assert = require('assert');

const url = 'http://localhost:5501/LoginScreen';

describe('SignIn functionality and handling wrong credentials', function () {
	this.timeout(10000);
	let driver;

	beforeEach(async () => {
		driver = await new Builder().forBrowser('chrome').build();
		await driver.get(url);
	});

	afterEach(async () => {
		await driver.quit();
	});

	it('Should allow a user to login with valid credentials', async () => {
		this.timeout(10000);
		await driver.get('http://localhost:5501/LoginScreen/');

		// Wait for loginForm to be visible
		const loginForm = await driver.wait(
			until.elementLocated(By.id('loginForm')),
			10000
		);
		await driver.wait(until.elementIsVisible(loginForm), 10000);

		const loginButton = await loginForm.findElement(By.css('button'));

		await driver.findElement(By.id('studentID')).sendKeys('666');
		await driver.findElement(By.id('password')).sendKeys('polonez');
		await loginButton.click();

		// Wait for redirect
		await driver.wait(until.urlContains('/ArcadeMachine'), 5000);

		const currentUrl = await driver.getCurrentUrl();
		assert(currentUrl.includes('/ArcadeMachine'));
	});

	it('Should show an error message with invalid credentials', async () => {
		this.timeout(10000);
		await driver.get('http://localhost:5501/LoginScreen/');

		// Wait for loginForm to be visible
		const loginForm = await driver.wait(
			until.elementLocated(By.id('loginForm')),
			10000
		);
		await driver.wait(until.elementIsVisible(loginForm), 10000);

		const loginButton = await loginForm.findElement(By.css('button'));

		await driver.findElement(By.id('studentID')).sendKeys('0000');
		await driver.findElement(By.id('password')).sendKeys('wrongPassword');
		await loginButton.click();

		const errorMessage = await driver.wait(
			until.elementLocated(By.css('.error-message')),
			5000
		);

		assert(errorMessage.isDisplayed());
	});
});
