document.addEventListener('DOMContentLoaded', function () {
	const welcomeText =
		'WELCOME TO PROJECT G.A.M.I.F.I.C.A.T.I.O.N PLEASE ENTER YOUR CREDENTIALS...';
	const welcomeElement = document.querySelector('.welcome');
	const loginForm = document.getElementById('loginForm');
	const radioButtons = document.querySelectorAll('input[type="radio"]');
	const indicatorLine = document.getElementById('line');
	const labels = document.querySelectorAll('label');

	// Function for setting Indicator Position
	const setIndicatorPosition = (radioIndex) => {
		const label = labels[radioIndex];
		const labelRect = label.getBoundingClientRect();
		const containerRect = indicatorLine.parentElement.getBoundingClientRect();

		indicatorLine.style.left = `${labelRect.left - containerRect.left}px`;
		indicatorLine.style.width = `${labelRect.width}px`;
	};

	const displayTextOneCharacterAtATime = (element, text, interval = 35) => {
		// Clear previous text
		while (element.firstChild) {
			element.removeChild(element.firstChild);
		}

		element.innerHTML = text
			.split('')
			.map((char) => `<span style="display: none; opacity: 0;">${char}</span>`)
			.join('');

		Array.from(element.getElementsByTagName('span')).forEach((span, i) => {
			setTimeout(() => {
				span.style.display = 'inline';
				span.style.opacity = 1;
			}, interval * i);
		});
	};

	displayTextOneCharacterAtATime(welcomeElement, welcomeText);

	// Change which inputs are visible and set the indicator line position
	radioButtons.forEach((radio, index) => {
		radio.addEventListener('change', () => {
			// Hide error message when a radio button is changed
			errorElement.style.display = 'none';

			// Set the class of the form to match the value of the radio button
			loginForm.className = radio.value;

			// Set the position of the indicator line
			setIndicatorPosition(index);
		});
	});

	// Set initial position of the indicator line
	setIndicatorPosition(0);

	// Show login input fields and login button
	setTimeout(
		() => loginForm.classList.remove('hidden'),
		40 * welcomeText.length
	);

	// Error element for displaying messages
	const errorElement = document.createElement('div');
	errorElement.className = 'error-message';
	errorElement.style.display = 'none';

	// Insert the error message after the button in the form
	const loginButton = document.querySelector('button');
	if (loginButton) {
		loginButton.insertAdjacentElement('afterend', errorElement);
	}

	// Handle form submission
	loginForm.addEventListener('submit', function (event) {
		event.preventDefault(); // Prevent the form from submitting normally

		const email = document.getElementById('email').value;
		const password = document.getElementById('password').value;
		const repassword = document.getElementById('repassword').value;
		const loginButton = document.querySelector('button span');
		const passwordInput = document.getElementById('password');
		const repasswordInput = document.getElementById('repassword');

		const signInRadio = document.getElementById('signin');
		const signUpRadio = document.getElementById('signup');
		const resetRadio = document.getElementById('reset');

		// Reset styles and hide error message
		passwordInput.style.border = '';
		passwordInput.style.boxShadow = '';
		repasswordInput.style.border = '';
		repasswordInput.style.boxShadow = '';
		errorElement.style.display = 'none';

		if (signInRadio.checked) {
			// Handle Sign In
			if (email === '' || password === '') {
				displayTextOneCharacterAtATime(
					errorElement,
					'Please fill in all fields.'
				);
				errorElement.style.display = 'block';
				return;
			}
			var data = {
				userId: email,
				password: password,
			};
			fetch('https://localhost:7186/api/Tokens', {
				method: 'POST',
				headers: {
					'Content-Type': 'application/json',
				},
				body: JSON.stringify(data),
			})
				.then(function (response) {
					if (!response.ok) {
						throw new Error('Invalid username or password');
					}
					return response.json();
				})
				.then(function (data) {
					document.cookie =
						'jwt=; expires=Thu, 01 Jan 1970 00:00:00 UTC; path=/;';
					document.cookie = `jwt=${data.token}; path=/`;
					window.location.href = '../AracadeMachine/index.html';
				})
				.catch(function (error) {
					console.error(error);
					displayTextOneCharacterAtATime(errorElement, 'Login failed.');
					errorElement.style.display = 'block';
				});
		} else if (signUpRadio.checked) {
			// Handle Sign Up

			if (email === '' || password === '' || repassword === '') {
				displayTextOneCharacterAtATime(
					errorElement,
					'Please fill in all fields.'
				);
				errorElement.style.display = 'block';
				return;
			}

			if (password !== repassword) {
				passwordInput.style.border = '1px solid var(--error)';
				passwordInput.style.boxShadow = '0 0 5px var(--error)';
				repasswordInput.style.border = '1px solid var(--error)';
				repasswordInput.style.boxShadow = '0 0 5px var(--error)';
				displayTextOneCharacterAtATime(errorElement, 'Passwords do not match.');
				errorElement.style.display = 'block';
				return;
			}

			// Prepare the request payload
			var data = {
				userId: email,
				password: password,
			};

			fetch('https://localhost:7186/api/Users', {
 					method: 'POST',
				headers: {
						'Content-Type': 'application/json',
					},
					body: JSON.stringify(data),
				})
					.then(function (response) {
						console.log(response);
						if (!response.ok) {
							throw new Error('Failed to create user');
						}
						return;
					})					
					.catch(function (error) {
						console.error(error);
						loginButton.textContent = '[ Failed to create user! ]';
					});
			// TODO: Send a POST request to the Sign Up endpoint
		} else if (resetRadio.checked) {
			// Handle Reset

			if (email === '') {
				displayTextOneCharacterAtATime(errorElement, 'Please enter your ID.');
				errorElement.style.display = 'block';
				return;
			}

			// TODO: Send a POST request to the Password Reset endpoint
			
		}
	});
});

