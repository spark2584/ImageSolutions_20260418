<%@ Page Language="C#" MasterPageFile="~/MasterPage.Master" AutoEventWireup="true" CodeBehind="CreateItemAttribute.aspx.cs" Inherits="ImageSolutionsWebsite.Admin.CreateItemAttribute" %>
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
                <h1>Create Attribute</h1>
            </td>
        </tr>
        <tr>
            <td colspan="2">
                <asp:Button ID="btnCancel" runat="server" Text="Cancel" OnClick="btnCancel_Click" />
            </td>
        </tr>
        <tr>
            <td>
                Attribute
            </td>
            <td>
                <asp:TextBox ID="txtName" runat="server" ValidationGroup="Submit"></asp:TextBox>
                <asp:RequiredFieldValidator ID="valName" runat="server" ControlToValidate="txtName" ValidationGroup="Submit" ErrorMessage="*" ForeColor="Red"></asp:RequiredFieldValidator>
            </td>
        </tr>
        <tr>
            <td>
                Sort
            </td>
            <td>
                <asp:TextBox ID="txtSort" runat="server" onkeydown = "return (!(event.keyCode>=65) && event.keyCode!=32);" Text="0" ValidationGroup="Submit"></asp:TextBox>
            </td>
        </tr>
        <tr>
            <td colspan="2">
                <asp:Button ID="btnCreate" runat="server" Text="Create" OnClick="btnCreate_Click" ValidationGroup="Submit"/>
            </td>
        </tr>        
    </table>

</asp:Content>