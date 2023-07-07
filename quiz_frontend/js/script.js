//selecting all required elements
const start_btn = document.querySelector('.start_btn button');
const info_box = document.querySelector('.info_box');
const exit_btn = info_box.querySelector('.buttons .quit');
const play_btn = info_box.querySelector('.buttons .play');
const continue_btn = info_box.querySelector('.buttons .restart');
const quiz_box = document.querySelector('.quiz_box');
const result_box = document.querySelector('.result_box');
const option_list = document.querySelector('.option_list');
const mute_btn = document.querySelector('.mute_btn');
const progressBar = document.querySelector('.progress_container');
const achievement = document.querySelector('.achievement');
const achievementText = document.querySelector('.achievement-text');
const achievementIcon = document.querySelector('.achievement-icon');
var extraLife = false;
var scoreMultiplier = false;
var quizPassed = false;
var achievementList = [55, 75, 100];

//import { showGame } from "../../ArcadeMachine/main";

let winSound = new Audio('./assets/sounds/win.mp3');
let loseSound = new Audio('./assets/sounds/game-over.mp3');
let bgSound = new Audio('./assets/sounds/bg.mp3');
let achievementSound = new Audio('./assets/sounds/achievement.mp3');

let studentResult;

let apiURL = 'https://aad-gamification.azurewebsites.net/';

// if mute button clicked
mute_btn.onclick = () => {
	if (bgSound.muted) {
		bgSound.muted = false;
		mute_btn.innerHTML = '<i class="fa-sharp fa-solid fa-volume-high"></i>';
	} else {
		bgSound.muted = true;
		mute_btn.innerHTML = '<i class="fa-sharp fa-solid fa-volume-xmark"></i>';
	}
};

async function getGeneratedTestForStudent() {
	const subjectName = localStorage.getItem('subject'); // Replace with your desired subject name
	const response = await fetch(
		`${apiURL}api/subjects/${subjectName}/test`, {
			mode: 'no-cors'
		}
	);
	const data = await response.json();
	let testId = data;
	console.log(`Test ID for ${subjectName}: ${testId}`);

	var token = document.cookie
		.split('; ')
		.find((row) => row.startsWith('jwt='))
		.split('=')[1];
	var decodedToken = parseJwt(token);
	console.log(decodedToken);
	var studentId = decodedToken['Id'];
	console.log(studentId);
	//var studentId = decodedToken["http://schemas.xmlsoap.org/ws/2005/05/identity/claims/name"];
	///console.log(studentId);
	try {
		// Fetch data from the API
		const response = await fetch('${apiURL}api/generatedTests', {
			method: 'POST',
			headers: {
				mode: 'no-cors',
				contentType: 'application/json',
				// 'Content-Type': 'application/json',
			},
			body: JSON.stringify({
				studentId: studentId,
				testId: testId,
				numberOfQuestions: 5,
			}),
		});

		const response2 = await fetch(
			`${apiURL}api/generatedTests/${studentId}/${testId}`
		);
		const data = await response2.json();
		questions = data['Questions'];
		generatedTestId = data['Id'];
		console.log(generatedTestId);
		console.log(questions);
	} catch (error) {
		console.error('Error fetching questions:', error);
	}
}

function parseJwt(token) {
	var base64Url = token.split('.')[1];
	var base64 = base64Url.replace(/-/g, '+').replace(/_/g, '/');
	var jsonPayload = decodeURIComponent(
		atob(base64)
			.split('')
			.map(function (c) {
				return '%' + ('00' + c.charCodeAt(0).toString(16)).slice(-2);
			})
			.join('')
	);

	return JSON.parse(jsonPayload);
}

// if startQuiz button clicked
start_btn.onclick = async () => {
	await getGeneratedTestForStudent();
	info_box.classList.add('activeInfo'); //show info box
};
// if exitQuiz button clicked
exit_btn.onclick = () => {
	info_box.classList.remove('activeInfo'); //hide info box
};
// if continueQuiz button clicked
continue_btn.onclick = () => {
	info_box.classList.remove('activeInfo'); //hide info box
	quiz_box.classList.add('activeQuiz'); //show quiz box
	showQuestions(0); //calling showQestions function
	queCounter(1); //passing 1 parameter to queCounter
	bgSound.play();
};
let que_count = 0;
let que_numb = 1;
let userScore = 0;
let counter;
let counterLine;
let widthValue = 0;
const restart_quiz = result_box.querySelector('.buttons .restart');
const play_game = result_box.querySelector('.buttons .play');
// if restartQuiz button clicked
restart_quiz.onclick = async () => {
	await getGeneratedTestForStudent();
	quiz_box.classList.add('activeQuiz'); //show quiz box
	result_box.classList.remove('activeResult'); //hide result box
	que_count = 0;
	que_numb = 1;
	userScore = 0;
	widthValue = 0;
	showQuestions(que_count); //calling showQestions function
	clearInterval(counter); //clear counter
	clearInterval(counterLine); //clear counterLine
	next_btn.classList.remove('show'); //hide the next button
	resetProgress();
};
// function callShowGame() {
//   if (typeof showGame === "function") {
//     showGame();
//   } else {
//     console.error("showGame is not a function");
//   }
// }

