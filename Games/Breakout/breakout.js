class BootScene extends Phaser.Scene {
	constructor() {
		super({ key: 'BootScene' });
		this.brickColors = ['blue', 'red', 'green', 'yellow', 'silver', 'purple'];
	}

	preload() {
		this.load.image('ball', 'assets/images/ball.png');
		this.load.image('paddle', 'assets/images/paddle.png');
		this.brickColors.forEach((color) => {
			this.load.image(color, 'assets/images/' + color + '.png');
		});
		this.load.image('heart', 'assets/images/heart.png');
	}

	create() {
		// Wait for the font to be loaded before starting the next scene
		this.fontsReady = false;
		this.fontsLoaded = this.fontsLoaded.bind(this);
		document.fonts.load('10pt "PressStart2P"').then(this.fontsLoaded);
	}

	fontsLoaded() {
		this.fontsReady = true;
		this.scene.start('StartScene');
	}
}

class StartScene extends Phaser.Scene {
	constructor() {
		super({ key: 'StartScene' });
	}

	create() {
		this.startScreenText = this.add
			.text(
				this.sys.game.config.width / 2,
				this.sys.game.config.height / 2,
				'Press the left mouse button to start',
				{
					fontFamily: 'PressStart2P',
					fontSize: '20px',
					fill: '#fff',
				}
			)
			.setOrigin(0.5);

		this.input.once(
			'pointerdown',
			function () {
				this.scene.start('MainScene');
			},
			this
		);
	}
}

class MainScene extends Phaser.Scene {
	constructor() {
		super({ key: 'MainScene' });

		this.gameWidth = 800;
		this.gameHeight = 600;

		this.bricks;
		this.brickWidth = 64;
		this.brickHeight = 32;
		this.brickSpacing = 10;
		this.brickTimer;
		this.bricksDestroyed = 0;
		this.brickColors = ['blue', 'red', 'green', 'yellow', 'silver', 'purple'];

		this.paddle;
		this.ball;

		this.lifes = 3;
		this.score = 0;
		this.startTime = 0;
		this.gameOver = false;
		this.scoreText;
		this.isPaused = false;

		this.livesSprites = [];
		this.heartSpacing = 30; // Spacing between hearts
		this.heartX = this.gameWidth - 70; // X position of the first heart
		this.heartY = 10; // Y position of the hearts
	}

	create() {
		this.initializeVariables();
		this.attachEventListeners();
		this.createBall();
		this.createPaddle();
		this.createScoreText();
		this.createLivesDisplay();
		this.createColliders();
		this.generateBricksRow();
	}

	initializeVariables() {
		this.bricks = this.physics.add.group();
		this.bricksDestroyed = 0;
		this.score = 0;
		this.startTime = 0;
		this.gameOver = false;
		this.physics.world.setBoundsCollision(true, true, true, false);

		// Check for localstorage items for extraLife and scoreMultiplier
		const gameDataLife = localStorage.getItem('extraLife');
		const gameDataScore = localStorage.getItem('scoreMultiplier');

		// Update lives based on localstorage
		if (gameDataLife === 'true') {
			this.lifes = 4; // Add 1 extra life
		} else {
			this.lifes = 3;
		}

		// Update score multiplier based on localstorage
		if (gameDataScore === 'true') {
			this.scoreMultiplier = 2; // Multiply score by 2
		} else {
			this.scoreMultiplier = 1;
		}
	}

	attachEventListeners() {
		this.input.keyboard.on('keydown-P', this.togglePause, this);
		window.addEventListener('blur', this.pauseGame.bind(this));
		window.addEventListener('visibilitychange', this.pauseGame.bind(this));
	}

	createBall() {
		this.ball = this.physics.add
			.image(400, 500, 'ball')
			.setCollideWorldBounds(true)
			.setBounce(1);
		this.ball.setData('onPaddle', true);
	}

	createPaddle() {
		this.paddle = this.physics.add.image(400, 550, 'paddle').setImmovable();

		this.input.on(
			'pointermove',
			function (pointer) {
				this.paddle.x = Phaser.Math.Clamp(pointer.x, 52, 748);

				if (this.ball.getData('onPaddle')) {
					this.ball.x = this.paddle.x;
				}
			},
			this
		);

		this.input.on(
			'pointerup',
			function (pointer) {
				if (this.ball.getData('onPaddle')) {
					this.ball.setVelocity(-75, -300);
					this.ball.setData('onPaddle', false);
				}
			},
			this
		);
	}

	createScoreText() {
		this.scoreText = this.add.text(16, 16, 'Score: 0', {
			fontFamily: 'PressStart2P',
			fontSize: '22px',
			fill: '#fff',
		});
	}

	createLivesDisplay() {
		this.livesSprites = [];
		for (let i = 0; i < this.lifes; i++) {
			let heart = this.add
				.image(this.heartX + i * this.heartSpacing, this.heartY, 'heart')
				.setOrigin(2.5, 0);
			heart.setScale(0.055);
			this.livesSprites.push(heart);
		}
	}

