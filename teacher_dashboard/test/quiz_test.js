const { Builder, By } = require('selenium-webdriver');
const assert = require('assert');


describe('quiz tests', function() {
  let driver;

  before (async function() {
    driver = await new Builder().forBrowser('chrome').build();
    const window = driver.manage().window();
    window.maximize();
  });

  after (async function() {
    await driver.quit();
  });

  it('edit quiz (GW)', async function() {
    this.timeout(12000);
    await driver.get('http://127.0.0.1:5501/teacher_dashboard/pages/login.html');
    
    const usernameField = await driver.findElement(By.id('username'));     // enter username and password
    await usernameField.sendKeys('Admin');

    const passwordField = await driver.findElement(By.id('password'));
    await passwordField.sendKeys('password123');

    const loginButton = await driver.findElement(By.id('login-button'));     // click login button
    await loginButton.click();

    await driver.sleep(300);

    const sidebarMenuDropdown = await driver.findElement(By.id('test-menu-dropdown'));
    await sidebarMenuDropdown.click();

    await driver.sleep(200);
    
    const sidebarLink = await driver.findElement(By.id('edit-quiz-link'));
    await sidebarLink.click();

    await driver.sleep(200);

    const quizName = await driver.findElement(By.id('quiz-name'));     // check the quiz name
    const quizNameText = await quizName.getText();

    const numberOfQuestions = await driver.findElement(By.id('quiz-question-count'));     // check the number of questions
    const numberOfQuestionsText = await numberOfQuestions.getText();

    const updateButton = await driver.findElement(By.id('edit-quiz-button'));
    await updateButton.click();

    await driver.sleep(200);

    const quizNameField = await driver.findElement(By.id('modal-quiz-name'));
    await quizNameField.clear();
    const randomQuizName = Math.random().toString(36).substring(2, 15) + Math.random().toString(36).substring(2, 15);   // generate random quiz name
    await quizNameField.sendKeys(randomQuizName);

    const addQuestionButton = await driver.findElement(By.id('add-question-to-quiz'));
    await addQuestionButton.click();

    const updateQuizButton = await driver.findElement(By.id('update-quiz-button'));
    await updateQuizButton.click();

    await driver.switchTo().alert().accept(); //accept alert

    await driver.sleep(200);

    const updatedQuizName = await driver.findElement(By.id('quiz-title'));
    const updatedQuizNameText = await updatedQuizName.getText();
    const updatedNumberOfQuestions = await driver.findElement(By.id('quiz-question-count'));
    const updatedNumberOfQuestionsText = await updatedNumberOfQuestions.getText();

    assert.notEqual(quizNameText, updatedQuizNameText);
    assert.notEqual(numberOfQuestionsText, updatedNumberOfQuestionsText);
});
});
