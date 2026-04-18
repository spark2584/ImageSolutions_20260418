<%@ Page Language="C#" MasterPageFile="~/MasterPage.Master" AutoEventWireup="true" CodeBehind="Budget.aspx.cs" Inherits="ImageSolutionsWebsite.Admin.Budget" %>
<%@ Register Src="~/Control/AdminNavigation.ascx" TagPrefix="uc" TagName="AdminNavigation" %>
<%@ Register Src="~/Control/Pager.ascx" TagPrefix="uc1" TagName="Pager" %>
<%@ Register src="~/Control/UserWebsiteSearchModal.ascx" tagname="UserWebsiteSearchModal" tagprefix="uc2"  %>

<asp:Content ID="cHead" ContentPlaceHolderID="head" runat="server">
</asp:Content>
<asp:Content ID="cHeader" ContentPlaceHolderID="cphHeader" runat="server">
    <%--<!-- Datepicker css-->
    <link rel="stylesheet" type="text/css" href="/assets/css/vendors/date-picker.css">

    <!-- App css-->
    <link rel="stylesheet" type="text/css" href="/assets/css/style.css">

    <!--Datepicker jquery-->
    <script src="/assets/js/datepicker/datepicker.js"></script>
    <script src="/assets/js/datepicker/datepicker.en.js"></script>
    <script src="/assets/js/datepicker/datepicker.custom.js"></script>--%>
