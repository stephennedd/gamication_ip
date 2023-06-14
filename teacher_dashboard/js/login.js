document.getElementById("loginForm").addEventListener("submit", function(event) {
  event.preventDefault(); // Prevent the form from submitting normally

  var username = document.getElementById("username").value;
  var password = document.getElementById("password").value;

  // Prepare the request payload
  var data = {
    userId: username,
    password: password
  };

  // Send a POST request to the new login endpoint
  fetch("https://localhost:7186/api/Tokens", {
    method: "POST",
    headers: {
      "Content-Type": "application/json"
    },
    body: JSON.stringify(data)
  })
    .then(function(response) {
      if (!response.ok) {
        // Invalid credentials, display an error message
        throw new Error("Invalid username or password");
      }
      // Successful login, handle the response (e.g., store token/session, redirect)
      return response.json();
    })
    .then(function(data) {
      // Here we assume your server response includes a property "token" containing the JWT
      // Store the JWT in a cookie
      document.cookie = `jwt=${data.token}; path=/`;

      // Redirect to the admin panel or perform other necessary actions
      window.location.href = "admin-panel.html";
    })
    .catch(function(error) {
      // Handle any error that occurred during the login process
      console.error(error);
      // Display an error message to the user
      alert(error.message);
    });
});