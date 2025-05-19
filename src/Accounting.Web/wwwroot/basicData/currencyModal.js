$(function () {
    $('#currencyForm').on('abp-ajax-success', function () {
        $('#currencyForm').slideUp();
        var l = abp.localization.getResource('Accounting');
        abp.notify.success(l('SavedSuccessfully'));
    });
    $('#currencyForm').on('change', '[name="Currency.SourceAmount"],[name="Currency.TargetAmount"]', function () {
        var source = $('[name="Currency.SourceAmount"]').val();
        var target = $('[name="Currency.TargetAmount"]').val();
        if (target) {
            var sourceAmount = parseFloat(source);
            var targetAmount = parseFloat(target);
            if (targetAmount && !isNaN(sourceAmount) && !isNaN(targetAmount)) {
                var rate = sourceAmount / targetAmount;
                var amountRender = DataTable.render.number(null, null, 7, '', '').display;
                $('[name="Currency.ExchangeRate"]').val(amountRender(rate));
            }
        }
    });
    var isEdit = $('#currencyForm').attr('data-edit');
    if (Boolean(isEdit)) {
        $('[name="Currency.SourceCurrency"],[name="Currency.TargetCurrency"]').attr('disabled', 'disabled');
    }
});