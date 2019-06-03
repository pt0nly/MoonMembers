
$(function () {
    /*
    $("#sortable").sortable();
    $("#sortable").disableSelection();
    */
    $("#sortable_table").sortable();
    $("#sortable_table").disableSelection();
});

$("#sortable_table").sortable({
    // This event is triggered when the user stopped sorting and the DOM position has changed.
    update: function (event, ui) {
        var memberId = ui.item[0].getAttribute('memberId'),
            memberOrder = ui.item[0].getAttribute('memberOrder');

        console.log('sortable UPDATE :: Id=' + memberId + '; Order=' + memberOrder);
    }
});

