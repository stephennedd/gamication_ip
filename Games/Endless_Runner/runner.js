var config = {
	type: Phaser.AUTO,
	width: 800,
	height: 600,
	physics: {
		default: 'arcade',
		arcade: {
			gravity: { y: 0 },
			debug: false,
		},
	},
	scene: {
		preload: preload,
		create: create,
		update: update,
	},
};

var game = new Phaser.Game(config);

function preload() {
	// Load your car and other vehicle assets here
	this.load.image('car', 'assets/car.png');
	this.load.image('otherCar', 'assets/otherCar.png');
	this.load.image('powerup', 'assets/powerup.png');
	this.load.image('truck', 'assets/truck.png');
	this.load.image('tree', 'assets/tree.png');
}

function create() {
	// Creating car controlled by the player
	player = this.physics.add.sprite(100, 450, 'car');
	cursors = this.input.keyboard.createCursorKeys();
	trees = this.physics.add.group();

	// Creating group for other vehicles
	otherCars = this.physics.add.group({
		key: 'otherCar',
		repeat: 11,
		setXY: { x: 12, y: 0, stepX: 70 },
	});

	// When creating new otherCar, randomly choose its type
	let type = Phaser.Math.RND.pick(['otherCar', 'truck']);
	let otherCar = otherCars.create(800, Phaser.Math.Between(100, 500), type);

	// You might apply different behaviors based on the type
	if (type === 'truck') {
		otherCar.setVelocityX(-100); // Trucks are slower
	} else {
		otherCar.setVelocityX(-200);
	}

	// Adding collider for car and other vehicles
	this.physics.add.collider(player, otherCars, hitOtherCar, null, this);

	powerups = this.physics.add.group();
}

function update() {
	//  Logic for player input
	if (cursors.left.isDown) {
		player.setVelocityX(-160);
	} else if (cursors.right.isDown) {
		player.setVelocityX(160);
	} else {
		player.setVelocityX(0);
	}

	// In update function
if (Phaser.Math.Between(0, 100) > 98) {
    let tree = trees.create(800, Phaser.Math.Between(100, 500), 'tree');
    tree.setVelocityX(-100);
}

// In update function, remove trees that are out of the screen
	trees.children.each(function (b) {
		if (b.x < -b.displayWidth) {
			b.destroy();
		}
	}

	// Generate new otherCars periodically and position them randomly in one of the lanes
	if (Phaser.Math.Between(0, 100) > 95) {
		let otherCar = otherCars.create(
			800,
			Phaser.Math.Between(100, 500),
			'otherCar'
		);
		otherCar.setVelocityX(-200); // They move from right to left
	}

	// Remove other cars that are out of the screen
	otherCars.children.each(function (b) {
		if (b.x < -b.displayWidth) {
			b.destroy();
		}
	});

	// In update function, increase speed over time
	let speed = baseSpeed + Math.floor(score / 1000); // Increase speed for every 1000 points of score

	// Apply this speed when you create new otherCar
	let otherCar = otherCars.create(
		800,
		Phaser.Math.Between(100, 500),
		'otherCar'
	);
	otherCar.setVelocityX(-speed);

	// Generate powerups periodically and position them randomly in one of the lanes
	if (Phaser.Math.Between(0, 100) > 98) {
		let powerup = powerups.create(
			800,
			Phaser.Math.Between(100, 500),
			'powerup'
		);
		powerup.setVelocityX(-200);
	}

	// Add a collider for the player and the powerups
	this.physics.add.overlap(player, powerups, collectPowerup, null, this);

	// And then define the collectPowerup function
	function collectPowerup(player, powerup) {
		powerup.disableBody(true, true);

		// Add powerup effect here
	}
}

function hitOtherCar(player, otherCar) {
	// Logic when player car hits other car
	this.physics.pause();
	player.setTint(0xff0000);
}
