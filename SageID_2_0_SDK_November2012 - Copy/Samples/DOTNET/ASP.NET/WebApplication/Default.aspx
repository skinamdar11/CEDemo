<%@ Page Language="C#" AutoEventWireup="True" CodeBehind="Default.aspx.cs" Inherits="WebApplication._Default" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title><%= ConfigurationManager.AppSettings["ApplicationName"]%> - Home</title>
    <link href="styles.css" type="text/css" rel="stylesheet" />
</head>
<body>
    <div id="content" class="content">
        <div id="header" class="header">
            <table cellpadding="0" cellspacing="0" width="100%">
                <tr>
                    <td width="40%" align="left"><asp:Label ID="LabelSessionId" runat="server"></asp:Label><br /><asp:Label ID="LabelSessionExpiry" runat="server"></asp:Label></td>
                    <td width="20%" align="center"><span style="font-weight:bold; font-size:small;"><%= ConfigurationManager.AppSettings["ApplicationName"]%></span></td>
                    <td width="40%" align="right"><asp:Label ID="LabelUserInfo" runat="server"></asp:Label></td>
                </tr>
            </table>
        </div>
        <form id="form1" runat="server">
            <table cellspacing="8px">
                <tr>
                    <td width="20%"><asp:LinkButton ID="LinkButtonRefreshPage" runat="server" onclick="LinkButtonRefreshPage_Click">Refresh Page</asp:LinkButton></td>
                    <td>Simulate activity in the web application, causing it to update the recorded last activity time and to extend the session if expiry is due.</td>
                </tr>
                <tr>
                    <td width="20%"><asp:LinkButton ID="LinkButtonChangePassword" runat="server" onclick="LinkButtonChangePassword_Click">Change Password</asp:LinkButton></td>
                    <td>Change the signed-on user's password and security questions.</td>
                </tr>
                <tr>
                    <td width="20%"><asp:LinkButton ID="LinkButtonManageAuthorisation" runat="server" onclick="LinkButtonManageAuthorisation_Click">Manage Authorisations</asp:LinkButton></td>
                    <td>Manage the authorisatiosn you have granted to third party applications on your resources.</td>
                </tr>
                <tr>
                    <td width="20%"><asp:LinkButton ID="LinkButtonSignOut" runat="server" onclick="LinkButtonSignOut_Click">Sign Out</asp:LinkButton></td>
                    <td>Sign out of this application only. This application will leave the SSO session but the SSO session will continue if there is more than one participant application.</td>
                </tr>
                <tr>
                    <td width="20%"><asp:LinkButton ID="LinkButtonSignOutAll" runat="server" onclick="LinkButtonSignOutAll_Click">Sign Out All</asp:LinkButton></td>
                    <td>End the SSO session. A Session.Ended notification will be sent to all participant applications.</td>
                </tr>
            </table>
        </form>
        <div id="push" class="push"></div>
    </div>
    <div id="footer" class="footer">
        <div id="footerMessage" class="footerMessage"><asp:Label ID="LabelMessage" runat="server"></asp:Label></div>
    </div>
</body>
</html>
