// Quiz creation form
$(document).ready(function () {
    // Counter to keep track of the number of questions
    let questionCounter = 0;

    // Function to generate the HTML for a question and its answers
    function generateQuestionHTML(questionId) {
        const colors = ['correct', 'wrong'];
        return `
            <div class="question mb-3">
            <label for="question[${questionId}]" class="form-label">Question ${questionId + 1}</label>
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
      const questionHTML = generateQuestionHTML(questionCounter);
      questionCounter++;
      $('#questions-container').append(questionHTML);
    });

    // Event handler for the "Remove Question" button click
    $('#remove-question-btn').click(function () {
        if (questionCounter > 0) {
            $('#questions-container').children().last().remove();
            questionCounter--;
        }
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

// update a quiz
function editQuiz(button) {
    var exampleQuestions = [
        {questionText: "What is the capital of France?", correctAnswer: "Paris" ,answers: ["Paris", "London", "Berlin", "Madrid"]},
        {questionText: "What is the capital of Spain?", correctAnswer: "Madrid" ,answers: ["Madrid", "London", "Berlin", "Paris"]},
        {questionText: "What is the capital of Germany?", correctAnswer: "Berlin" ,answers: ["Berlin", "London", "Paris", "Madrid"]},
    ];

    // Get the quiz ID from the row using the data attribute
    var row = button.closest("tr"); // Get the closest parent <tr> element

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
        // TODO: submit the form using Ajax
        console.log('Quiz updated');
        // dismiss the modal
        $('#edit-quiz-modal').modal('hide');
    }
}