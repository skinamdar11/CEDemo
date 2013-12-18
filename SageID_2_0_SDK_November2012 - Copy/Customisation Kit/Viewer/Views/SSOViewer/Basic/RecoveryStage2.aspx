<%@ Page Language="C#" MasterPageFile="~/Views/Shared/Template.Master" Inherits="System.Web.Mvc.ViewPage<RecoveryModel>" %>
<%@ Import Namespace="WebSignOn.Application" %>
<%@ Import Namespace="Resources" %>

<asp:Content ID="titleContent" ContentPlaceHolderID="TitleContent" runat="server">
    Sage - <%=Html.Resource(SSO.TitleSecurityQuestion) %>
</asp:Content>

<asp:Content ID="headContent" ContentPlaceHolderID="HeadContent" runat="server">
    <!-- This appears within the HEAD section. Link to stylesheets here via /Template/<TemplateName>/file.ext -->
    <link rel="stylesheet" type="text/css" href="/Template/Basic/style.css" />
    <link rel="stylesheet" type="text/css" href="/Template/Basic/confirm.css" />
</asp:Content>

<asp:Content ID="recoveryContent" ContentPlaceHolderID="MainContent" runat="server">
    <div>
        <img id="printLogo" src="/Images/sageclearzone.gif" title="Sage Group plc Logo" alt="Sage Logo, used for printout" />

        <h1><%= Html.Resource(Resources.SSO.TitleSecurityQuestion) %></h1>
        
        <p><%= Html.Resource(Resources.SSO.BlurbSecurityQuestion) %></p>
        
        <% Html.SSO(SSOWidget.RecoveryStage2ValidationSummary); %>

        <% using (Html.BeginRecoveryForm())
         { %>
            <%= Html.AntiForgeryToken() %>
                
                <h4><%= Html.Encode(Model.RecoveryQuestion1) %></h4>

                <p><% Html.SSO(SSOWidget.RecoveryAnswer1); %></p>

                <h4><%= Html.Encode(Model.RecoveryQuestion2) %></h4>
                
                <p><% Html.SSO(SSOWidget.RecoveryAnswer2); %></p>

                <p>
                    <% Html.SSO(SSOWidget.Cancel); %>
                    <% Html.SSO(SSOWidget.Continue); %>
                </p>

        <% } %>
                
        <% Html.SSO(SSOWidget.RecoveryStage2ValidationScript); %>
    </div>
    
    <script type="text/javascript">
    <!--
        document.getElementById('sso_RecoveryAnswer1').setAttribute("autocomplete", "off");
        document.getElementById('sso_RecoveryAnswer2').setAttribute("autocomplete", "off");
    //-->
    </script>   
    
</asp:Content>
<asp:Content ID="footerContent" ContentPlaceHolderID="FooterContent" runat="server">
    <!-- This appears at the bottom of the page -->
    <% Html.SSO(SSOWidget.CultureSwitcher); %>
</asp:Content>