function callShowGame() {
	// Store extraLife and scoreMultiplier in localStorage
	localStorage.setItem('extraLife', extraLife);
	localStorage.setItem('scoreMultiplier', scoreMultiplier);

	// Post a message to the parent document asking it to call showGame
	parent.postMessage({ action: 'showGame' }, '*');
	parent.postMessage({ action: 'showButtons' }, '*');
	// create a new file in local storage that will save the fact that student has finished the test
	localStorage.setItem('quizPassed', quizPassed);
}
// if quitQuiz button clicked
play_game.onclick = () => {
	callShowGame();
};
const next_btn = document.querySelector('footer .next_btn');
const bottom_ques_counter = document.querySelector('footer .total_que');
// if Next Que button clicked
next_btn.onclick = async () => {
	if (que_count < questions.length - 1) {
		//if question count is less than total question length
		que_count++; //increment the que_count value
		que_numb++; //increment the que_numb value
		showQuestions(que_count); //calling showQestions function
		clearInterval(counter); //clear counter
		clearInterval(counterLine); //clear counterLine
		next_btn.classList.remove('show'); //hide the next button
		queCounter(que_numb); //passing que_numbfetch value to queCounter
	} else {
		var token = document.cookie
			.split('; ')
			.find((row) => row.startsWith('jwt='))
			.split('=')[1];
		var decodedToken = parseJwt(token);
		console.log(decodedToken);
		var studentId = decodedToken['Id'];
		await getStudentResult(studentId);
		clearInterval(counter); //clear counter
		clearInterval(counterLine); //clear counterLine
		showResult(); //calling showResult function
		queCounter(1); //passing que_numb value to queCounter
		bgSound.pause();
		bgSound.currentTime = 0;

		// Call getStudentResult function
	}
};
// getting questions and options from array
function showQuestions(index) {
	const que_text = document.querySelector('.que_text');
	//creating a new span and div tag for question and option and passing the value using array index
	let que_tag =
		'<span>' + (index + 1) + '. ' + questions[index].QuestionText + '</span>';
	let option_tag =
		'<div class="option" data-answer-id="' +
		questions[index].Answers[0].Id +
		'"><span>' +
		questions[index].Answers[0].AnswerText +
		'</span></div>' +
		'<div class="option" data-answer-id="' +
		questions[index].Answers[1].Id +
		'"><span>' +
		questions[index].Answers[1].AnswerText +
		'</span></div>' +
		'<div class="option" data-answer-id="' +
		questions[index].Answers[2].Id +
		'"><span>' +
		questions[index].Answers[2].AnswerText +
		'</span></div>' +
		'<div class="option" data-answer-id="' +
		questions[index].Answers[3].Id +
		'"><span>' +
		questions[index].Answers[3].AnswerText +
		'</span></div>';
	que_text.innerHTML = que_tag; //adding new span tag inside que_tag
	option_list.innerHTML = option_tag; //adding new div tag inside option_tag

	const option = option_list.querySelectorAll('.option');
	// set onclick attribute to all available options
	for (i = 0; i < option.length; i++) {
		option[i].setAttribute('onclick', 'optionSelected(this)');
	}
}
// creating the new div tags which for icons
let tickIconTag = '<div class="icon tick"><i class="fas fa-check"></i></div>';
let crossIconTag = '<div class="icon cross"><i class="fas fa-times"></i></div>';

async function getStudentResult(studentId) {
	try {
		const response = await fetch(
			`${apiURL}api/generatedTests/studentResults?studentId=${studentId}&generatedTestId=${generatedTestId}`
		);
		const data = await response.json();
		studentResult = data;
		console.log(studentResult);
	} catch (error) {
		console.error('Error submitting answer:', error);
	}
}

