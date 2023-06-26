// Function to check if user is authenticated and redirect after a delay
window.addEventListener("DOMContentLoaded", function() {
    // Check if user is authenticated
    if (isAuthenticated()) {
      // User is authenticated, redirect to dashboard
      console.log("User is authenticated");
      if (!window.location.href.includes("admin-panel.html")) {
        setTimeout(function() {
          window.location.href = "../teacher_dashboard/pages/admin-panel.html";
        }, 1000);
      }
    } else {
      // User is not authenticated, redirect to login
      console.log("User is not authenticated");
      this.window.location.href = "../teacher_dashboard/pages/login.html";
    }
});

  // Function to check if the JWT token is authenticated
function isAuthenticated() {
  // try and get the JWT token from the cookie
  try {
    var jwtToken = document.cookie.split(";").filter(function(item) {
      return item.includes("jwt=");
    })[0].split("=")[1];
  } catch (error) {
    console.log("Error getting JWT token from cookie/token not found");
    return false;
  }

  // Check if the JWT token exists
  if (!jwtToken) {
    console.log("Token not found");
    return false;
  }

  var decodedToken = parseJwt(jwtToken); // Parse the JWT token and extract the claims
  var expirationDate = new Date(decodedToken.exp * 1000); // Extract the JWT token expiration date

  // Check if the current date is past the JWT token expiration date
  if (new Date() > expirationDate) {
    return false;
  } else {
    return true;
  }
}

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
