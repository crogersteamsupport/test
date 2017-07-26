$(document).ready(function(){
    $('.dropdown-submenu a.expand-submenu').on("click", function(e){
        $(this).next('ul').toggle();
        e.stopPropagation();
        e.preventDefault();
    }); 
});
