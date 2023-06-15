// Quiz creation form
$(document).ready(function () {
    // Function to generate the HTML for a question and its answers
    function generateQuestionHTML(questionId) {
        const colors = ['correct', 'wrong'];
        return `
            <div class="created-question mb-3">
            <div class="col-md-12">
                <div class="row align-items-center">
                    <div class="col me-2">
                        <label for="question[${questionId}]" class="form-label">Question ${questionId + 1}</label>
                    </div>
                    <div class="col mb-2 text-end">
                        <button id="remove-question" type="button" class="btn btn-danger btn-sm" onclick="removeQuestion(this)">Remove</button>
                    </div>
                </div>
            </div>
            <input type="text" name="question[${questionId}]" class="form-control" placeholder="Enter a question" required>
            <div class="mt-2 mb-3 me-3">
                <input class="form-control mx-3 mb-1 ${colors[0]}" type="text" name="answer[${questionId}][]" placeholder="Correct answer" required>
                <input class="form-control mx-3 mb-1 ${colors[1]}" type="text" name="answer[${questionId}][]" placeholder="Wrong answer" required>
                <input class="form-control mx-3 mb-1 ${colors[1]}" type="text" name="answer[${questionId}][]" placeholder="Wrong answer" required>
                <input class="form-control mx-3 ${colors[1]}" type="text" name="answer[${questionId}][]" placeholder="Wrong answer" required>
            </div>
            </div>
        `;
    }

    // Event handler for the "Add Question" button click
    $('#add-question-btn').click(function () {
        // count number of questions
        var questions = document.getElementsByClassName('created-question');
        questionCounter = questions.length; 
        const questionHTML = generateQuestionHTML(questionCounter);
        // Add the question to the page
      $('#questions-container').append(questionHTML);
    });


    // Event handler for the form submission
    $('#quiz-form').submit(function (e) {
        //get the form data
        var formData = $(this).serializeArray();
        // Convert form data to JSON
        const jsonData = {};
        $(formData).each(function(index, field) {
          if (jsonData[field.name] !== undefined) {
            if (!jsonData[field.name].push) {
              jsonData[field.name] = [jsonData[field.name]];
            }
            jsonData[field.name].push(field.value || '');
          } else {
            jsonData[field.name] = field.value || '';
          }
        });

        // Log JSON data
        console.log(JSON.stringify(jsonData));

        console.log('Form submitted');
      e.preventDefault(); // Prevent the form from submitting for now
      // TODO: Submit the form using Ajax
    });
});

// Event handler for the "Remove Question" button click
function removeQuestion(button) {
    // Get the question container
    const questionContainer = button.parentNode.parentNode.parentNode.parentNode;
    // Remove the question container
    questionContainer.remove();

    // renumber the questions
    var questions = document.getElementsByClassName('created-question');
    for (var i = 0; i < questions.length; i++) {
        var question = questions[i];
        question.querySelector('label').innerText = `Question ${i + 1}`;
    }
}

