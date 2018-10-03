$(document).on('click', '.action-hydrate-deflector', function(e) {
    e.stopPropagation();
    e.preventDefault();
    window.parent.Ts.Services.Deflector.HydrateOrganization(selectedOrgId, function(result) {
        console.log(result);
    });
});

function GetDeflectionResults(organizationID, phrase) {
    debugger;
    window.parent.Ts.Services.Deflector.GetDeflections(organizationID, phrase, function (result) {
        console.log(result);
    });
}


