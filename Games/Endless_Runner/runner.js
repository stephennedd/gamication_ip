const config = {
	type: Phaser.AUTO,
	width: 800,
	height: 600,
	scene: {
		preload: preload,
		create: create,
		update: update,
	},
	physics: {
		default: 'arcade',
		arcade: {
			gravity: { y: 0 },
		},
	},
};

const game = new Phaser.Game(config);

function preload() {
	this.load.image('road', '/../Games/Endless_Runner/assets/images/road.png');

	this.load.image(
		'background',
		'/../Games/Endless_Runner/Assets/Images/background.jpg'
	);
	this.load.image('car', '/../Games/Endless_Runner/Assets/Images/car.png');
	this.load.image('car2', '/../Games/Endless_Runner/Assets/Images/car2.png');
}

function create() {
	let car;

	function create() {
		this.add.image(400, 300, 'road');
		car = this.physics.add.sprite(400, 500, 'car');
	}
}

function update() {
	function update() {
		const cursors = this.input.keyboard.createCursorKeys();

		if (cursors.left.isDown) {
			car.setVelocityX(-200);
		} else if (cursors.right.isDown) {
			car.setVelocityX(200);
		} else {
			car.setVelocityX(0);
		}
	}
}
