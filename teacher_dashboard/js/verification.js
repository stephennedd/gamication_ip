const inputs = ["input1", "input2", "input3", "input4", "input5", "input6"];

inputs.map((id) => {
  const input = document.getElementById(id);
  addListener(input);
});

function addListener(input) {
  input.addEventListener("keyup", () => {
    const n = input.nextElementSibling;
    if (n) n.focus();
    
    const key = event.key; // const {key} = event; ES6+
    if (key === "Backspace" || key === "Delete") {
      const prev = input.previousElementSibling;
      if (prev) prev.focus();
    }
  });
}

$(document).ready(function(){

  // Event handler for the form submit
  $('#verification-form').submit(function(e){

    // Get the value of the input field with id="input1..6"
    var input1 = document.getElementById("input1").value;
    var input2 = document.getElementById("input2").value;
    var input3 = document.getElementById("input3").value;
    var input4 = document.getElementById("input4").value;
    var input5 = document.getElementById("input5").value;
    var input6 = document.getElementById("input6").value;

    // create a variable to store the value of the input field
    var code = input1 + input2 + input3 + input4 + input5 + input6;
    console.log(code);

    // TODO submit form to server
    var token = document.cookie
    .split('; ')
    .find(row => row.startsWith('jwt='))
    .split('=')[1];

fetch('https://localhost:7186/api/Users/' + code, {
  method: 'POST', // or 'PUT'
  headers: {
    'Content-Type': 'application/json',
    // Include the token in the Authorization header
    'Authorization': `Bearer ${token}`
  },
  // Convert the code to JSON before sending
  body: JSON.stringify({code: code})
})
.then((response) => {
  // Check if request was successful
  if(response.ok) {
    return response.json();
  } else {
    throw new Error('Server response wasn\'t OK');
  }
})
.then((data) => {
  console.log(data);
  // Do something with the data
  $('#response-message').empty();
  if (data.success) { // Assuming the API returns { success: true } for correct codes
    var html = `<p class="text-success">Code is correct</p>
                <a href="login.html" class="btn btn-primary">Login</a>`;
    $('#response-message').append(html);
  } else {
    var html = `<p class="text-danger">Code is incorrect</p>`;
    $('#response-message').append(html);
  }
})
.catch((error) => {
  console.error('Error:', error);
});
    // simulate server response
    var serverResponse = true;
    // clear previous response message
    $('#response-message').empty();
    // alert user if code is correct
    if (serverResponse) {
      var html = `<p class="text-success">Code is correct</p>
                  <a href="login.html" class="btn btn-primary">Login</a>`;
      $('#response-message').append(html);
    } else {
      var html = `<p class="text-danger">Code is incorrect</p>`;
      $('#response-message').append(html);
    }
    // REMOVE AFTER TEST Prevent form submission which refreshes page for now
    e.preventDefault();
  });
  
});