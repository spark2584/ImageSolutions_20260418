<%@ Page Language="C#" MasterPageFile="~/MasterPage.Master" AutoEventWireup="true" CodeBehind="CreateCustomizationField.aspx.cs" Inherits="ImageSolutionsWebsite.Admin.CreateCustomizationField" %>
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
                <h1>Create Customization Field</h1>
            </td>
        </tr>
        <tr>
            <td colspan="2">
                <asp:Button ID="btnCancel" runat="server" Text="Cancel" OnClick="btnCancel_Click" />
            </td>
        </tr>     
        <tr>
            <td>
                Field Name
            </td>
            <td>
                <asp:TextBox ID="txtName" runat="server"></asp:TextBox>
                <asp:RequiredFieldValidator ID="valName" runat="server" ControlToValidate="txtName" ValidationGroup="Submit" ErrorMessage="*" ForeColor="Red"></asp:RequiredFieldValidator>
            </td>
        </tr>
        <tr>
            <td>
                Type
            </td>
            <td>
                <asp:DropDownList ID="ddlType" runat="server">
                    <asp:ListItem Selected="True" Value="freetext">Free Text</asp:ListItem>
                    <asp:ListItem Value="dropdown">Drop Down</asp:ListItem>
                    <asp:ListItem Value="radiobutton">Radio Button</asp:ListItem>
                    <asp:ListItem Value="checkbox">Checkbox</asp:ListItem>
                </asp:DropDownList>
            </td>
        </tr>
        <tr>
            <td colspan="2">
                <asp:Button ID="btnCreate" runat="server" Text="Create" OnClick="btnCreate_Click" ValidationGroup="Submit"/>
            </td>
        </tr>  
    </table>

</asp:Content>