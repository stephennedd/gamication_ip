const tableHeader = document.getElementById('student-results-card-header');
const groupSelector = document.getElementById("group-selection-dropdown");
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


$(document).ready(function () {
    //var groups = [];
    var groups = [{id: 1 ,name: "Group 1"}, {id: 2 ,name: "Group 2"}, {id: 3 ,name: "Group 3"}];
    // TODO get the groups from the server

    // Add the groups to the dropdown
    groups.forEach((group) => {
        var option = document.createElement("option");
        option.text = group.name;
        option.value = group.id;
        console.log(group.name);
        groupSelector.appendChild(option);

    });
    
});

groupSelector.addEventListener('change', function () {
    console.log('group changed');
    // TODO get the students from the selected group
    // TODO update the students table
}
);