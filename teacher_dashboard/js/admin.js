document.getElementById("user-form").addEventListener("submit", function(event) {
    event.preventDefault(); // Prevent the form from submitting normally
  
    // Get the form input values
    var firstName = document.getElementById("first-name").value;
    var lastName = document.getElementById("last-name").value;
    var username = document.getElementById("username").value;
    var email = document.getElementById("email").value;
    var userRole = document.getElementById("user-role").value;
    const userId = email.split('@')[0];
    // const capitalizedRole = userRole.charAt(0).toUpperCase() + userRole.slice(1);  
    // Prepare the request payload
    // var data = {
    //   firstName: firstName,
    //   lastName: lastName,
    //   username: username,
    //   userId: userId,
    //   userRole: capitalizedRole
    // };

   // console.log(data);

    let admin = true; // Set the admin value

    if(userRole=="admin"){
        admin = true;
    } else{
        admin=false;
    }

    const teacherRegister = {
        // Populate teacherRegister object with the required data
        UserId: userId,
        Name: firstName,
        Surname: lastName
      };

      var token = document.cookie
			.split('; ')
			.find((row) => row.startsWith('jwt='))
			.split('=')[1];

            console.log(teacherRegister);
            console.log(admin);
fetch(`https://localhost:7186/api/Users/Admin?admin=${admin}`, {
  method: 'POST',
  headers: {
    'Content-Type': 'application/json',
    'Authorization': `Bearer ${token}` // Replace with your actual authorization token
  },
  body: JSON.stringify(teacherRegister)
})
  .then(response => {
    if (!response.ok) {
        console.log(response);
      throw new Error('Request failed.');
    }
    return response.json();
  })
  .then(data => {
    // Process the response data
    console.log(data);
  })
  .catch(error => {
    // Handle any errors that occurred during the fetch request
    console.error(error);
  });
  
   
  });
  