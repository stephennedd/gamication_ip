require('chromedriver');
const { Builder, By, until } = require('selenium-webdriver');
const assert = require('assert');

describe('Checking', function () {
	this.timeout(30000);
	let driver;

	before(async function () {
		driver = await new Builder().forBrowser('chrome').build();
	});

	after(async function () {
		await driver.quit();
	});

	it('Should correctly parse a JWT token', async function () {
		this.timeout(30000);

		await driver.get('http://127.0.0.1:5501/LoginScreen/');

		// A sample JWT token for testing.
		const sampleToken =
			'eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJzdWIiOiIxMjM0NTY3ODkwIiwibmFtZSI6IkpvaG4gRG9lIiwiaWF0IjoxNTE2MjM5MDIyfQ.SflKxwRJSMeKKF2QT4fwpMeJf36POk6yJV_adQssw5c';

		// Execute the parseJwt function in the browser's context and get the result.
		const parsedToken = await driver.executeScript(
			`
            // Define the parseJwt function if it's not already defined on the page.
            function parseJwt(token) {
                const base64Url = token.split('.')[1];
                const base64 = base64Url.replace(/-/g, '+').replace(/_/g, '/');
                const jsonPayload = decodeURIComponent(
                    atob(base64)
                        .split('')
                        .map(function (c) {
                            return '%' + ('00' + c.charCodeAt(0).toString(16)).slice(-2);
                        })
                        .join('')
                );

                return JSON.parse(jsonPayload);
            }

            // Call the parseJwt function with the sample token.
            return parseJwt(arguments[0]);
        `,
			sampleToken
		);

		// Assert that the parsed token is as expected.
		assert.strictEqual(
			parsedToken.sub,
			'1234567890',
			'Sub field should be 1234567890'
		);
		assert.strictEqual(
			parsedToken.name,
			'John Doe',
			'Name field should be John Doe'
		);
	});
	// 	this.timeout(30000); // Increase the timeout

	// 	// Load the webpage that contains the refreshJWT function
	// 	await driver.get('http://127.0.0.1:5501/LoginScreen/');

	// 	// Assuming you have some means to set a cookie before calling refreshJWT
	// 	// Set a cookie for jwt (token should be valid)
	// 	await driver.manage().addCookie({
	// 		name: 'jwt',
	// 		value: 'your_actual_jwt_token',
	// 		path: '/',
	// 	});

	// 	// Execute the refreshJWT function
	// 	await driver.executeScript(`
	//         // Define the refreshJWT function here if it's not already defined on the page.

	//         // [your refreshJWT function here]

	//         // Call the refreshJWT function.
	//         refreshJWT();
	//     `);

	// 	// Give some time for the async function to complete.
	// 	await driver.sleep(2000);

	// 	// Check for UI updates (assuming the JWT was refreshed successfully).
	// 	// For example, you can check if the loginForm has class 'verify' and the errorElement is hidden.
	// 	const loginFormClass = await driver
	// 		.findElement(By.id('loginForm'))
	// 		.getAttribute('class');
	// 	assert(
	// 		loginFormClass.includes('verify'),
	// 		'Login form should switch to verification'
	// 	);

	// 	const errorElementDisplay = await driver
	// 		.findElement(By.id('errorElement'))
	// 		.getCssValue('display');
	// 	assert.strictEqual(
	// 		errorElementDisplay,
	// 		'none',
	// 		'Error element should be hidden'
	// 	);
	// });

	// it('Test verifyCode()', async function () {
	// 	// Load the webpage that contains the verifyCode function and the necessary elements
	// 	await driver.get('http://localhost:5501/path-to-your-webpage');

	// 	// Define the verifyCode function and other necessary functions/variables in browser context
	// 	await driver.executeScript(`
	//         // You would need to define or import displayTextOneCharacterAtATime, setIndicatorPosition
	//         // as well as any necessary DOM elements like errorElement, welcomeElement, etc.
	//         // These would typically be defined in your actual web page scripts.

	//         window.verifyCode = function (code) {
	//             let token = document.cookie
	//                 .split('; ')
	//                 .find((row) => row.startsWith('jwt='))
	//                 .split('=')[1];

	//             fetch('http://localhost:4434/api/Users/' + code, {
	//                 method: 'POST',
	//                 headers: {
	//                     'Content-Type': 'application/json',
	//                     Authorization: Bearer ${token},
	//                 },
	//                 body: JSON.stringify({ code: code }),
	//             })
	//             .then((response) => {
	//                 // ... rest of your function
	//             })
	//             .catch((error) => {
	//                 console.error('Error:', error);
	//             });
	//         }
	//     `);

	// 	// Execute the verifyCode function with a sample code
	// 	await driver.executeScript(`
	//         window.verifyCode('sample-code');
	//     `);

	// 	// You would now need to assert the behavior depending on the expected outcome.
	// 	// For instance, you could check if a certain element becomes visible,
	// 	// or if the page is redirected somewhere, etc.

	// 	// Example: Check if errorElement is displayed
	// 	// Note: This is just an example and may not reflect your actual expected behavior.
	// 	await driver.wait(
	// 		until.elementIsVisible(driver.findElement(By.id('errorElement'))),
	// 		10000
	// 	);
	// 	assert.isTrue(
	// 		await driver.findElement(By.id('errorElement')).isDisplayed()
	// 	);
	// });

	it('Should populate the groups dropdown with data fetched from the server', async () => {
		this.timeout(30000);

		const loginForm = await driver.wait(
			until.elementLocated(By.id('loginForm')),
			10000
		);
		await driver.wait(until.elementIsVisible(loginForm), 10000);

		const signUpLabel = await driver.findElement(By.css("label[for='signup']"));
		await signUpLabel.click();

		const studentIDInput = await driver.findElement(By.id('studentID'));
		const passwordInput = await driver.findElement(By.id('password'));
		const repasswordInput = await driver.findElement(By.id('repassword'));
		await driver.wait(until.elementIsVisible(studentIDInput), 10000);

		// Enter details
		studentIDInput.sendKeys('3920');
		passwordInput.sendKeys('testPassword');
		repasswordInput.sendKeys('testPassword');

		const Button = await driver.findElement(By.css('button'));
		await Button.click();

		await driver.wait(until.elementIsNotVisible(repasswordInput), 10000);
		await Button.click();

		const verificationCodeInput = await driver.wait(
			until.elementLocated(By.id('verificationCode')),
			10000
		);
		await driver.wait(until.elementIsVisible(verificationCodeInput), 10000);

		// Wait for the groups dropdown to be populated
		const groupsDropdown = await driver.wait(
			until.elementLocated(By.id('groupSelect')),
			10000
		);
		await driver.wait(until.elementIsVisible(groupsDropdown), 10000);

		// Check if dropdown has been populated with values
		const options = await groupsDropdown.findElements(By.tagName('option'));
		assert(
			options.length > 0,
			'Dropdown should be populated with at least one option'
		);
	});
});
