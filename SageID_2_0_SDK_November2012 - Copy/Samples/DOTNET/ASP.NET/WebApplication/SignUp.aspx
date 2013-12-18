<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="SignUp.aspx.cs" Inherits="WebApplication.SignUp" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title><%= ConfigurationManager.AppSettings["ApplicationName"]%> - Registration</title>
    <link href="styles.css" type="text/css" rel="stylesheet" />
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
        <form id="form1" runat="server">
            <table cellspacing="8px">
                <tr>
                    <td width="20%"><asp:LinkButton ID="LinkButtonNewUser" runat="server" onclick="LinkButtonNewUser_Click">New SSO User</asp:LinkButton></td>
                    <td>Create a new Sage SSO user and register the user for this application.</td>
                </tr>
                <tr>
                    <td width="20%"><asp:LinkButton ID="LinkButtonExistingUser" runat="server" onclick="LinkButtonExistingUser_Click">Existing SSO User</asp:LinkButton></td>
                    <td>Register an existing Sage SSO user for this application.</td>
                </tr>
                <tr>
                    <td width="20%"><asp:LinkButton ID="LinkButtonBack" runat="server" onclick="LinkButtonBack_Click">Back</asp:LinkButton></td>
                    <td>Go back to the sign-in page.</td>
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
