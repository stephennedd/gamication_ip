const editQuizLink = document.getElementById('edit-quiz-link');

editQuizLink.addEventListener('click', async function (event) {
  event.preventDefault();

  try {
    const response = await fetch('https://localhost:7186/api/subjects');
    const data = await response.json();
    const subjects = data;
    console.log(subjects);

    // Store the subjects data in localStorage
    localStorage.setItem('subjectsData', JSON.stringify(subjects));

    populateTable(subjects);

    // Perform any further actions based on the response
    window.location.href = '#edit-quiz';
  } catch (error) {
    console.error(error);
  }
});

// Check if subjects data exists in localStorage on page load
window.addEventListener('DOMContentLoaded', function () {
  const storedSubjects = localStorage.getItem('subjectsData');

  if (storedSubjects) {
    const subjects = JSON.parse(storedSubjects);
    populateTable(subjects);
  }
});

function populateTable(subjects) {
    // Assuming you have the `subjects` array available
  
    // Get the table body element
    const quizTableBody = document.getElementById('quiz-table-body');
  
    // Function to create a table row based on a subject object
    function createTableRow(subject) {
      const tr = document.createElement('tr');
      tr.setAttribute('data-quiz-id', subject.Id);
      tr.setAttribute('data-quiz-name', subject.Test.Title);
  
      const td1 = document.createElement('td');
      td1.textContent = subject.Test.Title;
      tr.appendChild(td1);
  
      const td2 = document.createElement('td');
      td2.textContent = subject.SubjectTitle;
      tr.appendChild(td2);
  
      const td3 = document.createElement('td');
      td3.className = 'text-center';
      td3.textContent = subject.Test.Questions.length;
      tr.appendChild(td3);
  
      const td4 = document.createElement('td');
      const button = document.createElement('button');
      button.type = 'button';
      button.className = 'btn-sm btn-outline-success';
      button.textContent = 'Edit';
      button.addEventListener('click', function () {
        editQuiz(this,subjects);
      });
      td4.appendChild(button);
      tr.appendChild(td4);
  
      return tr;
    }
  
    // Clear existing table rows
    quizTableBody.innerHTML = '';
  
    // Create table rows for each subject
    subjects.forEach(function (subject) {
      const tableRow = createTableRow(subject);
      quizTableBody.appendChild(tableRow);
    });
  }
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
function editQuiz(button,subjects) {
    var row = button.parentNode.parentNode;
    const subjectId = row.dataset.quizId;
   // Find the subject by its ID
   var chosenSubject = subjects.find(function (subject) {
    return subject.Id == subjectId;
  });

    var subjectQuestions = chosenSubject.Test.Questions;

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
    for (var i = 0; i < subjectQuestions.length; i++) {
        var question = subjectQuestions[i];
        // Sort the answers array by checking if the AnswerText is equal to the CorrectAnswer
        question.Answers.sort(function(a, b) {
         if (a.AnswerText === question.CorrectAnswer) {
          return -1; // a should come before b
          } else if (b.AnswerText === question.CorrectAnswer) {
           return 1; // b should come before a
          } else {
          return 0; // the order doesn't matter
        }

    
  });

        var questionHTML = `
            <div class="question mb-3">
            <label type="text" class="form-label">Question ${i+1}</label>
            <input type="text" name="question[${i}]" class="form-control" placeholder="Enter a question" required value="${question.QuestionText}">
            <div class="mt-2 mb-3 me-3">
                <input class="form-control mx-3 mb-1 correct" type="text" name="answer[${i}][]" placeholder="Correct answer" required value="${question.Answers[0].AnswerText}">
                <input class="form-control mx-3 mb-1 wrong" type="text" name="answer[${i}][]" placeholder="Wrong answer" required value="${question.Answers[1].AnswerText}">
                <input class="form-control mx-3 mb-1 wrong" type="text" name="answer[${i}][]" placeholder="Wrong answer" required value="${question.Answers[2].AnswerText}">
                <input class="form-control mx-3 wrong" type="text" name="answer[${i}][]" placeholder="Wrong answer" required value="${question.Answers[3].AnswerText}">
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

