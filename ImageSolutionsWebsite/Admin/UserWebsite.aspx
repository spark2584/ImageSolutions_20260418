<%@ Page Language="C#" MasterPageFile="~/MasterPage.Master" AutoEventWireup="true" CodeBehind="UserWebsite.aspx.cs" Inherits="ImageSolutionsWebsite.Admin.UserWebsite" %>
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
                        <div class="tab-pane fade show active" id="websites">
                            <div class="row">
                                <div class="col-sm-12 col-lg-12">
                                    <section class="tab-product m-0">
                                        <ul class="nav nav-tabs nav-material" id="top-tab" role="tablist">
                                            <li class="nav-item">
                                                <a class="nav-link active" id="top_1_tab" runat="server" data-bs-toggle="tab"
                                                    href="#cphBody_top_1" role="tab" aria-selected="false">
                                                    <i class="icofont icofont-ui-home"></i>User Information
                                                </a>
                                                <div class="material-border"></div>
                                            </li>
                                            <li class="nav-item">
                                                <a class="nav-link" id="top_2_tab" runat="server" data-bs-toggle="tab"
                                                    href="#cphBody_top_2" role="tab" aria-selected="true">
                                                    <i class="icofont icofont-man-in-glasses"></i>Assigned Accounts
                                                </a>
                                                <div class="material-border"></div>
                                            </li>
                                            <li class="nav-item">
                                                <a class="nav-link" id="top_3_tab" runat="server" data-bs-toggle="tab"
                                                    href="#cphBody_top_3" role="tab" aria-selected="true">
                                                    <i class="icofont icofont-man-in-glasses"></i>User Credits
                                                </a>
                                                <div class="material-border"></div>
                                            </li>
                                            <li class="nav-item">
                                                <a class="nav-link" id="top_4_tab" runat="server" data-bs-toggle="tab"
                                                    href="#cphBody_top_4" role="tab" aria-selected="true">
                                                    <i class="icofont icofont-man-in-glasses"></i>Custom Fields
                                                </a>
                                                <div class="material-border"></div>
                                            </li>
                                            <li class="nav-item">
                                                <a class="nav-link" id="top_5_tab" runat="server" data-bs-toggle="tab"
                                                    href="#cphBody_top_5" role="tab" aria-selected="true">
                                                    <i class="icofont icofont-man-in-glasses"></i>Admin
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
                                                        <h3>User Website Management</h3> 
                                                    </div>--%>
                                                    <div class="table-responsive-xl">
                                                        <div class="theme-form">
                                                            <div class="form-row row">
                                                                <div class="col-md-6">
                                                                    <label>First Name</label>
                                                                    <asp:TextBox ID="txtFirstName" runat="server" CssClass="form-control" placeholder="First Name" required=""></asp:TextBox>
                                                                    <asp:RequiredFieldValidator ID="valName" runat="server" ControlToValidate="txtFirstName" ValidationGroup="Submit" ErrorMessage="*" ForeColor="Red"></asp:RequiredFieldValidator>
                                                                </div>
                                                                <div class="col-md-6">
                                                                    <label>Last Name</label>
                                                                    <asp:TextBox ID="txtLastName" runat="server" CssClass="form-control" placeholder="Last Name" required=""></asp:TextBox>
                                                                    <asp:RequiredFieldValidator ID="RequiredFieldValidator2" runat="server" ControlToValidate="txtLastName" ValidationGroup="Submit" ErrorMessage="*" ForeColor="Red"></asp:RequiredFieldValidator>
                                                                </div>
                                                                <div class="col-md-12">
                                                                    <label>Email Address</label>
                                                                   <asp:TextBox ID="txtEmailAddress" runat="server" CssClass="form-control" placeholder="Email Address" required=""></asp:TextBox>
                                                                    <asp:RequiredFieldValidator ID="RequiredFieldValidator1" runat="server" ControlToValidate="txtEmailAddress" ValidationGroup="Submit" ErrorMessage="*" ForeColor="Red"></asp:RequiredFieldValidator>
                                                                </div>
                                                                <div class="col-md-12">
                                                                    <label>Notification Email Address (if different from Email Address)</label>
                                                                    <asp:TextBox ID="txtNotificationEmailAddress" runat="server" CssClass="form-control" placeholder="Notification Email Address"></asp:TextBox>
                                                                    <asp:RegularExpressionValidator ID="valNotificaitonEmail" runat="server" ValidationExpression="\w+([-+.]\w+)*@\w+([-.]\w+)*\.\w+([-.]\w+)*" ControlToValidate="txtNotificationEmailAddress" ValidationGroup="Submit" ErrorMessage="*" ForeColor="Red"></asp:RegularExpressionValidator>
                                                                </div>

                                                                <div class="col-md-12">
                                                                    <label>Disable Notification Email</label>&nbsp;
                                                                    <asp:CheckBox ID="chkDisableNotificationEmail" runat="server" />
                                                                </div>
                                                                <div class="col-md-12">
                                                                    <label>Display Notification Email at Checkout</label>&nbsp;
                                                                    <asp:CheckBox ID="chkDisplayNotificationEmailAtCheckout" runat="server" />
                                                                </div>

                                                                <div class="col-md-6">
                                                                    <label>Username</label>
                                                                    <asp:TextBox ID="txtUsername" runat="server" CssClass="form-control" placeholder="Username"></asp:TextBox>
                                                                </div>
                                                                <asp:PlaceHolder ID="phChangePassword" runat="server" Visible ="false">
                                                                    <div class="col-md-12" style="padding-bottom:20px">
                                                                        <br />
                                                                        <asp:Button ID="btnChangePassword" runat="server" Text="Change Password" CssClass="btn btn-sm btn-solid" OnClick="btnChangePassword_Click" ValidationGroup="ChangePassword" CausesValidation="false"/>
                                                                        &nbsp;&nbsp;
                                                                        <asp:Button ID="btnSendWelcomeEmail" runat="server" Text="Send Welcome Email" CssClass="btn btn-sm btn-solid" OnClick="btnSendWelcomeEmail_Click" CausesValidation="false"/>
                                                                    </div>
                                                                    <asp:PlaceHolder ID="phPassword" runat="server" Visible ="false">
                                                                        <div class="col-md-12">
                                                                            <label>Password</label>
                                                                            <asp:TextBox ID="txtPassword" runat="server" TextMode="Password" CssClass="form-control" placeholder="Password"></asp:TextBox>
                                                                            <asp:RequiredFieldValidator ID="RequiredFieldValidator4" runat="server" ControlToValidate="txtPassword" ValidationGroup="Submit" ErrorMessage="*" ForeColor="Red"></asp:RequiredFieldValidator>
                                                                            <asp:RegularExpressionValidator ID="valPassword" runat="server" ControlToValidate="txtPassword" ValidationGroup="Submit" ErrorMessage="Password must be between 6 to 20 characters" ValidationExpression=".{6,20}.*" ForeColor="Red" />
                                                                        </div>
                                                                        <div class="col-md-12">
                                                                            <label>Confirm Password</label>
                                                                            <asp:TextBox ID="txtPasswordConfirm" runat="server" TextMode="Password" CssClass="form-control" placeholder="Password Confirm"></asp:TextBox>
                                                                            <asp:RequiredFieldValidator ID="RequiredFieldValidator5" runat="server" ControlToValidate="txtPasswordConfirm" ValidationGroup="Submit" ErrorMessage="*" ForeColor="Red"></asp:RequiredFieldValidator>
                                                                            <asp:CompareValidator ID="CompareValidator1" runat="server" ControlToCompare="txtPassword" ControlToValidate="txtPasswordConfirm" ValidationGroup="Submit" ForeColor="Red" Display="Dynamic" ErrorMessage="Passwords do not Match"></asp:CompareValidator>
                                                                        </div>
                                                                    </asp:PlaceHolder>
                                                                </asp:PlaceHolder>
                                                                <asp:PlaceHolder ID="phRequestTaxExempt" runat="server">
                                                                    <div class="col-md-12">
                                                                        <label>Request Tax Exemption</label>&nbsp;
                                                                        <asp:CheckBox ID="chkRequestTaxExempt" runat="server" />
                                                                    </div>
                                                                </asp:PlaceHolder>
                                                                <asp:PlaceHolder ID="phTaxExempt" runat="server">
                                                                    <div class="col-md-12">
                                                                        <label>Tax Exemption</label>&nbsp;
                                                                        <asp:CheckBox ID="chkTaxExempt" runat="server" />
                                                                    </div>
                                                                </asp:PlaceHolder>
                                                                <asp:PlaceHolder ID="phPaymnetTerm" runat="server"> 
                                                                    <div class="col-md-12"> 
                                                                        <label>Payment Term</label>&nbsp; 
                                                                        <asp:DropDownList ID="ddlPaymentTerm" runat="server" DataTextField="Description" DataValueField="PaymentTermID" CssClass="form-control"></asp:DropDownList> 
                                                                        <br />
                                                                    </div> 
                                                                    <div class="col-md-6">
                                                                        <label>Payment Term Amount</label>
                                                                        <asp:TextBox ID="txtPaymentTermAmount" runat="server" CssClass="datepicker-here form-control digits" placeholder="amount" ></asp:TextBox>
                                                                    </div>
                                                                    <div class="col-md-6">
                                                                        <label>Payment Term Balance</label>
                                                                        <asp:TextBox ID="txtPaymentTermBalance" runat="server" CssClass="datepicker-here form-control digits" Enabled ="false" ></asp:TextBox>
                                                                    </div>
                                                                    <div class="col-md-12">
                                                                        <label>Payment Term Start Date</label>
                                                                        <asp:TextBox ID="txtPaymentTermStartDate" runat="server" CssClass="datepicker-here form-control digits" placeholder="start date"></asp:TextBox>
                                                                    </div>
