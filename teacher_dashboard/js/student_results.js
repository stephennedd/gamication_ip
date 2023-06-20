const studentTableCheckbox = document.getElementById('all-students-checkbox');
const tableHeader = document.getElementById('student-results-card-header');
var students = [];

studentTableCheckbox.addEventListener('change', function () {
    if (studentTableCheckbox.checked) {
        console.log('all-students-checked');
        tableHeader.innerHTML = 'All Students';
        getStudents(true);
    } else {
        console.log('all-students-not-checked');
        tableHeader.innerHTML = 'My Students';
        getStudents(false);
    }
});

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