class BootScene extends Phaser.Scene {
	constructor() {
		super({ key: 'BootScene' });
		this.brickColors = ['blue', 'red', 'green', 'yellow', 'silver', 'purple'];
	}

	preload() {
		this.load.image('ball', '/../Games/Breakout/assets/images/ball.png');
		this.load.image('paddle', '/../Games/Breakout/assets/images/paddle.png');
		this.brickColors.forEach((color) => {
			this.load.image(
				color,
				'/../Games/Breakout/assets/images/' + color + '.png'
			);
		});
		this.load.image('heart', '/../Games/Breakout/assets/images/heart.png');
		// this.load.bitmapFont(
		// 	'PressStart2P',
		// 	'/../Games/Breakout/assets/Fonts/Press_Start_2P/PressStart2P-Regular.png',
		// );
	}

	create() {
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
					fontSize: '32px',
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

		this.livesSprites = [];
		this.heartSpacing = 30; // Spacing between hearts
		this.heartX = this.gameWidth - 70; // X position of the first heart
		this.heartY = 10; // Y position of the hearts
	}

	create() {
		// dirty way to fix issue with game not restarting
		// TODO: find a better way to fix this
		// TODO: there is a bug where if you click when ball is about to fall it will restart the game
		// TODO: after restart lives are not decreasing

		this.isPaused = false; // To track if the game is paused
		this.input.keyboard.on('keydown-P', this.togglePause, this);

		this.bricks = this.physics.add.group();
		this.bricksDestroyed = 0;
		this.lifes = 3;
		this.score = 0;
		this.startTime = 0;
		this.gameOver = false;

		//  Enable world bounds, but disable the floor
		this.physics.world.setBoundsCollision(true, true, true, false);

		// create ball and paddle
		this.ball = this.physics.add
			.image(400, 500, 'ball')
			.setCollideWorldBounds(true)
			.setBounce(1);
		this.ball.setData('onPaddle', true);

		this.paddle = this.physics.add.image(400, 550, 'paddle').setImmovable();

		// display score
		this.scoreText = this.add.text(16, 16, 'Score: 0', {
			fontFamily: 'PressStart2P',
			fontSize: '22px',
			fill: '#fff',
		});

		// display lives
		for (let i = 0; i < this.lifes; i++) {
			let heart = this.add
				.image(this.heartX + i * this.heartSpacing, this.heartY, 'heart')
				.setOrigin(1, 0);
			heart.setScale(0.055);
			this.livesSprites.push(heart);
		}

		// collider between ball and paddle
		this.physics.add.collider(
			this.ball,
			this.paddle,
			this.hitPaddle,
			null,
			this
		);

		//  Input events
		this.input.on(
			'pointermove',
			function (pointer) {
				//  Keep the paddle within the game
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

		this.generateBricksRow();
		this.startTime = this.time.now;
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
		} else {
			// Resume the game
			this.physics.resume();
			this.time.paused = false; // resume timers

			// Remove the paused text and background
			this.pausedText.destroy();
			this.pauseCover.destroy();
		}
	}

	update() {
		if (this.isPaused) {
			return; // Don't execute the update loop if the game is paused
		}
		if (this.gameOver) {
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

		if (gameOver) {
			this.gameOver = true;

			this.showGameOverScreen();
			return;
		}

		// Check if the game window is hidden
		const gameWindow = document.getElementById('game');
		if (gameWindow.classList.contains('hide')) {
			this.togglePause();
		}
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
		this.score += 10;
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
    // ...existing code...

    const score = this.score;
    const leaderboardName = 'main';

    // Get JWT from cookies (replace 'jwtCookieName' with the actual name of the JWT cookie)
    var token = document.cookie
  .split('; ')
  .find(row => row.startsWith('jwt='))
  .split('=')[1];

    // Send POST request to the endpoint
    if (token) {
        const requestOptions = {
            method: 'POST',
            headers: {
                'Content-Type': 'application/json',
                Authorization: `Bearer ${token}`,
            },
            body: JSON.stringify({ score, leaderboardName }),
        };

        fetch('https://localhost:7186/api/HighScores', requestOptions)
            .then(response => {
                if (!response.ok) {
                    throw new Error('Failed to send the high score.');
                }
                // Handle the response if needed
            })
            .catch(error => {
                console.error(error);
                // Handle the error if needed
            });
    } else {
        console.warn('JWT not found in cookies. High score not submitted.');
    }

    this.scene.start('GameOverScene', {
        score: this.score,
        bricksDestroyed: this.bricksDestroyed,
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
	backgroundColor: '#46576b',
	width: 800,
	height: 600,
	parent: 'game',
	scene: [BootScene, StartScene, MainScene, GameOverScene],
	physics: {
		default: 'arcade',
	},
};

const game = new Phaser.Game(config);
