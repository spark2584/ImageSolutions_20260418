<%@ Page Language="C#" MasterPageFile="~/MasterPage.Master" AutoEventWireup="true" CodeBehind="CreateWebsite.aspx.cs" Inherits="ImageSolutionsWebsite.Admin.CreateWebsite" %>
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
                <h1>Create New Website</h1>
            </td>
        </tr>
        <tr>
            <td>
                <asp:Button ID="btnCancel" runat="server" Text="Back" OnClick="btnCancel_Click" />
            </td>
        </tr>
        <tr>
            <td>
                Name
            </td>
            <td>
                <asp:TextBox ID="txtName" runat="server"></asp:TextBox>
                <asp:RequiredFieldValidator ID="valName" runat="server" ControlToValidate="txtName" ValidationGroup="Submit" ErrorMessage="*" ForeColor="Red"></asp:RequiredFieldValidator>
            </td>
        </tr>
        <tr>
            <td>
                Domain
            </td>
            <td>
                <asp:TextBox ID="txtDomain" runat="server"></asp:TextBox>
                <asp:RequiredFieldValidator ID="valDomain" runat="server" ControlToValidate="txtDomain" ValidationGroup="Submit" ErrorMessage="*" ForeColor="Red"></asp:RequiredFieldValidator>
            </td>
        </tr>
        <tr>
            <td>
                Logo Upload
            </td>
            <td>
                <asp:FileUpload ID="fuLogoImage" runat="server" />
                <asp:Button ID="btnUpload" runat="server" Text="Upload" OnClick="btnUpload_Click" />
                <br />
                <asp:Image ID="imgLogo" runat="server" />               
                <asp:Button ID="btnRemoveImage" runat="server" Text="Remove" OnClick="btnRemoveImage_Click" />
            </td>
        </tr> 
        <tr>
            <td>
                Inactive
            </td>
            <td>
                <asp:CheckBox ID="cbInactive" runat="server" />
            </td>
        </tr>
        <tr>
            <td colspan="2">
                <asp:Button ID="btnCreate" runat="server" Text="Create" OnClick="btnCreate_Click" ValidationGroup="Submit" />
            </td>
        </tr>
    </table>
</asp:Content>