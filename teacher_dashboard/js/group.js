const groupTable = document.getElementById('groups-table');

$(document).ready(function () {
    populateGroupsTable();
});

// populate the groups table with the groups from the server
function populateGroupsTable() {
    // example data
    let groups = [{id: 1 ,name: "Group 1", students: [{id: 1, name: "Student 1"}]}];

    // TODO get the groups from the server

    groups.forEach((group) => {
        var row = groupTable.insertRow();
        row.setAttribute("data-groupId", group.id);
        var cell1 = row.insertCell();
        var cell2 = row.insertCell();
        var cell3 = row.insertCell();
        cell1.innerHTML = group.name;
        cell2.innerHTML = group.students.length;
        cell3.innerHTML = `<button class="btn btn-primary btn-sm me-1" onclick="editGroup(this)">Edit Group</button>
                            <button class="btn btn-success btn-sm me-1" onclick="addUser(this)">Add User</button>
                            <button class="btn btn-danger btn-sm" onclick="deleteGroup(this)">Delete Group</button>`;
    });
}

// open the add group modal
function editGroup(button) {
    // open modal
    $('#edit-group-modal').modal('show');

    // get the group name
    var groupName = button.parentNode.parentNode.cells[0].innerHTML;
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