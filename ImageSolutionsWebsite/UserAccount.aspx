<%@ Page Title="" Language="C#" MasterPageFile="~/MasterPage.Master" AutoEventWireup="true" CodeBehind="UserAccount.aspx.cs" Inherits="ImageSolutionsWebsite.UserAccount" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="cphHeader" runat="server">
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="cphBody" runat="server">
    <%--<section class="cart-section section-b-space">--%>
        <div class="container-fluid">
            <div class="card">
                <div class="card-header">
                    <p><asp:Label ID="lblMessage" runat="server" EnableViewState="false" ForeColor="Red"></asp:Label></p>
                </div>
                <div class="card-body">
                    <div class="table-responsive table-desi" style="text-align:center">
                        <asp:GridView ID="gvUserAccounts" runat="server" AutoGenerateColumns="False" DataKeyNames="UserAccountID" CssClass="table" onrowupdating="gvUserAccounts_RowUpdating" Width="100%" 
                            GridLines="None" AllowSorting="true" CellSpacing="0" CellPadding="0" onsorting="gvUserAccounts_Sorting">
                            <Columns>
                                <asp:TemplateField HeaderText="Website" HeaderStyle-HorizontalAlign="Center" ItemStyle-HorizontalAlign="Center">
                                    <ItemTemplate>
                                        <%# Eval("WebsiteGroup.Website.Name")%>                                
                                    </ItemTemplate>
                                </asp:TemplateField>
                                <asp:TemplateField HeaderText="Assigned Account" HeaderStyle-HorizontalAlign="Center" ItemStyle-HorizontalAlign="Center">
                                    <ItemTemplate>
                                        <%# Eval("Account.AccountName")%>
                                    </ItemTemplate>
                                </asp:TemplateField>
                                <asp:TemplateField HeaderText="Assigned Group" HeaderStyle-HorizontalAlign="Center" ItemStyle-HorizontalAlign="Center">
                                    <ItemTemplate>
                                        <asp:LinkButton ID="lbnUserAccount" runat="server" ForeColor="Blue" CommandArgument='<%# Eval("UserAccountID") %>' CommandName="Update"><%# Eval("WebsiteGroup.GroupName") %></asp:LinkButton>
                                    </ItemTemplate>
                                </asp:TemplateField>
                            </Columns>
                        </asp:GridView>
                    </div>
                </div>
            </div>
        </div>
   <%-- </section>--%>
</asp:Content>
