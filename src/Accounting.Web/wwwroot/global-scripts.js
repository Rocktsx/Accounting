/* Your Global Scripts */
function getSelect2Language() {
    const languageMap = { 'zh-Hans': 'zh-CN', 'zh-Hant': 'zh-TW' }
    const cultureName = abp.localization.currentCulture.cultureName;
    return languageMap[cultureName] || 'en';
}