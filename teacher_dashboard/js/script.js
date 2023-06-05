$(document).ready(function () {
    $(".data-table").each(function (_, table) {
      $(table).DataTable();
    });
  });


  document.addEventListener('click', function(event) {
    if (event.target.matches('.sidebar-link')) {
      event.preventDefault();
  
      var target = event.target.getAttribute('href');
      // Update the main content based on the target
  
      // Remove 'active' class from all sidebar links
      var sidebarLinks = document.querySelectorAll('.sidebar-link');
      sidebarLinks.forEach(function(link) {
        link.classList.remove('active');
      });
  
      // Add 'active' class to the clicked sidebar link
      event.target.classList.add('active');
    }
  });