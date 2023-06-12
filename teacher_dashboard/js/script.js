
const checkbox = document.getElementById('admin-checkbox');
const adminSection = document.getElementById('admin-section');

// Check if checkbox is checked and display admin section
// update this with a function that checks if the user is an admin
checkbox.addEventListener('change', function () {
    if (checkbox.checked) {
        console.log('checked');
        adminSection.style.display = 'block';
    } else {
        console.log('not checked');
        adminSection.style.display = 'none';
    }
});


// javascript for the data table
$(document).ready(function () {
    $(".data-table").each(function (_, table) {
      $(table).DataTable();
    });
});

//javascript for the sidebar
document.addEventListener('click', function(event) {
if (event.target.matches('.nav-link')) {
    event.preventDefault();

    var target = event.target.getAttribute('href');
    // Update the main content based on the target

    // Remove 'active' class from all sidebar links
    var sidebarLinks = document.querySelectorAll('.nav-link');
    sidebarLinks.forEach(function(link) {
    link.classList.remove('active');
    });

    // Add 'active' class to the clicked sidebar link
    event.target.classList.add('active');
}
});

// Function to show the specified section and update the active link
function showSection(sectionId, linkId) {
    // Hide all sections
    document.querySelectorAll('main').forEach((section) => {
        section.style.display = 'none';
    });

    // Show the selected section
    document.getElementById(sectionId).style.display = 'block';

    // Remove the 'active' class from all links
    document.querySelectorAll('.nav-link').forEach((link) => {
        link.classList.remove('active');
    });

    // Add the 'active' class to the selected link
    document.getElementById(linkId).classList.add('active');
}

 // Function to handle the page load or URL hash change event
function handlePageChange() {
    // Get the current URL hash (e.g., #student-results or #create-quiz)
    const hash = window.location.hash;
  
    // Define a mapping of hash values to section IDs and link IDs
    const sectionMappings = {
      '#create-quiz': { sectionId: 'create-quiz', linkId: 'create-quiz-link' },
      '#edit-quiz': { sectionId: 'edit-quiz', linkId: 'edit-quiz-link' },
      '#student-results': { sectionId: 'student-results', linkId: 'student-results-link' },
      '#delete-quiz': { sectionId: 'delete-quiz', linkId: 'delete-quiz-link' },
      '#add-student': { sectionId: 'add-student', linkId: 'add-student-link' },
      '#delete-student': { sectionId: 'delete-student', linkId: 'delete-student-link' },
      '#update-student': { sectionId: 'update-student', linkId: 'update-student-link' },
      '#ban-student': { sectionId: 'ban-student', linkId: 'ban-student-link' },
    };
  
    // Default section and link IDs if the hash doesn't match any mapping
    let defaultSectionId = 'student-results';
    let defaultLinkId = 'student-results-link';
  
    // Find the corresponding section and link IDs based on the hash
    const { sectionId, linkId } = sectionMappings[hash] || { sectionId: defaultSectionId, linkId: defaultLinkId };
  
    // Show the appropriate section and update the active link
    showSection(sectionId, linkId);
}

// Handle the initial page load
handlePageChange();

// Handle the URL hash change event
window.addEventListener('hashchange', handlePageChange);

// Quiz creation form
$(document).ready(function () {
    // Counter to keep track of the number of questions
    let questionCounter = 0;

    // Function to generate the HTML for a question and its answers
    function generateQuestionHTML(questionId) {
        const colors = ['correct', 'wrong'];
        return `
            <div class="question mb-3">
            <input type="text" name="question[${questionId}]" class="form-control" placeholder="Enter a question" required>
            <div class="mt-2 mb-3 me-3">
                <input class="form-control mx-3 mb-1 ${colors[0]}" type="text" name="answer[${questionId}][]" placeholder="Correct answer" required>
                <input class="form-control mx-3 mb-1 ${colors[1]}" type="text" name="answer[${questionId}][]" placeholder="Wrong answer" required>
                <input class="form-control mx-3 mb-1 ${colors[1]}" type="text" name="answer[${questionId}][]" placeholder="Wrong answer" required>
                <input class="form-control mx-3 ${colors[1]}" type="text" name="answer[${questionId}][]" placeholder="Wrong answer" required>
            </div>
            </div>
        `;
    }

    // Event handler for the "Add Question" button click
    $('#add-question-btn').click(function () {
      questionCounter++;
      const questionHTML = generateQuestionHTML(questionCounter);
      $('#questions-container').append(questionHTML);
    });

    // Event handler for the "Remove Question" button click
    $('#remove-question-btn').click(function () {
        if (questionCounter > 0) {
            $('#questions-container').children().last().remove();
            questionCounter--;
        }
    });

    // Event handler for the form submission
    $('#quiz-form').submit(function (e) {
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
        // open the modal
        $('#update-student-modal').modal('show');
    }

    function confirmUpdate(button) {
        // todo: send a put request to the server

        
        // close the modal
        $('#update-student-modal').modal('hide');
    }

    // Banning a student
    function banStudent(button) {
        if (confirm("Are you sure you want to ban this student?")) {
            // todo: send a put request to the server
        }
    }
