var player;
var hit;
var floor;
var tile;
var tilesGroup;
var tileChild;
var breakTilesGroup;
var breakTileChild;
var DisTilesGroup;
var DisTileChild;
var springGroup;
var springChild;
var starGroup;
var starChild;
var enemyNgroup;
var enemyNchild;
var enemySgroup;
var enemySchild;
var particles;
var score = 0;
var scoreText;
var GameOverText;
var lifeText;
var retryText;
var tn;
var td;
var tb;
var zoneL;
var zoneR;
var rocket;
var spring;
var star;
var enemy_n;
var enemy_s;
let jumpheight = 840;
let life = 2;
let invincible = false;
let isProcessing = false;
let scoreMultiplier = 2;

export class GameScene extends Phaser.Scene {
	constructor() {
		super('GameScene');
	}

	preload() {
		// Load your assets here
		this.load.image('tile-n', './../public/images/tileblue.png');
		this.load.image('player', './../public/images/rocketLowBoost.png');
		this.load.image('playerBoosted', './../public/images/rocketHighBoost.png');
		this.load.image('tile-b', './../public/images/tilered.png');
		this.load.image('tile-d', './../public/images/tileyellow.png');
		this.load.image('spring', './../public/images/jump.png');
		this.load.image('star', './../public/images/star.png');
		this.load.image('rocket', './../public/images/bomb.png');
		this.load.image('enemy-n', './../public/images/bomb.png');
		this.load.image('enemy-s', './../public/images/bomb.png');
		this.load.image('bullet', './../public/images/bomb.png');

		/*
		
		
		this.load.svg("rocket", "assets/");
		this.load.svg("enemy-n", "assets/enemy-n-01.svg", {scale: 2.7});
		this.load.svg("enemy-s", "assets/enemy-s-01.svg", {scale: 2.7});
		this.load.image("bullet", "assets/laser.png")
        */
	}

	create() {
		this.createTiles();
		/* Create Breaking Tiles/Platforms Group */
		this.createBreakTiles();
		/* Create Disappearing Tiles/Platforms Group */
		this.createDisTiles();
		/* Create Player Model */
		this.createPlayer();
		/* Create Springs */
		this.createSpring();
		/* Create Stars */
		this.createStars();
		/* Create Normal Enemies */
		this.createEnemyN();
		/* Create Shooting Enemies */
		this.createEnemyS();

		// this.emitter.onParticleDeath( this.GameOver);

		//particles.enableBody = true; TODO: figure out how to make particles collide with player

		lifeText = this.add
			.text(200, 16, 'lives: ' + life, {
				fontFamily: '"Montserrat"',
				fontSize: '32px',
			})
			.setScrollFactor(0);
		lifeText.depth = 2;
		/* Score Text */
		scoreText = this.add
			.text(16, 16, 'Score: 0', {
				fontFamily: '"Montserrat"',
				fontSize: '32px',
			})
			.setScrollFactor(0);
		scoreText.depth = 2;
		/* Game Over Text */
		GameOverText = this.add
			.text(
				this.scene.scene.sys.game.canvas.width / 2,
				this.scene.scene.sys.game.canvas.height / 2,
				'GAME OVER',
				{ fontFamily: '"Montserrat"', fontSize: '90px' }
			)
			.setScrollFactor(0);
		GameOverText.setOrigin(0.5);
		GameOverText.depth = 2;
		GameOverText.visible = false;

		/* Retry Text */
		retryText = this.add
			.text(
				this.scene.scene.sys.game.canvas.width / 2,
				this.scene.scene.sys.game.canvas.height / 2 + 180,
				'RETRY',
				{ fontFamily: '"Montserrat"', fontSize: '32px' }
			)
			.setScrollFactor(0);
		retryText.setOrigin(0.5);
		retryText.depth = 2;
		retryText.visible = false;
		/* retryText.setInteractive(
			{useHandCursor: true});
		retryText.on('pointerdown', function(){
			var red = Phaser.Math.Between(50, 255);
			var green = Phaser.Math.Between(50, 255);
			var blue = Phaser.Math.Between(50, 255);
			this.cameras.main.fade(1000, red, green, blue);
		}, this);
		this.cameras.main.on('camerafadeoutcomplete', function () {
			this.scene.restart();

		}, this); */

		/* Collision checks and events */
		this.physics.add.collider(player, tilesGroup, this.bounceBack, null, this);
		this.physics.add.collider(
			player,
			DisTilesGroup,
			this.TileDisappear,
			null,
			this
		);
		this.physics.add.overlap(
			player,
			breakTilesGroup,
			this.TileBreak,
			null,
			this
		);
		this.physics.add.collider(player, springGroup, this.BigBounce, null, this);
		this.physics.add.overlap(player, starGroup, this.pickStars, null, this);
		this.physics.add.overlap(player, enemyNgroup, this.LoseLife, null, this);
		this.physics.add.overlap(player, enemySgroup, this.LoseLife, null, this);

		/* camera and tile tracking vars */
		this.cameraYMin = 99999;
		this.tileYMin = 99999;

		/* Control setup, Kbd  */
		this.key_left = this.input.keyboard.addKey(
			Phaser.Input.Keyboard.KeyCodes.LEFT
		);
		this.key_right = this.input.keyboard.addKey(
			Phaser.Input.Keyboard.KeyCodes.RIGHT
		);
		this.key_Up = this.input.keyboard.addKey(Phaser.Input.Keyboard.KeyCodes.UP);
		/* Mouse Clicks */
		this.input.mouse.disableContextMenu();
	}

