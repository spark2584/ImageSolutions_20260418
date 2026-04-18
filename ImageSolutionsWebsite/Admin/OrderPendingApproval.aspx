<%@ Page Language="C#" MasterPageFile="~/MasterPage.Master" AutoEventWireup="true" CodeBehind="OrderPendingApproval.aspx.cs" Inherits="ImageSolutionsWebsite.Admin.OrderPendingApproval" %>
<%@ Register Src="~/Control/AdminNavigation.ascx" TagPrefix="uc" TagName="AdminNavigation" %>

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
                                                <h3>Order Approval</h3>
                                            </div>

                                            <div class="top-sec">
                                                <div class="col-md-12">
                                                    <div>
                                                        Top 100:&nbsp;&nbsp;<asp:CheckBox ID="chkTop100" runat="server" Checked="true" AutoPostBack="true" OnCheckedChanged="chkTop100_CheckedChanged"/>
                                                    </div>
                                                </div>
                                            </div>

                                            <div class="table-responsive-xl">
                                                <asp:GridView ID="gvSalesOrders" runat="server" AutoGenerateColumns="False" DataKeyNames="SalesOrderID" CssClass="table cart-table wishlist-table" HeaderStyle-CssClass="table-head" Width="100%" 
                                                    GridLines="None" AllowSorting="true" CellSpacing="0" CellPadding="0" OnRowDataBound="gvSalesOrders_RowDataBound">
                                                    <Columns>
                                                        <asp:TemplateField HeaderText="Order Number" ItemStyle-HorizontalAlign="Center" HeaderStyle-HorizontalAlign="Center">
                                                            <ItemTemplate>
                                                                <%# string.Format("{0:c}", Eval("SalesOrderID"))%>
                                                            </ItemTemplate>
                                                        </asp:TemplateField>
                                                        <asp:TemplateField HeaderText="Transaction Date" HeaderStyle-HorizontalAlign="Center" ItemStyle-HorizontalAlign="Center">
                                                            <ItemTemplate>
                                                                <%# string.Format("{0:d}", Eval("TransactionDate"))%>
                                                            </ItemTemplate>
                                                            </asp:TemplateField>
                                                        <asp:TemplateField HeaderText="Account" ItemStyle-HorizontalAlign="Center" HeaderStyle-HorizontalAlign="Center">
                                                            <ItemTemplate>
                                                                <%# Eval("Account.AccountName")%>
                                                            </ItemTemplate>
                                                        </asp:TemplateField>
                                                        <asp:TemplateField HeaderText="User" HeaderStyle-HorizontalAlign="Center" ItemStyle-HorizontalAlign="Center">
                                                            <ItemTemplate>
                                                                <%# Eval("UserWebsite.Description")%>
                                                            </ItemTemplate>
                                                        </asp:TemplateField>
                                                        <asp:TemplateField HeaderText="Pending Order Approval" HeaderStyle-HorizontalAlign="Center"  ItemStyle-HorizontalAlign="Center">
                                                            <ItemTemplate>
                                                                <asp:CheckBox ID="cbPendingOrderApproval" runat="server" Enabled="false"/>
                                                            </ItemTemplate>
                                                        </asp:TemplateField>
                                                        <asp:TemplateField HeaderText="Pending Personialization Approval" HeaderStyle-HorizontalAlign="Center"  ItemStyle-HorizontalAlign="Center">
                                                            <ItemTemplate>
                                                                <asp:CheckBox ID="cbPendingPersonalizationApproval" runat="server" Enabled="false"/>
                                                            </ItemTemplate>
                                                        </asp:TemplateField>
                                                        <asp:TemplateField HeaderText="Order Total" HeaderStyle-HorizontalAlign="Center" ItemStyle-HorizontalAlign="Center">
                                                            <ItemTemplate>
                                                                <%# string.Format("{0:c}", Eval("Total"))%>
                                                            </ItemTemplate>
                                                        </asp:TemplateField>
                                                        <asp:TemplateField HeaderText="Credit Applied" HeaderStyle-HorizontalAlign="Center" ItemStyle-HorizontalAlign="Center">
                                                            <ItemTemplate>
                                                                <%# string.Format("{0:c}", Eval("BudgetApplied"))%>
                                                            </ItemTemplate>
                                                        </asp:TemplateField>
                                                        <asp:TemplateField HeaderText="Balance" HeaderStyle-HorizontalAlign="Center" ItemStyle-HorizontalAlign="Center">
                                                            <ItemTemplate>
                                                                <%# string.Format("{0:c}", Eval("Balance") )%>
                                                            </ItemTemplate>
                                                        </asp:TemplateField>
                                                        <asp:TemplateField HeaderText="VIEW">
                                                            <ItemTemplate>
                                                                <a href='/admin/Order.aspx?id=<%# Eval("SalesOrderID") %>'>
                                                                    <i class="fa fa-eye text-theme"></i>
                                                                </a>
                                                            </ItemTemplate>
                                                        </asp:TemplateField>
                                                    </Columns>
                                                </asp:GridView>
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
