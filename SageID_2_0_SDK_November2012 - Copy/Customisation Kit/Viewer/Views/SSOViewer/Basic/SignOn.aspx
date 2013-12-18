<%@ Page Language="C#" MasterPageFile="~/Views/Shared/Template.Master" Inherits="System.Web.Mvc.ViewPage<WebSignOn.Application.SignOnModel>" %>
<%@ Import Namespace="WebSignOn.Application" %>
<%@ Import Namespace="Resources" %>

<asp:Content ID="titleContent" ContentPlaceHolderID="TitleContent" runat="server">
    Sage - <%=Html.Resource(SSO.TitleSignIn) %>
</asp:Content>

<asp:Content ID="headContent" ContentPlaceHolderID="HeadContent" runat="server">
    <!-- This appears within the HEAD section. Link to stylesheets here via /Template/<TemplateName>/file.ext -->
    <link rel="stylesheet" type="text/css" href="/Template/Basic/style.css" />
    <link rel="stylesheet" type="text/css" href="/Template/Basic/confirm.css" />    
</asp:Content>

<asp:Content ID="loginContent" ContentPlaceHolderID="MainContent" runat="server">   
    <div>
        <img src="/Images/sageclearzone.gif" title="Sage Group plc Logo" alt="Sage Logo" />
        <h1>
            <%= Html.Resource(SSO.TitleSignIn) %></h1>
        <h2>
            <%= Html.Resource(SSO.MessageSigningInto)%>
            <%= Html.Encode(Model.WebApplicationDisplayName)%>.</h2>
        <p>
            <%= Html.Resource(SSO.BlurbSignIn) %>
        </p>
        <% using (Html.BeginForm())
           { %>
        <%= Html.AntiForgeryToken() %>
        <div>
            <fieldset>
            
                <legend><%= Html.Resource(SSO.TitleAccountInformation) %></legend>
                
                <ul>
                    <% Html.SSO(SSOWidget.ForgotPasswordLinkListItem); %>
                </ul>
                
                <% Html.SSO(SSOWidget.SignInValidationSummary); %>
                               
                <p><% Html.SSO(SSOWidget.Email); %></p>
                <p><% Html.SSO(SSOWidget.Password); %></p>
                    
                <% Html.SSO(SSOWidget.Captcha); %>
                
                <p><% Html.SSO(SSOWidget.RememberMe); %></p>
                
                <p>
                    <% Html.SSO(SSOWidget.Cancel); %>
                    <% Html.SSO(SSOWidget.SignIn); %>
                </p> 
            </fieldset>
        </div>
        <% } %>
        
        <!-- Validation script needs to be positioned after the form -->
        <% Html.SSO(SSOWidget.SignInValidationScript); %>
        
    </div>

    <script type="text/javascript">
    <!--
        document.getElementById('sso_Email').setAttribute("autocomplete","off");
        document.getElementById('sso_Password').setAttribute("autocomplete","off");
    //-->
    </script>

</asp:Content>

<asp:Content ID="footerContent" ContentPlaceHolderID="FooterContent" runat="server">
    <!-- This appears at the bottom of the page -->
    <% Html.SSO(SSOWidget.CultureSwitcher); %>
</asp:Content>
