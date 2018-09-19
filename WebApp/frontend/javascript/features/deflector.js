$(document).on('click', '.action-hydrate-deflector', function(e) {
    e.stopPropagation();
    e.preventDefault();

    window.parent.Ts.Services.Deflector.Testing('tagname', function(result) {
        alert(result);
    });
});
