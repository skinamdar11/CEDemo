<%@ Page Language="C#" AutoEventWireup="True" CodeBehind="SignIn.aspx.cs" Inherits="WebApplication.SignIn" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title><%= ConfigurationManager.AppSettings["ApplicationName"]%> - Sign In</title>
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
        <form id="form2" runat="server">
            <table cellspacing="8px">
                <tr>
                    <td width="20%"><asp:LinkButton ID="LinkButtonSignIn" runat="server" OnClick="LinkButtonSignIn_Click">Sign In</asp:LinkButton></td>
                    <td>Sign into this application using Sage SSO.</td>
                </tr>
                <tr>
                    <td width="20%"><asp:LinkButton ID="LinkButtonSignUp" runat="server" OnClick="LinkButtonSignUp_Click">Sign Up</asp:LinkButton></td>
                    <td>Register a new or existing Sage SSO user for this application.</td>
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
