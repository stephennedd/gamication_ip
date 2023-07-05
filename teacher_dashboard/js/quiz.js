let subjects;
let chosenSubject;
let numberOfExistingQuestions;

const editQuizLink = document.getElementById('edit-quiz-link');

// Get the form element
const form = document.getElementById('add-subject-form');

// Add submit event listener to the form
form.addEventListener('submit', (e) => {
	e.preventDefault(); // Prevent form submission

	// Get the subject name and week values
	const subjectName = document.getElementById('subject-name').value;
	const subjectWeek = document.getElementById('subject-week').value;
	const gameId = document.getElementById('subject-game').value;

	// Create an API request object
	const apiRequest = {
		subjectTitle: subjectName,
		weekNumber: subjectWeek,
		gameId: gameId,
	};

	console.log(apiRequest);

	fetch('http://localhost:4434/api/subjects', {
		method: 'POST',
		headers: {
			'Content-Type': 'application/json',
		},
		body: JSON.stringify(apiRequest),
	})
		.then((response) => {
			if (response.ok) {
				console.log('API request successful');
				$('#add-subject-success-modal').modal('show'); // Show success modal
				// redirect to the student dashboard
			} else {
				console.log('API request failed');
				// Handle the error case
			}
		})
		.catch((error) => {
			console.log('API request failed:', error);
			// Handle any network or other errors
		});
});