////////////////////////////////////////// OLD CODE IN CASE SOMETHING BREAKS //////////////////////////////////////////////

// document.addEventListener('DOMContentLoaded', function () {
// 	// Display the welcome text one character at a time
// 	var text =
// 		'WELCOME TO PROJECT G.A.M.I.F.I.C.A.T.I.O.N PLEASE ENTER YOUR CREDENTIALS...';
// 	var characters = text
// 		.split('')
// 		.map((char, index) => {
// 			if (index === text.length - 1) {
// 				return `<span class="blink" style="display: none; opacity: 0;">${char}</span>`;
// 			} else {
// 				return `<span style="display: none; opacity: 0;">${char}</span>`;
// 			}
// 		})
// 		.join('');

// 	var welcomeElement = document.querySelector('.welcome');
// 	welcomeElement.innerHTML = characters;

// 	var spans = welcomeElement.getElementsByTagName('span');

// 	for (let i = 0; i < spans.length; i++) {
// 		setTimeout(function () {
// 			spans[i].style.display = 'inline';
// 			spans[i].style.opacity = 1;
// 		}, 35 * i);
// 	}

// 	// Change which inputs are visible
// 	const loginForm = document.getElementById('loginForm');
// 	const radioButtons = document.querySelectorAll('input[type="radio"]');
// 	radioButtons.forEach((radio) => {
// 		radio.addEventListener('change', function () {
// 			loginForm.className = this.value;
// 		});
// 	});

// 	const indicatorLine = document.getElementById('line');
// 	const labels = document.querySelectorAll('label');

// 	function setIndicatorPosition(radioIndex) {
// 		const label = labels[radioIndex];
// 		const labelRect = label.getBoundingClientRect();
// 		const containerRect = indicatorLine.parentElement.getBoundingClientRect();

// 		indicatorLine.style.left = `${labelRect.left - containerRect.left}px`;
// 		indicatorLine.style.width = `${labelRect.width}px`;
// 	}

// 	// Set initial position of the indicator line
// 	setIndicatorPosition(0);

// 	radioButtons.forEach((radio, index) => {
// 		radio.addEventListener('change', function () {
// 			loginForm.className = this.value;
// 			setIndicatorPosition(index);
// 		});
// 	});

// 	// Show login input fields and login button
// 	setTimeout(function () {
// 		loginForm.classList.remove('hidden');
// 	}, 40 * spans.length);

// 	// Handle form submission
// 	document
// 		.getElementById('loginForm')
// 		.addEventListener('submit', function (event) {
// 			event.preventDefault(); // Prevent the form from submitting normally

// 			var email = document.getElementById('email').value;
// 			var password = document.getElementById('password').value;
// 			var repassword = document.getElementById('repassword').value; // For Sign Up
// 			var loginButton = document.querySelector('button span');

