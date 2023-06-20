// Event handler for form submit
$('#create-leaderboard-form').submit(function (e) {
    // get the form data
    var formData = $(this).serializeArray();
    // Convert form data to JSON
    console.log(formData);

    const jsonData = {};
    for (let i = 0; i < formData.length; i++) {
        jsonData[formData[i].name] = formData[i].value;
    }


    // Log JSON data
    console.log(JSON.stringify(jsonData));

    console.log('Form submitted');
    e.preventDefault(); // Prevent the form from submitting for now
    
    //TODO Send the form data to the server and handle the response


});

// Event handler for updating the leaderboard
$('#update-leaderboard-form').submit(function (e) {
    // get the form data
    var formData = $(this).serializeArray();
    // Convert form data to JSON
    console.log(formData);

    const jsonData = {};
    for (let i = 0; i < formData.length; i++) {
        jsonData[formData[i].name] = formData[i].value;
    }

    // Log JSON data
    console.log(JSON.stringify(jsonData));

    console.log('Form submitted');
    e.preventDefault(); // Prevent the form from submitting for now
    // TODO Send the form data to the server
});

// Deleting a leaderboard
function deleteLeaderboard(button) {
    if (confirm("Are you sure you want to delete this leaderboard?")) {
        console.log('Deleting leaderboard');

        // Get the leaderboard id from the data attribute
        const leaderboardId = button.parentNode.parentNode.getAttribute('data-id');
        console.log('Leaderboard id: ' + leaderboardId);

        // TODO send a delete request to the server with the id ad parameter

        // if response is successful, remove the row from the table
        if (true) {
            var row = button.parentNode.parentNode; // Get the parent row
            row.parentNode.removeChild(row); // Remove the row from the table
        } else {
            alert('Error deleting leaderboard');
        }
        
    }
}

// populate the delete leaderboard table on page load
$(document).ready(function () {
    populateLeaderboardTable();
});

// populating the leaderboard table
async function populateLeaderboardTable() {
    console.log('Populating leaderboard table');
    const leaderboardTable = document.getElementById('delete-leaderboard-table');
    
    let leaderboards = [];    
    // TODO get all leaderboards from server and store in leaderboards array
    try {
        const response = await fetch('https://localhost:7186/api/leaderboards');
        const data = await response.json();
        leaderboards = data;
        // Store the subjects data in localStorage
        localStorage.setItem('subjectsData', JSON.stringify(subjects));
    
        console.log('Leaderboards data retrieved'); 
        for (let i = 0; i < leaderboards.length; i++) {
            const leaderboard = leaderboards[i];
            const row = createLeaderboardTableRow(leaderboard);
            leaderboardTable.appendChild(row);
        }
        console.log('Leaderboard table populated');
      } catch (error) {
        console.error(error);
        alert('Error retrieving leaderboards data');
    }

    
}

// create table rows for the leaderboard
function createLeaderboardTableRow(leaderboard) {
    const row = document.createElement('tr'); // Create the row
    const name = document.createElement('td'); // Create the name cell
    name.innerText = leaderboard.name; // Set the name

    // Create the delete button
    const deleteButton = document.createElement('td');
    deleteButton.classList.add('text-center');
    const button = document.createElement('button');
    button.classList.add('btn', 'btn-outline-danger', 'btn-sm');
    button.setAttribute('type', 'button');
    button.setAttribute('onclick', 'deleteLeaderboard(this)');
    button.innerText = 'Delete';
    deleteButton.appendChild(button);

    // Add the name and delete button to the row
    row.appendChild(name);
    row.appendChild(deleteButton);

    // Add the leaderboard id as a data attribute
    row.setAttribute('data-id', leaderboard.id);

    return row;
}