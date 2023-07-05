require('chromedriver');
const { Builder, By, until } = require('selenium-webdriver');
const assert = require('assert');

describe('Testing if errors for input fields are handled correctly', function () {
	let driver;

	before(async function () {
		driver = await new Builder().forBrowser('chrome').build();
	});

	after(async function () {
		await driver.quit();
	});

	it('Should display error messages correctly on Sign-In form', async function () {
		this.timeout(30000); // Increase the timeout
		await driver.get('http://localhost:5501/LoginScreen/');

		const signInLabel = await driver.findElement(By.css("label[for='signin']"));
		// Wait for loginForm to be visible
		const loginForm = await driver.wait(
			until.elementLocated(By.id('loginForm')),
			10000
		);
		await driver.wait(until.elementIsVisible(loginForm), 10000);

		const submitButton = await loginForm.findElement(By.css('button'));

		const errorElement = await driver.findElement(
			By.className('error-message')
		);

		// Click sign-in radio button
		await signInLabel.click();

		// Submit the form with empty fields
		await submitButton.click();

		// Wait for the error message to be displayed
		await driver.wait(async function () {
			const errorMessage = await errorElement.getText();
			return errorMessage === 'Please fill in all fields.';
		}, 10000);

		// Assert that the error message is displayed and contains the expected text
		let errorMessage = await errorElement.getText();
		assert.strictEqual(errorMessage, 'Please fill in all fields.');
	});

	it('Should display error messages correctly on Sign-Up form', async function () {
		this.timeout(30000);
		await driver.get('http://localhost:5501/LoginScreen/');

		const signUpLabel = await driver.findElement(By.css("label[for='signup']"));
		// Wait for loginForm to be visible
		const loginForm = await driver.wait(
			until.elementLocated(By.id('loginForm')),
			10000
		);
		await driver.wait(until.elementIsVisible(loginForm), 10000);

		const submitButton = await loginForm.findElement(By.css('button'));

		const errorElement = await driver.findElement(
			By.className('error-message')
		);

		// Click sign-up radio button
		await signUpLabel.click();

		// Submit the form with empty fields
		await submitButton.click();

		// Wait for the error message to be displayed
		await driver.wait(async function () {
			const errorMessage = await errorElement.getText();
			return errorMessage === 'Please fill in all fields.';
		}, 10000);

		// Assert that the error message is displayed and contains the expected text
		let errorMessage = await errorElement.getText();
		assert.strictEqual(errorMessage, 'Please fill in all fields.');

		// Now enter different passwords and check error
		const studentID = await driver.findElement(By.id('studentID'));
		const passwordInput = await driver.findElement(By.id('password'));
		const repasswordInput = await driver.findElement(By.id('repassword'));

		await studentID.sendKeys('1234');
		await passwordInput.sendKeys('password');
		await repasswordInput.sendKeys('differentPassword');

		// Submit the form with different passwords
		await loginForm.submit();

		// Wait for the error message to be displayed
		await driver.wait(async function () {
			const errorMessage = await errorElement.getText();
			return errorMessage === 'Passwords do not match.';
		}, 10000);

		// Assert that the error message is displayed and contains the expected text
		errorMessage = await errorElement.getText();
		assert.strictEqual(errorMessage, 'Passwords do not match.');
	});

	// TODO error messages on verification
	// it('Should display error messages correctly on Verification form', async function () {
	// 	this.timeout(30000); // Increase the timeout
	// 	await driver.get('http://localhost:5501/LoginScreen/'); // Go to the Verification Screen

	// 	// Wait for verificationForm to be visible
	// 	const verificationForm = await driver.wait(
	// 		until.elementLocated(By.id('verificationForm')),
	// 		10000
	// 	);
	// 	await driver.wait(until.elementIsVisible(verificationForm), 10000);

	// 	const submitButton = await verificationForm.findElement(By.css('button'));
	// 	const errorElement = await driver.findElement(
	// 		By.className('error-message')
	// 	);

	// 	// Insert invalid verification code
	// 	const verificationCodeInput = await driver.findElement(
	// 		By.id('verificationCode')
	// 	);
	// 	await verificationCodeInput.sendKeys('invalidCode');

	// 	// Submit the form with the invalid verification code
	// 	await submitButton.click();

	// 	// Wait for the error message to be displayed
	// 	await driver.wait(async function () {
	// 		const errorMessage = await errorElement.getText();
	// 		return errorMessage === 'Invalid verification code.';
	// 	}, 10000);

	// 	// Assert that the error message is displayed and contains the expected text
	// 	let errorMessage = await errorElement.getText();
	// 	assert.strictEqual(errorMessage, 'Invalid verification code.');
	// });
});
