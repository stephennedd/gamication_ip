const groupSelector = document.getElementById("group-selection-dropdown");

$(document).ready(function () {
    //var groups = [];
    var groups = [{name: "Group 1"}, {name: "Group 2"}, {name: "Group 3"}];
    // TODO get the groups from the server

    // Add the groups to the dropdown
    groups.forEach((group) => {
        var option = document.createElement("option");
        option.text = group;
        groupSelector.add(option);
    }
    );
    
});