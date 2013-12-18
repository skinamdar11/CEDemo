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
                    <td width="40%" align="left"><%= ViewData["SessionId"] %><br /><%= ViewData["SessionExpiry"] %></td>
                    <td width="20%" align="center"><span style="font-weight:bold; font-size:small;"><%= ConfigurationManager.AppSettings["ApplicationName"]%></span></td>
                    <td width="40%" align="right"><%= ViewData["UserInfo"] %></td>
                </tr>
            </table>
        </div>
        <form id="form1" runat="server">
            <table cellspacing="8px">
                <tr>
                    <td width="20%"><%= Html.ActionLink("Refresh Page", "App", "Home") %></td>
                    <td>Simulate activity in the web application, causing it to update the recorded last activity time and to extend the session if expiry is due.</td>
                </tr>
                <tr>
                    <td width="20%"><%= Html.ActionLink("Change Password", "ChangePassword", "SSO") %></td>
                    <td>Change the signed-on user's password and security questions.</td>
                </tr>
                <tr>
                    <td width="20%"><%= Html.ActionLink("Manage Authorisations", "ManageAuthorisations", "SSO") %></td>
                    <td>Manage the signed in users authorisations for clients of this web application.</td>
                </tr>
                <tr>
                    <td width="20%"><%= Html.ActionLink("Sign Out", "SignOut", "SSO") %></td>
                    <td>Sign out of this application only. This application will leave the SSO session but the SSO session will continue if there is more than one participant application.</td>
                </tr>
                <tr>
                    <td width="20%"><%= Html.ActionLink("Sign Out All", "SignOutAll", "SSO") %></td>
                    <td>End the SSO session. A Session.Ended notification will be sent to all participant applications.</td>
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