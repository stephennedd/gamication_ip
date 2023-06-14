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
    
    //TODO Send the form data to the server

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
        // TODO send a delete request to the server

        var row = button.parentNode.parentNode; // Get the parent row
        row.parentNode.removeChild(row); // Remove the row from the table
    }
}
