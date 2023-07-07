let move_speed = 3,
	grativy = 0.5;
let bird = document.querySelector('.bird');
let img = document.getElementById('bird-1');
let sound_point = new Audio('sounds effect/point.mp3');
let sound_die = new Audio('sounds effect/die.mp3');

// getting bird element properties
let bird_props = bird.getBoundingClientRect();

// This method returns DOMReact -> top, right, bottom, left, x, y, width and height
let background = document.querySelector('.background').getBoundingClientRect();

let message = document.querySelector('.message');
let score_val = document.querySelector('.score_val');
let score_title = document.querySelector('.score_title');
let lives_val = document.querySelector('.lives_val');
let lives_title = document.querySelector('.lives_title');
let lives;
let scoreMultiplier;
let invincibility = false;

// Check for localstorage items for extraLife and scoreMultiplier
const gameDataLife = localStorage.getItem('extraLife');
const gameDataScore = localStorage.getItem('scoreMultiplier');

if (gameDataLife === 'true') {
	lives = 3; // Add 2 extra lifes
} else {
	lives = 1;
}

// Update score multiplier based on localstorage
if (gameDataScore === 'true') {
	scoreMultiplier = 2; // Multiply score by 2
} else {
	scoreMultiplier = 1;
}

let game_state = 'Start';
img.style.display = 'none';
message.classList.add('messageStyle');

document.addEventListener('keydown', (e) => {
	if (e.key == 'Enter' && game_state != 'Play') {
		document.querySelectorAll('.pipe_sprite').forEach((e) => {
			e.remove();
		});
		img.style.display = 'block';
		bird.style.top = '40vh';
		game_state = 'Play';
		message.innerHTML = '';
		score_title.innerHTML = 'Score : ';
		score_val.innerHTML = '0';
		lives_title.innerHTML = 'Lives : ';
		lives_val.innerHTML = lives;
		message.classList.remove('messageStyle');
		play();
	}
});

function play() {
	function move() {
		if (game_state != 'Play') return;

		let pipe_sprite = document.querySelectorAll('.pipe_sprite');
		pipe_sprite.forEach((element) => {
			let pipe_sprite_props = element.getBoundingClientRect();
			bird_props = bird.getBoundingClientRect();

			if (pipe_sprite_props.right <= 0) {
				element.remove();
			} else {
				// Check for collision with pipes
				if (
					!invincibility && // Add this condition
					bird_props.left < pipe_sprite_props.left + pipe_sprite_props.width &&
					bird_props.left + bird_props.width > pipe_sprite_props.left &&
					bird_props.top < pipe_sprite_props.top + pipe_sprite_props.height &&
					bird_props.top + bird_props.height > pipe_sprite_props.top
				) {
					lives--; // Decrease life by 1 when hit
					lives_val.innerHTML = lives;

					// Make bird invincible for a brief period
					invincibility = true;
					setTimeout(() => {
						invincibility = false;
					}, 1000); // 1 second invincibility period

					if (lives <= 0) {
						// Check if lives are 0 to end the game
						game_state = 'End';
						message.innerHTML =
							'Game Over'.fontcolor('red') + '<br>Press Enter To Restart';
						message.classList.add('messageStyle');
						img.style.display = 'none';
						sound_die.play();
						sendScore(score_val.innerHTML);
						return;
					}
				} else {
					if (
						pipe_sprite_props.right < bird_props.left &&
						pipe_sprite_props.right + move_speed >= bird_props.left &&
						element.increase_score == '1'
					) {
						score_val.innerHTML = (+score_val.innerHTML + 1) * scoreMultiplier; // Multiply the score
						sound_point.play();
					}
					element.style.left = pipe_sprite_props.left - move_speed + 'px';
				}
			}
		});
		requestAnimationFrame(move);
	}
	requestAnimationFrame(move);

	let bird_dy = 0;
	function apply_gravity() {
		if (game_state != 'Play') return;
		bird_dy = bird_dy + grativy;
		document.addEventListener('keydown', (e) => {
			if (e.key == 'ArrowUp' || e.key == ' ') {
				img.src = 'images/Bird-2.png';
				bird_dy = -7.6;
			}
		});

		document.addEventListener('keyup', (e) => {
			if (e.key == 'ArrowUp' || e.key == ' ') {
				img.src = 'images/Bird.png';
			}
		});

		if (bird_props.top <= 0 || bird_props.bottom >= background.bottom) {
			game_state = 'End';
			message.style.left = '28vw';
			window.location.reload();
			message.classList.remove('messageStyle');
			return;
		}
		bird.style.top = bird_props.top + bird_dy + 'px';
		bird_props = bird.getBoundingClientRect();
		requestAnimationFrame(apply_gravity);
	}
	requestAnimationFrame(apply_gravity);

	let pipe_seperation = 0;

	let pipe_gap = 35;

	function create_pipe() {
		if (game_state != 'Play') return;

		if (pipe_seperation > 115) {
			pipe_seperation = 0;

			let pipe_posi = Math.floor(Math.random() * 30) + 8;
			let pipe_sprite_inv = document.createElement('div');
			pipe_sprite_inv.className = 'pipe_sprite';
			pipe_sprite_inv.style.top = pipe_posi - 70 + 'vh';
			pipe_sprite_inv.style.left = '100vw';

			document.body.appendChild(pipe_sprite_inv);
			let pipe_sprite = document.createElement('div');
			pipe_sprite.className = 'pipe_sprite';
			pipe_sprite.style.top = pipe_posi + pipe_gap + 'vh';
			pipe_sprite.style.left = '100vw';
			pipe_sprite.increase_score = '1';

			document.body.appendChild(pipe_sprite);
		}
		pipe_seperation++;
		requestAnimationFrame(create_pipe);
	}
	requestAnimationFrame(create_pipe);
}

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
