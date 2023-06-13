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
    
    
});
