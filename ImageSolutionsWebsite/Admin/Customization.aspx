<%@ Page Language="C#" MasterPageFile="~/MasterPage.Master" AutoEventWireup="true" CodeBehind="Customization.aspx.cs" Inherits="ImageSolutionsWebsite.Admin.Customization" %>
<%@ Register Src="~/Control/AdminHeader.ascx" TagPrefix="uc" TagName="AdminHeader" %>

<asp:Content ID="cHead" ContentPlaceHolderID="head" runat="server">
</asp:Content>
<asp:Content ID="cHeader" ContentPlaceHolderID="cphHeader" runat="server">
</asp:Content>
<asp:Content ID="cBody" ContentPlaceHolderID="cphBody" runat="server">
    <uc:AdminHeader id="AdminHeader" runat="server"></uc:AdminHeader>

    <table style="width:100%; border:solid">
        <tr>
            <td colspan="2">
                <h1><asp:Label ID="lblHeader" runat="server"></asp:Label></h1>
            </td>
        </tr>
        <tr>
            <td>
                <asp:Button ID="btnCancel" runat="server" Text="Back" OnClick="btnCancel_Click" />
            </td>
            <td style="text-align:right">
                <asp:Button ID="btnDelete" runat="server" Text="Delete Customization" OnClick="btnDelete_Click" />
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
            <td colspan="2">
                <asp:Button ID="btnEdit" runat="server" Text="Edit" OnClick="btnEdit_Click" ValidationGroup="Submit"/>
            </td>
        </tr>        
    </table>
    <br />
    <br />
    <table style="width:100%; border:solid">
        <tr>
            <td>
                Customization Fields
            </td>
        </tr>
        <tr>
            <td>
                <asp:Button ID="btnAddField" runat="server" Text="Add Field" OnClick="btnAddField_Click" />
            </td>
        </tr>
        <tr>
            <td>
                <asp:GridView runat="server" ID="gvCustomizationField" AutoGenerateColumns="False" Width="100%" AllowSorting="true" CellSpacing="0" CellPadding="0"
                    OnRowCommand="gvCustomizationField_RowCommand">
                    <Columns>
                        <asp:BoundField DataField="CustomizationFieldName" HeaderText="Field Name" HeaderStyle-HorizontalAlign="Left" ItemStyle-HorizontalAlign="Left" ItemStyle-Width="80%" />
                        <asp:TemplateField HeaderStyle-HorizontalAlign="Center" ItemStyle-HorizontalAlign="Center" ItemStyle-Width="20%">
                            <ItemTemplate>
                                <asp:Button runat="server" ID="btnUpdate" Text="Update" CommandName="LineUpdate" CommandArgument='<%# Eval("CustomizationFieldID") %>' CausesValidation="false" Width="100%"/>
                            </ItemTemplate>
                        </asp:TemplateField>
                    </Columns>
                </asp:GridView>
            </td>    
        </tr>
    </table>
</asp:Content>