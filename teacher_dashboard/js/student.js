// Adding a student
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
function confirmDelete(button) {
    if (confirm("Are you sure you want to delete this student?")) {          
      // todo: send a delete request to the server


      var row = button.parentNode.parentNode; // Get the parent row
      row.parentNode.removeChild(row); // Remove the row from the table
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
function banStudent(button) {
    if (confirm("Are you sure you want to ban this student?")) {
        // TODO: send a put request to the server
    }
}