<%--                                                                    <div class="col-md-12">
                                                                        <label>Payment Term End Date</label>
                                                                        <asp:TextBox ID="txtPaymentTermEndDate" runat="server" CssClass="datepicker-here form-control digits" placeholder="end date"></asp:TextBox>
                                                                    </div>--%>

                                                                </asp:PlaceHolder> 
                                                                <div class="col-md-12">
                                                                    <label>Pending Approval</label>&nbsp;
                                                                    <asp:CheckBox ID="chkIsPendingApproval" runat="server" />
                                                                </div>
                                                                <asp:Panel ID="phPermission" runat="server">
                                                                    <div class="col-md-12">
                                                                        <label>Is Account Admin</label>&nbsp;
                                                                        <asp:CheckBox ID="chkIsAdmin" runat="server" OnCheckedChanged="chkIsAdmin_CheckedChanged" AutoPostBack="true" />
                                                                    </div>
                                                                    <asp:Panel ID="pnlWebsiteManagement" runat="server">                                                                        
                                                                        <div class="col-md-12">
                                                                            <label>Website Management</label>&nbsp;
                                                                            <asp:CheckBox ID="chkWebsiteManagement" runat="server" />
                                                                        </div>
                                                                    </asp:Panel>
                                                                    <asp:Panel ID="pnlStoreManagement" runat="server">
                                                                        <div class="col-md-12">
                                                                            <label>Account Management</label>&nbsp;
                                                                            <asp:CheckBox ID="chkStoreManagement" runat="server" />
                                                                        </div>
                                                                    </asp:Panel>
                                                                    <asp:Panel ID="pnlUserManagement" runat="server">
                                                                        <div class="col-md-12">
                                                                            <label>User Management</label>&nbsp;
                                                                            <asp:CheckBox ID="chkUserManagement" runat="server" />
                                                                        </div>
                                                                    </asp:Panel>
                                                                    <asp:Panel ID="pnlGroupManagement" runat="server">
                                                                        <div class="col-md-12">
                                                                            <label>Group Management</label>&nbsp;
                                                                            <asp:CheckBox ID="chkGroupManagement" runat="server" />
                                                                        </div>
                                                                    </asp:Panel>
                                                                    <asp:Panel ID="pnlTabManagement" runat="server">
                                                                        <div class="col-md-12">
                                                                            <label>Tab Management</label>&nbsp;
                                                                            <asp:CheckBox ID="chkTabManagement" runat="server" />
                                                                        </div>
                                                                    </asp:Panel>
                                                                    <asp:Panel ID="pnlItemManagement" runat="server">
                                                                        <div class="col-md-12">
                                                                            <label>Item Management</label>&nbsp;
                                                                            <asp:CheckBox ID="chkItemManagement" runat="server" />
                                                                        </div>
                                                                    </asp:Panel>
                                                                    <asp:Panel ID="pnlBudgetManagement" runat="server">
                                                                        <div class="col-md-12">
                                                                            <label>Budget Management</label>&nbsp;
                                                                            <asp:CheckBox ID="chkBudgetManagement" runat="server" />
                                                                        </div>
                                                                        <div class="col-md-12">
                                                                            <label>Is Budget Admin</label>&nbsp;
                                                                            <asp:CheckBox ID="chkIsBudgetAdmin" runat="server" />
                                                                        </div>
                                                                        <div class="col-md-12">
                                                                            <label>Is Budget View Only</label>&nbsp;
                                                                            <asp:CheckBox ID="chkIsBudgetViewOnly" runat="server" />
                                                                        </div>
                                                                    </asp:Panel>
                                                                    <asp:Panel ID="pnlOrderManagement" runat="server">                                                                        
                                                                        <div class="col-md-12">
                                                                            <label>Order Management</label>&nbsp;
                                                                            <asp:CheckBox ID="chkOrderManagement" runat="server" />
                                                                        </div>
                                                                    </asp:Panel>
                                                                    <asp:Panel ID="pnlCreditCardManagement" runat="server">
                                                                        <div class="col-md-12">
                                                                            <label>Credit Card Management</label>&nbsp;
                                                                            <asp:CheckBox ID="chkCreditCardManagement" runat="server" />
                                                                        </div>
                                                                    </asp:Panel>
                                                                    <asp:Panel ID="pnlShippingManagement" runat="server">
                                                                        <div class="col-md-12">
                                                                            <label>Shipping Management</label>&nbsp;
                                                                            <asp:CheckBox ID="chkShippingManagement" runat="server" />
                                                                        </div>
                                                                    </asp:Panel>
                                                                    <asp:Panel ID="pnlMessageManagement" runat="server">
                                                                        <div class="col-md-12">
                                                                            <label>Message Management</label>&nbsp;
                                                                            <asp:CheckBox ID="chkMessageManagement" runat="server" />
                                                                        </div>
                                                                    </asp:Panel>
                                                                    <asp:Panel ID="pnlAllowPasswordUpdate" runat="server">
                                                                        <div class="col-md-12">
                                                                            <label>Allow Password Update</label>&nbsp;
                                                                            <asp:CheckBox ID="chkAllowPasswordUpdate" runat="server" />
                                                                        </div>
                                                                    </asp:Panel>


                                                                    <asp:Panel ID="Panel1" runat="server">
                                                                        <div class="col-md-12">
                                                                            <label>Hide Inventory Report</label>&nbsp;
                                                                            <asp:CheckBox ID="chkHideInventoryReport" runat="server" />
                                                                        </div>
                                                                    </asp:Panel>

                                                                    <asp:Panel ID="Panel2" runat="server">
                                                                        <div class="col-md-12">
                                                                            <label>Hide Order Approval</label>&nbsp;
                                                                            <asp:CheckBox ID="chkHideOrderApproval" runat="server" />
                                                                        </div>
                                                                    </asp:Panel>

                                                                </asp:Panel>

                                                                
                                                                    <asp:Panel ID="pnlApplyToBudgetProgram" runat="server">
                                                                        <div class="col-md-12">
                                                                            <label>Apply To Budget Program</label>&nbsp;
                                                                            <asp:CheckBox ID="chkApplyToBudgetProgram" runat="server" />
                                                                        </div>
                                                                    </asp:Panel>

                                                                <div class="col-md-12">
                                                                    <label>Opt In For Notification</label>&nbsp;
                                                                    <asp:CheckBox ID="chkOptInForNotification" runat="server" Checked="true" />
                                                                </div>

                                                                
                                                                <div class="col-md-12">
                                                                    <label>Enable Email Opt In</label>&nbsp;
                                                                    <asp:CheckBox ID="chkEnableEmailOptIn" runat="server" />
                                                                </div>
                                                                <div class="col-md-12">
                                                                    <label>Enable SMS Opt In</label>&nbsp;
                                                                    <asp:CheckBox ID="chkEnableSMSOptIn" runat="server" />
                                                                </div>

                                                                <div>
                                                                    <label>Address Permission</label>&nbsp;
                                                                    <asp:DropDownList ID="ddlAddressPermission" runat="server" CssClass="form-control" OnSelectedIndexChanged="ddlAddressPermission_SelectedIndexChanged" AutoPostBack="true">
                                                                        <asp:ListItem Selected="True" Value=""></asp:ListItem>
                                                                        <asp:ListItem Value="Account">Account Address</asp:ListItem>
                                                                        <asp:ListItem Value="Default">Default Address</asp:ListItem>
                                                                    </asp:DropDownList>
                                                                    <asp:Panel ID="pnlAddressPermissionDefault" runat="server">
                                                                        <div class="col-md-12">
                                                                            <label>Default Shipping</label>&nbsp;
                                                                            <asp:DropDownList ID="ddlDefaultShippingAddress" runat="server" CssClass="form-control"></asp:DropDownList>
                                                                        </div>
                                                                        <div class="col-md-12">
                                                                            <label>Default Billing</label>&nbsp;
                                                                            <asp:DropDownList ID="ddlDefaultBillingAddress" runat="server" CssClass="form-control"></asp:DropDownList>
                                                                        </div>
                                                                    </asp:Panel>
                                                                    <br />
                                                                </div>
                                                                <div class="col-md-12">
                                                                    <asp:Button ID="btnSave" runat="server" Text="Save" OnClick="btnSave_Click" CssClass="btn btn-sm btn-solid" ValidationGroup="Submit" />
                                                                    <a id="aCancel" runat="server" href="/Admin/GroupOverview.aspx" class="btn btn-sm btn-solid">Cancel</a>
                                                                    <asp:Button ID="btnDelete" runat="server" Text="Delete" OnClick="btnDelete_Click" CssClass="btn btn-sm btn-solid" CausesValidation="false"/>
                                                                    <asp:Button ID="btnLogin" runat="server" Text="Login As" OnClick="btnLogin_Click" CssClass="btn btn-sm btn-solid" CausesValidation="false"/>
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
                                                                <a href="/Admin/UserAccount.aspx?userwebsiteid=<%= _UserWebSite.UserWebsiteID %>" class="btn btn-sm btn-solid">+ add new</a>
                                                            </div>
                                                            <div class="table-responsive-xl">                                                                <asp:GridView runat="server" ID="gvUserAccount" AutoGenerateColumns="False" CssClass="table cart-table order-table" HeaderStyle-CssClass="table-head" GridLines="None" Width="100%" AllowSorting="true" CellSpacing="0" CellPadding="0" >
                                                                    <Columns>
                                                                        <asp:BoundField DataField="UserWebsite.UserInfo.FullName" HeaderText="User" HeaderStyle-HorizontalAlign="Left" ItemStyle-HorizontalAlign="Left" />
                                                                        <asp:BoundField DataField="Account.AccountName" HeaderText="Store" HeaderStyle-HorizontalAlign="Left" ItemStyle-HorizontalAlign="Left" />
                                                                        <asp:BoundField DataField="WebsiteGroup.GroupName" HeaderText="Group Name" HeaderStyle-HorizontalAlign="Left" ItemStyle-HorizontalAlign="Left" />
                                                                        <%--<asp:TemplateField HeaderText="Created By" HeaderStyle-HorizontalAlign="Center" ItemStyle-HorizontalAlign="Center">
                                                                            <ItemTemplate>
                                                                                <%# Eval("CreatedByUser.FullName") %>
                                                                            </ItemTemplate>
                                                                        </asp:TemplateField>--%>
                                                                        <asp:TemplateField HeaderText="Created Date" HeaderStyle-HorizontalAlign="Center" ItemStyle-HorizontalAlign="Center">
                                                                            <ItemTemplate>
                                                                                <%# Eval("CreatedOn") %>
                                                                            </ItemTemplate>
                                                                        </asp:TemplateField>
                                                                        <asp:TemplateField HeaderText="VIEW">
                                                                            <ItemTemplate>
                                                                                <a href='/admin/UserAccount.aspx?id=<%# Eval("UserAccountID") %>'>
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
<%--                                                            <div class="top-sec">
                                                                <h3></h3>
                                                                <a href="/Admin/UserAccount.aspx?userwebsiteid=<%= _UserWebSite.UserWebsiteID %>" class="btn btn-sm btn-solid">+ add new</a>
                                                            </div>--%>
                                                            <div class="table-responsive-xl">                                                                <asp:GridView ID="gvBudgetAssignments" runat="server" AutoGenerateColumns="False" DataKeyNames="BudgetAssignmentID" CssClass="table cart-table wishlist-table" HeaderStyle-CssClass="table-head" Width="100%" 
                                                                    GridLines="None" AllowSorting="true" CellSpacing="0" CellPadding="0" OnRowDataBound="gvBudgetAssignments_RowDataBound">
                                                                    <Columns>
                                                                        <asp:TemplateField HeaderText="Website" HeaderStyle-HorizontalAlign="Center" ItemStyle-HorizontalAlign="Center">
                                                                            <ItemTemplate>
                                                                                <%# Eval("BudgetAssignment.Budget.Website.Name")%>
                                                                            </ItemTemplate>
                                                                         </asp:TemplateField>
                                                                        <asp:TemplateField HeaderText="Budget Name" ItemStyle-HorizontalAlign="Center" HeaderStyle-HorizontalAlign="Center">
                                                                            <ItemTemplate>
                                                                                <%# Eval("BudgetAssignment.Budget.BudgetName")%>
                                                                            </ItemTemplate>
                                                                        </asp:TemplateField>
                                                                        <asp:TemplateField HeaderText="Budget Amount" ItemStyle-HorizontalAlign="Center" HeaderStyle-HorizontalAlign="Center">
                                                                            <ItemTemplate>
                                                                                <%# string.Format("{0:c}", Eval("BudgetAssignment.Budget.BudgetAmount"))%>
                                                                            </ItemTemplate>
                                                                        </asp:TemplateField>
                                                                        <asp:TemplateField HeaderText="Available Amount" ItemStyle-HorizontalAlign="Center" HeaderStyle-HorizontalAlign="Center">
                                                                            <ItemTemplate>
                                                                                <%# string.Format("{0:c}", Eval("Balance"))%>
                                                                            </ItemTemplate>
                                                                        </asp:TemplateField>
                                                                        <asp:TemplateField HeaderText="Created On" HeaderStyle-HorizontalAlign="Center" ItemStyle-HorizontalAlign="Center">
                                                                            <ItemTemplate>
                                                                                <%# string.Format("{0:d}", Eval("BudgetAssignment.CreatedOn"))%>
                                                                            </ItemTemplate>
                                                                         </asp:TemplateField>
                                                                        <asp:TemplateField HeaderText="VIEW">
                                                                            <ItemTemplate>
                                                                                <a href='/Admin/Budget.aspx?id=<%# Eval("BudgetAssignment.Budget.BudgetID") %>'>
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

                                        <div class="tab-pane fade" id="top_4" runat="server" role="tabpanel" aria-labelledby="top_4_tab">
                                            <div class="row">
                                                <div class="col-12">
                                                    <div class="card dashboard-table mt-0">
                                                        <div class="card-body table-responsive-sm">
                                                            <div class="top-sec">
                                                                <h3></h3>
                                                            </div>
                                                            <div class="table-responsive-xl">                                                                <div class="theme-form">
                                                                    <div class="form-row row">                                                                        
                                                                        <asp:Repeater ID="rptCustomField" runat="server" DataMember="CustomFieldID" OnItemDataBound="rptCustomField_ItemDataBound">
                                                                            <ItemTemplate>
                                                                                 <div class="col-md-12">
                                                                                    <asp:Label ID="lblLabel" runat="server" Text='<%# Eval("Name")%>' Font-Bold="true"></asp:Label>
                                                                                    <asp:TextBox ID="txtCustomValue"  runat="server" CssClass="form-control" Visible="true"></asp:TextBox>
                                                                                    <asp:DropDownList ID="ddlCustomValueList" runat="server" DataTextField="Value" DataValueField="Value" CssClass="form-control" Visible="false"></asp:DropDownList>
                                                                                    <asp:HiddenField ID="hfCustomFieldID" runat="server" Value='<%# Eval("CustomFieldID")%>'/>
                                                                                    <asp:HiddenField ID="hfCustomValueID" runat="server" />
                                                                                </div>
                                                                                <br />
                                                                            </ItemTemplate>
                                                                        </asp:Repeater>         
                                                                        <div class="col-md-12">
                                                                            <br />
                                                                            <asp:Button ID="btnUpdateCustomField" runat="server" Text="Update" OnClick="btnUpdateCustomField_Click" CssClass="btn btn-sm btn-solid" ValidationGroup="UpdateCustomField" />
                                                                        </div>
                                                                    </div>
                                                                </div>
                                                            </div>
                                                        </div>
                                                    </div>
                                                </div>
                                            </div>
                                        </div>

                                        <div class="tab-pane fade" id="top_5" runat="server" role="tabpanel" aria-labelledby="top_5_tab">
                                            <div class="card dashboard-table mt-0">
                                                <div class="card-body table-responsive-sm">
                                                    <div class="table-responsive-xl">
                                                        <div class="theme-form">
                                                            <div class="form-row row">
                                                                <div class="col-md-12">
                                                                    <label>Customer Internal ID</label>
                                                                    <asp:TextBox ID="txtInternalID" runat="server" type="number" CssClass="form-control" min="0" placeholder="Internal ID" ></asp:TextBox>
                                                                </div>
                                                                <div class="col-md-12">
                                                                    <label>Employee ID</label>
                                                                    <asp:TextBox ID="txtEmployeeID" runat="server" CssClass="form-control" placeholder="Employee ID"></asp:TextBox>
                                                                </div>
                                                                <div class="col-md-12">
                                                                    <label>Hire Date</label>
                                                                    <asp:TextBox ID="txtHireDate" runat="server" CssClass="form-control" placeholder="Hire Date"></asp:TextBox>
                                                                </div>
                                                                <div class="col-md-12">
                                                                    <label>Is Part Time</label>&nbsp;
                                                                    <asp:CheckBox ID="chkIsPartTime" runat="server" />
                                                                </div>
                                                                <div class="col-md-12">
                                                                    <label>Is Store</label>&nbsp;
                                                                    <asp:CheckBox ID="chkIsStore" runat="server" />
                                                                </div>
                                                                <div class="col-md-12">
                                                                    <label>Auto Assign Group</label>&nbsp;
                                                                    <asp:CheckBox ID="chkAutoAssignGroup" runat="server" />
                                                                </div>
                                                                <%--<div class="col-md-12">
                                                                    <label>Auto Assign Budget</label>&nbsp;
                                                                    <asp:CheckBox ID="chkAutoAssignBudget" runat="server" />
                                                                </div>--%>
                                                                <div class="col-md-12">
                                                                    <label>Budget Setting</label>&nbsp;
                                                                    <asp:DropDownList ID="ddlBudgetSetting" runat="server" CssClass="form-control">
                                                                        <asp:ListItem Selected="True" Value=""></asp:ListItem>
                                                                        <asp:ListItem Value="No Budget">No Budget</asp:ListItem>
                                                                        <asp:ListItem Value="Full Time">Full Time</asp:ListItem>
                                                                        <asp:ListItem Value="Part Time">Part Time</asp:ListItem>
                                                                        <asp:ListItem Value="Warehouse/Facilities">Warehouse/Facilities</asp:ListItem>
                                                                        <asp:ListItem Value="Fleet">Fleet</asp:ListItem>
                                                                    </asp:DropDownList>
                                                                </div>

                                                                <div class="col-md-12">
                                                                    <label>Package Available Date</label>
                                                                    <asp:TextBox ID="txtPackageAvailableDate" runat="server" CssClass="form-control" placeholder="Package Available Date"></asp:TextBox>
                                                                </div>
                                                                
                                                                <div class="col-md-12">
                                                                    <label>Allow Only SSO Login</label>&nbsp;
                                                                    <asp:CheckBox ID="chkAllowOnlySSO" runat="server" />
                                                                </div>

                                                                <div class="col-md-12">
                                                                    <label>Inactive</label>&nbsp;
                                                                    <asp:CheckBox ID="chkInactive" runat="server" />
                                                                </div>

                                                                <div class="col-md-12">
                                                                    <br />
                                                                    <asp:Button ID="btnSave2" runat="server" Text="Save" OnClick="btnSave_Click" CssClass="btn btn-sm btn-solid" ValidationGroup="Submit" />
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
        </div>
    </section>
    <!--  dashboard section end -->
</asp:Content>