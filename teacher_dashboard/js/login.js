document.getElementById("loginForm").addEventListener("submit", function(event) {
  event.preventDefault(); // Prevent the form from submitting normally

  var username = document.getElementById("username").value;
  var password = document.getElementById("password").value;

  // Prepare the request payload
  var data = {
    username: username,
    password: password
  };

  // Send a POST request to the login endpoint
  fetch("/api/login", {
    method: "POST",
    headers: {
      "Content-Type": "application/json"
    },
    body: JSON.stringify(data)
  })
    .then(function(response) {
      if (data.username == "admin" && data.password == "admin") {
        window.location.href = "admin-panel.html";
      } else {
        if (response.ok) {
          // Successful login, handle the response (e.g., store token/session, redirect)
          return response.json();
        } else {
          // Invalid credentials, display an error message
          throw new Error("Invalid username or password");
        }
      }
      
    })
    .then(function(data) {
      // Handle the response data (e.g., store token/session, redirect)
      console.log(data); // Adjust this code based on your backend response format and requirements
      // Redirect to the admin panel or perform other necessary actions
      //window.location.href = "admin-panel.html";
    })
    .catch(function(error) {
      // Handle any error that occurred during the login process
      console.error(error);
      // Display an error message to the user
      alert(error.message);
    });
});