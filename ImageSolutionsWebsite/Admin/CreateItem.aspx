<%@ Page Language="C#" MasterPageFile="~/MasterPage.Master" AutoEventWireup="true" CodeBehind="CreateItem.aspx.cs" Inherits="ImageSolutionsWebsite.Admin.CreateItem" %>
<%@ Register Src="~/Control/AdminHeader.ascx" TagPrefix="uc" TagName="AdminHeader" %>

<asp:Content ID="cHead" ContentPlaceHolderID="head" runat="server">
</asp:Content>
<asp:Content ID="cHeader" ContentPlaceHolderID="cphHeader" runat="server">
</asp:Content>
<asp:Content ID="cBody" ContentPlaceHolderID="cphBody" runat="server">
    <uc:AdminHeader id="AdminHeader" runat="server"></uc:AdminHeader>

    <table>
        <tr>
            <td colspan="2">
                <h1>Create Item</h1>
                <h2><asp:Label ID="lblWebsite" runat="server"></asp:Label></h2>
            </td>
        </tr>
        <tr>
            <td colspan="2">
                <asp:Button ID="btnCancel" runat="server" Text="Back" OnClick="btnCancel_Click" />
            </td>
        </tr>   
        <tr>
            <td>
                Item Number
            </td>
            <td>
                <asp:TextBox ID="txtItemNumber" runat="server"></asp:TextBox>
                <asp:RequiredFieldValidator ID="valItemNumber" runat="server" ControlToValidate="txtItemNumber" ValidationGroup="Submit" ErrorMessage="*" ForeColor="Red"></asp:RequiredFieldValidator>
            </td>
        </tr>
        <tr>
            <td>
                Item Name
            </td>
            <td>
                <asp:TextBox ID="txtItemName" runat="server"></asp:TextBox>
                <asp:RequiredFieldValidator ID="valItemName" runat="server" ControlToValidate="txtItemName" ValidationGroup="Submit" ErrorMessage="*" ForeColor="Red"></asp:RequiredFieldValidator>
            </td>
        </tr>   
        <tr>
            <td>
                Image Upload
            </td>
            <td>
                <asp:FileUpload ID="fuItemImage" runat="server" />
                <asp:Button ID="btnUpload" runat="server" Text="Upload" OnClick="btnUpload_Click" />
                <br />
                <asp:Image ID="imgItem" runat="server" />
                <asp:Button ID="btnRemoveImage" runat="server" Text="Remove" OnClick="btnRemoveImage_Click" />
            </td>
        </tr>  
        <tr>
            <td>
                Item Type
            </td>
            <td>
                <asp:DropDownList ID="ddlItemType" runat="server">
                    <asp:ListItem Selected="True" Value="_nonInventoryItem">Non-Inventory</asp:ListItem>
                    <asp:ListItem Value="_inventoryItem">Inventory</asp:ListItem>
                </asp:DropDownList>
                <asp:RequiredFieldValidator ID="valItemType" runat="server" ControlToValidate="ddlItemType" ValidationGroup="Submit" ErrorMessage="*" ForeColor="Red"></asp:RequiredFieldValidator>
            </td>
        </tr>
        <tr>
            <td>
                Store Display Name
            </td>
            <td>
                <asp:TextBox ID="txtStoreDisplayName" runat="server"></asp:TextBox>
                <asp:RequiredFieldValidator ID="valStoreDisplayName" runat="server" ControlToValidate="txtStoreDisplayName" ValidationGroup="Submit" ErrorMessage="*" ForeColor="Red"></asp:RequiredFieldValidator>
            </td>
        </tr>
        <tr>
            <td>
                Sales Description
            </td>
            <td>
                <asp:TextBox ID="txtSalesDescription" runat="server"></asp:TextBox>
            </td>
        </tr>
        <tr>
            <td>
                Base Price
            </td>
            <td>
                <asp:TextBox ID="txtBasePrice" runat="server" onKeyPress="if (event.keyCode!=46 && event.keyCode!=45 && (event.keyCode<48 || event.keyCode>57)) event.returnValue=false"></asp:TextBox>
            </td>
        </tr>
        <tr>
            <td>
                Purchase Price
            </td>
            <td>
                <asp:TextBox ID="txtPurchasePrice" runat="server" onKeyPress="if (event.keyCode!=46 && event.keyCode!=45 && (event.keyCode<48 || event.keyCode>57)) event.returnValue=false"></asp:TextBox>
            </td>
        </tr
        <tr>
            <td>
                Is Online
            </td>
            <td>
                <asp:CheckBox ID="cbIsOnline" runat="server" />
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
                <asp:Button ID="btnCreate" runat="server" Text="Create" OnClick="btnCreate_Click" ValidationGroup="Submit"/>
            </td>
        </tr>        
    </table>
</asp:Content>
