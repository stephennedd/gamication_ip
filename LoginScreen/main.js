let token;

document.addEventListener('DOMContentLoaded', function () {
	const mainText =
		'WELCOME TO PROJECT G.A.M.I.F.I.C.A.T.I.O.N PLEASE ENTER YOUR CREDENTIALS...';
	const verificationText =
		'PLEASE ENTER THE VERIFICATION CODE SENT TO YOUR EMAIL...';
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

	displayTextOneCharacterAtATime(welcomeElement, mainText);

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
	setTimeout(() => loginForm.classList.remove('hide'), 40 * mainText.length);

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

		const studentID = document.getElementById('studentID').value;
		const password = document.getElementById('password').value;
		const repassword = document.getElementById('repassword').value;
		const verificationCode = document.getElementById('verificationCode').value;
		const firstName = document.getElementById('firstName').value;
		const lastName = document.getElementById('lastName').value;

		const studentIDInput = document.getElementById('studentID');
		const passwordInput = document.getElementById('password');
		const repasswordInput = document.getElementById('repassword');
		const verificationCodeInput = document.getElementById('verificationCode');
		const firstNameInput = document.getElementById('firstName');
		const lastNameInput = document.getElementById('lastName');
		const groupSelector = document.getElementById('groupSelect');
		let groupName = null;

		const signInRadio = document.getElementById('signin');
		const signUpRadio = document.getElementById('signup');
		const resetRadio = document.getElementById('reset');
		const verificationRadio = document.getElementById('verify');

		// get label for verification radio button
		const verifyLabel = labels[1];

		// Reset styles and hide error message
		passwordInput.style.border = '';
		passwordInput.style.boxShadow = '';
		repasswordInput.style.border = '';
		repasswordInput.style.boxShadow = '';
		errorElement.style.display = 'none';

		if (signInRadio.checked) {
			if (studentID === '' || password === '') {
				displayTextOneCharacterAtATime(
					errorElement,
					'Please fill in all fields.'
				);
				errorElement.style.display = 'block';
				return;
			}
			const data = {
				userId: studentID,
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
						throw (
							(new Error('Invalid username or password'),
							displayTextOneCharacterAtATime(
								errorElement,
								'Invalid username or password'
							),
							(errorElement.style.display = 'block'))
						);
					}
					return response.json();
				})
				.then(function (data) {
					let date = new Date();
					date.setMinutes(date.getMinutes() + 45);
					document.cookie =
						'jwt=; expires=Thu, 01 Jan 1970 00:00:00 UTC; path=/;';
					document.cookie = `jwt=${
						data.token
					}; path=/ ; expires=${date.toUTCString()};`;
					token = data.token;
					const decodedToken = parseJwt(token);

					if (decodedToken.IsVerified == 'False') {
						displayTextOneCharacterAtATime(welcomeElement, verificationText);
						studentIDInput.classList.add('hidden');
						verificationRadio.classList.remove('hide');
						verifyLabel.classList.remove('hide');
						verificationCodeInput.classList.remove('hidden');
						groupSelector.classList.remove('hidden');

						loginForm.classList.replace('signin', 'verify');
						verificationRadio.checked = true;
						setIndicatorPosition(1);
					} else window.location.href = '../ArcadeMachine';
				})
				.catch(function (error) {
					console.error(error);
					displayTextOneCharacterAtATime(errorElement, 'Login failed.');
					errorElement.style.display = 'block';
				});
		} else if (signUpRadio.checked) {
			if (studentID === '' || password === '' || repassword === '') {
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
			const data = {
				userId: studentID,
				password: password,
				name: firstName,
				surname: lastName,
			};

			displayTextOneCharacterAtATime(welcomeElement, 'Creating user...');

			fetch('https://localhost:7186/api/Users', {
				method: 'POST',
				headers: {
					'Content-Type': 'application/json',
				},
				body: JSON.stringify(data),
			})
				.then(function (response) {
					if (!response.ok) {
						throw new Error('Failed to create user');
					}
					console.log(response);
					return response.json();
				})
				.then(function (data) {
					displayTextOneCharacterAtATime(welcomeElement, 'Account created!');
					setTimeout(() => {
						displayTextOneCharacterAtATime(
							welcomeElement,
							'Login to continue.'
						);
						// change selected view to sign in
						loginForm.classList.replace('signup', 'signin');
						signInRadio.checked = true;
						setIndicatorPosition(0);
					}, 1500);
				})
				.catch(function (error) {
					console.error(error);
					displayTextOneCharacterAtATime(
						errorElement,
						'Failed to create user.'
					);
					errorElement.style.display = 'block';
				});
		} else if (resetRadio.checked) {
			if (studentID === '') {
				displayTextOneCharacterAtATime(errorElement, 'Please enter your ID.');
				errorElement.style.display = 'block';
				return;
			}

			// TODO: Send a POST request to the Password Reset endpoint
		} else if (verificationRadio.checked) {
			if (verificationCode === '') {
				displayTextOneCharacterAtATime(
					errorElement,
					'Please enter your verification code.'
				);
				errorElement.style.display = 'block';
				return;
			}
			displayTextOneCharacterAtATime(
				welcomeElement,
				'Enter verification code sent to your email and chose your group.'
			);

			verifyCode(verificationCode);
			refreshJWT();
			//location.reload();
		}
	});

	function parseJwt(token) {
		const base64Url = token.split('.')[1];
		const base64 = base64Url.replace(/-/g, '+').replace(/_/g, '/');
		const jsonPayload = decodeURIComponent(
			atob(base64)
				.split('')
				.map(function (c) {
					return '%' + ('00' + c.charCodeAt(0).toString(16)).slice(-2);
				})
				.join('')
		);

		return JSON.parse(jsonPayload);
	}

	function refreshJWT() {
		let token = document.cookie
			.split('; ')
			.find((row) => row.startsWith('jwt='))
			.split('=')[1];
		fetch('https://localhost:7186/api/Tokens', {
			method: 'GET',
			headers: {
				Authorization: `Bearer ${token}`,
				'Content-Type': 'application/json',
			},
		})
			.then(function (response) {
				if (!response.ok) {
					displayTextOneCharacterAtATime(
						errorElement,
						'Invalid username or password'
					);
					errorElement.style.display = 'block';
					throw new Error('Invalid username or password');
				}
				return response.json();
			})
			.then(function (data) {
				let date = new Date();
				date.setMinutes(date.getMinutes() + 45);
				document.cookie =
					'jwt=; expires=Thu, 01 Jan 1970 00:00:00 UTC; path=/;';
				document.cookie = `jwt=${
					data.token
				}; path=/ ; expires=${date.toUTCString()};`;
				token = data.token;
				document.cookie = `jwt=${token}; path=/`;
				const decodedToken = parseJwt(token);
				if (decodedToken.IsVerified == 'False') {
					displayTextOneCharacterAtATime(welcomeElement, verificationText);
					studentIDInput.classList.add('hidden');
					verificationRadio.classList.remove('hide');
					verifyLabel.classList.remove('hide');
					verificationCodeInput.classList.remove('hidden');

					loginForm.classList.replace('signin', 'verify');
					setIndicatorPosition(1);
				} else {
					window.location.href = '../ArcadeMachine';
				}
			})
			.catch(function (error) {
				console.error(error);
				displayTextOneCharacterAtATime(errorElement, 'Verification failed.');
				errorElement.style.display = 'block';
			});
	}

	function verifyCode(code) {
		let token = document.cookie
			.split('; ')
			.find((row) => row.startsWith('jwt='))
			.split('=')[1];

		fetch('https://localhost:7186/api/Users/' + code, {
			method: 'POST',
			headers: {
				'Content-Type': 'application/json',
				Authorization: `Bearer ${token}`,
			},
			body: JSON.stringify({ code: code }),
		})
			.then((response) => {
				if (response.ok) {
					return response.json();
				} else {
					throw new Error("Server response wasn't OK");
				}
			})
			.then((data) => {
				$('#response-message').empty();
				if (data.success) {
					displayTextOneCharacterAtATime(
						welcomeElement,
						'Verification sucessfull!'
					);
					setTimeout(() => {
						displayTextOneCharacterAtATime(
							welcomeElement,
							'Login to continue.'
						);
						loginForm.classList.replace('verify', 'signin');
						setIndicatorPosition(0);
					}, 1500);
				} else {
					displayTextOneCharacterAtATime(
						errorElement,
						'Verification code incorrect.'
					);
					errorElement.style.display = 'block';
				}
			})
			.catch((error) => {
				console.error('Error:', error);
			});
	}

	// For group selection
	async function fetchGroupNames() {
		try {
			const response = await fetch('https://localhost:7186/api/Groups', {
				method: 'GET',
				headers: {
					Authorization: `Bearer ${token}`,
					'Content-Type': 'application/json',
				},
			});

			if (!response.ok) {
				throw new Error(`HTTP error! status: ${response.status}`);
			}

			const data = await response.json();
			let groups = data.$values;

			populateGroupsDropdown(groups);
		} catch (error) {
			console.log('Fetch Error: ', error);
		}
	}

	function populateGroupsDropdown(groups) {
		const dropdown = document.getElementById('groupSelect');
		dropdown.innerHTML =
			'<option selected disabled value="">Select a group</option>'; // clear existing options

		// Add the groups to the dropdown
		groups.forEach((group) => {
			var option = document.createElement('option');
			option.text = group.name;
			option.value = group.name;
			console.log(group.name);
			dropdown.appendChild(option);
		});
		dropdown.addEventListener('change', function () {
			groupName = this.value; // update the label
			fetchLeaderboardData();
		});
	}

	fetchGroupNames();
});
