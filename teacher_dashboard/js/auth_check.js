// Function to check if user is authenticated and redirect after a delay
window.addEventListener("DOMContentLoaded", function() {
    var authenticationCheckComplete = false; // Flag to track authentication check completion
  
    // Function to redirect after a delay
    function redirectTo(destination) {
      if (authenticationCheckComplete) {
        window.location.href = destination;
      } else {
        setTimeout(function() {
          console.log("Redirecting to " + destination);
          redirectTo(destination);
        }, 500 * 4);
      }
    }
  
    // Make a GET request to check the user's authentication status
    fetch("/api/check-authentication")
      .then(function(response) {
        if (response.ok) {
          // User is authenticated
          authenticationCheckComplete = true;
          console.log("User is authenticated");
          redirectTo("/pages/admin-panel.html");
        } else {
          // User is not authenticated
          authenticationCheckComplete = true;
          console.log("User is not authenticated");
          //redirectTo("/pages/login.html");
          redirectTo("pages/admin-panel.html");
        }
      })
      .catch(function(error) {
        // Handle any error that occurred during the authentication status check
        console.error(error);
        // Show an error message or take appropriate action
      });
  });