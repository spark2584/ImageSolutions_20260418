<%@ Page Language="C#" MasterPageFile="~/MasterPage.Master" AutoEventWireup="true" CodeBehind="AccountOverview.aspx.cs" Inherits="ImageSolutionsWebsite.Admin.AccountOverview" %>
<%@ Register Src="~/Control/AdminNavigation.ascx" TagPrefix="uc" TagName="AdminNavigation" %>

<%@ Register Src="~/Control/Pager.ascx" TagPrefix="uc1" TagName="Pager" %>

<asp:Content ID="cHead" ContentPlaceHolderID="head" runat="server">
</asp:Content>
<asp:Content ID="cHeader" ContentPlaceHolderID="cphHeader" runat="server">
</asp:Content>
<asp:Content ID="cBody" ContentPlaceHolderID="cphBody" runat="server">
    <!--  dashboard section start -->
    <section class="dashboard-section section-b-space user-dashboard-section">
        <div class="container">
            <div class="row">
                <uc:AdminNavigation runat="server" id="AdminNavigation" />
                <div class="col-lg-9">
                    <div class="faq-content tab-content" id="top-tabContent">
                        <div class="tab-pane fade show active" id="orders">
                            <div class="row">
                                <div class="col-12">
                                    <div class="card dashboard-table mt-0">
                                        <div class="card-body table-responsive-sm">
                                            <div class="top-sec">
                                                <h3>Sub Account Management</h3> 
                                                <a href="/admin/account.aspx" class="btn btn-sm btn-solid" id="aAddNew" runat="server">+ add new</a>
                                                <asp:LinkButton ID="btnExport" runat="server" class="btn btn-sm btn-solid" OnClick="btnExport_Click">export</asp:LinkButton>
                                                <%--<asp:LinkButton ID="btnExport" runat="server" CssClass="btn btn-sm btn-solid" OnClick="btnExport_Click">export</asp:LinkButton>--%>
                                            </div>
                                            <div class="top-sec" id="filter" runat="server" visible="true">
                                                Email:&nbsp;<asp:TextBox ID="txtEmail" runat="server" placeholder="email" required="" AutoPostBack="true" OnTextChanged="txtFilter_TextChanged"></asp:TextBox>&nbsp;
                                            </div>
                                            <div class="top-sec" id="filteraddress" runat="server" visible="false">
                                                Sub Account Name:&nbsp;<asp:TextBox ID="txtStoreName" runat="server" placeholder="sub account name" required="" AutoPostBack="true" OnTextChanged="txtFilter_TextChanged"></asp:TextBox>&nbsp;
                                                Street:&nbsp;<asp:TextBox ID="txtStreet" runat="server" placeholder="street" required="" AutoPostBack="true" OnTextChanged="txtFilter_TextChanged"></asp:TextBox>&nbsp;
                                                City:&nbsp;<asp:TextBox ID="txtCity" runat="server" placeholder="city" required="" AutoPostBack="true" OnTextChanged="txtFilter_TextChanged"></asp:TextBox>&nbsp;
                                                State:&nbsp;<asp:TextBox ID="txtState" runat="server" placeholder="state" required="" AutoPostBack="true" OnTextChanged="txtFilter_TextChanged"></asp:TextBox>&nbsp;
                                                Zip:&nbsp;<asp:TextBox ID="txtZip" runat="server" placeholder="zip" required="" AutoPostBack="true" OnTextChanged="txtFilter_TextChanged"></asp:TextBox>&nbsp;
                                            </div>
                                            <div class="table-responsive-xl">
                                                <asp:GridView runat="server" ID="gvAccounts" AutoGenerateColumns="False" CssClass="table cart-table order-table" HeaderStyle-CssClass="table-head" GridLines="None" Width="100%" AllowSorting="true" CellSpacing="0" CellPadding="0">
                                                    <Columns>
                                                        <asp:BoundField DataField="EmailAddress" HeaderText="Email" HeaderStyle-HorizontalAlign="Left" ItemStyle-HorizontalAlign="Left" />
                                                        <asp:BoundField DataField="AccountNamePath" HeaderText="Parent Account" HeaderStyle-HorizontalAlign="Left" ItemStyle-HorizontalAlign="Left" />
                                                        <asp:TemplateField HeaderText="Shipping Address" HeaderStyle-HorizontalAlign="Center" ItemStyle-HorizontalAlign="Center">
                                                            <ItemTemplate>
                                                                <%# Eval("DefaultShippingAddressBook.GetHTML") %>
                                                            </ItemTemplate>
                                                        </asp:TemplateField>
                                                        <%--<asp:TemplateField HeaderText="Created By" HeaderStyle-HorizontalAlign="Center" ItemStyle-HorizontalAlign="Center">
                                                            <ItemTemplate>
                                                                <%# Eval("CreatedByUser.FullName") %>
                                                            </ItemTemplate>
                                                        </asp:TemplateField>--%>
                                                        <asp:TemplateField HeaderText="Pending Approval" HeaderStyle-HorizontalAlign="Center" ItemStyle-HorizontalAlign="Center">
                                                            <ItemTemplate>
                                                                <asp:CheckBox ID="chkIsPendingApproval" runat="server" Checked='<%# Eval("IsPendingApproval") %>' Enabled="false" /> 
                                                            </ItemTemplate>
                                                        </asp:TemplateField>
                                                        <asp:TemplateField HeaderText="Created Date" HeaderStyle-HorizontalAlign="Center" ItemStyle-HorizontalAlign="Center">
                                                            <ItemTemplate>
                                                                <%# Eval("CreatedOn") %>
                                                            </ItemTemplate>
                                                        </asp:TemplateField>
                                                        <asp:TemplateField HeaderText="View" HeaderStyle-HorizontalAlign="Center" ItemStyle-HorizontalAlign="Center" ItemStyle-Width="20%">
                                                            <ItemTemplate>
                                                                 <a href='/admin/account.aspx?id=<%# Eval("AccountID") %>'>
                                                                    <i class="fa fa-eye text-theme"></i>
                                                                </a>
                                                            </ItemTemplate>
                                                        </asp:TemplateField>
                                                    </Columns>
                                                </asp:GridView>
                                                <uc1:Pager runat="server" ID="ucPager" PagingMode="Redirect" PageSize="20" PagingRecordText="Sub Accounts" />
                                            </div>
                                        </div>
                                    </div>
                                </div>
                            </div>
                        </div>
                    </div>
                </div>
            </div>
        </div>
    </section>
    <!--  dashboard section end -->
</asp:Content>

