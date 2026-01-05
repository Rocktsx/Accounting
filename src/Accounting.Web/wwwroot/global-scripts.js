/* Your Global Scripts */
function getSelect2Language() {
    const languageMap = { 'zh-Hans': 'zh-CN', 'zh-Hant': 'zh-TW' }
    const cultureName = abp.localization.currentCulture.cultureName;
    return languageMap[cultureName] || 'en';
}
function formatDate(value) {
    let format = abp.localization.currentCulture.dateTimeFormat.shortDatePattern;
    format = format.replaceAll('d', 'D');
    return (new moment(new Date(value))).format(format)
}

function renderAmount(amount, scale) {
    const num = Number(amount);
    if (Number.isNaN(num)) {
        return '0.00';
    }
    const dot = '.';
    const arr = num.toFixed(scale ? scale : 2).split(dot);

    return arr[0].replace(/\B(?=(\d{3})+(?!\d))/g, ",") + dot + arr[1];
}

$(function () {
    $.each($('.lpx-menu-item .lpx-menu-item-text'), function (idx, item) {
        const $this = $(item);
        $this.attr('title', $this.text());
    });
});