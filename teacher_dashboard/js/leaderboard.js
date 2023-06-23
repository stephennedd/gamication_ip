


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
    
    let matched = JSON.stringify(jsonData).match(/"leaderboard-name":"(.*?)"/);
    leaderboardName = null;

    if (matched && matched.length > 1) {
        leaderboardName = matched[1];
    }  
    e.preventDefault(); // Prevent the form from submitting for now

    createLeaderboard();


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
        leaderboardName = button.value;
        console.log('Leaderboard id: ' + leaderboardName);

        let x = deleteLeaderboardAction();
        
        // if response is successful, remove the row from the table
        if (x) {
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
    const leaderboardTable = document.getElementById('delete-leaderboard-table');
    const updateLeaderboardTable = document.getElementById('update-leaderboard-table');

    let leaderboards = [];  
    var token = document.cookie
        .split('; ')
        .find(row => row.startsWith('jwt='))
        .split('=')[1];  
    try {
        const response = await fetch('https://localhost:7186/api/Leaderboards/', {
            method: 'GET',
            headers: {
                'Authorization': `Bearer ${token}`,
                'Content-Type': 'application/json'
            }
        });
        let data = await response.json();

        let leaderboards = data.$values.map(item => item.name);
        console.log(leaderboards)
        // Store the subjects data in localStorage
        localStorage.setItem('subjectsData', JSON.stringify(subjects));
    
        console.log('Leaderboards data retrieved'); 
        // Clear the table
        leaderboardTable.innerHTML = '';
        updateLeaderboardTable.innerHTML = '';

        for (let i = 0; i < leaderboards.length; i++) {
            const leaderboard = leaderboards[i];
            const row = createLeaderboardTableRow(leaderboard);
            const updateRow = createUpdateLeaderboardTableRow(leaderboard);
            leaderboardTable.appendChild(row);
            updateLeaderboardTable.appendChild(updateRow);
        }

      } catch (error) {
        console.error(error);
        alert('Error retrieving leaderboards data');
    }

}


// create table rows for the delete leaderboard table
function createLeaderboardTableRow(leaderboard) {
    const row = document.createElement('tr'); // Create the row
    const name = document.createElement('td'); // Create the name cell
    name.innerText = leaderboard; // Set the name

    // Create the delete button
    const deleteButton = document.createElement('td');
    deleteButton.classList.add('text-center');
    const button = document.createElement('button');
    button.classList.add('btn', 'btn-outline-danger', 'btn-sm');
    button.setAttribute('type', 'button');
    button.setAttribute('onclick', 'deleteLeaderboard(this)');
    button.setAttribute('value', leaderboard);
    button.innerText = 'Delete';
    deleteButton.appendChild(button);

    // Add the name and delete button to the row
    row.appendChild(name);
    row.appendChild(deleteButton);

    // Add the leaderboard id as a data attribute
    row.setAttribute('data-id', leaderboard.id);

    return row;
}

// create table rows for the update leaderboard table
function createUpdateLeaderboardTableRow(leaderboard) {
    //example row
//     `<tr>
//     <td>Flappy-Bird leaderboard</td>
//     <td>Flappy-Bird</td>
//     <td class="text-center"><button type="button" class="btn btn-outline-primary btn-sm"
//         data-bs-toggle="modal" data-bs-target="#update-leaderboard-modal">Update</button></td>
//   </tr>`
    const row = document.createElement('tr'); // Create the row
    const name = document.createElement('td'); // Create the name cell
    name.innerText = leaderboard; // Set the name

    // create update button
    const updateButton = document.createElement('td');
    updateButton.classList.add('text-center');
    const button = document.createElement('button');
    button.classList.add('btn', 'btn-outline-primary', 'btn-sm');
    button.setAttribute('type', 'button');
    button.setAttribute('data-bs-toggle', 'modal');
    button.setAttribute('data-bs-target', '#update-leaderboard-modal');
    button.innerText = 'Update';
    updateButton.appendChild(button);

    // Add the name and update button to the row
    row.appendChild(name);
    row.appendChild(updateButton);

    // Add the leaderboard id as a data attribute
    row.setAttribute('data-id', leaderboard.id);

    return row;
}

// populate the update leaderboard modal
$('#update-leaderboard-modal').on('show.bs.modal', function (event) {
    const button = $(event.relatedTarget); // Button that triggered the modal
    const leaderboardId = button.parents('tr').attr('data-id'); // Extract info from data-* attributes
    console.log('Leaderboard id: ' + leaderboardId);

    // Get the leaderboard data from localStorage
    const leaderboards = JSON.parse(localStorage.getItem('leaderboardsData'));
    const leaderboard = leaderboards.find(leaderboard => leaderboard.id == leaderboardId);
    console.log(leaderboard);

    // Populate the form
    const modal = $(this);
    modal.find('#update-leaderboard-id').val(leaderboard.id);
    modal.find('#update-leaderboard-name').val(leaderboard.name);
});
async function createLeaderboard(){
    try {
        var token = document.cookie
            .split('; ')
            .find(row => row.startsWith('jwt='))
            .split('=')[1];

        // Ensure groupName is not empty or undefined
        if (!leaderboardName || leaderboardName.trim() === '') {
            console.error("Invalid or empty leaderboard name!");
            return;
        }
        let encodedleaderboardName = encodeURI(leaderboardName);

        let response = await fetch(`https://localhost:7186/api/Leaderboards?leaderboardName=${encodedleaderboardName}`,
            {
                method: 'POST',
                headers: {
                    'Authorization': `Bearer ${token}`,
                    'Content-Type': 'application/json'
                },
            });

        if (!response.ok) {
            throw new Error(`HTTP error! status: ${response.status}`);
        }

        console.log('Leaderboard added successfully:');
        
        populateLeaderboardTable();
    } catch (error) {
        console.log('Fetch Error: ', error);
    }
}
async function deleteLeaderboardAction(){
    try {
        let response;
        var token = document.cookie
            .split('; ')
            .find(row => row.startsWith('jwt='))
            .split('=')[1];

        // Ensure groupName is not empty or undefined
        if (!leaderboardName || leaderboardName.trim() === '') {
            console.error("Invalid or empty leaderboard name!");
            return;
        }
        let encodedleaderboardName = encodeURI(leaderboardName);

        response = await fetch(`https://localhost:7186/api/Leaderboards/${encodedleaderboardName}`,
            {
                method: 'DELETE',
                headers: {
                    'Authorization': `Bearer ${token}`,
                    'Content-Type': 'application/json'
                },
            });

        if (!response.ok) {
            throw new Error(`HTTP error! status: ${response.status}`);
        }

        console.log('Leaderboard deleted successfully:');
        return true;
    } catch (error) {
        console.log('Fetch Error: ', error);
    }
}