	createColliders() {
		this.physics.add.collider(
			this.ball,
			this.paddle,
			this.hitPaddle,
			null,
			this
		);
	}

	pauseGame() {
		if (this.isPaused) return; // Prevent pausing multiple times
		this.isPaused = true;
		this.physics.pause();
		this.time.paused = true;
		this.pauseCover = this.add.rectangle(
			0,
			0,
			this.gameWidth,
			this.gameHeight,
			0x000000,
			0.7
		);
		this.pauseCover.setOrigin(0, 0);
		this.pausedText = this.add
			.text(this.gameWidth / 2, this.gameHeight / 2, 'Paused', {
				fontFamily: 'PressStart2P',
				fontSize: '64px',
				fill: '#fff',
			})
			.setOrigin(0.5);
		this.pausedText2 = this.add
			.text(
				this.gameWidth / 2,
				this.gameHeight / 2 + 100,
				'Press P to resume',
				{
					fontFamily: 'PressStart2P',
					fontSize: '32px',
					fill: '#fff',
				}
			)
			.setOrigin(0.5);
	}

	togglePause() {
		this.isPaused = !this.isPaused;

		if (this.isPaused) {
			// Pause the game
			this.physics.pause();
			this.time.paused = true; // pause timers

			// Add a semi-transparent background to darken the scene
			this.pauseCover = this.add.rectangle(
				0,
				0,
				this.gameWidth,
				this.gameHeight,
				0x000000,
				0.7
			);
			this.pauseCover.setOrigin(0, 0);

			// Add paused text
			this.pausedText = this.add
				.text(this.gameWidth / 2, this.gameHeight / 2, 'Paused', {
					fontFamily: 'PressStart2P',
					fontSize: '64px',
					fill: '#fff',
				})
				.setOrigin(0.5);
			this.pausedText2 = this.add
				.text(
					this.gameWidth / 2,
					this.gameHeight / 2 + 100,
					'Press P to resume',
					{
						fontFamily: 'PressStart2P',
						fontSize: '32px',
						fill: '#fff',
					}
				)
				.setOrigin(0.5);
		} else {
			// Resume the game
			this.physics.resume();
			this.time.paused = false; // resume timers

			// Remove the paused text and background
			this.pausedText.destroy();
			this.pausedText2.destroy();
			this.pauseCover.destroy();
		}
	}

	update() {
		if (this.isPaused) {
			return; // Don't execute the update loop if the game is paused
		}
		if (this.gameOver) {
			this.gameOver = true;
			sendScore(this.score);
			this.showGameOverScreen();
			return;
		}

		if (this.ball.y > 600) {
			this.resetBall();
		}

		// Check if any bricks have reached the bottom
		let gameOver = false;
		this.bricks.children.iterate((brick) => {
			if (brick.active && brick.y >= this.sys.game.config.height) {
				gameOver = true;
			}
		});

		// Handle game over if a brick reached the bottom
		if (gameOver) {
			this.gameOver = true; // Set the class's gameOver property to true
			return; // Exit the update method early, the game over logic will be handled on the next frame.
		}

		// Check if the game window is hidden
		const gameWindow = document.getElementById('game');
		if (gameWindow.classList.contains('hide') & (gameOver == false)) {
			this.togglePause();
		}

		window.ballPosition = {
			x: this.ball.x,
			y: this.ball.y,
		};
	}

	startGame() {
		this.startTime = this.time.now;
	}

	generateBricksRow() {
		// Create the bricks in a row at the top of the screen
		if (!this.bricks) {
			this.bricks = this.physics.add.group();
		}

		const xStartPosition =
			(this.gameWidth - (this.brickWidth + this.brickSpacing) * 10) / 2;
		const brickCount = Phaser.Math.Between(1, 8); // Random number of bricks in each row

		const brickPositions = Phaser.Utils.Array.NumberArray(0, 10); // Array of positions for bricks

		for (let i = 0; i < brickCount; i++) {
			const randomIndex = Phaser.Math.RND.integerInRange(
				0,
				brickPositions.length - 1
			);
			const positionIndex = brickPositions.splice(randomIndex, 1)[0];

			const x =
				xStartPosition + positionIndex * (this.brickWidth + this.brickSpacing);
			const y = this.brickHeight / 2;
			const brickColor = Phaser.Utils.Array.GetRandom(this.brickColors);

			const brick = this.bricks.create(x, y, brickColor).setOrigin(0.5, 0.5);
			brick.setDisplaySize(this.brickWidth, this.brickHeight);
			brick.setImmovable(true);
		}

		// Move the previous rows down
		this.bricks.children.iterate((brick) => {
			brick.y += this.brickHeight + this.brickSpacing;
		});

		// Schedule the next row generation
		if (!this.gameOver) {
			this.brickTimer = this.time.addEvent({
				delay: 5000,
				callback: () => this.generateBricksRow(),
				callbackScope: this,
				loop: false,
			});
		}

		this.physics.add.collider(
			this.ball,
			this.bricks,
			this.hitBrick,
			null,
			this
		);
	}

