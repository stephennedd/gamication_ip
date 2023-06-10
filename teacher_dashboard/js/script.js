
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


// javascript for the quiz creation page
var fieldCounter = 2;

function addFormField() {
    var dynamicFieldsDiv = document.getElementById('dynamicFields');

    // Create a new form group
    var formGroup = document.createElement('div');
    formGroup.classList.add('form-group');

    // Create a new input element
    var input = document.createElement('input');
    input.type = 'text';
    input.classList.add('form-control');
    input.classList.add('mb-2');
    input.name = 'question' + fieldCounter;
    input.placeholder = 'Question ' + fieldCounter;

    // Append the input element to the form group
    formGroup.appendChild(input);

    // Append the form group to the dynamicFields div
    dynamicFieldsDiv.appendChild(formGroup);

    fieldCounter++;
}

function removeFormField() {
    var dynamicFieldsDiv = document.getElementById('dynamicFields');

    if (fieldCounter > 2) {
    // Remove the last form group
    dynamicFieldsDiv.removeChild(dynamicFieldsDiv.lastChild);
    
    fieldCounter--;
    }

}