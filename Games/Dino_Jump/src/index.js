import Phaser from 'phaser';

import PlayScene from './playScene';
import PreloadScene from './preloadScene';

const config = {
	type: Phaser.AUTO,
	width: 1000,
	height: 340,
	pixelArt: true,
	transparent: true,
	physics: {
		default: 'arcade',
		arcade: {
			debug: false,
		},
	},
	scene: [PreloadScene, PlayScene],
};

new Phaser.Game(config);
