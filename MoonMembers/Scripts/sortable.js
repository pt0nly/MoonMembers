
$(function () {
    /*
    $("#sortable").sortable();
    $("#sortable").disableSelection();
    */
    $("#sortable_table").sortable();
    $("#sortable_table").disableSelection();
});

/*
var ids = [];
$('.list-item').each(function (index, value) {
    var id = $(value).prop('id');
    ids.push(id);
});
*/


$("#sortable_table").sortable({
    axis: 'y',
    // This event is triggered when the user stopped sorting and the DOM position has changed.
    update: function (event, ui) {
        var order = 1,
            model = [];

        $('#sortable_table tr').each(function (index, value) {
            /*
             * Construindo um novo objecto.
             */
            var objModel = { MemberID: parseInt(value.getAttribute('memberId')), MemberOrder: order };
            model.push(objModel);
            order++;
        });

        if (model.length > 1) {
            $.ajax({
                type: "POST",
                contentType: "application/json; charset=utf-8",
                cache: false,
                async: true,
                // This is my url put your url here and pass model as data it is in array of my items.
                url: './members/updateOrder',
                data: JSON.stringify({ model: model }),
                success: function (data) {
                    // Sucesso
                },
                error: function (e) {
                }
            });
        }
    }
});

