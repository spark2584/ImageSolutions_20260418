<%@ Page Title="" Language="C#" MasterPageFile="~/MasterPage.Master" AutoEventWireup="true" CodeBehind="Orders.aspx.cs" Inherits="ImageSolutionsWebsite.MyAccount.Orders" %>
<%@ Register Src="~/Control/MyAccountNavigation.ascx" TagPrefix="uc1" TagName="MyAccountNavigation" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="cphHeader" runat="server">
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="cphBody" runat="server">
    <!--  dashboard section start -->
    <section class="dashboard-section section-b-space user-dashboard-section">
        <div class="container">
            <div class="row">
                <uc1:MyAccountNavigation runat="server" id="MyAccountNavigation" />
                <div class="col-lg-9">
                    <div class="faq-content tab-content" id="top-tabContent">
                        <div class="tab-pane fade show active" id="orders">
                            <div class="row">
                                <div class="col-12">
                                    <div class="card dashboard-table mt-0">
                                        <div class="card-body table-responsive-sm">
                                            <div class="top-sec">
                                                <h3>My Orders</h3>
                                            </div>
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
                                                    <asp:TemplateField HeaderText="Store" HeaderStyle-HorizontalAlign="Center" ItemStyle-HorizontalAlign="Center">
                                                        <ItemTemplate>
                                                            <%# Eval("Account.AccountName")%>
                                                        </ItemTemplate>
                                                    </asp:TemplateField>
                                                    <asp:CheckBoxField DataField="IsPendingApproval" HeaderText="Pending Approval" HeaderStyle-HorizontalAlign="Center" ItemStyle-HorizontalAlign="Center"/>
                                                    <asp:TemplateField HeaderText="Status" HeaderStyle-HorizontalAlign="Center" ItemStyle-HorizontalAlign="Center">
                                                        <ItemTemplate>
                                                            <%# Eval("Status")%>
                                                        </ItemTemplate>
                                                    </asp:TemplateField>
                                                    <asp:TemplateField HeaderText="Total" HeaderStyle-HorizontalAlign="Center" ItemStyle-HorizontalAlign="Center">
                                                        <ItemTemplate>
                                                            <%# string.Format("{0:c}", Eval("Total"))%>
                                                        </ItemTemplate>
                                                     </asp:TemplateField>
<%--                                                    <asp:TemplateField HeaderText="CreatedOn" HeaderStyle-HorizontalAlign="Center" ItemStyle-HorizontalAlign="Center">
                                                        <ItemTemplate>
                                                            <%# string.Format("{0:d}", Eval("CreatedOn"))%>
                                                        </ItemTemplate>
                                                     </asp:TemplateField>--%>
                                                    <asp:TemplateField HeaderText="View">
                                                        <ItemTemplate>
                                                            <a href='/OrderConfirmation.aspx?SalesOrderID=<%# Eval("SalesOrderID") %>'>
                                                                <i class="fa fa-eye text-theme"></i>
                                                            </a>
                                                        </ItemTemplate>
                                                    </asp:TemplateField>

                                                    <asp:TemplateField HeaderText="Order File">
                                                        <ItemTemplate>
                                                            <asp:HiddenField ID="hfSalesOrderID" runat="server" Value='<%# Eval("SalesOrderID")%>'/>
                                                            <asp:HyperLink ID="lnkOrderFile" runat="server" Target="_blank" NavigateUrl='<%#Eval("OrderFilePath") %>'><i class="fa fa-file text-theme"></i></asp:HyperLink>
                                                            <%--<a href='<%#Eval("OrderFilePath") %>' target="_blank"><i class="fa fa-file text-theme"></i></a>--%>
                                                        </ItemTemplate>
                                                    </asp:TemplateField>

                                                    <asp:TemplateField HeaderText="Invoice File">
                                                        <ItemTemplate>
                                                            <asp:HyperLink ID="lnkInvoiceFile" runat="server" Target="_blank" NavigateUrl='<%#Eval("InvoiceFilePath") %>'><i class="fa fa-file text-theme"></i></asp:HyperLink>
                                                        </ItemTemplate>
                                                    </asp:TemplateField>

                                                    
                                                    <asp:TemplateField HeaderText="Return">
                                                        <ItemTemplate>
                                                            <asp:HyperLink ID="lnkReturnPath" runat="server" Targe="_blank"><i class="fa fa-envelope text-theme"></i></asp:HyperLink>
                                        <%--                    <a href='/ReturnAuthorization.aspx?SalesOrderID=<%# Eval("SalesOrderID") %>'>
                                                                <i class="fa fa-envelope text-theme"></i>
                                                            </a>--%>
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
    </section>
    <!--  dashboard section end -->
</asp:Content>