// Function to handle the student's answer submission
async function submitAnswer(answerId, studentQuestionId) {
	// Perform your fetch request here with the selected option
	try {
		// Perform your fetch request here with the selected option
		await fetch(
			`${apiURL}api/generatedTests/studentQuestions/${studentQuestionId}/answer`,
			{
				method: 'POST',
				headers: {
					'Content-Type': 'application/json',
				},
				body: JSON.stringify({
					answerId: answerId,
				}),
			}
		);

		console.log('Answer submitted successfully!');
	} catch (error) {
		console.error('Error submitting answer:', error);
	}
}
//if user clicked on option
async function optionSelected(answer) {
	const answerId = answer.getAttribute('data-answer-id');
	await submitAnswer(answerId, questions[que_count].Id);
	clearInterval(counter); //clear counter
	clearInterval(counterLine); //clear counterLine
	let userAns = answer.textContent; //getting user selected option
	let correcAns = questions[que_count].CorrectAnswer; //getting correct answer from array
	const allOptions = option_list.children.length; //getting all option items

	if (userAns == correcAns) {
		//if user selected option is equal to array's correct answer
		userScore += 1; //upgrading score value with 1
		answer.classList.add('correct'); //adding green color to correct selected option
		answer.insertAdjacentHTML('beforeend', tickIconTag); //adding tick icon to correct selected option
		console.log('Correct Answer');
		console.log('Your correct answers = ' + userScore);
		//Yehor
		console.log(questions.length);
		updateProgress(userScore, questions.length);
	} else {
		answer.classList.add('incorrect'); //adding red color to correct selected option
		answer.insertAdjacentHTML('beforeend', crossIconTag); //adding cross icon to correct selected option
		console.log('Wrong Answer');
		for (i = 0; i < allOptions; i++) {
			if (option_list.children[i].textContent == correcAns) {
				//if there is an option which is matched to an array answer
				option_list.children[i].setAttribute('class', 'option correct'); //adding green color to matched option
				option_list.children[i].insertAdjacentHTML('beforeend', tickIconTag); //adding tick icon to matched option
				console.log('Auto selected correct answer.');
			}
		}
	}
	for (i = 0; i < allOptions; i++) {
		option_list.children[i].classList.add('disabled'); //once user select an option then disabled all options
	}
	next_btn.classList.add('show'); //show the next button if user selected any option
}
function showResult() {
	info_box.classList.remove('activeInfo'); //hide info box
	quiz_box.classList.remove('activeQuiz'); //hide quiz box
	result_box.classList.add('activeResult'); //show result box
	const scoreText = result_box.querySelector('.score_text');
	const replayBtn = result_box.querySelector('.buttons .restart');
	const playBtn = result_box.querySelector('.buttons .play');

	console.log('Result is ' + studentResult);
	if (studentResult >= achievementList[0]) {
		replayBtn.style.display = 'none'; // Display the "Replay Quiz" button
		playBtn.style.display = 'block'; // Display the "Replay Quiz" button
		winSound.play(); // play win sound
	} else {
		replayBtn.style.display = 'block'; // Hide the "Replay Quiz" button
		playBtn.style.display = 'none';
		loseSound.play(); // play lose sound
	}
	if (userScore > 3) {
		// if user scored more than 3
		//creating a new span tag and passing the user score number and total question number
		let scoreTag =
			'<span>and congrats! You got  <p>' +
			userScore +
			'</p> out of <p>' +
			questions.length +
			'</p></span>';
		scoreText.innerHTML = scoreTag; //adding new span tag inside score_Text
	} else if (userScore > 1) {
		// if user scored more than 1
		let scoreTag =
			'<span>and nice, You got  <p>' +
			userScore +
			'</p> out of <p>' +
			questions.length +
			'</p></span>';
		scoreText.innerHTML = scoreTag;
	} else {
		// if user scored less than 1
		let scoreTag =
			'<span>but sorry, You got <p>' +
			userScore +
			'</p> out of <p>' +
			questions.length +
			'</p></span>';
		scoreText.innerHTML = scoreTag;
	}
}

function queCounter(index) {
	//creating a new span tag and passing the question number and total question
	let totalQueCounTag =
		'<span><p>' +
		index +
		'</p> of <p>' +
		questions.length +
		'</p> Questions</span>';
	bottom_ques_counter.innerHTML = totalQueCounTag; //adding new span tag inside bottom_ques_counter
}

