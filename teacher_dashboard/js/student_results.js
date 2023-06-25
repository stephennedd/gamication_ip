const tableHeader = document.getElementById('student-results-card-header');
const groupSelector = document.getElementById("group-selection-dropdown");
const leaderboardSelector = document.getElementById("leaderboard-selection-dropdown");
let groupName = null;
let leaderboardName = "main";

//var students = [];

// studentTableCheckbox.addEventListener('change', function () {
//     if (studentTableCheckbox.checked) {
//         console.log('all-students-checked');
//         tableHeader.innerHTML = 'All Students';
//         getStudents(true);
//     } else {
//         console.log('all-students-not-checked');
//         tableHeader.innerHTML = 'My Students';
//         getStudents(false);
//     }
// });

async function getStudents(allStudents) {
    students = [];

    if (allStudents) {
        console.log('getting all-students');
        // TODO get all students and store in students array
    }
    else {
        console.log('getting my-students');
        // TODO get my students and store in students array
    }

    return students;
}


const groupTable = document.getElementById('groups-table');

$(document).ready(function () {
    fetchGroupNames();
    fetchLeaderboardNames();
});

async function fetchGroupNames() {
    var token = document.cookie
        .split('; ')
        .find(row => row.startsWith('jwt='))
        .split('=')[1];
    try {
        const response = await fetch('https://localhost:7186/api/Groups', {
            method: 'GET',
            headers: {
                'Authorization': `Bearer ${token}`,
                'Content-Type': 'application/json'
            }
        });

        if (!response.ok) {
            throw new Error(`HTTP error! status: ${response.status}`);
        }

        const data = await response.json();
        let groups = data.$values;

        populateGroupsDropdown(groups);
    } catch (error) {
        console.log('Fetch Error: ', error);
    }
}
async function fetchLeaderboardNames() {
    var token = document.cookie
        .split('; ')
        .find(row => row.startsWith('jwt='))
        .split('=')[1];
    try {
        const response = await fetch('https://localhost:7186/api/Leaderboards', {
            method: 'GET',
            headers: {
                'Authorization': `Bearer ${token}`,
                'Content-Type': 'application/json'
            }
        });

        if (!response.ok) {
            throw new Error(`HTTP error! status: ${response.status}`);
        }

        let data = await response.json();
        console.log('Received leaderboard data:', data); 

        // Extract the leaderboard objects
        const leaderboards = data.$values;
        console.log('Leaderboard objects:', leaderboards); 

        // Extract leaderboard names
        const leaderboardNames = leaderboards.map(leaderboard => leaderboard.name);


        populateLeaderboardsDropdown(leaderboardNames);
    } catch (error) {
        console.log('Fetch Error: ', error);
    }
}

function populateGroupsDropdown(groups) {
    const dropdown = document.getElementById('group-selection-dropdown');
    dropdown.innerHTML = '<option selected disabled value="">Select a group</option>';  // clear existing options
    var option = document.createElement("option");
        option.text = "All Students";   
        option.value = "All Students";
        groupSelector.appendChild(option);
    // Add the groups to the dropdown
    groups.forEach((group) => {
        var option = document.createElement("option");
        option.text = group.name;
        option.value = group.name;
        groupSelector.appendChild(option);

    });
    dropdown.addEventListener('change', function() {
        groupName = this.value;  // update the label
        fetchLeaderboardData();
    });
}
function populateLeaderboardsDropdown(leaderboardNames) {
    const dropdown = document.getElementById('leaderboard-selection-dropdown');
    dropdown.innerHTML = '<option selected disabled value="">Select a leaderboard</option>';  // clear existing options
    // Add the groups to the dropdown
    leaderboardNames.forEach((leaderboard) => {
        var option = document.createElement("option");
        option.text = leaderboard;
        option.value = leaderboard;
        leaderboardSelector.appendChild(option);

    });
    dropdown.addEventListener('change', function() {
        leaderboardName = this.value;  // update the label
        fetchLeaderboardData();
    });
}

async function fetchLeaderboardData() {

    var token = document.cookie
        .split('; ')
        .find(row => row.startsWith('jwt='))
        .split('=')[1];  
    try {
        console.log("groupname: " + groupName)
        let response;
        if (groupName == null || groupName == "All Students")
        {
         response = await fetch(`https://localhost:7186/api/Leaderboards/${leaderboardName}`, 
            {
            method: 'GET',
            headers: {
                'Authorization': `Bearer ${token}`,
                'Content-Type': 'application/json'
            }
            });
        }   
        else {
        let encodedGroupName = encodeURI(groupName);
        console.log("encoded name: " + encodedGroupName)
        response = await fetch(`https://localhost:7186/api/Leaderboards/${leaderboardName}?group=${encodedGroupName}`, {
            method: 'GET',
            headers: {
                'Authorization': `Bearer ${token}`,
                'Content-Type': 'application/json'
            }
        });
        console.log("we are here");
        }
        if (!response.ok) {
            throw new Error(`HTTP error! status: ${response.status}`);
        }

        const data = await response.json();

        let highScores = data.highScores.$values;

        let leaderboardData = highScores.map((scoreEntry, index) => {
            if (scoreEntry.user) {
                return {
                    leaderboardPosition: index + 1,
                    username: scoreEntry.user.userId,
                    score: scoreEntry.score,
                    isBanned: scoreEntry.user.isBanned ? 'Yes' : 'No'
                };
            }
        });
        console.log(data);
        populateLeaderboard(leaderboardData);
    } catch (error) {
        console.log('Fetch Error: ', error);
    }
}
fetchLeaderboardData();

function populateLeaderboard(leaderboardData) {
    console.log('Populating leaderboard');
    // Clear the table
    $("#student-results-table").empty();

    // Populate the table
    leaderboardData.forEach(entry => {
        var row = `<tr><td>${entry.leaderboardPosition}</td><td>${entry.username}</td><td>${entry.score}</td><td>${entry.isBanned}</td></tr>`;
        $("#student-results-table").append(row);
    });
}

groupSelector.addEventListener('change', function () {
    console.log('Group changed to: ' + groupSelector.value);
    tableHeader.innerHTML = 'Group ' + groupSelector.value;
    // TODO get the students from the selected group
    // TODO update the students table

}
);
function encodeSpaces(str) {
    return str.replace(/ /g, "%20");
}
