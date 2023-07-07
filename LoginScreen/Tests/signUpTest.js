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

	it('Should allow a user to sign up with valid details', async () => {
		// Wait for loginForm to be visible
		const loginForm = await driver.wait(
			until.elementLocated(By.id('loginForm')),
			10000
		);
		await driver.wait(until.elementIsVisible(loginForm), 10000);

		const signUpLabel = await driver.findElement(By.css("label[for='signup']"));
		await signUpLabel.click();

		// Enter details
		await driver.findElement(By.id('firstName')).sendKeys('Test');
		await driver.findElement(By.id('lastName')).sendKeys('User');
		await driver.findElement(By.id('studentID')).sendKeys('123458');
		await driver.findElement(By.id('password')).sendKeys('testPassword');
		await driver.findElement(By.id('repassword')).sendKeys('testPassword');

		const signUpButton = await driver.findElement(By.css('button'));
		await signUpButton.click();

		await driver.wait(until.findElement(By.id('studentId'), 5000));
		await driver.wait(until.findElement(By.id('password'), 5000));
		await signUpButton.click();

		await driver.wait(until.findElement(By.id('verificationCode'), 5000));
		await driver.wait(until.findElement(By.id('groupSelect'), 5000));

		await driver.findElement(By.id('verificationCode')).sendKeys('Test');
	});
	// 	// Assuming you are on the login page
	// 	await driver.findElement(By.id('studentID')).sendKeys('12345');
	// 	await driver.findElement(By.id('password')).sendKeys('testPassword');

	// 	const loginForm = await driver.wait(
	// 		until.elementLocated(By.id('loginForm')),
	// 		10000
	// 	);
	// 	const loginButton = await loginForm.findElement(By.css('button'));
	// 	await loginButton.click();

	// 	// Wait for redirect to dashboard or main page
	// 	await driver.wait(until.urlContains('/ArcadeMachine'), 5000);

	// 	const currentUrl = await driver.getCurrentUrl();
	// 	assert(currentUrl.includes('/ArcadeMachine'));
	// });
});
