
$(function () {
    /*
    $("#sortable").sortable();
    $("#sortable").disableSelection();
    */
    $("#sortable_table").sortable();
    $("#sortable_table").disableSelection();
});

$("#sortable_table").sortable({
    axis: 'y',
    // This event is triggered when the user stopped sorting and the DOM position has changed.
    update: function (event, ui) {
        var order = 1,
            model = [];

        $('#sortable_table tr').each(function () {
            /*
             * Building a new object and pushing in modal array.
             * Here I am setting memberOrder property which is I am using in my db and building my object
             */
            // This is for example to build your object and push in a modal array.
            var objModel = { MemberID: 1, MemberOrder: order };
            model.push(objModel);
            order++;
        });

        if (model.length > 1) {
            $.ajax({
                type: "POST",
                contentType: "application/json; charset=utf-8",
                // This is my url put your url here and pass model as data it is in array of my items.
                url: './members/updateStatusOrder',
                data: JSON.stringify({ model: model }),
                success: function (data) {
                    // Do something
                },
                error: function (e) {
                    // Do something
                }
            });
        }





        /*
        var memberId = ui.item[0].getAttribute('memberId'),
            memberOrder = ui.item[0].getAttribute('memberOrder');

        console.log('sortable UPDATE :: Id=' + memberId + '; Order=' + memberOrder);
        */
    }
});