editQuizLink.addEventListener('click', async function (event) {
	event.preventDefault();

	try {
		const response = await fetch('http://localhost:4434/api/subjects');
		const data = await response.json();
		subjects = data;
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

// delete a quiz from the database and remove it from the page
async function removeQuiz(button, subject) {
	if (confirm('Are you sure you want to delete this quiz?')) {
		// TODO send the delete request to the server

		console.log(subject.Id);
		await fetch(`http://localhost:4434/api/subjects/?id=${subject.Id}`, {
			method: 'DELETE',
		})
			.then((response) => {
				if (!response.ok) {
					throw new Error('Failed to delete subject');
				}
				return response.json();
			})
			.then((data) => {
				// Handle the successful response
				console.log('Subject deleted successfully');
				// Perform any necessary actions or update the UI accordingly
			})
			.catch((error) => {
				// Handle errors
				console.error('Error deleting subject:', error.message);
			});

		const response = await fetch('http://localhost:4434/api/subjects');
		const data = await response.json();
		subjectsToBeDeleted = data;
		console.log(subjectsToBeDeleted);
		localStorage.setItem(
			'subjectsDataForRemovePage',
			JSON.stringify(subjectsToBeDeleted)
		);
		populateTable2(subjectsToBeDeleted);

		// if response == OK: remove the quiz from the page
		var serverResponse = 'OK';
		if (serverResponse == 'OK') {
			var row = button.parentNode.parentNode; // Get the parent row
			row.remove();
		}
		// else: show an error message
		else {
			alert('Error deleting quiz');
		} // TODO: show an error message
	}
}

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
		td1.id = 'quiz-title';
		tr.appendChild(td1);

		const td2 = document.createElement('td');
		td2.textContent = subject.WeekNumber;
		tr.appendChild(td2);

		const td3 = document.createElement('td');
		td3.textContent = subject.SubjectTitle;
		tr.appendChild(td3);

		const td4 = document.createElement('td');
		td4.className = 'text-center';
		td4.textContent = subject.Test.Questions.length;
		td4.id = 'quiz-question-count';
		tr.appendChild(td4);

		const td5 = document.createElement('td');
		const button = document.createElement('button');
		button.type = 'button';
		button.className = 'btn btn-sm btn-outline-success';
		button.textContent = 'Edit';
		button.id = 'edit-quiz-button';
		button.addEventListener('click', function () {
			chosenSubject = subject;
			numberOfExistingQuestions = subject.Test.Questions.length;
			editQuiz(this, subjects, subject);
		});
		td5.appendChild(button);
		tr.appendChild(td5);

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
                <input class="form-control mx-3 mb-1 ${
									colors[0]
								}" type="text" name="answer[${questionId}][]" placeholder="Correct answer" required>
                <input class="form-control mx-3 mb-1 ${
									colors[1]
								}" type="text" name="answer[${questionId}][]" placeholder="Wrong answer" required>
                <input class="form-control mx-3 mb-1 ${
									colors[1]
								}" type="text" name="answer[${questionId}][]" placeholder="Wrong answer" required>
                <input class="form-control mx-3 ${
									colors[1]
								}" type="text" name="answer[${questionId}][]" placeholder="Wrong answer" required>
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

	// Event handler for the create-quiz form submission
	$('#quiz-form').submit(function (e) {
		//get the form data
		var formData = $(this).serializeArray();
		// Convert form data to JSON
		const jsonData = {};
		$(formData).each(function (index, field) {
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

// Event handler for the "Remove Question" button click for the create quiz page
function removeQuestion(button) {
	// Get the question container
	var row = button.parentNode.parentNode;
	const subjectId = row.dataset.quizId;
	console.log(subjectId);
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
function editQuiz(button, subjects, subject) {
	// var exampleQuestions = [
	//     {questionText: "What is the capital of France?", correctAnswer: "Paris" ,answers: ["Paris", "London", "Berlin", "Madrid"]},
	//     {questionText: "What is the capital of Spain?", correctAnswer: "Madrid" ,answers: ["Madrid", "London", "Berlin", "Paris"]},
	//     {questionText: "What is the capital of Germany?", correctAnswer: "Berlin" ,answers: ["Berlin", "London", "Paris", "Madrid"]},
	// ];

	var row = button.parentNode.parentNode;
	const subjectId = row.dataset.quizId;
	// Find the subject by its ID
	var chosenSubject = subjects.find(function (subject) {
		return subject.Id == subjectId;
	});

	var subjectQuestions = chosenSubject.Test.Questions;

	for (var i = 0; i < subjectQuestions.length; i++) {
		var question = subjectQuestions[i];
		// Sort the answers array by checking if the AnswerText is equal to the CorrectAnswer
		question.Answers.sort(function (a, b) {
			if (a.AnswerText === question.CorrectAnswer) {
				return -1; // a should come before b
			} else if (b.AnswerText === question.CorrectAnswer) {
				return 1; // b should come before a
			} else {
				return 0; // the order doesn't matter
			}
		});
	}

	// Get the quiz ID from the row using the data attribute
	var row = button.parentNode.parentNode; // Get the parent row
	const quizId = row.dataset.quizId;
	var quizName = row.dataset.quizName;

	// open the modal and show the quiz name
	$('#edit-quiz-modal').modal('show');
	$('#edit-quiz-modal-header').text(`Edit Quiz ${quizName}`);
	$('#modal-quiz-name').val(quizName);

	// document.getElementById('modal-quiz-subject').value = "methods_1";
	//document.getElementById('modal-quiz-week').value = chosenSubject.WeekNumber;

	// load questions into the modal
	var questionsContainer = $('#modal-questions-container');
	questionsContainer.empty();

	// Loop through the example questions and add them to the modal
	for (var i = 0; i < subjectQuestions.length; i++) {
		var question = subjectQuestions[i];
		var questionHTML = `
            <div class="question mb-3">
            <div class="col-md-12">
                <div class="row align-items-center">
                    <div class="col me-2">
                        <label for="question[${i}]" class="form-label">Question ${
			i + 1
		}</label>
                    </div>
                    <div class="col mb-2 text-end">
                        <button id="remove-question" type="button" class="btn btn-danger btn-sm" onclick="removeEditModalQuestion(this,'${quizId}','${
			subjectQuestions[i].Id
		}')">Remove</button>
                    </div>
                </div>
            </div>
  
            <input type="text" name="question[${i}]" class="form-control" placeholder="Enter a question" required value="${
			question.QuestionText
		}">
            <div class="mt-2 mb-3 me-3">
                <input class="form-control mx-3 mb-1 correct" type="text" name="answer[${i}][]" placeholder="Correct answer" required value="${
			question.Answers[0].AnswerText
		}">
                <input class="form-control mx-3 mb-1 wrong" type="text" name="answer[${i}][]" placeholder="Wrong answer" required value="${
			question.Answers[1].AnswerText
		}">
                <input class="form-control mx-3 mb-1 wrong" type="text" name="answer[${i}][]" placeholder="Wrong answer" required value="${
			question.Answers[2].AnswerText
		}">
                <input class="form-control mx-3 wrong" type="text" name="answer[${i}][]" placeholder="Wrong answer" required value="${
			question.Answers[3].AnswerText
		}">
            </div>
            </div>
        `;
		// add the question to the modal
		questionsContainer.append(questionHTML);
	}
}

// add a question to the edit quiz modal
function addQuestionToModal(subjects) {
	var numQuestions = $('#modal-questions-container').children().length;

	// add a new question to the modal
	var questionHTML = `
        <div class="question mb-3">
        <div class="col-md-12">
                <div class="row align-items-center">
                    <div class="col me-2">
                        <label for="question[${
													numQuestions + 1
												}]" class="form-label">Question ${
		numQuestions + 1
	}</label>
                    </div>
                    <div class="col mb-2 text-end">
                        <button id="remove-question" type="button" class="btn btn-danger btn-sm" onclick="removeEditModalQuestion(this)">Remove</button>
                    </div>
                </div>
            </div>

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

// remove a question from the edit-quiz modal
async function removeEditModalQuestion(button, quizId, questionId) {
	for (let i = 0; i < chosenSubject.Test.Questions.length; i++) {
		if (chosenSubject.Test.Questions[i].Id == questionId) {
			chosenSubject.Test.Questions.splice(i, 1);
			break;
		}
	}
	// Send chosenSubject via API using fetch
	await fetch('http://localhost:4434/api/subjects', {
		method: 'PUT',
		headers: {
			'Content-Type': 'application/json',
		},
		body: JSON.stringify(chosenSubject),
	})
		.then((response) => {
			if (response.ok) {
				console.log('Quiz updated successfully');
				// dismiss the modal
				// $('#edit-quiz-modal').modal('hide');
			} else {
				throw new Error('Failed to update quiz');
			}
		})
		.catch((error) => {
			console.error(error);
		});

	// TODO: submit the form using to server
	// console.log('Quiz updated');
	// dismiss the modal
	//$('#edit-quiz-modal').modal('hide');

	// Get the question container
	const questionContainer = button.parentNode.parentNode.parentNode.parentNode;
	// Remove the question container
	questionContainer.remove();

	// renumber the questions
	var questions = document.getElementsByClassName('question');
	for (var i = 0; i < questions.length; i++) {
		var question = questions[i];
		question.querySelector('label').innerText = `Question ${i + 1}`;
	}

	try {
		const response = await fetch('http://localhost:4434/api/subjects');
		const data = await response.json();
		subjects = data;
		// Store the subjects data in localStorage
		localStorage.setItem('subjectsData', JSON.stringify(subjects));
	} catch (error) {
		console.error(error);
	}

	populateTable(subjects);
}

// submit the edit-quiz form
$('#edit-quiz-form').submit(function (e) {
	// get the form data
	var formData = $(this).serializeArray();

	// convert form data to JSON
	const jsonData = {};
	for (let i = 0; i < formData.length; i++) {
		jsonData[formData[i].name] = formData[i].value;

		console.log(jsonData);

		e.preventDefault();

		// close the modal
		$('#edit-quiz-modal').modal('hide');
	}
});

async function confirmQuizUpdate(button, subjects) {
	if (confirm('Are you sure you want to update this quiz?')) {
		// Get the questions and answers entered by the userchosenSubject
		var questions = $('#modal-questions-container').find('.question');

		// Iterate over each question and extract the values
		questions.each(function (index) {
			var questionText = $(this).find('input[name^="question"]').val();
			var correctAnswer = $(this).find('input[name^="answer"]:first').val();
			var secondAnswer = $(this).find('input[name^="answer"]:eq(1)').val();
			var thirdAnswer = $(this).find('input[name^="answer"]:eq(2)').val();
			var fourthAnswer = $(this).find('input[name^="answer"]:eq(3)').val();

			if (numberOfExistingQuestions >= index + 1) {
				chosenSubject.Test.Questions[index].QuestionText = questionText;
				chosenSubject.Test.Questions[index].CorrectAnswer = correctAnswer;
				chosenSubject.Test.Questions[index].Answers[0].AnswerText =
					correctAnswer;
				chosenSubject.Test.Questions[index].Answers[1].AnswerText =
					secondAnswer;
				chosenSubject.Test.Questions[index].Answers[2].AnswerText = thirdAnswer;
				chosenSubject.Test.Questions[index].Answers[3].AnswerText =
					fourthAnswer;
			} else {
				chosenSubject.Test.Questions.push({
					Answers: [
						{ Identifier: 'A', AnswerText: correctAnswer },
						{ Identifier: 'B', AnswerText: secondAnswer },
						{ Identifier: 'C', AnswerText: thirdAnswer },
						{ Identifier: 'D', AnswerText: fourthAnswer },
					],
					CorrectAnswer: correctAnswer,
					QuestionText: questionText,
					SelectedAnswer: '',
					TestId: chosenSubject.Test.Id,
				});
			}
		});

		//chosenSubject.WeekNumber = document.getElementById('modal-quiz-week').value;
		chosenSubject.Test.Title = document.getElementById('modal-quiz-name').value;
		//console.log(document.getElementById('modal-quiz-week').value);
		// Send chosenSubject via API using fetch
		await fetch('http://localhost:4434/api/subjects', {
			method: 'PUT',
			headers: {
				'Content-Type': 'application/json',
			},
			body: JSON.stringify(chosenSubject),
		})
			.then((response) => {
				if (response.ok) {
					console.log('Quiz updated successfully');
					// dismiss the modal
					$('#edit-quiz-modal').modal('hide');
				} else {
					throw new Error('Failed to update quiz');
				}
			})
			.catch((error) => {
				console.error(error);
			});

		try {
			const response = await fetch('http://localhost:4434/api/subjects');
			const data = await response.json();
			subjects = data;
			// Store the subjects data in localStorage
			localStorage.setItem('subjectsData', JSON.stringify(subjects));
		} catch (error) {
			console.error(error);
		}

		populateTable(subjects);

		// TODO: submit the form using to server
		console.log('Quiz updated');
		// dismiss the modal
		$('#edit-quiz-modal').modal('hide');
	}
}
