document.addEventListener('DOMContentLoaded', function () {
	var text =
		'WELCOME TO PROJECT G.A.M.I.F.I.C.A.T.I.O.N PLEASE ENTER YOUR CREDENTIALS...';
	var characters = text
		.split('')
		.map((char, index) => {
			if (index === text.length - 1) {
				return `<span class="blink" style="display: none; opacity: 0;">${char}</span>`;
			} else {
				return `<span style="display: none; opacity: 0;">${char}</span>`;
			}
		})
		.join('');

	var welcomeElement = document.querySelector('.welcome');
	welcomeElement.innerHTML = characters;

	var spans = welcomeElement.getElementsByTagName('span');

	for (let i = 0; i < spans.length; i++) {
		setTimeout(function () {
			spans[i].style.display = 'inline';
			spans[i].style.opacity = 1;
		}, 35 * i);
	}

	// Show login input fields and login button
	setTimeout(function () {
		var hiddenElements = document.querySelectorAll('.hidden');
		hiddenElements.forEach(function (element) {
			element.classList.remove('hidden');
		});
	}, 50 * spans.length);
});

document
	.getElementById('loginForm')
	.addEventListener('submit', function (event) {
		var email = document.getElementById('email').value;
		var password = document.getElementById('password').value;
		var loginButton = document.getElementById('login');

		// Check if either field is empty
		if (email === '' || password === '') {
			// Prevent form from submitting
			event.preventDefault();

			// Show error message on the button and change its background to red
			loginButton.textContent = '[ EMPTY INPUT! ]';
			// loginButton.classList.add('error');
		} else {
			// You can add additional logic here for successful submission
		}
	});
