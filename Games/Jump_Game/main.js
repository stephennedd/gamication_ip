

import {GameScene} from './scenes/game.js'

const config = {
	type: Phaser.AUTO,
	width: 800,
	height: 600,
	physics: {
		default: 'arcade',
		arcade: {
			
		}
	},
	scene: [GameScene]
}

export default new Phaser.Game(config)
