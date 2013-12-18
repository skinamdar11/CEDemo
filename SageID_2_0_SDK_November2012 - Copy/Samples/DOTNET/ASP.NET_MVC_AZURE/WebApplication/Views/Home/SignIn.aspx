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
                <tr>
                    <td width="10%" align="left"></td>
                    <td width="80%" align="center"><span style="font-weight:bold; font-size:small;"><%= ConfigurationManager.AppSettings["ApplicationName"]%></span></td>
                    <td width="10%" align="right">Not signed in</td>
                </tr>
            </table>
        </div>
        <form id="form2" runat="server">
            <table cellspacing="8px">
                <tr>
                    <td width="20%"><%= Html.ActionLink("Sign In", "SignOnInit", "SSO") %></td>
                    <td>Sign into this application using Sage SSO.</td>
                </tr>
                <tr>
                    <td width="20%"><%= Html.ActionLink("Sign Up", "SignUp", "Home") %></td>
                    <td>Register a new or existing Sage SSO user for this application.</td>
                </tr>
            </table>
        </form>
        <div id="push" class="push"></div>
    </div>
    <div id="footer" class="footer">
        <div id="footerMessage" class="footerMessage"><%= ViewData["Message"] %></div>
    </div>
</body>
</html>