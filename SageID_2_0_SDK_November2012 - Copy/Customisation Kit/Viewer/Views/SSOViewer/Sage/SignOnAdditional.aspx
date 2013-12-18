<%@ Page Language="C#" MasterPageFile="~/Views/Shared/Template.Master" Inherits="System.Web.Mvc.ViewPage<SignOnModel>" %>

<%@ Import Namespace="WebSignOn.Application" %>
<%@ Import Namespace="Resources" %>
<asp:Content ID="titleContent" ContentPlaceHolderID="TitleContent" runat="server">
    Sage -
    <%=Html.Resource(SSO.TitleSignIn) %>
</asp:Content>
<asp:Content ID="headContent" ContentPlaceHolderID="HeadContent" runat="server">
    <!-- This appears within the HEAD section. Link to stylesheets here via /Template/<TemplateName>/file.ext -->
    <link rel="stylesheet" type="text/css" href="/Template/Sage/style.css" />
    <link rel="stylesheet" type="text/css" href="/Template/Sage/basic.css" />
    <link rel="stylesheet" type="text/css" href="/Template/Sage/confirm.css" />
</asp:Content>
<asp:Content ID="loginContent" ContentPlaceHolderID="MainContent" runat="server">
    <div class="page">
        <div id="headerContainer">
            <div id="headerBare">
                <div id="sageLogo">
                    <img src="/Template/Sage/images/<%= Html.Resource(Resources.SSO.FileTitleImage) %>" alt="Sage" /></div>
            </div>
        </div>
    </div>
    <div id="bodyContainerBare">

        <% Html.SSO(SSOWidget.ConfirmSignoutDialog); %>

        <div id="leftMenu"></div>
        <div id="breadcrumb">
            <span class="breadItem">
                <%= Html.Encode(Model.WebApplicationDisplayName)%></span>&nbsp;/&nbsp;<span class="breadItemSelected"><%= Html.Resource(Resources.SSO.TitleSignIn) %></span>
        </div>
        <div id="contentBorderTop">
        </div>
        <div id="contentBorder">
            <div id="content">
                <img id="printLogo" src="/Template/Sage/images/sageclearzone.gif" title="Sage Group plc Logo" alt="Sage Logo, used for printout" />
                <div id="contentOneCol">
                    <h1>
                        <%= Html.Resource(Resources.SSO.TitleSignIn) %></h1>
                    <h2>
                        <%= Html.Resource(Resources.SSO.MessageSigningInto) %>
                        <%= Html.Encode(Model.WebApplicationDisplayName)%>.</h2>
                    <h3><%= String.Format(Html.Resource(Resources.SSO.MessageAlreadySignedInto), Html.Encode(Model.DisplayName)) %></h3>
                    <% Html.SSO(SSOWidget.ApplicationList); %>
                    <p class="tagline">
                        <%= Html.Resource(Resources.SSO.BlurbSignOnAdditional) %></p>
                    <% using (Html.BeginForm())
                       { %>
                    <%= Html.SSOAntiForgeryToken()%>
                    <div>
                        <fieldset>
                            <legend>
                                <%= Html.Resource(Resources.SSO.TitleSignInConfirmation) %></legend>
                           <div id="contentRight">                         
                                <div id="RightSideMenu" class="thirdLevelNav" <% if (!Model.SwitchUserAllowed) { %>style="display: none;"<% } %>>
                                    <div id="RightSideMenuTop">
                                        <img src="/Template/Sage/images/spacer.gif" alt="spacer" height="6" width="197" /></div>
                                        <ul>
                                            <% Html.SSO(SSOWidget.SwitchUserLinkListItem); %>
                                        </ul>
                                    <div id="RightSideMenuBot">
                                        <img src="/Template/Sage/images/spacer.gif" alt="spacer" height="6" width="197" /></div>
                                </div>                    
                                <div style="clear: both;">
                                    <!--clear:both-->
                                </div>
                            </div>   
                            <% Html.SSO(SSOWidget.SignInValidationSummary); %>
                            <p>
                                <% Html.SSO(SSOWidget.Email, new { @readonly = true }); %></p>
                            <p>
                                <% Html.SSO(SSOWidget.Password); %></p>
                            <% Html.SSO(SSOWidget.Captcha); %>
                            <p>
                                <% Html.SSO(SSOWidget.RememberMe); %></p>
                            <p>
                                <% Html.SSO(SSOWidget.Cancel); %>
                                <% Html.SSO(SSOWidget.SignIn); %>
                            </p>
                        </fieldset>
                    </div>
                    <% } %>
                    <% Html.SSO(SSOWidget.SignInValidationScript); %>
                </div>
                <br style="clear: both;" />
            </div>
        </div>
        <div id="footer">
            <div class="floatLeft">
                <img id="ssl" src="/Template/Sage/images/padlock.jpg" alt="SSL Secured" title="SSL Secured" /></div>
            <ul id="footerMenu">
                <li><a href="http://www.sage.com/">
                    <%= Html.Resource(Resources.SSO.LinkHome) %></a></li>
                <li>| </li>
                <li><a href="http://www.sage.com/privacypolicy">
                    <%= Html.Resource(Resources.SSO.LinkPrivacyPolicy) %></a></li>
                <li>| </li>
                <li><a href="http://www.sage.com/termsandconditions">
                    <%= Html.Resource(Resources.SSO.LinkTermsConditions) %></a></li>
                <li>| </li>
                <li>
                    <% Html.SSO(SSOWidget.CultureSwitcher); %></li>
            </ul>
            <p class="copyright">
                <%= Html.Resource(Resources.SSO.FooterCopyright) %></p>
            <p class="companyDisclaimer">
                <%= Html.Resource(Resources.SSO.FooterCompanyDisclaimer) %></p>
        </div>
    </div>
    <script type="text/javascript">
    <!--
        document.getElementById('sso_Email').setAttribute("autocomplete", "off");
        document.getElementById('sso_Password').setAttribute("autocomplete", "off");
        document.getElementById('sso_Password').focus();
    //-->
    </script>
</asp:Content>
<asp:Content ID="footerContent" ContentPlaceHolderID="FooterContent" runat="server"></asp:Content>
