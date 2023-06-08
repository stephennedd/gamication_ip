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
