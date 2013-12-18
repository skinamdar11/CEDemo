<%@ Page Language="C#" MasterPageFile="~/Views/Shared/Template.Master" Inherits="System.Web.Mvc.ViewPage<WebSignOn.Application.RegisterModel>" %>
<%@ Import Namespace="WebSignOn.Application" %>
<%@ Import Namespace="Resources" %>

<asp:Content ID="titleContent" ContentPlaceHolderID="TitleContent" runat="server">
    Sage - <%=Html.Resource(SSO.TitleRegister) %>
</asp:Content>

<asp:Content ID="headContent" ContentPlaceHolderID="HeadContent" runat="server">
    <!-- This appears within the HEAD section. Link to stylesheets here via /Template/<TemplateName>/file.ext -->
    <link rel="stylesheet" type="text/css" href="/Template/Basic/style.css" />
    <link rel="stylesheet" type="text/css" href="/Template/Basic/confirm.css" />    
</asp:Content>

<asp:Content ID="loginContent" ContentPlaceHolderID="MainContent" runat="server">   
    <div>
        <img src="/Images/sageclearzone.gif" title="Sage Group plc Logo" alt="Sage Logo" />

        <h1><%= Html.Resource(Resources.SSO.TitleRegister) %></h1>

        <%= Html.Resource(Resources.SSO.BlurbRegisterAccount) %>

        <p><%= Html.Resource(Resources.SSO.RegisterAccountPassword) %></p>

        <% using (Html.BeginForm())
           { %>
        <%= Html.AntiForgeryToken() %>
        <div>
            <fieldset>
            
                <legend><%= Html.Resource(SSO.TitleAccountInformation) %></legend>
                                
                <% Html.SSO(SSOWidget.RegisterValidationSummary); %>
                               
                <p><% Html.SSO(SSOWidget.Email); %></p>
                <p><% Html.SSO(SSOWidget.DisplayName); %></p>

                <p><% Html.SSO(SSOWidget.Password); %></p>
                <p><% Html.SSO(SSOWidget.ConfirmPassword); %></p>

                <p><% Html.SSO(SSOWidget.SecurityQuestion1); %></p>
                <p><% Html.SSO(SSOWidget.SecurityAnswer1); %></p>

                <p><% Html.SSO(SSOWidget.SecurityQuestion2); %></p>
                <p><% Html.SSO(SSOWidget.SecurityAnswer2); %></p>

                <p><% Html.SSO(SSOWidget.SecurityQuestion3); %></p>
                <p><% Html.SSO(SSOWidget.SecurityAnswer3); %></p>
                    
                <% Html.SSO(SSOWidget.Captcha); %>
                                
                <p>
                    <% Html.SSO(SSOWidget.Cancel); %>
                    <% Html.SSO(SSOWidget.Register); %>
                </p> 
            </fieldset>
        </div>
        <% } %>
        
        <!-- Validation script needs to be positioned after the form -->
        <% Html.SSO(SSOWidget.RegisterValidationScript); %>
        
    </div>

    <script type="text/javascript">
    <!--
        document.getElementById('sso_DisplayName').setAttribute("autocomplete", "off");
        document.getElementById('sso_Email').setAttribute("autocomplete", "off");
        document.getElementById('sso_SecurityAnswer1').setAttribute("autocomplete", "off");
        document.getElementById('sso_SecurityAnswer2').setAttribute("autocomplete", "off");
        document.getElementById('sso_SecurityAnswer3').setAttribute("autocomplete", "off");
    //-->
    </script>

</asp:Content>

<asp:Content ID="footerContent" ContentPlaceHolderID="FooterContent" runat="server">
    <!-- This appears at the bottom of the page -->
    <% Html.SSO(SSOWidget.CultureSwitcher); %>
</asp:Content>
