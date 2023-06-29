document.addEventListener('DOMContentLoaded', () => {
	const container = document.getElementById('hexBackground');

	const hexagonSize = 25;
	const haloSize = hexagonSize * 3; // Adjust this for the halo size
	document.documentElement.style.setProperty('--v1', `${hexagonSize}px`);

	const hexagonsPerRow = Math.ceil(window.innerWidth / hexagonSize);
	const numberOfRows = Math.ceil(window.innerHeight / (hexagonSize * 0.866)); // Adjusted height approximation

	for (let i = 0; i < numberOfRows; i++) {
		const row = document.createElement('div');
		row.className = 'row';
		container.appendChild(row);

		for (let j = 0; j < hexagonsPerRow; j++) {
			const hexagon = document.createElement('div');
			hexagon.className = 'hexagon';
			row.appendChild(hexagon);
		}
	}

	// Initial cursor
	const initialCursor = document.createElement('div');
	initialCursor.className = 'cursor';
	initialCursor.style.width = `${haloSize}px`;
	initialCursor.style.height = `${haloSize}px`;
	container.appendChild(initialCursor);

	// Function to update position of the cursor according to ball's position
	const updatePosition = () => {
		// Get ball position from global variable
		const ballPosition = window.ballPosition;
		if (ballPosition) {
			const X = ballPosition.x;
			const Y = ballPosition.y;

			// Set initial cursor position
			initialCursor.style.left = X + 'px';
			initialCursor.style.top = Y + 'px';

			const trail = document.createElement('div');
			trail.className = 'trail';
			trail.style.width = `${haloSize}px`;
			trail.style.height = `${haloSize}px`;
			trail.style.left = X + 'px';
			trail.style.top = Y + 'px';

			// Sync animation with the current time
			const timeElapsed = Date.now() / 1000;
			const animationDelay = -timeElapsed % 2; // 2 seconds is the duration of our animation
			trail.style.animationDelay = `${animationDelay}s`;

			container.appendChild(trail);

			setTimeout(() => {
				container.removeChild(trail);
			}, 150); // adjust this for how long the trail lasts
		}

		// Call updatePosition again for the next animation frame
		requestAnimationFrame(updatePosition);
	};

	// Start the animation
	updatePosition();
});
