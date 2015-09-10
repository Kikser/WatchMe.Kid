$(document).ready(function () {
    $(".icon-bg").click(function (e) {
        ShowDialog(false);
        e.preventDefault();
    });

    $("#btnClose").click(function (e) {
        HideDialog();
        e.preventDefault();
    });

    //$("#btnSubmit").click(function (e) {
    //    var brand = $("#brands input:radio:checked").val();
    //    $("#output").html("<b>Your favorite mobile brand: </b>" + brand);
    //    HideDialog();
    //    e.preventDefault();
    //});

});

function ShowDialog(modal) {
    $("#overlay").show();
    $("#dialog").fadeIn(700);

    if (modal) {
        $("#overlay").unbind("click");
    }
    else {
        $("#overlay").click(function (e) {
            HideDialog();
        });
    }
}

function HideDialog() {
    $("#overlay").hide();
    $("#dialog").fadeOut(700);
}
