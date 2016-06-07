$(document).ready(function () {
	var encrypted = CryptoJS.AES.encrypt(Date.now() + "," + parent.parent.Ts.System.User.Email + "", parent.parent.Ts.System.Organization.PortalGuid);

	parent.parent.Ts.Services.Admin.GetHubURL(function (url) {
		$('#hub_admin').attr('src', "https://" + url + "/#/sso/" + encrypted)
	});
});