// 			var signInRadio = document.getElementById('signin');
// 			var signUpRadio = document.getElementById('signup');
// 			var resetRadio = document.getElementById('reset');

// 			if (signInRadio.checked) {
// 				// Handle Sign In
// 				if (email === '' || password === '') {
// 					// loginButton.textContent = '[ EMPTY INPUT! ]';
// 					return;
// 				}
// 				var data = {
// 					userId: email,
// 					password: password,
// 				};
// 				fetch('https://localhost:7186/api/Tokens', {
// 					method: 'POST',
// 					headers: {
// 						'Content-Type': 'application/json',
// 					},
// 					body: JSON.stringify(data),
// 				})
// 					.then(function (response) {
// 						if (!response.ok) {
// 							throw new Error('Invalid username or password');
// 						}
// 						return response.json();
// 					})
// 					.then(function (data) {
// 						document.cookie =
// 							'jwt=; expires=Thu, 01 Jan 1970 00:00:00 UTC; path=/;';
// 						document.cookie = `jwt=${data.token}; path=/`;
// 						window.location.href = 'index.html'; // TODO: change to whatever.html
// 					})
// 					.catch(function (error) {
// 						console.error(error);
// 						loginButton.textContent = '[ LOGIN FAILED! ]';
// 					});
// 			} else if (signUpRadio.checked) {
// 				// Handle Sign Up
// 				// TODO: Add Sign Up logic

// 				if (email === '' || password === '' || repassword === '') {
// 					// loginButton.textContent = '[ EMPTY INPUT! ]';
// 					return;
// 				}

// 				if (password !== repassword) {
// 					// loginButton.textContent = "[ PASSWORDS DON'T MATCH! ]";
// 					return;
// 				}

// 				// Prepare the request payload
// 				var data = {
// 					email: email,
// 					password: password,
// 				};

// 				// TODO: Send a POST request to the Sign Up endpoint
// 				// fetch("YOUR_SIGNUP_ENDPOINT", {...})
// 			} else if (resetRadio.checked) {
// 				// Handle Reset
// 				// TODO: Add Password Reset logic

// 				if (email === '') {
// 					// loginButton.textContent = '[ EMPTY EMAIL INPUT! ]';
// 					return;
// 				}

// 				// TODO: Send a POST request to the Password Reset endpoint
// 				// fetch("YOUR_PASSWORD_RESET_ENDPOINT", {...})
// 			}
// 		});
// });

////////////////////////////////////////////////////////////////////////////////////////////

// document
// 	.getElementById('loginForm')
// 	.addEventListener('submit', function (event) {
// 		event.preventDefault(); // Prevent the form from submitting normally

// 		var email = document.getElementById('email').value;
// 		var password = document.getElementById('password').value;
// 		var loginButton = document.getElementById('login');

// 		if (email === '' || password === '') {
// 			loginButton.textContent = '[ EMPTY INPUT! ]';
// 		} else {
// 			// Prepare the request payload
// 			var data = {
// 				userId: email,
// 				password: password,
// 			};

// 			// Send a POST request to the login endpoint
// 			fetch('https://localhost:7186/api/Tokens', {
// 				method: 'POST',
// 				headers: {
// 					'Content-Type': 'application/json',
// 				},
// 				body: JSON.stringify(data),
// 			})
// 				.then(function (response) {
// 					if (!response.ok) {
// 						// Invalid credentials, display an error message
// 						throw new Error('Invalid username or password');
// 					}
// 					// Successful login, handle the response (e.g., store token/session, redirect)
// 					return response.json();
// 				})
// 				.then(function (data) {
// 					// Store the JWT in a cookie
// 					document.cookie =
// 						'jwt=; expires=Thu, 01 Jan 1970 00:00:00 UTC; path=/;';

// 					document.cookie = `jwt=${data.token}; path=/`;

// 					// Redirect to the admin panel or perform other necessary actions
// 					window.location.href = 'index.html'; //TODO: change to whatever.html
// 				})
// 				.catch(function (error) {
// 					// Handle any error that occurred during the login process
// 					console.error(error);
// 					// Display an error message to the user
// 					loginButton.textContent = '[ LOGIN FAILED! ]';
// 				});
// 		}
// 	});
