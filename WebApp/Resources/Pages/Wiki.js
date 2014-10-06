var wikiPage = null;

$(document).ready(function () {
    wikiPage = new WikiPage();
});

function onShow() {
  // this fires everytime the main tab is selected
    wikiPage.refresh();
};

WikiPage = function () {
    this.refresh = function () {
    }

    function getWiki(wikiID) {
        top.Ts.Services.Wiki.GetWiki(wikiID, function (wiki) { 
          //if not null, this is my wiki article object.
        
        });

    }
}
