$(document).on('click', '.action-hydrate-deflector', function(e) {
    e.stopPropagation();
    e.preventDefault();
    window.parent.Ts.Services.Deflector.HydrateOrganization(1078, function(result) {
    });
});
