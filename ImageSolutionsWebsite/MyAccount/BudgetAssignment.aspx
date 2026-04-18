<%@ Page Title="" Language="C#" MasterPageFile="~/MasterPage.Master" AutoEventWireup="true" CodeBehind="BudgetAssignment.aspx.cs" Inherits="ImageSolutionsWebsite.MyAccount.BudgetAssignment" %>
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
                        <div class="tab-pane fade show active" id="websites">
                            <div class="row">
                                <div class="col-sm-12 col-lg-12">
                                    <section class="tab-product m-0">
                                        <ul class="nav nav-tabs nav-material" id="top-tab" role="tablist">
                                            <li class="nav-item">
                                                <a class="nav-link active" id="top-1-tab" data-bs-toggle="tab"
                                                    href="#top-1" role="tab" aria-selected="true">
                                                    <i class="icofont icofont-ui-home"></i>Employee Credit Summary
                                                </a>
                                                <div class="material-border"></div>
                                            </li>
                                            <li class="nav-item">
                                                <a class="nav-link" id="top-2-tab" data-bs-toggle="tab"
                                                    href="#top-2" role="tab" aria-selected="false">
                                                    <i class="icofont icofont-man-in-glasses"></i>Orders
                                                </a>
                                                <div class="material-border"></div>
                                            </li>
                                            <li class="nav-item">
                                                <a class="nav-link" id="top-3-tab" data-bs-toggle="tab"
                                                    href="#top-3" role="tab" aria-selected="false">
                                                    <i class="icofont icofont-man-in-glasses"></i>Adjustments
                                                </a>
                                                <div class="material-border"></div>
                                            </li>
                                        </ul>
                                    </section><br />
                                    <div class="tab-content nav-material">
                                        <div class="tab-pane fade show active" id="top-1" role="tabpanel" aria-labelledby="top-1-tab">
                                            <div class="card dashboard-table mt-0">
                                                <div class="card-body table-responsive-sm">
                                                    <%--<div class="top-sec">
                                                        <h3>User Management</h3>
                                                    </div>--%>
                                                    <div class="table-responsive-xl">
                                                        <div class="theme-form">
                                                            <div class="form-row row">
                                                                <div class="col-md-12">
                                                                    <label>Employee Credit Name</label>
                                                                    <asp:TextBox ID="txtName" runat="server" Enabled="false" CssClass="form-control" placeholder="Budget name" required=""></asp:TextBox>
                                                                    <br />
                                                                </div>
                                                                <div class="col-md-12">
                                                                    <label>Start Date</label>
                                                                    <asp:TextBox ID="txtStartDate" runat="server" Enabled="false" CssClass="datepicker-here form-control digits" placeholder="start date" required=""></asp:TextBox>
                                                                    <br />
                                                                </div>
                                                                <div class="col-md-12">
                                                                    <label>End Date</label>
                                                                    <asp:TextBox ID="txtEndDate" runat="server" Enabled="false" CssClass="datepicker-here form-control digits" placeholder="end date" required=""></asp:TextBox>
                                                                    <br />
                                                                </div>
                                                                <div class="col-md-12">
                                                                    <label>Budget Amount</label>
                                                                    <asp:TextBox ID="txtBudgetAmount" runat="server" Enabled="false" CssClass="datepicker-here form-control digits" placeholder="budget amount" required=""></asp:TextBox>
                                                                    <br />
                                                                </div>
                                                                <div class="col-md-12">
                                                                    <label>Available Balance</label>
                                                                    <asp:TextBox ID="txtAvailableAmount" runat="server" Enabled="false" CssClass="datepicker-here form-control digits" placeholder="budget amount" required=""></asp:TextBox>
                                                                    <br />
                                                                </div>
                                                                <div class="col-md-12">
                                                                    <a id="aCancel" runat="server" href="/Admin/Budget.aspx" class="btn btn-sm btn-solid">Cancel</a>
                                                                </div>
                                                            </div>
                                                        </div>
                                                    </div>
                                                </div>
                                            </div>
                                        </div>
                                        <div class="tab-pane fade" id="top-2" role="tabpanel" aria-labelledby="top-2-tab">
                                            <div class="row">
                                                <div class="col-12">
                                                    <div class="card dashboard-table mt-0">
                                                        <div class="card-body table-responsive-sm">
                                                            <div class="table-responsive-xl">                                                                <asp:GridView ID="gvSalesOrders" runat="server" AutoGenerateColumns="False" DataKeyNames="SalesOrderID" CssClass="table cart-table wishlist-table" HeaderStyle-CssClass="table-head" Width="100%" 
                                                                    GridLines="None" AllowSorting="true" CellSpacing="0" CellPadding="0">
                                                                    <Columns>
                                                                        <asp:TemplateField HeaderText="Order Number" ItemStyle-HorizontalAlign="Center" HeaderStyle-HorizontalAlign="Center">
                                                                            <ItemTemplate>
                                                                                <%# string.Format("{0:c}", Eval("SalesOrderID"))%>
                                                                            </ItemTemplate>
                                                                        </asp:TemplateField>
                                                                        <asp:TemplateField HeaderText="Store" HeaderStyle-HorizontalAlign="Center" ItemStyle-HorizontalAlign="Center">
                                                                            <ItemTemplate>
                                                                                <%# Eval("Account.AccountName")%>
                                                                            </ItemTemplate>
                                                                        </asp:TemplateField>

                                                                        <asp:BoundField DataField="Status" HeaderText="Status" HeaderStyle-HorizontalAlign="Center" ItemStyle-HorizontalAlign="Center"/>
                                                                        
                                                                        <asp:CheckBoxField DataField="IsPendingApproval" HeaderText="Pending Approval" HeaderStyle-HorizontalAlign="Center" ItemStyle-HorizontalAlign="Center"/>
                                                                        <asp:TemplateField HeaderText="Total" HeaderStyle-HorizontalAlign="Center" ItemStyle-HorizontalAlign="Center">
                                                                            <ItemTemplate>
                                                                                <%# string.Format("{0:c}", Eval("Total"))%>
                                                                            </ItemTemplate>
                                                                        </asp:TemplateField>

                                                                        <asp:TemplateField HeaderText="Budget" HeaderStyle-HorizontalAlign="Center" ItemStyle-HorizontalAlign="Center">
                                                                            <ItemTemplate>
                                                                                <%# string.Format("{0:c}", Eval("BudgetApplied"))%>
                                                                            </ItemTemplate>
                                                                         </asp:TemplateField>

                                                                        <asp:TemplateField HeaderText="CreatedOn" HeaderStyle-HorizontalAlign="Center" ItemStyle-HorizontalAlign="Center">
                                                                            <ItemTemplate>
                                                                                <%# string.Format("{0:d}", Eval("CreatedOn"))%>
                                                                            </ItemTemplate>
                                                                         </asp:TemplateField>
                                                                        <asp:TemplateField HeaderText="VIEW">
                                                                            <ItemTemplate>
                                                                                <a href='/OrderConfirmation.aspx?SalesOrderID=<%# Eval("SalesOrderID") %>'>
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
                                        <div class="tab-pane fade" id="top-3" role="tabpanel" aria-labelledby="top-3-tab">
                                            <div class="row">
                                                <div class="col-12">
                                                    <div class="card dashboard-table mt-0">
                                                        <div class="card-body table-responsive-sm">
                                                            <div class="table-responsive-xl">                                                                <asp:GridView ID="gvBudgetAssignmentAdjustment" runat="server" AutoGenerateColumns="False" DataKeyNames="BudgetAssignmentAdjustmentID" CssClass="table cart-table wishlist-table" HeaderStyle-CssClass="table-head" Width="100%" 
                                                                    GridLines="None" AllowSorting="true" CellSpacing="0" CellPadding="0">
                                                                    <Columns>
                                                                        <asp:TemplateField HeaderText="Reason" ItemStyle-HorizontalAlign="Center" HeaderStyle-HorizontalAlign="Center">
                                                                            <ItemTemplate>
                                                                                <%# Eval("Reason")%>
                                                                            </ItemTemplate>
                                                                        </asp:TemplateField>
                                                                        <asp:TemplateField HeaderText="Amount" ItemStyle-HorizontalAlign="Center" HeaderStyle-HorizontalAlign="Center">
                                                                            <ItemTemplate>
                                                                                <%# string.Format("{0:c}", Eval("Amount"))%>
                                                                            </ItemTemplate>
                                                                        </asp:TemplateField>
                                                                        <asp:TemplateField HeaderText="CreatedOn" HeaderStyle-HorizontalAlign="Center" ItemStyle-HorizontalAlign="Center">
                                                                            <ItemTemplate>
                                                                                <%# string.Format("{0:d}", Eval("CreatedOn"))%>
                                                                            </ItemTemplate>
                                                                            </asp:TemplateField>
                                                                        <asp:TemplateField HeaderText="CreatedOn" HeaderStyle-HorizontalAlign="Center" ItemStyle-HorizontalAlign="Center">
                                                                            <ItemTemplate>
                                                                                <%# Eval("CreatedByUser.FirstName")%> <%# Eval("CreatedByUser.LastName")%>
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
                    </div>
                </div>
            </div>
        </div>
    </section>
    <!--  dashboard section end -->
</asp:Content>
