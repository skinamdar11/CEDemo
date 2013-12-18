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
            </table>
        </div>
        <form id="form1" runat="server">
            <table cellspacing="8px">
                <tr>
                    <td width="20%"><%= Html.ActionLink("Start Authorisation", "StartAuthorisationA", "Home") %></td>
                    <td>Start a new authorisation attempt for WebAppA.</td>
                </tr>
                <tr>
                    <td width="20%"><%= Html.ActionLink("Start Authorisation", "StartAuthorisationB", "Home") %></td>
                    <td>Start a new authorisation attempt for WebAppB.</td>
                </tr>
            
            <% if (Session["WebAppATokens"] != null || Session["WebAppBTokens"] != null)
            { %>
                    <tr><td><%= Html.ActionLink("Refresh tokens", "RefreshTokens", "Home") %></td><td>Refresh tokens</td></tr>
            <%} %>

            <% if (Session["WebAppATokens"] != null)
            { %>
                    <tr><td><%= Html.ActionLink("Use access token (SOAP)", "UseAccessTokenSoap", "Home") %></td><td>Use access token with a WebAppA via SOAP</td></tr>
            <%} %>

            <% if (Session["WebAppBTokens"] != null)
            { %>
                    <tr><td><%= Html.ActionLink("Use access token (REST, Header)", "UseAccessTokenRestHeader", "Home") %></td><td>Use access token with a WebAppB via REST and HTTP Header authentication.</td></tr>
                    <tr><td><%= Html.ActionLink("Use access token (REST, Body)", "UseAccessTokenRestBody", "Home") %></td><td>Use access token with a WebAppB via REST and HTTP POST body authentication.</td></tr>
            <%} %>

            </table>
            <div>
            <% if (Session["WebAppATokens"] != null)
            { %>
                <h3>Current WebAppA Tokens:</h3>
                <p><b>AccessToken:</b><%= Html.Raw(((OAuthClientWebApp.DataContracts.GetAccessTokenSuccess)Session["WebAppATokens"]).AccessToken ?? string.Empty)%></p>
                <p><b>RefreshToken:</b><%= Html.Raw(((OAuthClientWebApp.DataContracts.GetAccessTokenSuccess)Session["WebAppATokens"]).RefreshToken ?? string.Empty)%></p>
                <p><b>ExpiresIn:</b><%= Html.Raw(((OAuthClientWebApp.DataContracts.GetAccessTokenSuccess)Session["WebAppATokens"]).ExpiresIn.ToString() ?? string.Empty)%></p>
                <p><b>Type:</b><%= Html.Raw(((OAuthClientWebApp.DataContracts.GetAccessTokenSuccess)Session["WebAppATokens"]).TokenType ?? string.Empty)%></p>
                <p><b>Scope:</b><%= Html.Raw(((OAuthClientWebApp.DataContracts.GetAccessTokenSuccess)Session["WebAppATokens"]).Scope ?? string.Empty)%></p>
            <%} %>
            </div>

            <div>
            <% if (Session["WebAppBTokens"] != null)
            { %>
                <h3>Current WebAppB Tokens:</h3>
                <p><b>AccessToken:</b><%= Html.Raw(((OAuthClientWebApp.DataContracts.GetAccessTokenSuccess)Session["WebAppBTokens"]).AccessToken ?? string.Empty)%></p>
                <p><b>RefreshToken:</b><%= Html.Raw(((OAuthClientWebApp.DataContracts.GetAccessTokenSuccess)Session["WebAppBTokens"]).RefreshToken ?? string.Empty)%></p>
                <p><b>ExpiresIn:</b><%= Html.Raw(((OAuthClientWebApp.DataContracts.GetAccessTokenSuccess)Session["WebAppBTokens"]).ExpiresIn.ToString() ?? string.Empty)%></p>
                <p><b>Type:</b><%= Html.Raw(((OAuthClientWebApp.DataContracts.GetAccessTokenSuccess)Session["WebAppBTokens"]).TokenType ?? string.Empty)%></p>
                <p><b>Scope:</b><%= Html.Raw(((OAuthClientWebApp.DataContracts.GetAccessTokenSuccess)Session["WebAppBTokens"]).Scope ?? string.Empty)%></p>
            <%} %>
            </div>

            <div>
            <% if (Session["AccessTokenSoapResponse"] != null)
            { %>
                <p>Access token (SOAP) response.</p>
                <p><%= Html.Raw(Session["AccessTokenSoapResponse"].ToString())%></p>
            <%} %>
            </div>
            
            <div>
            <% if (Session["AccessTokenRestHeaderResponse"] != null)
            { %>
                <p>Access token (REST, Header) response.</p>
                <p><%= Html.Raw(Session["AccessTokenRestHeaderResponse"].ToString())%></p>
            <%} %>
            </div>

            <div>
            <% if (Session["AccessTokenRestBodyResponse"] != null)
            { %>
                <p>Access token (REST, Body) response.</p>
                <p><%= Html.Raw(Session["AccessTokenRestBodyResponse"].ToString())%></p>
            <%} %>
            </div>

        </form>
        <div id="push" class="push"></div>
    </div>
    <div id="footer" class="footer">
        <div id="footerMessage" class="footerMessage"></div>
    </div>
</body>
</html>