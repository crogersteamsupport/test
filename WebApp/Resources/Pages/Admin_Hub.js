$(document).ready(function () {
	var encrypted = CryptoJS.AES.encrypt(Date.now() + "," + top.Ts.System.User.Email + "", top.Ts.System.Organization.PortalGuid);

	top.Ts.Services.Admin.GetHubURL(function (url) {
		$('#hub_admin').attr('src', "https://" + url + "/#/sso/" + encrypted)
	});
});

