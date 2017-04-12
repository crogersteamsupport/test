$(document).ready(function () {

    pageLoad();

    function GetHubURL(hubRecord, callback) {
        var encrypted = CryptoJS.AES.encrypt(Date.now() + "," + parent.parent.Ts.System.User.Email + "", parent.parent.Ts.System.Organization.PortalGuid);
        callback("https://" + hubRecord.URL + "/#/sso/" + encrypted);
    };

    function ClearNewHubSettings() {
        $('#HubName').val("");
    }

    function pageLoad(callback) {
        parent.parent.Ts.Services.Admin.GetHubURL(function (hubList) {
            var enableNewHubs = false;

            if (parent.parent.Ts.System.Organization.UseProductFamilies == true && parent.parent.Ts.System.Organization.ProductType == parent.parent.Ts.ProductType.Enterprise) {
                enableNewHubs = true;
            }

            if (!callback) {
                GetHubURL(hubList[0], function (url) {
                    $('#hub_admin').attr('src', url);
                });
            }

            var template = Handlebars.compile($("#hub-list-template").html());
            data = { hubList: hubList, enableNewHubs: enableNewHubs };

            $("#HubList").html(template(data));

            $('#btnNewHub').on('click', function (e) {
                e.preventDefault();
                parent.parent.Ts.Services.Products.GetProductFamilies(function (data) {

                    var productLineTemplate = Handlebars.compile($("#ProductLineTemplate").html());
                    data = { ProductLines: data };

                    $("#ProductLineList").html(productLineTemplate(data));

                    $('#hub_admin').fadeOut();
                    $('#newHub').delay(400).fadeIn();
                });

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
                            $('#hub_admin').delay(400).fadeIn();
                        });
                    }
                }
            });

            $('.btnDeleteHub').on('click', function (e) {
                debugger;
                e.preventDefault();
                var HubID = $(this).data('hubid');

                if (confirm('Are you sure you want to delete this hub? All customized hub page data will be lost.')) {
                    parent.parent.Ts.Services.Admin.DeleteHub(HubID, function () {
                        pageLoad();
                    });
                } else {
                    // Do nothing!
                }

            });

        });

        if (callback) {
            callback();
        }
    }
    $('#btnFinalizeNewHub').on('click', function (e) {
        parent.parent.Ts.Services.Admin.CreateNewHub($('#HubName').val(), $('#ProductLineList').val(), function (data) {
            pageLoad(function () {
                GetHubURL(data, function (url) {
                    $('#hub_admin').attr('src', url);
                    $('#newHub').fadeOut();
                    $('#disabledNewHub').fadeOut();
                    $('#hub_admin').delay(400).fadeIn();
                    ClearNewHubSettings();
                })
            });
        });
    });

});

