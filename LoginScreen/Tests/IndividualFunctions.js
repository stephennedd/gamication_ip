require('chromedriver');
const { Builder, By, until } = require('selenium-webdriver');
const assert = require('assert');

describe('Checking', function () {
	let driver;

	before(async function () {
		driver = await new Builder().forBrowser('chrome').build();
	});

	after(async function () {
		await driver.quit();
	});

	it('Test parseJwt()', async function () {});

	it('Test refreshJWT()', async function () {});

	it('Test verifyCode()', async function () {});

	it('Test fetchGroupNames()', async function () {});

	it('Test populateGroupsDropdown()', async function () {});

	it('Test assignGroup()', async function () {});
});