	update() {
		/* track the maximum amount that the hero has travelled */
		player.yChange = Math.max(
			player.yChange,
			Math.abs(player.y - player.yOrig)
		);

		/* Dynamically change world bounds based on player pos */
		// scene.physics.world.setBounds(x, y, width, height, checkLeft, checkRight, checkUp, checkDown);
		this.physics.world.setBounds(
			0,
			-player.yChange,
			this.physics.world.bounds.width,
			this.game.config.height + player.yChange
		);

		/* Camera tracking */
		// this.cameras.main.startFollow(player, true);
		this.cameras.main.setLerp(0.5);
		this.cameras.main.centerOnY(player.y);

		/* Arrow buttons */
		if (this.key_right.isDown) player.body.velocity.x = 330;
		else if (this.key_left.isDown) player.body.velocity.x = -330;
		else player.body.velocity.x = 0;

		/* Device Orientation */
		window.addEventListener('deviceorientation', this.handleOrientation, true);

		/* Up arrow to give Y velocity for debug beyond camera screen */
		// if (this.key_Up.isDown) player.body.velocity.y = -400;

		/* Wrap the player from left <==> right of the screen. */
		this.physics.world.wrap(player, player.width / 6);

		/* if the hero falls below the camera view, gameover */
		if (
			player.y > this.cameraYMin + this.scene.scene.sys.game.canvas.height &&
			isProcessing == false
		) {
			this.LoseLife();
		}

		/* For each tilesGroup child, find out which is the highest
		if one goes below the camera view, then create a new one at a distance from the highest one
		these are pooled so they are very performant */
		tilesGroup.children.iterate(function (item) {
			var chance = Phaser.Math.Between(1, 100);
			var chance2 = Phaser.Math.Between(1, 100);
			var chance3 = Phaser.Math.Between(1, 100);
			var xAxis;
			var yAxis = this.tileYMin - 160;
			this.tileYMin = Math.min(this.tileYMin, item.y);
			this.cameraYMin = Math.min(
				this.cameraYMin,
				player.y - this.game.config.height + 430
			);

			if (item.y > this.cameraYMin + this.game.config.height) {
				item.destroy();
				/* 15% chance for Disappearing Tile */
				if (chance > 75 && chance < 81) {
					xAxis = Phaser.Math.Between(
						100,
						this.physics.world.bounds.width - 100
					);
					tn = this.spawnTile(xAxis, yAxis, 'tile-n');
					td = this.spawnTileDis(
						Phaser.Math.Between(100, xAxis - 100) ||
							Phaser.Math.Between(
								xAxis + 100,
								this.physics.world.bounds.width - 100
							),
						Phaser.Math.Between(yAxis + 100, yAxis - 100),
						'tile-d'
					);
				} else if (chance > 80) {
					/* 15% chance for Breaking Tile */
					xAxis = Phaser.Math.Between(
						100,
						this.physics.world.bounds.width - 100
					);
					tn = this.spawnTile(xAxis, yAxis, 'tile-n');
					tb = this.spawnTileBreak(
						Phaser.Math.Between(
							xAxis + 100,
							this.physics.world.bounds.width - 100
						) || Phaser.Math.Between(100, xAxis - 100),
						Phaser.Math.Between(yAxis + 100, yAxis - 100),
						'tile-b'
					);
				} else if (chance < 71)
					/* Else Regular Tiles */
					xAxis = Phaser.Math.Between(
						100,
						this.physics.world.bounds.width - 100
					);
				tn = this.spawnTile(xAxis, yAxis, 'tile-n');

				/* 20% chance2 of spawning spring */
				if (chance2 > 60 && chance2 < 81) {
					this.spawnSpring(
						Phaser.Math.Between(xAxis - 50, xAxis + 50),
						yAxis - 5,
						'spring'
					);
				} else if (chance2 > 80) {
					/* 20% chance2 of spawning stars */
					this.spawnStar(
						Phaser.Math.Between(100, this.physics.world.bounds.width - 100),
						Phaser.Math.Between(yAxis, yAxis - 100),
						'star'
					);
				} else if (chance2 < 20) {
					xAxis = Phaser.Math.Between(
						100,
						this.physics.world.bounds.width - 100
					);
					tn = this.spawnTile(xAxis, yAxis, 'tile-d');
				}
				/* NO STARS
				else if (chance2 < 61){
					
				}
				*/

				/* scaling chance of spawning enemies */
				if (chance3 < 31) {
					this.spawnEnemyN(
						0,
						Phaser.Math.Between(yAxis, yAxis - 100),
						'enemy-n'
					);
				} else if (chance3 > 90) {
					this.spawnEnemyS(
						Phaser.Math.Between(100, this.physics.world.bounds.width - 100),
						Phaser.Math.Between(yAxis, yAxis - 100),
						'enemy-s'
					);
				}
			}
		}, this);
	}

