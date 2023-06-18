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
		var hiddenLoginForm = document.getElementById('loginForm');
		hiddenLoginForm.classList.remove('hidden');
	}, 50 * spans.length);
});

document
	.getElementById('loginForm')
	.addEventListener('submit', function (event) {
		event.preventDefault(); // Prevent the form from submitting normally

		var email = document.getElementById('email').value;
		var password = document.getElementById('password').value;
		var loginButton = document.getElementById('login');

		if (email === '' || password === '') {
			loginButton.textContent = '[ EMPTY INPUT! ]';
		} else {
			// Prepare the request payload
			var data = {
				userId: email,
				password: password,
			};

			// Send a POST request to the login endpoint
			fetch('https://localhost:7186/api/Tokens', {
				method: 'POST',
				headers: {
					'Content-Type': 'application/json',
				},
				body: JSON.stringify(data),
			})
				.then(function (response) {
					if (!response.ok) {
						// Invalid credentials, display an error message
						throw new Error('Invalid username or password');
					}
					// Successful login, handle the response (e.g., store token/session, redirect)
					return response.json();
				})
				.then(function (data) {
					// Store the JWT in a cookie
					document.cookie =
						'jwt=; expires=Thu, 01 Jan 1970 00:00:00 UTC; path=/;';

					document.cookie = `jwt=${data.token}; path=/`;

					// Redirect to the admin panel or perform other necessary actions
					window.location.href = 'index.html'; //TODO: change to whatever.html
				})
				.catch(function (error) {
					// Handle any error that occurred during the login process
					console.error(error);
					// Display an error message to the user
					loginButton.textContent = '[ LOGIN FAILED! ]';
				});
		}
	});

document
	.getElementById('registerButton')
	.addEventListener('click', function () {
		var registerFields = document.getElementById('registerFields');
		if (registerFields.classList.contains('hidden')) {
			registerFields.classList.remove('hidden');
		} else {
			registerFields.classList.add('hidden');
		}
	});

document.getElementById('submitButton').addEventListener('click', function () {
	var password = document.getElementById('registerPassword').value;
	var repeatPassword = document.getElementById('repeatPassword').value;
	var registerButton = document.getElementById('registerButton');
	var registerPasswordInput = document.getElementById('registerPassword');
	var repeatPasswordInput = document.getElementById('repeatPassword');

	if (password !== repeatPassword) {
		registerButton.textContent = '[ PASSWORDS DO NOT MATCH! ]';
		registerPasswordInput.style.border = '1px solid var(--error)';
		repeatPasswordInput.style.border = '1px solid var(--error)';
	} else {
		registerPasswordInput.style.border = '1px solid var(--text)';
		repeatPasswordInput.style.border = '1px solid var(--text)';
		// Handle registration here (e.g. send data to the server)
	}
});
