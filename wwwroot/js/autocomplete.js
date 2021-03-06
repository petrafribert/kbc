$(function () {

    $("[data-autocomplete]").each(function (index, element) {
        var url = $(element).data('autocomplete');
        var resultplaceholder = $(element).data('autocomplete-placeholder-name');
        if (resultplaceholder === undefined)
            resultplaceholder = url;

        $(element).change(function () {
            var dest = $("[data-autocomplete-placeholder='" + resultplaceholder + "']");
            var text = $(element).val();
            if (text.length === 0 || text !== $(dest).data('selected-label')) {
                $(dest).val('');
            }
        });

        $(element).autocomplete({
            source: window.applicationBaseUrl + "autocomplete/" + url,
            autoFocus: true,
            minLength: 1,
            select: function (event, ui) {
                $(element).val(ui.item.label);
                var dest = $("[data-autocomplete-placeholder='" + resultplaceholder + "']");
                $(dest).val(ui.item.id);
                $(dest).data('selected-label', ui.item.label);

                //za naziv tipa kontakta
                var dest_naziv = $(`[data-autocomplete-placeholder-naziv='${resultplaceholder}']`);
                if (dest_naziv !== undefined) {
                    $(dest_naziv).val(ui.item.naziv);
                }
            }
        });
    });
});