// const canvas = document.getElementById('game');
// const arcadeMachine = document.getElementById('machine');
// const button = document.getElementById('animate');
// const back = document.getElementById('back');
// const background = document.getElementById('background');
// const quizButton = document.getElementById('showQuiz');
// const quiz = document.getElementById('quiz');
// const leaderboardButton = document.getElementById('leaderboardButton');

// button.addEventListener('click', moveCorridor);
// back.addEventListener('click', walkBack);
// quizButton.addEventListener('click', showQuiz);
// leaderboardButton.addEventListener('click', showLeaderboard);

// function moveCorridor() {
// 	arcadeMachine.classList.add('move-out-machine');
// 	background.classList.add('move-out');

// 	setTimeout(() => {
// 		arcadeMachine.classList.remove('blur');
// 	}, 1200);

// 	setTimeout(() => {
// 		canvas.classList.remove('hide');
// 	}, 1850);

// 	// setTimeout(() => {
// 	// 	canvas.classList.add('crtOn');
// 	// }, 1850);
// }

// function walkBack() {
// 	arcadeMachine.classList.remove('move-out-machine');

// 	// canvas.classList.remove('crtOn');
// 	// canvas.classList.add('crtOff');
// 	// setTimeout(() => {
// 	// 	canvas.classList.add('hide');
// 	// }, 500);

// 	canvas.classList.add('hide');
// 	quiz.classList.add('hide');
// 	leaderboard.classList.add('hide');

// 	background.classList.remove('move-out');
// 	setTimeout(() => {
// 		arcadeMachine.classList.add('blur');
// 	}, 1300);
// }

// function showQuiz() {
// 	if (quiz.classList.contains('hide')) {
// 		canvas.classList.add('hide');
// 		quiz.classList.remove('hide');
// 	} else {
// 		quiz.classList.add('hide');
// 		canvas.classList.remove('hide');
// 	}
// }

// function showLeaderboard() {
// 	if (leaderboard.classList.contains('hide')) {
// 		canvas.classList.add('hide');
// 		quiz.classList.add('hide');
// 		leaderboard.classList.remove('hide');
// 	} else {
// 		leaderboard.classList.add('hide');
// 		canvas.classList.remove('hide');
// 	}
// }

const canvas = document.getElementById('game');
const arcadeMachine = document.getElementById('machine');
const moveIn = document.getElementById('animate');
const moveOut = document.getElementById('back');
const background = document.getElementById('background');
const gameButton = document.getElementById('showGame');
const quizButton = document.getElementById('showQuiz');
const quiz = document.getElementById('quiz');
const leaderboardButton = document.getElementById('leaderboardButton');
const leaderboard = document.getElementById('leaderboard');
const screens = document.querySelectorAll('.screen');

moveIn.addEventListener('click', moveCorridor);
moveOut.addEventListener('click', walkBack);
quizButton.addEventListener('click', showQuiz);
gameButton.addEventListener('click', showGame);
leaderboardButton.addEventListener('click', showLeaderboard);

function moveCorridor() {
	arcadeMachine.classList.add('move-out-machine');
	background.classList.add('move-out');

	setTimeout(() => {
		arcadeMachine.classList.remove('blur');
	}, 1200);

	setTimeout(() => {
		canvas.classList.remove('hide');
	}, 1850);
}

function walkBack() {
	hideAllScreens();

	arcadeMachine.classList.remove('move-out-machine');
	background.classList.remove('move-out');

	setTimeout(() => {
		arcadeMachine.classList.add('blur');
	}, 1300);
}

function showQuiz() {
	if (arcadeMachine.classList.contains('move-out-machine')) {
		hideAllScreens();
		quiz.classList.remove('hide');
	}
}

function showLeaderboard() {
	if (arcadeMachine.classList.contains('move-out-machine')) {
		hideAllScreens();
		leaderboard.classList.remove('hide');
	}
}

function showGame() {
	if (arcadeMachine.classList.contains('move-out-machine')) {
		hideAllScreens();
		canvas.classList.remove('hide');
	}
}

function hideAllScreens() {
	canvas.classList.add('hide');
	quiz.classList.add('hide');
	leaderboard.classList.add('hide');
}
