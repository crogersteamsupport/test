<%@ Page Title="" Language="C#" MasterPageFile="~/Frames/Frame.master" AutoEventWireup="true" CodeFile="VersionInformation.aspx.cs" Inherits="Frames_VersionInformation" %>
<%@ Register assembly="Telerik.Web.UI" namespace="Telerik.Web.UI" tagprefix="telerik" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" Runat="Server">
  <link href="../Resources_144/Css/jquery.fileupload-ui.css" rel="stylesheet" type="text/css" />

  <script src="../Resources_144/Js/jquery.fileupload.js" type="text/javascript"></script>

  <script src="../Resources_144/Js/jquery.fileupload-ui.js" type="text/javascript"></script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" Runat="Server">
<div style="width:100%; height:100%; background-color:#ffffff; overflow:auto;">
<div style="width:95%; margin:0 auto; padding-bottom: 20px;">
      <div id="pnlContent" runat="server">
        <div class="groupDiv groupLightBlue" style="padding-top: 10px;">
          <div class="groupHeaderDiv">
            <span class="groupHeaderSpan"></span>
            <span class="groupCaptionSpan">Version Properties</span>
          </div>
          <div class="groupBodyWrapperDiv">
            <div class="groupBodyDiv">
              <div id="pnlProperties" runat="server" class="adminDiv" style="padding: 5px 5px 5px 5px;">
                <asp:Label ID="lblProperties" runat="server" Text="There are no properties to display."></asp:Label>
                <asp:Repeater ID="rptProperties" runat="server">
                  <ItemTemplate>
                    <div style="margin: 5px 5px 5px 15px; line-height: 20px;">
                      <span style="font-weight: bold;">
                        <%# DataBinder.Eval(Container.DataItem, "Name")%></span>
                      <span>
                        <%# DataBinder.Eval(Container.DataItem, "Value")%>
                        <br />
                      </span>
                    </div>
                  </ItemTemplate>
                </asp:Repeater>
              </div>
            </div>
          </div>
        </div>
        <div class="groupDiv groupLightBlue" style="padding-top: 10px;">
          <div class="groupHeaderDiv">
            <span class="groupHeaderSpan"></span>
            <span class="groupCaptionSpan">Attachments</span>
          </div>
          <div class="groupBodyWrapperDiv">
            <div class="groupBodyDiv">
            </div>
          </div>
        </div>
      </div>
    </div>
</div>

<form id="file_upload" action="upload.php" method="POST" enctype="multipart/form-data">
    <input type="file" name="file" multiple>
    <button>Upload</button>
    <div class="js">Upload files</div>
</form>
</asp:Content>