// Update the progress bar and milestones based on the number of correct answers
async function updateProgress(correctAnswers, totalQuestions) {
	var progressBar = document.getElementById('progress-bar');
	var milestone50 = document.querySelector('.milestone-50');
	var milestone75 = document.querySelector('.milestone-75');
	var milestone100 = document.querySelector('.milestone-100');
	var progress = (correctAnswers / totalQuestions) * 100;
	progressBar.style.width = progress + '%';

	// Activate the milestone at 100% progress
	if (progress >= 55) {
		milestone50.classList.add('milestone-done');
		milestone50.style.backgroundColor = 'green';
		progressBar.style.width = progress + '%';

		// Activate the milestones at 55%,75%,100% progress
		if (progress >= achievementList[0]) {
			milestone50.classList.add('milestone-done');
			milestone50.style.backgroundColor = 'green';
			achievementSound.play(); // play achievement sound
			showAchievement(
				'Game Unlocked',
				`You've unlocked the game!`,
				`<i class="fa-sharp fa-solid fa-gamepad"></i>`
			); // show achievement popup
		}

		if (progress >= 75) {
			milestone75.classList.add('milestone-done');
			milestone75.style.backgroundColor = 'red';
			if (progress >= achievementList[1]) {
				extraLife = true; // enable extra life
				milestone75.classList.add('milestone-done');
				milestone75.style.backgroundColor = 'red';
				achievementSound.play(); // play achievement sound
				showAchievement(
					'Bonus Life',
					`You've unlocked +1 life`,
					`<i class="fa-sharp fa-solid fa-heart" style="color: red"></i>`
				); // show achievement popup
			}

			if (progress === 100) {
				milestone100.classList.add('milestone-done');
				milestone100.style.backgroundColor = 'var(--achievement-color)';
				showAchievement(
					'Bonus Life',
					`You've unlocked +1 life`,
					`<i class="fa-sharp fa-solid fa-heart" style="color: red"></i>`
				); // show achievement popup
			}

			if (progress === achievementList[2]) {
				scoreMultiplier = true; // enable score multiplier
				milestone100.classList.add('milestone-done');
				milestone100.style.backgroundColor = 'var(--achievement-color)';
				achievementSound.play(); // play achievement sound
				showAchievement(
					'Score Multiplier',
					`You've unlocked 2x score multiplier`,
					`<i class="fa-sharp fa-solid fa-trophy" style="color: green"></i>`
				); // show achievement popup
			}
		}

		function showAchievement(title, text, icon) {
			var achievmentTitle = document.querySelector('.achievement-title');
			// clear the achievement text and icon and title
			achievementText.innerHTML = '';
			achievementIcon.innerHTML = '';
			achievmentTitle.innerHTML = '';

			achievementText.innerHTML = text;
			achievementIcon.innerHTML = icon;
			achievmentTitle.innerHTML = title;
			console.log('Achievement unlocked: ' + text);
			achievement.classList.add('show-achievement');

			setTimeout(function () {
				achievement.classList.remove('show-achievement');
			}, 3000);
		}

		function resetProgress() {
			var progressBar = document.getElementById('progress-bar');
			var milestone50 = document.querySelector('.milestone-50');
			var milestone75 = document.querySelector('.milestone-75');
			var milestone100 = document.querySelector('.milestone-100');
			progressBar.style.width = 0 + '%';
			milestone50.classList.remove('milestone-done');
			milestone75.classList.remove('milestone-done');
			milestone100.classList.remove('milestone-done');
			milestone50.style.backgroundColor = '#ddd';
			milestone75.style.backgroundColor = '#ddd';
			milestone100.style.backgroundColor = '#ddd';
		}

		function resetProgress() {
			scoreMultiplier = false; // disable score multiplier
			extraLife = false; // disable extra life
			var progressBar = document.getElementById('progress-bar');
			var milestone50 = document.querySelector('.milestone-50');
			var milestone75 = document.querySelector('.milestone-75');
			var milestone100 = document.querySelector('.milestone-100');
			progressBar.style.width = 0 + '%';
			milestone50.classList.remove('milestone-done');
			milestone75.classList.remove('milestone-done');
			milestone100.classList.remove('milestone-done');
			milestone50.style.backgroundColor = '#ddd';
			milestone75.style.backgroundColor = '#ddd';
			milestone100.style.backgroundColor = '#ddd';
		}
	}
}