	setInvincible() {
		player.invincible = true;
		player.alpha = 0.5;
		this.time.addEvent({
			delay: 1000,
			callback: this.resetInvincible,
			callbackScope: this,
		});
	}
	resetInvincible() {
		player.invincible = false;
		player.alpha = 1;
	}
	/* Create Player Model */
	createPlayer() {
		player = this.physics.add.image(
			this.scene.scene.sys.canvas.width / 2,
			(3 * this.scene.scene.sys.canvas.height) / 4,
			'player'
		);
		// player.body.setCollideWorldBounds();
		player.setVelocity(0, -1000);
		player.setGravityY(800);
		player.setBounce(0.4);
		player.setScale(0.4);
		player.body.checkCollision.up = false;
		player.depth = 1;

		player.yOrig = player.y;
		player.yChange = 0;
	}

	/* Create Regular Tiles/Platform */
	createTiles() {
		tilesGroup = this.physics.add.staticGroup({ runChildUpdate: true });
		tilesGroup.enableBody = true;
		tileChild = tilesGroup.getChildren();

		// spawnTile();
		for (var i = 0; i < 5; i++) {
			tn = this.spawnTile(
				Phaser.Math.Between(25, this.physics.world.bounds.width - 25),
				this.physics.world.bounds.height - 200 - 200 * i,
				'tile-n'
			);
		}
	}

	/* Create Breaking Tiles */
	createBreakTiles() {
		breakTilesGroup = this.physics.add.staticGroup({ runChildUpdate: true });
		breakTilesGroup.enableBody = true;
		breakTileChild = breakTilesGroup.getChildren();
	}

	/* Create Disappearing Tiles */
	createDisTiles() {
		DisTilesGroup = this.physics.add.staticGroup({ runChildUpdate: true });
		DisTilesGroup.enableBody = true;
		DisTileChild = DisTilesGroup.getChildren();
	}

	/* Create Springs */
	createSpring() {
		springGroup = this.physics.add.staticGroup({ runChildUpdate: true });
		springGroup.enableBody = true;
		springChild = springGroup.getChildren();
	}

	/* Create Stars */
	createStars() {
		starGroup = this.physics.add.staticGroup({ runChildUpdate: true });
		starGroup.enableBody = true;
		starChild = starGroup.getChildren();
	}

	/* Create Normal Enemies group */
	createEnemyN() {
		enemyNgroup = this.physics.add.group({ runChildUpdate: true });
		enemyNgroup.enableBody = true;
		enemyNchild = enemyNgroup.getChildren();
	}

	/* Create Shooting Enemies group */
	createEnemyS() {
		enemySgroup = this.physics.add.group({ runChildUpdate: true });
		enemySgroup.children.enableBody = true;
		enemySchild = enemySgroup.getChildren();
	}

	/* Sub function for Regular tiles.*/
	spawnTile(x, y, type) {
		tile = tilesGroup.create(x, y, type);
		tile.setImmovable();

		return tile;
	}

	/* Sub function for Breaking tiles.*/
	spawnTileBreak(x, y, type) {
		tile = breakTilesGroup.create(x, y, type);
		tile.setImmovable();
		return tile;
	}

