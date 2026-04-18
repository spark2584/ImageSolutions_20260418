<%@ Page Language="C#" MasterPageFile="~/MasterPage.Master" AutoEventWireup="true" CodeBehind="Default.aspx.cs" Inherits="ImageSolutionsWebsite.Admin.Default" %>
<%@ Register Src="~/Control/AdminHeader.ascx" TagPrefix="uc" TagName="AdminHeader" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="cphHeader" runat="server">
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="cphBody" runat="server">
    <uc:AdminHeader id="AdminHeader" runat="server"></uc:AdminHeader>
    <table>
        <tr>
            <td>
                
            </td>
        </tr>
    </table>
</asp:Content>