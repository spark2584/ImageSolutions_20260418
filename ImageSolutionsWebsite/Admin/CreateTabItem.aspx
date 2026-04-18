<%@ Page Language="C#" MasterPageFile="~/MasterPage.Master" AutoEventWireup="true" CodeBehind="CreateTabItem.aspx.cs" Inherits="ImageSolutionsWebsite.Admin.CreateTabItem" %>
<%@ Register Src="~/Control/AdminHeader.ascx" TagPrefix="uc" TagName="AdminHeader" %>

<asp:Content ID="cHead" ContentPlaceHolderID="head" runat="server">
</asp:Content>
<asp:Content ID="cHeader" ContentPlaceHolderID="cphHeader" runat="server">
</asp:Content>
<asp:Content ID="cBody" ContentPlaceHolderID="cphBody" runat="server">
    <uc:AdminHeader id="AdminHeader" runat="server"></uc:AdminHeader>
    <table style="width:100%">
        <tr>
            <td colspan="2">
                <h1><asp:Label ID="lblHeader" runat="server"></asp:Label></h1>
            </td>
        </tr>
        <tr>
            <td colspan="2">
                <asp:Button ID="btnCancel" runat="server" Text="Back" OnClick="btnCancel_Click" />
            </td>
        </tr>
        <tr>
            <td>
                Item
            </td>
            <td>
                <asp:DropDownList ID="ddlItem" runat="server"></asp:DropDownList>
                <asp:RequiredFieldValidator ID="valItem" runat="server" ControlToValidate="ddlItem" ValidationGroup="Item" ErrorMessage="*" ForeColor="Red"></asp:RequiredFieldValidator>
            </td>
        </tr>
        <tr>
            <td>
                Inactive
            </td>
            <td>
                <asp:CheckBox ID="cbItemInactive" runat="server" />
            </td>
        </tr>
        <tr>
            <td colspan="2">
                <asp:Button ID="btnCreate" runat="server" Text="Add" OnClick="btnCreate_Click" ValidationGroup="Item"/>
            </td>
        </tr>
    </table>
</asp:Content>
