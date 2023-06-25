let subjectsToBeDeleted;

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

  const deleteQuizLink = document.getElementById('delete-quiz-link');

  deleteQuizLink.addEventListener('click', async function (event) {
    event.preventDefault();
    try {
      const response = await fetch('https://localhost:7186/api/subjects');
      const data = await response.json();
      subjectsToBeDeleted = data;
      console.log(subjectsToBeDeleted);
      localStorage.setItem('subjectsDataForRemovePage', JSON.stringify(subjectsToBeDeleted));
      populateTable2(subjectsToBeDeleted);
  
      // Perform any further actions based on the response
      window.location.href = '#delete-quiz';
    } catch (error) {
      console.error(error);
    }
  });

// Check if subjects data exists in localStorage on page load
window.addEventListener('DOMContentLoaded', function () {
    console.log(localStorage.getItem('subjectsDataForRemovePage'))
    const storedSubjects = localStorage.getItem('subjectsDataForRemovePage');
  
    if (storedSubjects) {
      const subjectsToBeDeleted = JSON.parse(storedSubjects);
      populateTable2(subjectsToBeDeleted);
    }
  });


  function populateTable2(subjects) {
    // Get the table body element
    const quizTableBody = document.getElementById('quiz-table-body-remove');

  
    // Function to create a table row based on a subject object
    function createTableRow(subject) {
        console.log(subject);
      const tr = document.createElement('tr');
      tr.setAttribute('data-quiz-id', subject.Test.Id);
      tr.setAttribute('data-quiz-name', subject.Test.Title);
  
      const td1 = document.createElement('td');
      td1.textContent = subject.Test.Title;
      tr.appendChild(td1);
  
      const td2 = document.createElement('td');
      td2.textContent = subject.SubjectTitle;
      tr.appendChild(td2);
  
      const td3 = document.createElement('td');
      td3.textContent = subject.Test.Questions.length;
      tr.appendChild(td3);
    
      const td5 = document.createElement('td');
      const button = document.createElement('button');
      button.type = 'button';
      button.className = 'btn btn-sm btn-outline-danger';
      button.textContent = 'Remove';
      button.addEventListener('click', function() {
        removeQuiz(this,subject);
      });
      td5.appendChild(button);
      tr.appendChild(td5);
  
      return tr;
    }
  
    // Clear existing table rows
    quizTableBody.innerHTML = '';
  
    // Create table rows for each subject
    subjects.forEach(function(subject) {
      const tableRow = createTableRow(subject);
      quizTableBody.appendChild(tableRow);
    });
  }
  
  