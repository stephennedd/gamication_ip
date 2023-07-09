document.addEventListener('DOMContentLoaded', (event) => {
	const rectangles = document.querySelectorAll('.subject');

	rectangles.forEach((rectangle) => {
		rectangle.addEventListener('click', (event) => {
			const url = event.target.dataset.url;
			window.location.href = url;
		});

		const image = rectangle.dataset.image;
		rectangle.style.backgroundImage = `url(${image})`;
	});
});
