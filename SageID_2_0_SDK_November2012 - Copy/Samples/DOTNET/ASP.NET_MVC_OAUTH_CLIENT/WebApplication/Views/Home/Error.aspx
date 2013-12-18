<%@ Page Language="C#" Inherits="System.Web.Mvc.ViewPage<dynamic>" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head id="Head1" runat="server">
    <title><%= ConfigurationManager.AppSettings["ApplicationName"]%> - Sign In</title>
    <link href="<%= Url.Content("~/Content/styles.css") %>" type="text/css" rel="stylesheet" />
</head>
<body>
    <div id="content" class="content">
        <div id="header" class="header">
            <table cellpadding="0" cellspacing="0" width="100%">
                <tr><td>&nbsp;</td></tr>
            </table>
        </div>
        <form id="form1" runat="server">
            <p><%= Html.ActionLink("Home", "Index", "Home")  %></p>
            <p>Error code: <%= ViewData["Error"]%></p>
            <p>Error description: <%= ViewData["ErrorDescription"]%></p>
        </form>
        <div id="push" class="push"></div>
    </div>
    <div id="footer" class="footer">
        <div id="footerMessage" class="footerMessage"></div>
    </div>
</body>
</html>