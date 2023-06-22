

// let groupName = " ";

// open the add group modal
function editGroup(button) {
    // open modal
    $('#edit-group-modal').modal('show');

    // get the group name
     groupName = button.parentNode.parentNode.cells[0].innerHTML;
    $('#edit-group-name').val(groupName);
}

// handle the edit group form submit
$('#edit-group-form').submit(function (event) {
    // get the form data
    var formData = {
        'name': $('#edit-group-name').val()
    };

    var json = JSON.stringify(formData);
    // TODO send the edit request to the server

    console.log(json);
    event.preventDefault();
});

function addUser(button) {
    // open modal
    $('#add-user-to-group-modal').modal('show');
}

// handle the add user form submit
$('#add-user-to-group-form').submit(function (event) {
    // get the form data
    var formData = {
        'username': $('#add-user-to-group-username').val()
    };

    var json = JSON.stringify(formData);
    // TODO send the add user request to the server

    console.log(json);
    event.preventDefault();
});

const groupForm = document.getElementById('group-form');
groupForm.addEventListener('submit', function (event) {
    event.preventDefault(); // Prevent the form from submitting normally
    groupName = document.getElementById('group-name').value;
    createGroup();
});


async function createGroup() {
    try {
        var token = document.cookie
            .split('; ')
            .find(row => row.startsWith('jwt='))
            .split('=')[1];

        // Ensure groupName is not empty or undefined
        if (!groupName || groupName.trim() === '') {
            console.error("Invalid or empty group name!");
            return;
        }
        let encodedGroupName = encodeURI(groupName);

        let response = await fetch(`https://localhost:7186/api/Groups?groupName=${encodedGroupName}`,
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

        console.log('Group added successfully:');
    } catch (error) {
        console.log('Fetch Error: ', error);
    }
}

// remove the group from the table and send the delete request to the server
function deleteGroup(button) {
    if (confirm("Are you sure you want to delete this group?")) {
        // TODO send the delete request to the server
  
        // if response == OK: remove the quiz from the page
        var serverResponse = "OK";
        if (serverResponse == "OK") {
            var row = button.parentNode.parentNode; // Get the parent row
            row.remove();
        }
        // else: show an error message
        else {alert("Error deleting group");} // TODO: show an error message
        
    }
}