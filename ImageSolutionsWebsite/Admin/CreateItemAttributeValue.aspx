<%@ Page Language="C#" MasterPageFile="~/MasterPage.Master" AutoEventWireup="true" CodeBehind="CreateItemAttributeValue.aspx.cs" Inherits="ImageSolutionsWebsite.Admin.CreateItemAttributeValue" %>
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
                <h1>Create Attribute Value</h1>
            </td>
        </tr>
        <tr>
            <td colspan="2">
                <asp:Button ID="btnCancel" runat="server" Text="Back" OnClick="btnCancel_Click" />
            </td>
        </tr>
        <tr>
            <td colspan="2">
                <asp:Label ID="lblExistingItemMessage" runat="server" ForeColor="Red"></asp:Label>
            </td>
        </tr>
        <tr>
            <td>
                Attribute Value
            </td>
            <td>
                <asp:TextBox ID="txtAttributeValue" runat="server"></asp:TextBox>
                <asp:RequiredFieldValidator ID="valAttributeValue" runat="server" ControlToValidate="txtAttributeValue" ValidationGroup="Submit" ErrorMessage="*" ForeColor="Red"></asp:RequiredFieldValidator>
            </td>
        </tr>
        <tr>
            <td>
                Abbreviation
            </td>
            <td>
                <asp:TextBox ID="txtAbbreviation" runat="server"></asp:TextBox>
            </td>
        </tr>
        <tr>
            <td>
                Is Default
            </td>
            <td>
                <asp:CheckBox ID="cbIsDefault" runat="server" />
            </td>
        </tr>
        <tr>
            <td colspan="2">
                <asp:Button ID="btnCreate" runat="server" Text="Create" OnClick="btnCreate_Click" ValidationGroup="Submit"/>
            </td>
        </tr>        
    </table>
</asp:Content>