// update a quiz
function editQuiz(button) {
    var exampleQuestions = [
        {questionText: "What is the capital of France?", correctAnswer: "Paris" ,answers: ["Paris", "London", "Berlin", "Madrid"]},
        {questionText: "What is the capital of Spain?", correctAnswer: "Madrid" ,answers: ["Madrid", "London", "Berlin", "Paris"]},
        {questionText: "What is the capital of Germany?", correctAnswer: "Berlin" ,answers: ["Berlin", "London", "Paris", "Madrid"]},
    ];



    // Get the quiz ID from the row using the data attribute
    var row = button.parentNode.parentNode; // Get the parent row
    const quizId = row.dataset.quizId;
    var quizName = row.dataset.quizName;

    // open the modal and show the quiz name
    $('#edit-quiz-modal').modal('show');
    $('#edit-quiz-modal-header').text(`Edit Quiz ${quizName}`);
    $('#modal-quiz-name').val(quizName);

    // load questions into the modal
    var questionsContainer = $('#modal-questions-container');
    questionsContainer.empty();

    // Loop through the example questions and add them to the modal
    for (var i = 0; i < exampleQuestions.length; i++) {
        var question = exampleQuestions[i];
        var questionHTML = `
            <div class="question mb-3">
            <label type="text" class="form-label">Question ${i+1}</label>
            <input type="text" name="question[${i}]" class="form-control" placeholder="Enter a question" required value="${question.questionText}">
            <div class="mt-2 mb-3 me-3">
                <input class="form-control mx-3 mb-1 correct" type="text" name="answer[${i}][]" placeholder="Correct answer" required value="${question.answers[0]}">
                <input class="form-control mx-3 mb-1 wrong" type="text" name="answer[${i}][]" placeholder="Wrong answer" required value="${question.answers[1]}">
                <input class="form-control mx-3 mb-1 wrong" type="text" name="answer[${i}][]" placeholder="Wrong answer" required value="${question.answers[2]}">
                <input class="form-control mx-3 wrong" type="text" name="answer[${i}][]" placeholder="Wrong answer" required value="${question.answers[3]}">
            </div>
            </div>
        `;
        // add the question to the modal
        questionsContainer.append(questionHTML);
    }
}

// add a question to the edit quiz modal
function addQuestionToModal() {
    // get the number of questions currently in the modal
    var numQuestions = $('#modal-questions-container').children().length;

    // add a new question to the modal
    var questionHTML = `
        <div class="question mb-3">
        <label type="text" class="form-label">Question ${numQuestions+1}</label>
        <input type="text" name="question[${numQuestions}]" class="form-control" placeholder="Enter a question" required>
        <div class="mt-2 mb-3 me-3">
            <input class="form-control mx-3 mb-1 correct" type="text" name="answer[${numQuestions}][]" placeholder="Correct answer" required>
            <input class="form-control mx-3 mb-1 wrong" type="text" name="answer[${numQuestions}][]" placeholder="Wrong answer" required>
            <input class="form-control mx-3 mb-1 wrong" type="text" name="answer[${numQuestions}][]" placeholder="Wrong answer" required>
            <input class="form-control mx-3 wrong" type="text" name="answer[${numQuestions}][]" placeholder="Wrong answer" required>
        </div>
        </div>
    `;
    $('#modal-questions-container').append(questionHTML);
}

// remove a question from the edit quiz modal
function removeQuestionFromModal() {
    // get the number of questions currently in the modal
    var numQuestions = $('#modal-questions-container').children().length;

    // remove the last question from the modal
    if (numQuestions > 0) {
        $('#modal-questions-container').children().last().remove();
    }
}

function confirmQuizUpdate() {
    if (confirm("Are you sure you want to update this quiz?")) {
        // TODO: submit the form using to server
        console.log('Quiz updated');
        // dismiss the modal
        $('#edit-quiz-modal').modal('hide');
    }
}

// delete a quiz
function removeQuiz(button) {
    if (confirm("Are you sure you want to delete this quiz?")) {
        // TODO send the delete request to the server

        // if response == OK: remove the quiz from the page
        var serverResponse = "OK";
        if (serverResponse == "OK") {
            var row = button.parentNode.parentNode; // Get the parent row
            row.remove();
        }
        // else: show an error message
        else {alert("Error deleting quiz");} // TODO: show an error message
        
    }
}

// adding a subject to the database
$('#add-subject-form').submit(function (e) {
    // get the form data
    var formData = $(this).serializeArray();

    // convert form data to JSON
    const jsonData = {};
    for (let i = 0; i < formData.length; i++) {
        jsonData[formData[i].name] = formData[i].value;
    }

    // Log JSON data
    console.log(JSON.stringify(jsonData));

    e.preventDefault(); // Prevent the form from submitting for now
    // TODO send the form data to the server
});