	hitBrick(ball, brick) {
		brick.disableBody(true, true);

		// Multiply the score by the score multiplier
		this.score += 10 * this.scoreMultiplier;

		this.bricksDestroyed++;
		this.scoreText.setText('Score: ' + this.score);
	}

	resetBall() {
		this.ball.setVelocity(0);
		this.ball.setPosition(this.paddle.x, 500);
		this.ball.setData('onPaddle', true);
		this.lifes--;

		if (this.lifes === 0) {
			this.gameOver = true;
		} else {
			let firstHeart = this.livesSprites.shift();
			firstHeart.destroy();
		}
	}

	hitPaddle(ball, paddle) {
		let diff = 0;

		if (ball.x < paddle.x) {
			//  Ball is on the left-hand side of the paddle
			diff = paddle.x - ball.x;
			ball.setVelocityX(-10 * diff);
		} else if (ball.x > paddle.x) {
			//  Ball is on the right-hand side of the paddle
			diff = ball.x - paddle.x;
			ball.setVelocityX(10 * diff);
		} else {
			//  Ball is perfectly in the middle
			//  Add a little random X to stop it bouncing straight up!
			ball.setVelocityX(2 + Math.random() * 8);
		}
	}

	showGameOverScreen() {
		this.scene.start('GameOverScene', {
			score: this.score,
			bricksDestroyed: this.bricksDestroyed, // assuming you count this somewhere in your code
			time: Math.floor((this.time.now - this.startTime) / 1000),
		});
	}
}

class GameOverScene extends Phaser.Scene {
	constructor() {
		super({ key: 'GameOverScene' });
		this.score = 0;
		this.bricksDestroyed = 0;
		this.time = 0;
	}

	init(data) {
		this.score = data.score;
		this.bricksDestroyed = data.bricksDestroyed;
		this.time = data.time;
	}

	create() {
		const gameOverText = this.add.text(
			this.sys.game.config.width / 2,
			this.sys.game.config.height / 4,
			'Game Over',
			{
				fontFamily: 'PressStart2P',
				fontSize: '64px',
				fill: '#fff',
				align: 'center',
			}
		);
		gameOverText.setOrigin(0.5);

		const scoreText = this.add.text(
			this.sys.game.config.width / 2,
			this.sys.game.config.height / 2 - 50,
			'Score: ' + this.score,
			{
				fontFamily: 'PressStart2P',
				fontSize: '32px',
				fill: '#fff',
				align: 'justify',
			}
		);
		scoreText.setOrigin(0.5);

		const bricksDestroyedText = this.add.text(
			this.sys.game.config.width / 2,
			this.sys.game.config.height / 2,
			'Bricks Destroyed: ' + this.bricksDestroyed,
			{
				fontFamily: 'PressStart2P',
				fontSize: '32px',
				fill: '#fff',
				align: 'justify',
			}
		);

		bricksDestroyedText.setOrigin(0.5);

		const timeText = this.add.text(
			this.sys.game.config.width / 2,
			this.sys.game.config.height / 2 + 50,
			'Time: ' + this.time + 's',
			{
				fontFamily: 'PressStart2P',
				fontSize: '32px',
				fill: '#fff',
				align: 'justify',
			}
		);

		timeText.setOrigin(0.5);

		this.pressToRestartText = this.add.text(
			this.sys.game.config.width / 2,
			this.sys.game.config.height / 2 + 200,
			'Press left mouse button to restart',
			{
				fontFamily: 'PressStart2P',
				fontSize: '20px',
				fill: '#fff',
				align: 'center',
			}
		);

		this.pressToRestartText.setOrigin(0.5);

		this.input.once(
			'pointerdown',
			function () {
				this.scene.start('MainScene');
			},
			this
		);
	}
}

const config = {
	type: Phaser.WEBGL,
	// backgroundColor: '#46576b',
	transparent: true,
	width: 800,
	height: 600,
	parent: 'game',
	scene: [BootScene, StartScene, MainScene, GameOverScene],
	physics: {
		default: 'arcade',
	},
};

const game = new Phaser.Game(config);

async function sendScore(score) {
	try {
		let response;
		var token = document.cookie
			.split('; ')
			.find((row) => row.startsWith('jwt='))
			.split('=')[1];
		const subject = localStorage.getItem('subject');
		// Ensure groupName is not empty or undefined
		if (!score) {
			console.error('Invalid or empty score!');
			return;
		}

		response = await fetch(
			`http://localhost:4434/api/HighScores?score=${score}&leaderboardName=${subject}`,
			{
				method: 'POST',
				headers: {
					Authorization: `Bearer ${token}`,
					'Content-Type': 'application/json',
				},
			}
		);

		if (!response.ok) {
			throw new Error(`HTTP error! status: ${response.status}`);
		}

		return true;
	} catch (error) {
		console.log('Fetch Error: ', error);
	}
}
