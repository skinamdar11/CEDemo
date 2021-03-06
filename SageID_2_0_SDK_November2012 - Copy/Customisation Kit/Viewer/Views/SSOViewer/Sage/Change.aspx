<%@ Page Language="C#" MasterPageFile="~/Views/Shared/Template.Master" Inherits="System.Web.Mvc.ViewPage<ChangeModel>" %>
<%@ Import Namespace="WebSignOn.Application" %>
<%@ Import Namespace="xVal.Rules" %>
<%@ Import Namespace="Resources" %>
<asp:Content ID="titleContent" ContentPlaceHolderID="TitleContent" runat="server">
    Sage -
    <%=Html.Resource(SSO.TitleChangePassword)%>
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
        <div id="leftMenu">
        </div>
        <div id="breadcrumb">
            <span class="breadItem">
                <%= Html.Encode(Model.WebApplicationDisplayName)%></span>&nbsp;/&nbsp;<span class="breadItemSelected"><%= Html.Resource(Resources.SSO.LinkChangePassword)%></span>
        </div>
        <div id="contentBorderTop">
        </div>
        <div id="contentBorder">
            <div id="content">
                <img id="printLogo" src="/Template/Sage/images/sageclearzone.gif" title="Sage Group plc Logo" alt="Sage Logo, used for printout" />
                <div id="contentOneCol">
                    <h1>
                        <%= Html.Resource(Resources.SSO.TitleChangePassword)%></h1>
                    <p>
                        <%= Html.Resource(Resources.SSO.BlurbChangePassword)%></p>
                    <% using (Html.BeginForm())
                       { %>
                    <%= Html.SSOAntiForgeryToken()%>
                    <div>
                        <fieldset>
                            <legend>
                                <%= Html.Resource(Resources.SSO.TitleAccountCredentials)%></legend>
                            <% Html.SSO(SSOWidget.ChangeValidationSummary); %>
                            <p>
                                <% Html.SSO(SSOWidget.Password, new { tooltip = false }); %>
                            </p>
                            <p>
                                <% Html.SSO(SSOWidget.NewPassword); %>
                            </p>
                            <p>
                                <% Html.SSO(SSOWidget.ConfirmPassword); %>
                            </p>
                            <p>
                                <% Html.SSO(SSOWidget.SecurityQuestion1); %>
                            </p>                        
                            <p>
                                <% Html.SSO(SSOWidget.SecurityAnswer1); %>
                            </p>
                            <p>
                                <% Html.SSO(SSOWidget.SecurityQuestion2); %>
                            </p>                        
                            <p>
                                <% Html.SSO(SSOWidget.SecurityAnswer2); %>
                            </p> 
                            <p>
                                <% Html.SSO(SSOWidget.SecurityQuestion3); %>
                            </p>                        
                            <p>
                                <% Html.SSO(SSOWidget.SecurityAnswer3); %>
                            </p>    
                            <p>
                                <% Html.SSO(SSOWidget.Cancel); %>
                                <% Html.SSO(SSOWidget.Continue); %>
                            </p>
                        </fieldset>
                    </div>
                    <% } %>
                </div>
                <% Html.SSO(SSOWidget.ChangeValidationScript); %>
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
        document.getElementById('sso_Password').setAttribute("autocomplete", "off");
        document.getElementById('sso_NewPassword').setAttribute("autocomplete", "off");
        document.getElementById('sso_ConfirmPassword').setAttribute("autocomplete", "off");
        document.getElementById('sso_SecurityAnswer1').setAttribute("autocomplete", "off");
        document.getElementById('sso_SecurityAnswer2').setAttribute("autocomplete", "off");
        document.getElementById('sso_SecurityAnswer3').setAttribute("autocomplete", "off");
    //-->
    </script>
</asp:Content>
<asp:Content ID="footerContent" ContentPlaceHolderID="FooterContent" runat="server"></asp:Content>