	/* Sub function for Disappearing tiles.*/
	spawnTileDis(x, y, type) {
		tile = DisTilesGroup.create(x, y, type);
		tile.setImmovable();
		return tile;
	}

	/* Sub function for Springs.*/
	spawnSpring(x, y, type) {
		spring = springGroup.create(x, y, type);
		spring.setImmovable();
		return spring;
	}

	/* Sub function for Stars.*/
	spawnStar(x, y, type) {
		star = starGroup.create(x, y, type);
		star.setImmovable();
		return star;
	}

	/* Sub function for enemy N.*/
	spawnEnemyN(x, y, type) {
		enemy_n = enemyNgroup.create(x, y, type);
		enemy_n.body.velocity.x = 150;
		enemy_n.setImmovable();
		return enemy_n;
	}

	/* Sub function for enemy S.*/
	spawnEnemyS(x, y, type) {
		enemy_s = enemySgroup.create(x, y, type);
		enemy_s.body.velocity.y = 150;
		enemy_s.setImmovable();
	}

	/* Bounce off Regular Tiles / Regular Tile interaction */
	bounceBack(_player, _tilesGroup) {
		if (_player.body.touching.down && _tilesGroup.body.touching.up) {
			score += 1;
			scoreText.setText('Score: ' + score);
			player.body.velocity.y = -jumpheight;
		}
	}

	/* Disappearing Tiles func / Dis Tile Interaction*/
	TileDisappear(_player) {
		DisTilesGroup.children.each(function (e) {
			if (_player.body.touching.down && e.body.touching.up) {
				DisTilesGroup.remove(e, true);
				score = score + 1;
				player.body.velocity.y = -jumpheight;
				scoreText.setText('Score: ' + score);
			}
		}, this);
	}

	/* Breaking Tiles func / Breaking Tile Interaction */
	TileBreak(_player) {
		breakTilesGroup.children.each(function (e) {
			if (_player.body.touching.down && e.body.touching.up) {
				breakTilesGroup.remove(e, true);
			}
		}, this);
	}

	/* Spring Interaction func */
	BigBounce(_player, _springGroup) {
		if (_player.body.touching.down && _springGroup.body.touching.up) {
			score += 15;
			scoreText.setText('Score: ' + score);
			player.body.velocity.y = -1100;
		}
	}

	/* Stars Interaction func */
	pickStars() {
		starGroup.children.each(function (e) {
			score += 25;
			scoreText.setText('Score:' + score);
			e.destroy();
		}, this);
	}

	GameOver() {
		this.score = this.score * scoreMultiplier;

		// Show Game Over Text
		GameOverText.visible = true;

		// retryText.visible = true;
		scoreText.setPosition(
			this.game.config.width / 2,
			this.game.config.height / 2 + 100
		);
		scoreText.setFontSize(45);
		scoreText.setOrigin(0.5);

		/* Hide and clear assets */
		tilesGroup.setAlpha(0);
		tilesGroup.clear();
		breakTilesGroup.setAlpha(0);
		breakTilesGroup.clear();
		DisTilesGroup.setAlpha(0);
		DisTilesGroup.clear();
		springGroup.setAlpha(0);
		springGroup.clear();
		starGroup.setAlpha(0);
		starGroup.clear();
		enemyNgroup.setAlpha(0);
		enemyNgroup.clear();
		enemySgroup.setAlpha(0);
		enemySgroup.clear();

		/* Player Opacity */
		player.setAlpha(0.45);
		//TODO SEND SCORE TO BACKEND
	}
	FallOff() {
		life -= 1;
		lifeText.setText('Life: ' + life);
		player.body.velocity.y = -1000;
		this.setInvincible();
	}
	LoseLife() {
		isProcessing = true;
		setTimeout(() => {
			isProcessing = false;
		}, 1000);
		if (invincible == true) {
			player.velocity.y = -1000;
			return;
		} else if (life > 1) {
			this.FallOff();
		} else {
			this.GameOver();
		}
	}

	handleOrientation(e) {
		var dx = e.gamma;
		var edx = (dx / 3.5) ** 4;
		console.log(dx, edx);
		if (dx < 0) {
			player.body.velocity.x = -edx;
		} else {
			player.body.velocity.x = edx;
		}
		// player.body.velocity.x = 0;
		if (player.body.velocity.x > 400) {
			player.body.velocity.x = 400;
		} else if (player.body.velocity.x < -400) player.body.velocity.x = -400;
	}
}
