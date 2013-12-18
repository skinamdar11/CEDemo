<%@ Page Language="C#" MasterPageFile="~/Views/Shared/Template.Master" Inherits="System.Web.Mvc.ViewPage<ChangeModel>" %>
<%@ Import Namespace="WebSignOn.Application" %>
<%@ Import Namespace="xVal.Rules" %>
<asp:Content ID="registerTitle" ContentPlaceHolderID="TitleContent" runat="server">
    Sage - <%=Html.Resource(Resources.SSO.TitleChangePassword) %>
</asp:Content>

<asp:Content ID="headContent" ContentPlaceHolderID="HeadContent" runat="server">
    <!-- This appears within the HEAD section. Link to stylesheets here via /Template/<TemplateName>/file.ext -->
    <link rel="stylesheet" type="text/css" href="/Template/Basic/style.css" />
    <link rel="stylesheet" type="text/css" href="/Template/Basic/confirm.css" />
</asp:Content>

<asp:Content ID="changeContent" ContentPlaceHolderID="MainContent" runat="server">

    <div>
        <img id="printLogo" src="/Images/sageclearzone.gif" title="Sage Group plc Logo" alt="Sage Logo, used for printout" />

        <h1><%= Html.Resource(Resources.SSO.TitleChangePassword) %></h1>
        <p><%= Html.Resource(Resources.SSO.BlurbForceChangePassword) %></p>

        <% using (Html.BeginPasswordChangeForm())
        { %>
            <%= Html.AntiForgeryToken() %>

            <% Html.SSO(SSOWidget.ChangeValidationSummary); %>
            
            <p><% Html.SSO(SSOWidget.NewPassword); %></p>

            <p><% Html.SSO(SSOWidget.ConfirmPassword); %></p>  

            <p><% Html.SSO(SSOWidget.Cancel); %><% Html.SSO(SSOWidget.Continue); %></p>
       <% } %>
            
       <% Html.SSO(SSOWidget.ChangeValidationScript); %>
    </div>
        
<script type="text/javascript">
<!--
    document.getElementById('sso_NewPassword').setAttribute("autocomplete", "off");
    document.getElementById('sso_ConfirmPassword').setAttribute("autocomplete", "off");
//-->
</script>           
        
</asp:Content>
<asp:Content ID="footerContent" ContentPlaceHolderID="FooterContent" runat="server">
    <!-- This appears at the bottom of the page -->
    <% Html.SSO(SSOWidget.CultureSwitcher); %>
</asp:Content>