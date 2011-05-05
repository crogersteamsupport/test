function GetQueryParamValue(name, url) {
  params = url.search.substring(1);
  name = name.toLowerCase();
  param = params.split("&");
  for (i = 0; i < param.length; i++) {
    value = param[i].split("=");
    if (value[0].toLowerCase() == name) {
      var result = value[1];
      result = result.replace('+', '%20').replace(' ', '%20');
      return this.unescape(result);
    }
  }
}


function OpenAttachment(attachmentID) {
    window.open('../Attachment.aspx?attachmentID=' + attachmentID, 'Attachment' + attachmentID);
}


