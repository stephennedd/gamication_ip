const canvas = document.getElementById('game');
const arcadeMachine = document.getElementById('machine');
const button = document.getElementById('animate');
const back = document.getElementById('back');
const background = document.getElementById('background');

button.addEventListener('click', moveCorridor);
back.addEventListener('click', walkBack);

function moveCorridor() {
	arcadeMachine.classList.add('move-out-machine');
	background.classList.add('move-out');

	setTimeout(() => {
		arcadeMachine.classList.remove('blur');
	}, 1200);

	setTimeout(() => {
		canvas.classList.remove('hide');
	}, 1850);

	// setTimeout(() => {
	// 	canvas.classList.add('crtOn');
	// }, 1850);
}

function walkBack() {
	arcadeMachine.classList.remove('move-out-machine');

	// canvas.classList.remove('crtOn');
	// canvas.classList.add('crtOff');
	// setTimeout(() => {
	// 	canvas.classList.add('hide');
	// }, 500);

	canvas.classList.add('hide');

	background.classList.remove('move-out');
	setTimeout(() => {
		arcadeMachine.classList.add('blur');
	}, 1300);
}
