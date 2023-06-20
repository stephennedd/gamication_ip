// Adding a student
let students;
let chosenStudent;

const tableBody = document.getElementById("studentTableBody");
const tableBody1 = document.getElementById("studentTableBodyForRemoveStudent");
const tableBody2 = document.getElementById("studentTableBodyForBanStudent");

// Add event listener to the "Update Student" button
document.getElementById('update-student-link').addEventListener('click', function(event) {
    fetchStudents(event, 'update');
  });
document.getElementById('delete-student-link').addEventListener('click', function(event) {
    fetchStudents(event, 'delete');
  });
document.getElementById('ban-student-link').addEventListener('click', function(event) {
    fetchStudents(event, 'ban');
  });  

  async function deleteStudent(userId) {
    // Perform the fetch request to delete the student
   await fetch(`https://localhost:7186/api/Users/${userId}`, {
      method: 'DELETE'
    })
      .then(response => {
        if (response.ok) {
          // Student deletion successful

          console.log(`Student with ID ${userId} deleted successfully.`);
          // Perform any additional actions or updates on the UI if needed
        } else {
          // Student deletion failed
          console.error('Error:', response.status);
          // Handle the error or display an appropriate error message
        }
      })
      .catch(error => {
        console.error('Error:', error);
      });
  }  

  async function updateBannedStatus(id, isBanned) {
    const url = `https://localhost:7186/api/users/ban/${id}`;
  
    // Prepare the request body
    const requestBody = JSON.stringify(isBanned);
  
    // Send the PUT request
    await fetch(url, {
      method: 'PUT',
      headers: {
        'Content-Type': 'application/json'
      },
      body: requestBody
    })
      .then(response => {
        if (response.ok) {
          console.log('User banned status updated successfully.');
          // Handle the success case if needed
        } else {
          console.error('Error updating user banned status:', response.status);
          // Handle the error case if needed
        }
      })
      .catch(error => {
        console.error('Error updating user banned status:', error);
        // Handle the error case if needed
      });
  }

async function fetchStudents(event,linkType) {
 // event.preventDefault(); // Prevent the default behavior of the link click

  // Perform the fetch request to get all existing students
  await fetch('https://localhost:7186/api/Users/role/students') // Replace '/api/students' with the appropriate API endpoint
    .then(response => response.json())
    .then(data => {
      // Do something with the received students data
      students = data;
      localStorage.setItem('studentsData', JSON.stringify(students));
      if(linkType=='update'){
      populateTableWithData(students,tableBody);
      window.location.href = '#update-student';
      } else if(linkType=='delete'){
        populateTableWithDataForRemoveStudent(students,tableBody1);
        window.location.href = '#delete-student';
      } else if(linkType=='ban'){
        populateTableForBanStudent(students,tableBody2);
        window.location.href = '#ban-student';
      }
    })
    .catch(error => {
      console.error('Error:', error);
    });
}

// Check if subjects data exists in localStorage on page load
window.addEventListener('DOMContentLoaded', function () {
    const storedStudents = localStorage.getItem('studentsData');
    if (storedStudents) {
      const students = JSON.parse(storedStudents);
      populateTableWithData(students,tableBody);
      populateTableWithDataForRemoveStudent(students,tableBody1);
      populateTableForBanStudent(students,tableBody2);
    }
  });

