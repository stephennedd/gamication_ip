document
	.getElementById('loginForm')
	.addEventListener('submit', function (event) {
		event.preventDefault(); // Prevent the form from submitting normally

		var username = document.getElementById('username').value;
		var password = document.getElementById('password').value;

		// Prepare the request payload
		var data = {
			userId: username,
			password: password,
		};

		// Send a POST request to the new login endpoint
		fetch('http://localhost:4434/api/Tokens', {
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
				// Here we assume your server response includes a property "token" containing the JWT
				// Store the JWT in a cookie
				let date = new Date();
				date.setMinutes(date.getMinutes() + 45);
				document.cookie =
					'jwt=; expires=Thu, 01 Jan 1970 00:00:00 UTC; path=/;';
				document.cookie = `jwt=${
					data.token
				}; path=/ ; expires=${date.toUTCString()};`;

				var decodedToken = parseJwt(data.token);
				const userRole =
					decodedToken[
						'http://schemas.microsoft.com/ws/2008/06/identity/claims/role'
					];
				localStorage.setItem('role', userRole);

				// Redirect to the admin panel or perform other necessary actions
				window.location.href = 'admin-panel.html';
			})
			.catch(function (error) {
				// Handle any error that occurred during the login process
				console.error(error);
				// Display an error message to the user
				alert(error.message);
			});
	});

function parseJwt(token) {
	var base64Url = token.split('.')[1];
	var base64 = base64Url.replace(/-/g, '+').replace(/_/g, '/');
	var jsonPayload = decodeURIComponent(
		atob(base64)
			.split('')
			.map(function (c) {
				return '%' + ('00' + c.charCodeAt(0).toString(16)).slice(-2);
			})
			.join('')
	);

	return JSON.parse(jsonPayload);
}
