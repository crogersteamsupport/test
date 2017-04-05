$(document).ready(function () {

    function GetHubURL(hubRecord, callback) {
        var encrypted = CryptoJS.AES.encrypt(Date.now() + "," + parent.parent.Ts.System.User.Email + "", parent.parent.Ts.System.Organization.PortalGuid);
        callback("https://" + hubRecord.URL + "/#/sso/" + encrypted);
    };

    parent.parent.Ts.Services.Admin.GetHubURL(function (hubList) {
        var enableNewHubs = false;

        if (parent.parent.Ts.System.Organization.UseProductFamilies == true && parent.parent.Ts.System.Organization.ProductType == parent.Ts.ProductType.Enterprise) {
            enableNewHubs = true;
        }

        GetHubURL(hubList[0], function (url) {
            $('#hub_admin').attr('src', url);
        });

        var template = Handlebars.compile($("#hub-list-template").html());
        data = { hubList: hubList, enableNewHubs: enableNewHubs };

        $("#HubList").html(template(data));

        $('#btnNewHub').on('click', function (e) {
            e.preventDefault();

            $('#hub_admin').fadeOut();
            $('#newHub').fadeIn();
        });

        $('#btnDisabledNewHub').on('click', function (e) {
            e.preventDefault();

            $('#hub_admin').fadeOut();
            $('#disabledNewHub').fadeIn();
        });

        $('.btnLoadHub').on('click', function (e) {
            e.preventDefault();
            var HubID = $(this).data('hubid');

            for (i = 0; i < hubList.length; i++) {
                if (hubList[i].HubID == HubID) {
                    GetHubURL(hubList[i], function (url) {
                        $('#hub_admin').attr('src', url);
                        $('#newHub').fadeOut();
                        $('#disabledNewHub').fadeOut();
                        $('#hub_admin').fadeIn();
                    });
                }
            }
        });
    });

    $('#btnFinalizeNewHub').on('click', function (e) {
        parent.parent.Ts.Services.Admin.CreateNewHub('Test', 10);
    });

});

