window.addEventListener('DOMContentLoaded', function () {
	const arcadeMachine = document.getElementById('machine');
	const moveIn = document.getElementById('animate');
	const moveOut = document.getElementById('back');
	const background = document.getElementById('background');
	let gameButton,
		quizButton,
		quiz,
		leaderboardButton,
		leaderboard,
		game,
		gameWindow;
	// just for testing assigned true, delete later
	const quizPassed = localStorage.getItem('quizPassed') || 'true';

	const unpressedImages = {
		moveIn: 'Media/Images/up.png',
		moveOut: 'Media/Images/down.png',
		gameButton: 'Media/Images/play.png',
		quizButton: 'Media/Images/quiz.png',
		leaderboardButton: 'Media/Images/leaderboard.png',
	};

	const pressedImages = {
		moveIn: 'Media/Images/upPressed.png',
		moveOut: 'Media/Images/downPressed.png',
		gameButton: 'Media/Images/playPressed.png',
		quizButton: 'Media/Images/quizPressed.png',
		leaderboardButton: 'Media/Images/leaderboardPressed.png',
	};

	// Initialize the elements that might not exist initially
	function initializeElements() {
		gameButton = document.getElementById('showGame');
		quizButton = document.getElementById('showQuiz');
		leaderboardButton = document.getElementById('leaderboardButton');

		game = document.getElementById('game');
		leaderboard = document.getElementById('leaderboard');
		quiz = document.getElementById('quiz');

		if (
			gameButton &&
			quizButton &&
			quiz &&
			leaderboardButton &&
			leaderboard &&
			game
		) {
			// Elements exist, attach event listeners
			attachEventListeners();
		} else {
			// Elements not found, wait and try again
			setTimeout(initializeElements, 100);
		}
	}

	function attachEventListeners() {
		window.addEventListener('message', function (event) {
			if (event.data.action === 'showGame') {
				showGame();
			}
		});

		moveIn.addEventListener('click', function () {
			moveCorridor();
			moveIn.src = pressedImages.moveIn;
			moveOut.src = unpressedImages.moveOut;
		});

		moveOut.addEventListener('click', function () {
			walkBack();
			moveOut.src = pressedImages.moveOut;
			moveIn.src = unpressedImages.moveIn;
		});

		quizButton.addEventListener('click', function () {
			showQuiz();
			quizButton.src = pressedImages.quizButton;
			gameButton.src = unpressedImages.gameButton;
			leaderboardButton.src = unpressedImages.leaderboardButton;
		});

		gameButton.addEventListener('click', function () {
			showGame();
			gameButton.src = pressedImages.gameButton;
			quizButton.src = unpressedImages.quizButton;
			leaderboardButton.src = unpressedImages.leaderboardButton;
		});

		leaderboardButton.addEventListener('click', function () {
			showLeaderboard();
			leaderboardButton.src = pressedImages.leaderboardButton;
			quizButton.src = unpressedImages.quizButton;
			gameButton.src = unpressedImages.gameButton;
		});

		// await for message 'showButtons' fire up showButtons function
		window.addEventListener('message', function (event) {
			if (event.data.action === 'showButtons') {
				showButtons();
			}
		});
	}

	function moveCorridor() {
		arcadeMachine.classList.add('move-out-machine');
		background.classList.add('move-out');

		setTimeout(() => {
			arcadeMachine.classList.remove('blur');
		}, 1200);

		setTimeout(() => {
			quiz.classList.remove('hide');

			// If quiz is passed, show buttons
			if (quizPassed === 'true') {
				setTimeout(() => {
					showButtons();
				}, 150);
			}
		}, 1850);
	}

	function walkBack() {
		hideAllScreens();

		arcadeMachine.classList.remove('move-out-machine');
		background.classList.remove('move-out');

		setTimeout(() => {
			arcadeMachine.classList.add('blur');
			hideButtons();
		}, 1300);
	}

	function showButtons() {
		document.getElementById('buttonsPanel').style.width = '650px';
		quizButton.classList.remove('hide');
		gameButton.classList.remove('hide');
		leaderboardButton.classList.remove('hide');
	}

	function hideButtons() {
		quizButton.classList.add('hide');
		gameButton.classList.add('hide');
		leaderboardButton.classList.add('hide');
		document.getElementById('buttonsPanel').style.width = '250px';
	}

	function showQuiz() {
		pressPause();
		if (arcadeMachine.classList.contains('move-out-machine')) {
			hideAllScreens();
			quiz.classList.remove('hide');
		}
	}

	function showLeaderboard() {
		pressPause();
		if (arcadeMachine.classList.contains('move-out-machine')) {
			hideAllScreens();
			leaderboard.classList.remove('hide');
		}
	}

	function showGame() {
		if (arcadeMachine.classList.contains('move-out-machine')) {
			hideAllScreens();
			game.classList.remove('hide');
			gameWindow = game.contentWindow;
		}
	}

	function hideAllScreens() {
		game.classList.add('hide');
		quiz.classList.add('hide');
		leaderboard.classList.add('hide');
	}

	function pressPause() {
		if (!game.classList.contains('hide')) {
			const message = {
				action: 'pauseGame',
			};

			// Send the message to the game iframe
			gameWindow.postMessage(message, '*');
		}
	}

	initializeElements();
});
