<%@ Page Title="" Language="C#" MasterPageFile="~/MasterPage.Master" AutoEventWireup="true" CodeBehind="Budgets.aspx.cs" Inherits="ImageSolutionsWebsite.MyAccount.Budgets" %>
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
                        <div class="tab-pane fade show active" id="wishlist">
                            <div class="row">
                                <div class="col-12">
                                    <div class="card dashboard-table mt-0">
                                        <div class="card-body table-responsive-sm">
                                            <div class="card-body table-responsive-sm">
                                                <div class="top-sec">
                                                    <h3>My Employee Credits</h3>
                                                </div>
                                                <asp:GridView ID="gvBudgetAssignments" runat="server" AutoGenerateColumns="False" DataKeyNames="BudgetAssignmentID" CssClass="table cart-table wishlist-table" HeaderStyle-CssClass="table-head" Width="100%" 
                                                    GridLines="None" AllowSorting="true" CellSpacing="0" CellPadding="0">
                                                    <Columns>
                                                        <asp:TemplateField HeaderText="Website" HeaderStyle-HorizontalAlign="Center" ItemStyle-HorizontalAlign="Center">
                                                            <ItemTemplate>
                                                                <%# Eval("BudgetAssignment.Budget.Website.Name")%>
                                                            </ItemTemplate>
                                                         </asp:TemplateField>
                                                        <asp:TemplateField HeaderText="Employee Credit Name" ItemStyle-HorizontalAlign="Center" HeaderStyle-HorizontalAlign="Center">
                                                            <ItemTemplate>
                                                                <%# Eval("BudgetAssignment.Budget.BudgetName")%>
                                                            </ItemTemplate>
                                                        </asp:TemplateField>
                                                        <asp:TemplateField HeaderText="Credit Amount" ItemStyle-HorizontalAlign="Center" HeaderStyle-HorizontalAlign="Center">
                                                            <ItemTemplate>
                                                                <%# string.Format("{0:c}", Eval("BudgetAssignment.Budget.BudgetAmount"))%>
                                                            </ItemTemplate>
                                                        </asp:TemplateField>
                                                        <asp:TemplateField HeaderText="Available Amount" ItemStyle-HorizontalAlign="Center" HeaderStyle-HorizontalAlign="Center">
                                                            <ItemTemplate>
                                                                <%# string.Format("{0:c}", Eval("Balance"))%>
                                                            </ItemTemplate>
                                                        </asp:TemplateField>
                                                        <asp:TemplateField HeaderText="Pending" ItemStyle-HorizontalAlign="Center" HeaderStyle-HorizontalAlign="Center">
                                                            <ItemTemplate>
                                                                <%# string.Format("{0:c}", Eval("PendingAmount"))%>
                                                            </ItemTemplate>
                                                        </asp:TemplateField>
                                                        <asp:TemplateField HeaderText="Created On" HeaderStyle-HorizontalAlign="Center" ItemStyle-HorizontalAlign="Center">
                                                            <ItemTemplate>
                                                                <%# string.Format("{0:d}", Eval("BudgetAssignment.CreatedOn"))%>
                                                            </ItemTemplate>
                                                         </asp:TemplateField>
                                                        <asp:TemplateField HeaderText="VIEW">
                                                            <ItemTemplate>
                                                                <a href='/MyAccount/BudgetAssignment.aspx?id=<%# Eval("BudgetAssignmentID") %>'>
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