function populateTableWithData(students,tableBody) {
     // Clear existing table data
      tableBody.innerHTML = '';
  
    students.forEach(student => {
      const row = document.createElement("tr");
  
      row.setAttribute("data-student-id", student.studentId);
      row.setAttribute("data-first-name", student.firstName);
      row.setAttribute("data-middle-name", student.middleName);
      row.setAttribute("data-last-name", student.lastName);
      row.setAttribute("data-student-email", student.studentEmail);
  
      const studentIdCell = createTableCell(student.UserId, "th", { scope: "row" });
      const firstNameCell = createTableCell(student.UserId, "td");
      const middleNameCell = createTableCell(student.UserId, "td");
      const lastNameCell = createTableCell(student.UserId, "td");
      const studentEmailCell = createTableCell(`${student.UserId}@student.saxion.nl`, "td");
  
      const updateButtonCell = document.createElement("td");
      const updateButton = createUpdateButton("Update");
      updateButton.addEventListener("click", function() {
        updateStudent(this);
      });
      updateButtonCell.appendChild(updateButton);
  
      row.appendChild(studentIdCell);
      row.appendChild(firstNameCell);
      row.appendChild(middleNameCell);
      row.appendChild(lastNameCell);
      row.appendChild(studentEmailCell);
      row.appendChild(updateButtonCell);
  
      tableBody.appendChild(row);
    });
  }

  function populateTableWithDataForRemoveStudent(students, tableBody) {
    // Clear existing table data
    tableBody.innerHTML = '';
  
    students.forEach(student => {
      const row = document.createElement("tr");
  
      const studentIdCell = createTableCell(student.UserId, "th", { scope: "row" });
      const fullNameCell = createTableCell(`${student.UserId}`, "td");
      const studentEmailCell = createTableCell(`${student.UserId}@student.saxion.nl`, "td");
  
      const removeButtonCell = document.createElement("td");
      const removeButton = createRemoveButton("Remove");
      removeButton.addEventListener("click", function() {
        chosenStudent = student;
        confirmDelete(this);
      });
      removeButtonCell.appendChild(removeButton);
  
      row.appendChild(studentIdCell);
      row.appendChild(fullNameCell);
      row.appendChild(studentEmailCell);
      row.appendChild(removeButtonCell);
  
      tableBody.appendChild(row);
    });
  }

  function populateTableForBanStudent(students, tableBody) {
    // Clear existing table data
    tableBody.innerHTML = '';
  
    students.forEach(student => {
      const row = document.createElement("tr");
  
      const studentIdCell = createTableCell(student.UserId, "th", { scope: "row" });
      const fullNameCell = createTableCell(`${student.UserId}`, "td");
      const studentEmailCell = createTableCell(`${student.UserId}@student.saxion.nl`, "td");
      const banStatusCell = createTableCell(student.IsBanned ? "Yes" : "No", "td");
  
      const banButtonCell = document.createElement("td");
      const banButton = createBanButton(student.IsBanned ? "Unban" : "Ban");
      banButton.addEventListener("click", function() {
        chosenStudent = student;
        banStudent(this);
      });
      banButtonCell.appendChild(banButton);
  
      row.appendChild(studentIdCell);
      row.appendChild(fullNameCell);
      row.appendChild(studentEmailCell);
      row.appendChild(banStatusCell);
      row.appendChild(banButtonCell);
  
      tableBody.appendChild(row);
    });
  }

  function createBanButton(text) {
    const button = document.createElement("button");
    button.type = "button";
    button.className = "btn btn-sm btn-outline-warning";
    button.textContent = text;
    return button;
  }
  
  function createRemoveButton(text) {
    const button = document.createElement("button");
    button.type = "button";
    button.className = "btn btn-sm btn-outline-danger";
    button.textContent = text;
    return button;
  }
  
  function createTableCell(text, tagName, attributes = {}) {
    const cell = document.createElement(tagName);
    Object.entries(attributes).forEach(([attr, value]) => {
      cell.setAttribute(attr, value);
    });
    cell.textContent = text;
    return cell;
  }
  
  function createUpdateButton(text) {
    const button = document.createElement("button");
    button.setAttribute("type", "button");
    button.classList.add("btn", "btn-sm", "btn-outline-primary");
    button.textContent = text;
    return button;
  }

$(document).ready(function () {
    // Event handler for the form submission
    $('#student-form').submit(function (e) {
        //get the form data
        var formData = $(this).serializeArray();
        // Convert form data to JSON
        const jsonData = {};
        $(formData).each(function(index, field) {
        if (jsonData[field.name] !== undefined) {
            if (!jsonData[field.name].push) {
            jsonData[field.name] = [jsonData[field.name]];
            }
            jsonData[field.name].push(field.value || '');
        } else {
            jsonData[field.name] = field.value || '';
        }
        });

        // Log JSON data
        console.log(JSON.stringify(jsonData));

        console.log('Form submitted');
    e.preventDefault(); // Prevent the form from submitting for now
    // TODO: Submit the form using Ajax
    });
});

// Deleting a student
async function confirmDelete(button) {
    if (confirm("Are you sure you want to delete this student?")) {          
      // todo: send a delete request to the server
      var row = button.parentNode.parentNode; // Get the parent row
      row.parentNode.removeChild(row); // Remove the row from the table
      await deleteStudent(chosenStudent.UserId);
      await fetchStudents(event,'delete');
    }
}

// Updating a student
function updateStudent(button) {
    var row = button.parentNode.parentNode; // Get the parent row
    var studentId = row.dataset.studentId;
    var firstName = row.dataset.firstName;
    var middleName = row.dataset.middleName;
    var lastName = row.dataset.lastName;

    // open the modal and show the student name
    $('#update-student-modal').modal('show');
    $('#edit-student-modal-title').text(`Edit Student: ${firstName} ${lastName}`);
    $('#modal-first-name').val(firstName);
    $('#modal-middle-name').val(middleName);
    $('#modal-last-name').val(lastName);
}

function confirmStudentUpdate(button) {
    if (confirm("Are you sure you want to update this student?")) {
        // TODO: send a put request to the server
        
        // close the modal
        $('#update-student-modal').modal('hide');
    }
}

// Banning a student
async function banStudent(button) {
    if(!chosenStudent.IsBanned){
    if (confirm("Are you sure you want to ban this student?")) {
       await updateBannedStatus(chosenStudent.Id,true);
       await fetchStudents(event,'ban');
        // TODO: send a put request to the server
    }
    } else{
        if (confirm("Are you sure you want to unban this student?")) {
            await updateBannedStatus(chosenStudent.Id,false);
            await fetchStudents(event,'ban');
             // TODO: send a put request to the server
         } 
    }
}
