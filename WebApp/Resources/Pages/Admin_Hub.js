$(document).ready(function () {
	var encrypted = CryptoJS.AES.encrypt(Date.now() + "," + parent.Ts.System.User.Email + "", parent.Ts.System.Organization.PortalGuid);

	parent.Ts.Services.Admin.GetHubURL(function (url) {
		$('#hub_admin').attr('src', "https://" + url + "/#/sso/" + encrypted)
	});
});

