var importPage = null;
$(document).ready(function () {
    importPage = new ImportPage();
    
});

function onShow() {
    importPage.refresh();
};

ImportPage = function () {
    $('.action-new').click(function (e) {
        e.preventDefault();
        $('.import-section').addClass('hidden');
        $('#import-new').removeClass('hidden');
    });
}

