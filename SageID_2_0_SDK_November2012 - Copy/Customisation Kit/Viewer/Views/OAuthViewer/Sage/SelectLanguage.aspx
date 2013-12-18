<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/TemplateOAuth.master" Inherits="System.Web.Mvc.ViewPage" %>

<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">
    Sage - <%= Resources.Global.TitleLanguageSelection %>
</asp:Content>

<asp:Content ID="OAuthHeaderContent" ContentPlaceHolderID="HeadContent" runat="server">
    <meta name="X-SageID-OAuthDialogSize" content="760,480" />

    <!-- This appears within the HEAD section. Link to stylesheets here via /TemplateOAuth/<TemplateName>/file.ext -->
    <link rel="stylesheet" type="text/css" href="/TemplateOAuth/Sage/style.css" />
    <link rel="stylesheet" type="text/css" href="/TemplateOAuth/Sage/confirm.css" />   
</asp:Content>

<asp:Content ID="selectLanguageContent" ContentPlaceHolderID="MainContent" runat="server">

    <h1><%= Resources.Global.TitleLanguageSelection %></h1>
    <p><%= Resources.Global.BlurbLanguagePopup %></p>
    <p>&nbsp;</p>
    <% Html.RenderPartial("CultureSwitcherTable"); %>

</asp:Content>

<asp:Content ID="OAuthFooterContent" ContentPlaceHolderID="FooterContent" runat="server">
    <script type='text/javascript'>
    <!--
        $(document).delegate('form', 'submit', function () {
            if ($('form').valid()) {
                document.getElementById("clickOverlay").style.display = 'inline-block';
            }
        });
    //-->
    </script>
</asp:Content>