</asp:Content>
<asp:Content ID="cBody" ContentPlaceHolderID="cphBody" runat="server">
    <!--  dashboard section start -->
    <section class="dashboard-section section-b-space user-dashboard-section">
        <div class="container">
            <div class="row">
                <uc:AdminNavigation runat="server" id="AdminNavigation" />
                <div class="col-lg-9">
                    <div class="faq-content tab-content" id="top-tabContent">
                        <div class="tab-pane fade show active" id="websites">
                            <div class="row">
                                <div class="col-sm-12 col-lg-12">
                                    <section class="tab-product m-0">
                                        <ul class="nav nav-tabs nav-material" id="top-tab" role="tablist">
                                            <li class="nav-item">
                                                <a class="nav-link active" id="top_1_tab" runat="server" data-bs-toggle="tab"
                                                    href="#cphBody_top_1" role="tab" aria-selected="true">
                                                    <i class="icofont icofont-ui-home"></i><asp:Label ID="lblAlias" runat="server" Text="Employee Credit"></asp:Label>
                                                </a>
                                                <div class="material-border"></div>
                                            </li>
                                            <li class="nav-item" visible="false">
                                                <a class="nav-link" id="top_2_tab" runat="server" data-bs-toggle="tab"
                                                    href="#cphBody_top_2" role="tab" aria-selected="false">
                                                    <i class="icofont icofont-man-in-glasses"></i>Groups
                                                </a>
                                                <div class="material-border"></div>
                                            </li>
                                            <li class="nav-item">
                                                <a class="nav-link" id="top_3_tab" runat="server" data-bs-toggle="tab"
                                                    href="#cphBody_top_3" role="tab" aria-selected="false">
                                                    <i class="icofont icofont-man-in-glasses"></i>Employees
                                                </a>
                                                <div class="material-border"></div>
                                            </li>
                                            <li class="nav-item">
                                                <a class="nav-link" id="top_4_tab" runat="server" data-bs-toggle="tab"
                                                    href="#cphBody_top_4" role="tab" aria-selected="false">
                                                    <i class="icofont icofont-man-in-glasses"></i>Balances
                                                </a>
                                                <div class="material-border"></div>
                                            </li>
                                            <li class="nav-item">
                                                <a class="nav-link" id="top_5_tab" runat="server" data-bs-toggle="tab"
                                                    href="#cphBody_top_5" role="tab" aria-selected="false">
                                                    <i class="icofont icofont-man-in-glasses"></i>Orders
                                                </a>
                                                <div class="material-border"></div>
                                            </li>
                                            <li class="nav-item">
                                                <a class="nav-link" id="top_6_tab" runat="server" data-bs-toggle="tab"
                                                    href="#cphBody_top_6" role="tab" aria-selected="false">
                                                    <i class="icofont icofont-man-in-glasses"></i>Adjustments
                                                </a>
                                                <div class="material-border"></div>
                                            </li>
                                        </ul>
                                    </section><br />
                                    <div class="tab-content nav-material">
                                        <div class="tab-pane fade show active" id="top_1" runat="server" role="tabpanel" aria-labelledby="top_1_tab">
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
                                                                    <asp:TextBox ID="txtName" runat="server" CssClass="form-control" placeholder="Budget name" required=""></asp:TextBox>
                                                                    <asp:RequiredFieldValidator ID="RequiredFieldValidator1" runat="server" ControlToValidate="txtName" ValidationGroup="Submit" ErrorMessage="*" ForeColor="Red"></asp:RequiredFieldValidator>
                                                                </div>
                                                                <div class="col-md-12">
                                                                    <label>Start Date</label>
                                                                    <asp:TextBox ID="txtStartDate" runat="server" CssClass="datepicker-here form-control digits" placeholder="start date" required=""></asp:TextBox>
                                                                    <asp:RequiredFieldValidator ID="RequiredFieldValidator2" runat="server" ControlToValidate="txtStartDate" ValidationGroup="Submit" ErrorMessage="*" ForeColor="Red"></asp:RequiredFieldValidator>
                                                                </div>
                                                                <div class="col-md-12">
                                                                    <label>End Date</label>
                                                                    <asp:TextBox ID="txtEndDate" runat="server" CssClass="datepicker-here form-control digits" placeholder="end date" required=""></asp:TextBox>
                                                                    <asp:RequiredFieldValidator ID="RequiredFieldValidator3" runat="server" ControlToValidate="txtEndDate" ValidationGroup="Submit" ErrorMessage="*" ForeColor="Red"></asp:RequiredFieldValidator>
                                                                </div>
                                                                <div class="col-md-12">
                                                                    <label>Division</label>
                                                                    <asp:TextBox ID="txtDivision" runat="server" CssClass="form-control" placeholder="division"></asp:TextBox>
                                                                    <br />
                                                                </div>
                                                                <div class="col-md-12">
                                                                    <label>Credit Amount</label>
                                                                    <asp:TextBox ID="txtBudgetAmount" runat="server" CssClass="datepicker-here form-control digits" placeholder="budget amount" required=""></asp:TextBox>
                                                                    <asp:RequiredFieldValidator ID="RequiredFieldValidator4" runat="server" ControlToValidate="txtBudgetAmount" ValidationGroup="Submit" ErrorMessage="*" ForeColor="Red"></asp:RequiredFieldValidator>
                                                                </div>
                                                                <div class="col-md-12">
                                                                    <label>Include Shipping and Taxes</label>
                                                                    <asp:CheckBox ID="cbIncludeShippingAndTaxes" runat="server" Checked="true" OnCheckedChanged="cbIncludeShippingAndTaxes_CheckedChanged" AutoPostBack="true"/>
                                                                </div>

                                                                <div class="col-md-12">
                                                                    <label>Display Shipping</label>
                                                                    <asp:CheckBox ID="cbDisplayShipping" runat="server" />
                                                                </div>
                                                                <div class="col-md-12">
                                                                    <label>Tax Non-Budget Amount</label>
                                                                    <asp:CheckBox ID="cbTaxNonBudgetAmount" runat="server" />
                                                                </div>

                                                                <div class="col-md-12">
                                                                    <label>Default Shipping</label>&nbsp;&nbsp;
                                                                    <asp:DropDownList ID="ddlWebsiteShippingService" runat="server" DataTextField="Description" DataValueField="WebsiteShippingServiceID"></asp:DropDownList>
                                                                </div>
                                                                <div class="col-md-12">
                                                                    <label>Allow Over Budget</label>
                                                                    <asp:CheckBox ID="cbAllowOverBudget" runat="server" />
                                                                </div>
                                                                <div class="col-md-12">
                                                                    <label>Exclude No Amount Budget</label>
                                                                    <asp:CheckBox ID="cbExcludeNoAmountBudget" runat="server" />
                                                                </div>
                                                                <div class="col-md-12">
                                                                    <label>Payment Term</label>&nbsp;&nbsp;
                                                                    <asp:DropDownList ID="ddlPaymentTerm" runat="server" DataTextField="Description" DataValueField="PaymentTermID"></asp:DropDownList>
                                                                </div>
                                                                <div class="col-md-12">
                                                                    <label>Over Budget Approver</label>                                                                    
                                                                </div>
                                                                <div class="col-md-9">
                                                                    <asp:DropDownList ID="ddlUserWebsite" runat="server" DataTextField="Description" DataValueField="UserWebsiteID" CssClass="form-control" Visible="false"></asp:DropDownList>
                                                                    <asp:TextBox ID="txtUserWebsite" runat="server" CssClass="form-control" Enabled="false"></asp:TextBox>
                                                                    <asp:HiddenField ID="hfUserWebsiteID" runat="server" />
                                                                </div>
                                                                <div class="col-md-3">
                                                                    <asp:LinkButton ID="btnUserWebsiteSearch" runat="server" OnClick="btnUserWebsiteSearch_Click"><i class="ti-search" style="font-size:large"></i></asp:LinkButton>
                                                                    <asp:LinkButton ID="btnUserWebsiteRemove" runat="server" OnClick="btnUserWebsiteRemove_Click"><i class="ti-trash" style="font-size:large"></i></asp:LinkButton>
                                                                </div>
                                                                <div class="col-md-12">
                                                                    <br />
                                                                    <asp:Button ID="btnSave" runat="server" Text="Save" OnClick="btnSave_Click" CssClass="btn btn-sm btn-solid" CausesValidation="false" ValidationGroup="Submit" />
                                                                    <a id="aCancel" runat="server" href="/Admin/BudgetOverview.aspx" class="btn btn-sm btn-solid">Cancel</a>
                                                                    <asp:Button ID="btnDelete" runat="server" Text="Delete" OnClick="btnDelete_Click" CssClass="btn btn-sm btn-solid" CausesValidation="false"/>
                                                                </div>
                                                            </div>
                                                        </div>
                                                    </div>
                                                </div>
                                            </div>
                                        </div>
                                        <div class="tab-pane fade" id="top_2" runat="server" role="tabpanel" aria-labelledby="top_2_tab">
                                            <div class="row">
                                                <div class="col-12">
                                                    <div class="card dashboard-table mt-0">
                                                        <div class="card-body table-responsive-sm">
                                                            <div class="top-sec">
                                                                <h3></h3>
                                                                <%--<a href="/Admin/BudgetImport.aspx" class="btn btn-sm btn-solid">+ import</a>--%>
                                                                <a href="/Admin/BudgetAssignmentGroup.aspx?budgetid=<%=_Budget.BudgetID %>" class="btn btn-sm btn-solid">+ add new</a>
                                                            </div>
                                                            <div class="table-responsive-xl">
                                                                <asp:GridView runat="server" ID="gvBudgetGroup" AutoGenerateColumns="False" CssClass="table cart-table order-table" HeaderStyle-CssClass="table-head" GridLines="None" Width="100%" AllowSorting="true" CellSpacing="0" CellPadding="0" >
                                                                    <Columns>
                                                                        <asp:BoundField DataField="WebsiteGroup.GroupName" HeaderText="Group" HeaderStyle-HorizontalAlign="Center" ItemStyle-HorizontalAlign="Center" />
                                                                        <asp:TemplateField HeaderText="Created By" HeaderStyle-HorizontalAlign="Center" ItemStyle-HorizontalAlign="Center">
                                                                            <ItemTemplate>
                                                                                <%# Eval("CreatedByUser.FullName") %>
                                                                            </ItemTemplate>
                                                                        </asp:TemplateField>
                                                                        <asp:TemplateField HeaderText="Created Date" HeaderStyle-HorizontalAlign="Center" ItemStyle-HorizontalAlign="Center">
                                                                            <ItemTemplate>
                                                                                <%# Eval("CreatedOn") %>
                                                                            </ItemTemplate>
                                                                        </asp:TemplateField>
                                                                        <asp:TemplateField HeaderText="VIEW">
                                                                            <ItemTemplate>
                                                                                <a href='/admin/BudgetAssignmentGroup.aspx?id=<%# Eval("BudgetAssignmentID") %>'>
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
                                        <div class="tab-pane fade" id="top_3" runat="server" role="tabpanel" aria-labelledby="top_3_tab">
                                            <div class="row">
                                                <div class="col-12">
                                                    <div class="card dashboard-table mt-0">
                                                        <div class="card-body table-responsive-sm">
                                                            <div class="top-sec">
                                                                <h3></h3>
                                                                <a href="/Admin/BudgetAssignmentImport.aspx?budgetid=<%=_Budget.BudgetID %>" class="btn btn-sm btn-solid">+ import</a>
                                                                <a href="/Admin/BudgetAssignmentUser.aspx?budgetid=<%=_Budget.BudgetID %>" class="btn btn-sm btn-solid">+ add new</a>
                                                            </div>
                                                            <div class="col-12">
                                                                <div class="top-sec" id="Div1" runat="server" visible="true">
                                                                    <asp:Label ID="lblEmail" runat="server" Text="Email:"></asp:Label>&nbsp;<asp:TextBox ID="txtFilterBudgetUserEmail" runat="server" CssClass="form-control" placeholder="email/employee id" AutoPostBack="true" OnTextChanged="txtFilterBudgetUserEmail_TextChanged"></asp:TextBox>&nbsp;
                                                                </div>
                                                            </div>
                                                            <div class="table-responsive-xl">
                                                                <asp:GridView runat="server" ID="gvBudgetUser" AutoGenerateColumns="False" CssClass="table cart-table order-table" HeaderStyle-CssClass="table-head" GridLines="None" Width="100%" AllowSorting="true" CellSpacing="0" CellPadding="0" OnRowDataBound="gvBudgetUser_RowDataBound">
                                                                    <Columns>
                                                                        <asp:TemplateField HeaderText="User" HeaderStyle-HorizontalAlign="Center" ItemStyle-HorizontalAlign="Center">
                                                                            <ItemTemplate>
                                                                                <%# Eval("UserWebsite.UserInfo.FullName") %>
                                                                            </ItemTemplate>
                                                                        </asp:TemplateField>
                                                                        <asp:TemplateField HeaderText="Email Address" HeaderStyle-HorizontalAlign="Center" ItemStyle-HorizontalAlign="Center">
                                                                            <ItemTemplate>
                                                                                <%# Eval("UserWebsite.UserInfo.EmailAddress") %>
                                                                            </ItemTemplate>
                                                                        </asp:TemplateField>
                                                                        <asp:TemplateField HeaderText="Employee ID" HeaderStyle-HorizontalAlign="Center" ItemStyle-HorizontalAlign="Center">
                                                                            <ItemTemplate>
                                                                                <%# Eval("UserWebsite.EmployeeID") %>
                                                                            </ItemTemplate>
                                                                        </asp:TemplateField>
                                                                        <asp:TemplateField HeaderText="Created By" HeaderStyle-HorizontalAlign="Center" ItemStyle-HorizontalAlign="Center">
                                                                            <ItemTemplate>
                                                                                <%# Eval("CreatedByUser.FullName") %>
                                                                            </ItemTemplate>
                                                                        </asp:TemplateField>
                                                                        <asp:TemplateField HeaderText="Created Date" HeaderStyle-HorizontalAlign="Center" ItemStyle-HorizontalAlign="Center">
                                                                            <ItemTemplate>
                                                                                <%# Eval("CreatedOn") %>
                                                                            </ItemTemplate>
                                                                        </asp:TemplateField>
                                                                        <asp:TemplateField HeaderText="VIEW">
                                                                            <ItemTemplate>
                                                                                <a href='/admin/BudgetAssignmentUser.aspx?id=<%# Eval("BudgetAssignmentID") %>'>
                                                                                    <i class="fa fa-eye text-theme"></i>
                                                                                </a>
                                                                            </ItemTemplate>
                                                                        </asp:TemplateField>
                                                                    </Columns>
                                                                </asp:GridView>
                                                                <uc1:Pager runat="server" ID="ucBudgetUserPager" PagingMode="PostBack" PageSize="20" PagingRecordText="Users" OnPostBackPageIndexChanging="ucBudgetUserPager_PostBackPageIndexChanging" />
                                                            </div>
                                                        </div>
                                                    </div>
                                                </div>
                                            </div>
                                        </div>
                                        <div class="tab-pane fade" id="top_4" runat="server" role="tabpanel" aria-labelledby="top_4_tab">
                                            <div class="row">

                                                <div class="col-12">
                                                    <div class="card dashboard-table mt-0">
                                                        <div class="card-body table-responsive-sm">
                                                            <div class="col-12">
                                                                <div class="top-sec" id="filterBalance" runat="server" visible="true">
                                                                    <asp:Label ID="lblBalanceEmailFilter" runat="server" Text="Email:"></asp:Label>&nbsp;<asp:TextBox ID="txtFilterBalanceEmail" runat="server" CssClass="form-control" AutoPostBack="true" OnTextChanged="txtFilterBalanceEmail_TextChanged"></asp:TextBox>&nbsp;
                                                                </div>
                                                            </div>
                                                            <div class="table-responsive-xl">
                                                                <asp:GridView runat="server" ID="gvUserBalance" AutoGenerateColumns="False" CssClass="table cart-table order-table" HeaderStyle-CssClass="table-head" GridLines="None" Width="100%" AllowSorting="true" CellSpacing="0" CellPadding="0"  OnRowDataBound="gvUserBalance_RowDataBound">
                                                                    <Columns>
                                                                        <asp:TemplateField HeaderText="User Name" HeaderStyle-HorizontalAlign="Center" ItemStyle-HorizontalAlign="Center">
                                                                            <ItemTemplate>
                                                                                <%# Eval("UserInfo.FullName") %>
                                                                            </ItemTemplate>
                                                                        </asp:TemplateField>
                                                                        <asp:TemplateField HeaderText="ID" HeaderStyle-HorizontalAlign="Center" ItemStyle-HorizontalAlign="Center">
                                                                            <ItemTemplate>
                                                                                <%# Eval("UserInfo.UserName") %>
                                                                            </ItemTemplate>
                                                                        </asp:TemplateField>
                                                                        <asp:TemplateField HeaderText="Email Address" HeaderStyle-HorizontalAlign="Center" ItemStyle-HorizontalAlign="Center">
                                                                            <ItemTemplate>
                                                                                <%# Eval("UserInfo.EmailAddress") %>
                                                                            </ItemTemplate>
                                                                        </asp:TemplateField>
                                                                        <asp:TemplateField HeaderText="Remaining Balance" ItemStyle-HorizontalAlign="Center" HeaderStyle-HorizontalAlign="Center">
                                                                            <ItemTemplate>
                                                                                <%# string.Format("{0:c}", Eval("Balance"))%>
                                                                            </ItemTemplate>
                                                                        </asp:TemplateField>
                                                                        <asp:TemplateField HeaderText="Pending" ItemStyle-HorizontalAlign="Center" HeaderStyle-HorizontalAlign="Center">
                                                                            <ItemTemplate>
                                                                                <%# string.Format("{0:c}", Eval("PendingAmount"))%>
                                                                            </ItemTemplate>
                                                                        </asp:TemplateField>
                                                                    </Columns>
                                                                </asp:GridView>
                                                                <uc1:Pager runat="server" ID="ucBudgetBalancePager" PagingMode="PostBack" PageSize="20" PagingRecordText="Rows" OnPostBackPageIndexChanging="ucBudgetBalancePager_PostBackPageIndexChanging" />
                                                            </div>
                                                        </div>
                                                    </div>
                                                </div>
                                            </div>
                                        </div>
                                        <div class="tab-pane fade" id="top_5" runat="server" role="tabpanel" aria-labelledby="top_5_tab">
                                            <div class="row">
                                                <div class="col-12">
                                                    <div class="card dashboard-table mt-0">
                                                        <div class="card-body table-responsive-sm">
                                                            <div class="col-12">
                                                                <div class="top-sec" id="filter" runat="server" visible="true">
                                                                    Store:&nbsp;<asp:DropDownList ID="ddlAccount" runat="server" DataTextField="AccountNamePath" DataValueField="AccountID" OnSelectedIndexChanged="ddlAccount_SelectedIndexChanged" AutoPostBack="true"></asp:DropDownList>&nbsp;
                                                                    Start Date:&nbsp;<asp:TextBox ID="txtOrderStartDate" runat="server" placeholder="start date" AutoPostBack="true" OnTextChanged="txtDate_TextChanged"></asp:TextBox>&nbsp;
                                                                    <asp:CompareValidator ID="CompareValidator1" runat="server" ErrorMessage="Invalid Start Date Format" ControlToValidate="txtOrderStartDate" Display="None" Operator="DataTypeCheck" Type="Date"></asp:CompareValidator>
                                                                    End Date:&nbsp;<asp:TextBox ID="txtOrderEndDate" runat="server" placeholder="end date" AutoPostBack="true" OnTextChanged="txtDate_TextChanged"></asp:TextBox>&nbsp;
                                                                    <asp:CompareValidator ID="CompareValidator2" runat="server" ErrorMessage="Invalid End Date Format" ControlToValidate="txtOrderEndDate" Display="None" Operator="DataTypeCheck" Type="Date"></asp:CompareValidator>
                                                                    <asp:LinkButton ID="lbnDownload" runat="server" CssClass="btn btn-sm btn-solid" CausesValidation="false" OnClick="lbnDownload_Click">download report</asp:LinkButton>
                                                                </div>
                                                            </div>
                                                            <div class="table-responsive-xl">
                                                                <asp:UpdatePanel ID="UpdatePanel1" runat="server">
                                                                    <Triggers>
                                                                        <asp:AsyncPostBackTrigger ControlID="ddlAccount" />
                                                                        <asp:AsyncPostBackTrigger ControlID="txtOrderStartDate" />
                                                                        <asp:AsyncPostBackTrigger ControlID="txtOrderEndDate" />
                                                                    </Triggers>
                                                                    <ContentTemplate>
                                                                        <asp:GridView ID="gvSalesOrders" runat="server" AutoGenerateColumns="False" DataKeyNames="SalesOrderID" CssClass="table cart-table wishlist-table" HeaderStyle-CssClass="table-head" Width="100%" 
                                                                            GridLines="None" AllowSorting="true" CellSpacing="0" CellPadding="0">
                                                                            <Columns>
                                                                                <asp:TemplateField HeaderText="Order Number" ItemStyle-HorizontalAlign="Center" HeaderStyle-HorizontalAlign="Center">
                                                                                    <ItemTemplate>
                                                                                        <%# string.Format("{0:c}", Eval("SalesOrderID"))%>
                                                                                    </ItemTemplate>
                                                                                </asp:TemplateField>
                                                                                <asp:TemplateField HeaderText="Store" ItemStyle-HorizontalAlign="Center" HeaderStyle-HorizontalAlign="Center">
                                                                                    <ItemTemplate>
                                                                                        <%# Eval("Account.AccountName")%>
                                                                                    </ItemTemplate>
                                                                                </asp:TemplateField>
                                                                                <asp:TemplateField HeaderText="User" HeaderStyle-HorizontalAlign="Center" ItemStyle-HorizontalAlign="Center">
                                                                                    <ItemTemplate>
                                                                                        <%# Eval("UserWebsite.Description")%>
                                                                                    </ItemTemplate>
                                                                                </asp:TemplateField>
                                                                                <asp:CheckBoxField DataField="IsPendingApproval" HeaderText="Pending Approval" HeaderStyle-HorizontalAlign="Center" ItemStyle-HorizontalAlign="Center"/>
                                                                                <asp:BoundField DataField="Status" HeaderText="Status" HeaderStyle-HorizontalAlign="Center" ItemStyle-HorizontalAlign="Center"/>                                                                        
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
                                                                                        <a href='/admin/Order.aspx?id=<%# Eval("SalesOrderID") %>'>
                                                                                            <i class="fa fa-eye text-theme"></i>
                                                                                        </a>
                                                                                    </ItemTemplate>
                                                                                </asp:TemplateField>
                                                                            </Columns>
                                                                        </asp:GridView>
                                                                        <uc1:Pager runat="server" ID="ucSalesOrderPager" PagingMode="PostBack" PageSize="20" PagingRecordText="Rows" OnPostBackPageIndexChanging="ucSalesOrderPager_PostBackPageIndexChanging" />

