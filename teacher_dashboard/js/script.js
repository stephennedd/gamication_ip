const checkbox = document.getElementById('admin-checkbox');
const adminSection = document.getElementById('admin-section');
const userRole = localStorage.getItem('role');

if (userRole=="Admin") {
 adminSection.style.display = 'block';
} else {
 adminSection.style.display = 'none';
}

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
        '#add-leaderboard': { sectionId: 'add-leaderboard', linkId: 'add-leaderboard-link' },
        '#delete-leaderboard': { sectionId: 'delete-leaderboard', linkId: 'delete-leaderboard-link' },
        '#update-leaderboard': { sectionId: 'update-leaderboard', linkId: 'update-leaderboard-link' },
        '#add-subject': { sectionId: 'add-subject', linkId: 'add-subject-link' },
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
