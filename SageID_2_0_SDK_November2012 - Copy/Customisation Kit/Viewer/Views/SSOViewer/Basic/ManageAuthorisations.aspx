<%@ Page Language="C#" MasterPageFile="~/Views/Shared/Template.Master" Inherits="System.Web.Mvc.ViewPage<ManageAuthorisationModel>" %>
<%@ Import Namespace="WebSignOn.Application" %>
<%@ Import Namespace="xVal.Rules" %>

<asp:Content ID="manageTitle" ContentPlaceHolderID="TitleContent" runat="server">
    Sage - <%=Html.Resource(Resources.SSO.TitleManageAuthorisation)%>
</asp:Content>

<asp:Content ID="manageContent" ContentPlaceHolderID="MainContent" runat="server">

    <div>
        <img src="/Images/sageclearzone.gif" title="Sage Group plc Logo" alt="Sage Logo" />
        <h1><%= Html.Resource(Resources.SSO.TitleManageAuthorisation) %></h1>
        <p><%= Html.Resource(Resources.SSO.BlurbManageAuthorisation) %></p>
        <% using (Html.BeginForm())
        { %>
            <%= Html.SSOAntiForgeryToken()%>
                
            <div >
                <% if (Model.Authorisations == null ||
                    Model.Authorisations.ResourceAuthorisations == null ||
                    Model.Authorisations.ResourceAuthorisations.Length == 0)
                { %>
                    <h4><%= Html.Resource(Resources.SSO.BlurbNoAuthorisationsToManage)%></h4>
                <%}
                else
                { %>
                           
                    <%Html.SSO(SSOWidget.ManageAuthorisationsList); %>   

                <%} %>
                <p>
                    <% Html.SSO(SSOWidget.Cancel); %>
                    <% Html.SSO(SSOWidget.Continue); %>
                </p>

            </div>
            <div class="clearAll">
            </div>

        <% } %>
    </div>
        
<script type="text/javascript">
<!--

//-->
</script>           
        
</asp:Content>
<asp:Content ID="footerContent" ContentPlaceHolderID="FooterContent" runat="server">
        <% Html.SSO(SSOWidget.CultureSwitcher); %>
</asp:Content>