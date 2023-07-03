const { Builder, By, until } = require('selenium-webdriver');
const assert = require('assert');

describe('Testing Signup and Verification', function () {
	const nock = require('nock');

	// At the beginning of your test case
	nock('http://localhost:5501')
		.post('/api/signup')
		.reply(200, { status: 'ok' })
		.post('/api/login')
		.reply(200, { status: 'unverified', redirectTo: '/VerificationScreen/' })
		.post('/api/verify')
		.reply(200, { status: 'verified', redirectTo: '/LoginScreen/' })
		.post('/api/login')
		.reply(200, { status: 'success', redirectTo: '/AnotherPage/' });

	let driver;

	before(async function () {
		driver = await new Builder().forBrowser('chrome').build();
	});

	after(async function () {
		await driver.quit();
	});

	it('Should sign up, verify, and login successfully', async function () {
		this.timeout(60000);

		// Go to login screen
		await driver.get('http://localhost:5501/LoginScreen/');

		// Click on sign-up label
		const signUpLabel = await driver.findElement(By.css("label[for='signup']"));
		await signUpLabel.click();

		// Enter credentials
		const studentID = await driver.findElement(By.id('studentID'));
		const passwordInput = await driver.findElement(By.id('password'));
		const repasswordInput = await driver.findElement(By.id('repassword'));
		await studentID.sendKeys('testuser');
		await passwordInput.sendKeys('testpassword');
		await repasswordInput.sendKeys('testpassword');
		const submitButton = await driver.findElement(By.css('button'));
		await submitButton.click();

		// Assume taken to sign-in page with prefilled fields
		await driver.wait(until.urlIs('http://localhost:5501/LoginScreen/'), 10000);

		// Try to log in
		const loginForm = await driver.wait(
			until.elementLocated(By.id('loginForm')),
			10000
		);
		await loginForm.submit();

		// Taken to verification form
		await driver.wait(
			until.urlIs('http://localhost:5501/VerificationScreen/'),
			10000
		);

		// Fill verification code and submit
		const verificationCodeInput = await driver.findElement(
			By.id('verificationCode')
		);
		const groupSelection = await driver.findElement(By.id('groupSelection'));
		await verificationCodeInput.sendKeys('correctVerificationCode');
		await groupSelection.sendKeys('testGroup');
		const verifyButton = await driver.findElement(By.id('verifyButton'));
		await verifyButton.click();

		// Taken back to login page
		await driver.wait(until.urlIs('http://localhost:5501/LoginScreen/'), 10000);

		// Log in again
		await loginForm.submit();

		// Taken to another page
		await driver.wait(until.urlIs('http://localhost:5501/AnotherPage/'), 10000);
		assert.strictEqual(
			await driver.getCurrentUrl(),
			'http://localhost:5501/AnotherPage/'
		);
	});
});
