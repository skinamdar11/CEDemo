<%@ Page Language="C#" MasterPageFile="~/Views/Shared/Template.Master" Inherits="System.Web.Mvc.ViewPage<RecoveryModel>" %>
<%@ Import Namespace="WebSignOn.Application" %>
<%@ Import Namespace="Resources" %>

<asp:Content ID="titleContent" ContentPlaceHolderID="TitleContent" runat="server">
    Sage - <%=Html.Resource(SSO.TitleRecovery) %>
</asp:Content>

<asp:Content ID="headContent" ContentPlaceHolderID="HeadContent" runat="server">
    <!-- This appears within the HEAD section. Link to stylesheets here via /Template/<TemplateName>/file.ext -->
    <link rel="stylesheet" type="text/css" href="/Template/Basic/style.css" />
    <link rel="stylesheet" type="text/css" href="/Template/Basic/confirm.css" />
</asp:Content>

<asp:Content ID="recoveryContent" ContentPlaceHolderID="MainContent" runat="server">
    <div>
        <img id="printLogo" src="/Images/sageclearzone.gif" title="Sage Group plc Logo" alt="Sage Logo, used for printout" />

        <h1><%= Html.Resource(Resources.SSO.TitleRecovery) %></h1>
        
        <p><%= Html.Resource(Resources.SSO.BlurbRecovery) %></p>
        
        <% Html.SSO(SSOWidget.RecoveryStage1ValidationSummary); %>

        <% using (Html.BeginRecoveryForm())
         { %>
            <%= Html.AntiForgeryToken() %>
                
                <p><% Html.SSO(SSOWidget.Email); %></p>

                <% Html.SSO(SSOWidget.Captcha); %>

                <p>
                    <% Html.SSO(SSOWidget.Cancel); %>
                    <% Html.SSO(SSOWidget.Continue); %>
                </p>

        <% } %>
                
        <% Html.SSO(SSOWidget.RecoveryStage1ValidationScript); %>
    </div>
    
    <script type="text/javascript">
    <!--
        document.getElementById('sso_Email').setAttribute("autocomplete", "off");
    //-->
    </script>   
    
</asp:Content>
<asp:Content ID="footerContent" ContentPlaceHolderID="FooterContent" runat="server">
    <!-- This appears at the bottom of the page -->
    <% Html.SSO(SSOWidget.CultureSwitcher); %>
</asp:Content>
