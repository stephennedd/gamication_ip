require('chromedriver');
const { Builder, By, until } = require('selenium-webdriver');
const { expect } = require('chai');
const assert = require('assert');
const chrome = require('selenium-webdriver/chrome');

describe('Arcade Website', function () {
	let driver;

	this.timeout(10000);

	beforeEach(async () => {
		driver = await new Builder()
			.forBrowser('chrome')
			.setChromeOptions(new chrome.Options().headless())
			.build();
		await driver.get('http://localhost:5501/ArcadeMachine/');
	});

	// Quit the driver after each test
	afterEach(async () => {
		await driver.quit();
	});

	it('INT016 - should add classes when moveIn button is clicked', async () => {
		const moveInButton = await driver.findElement(By.id('animate'));
		await moveInButton.click();

		const arcadeMachine = await driver.findElement(By.id('machine'));
		const background = await driver.findElement(By.id('background'));

		await driver.sleep(2000);

		const arcadeMachineClass = await arcadeMachine.getAttribute('class');
		const backgroundClass = await background.getAttribute('class');

		expect(arcadeMachineClass).to.include('move-out-machine');
		expect(backgroundClass).to.include('move-out');
	});

	it('INT017 - should remove classes when moveOut button is clicked', async () => {
		const moveInButton = await driver.findElement(By.id('animate'));
		const moveOutButton = await driver.findElement(By.id('back'));

		await moveInButton.click();

		await driver.sleep(2000);

		await moveOutButton.click();

		await driver.sleep(2000);

		const arcadeMachine = await driver.findElement(By.id('machine'));
		const background = await driver.findElement(By.id('background'));

		const arcadeMachineClass = await arcadeMachine.getAttribute('class');
		const backgroundClass = await background.getAttribute('class');

		expect(arcadeMachineClass).not.to.include('move-out-machine');
		expect(backgroundClass).not.to.include('move-out');
	});

	it('INT018 - should have all initially visible elements', async function () {
		// Set a generous timeout
		this.timeout(20000);

		// Navigate to your page
		await driver.get('http://localhost:5501/ArcadeMachine/'); // replace with the URL of your page

		// Find elements by id
		const arcadeMachine = await driver.findElement(By.id('machine'));
		const moveIn = await driver.findElement(By.id('animate'));
		const moveOut = await driver.findElement(By.id('back'));
		const background = await driver.findElement(By.id('background'));

		// Assert elements are present
		assert.ok(arcadeMachine);
		assert.ok(moveIn);
		assert.ok(moveOut);
		assert.ok(background);

		// Check if elements are displayed
		const isArcadeMachineDisplayed = await arcadeMachine.isDisplayed();
		const isMoveInDisplayed = await moveIn.isDisplayed();
		const isMoveOutDisplayed = await moveOut.isDisplayed();
		const isBackgroundDisplayed = await background.isDisplayed();

		// Assert elements are displayed
		assert.ok(isArcadeMachineDisplayed);
		assert.ok(isMoveInDisplayed);
		assert.ok(isMoveOutDisplayed);
		assert.ok(isBackgroundDisplayed);
	});
});
