$(document).ready(function () {

    $('#confirm-dialog input.confirm, #confirm-dialog a.confirm').click(function (e) {
        e.preventDefault();

        // you must use a callback function to perform the "yes" action
        confirm("", function () {
            window.location.href = $('#confirm-dialog a.confirm').get(0).href;
        });
    });

    $('#culture-dialog input.confirm, #culture-dialog a.confirm').click(function (e) {
        e.preventDefault();

        // you must use a callback function to perform the "yes" action
        culture("", function () {
            window.location.href = $('#culture-dialog a.confirm').get(0).href;
        });
    });
});

function confirm(message, callback) {
    $('#confirm').modal({
        closeHTML: "",
        position: ["20%", ],
        overlayId: 'confirm-overlay',
        containerId: 'confirm-container',
        onShow: function (dialog) {
            $('.message', dialog.data[0]).append(message);

            // if the user clicks "yes"
            $('.yes', dialog.data[0]).click(function () {
                // call the callback
                if ($.isFunction(callback)) {
                    callback.apply();
                }
                // close the dialog
                $.modal.close();
            });
        }
    });
}

function culture(message, callback) {
    $('#culture').modal({
        closeHTML: "",
        position: ["5%", ],
        overlayId: 'culture-overlay',
        containerId: 'culture-container',
        onShow: function (dialog) {
            $('.message', dialog.data[0]).append(message);

            // if the user clicks "yes"
            $('.yes', dialog.data[0]).click(function () {
                // call the callback
                if ($.isFunction(callback)) {
                    callback.apply();
                }
                // close the dialog
                $.modal.close();
            });
        }
    });
}

