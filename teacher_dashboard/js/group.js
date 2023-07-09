let oldGroupName = null;

$(document).ready(function () {
	populateGroupsTable();
});

// open the add group modal
function editGroup(button) {
	// open modal
	$('#edit-group-modal').modal('show');

	// get the group name
	groupName = button.parentNode.parentNode.cells[0].innerHTML;
	oldGroupName = groupName;
	$('#edit-group-name').val(groupName);
}

// handle the edit group form submit
$('#edit-group-form').submit(function (event) {
	// get the form data
	var formData = {
		name: $('#edit-group-name').val(),
	};

	var json = JSON.stringify(formData);


	let newGroupName = JSON.parse(json).name;
	event.preventDefault();
	updateGroup(oldGroupName, newGroupName);
});

function addUser(button) {
	// open modal
	$('#add-user-to-group-modal').modal('show');
}

// handle the add user form submit
$('#add-user-to-group-form').submit(function (event) {
	// get the form data
	var formData = {
		username: $('#add-user-to-group-username').val(),
	};

	var json = JSON.stringify(formData);
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
			.find((row) => row.startsWith('jwt='))
			.split('=')[1];

		// Ensure groupName is not empty or undefined
		if (!groupName || groupName.trim() === '') {
			console.error('Invalid or empty group name!');
			return;
		}
		let encodedGroupName = encodeURI(groupName);

		let response = await fetch(
			`http://localhost:4434/api/Groups?groupName=${encodedGroupName}`,
			{
				method: 'POST',
				headers: {
					Authorization: `Bearer ${token}`,
					'Content-Type': 'application/json',
				},
			}
		);

		if (!response.ok) {
			alert('Error adding group');
			throw new Error(`HTTP error! status: ${response.status}`);
		}
		if (response.status == 200) {
			refreshGroupsTable();
			$('#add-group-success-modal').modal('show');
		}
	} catch (error) {
		console.log('Fetch Error: ', error);
	}
}

// remove the group from the table and send the delete request to the server
function deleteGroup(button) {
	if (confirm('Are you sure you want to delete this group?')) {
		// TODO send the delete request to the server

		// if response == OK: remove the quiz from the page
		var serverResponse = 'OK';
		if (serverResponse == 'OK') {
			var row = button.parentNode.parentNode; // Get the parent row
			row.remove();
		}
		// else: show an error message
		else {
			alert('Error deleting group');
		} // TODO: show an error message
	}
}
async function populateGroupsTable() {
	try {
		var token = document.cookie
			.split('; ')
			.find((row) => row.startsWith('jwt='))
			.split('=')[1];

		let response = await fetch('http://localhost:4434/api/Groups', {
			method: 'GET',
			headers: {
				Authorization: `Bearer ${token}`, // replace `token` with your actual token variable
				'Content-Type': 'application/json',
			},
		});

		if (!response.ok) {
			throw new Error(`HTTP error! status: ${response.status}`);
		}

		let data = await response.json();

		// Extract groups array
		let groups = data.$values;

		// Get a reference to the table body
		let tableBody = document.querySelector('#groups-table tbody');

		// Remove any existing rows in the table
		while (tableBody.firstChild) {
			tableBody.firstChild.remove();
		}

		// Iterate over the groups array and add a new row for each group
		for (let group of groups) {
			let row = document.createElement('tr');

			let nameCell = document.createElement('td');
			nameCell.textContent = group.name;
			row.appendChild(nameCell);

			//TODO get value from server
			//let studentsCountCell = document.createElement('td');
			//studentsCountCell.textContent = group.studentsCount;
			//row.appendChild(studentsCountCell);

			let actionsCell = document.createElement('td');
			// <button class="btn btn-danger" onclick="deleteGroup(this)">Delete</button>
			// <button class="btn btn-success" onclick="addUser(this)">Add User</button>
			actionsCell.innerHTML = `
                <button class="btn btn-sm btn-primary" onclick="editGroup(this)">Edit</button>
            `;
			// Add any actions buttons or links here
			row.appendChild(actionsCell);

			tableBody.appendChild(row);
		}
	} catch (error) {
		console.log('Fetch Error: ', error);
	}
}

// refresh the groups table
function refreshGroupsTable() {
	populateGroupsTable();
}
async function updateGroup(oldGroupName, newGroupName) {
	try {
		let response;
		var token = document.cookie
			.split('; ')
			.find((row) => row.startsWith('jwt='))
			.split('=')[1];

		// Ensure groupName is not empty or undefined
		if (!oldGroupName || oldGroupName.trim() === '' || !newGroupName) {
			console.error('Invalid or empty Group name!');
			return;
		}
		let encodedoldGroupName = encodeURI(oldGroupName);
		let encodednewGroupName = encodeURI(newGroupName);
		response = await fetch(
			`https://localhost:7186/api/Groups/${encodedoldGroupName}?newName=${encodednewGroupName}`,
			{
				method: 'PATCH',
				headers: {
					Authorization: `Bearer ${token}`,
					'Content-Type': 'application/json',
				},
			}
		);

		if (!response.ok) {
			alert(`Failed to update Group. error code: ${response.status} `);
			
		}
		$('#edit-group-modal').modal('hide');

		refreshGroupsTable();
		return true;
	} catch (error) {
		console.log('Fetch Error: ', error);
		alert(`Failed to update group. error code: ${error} `);
	}
}
