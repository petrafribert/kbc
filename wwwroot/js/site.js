// Please see documentation at https://docs.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.

$(function () {
    $(document).on('click', '.delete', function (event) {
        if (!confirm("Obrisati zapis?")) {
            event.preventDefault();
        }
    })
});

function clearOldMessage() {
    $('#tempmessage').siblings().remove();
    $('#tempmessage').removeClass("alert-success");
    $('#tempmessage').removeClass("alert-danger");
    $('#tempmessage').html('');
}

function SetDeleteAjax(selector, url, paraname) {
    $(document).on('click', selector, function (event) {
        event.preventDefault();
        var paramval = $(this).data(paraname);
        var tr = $(this).parents("tr");
        var aktivan = $(tr).data("aktivan");
        if (aktivan != true) {
            $(tr).data('aktivan', true);
            if (confirm("Obrisati zapis?")) {
                var token = $('input[name=__RequestVerificationToken').first().val();
                clearOldMessage;
                $.post(url, { id: paramval, __RequestVerificationToken: token }, function (data) {
                    if (data.successful) {
                        $(tr).remove();
                    }
                    $('#tempmessage').addClass(data.successful ? "alert-success" : "alert-danger");
                    $('#tempmessage').html(data.message);

                }).fail(function (jqXHR) {
                    alert(jqXHR.status + " : " + jqXHR.responseText);
                    $(tr).dara('aktivan', false);
                })
            }
            else {
                $(tr).dara('aktivan', false);
            }
        }
    })
}
function SetEditAjax(selector, url, paraname) {
    $(document).on('click', selector, function (event) {
        event.preventDefault();
        var paramval = $(this).data(paraname);
        var tr = $(this).parents("tr");
        var aktivan = $(tr).data("aktivan");
        if (aktivan !== true) {
            $(tr).data('aktivan', true);
            clearOldMessage;
            $.get(url, { id: paramval }, function (data) {
                tr.toggle();
                var inserted = $(data).insertAfter(tr);
                SetCancelAndSaveBehaviour(tr, inserted, url);
            }).fail(function (jqXHR) {
                alert(jqXHR.status + " : " + jqXHR.responseText);
                $(tr).dara('aktivan', false);
            })

        }
    })
}

function SetCancelAndSaveBehaviour(hiddenRow, insertedData, url) {

    insertedData.find(".cancel").click(function (event) {
        insertedData.remove();
        hiddenRow.toggle();
        $(hiddenRow).data('aktivan', false);
    })

    insertedData.find(".save").click(function (event) {
        event.preventDefault();
        var formData = new FormData();
        insertedData.find("[data-save]").each(function (index, element) {
            if ($(element).is(':checkbox')) {
                formData.append($(element).attr('name'), $(element).is(':checked'));
            }
            else if ($(element).is("input[type=files]")) {
                var files = $(element).get(0).files;
                if (files.length > 0) {
                    formData.append($(element).attr('name'), files[0]);
                }
            }
            else {
                var val = $.trim($(element).val());
                if (val !== '') {
                    formData.append($(element).attr('name'), val);
                }
            }
        })
        var token = $('input[name="__RequestVerificationToken"]').first().val();
        formData.append("__RequestVerificationToken", token);
        $.ajax({
            type: "POST",
            url: url,
            contentType: false,
            processData: false,
            data: formData,
            success: function (data, textStatus, jqXHR) {
                insertedData.remove();
                var inserted = $(data).insertAfter(hiddenRow);
                SetCancelAndSaveBehaviour(hiddenRow, inserted, url);
            },
            error: function (jqXHR) {
                if (jqXHR.status == 302) {
                    insertedData.remove();
                    $.get(jqXHR.responseText, {}, function (refreshedRow) {
                        $(hiddenRow).replaceWith(refreshedRow);
                    });
                }
                else {
                    alert(jqXHR.status + " : " + jqXHR.responseText);
                }
            }
        });
    });
}
