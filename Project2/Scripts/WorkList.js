function dropDownChange() {
    $.ajax({
        url: '/WorkList/Index',
        type: 'GET',
        dataType: 'text',
        data: { month: $('#dropdown').val() },
        success: function (result) {
            $('#result').html(result);
        },
        error: function () {
            alert($('#dropdown').val().error);
        }
    });
}