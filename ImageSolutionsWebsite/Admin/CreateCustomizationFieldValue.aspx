<%@ Page Language="C#" MasterPageFile="~/MasterPage.Master" AutoEventWireup="true" CodeBehind="CreateCustomizationFieldValue.aspx.cs" Inherits="ImageSolutionsWebsite.Admin.CreateCustomizationFieldValue" %>
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
                <h1>Create Field Value</h1>
            </td>
        </tr>
        <tr>
            <td colspan="2">
                <asp:Button ID="btnCancel" runat="server" Text="Cancel" OnClick="btnCancel_Click" />
            </td>
        </tr>
        <tr>
            <td>
                Field Value
            </td>
            <td>
                <asp:TextBox ID="txtFieldValue" runat="server"></asp:TextBox>
                <asp:RequiredFieldValidator ID="valFieldValue" runat="server" ControlToValidate="txtFieldValue" ValidationGroup="Submit" ErrorMessage="*" ForeColor="Red"></asp:RequiredFieldValidator>
            </td>
        </tr>
        <tr>
            <td>
                Query
            </td>
            <td>
                <asp:TextBox ID="txtQuery" runat="server" TextMode="MultiLine"></asp:TextBox>
            </td>
        </tr>
        <tr>
            <td colspan="2">
                <asp:Button ID="btnCreate" runat="server" Text="Create" OnClick="btnCreate_Click" ValidationGroup="Submit"/>
            </td>
        </tr>        
    </table>

</asp:Content>