<%--                                                                        <asp:GridView ID="gvBudgetAssignmentAdjustment" runat="server" AutoGenerateColumns="False" DataKeyNames="SalesOrderID" CssClass="table cart-table wishlist-table" HeaderStyle-CssClass="table-head" Width="100%" 
                                                                            GridLines="None" AllowSorting="true" CellSpacing="0" CellPadding="0">
                                                                            <Columns>
                                                                                <asp:TemplateField HeaderText="Reason" ItemStyle-HorizontalAlign="Center" HeaderStyle-HorizontalAlign="Center">
                                                                                    <ItemTemplate>
                                                                                        <%# Eval("Reason")%>
                                                                                    </ItemTemplate>
                                                                                </asp:TemplateField>
                                                                                <asp:TemplateField HeaderText="Reason" ItemStyle-HorizontalAlign="Center" HeaderStyle-HorizontalAlign="Center">
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
                                                                        </asp:GridView>--%>
                                                                    </ContentTemplate>
                                                                </asp:UpdatePanel>
                                                            </div>
                                                        </div>
                                                    </div>
                                                </div>
                                            </div>
                                        </div>


                                        <div class="tab-pane fade" id="top_6" runat="server" role="tabpanel" aria-labelledby="top_6_tab">
                                            <div class="row">
                                                <div class="col-12">
                                                    <div class="card dashboard-table mt-0">
                                                        <div class="card-body table-responsive-sm">
                                                            <div class="col-12">
                                                                <div class="top-sec" id="filterAdjustment" runat="server" visible="true">
                                                                    Start Date:&nbsp;<asp:TextBox ID="txtAdjustmentStartDate" runat="server" placeholder="start date" AutoPostBack="true" OnTextChanged="txtAdjustmentStartDate_TextChanged"></asp:TextBox>&nbsp;
                                                                    <asp:CompareValidator ID="CompareValidator3" runat="server" ErrorMessage="Invalid Start Date Format" ControlToValidate="txtAdjustmentStartDate" Display="None" Operator="DataTypeCheck" Type="Date"></asp:CompareValidator>
                                                                    End Date:&nbsp;<asp:TextBox ID="txtAdjustmentEndDate" runat="server" placeholder="end date" AutoPostBack="true" OnTextChanged="txtAdjustmentEndDate_TextChanged"></asp:TextBox>&nbsp;
                                                                    <asp:CompareValidator ID="CompareValidator4" runat="server" ErrorMessage="Invalid End Date Format" ControlToValidate="txtAdjustmentEndDate" Display="None" Operator="DataTypeCheck" Type="Date"></asp:CompareValidator>
                                                                    <asp:LinkButton ID="lbnAdjustmentDownload" runat="server" CssClass="btn btn-sm btn-solid" CausesValidation="false" OnClick="lbnAdjustmentDownload_Click">download report</asp:LinkButton>
                                                                </div>
                                                            </div>
                                                            <div class="table-responsive-xl">
                                                                <asp:UpdatePanel ID="UpdatePanel2" runat="server">
                                                                    <Triggers>
                                                                        <asp:AsyncPostBackTrigger ControlID="txtOrderStartDate" />
                                                                        <asp:AsyncPostBackTrigger ControlID="txtOrderEndDate" />
                                                                    </Triggers>
                                                                    <ContentTemplate>
                                                                        <asp:GridView ID="gvAdjustments" runat="server" AutoGenerateColumns="False" DataKeyNames="BudgetAssignmentAdjustmentID" CssClass="table cart-table wishlist-table" HeaderStyle-CssClass="table-head" Width="100%" 
                                                                            GridLines="None" AllowSorting="true" CellSpacing="0" CellPadding="0">
                                                                            <Columns>
                                                                                <asp:TemplateField HeaderText="User" HeaderStyle-HorizontalAlign="Center" ItemStyle-HorizontalAlign="Center">
                                                                                    <ItemTemplate>
                                                                                        <%# Eval("BudgetAssignment.UserWebsite.Description")%>
                                                                                    </ItemTemplate>
                                                                                </asp:TemplateField>
                                                                                <asp:TemplateField HeaderText="Reason" HeaderStyle-HorizontalAlign="Center" ItemStyle-HorizontalAlign="Center">
                                                                                    <ItemTemplate>
                                                                                        <%# Eval("Reason")%>
                                                                                    </ItemTemplate>
                                                                                </asp:TemplateField>
                                                                                <asp:TemplateField HeaderText="Amount" HeaderStyle-HorizontalAlign="Center" ItemStyle-HorizontalAlign="Center">
                                                                                    <ItemTemplate>
                                                                                        <%# string.Format("{0:c}", Eval("Amount"))%>
                                                                                    </ItemTemplate>
                                                                                 </asp:TemplateField>
                                                                                <asp:TemplateField HeaderText="CreatedOn" HeaderStyle-HorizontalAlign="Center" ItemStyle-HorizontalAlign="Center">
                                                                                    <ItemTemplate>
                                                                                        <%# string.Format("{0:d}", Eval("CreatedOn"))%>
                                                                                    </ItemTemplate>
                                                                                 </asp:TemplateField>
                                                                            </Columns>
                                                                        </asp:GridView>
                                                                        <uc1:Pager runat="server" ID="ucAdjustmentPager" PagingMode="PostBack" PageSize="20" PagingRecordText="Rows" OnPostBackPageIndexChanging="ucAdjustmentPager_PostBackPageIndexChanging" />
                                                                    </ContentTemplate>
                                                                </asp:UpdatePanel>
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

<asp:Content ID="Content4" ContentPlaceHolderID="cphFooter" runat="server">
    <uc2:UserWebsiteSearchModal ID="ucUserWebsiteSearchModal" runat="server" />
</asp:Content>

