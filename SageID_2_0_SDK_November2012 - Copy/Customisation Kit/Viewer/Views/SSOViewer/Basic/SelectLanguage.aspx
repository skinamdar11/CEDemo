<%@ Page Language="C#" MasterPageFile="~/Views/Shared/Template.Master" Inherits="System.Web.Mvc.ViewPage" %>
<%@ Import Namespace="WebSignOn.Application" %>
<%@ Import Namespace="Resources" %>

<asp:Content ID="selectLanguageTitle" ContentPlaceHolderID="TitleContent" runat="server">
    Sage - <%= Resources.Global.TitleLanguageSelection %>
</asp:Content>

<asp:Content ID="headContent" ContentPlaceHolderID="HeadContent" runat="server">
    <!-- This appears within the HEAD section. Link to stylesheets here via /Template/<TemplateName>/file.ext -->
    <link rel="stylesheet" type="text/css" href="/Template/Basic/style.css" />
    <link rel="stylesheet" type="text/css" href="/Template/Basic/confirm.css" />    
</asp:Content>

<asp:Content ID="selectLanguageContent" ContentPlaceHolderID="MainContent" runat="server">
    <div>
        <img id="printLogo" src="/Images/sageclearzone.gif" title="Sage Group plc Logo" alt="Sage Logo" />
        <h1><%= Html.Resource(SSO.TitleLanguageSelection) %></h1>
        <p><%= Html.Resource(SSO.BlurbLanguagePopup) %></p>
        <p>&nbsp;</p>
        <% Html.RenderPartial("CultureSwitcherTable"); %>
    </div>
</asp:Content>

<asp:Content ID="footerContent" ContentPlaceHolderID="FooterContent" runat="server"></asp:Content>