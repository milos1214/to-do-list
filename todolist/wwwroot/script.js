const apiUrl = "http://localhost:5139/api/todo";

function loadTasks() {
    $.get(apiUrl, function (tasks) {
        $('#list').empty();
        tasks.forEach(task => {
            $('#list').append(`<li>${task} <button class="delete" data-task="${task}">x</button></li>`);
        });
    })
}

$('#add-button').click(function () {
    const toDo = $('#input-field').val().trim();
    if (toDo) {
        $.ajax({
            url: apiUrl,
            type: 'POST',
            contentType: 'application/json',
            data: JSON.stringify(toDo),
            success: function() {
                loadTasks();
                $('#input-field').val('');
            }
        })
    }
})


$(document).on('click', '.delete', function () {
    const taskDelete = $(this).data('task');
    $.ajax({
        url: apiUrl,
        type: 'DELETE',
        contentType: 'application/json',
        data: JSON.stringify(taskDelete),
        success: function () {
            loadTasks();
        }
    })
})

$(document).ready(function () {
    loadTasks();
})