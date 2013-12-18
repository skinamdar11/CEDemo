<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/TemplateOAuth.master" Inherits="System.Web.Mvc.ViewPage<WebSignOn.Application.OAuth.AuthoriseModel>" %>

<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">
    Sage ID - Sign In
</asp:Content>

<asp:Content ID="OAuthHeaderContent" ContentPlaceHolderID="HeadContent" runat="server">
    <!-- This appears within the HEAD section. Link to stylesheets here via /TemplateOAuth/<TemplateName>/file.ext -->
    <link rel="stylesheet" type="text/css" href="/TemplateOAuth/Sage/style.css" />
    <link rel="stylesheet" type="text/css" href="/TemplateOAuth/Sage/confirm.css" />
    <meta name="X-SageID-OAuthDialogSize" content="760,550" />
</asp:Content>

<asp:Content ID="OAuthBodyContent" ContentPlaceHolderID="MainContent" runat="server">
    <div id="headerContainer">
        <div id="headerBare">
            <div id="sageLogo">
                <img src="/Images/<%= Html.Resource(Resources.SSO.FileTitleImage) %>" alt="Sage" /></div>
        </div>
    </div>
    <div id="bodyContainerBare">
        <div id="contentBorderTop"></div>
        <div id="contentBorder">
            <div id="content">
                <div id="contentOneCol">
                    <h2>
                        <%= Model.WebApplicationDisplayName %>&nbsp;<%= Html.Resource(Resources.SSO.MessageRequestingAccess) %></h2>

                    <% using (Html.BeginForm()) {%>

                        <%= Html.SSOAntiForgeryToken() %>

                        <% Html.SSO(SSOWidget.PermissionList); %>

                        <% Html.SSO(SSOWidget.AuthoriseValidationSummary); %>

                        <% Html.SSO(SSOWidget.Captcha, new { ToolTip = false, PrefixBreak = false }); %>
                        
                        <p>
                            <% Html.SSO(SSOWidget.Deny); %>
                            <% Html.SSO(SSOWidget.Allow); %>
                        </p>
                    <%}%>

                    <!-- Validation script needs to be positioned after the form -->
                    <% Html.SSO(SSOWidget.AuthoriseValidationScript); %>

                    <br style="clear: both;" />
                </div>
            </div>
        </div>
        <div id="footer">
            <div class="floatLeft">
                <% Html.RenderPartial("CultureSwitcher"); %>
            </div>
        </div>
    </div>

    <% Html.RenderPartial("CultureSwitcherPopup"); %>

</asp:Content>

<asp:Content ID="OAuthFooterContent" ContentPlaceHolderID="FooterContent" runat="server" />
