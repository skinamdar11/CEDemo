<%@ Page Language="C#" Inherits="System.Web.Mvc.ViewPage" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml" >
<head runat="server">
    <title>Index</title>
    <style type="text/css">body { font-family: Arial; margin-left: 30px; background: url(/Images/bck.jpg) no-repeat scroll 60% -80px #FFFFFF; }</style>
</head>
<body>
    <h1 style="color: Green">Sage ID Template Viewer 1.1</h1>
    <h2>Sage ID - Single Sign On</h2>
    <div>
        <ul>
            <li><%= Html.ActionLink("First sign on", "SignOn", new { id = "1234567890" }) %></li>
            <li><%= Html.ActionLink("Additional sign on", "SignOnAdditional", new { id = "1234567890" }) %></li>
            <li><%= Html.ActionLink("Express sign on", "SignOnExpress", new { id = "1234567890" }) %></li>
            <li><%= Html.ActionLink("Register", "Register", new { id = "1234567890" }) %></li>

            <li><%= Html.ActionLink("Password Recovery (Stage 1)", "RecoveryStage1", new { id = "1234567890" }) %></li>
            <li><%= Html.ActionLink("Password Recovery (Stage 2)", "RecoveryStage2", new { id = "1234567890" })%></li>
            <li><%= Html.ActionLink("Password Recovery (Stage 3)", "RecoveryStage3", new { id = "1234567890" })%></li>

            <li><%= Html.ActionLink("Password Change", "Change", new { id = "1234567890" })%></li>
            <li><%= Html.ActionLink("Forced Password Change", "ChangePasswordOnly", new { id = "1234567890" })%></li>
            
            <li><%= Html.ActionLink("Custom ReCaptcha Theme Demo", "RegisterCustomCaptchaDemo", new { id = "1234567890" })%></li>
        </ul>
    </div>
    <h2>Sage ID OAuth</h2>
    <div>
        <ul>
            <li><%= Html.ActionLink("Authenticate", "Authenticate", "OAuthViewer", new { id = "1234567890" }, null)%></li>
            <li><%= Html.ActionLink("Authorise", "Authorise", "OAuthViewer", new { id = "1234567890" }, null) %></li>
            <li><%= Html.ActionLink("Manage Authorisations", "ManageAuthorisations", new { id = "1234567890" }, null) %></li>
        </ul>
    </div>
    <h2>Settings</h2>
    <div>
        <ul>
            <li><%= Html.ActionLink("Select language (SSO)", "SelectLanguage", new { id = "1234567890" }) %></li>
            <li><%= Html.ActionLink("Select language (OAuth)", "SelectLanguage", "OAuthViewer", new { id = "1234567890"}, null)%></li>
        </ul>
    </div>
    <h2>Support</h2>
    <div>
        <ul>
            <li><a href="mailto:ssdpdevelopersupport@sage.com?subject=Sage%20ID%20Customisation%20Kit">SSDP Developer Support (R&amp;D Manchester)</a></li>
        </ul>
    </div>
</body>
</html>
