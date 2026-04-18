<%@ Page Language="C#" MasterPageFile="~/MasterPage.Master" AutoEventWireup="true" CodeBehind="Website.aspx.cs" Inherits="ImageSolutionsWebsite.Admin.Website" %>
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
                        <div class="tab-pane fade show active" id="websites">
                            <div class="row">
                                <div class="col-sm-12 col-lg-12">
                                    <section class="tab-product m-0">
                                        <ul class="nav nav-tabs nav-material" id="top-tab" role="tablist">
                                            <li class="nav-item">
                                                <a class="nav-link active" id="top_1_tab" data-bs-toggle="tab"
                                                    href="#top_1" role="tab" aria-selected="true">
                                                    <i class="icofont icofont-ui-home"></i>Website Information
                                                </a>
                                                <div class="material-border"></div>
                                            </li>
                                            <li class="nav-item" style="display:none;">
                                                <a class="nav-link" id="top_2_tab" data-bs-toggle="tab"
                                                    href="#top_2" role="tab" aria-selected="false">
                                                    <i class="icofont icofont-man-in-glasses"></i>Groups
                                                </a>
                                                <div class="material-border"></div>
                                            </li>
                                            <li class="nav-item" style="display:none;">
                                                <a class="nav-link" id="top_3_tab" data-bs-toggle="tab"
                                                    href="#top_3" role="tab" aria-selected="false">
                                                    <i class="icofont icofont-contacts"></i>Users
                                                </a>
                                                <div class="material-border"></div>
                                            </li>
                                            <li class="nav-item" style="display:none;">
                                                <a class="nav-link" id="top_4_tab" data-bs-toggle="tab"
                                                    href="#top_4" role="tab" aria-selected="false">
                                                    <i class="icofont icofont-contacts"></i>Tabs
                                                </a>
                                                <div class="material-border"></div>
                                            </li>
                                            <li class="nav-item">
                                                <a class="nav-link" id="top_5_tab" data-bs-toggle="tab"
                                                    href="#top_5" role="tab" aria-selected="false">
                                                    <i class="icofont icofont-contacts"></i>Countries
                                                </a>
                                                <div class="material-border"></div>
                                            </li>
                                            <li class="nav-item">
                                                <a class="nav-link" id="top_6_tab" data-bs-toggle="tab"
                                                    href="#top_6" role="tab" aria-selected="false">
                                                    <i class="icofont icofont-contacts"></i>Approved Personalization
                                                </a>
                                                <div class="material-border"></div>
                                            </li>
                                        </ul>
                                    </section><br />
                                    <div class="tab-content nav-material">
                                        <div class="tab-pane fade show active" id="top_1" role="tabpanel" aria-labelledby="top_1_tab">
                                            <div class="card dashboard-table mt-0">
                                                <div class="card-body table-responsive-sm">
                                                    <%--<div class="top-sec">
                                                        <h3>User Website Management</h3> 
                                                    </div>--%>
                                                    <div class="table-responsive-xl">
                                                        <div class="theme-form">
                                                            <div class="form-row row">
                                                                <div class="col-md-12">
                                                                    <label>Website Name</label>
                                                                    <asp:TextBox ID="txtName" runat="server" CssClass="form-control" placeholder="website name" required=""></asp:TextBox>
                                                                    <asp:RequiredFieldValidator ID="valName" runat="server" ControlToValidate="txtName" ValidationGroup="Submit" ErrorMessage="*" ForeColor="Red"></asp:RequiredFieldValidator>
                                                                </div>
                                                                <div class="col-md-12">
                                                                    <label>Abbreviation</label>
                                                                    <asp:TextBox ID="txtAbbreviation" runat="server" CssClass="form-control" placeholder="abbreviation" MaxLength="2"></asp:TextBox>
                                                                </div>
                                                                <div class="col-md-12">
                                                                    <label>Domain</label>
                                                                    <asp:TextBox ID="txtDomain" runat="server" CssClass="form-control" placeholder="domain" required="true"></asp:TextBox>
                                                                    <asp:RequiredFieldValidator ID="valDomain" runat="server" ControlToValidate="txtDomain" ValidationGroup="Submit" ErrorMessage="*" ForeColor="Red"></asp:RequiredFieldValidator>
                                                                </div>
                                                                <div class="col-md-12">
                                                                    <label>User Registration</label>&nbsp;
                                                                    <asp:CheckBox ID="chkUserRegistration" runat="server" />
                                                                </div>
                                                                <div class="col-md-12">
                                                                    <label>User Registration Key Required</label>&nbsp;
                                                                    <asp:CheckBox ID="chkUserRegistrationKeyRequired" runat="server" />
                                                                </div>
                                                                <div class="col-md-12">
                                                                    <label>User Registration Approval Required</label>&nbsp;
                                                                    <asp:CheckBox ID="chkUserApprovalRequired" runat="server" />
                                                                </div>
                                                                <div class="col-md-12">
                                                                    <label>Account Registration</label>&nbsp;
                                                                    <asp:CheckBox ID="chkAccountRegistration" runat="server" />
                                                                </div>
                                                                <div class="col-md-12">
                                                                    <label>Account Registration Key Required</label>&nbsp;
                                                                    <asp:CheckBox ID="chkAccountRegistrationKeyRequired" runat="server" />
                                                                </div>
                                                                <div class="col-md-12">
                                                                    <label>Account Registration Approval Required</label>&nbsp;
                                                                    <asp:CheckBox ID="chkAccountApprovalRequired" runat="server" />
                                                                </div>
                                                                <div class="col-md-12">
                                                                    <label>Default Website Group</label>&nbsp;
                                                                    <asp:DropDownList ID="ddlWebsiteGroup" runat="server" DataTextField="GroupName" DataValueField="WebsiteGroupID"></asp:DropDownList>
                                                                </div>
                                                                <div class="col-md-12">
                                                                    <label>Show Available Inventory</label>&nbsp;
                                                                    <asp:CheckBox ID="chkShowAvailableInventory" runat="server" />
                                                                </div>
                                                                <div class="col-md-12">
                                                                    <label>Show Sales Description</label>&nbsp;
                                                                    <asp:CheckBox ID="chkShowSalesDescription" runat="server" />
                                                                </div>
                                                                <div class="col-md-12">
                                                                    <label>Show Detailed Description</label>&nbsp;
                                                                    <asp:CheckBox ID="chkShowDetailedDescription" runat="server" />
                                                                </div>
                                                                <div class="col-md-12">
                                                                    <label>Order Approval Required</label>&nbsp;
                                                                    <asp:CheckBox ID="chkOrderApprovalRequired" runat="server" />
                                                                </div>
                                                                <div class="col-md-12">
                                                                    <label>Enable User Credit</label>&nbsp;
                                                                    <asp:CheckBox ID="chkEnableEmployeeCredit" runat="server" />
                                                                </div>
                                                                <div class="col-md-12">
                                                                    <label>Must Use Existing User Credit</label>&nbsp;
                                                                    <asp:CheckBox ID="chkMustUseExistingEmployeeCredit" runat="server" />
                                                                </div>
                                                                
                                                                <div class="col-md-12">
                                                                    <label>Budget Alias</label>
                                                                    <asp:TextBox ID="txtBudgetAlias" runat="server" CssClass="form-control" placeholder="budget alias"></asp:TextBox>
                                                                </div>
                                                                <div class="col-md-12">
                                                                    <label>Over-Budget Message</label>
                                                                    <asp:TextBox ID="txtOverBudgetMessage" runat="server" CssClass="form-control" placeholder="over budget message"></asp:TextBox>
                                                                </div>

                                                                <div class="col-md-12">
                                                                    <label>Is One Budget Per User</label>&nbsp;
                                                                    <asp:CheckBox ID="chkIsOneBudgetPerUser" runat="server" />
                                                                </div>

                                                                <div class="col-md-12">
                                                                    <label>Enable Credit Card</label>&nbsp;
                                                                    <asp:CheckBox ID="chkEnableCreditCard" runat="server" />
                                                                </div>

                                                                <div class="col-md-12">
                                                                    <label>Credit Card Limit (Per Order)</label>
                                                                    <asp:TextBox ID="txtCreditCardLimitPerOrder" runat="server" CssClass="form-control" placeholder="amount" onKeyPress="if (event.keyCode!=46 && event.keyCode!=45 && (event.keyCode<48 || event.keyCode>57)) event.returnValue=false"></asp:TextBox>
                                                                </div>

                                                                <div class="col-md-12">
                                                                    <label>Enable Payment Term</label>&nbsp;
                                                                    <asp:CheckBox ID="chkEnablePaymentTerm" runat="server" />
                                                                </div>
                                                                <div class="col-md-12">
                                                                    <label>Enable Promo Code</label>&nbsp;
                                                                    <asp:CheckBox ID="chkEnablePromoCode" runat="server" />
                                                                </div>
                                                                <div class="col-md-12">
                                                                    <label>Default Payment Term</label>&nbsp;
                                                                    <asp:DropDownList ID="ddlPaymentTerm" runat="server" DataTextField="Description" DataValueField="PaymentTermID"></asp:DropDownList>
                                                                </div>
                                                                <div class="col-md-12">
                                                                    <label>Display Default Group Per Account</label>&nbsp;
                                                                    <asp:CheckBox ID="chkDisplayDefaultGroupPerAccount" runat="server" />
                                                                </div>
                                                                <div class="col-md-12">
                                                                    <label>Display Sub Category</label>&nbsp;
                                                                    <asp:CheckBox ID="chkDisplaySubCategory" runat="server" />
                                                                </div>
                                                                <div class="col-md-12">
                                                                    <label>Display Attribute Filter</label>&nbsp;
                                                                    <asp:CheckBox ID="chkDisplayAttributeFilter" runat="server" />
                                                                </div>
                                                                <div class="col-md-12">
                                                                    <label>Display Left Navigation</label>&nbsp;
                                                                    <asp:CheckBox ID="chkDisplayLeftNavigation" runat="server" />
                                                                </div>
                                                                <div class="col-md-12">
                                                                    <label>Display Tariff Charge</label>&nbsp;
                                                                    <asp:CheckBox ID="chkDisplayTariffCharge" runat="server" />
                                                                </div>
                                                                <div class="col-md-12">
                                                                    <label>Combine Website Group</label>&nbsp;
                                                                    <asp:CheckBox ID="chkCombineWebsiteGroup" runat="server" />
                                                                </div>
                                                                <div class="col-md-12">
                                                                    <label>Item Category Display Type</label>&nbsp;
                                                                    <asp:DropDownList ID="ddlItemDisplayType" runat="server">
                                                                        <asp:ListItem Value="Grid">Grid</asp:ListItem>
                                                                        <asp:ListItem Value="List">List</asp:ListItem>
                                                                    </asp:DropDownList>
                                                                    <asp:RequiredFieldValidator ID="RequiredFieldValidator1" runat="server" ControlToValidate="ddlItemDisplayType" ValidationGroup="Submit" ErrorMessage="*" ForeColor="Red"></asp:RequiredFieldValidator>
                                                                </div>
                                                                <div class="col-md-12">
                                                                    <label>Product Detail Display Type</label>&nbsp;
                                                                    <asp:DropDownList ID="ddlProductDisplayType" runat="server">
                                                                        <asp:ListItem Value="Grid">Grid</asp:ListItem>
                                                                        <asp:ListItem Value="List">List</asp:ListItem>
                                                                    </asp:DropDownList>
                                                                    <asp:RequiredFieldValidator ID="RequiredFieldValidator2" runat="server" ControlToValidate="ddlItemDisplayType" ValidationGroup="Submit" ErrorMessage="*" ForeColor="Red"></asp:RequiredFieldValidator>
                                                                </div>
                                                                <div class="col-md-12">
                                                                    <br />
                                                                    <label>Logo Upload</label>
                                                                    <asp:FileUpload ID="fuLogoImage" runat="server" />
                                                                    <asp:Button ID="btnUpload" runat="server" Text="Upload" OnClick="btnUpload_Click" CssClass="btn btn-sm btn-solid" />
                                                                    <asp:Button ID="btnResetImage" runat="server" Text="Reset" OnClick="btnResetImage_Click" CssClass="btn btn-sm btn-solid" />
                                                                    <asp:Button ID="btnRemoveImage" runat="server" Text="Remove" OnClick="btnRemoveImage_Click" CssClass="btn btn-sm btn-solid"/>
                                                                    <br />
                                                                    <asp:Image ID="imgLogo" runat="server" Width="200px" /><br /><br />
                                                                </div>
                                                                <div class="col-md-12">
                                                                    <br />
                                                                    <label>Email Logo Upload</label>
                                                                    <asp:FileUpload ID="fuEmailLogoImage" runat="server" />
                                                                    <asp:Button ID="btnEmailLogoUpload" runat="server" Text="Upload" OnClick="btnEmailLogoUpload_Click" CssClass="btn btn-sm btn-solid" />
                                                                    <asp:Button ID="btnEmailLogoResetImage" runat="server" Text="Reset" OnClick="btnEmailLogoResetImage_Click" CssClass="btn btn-sm btn-solid" />
                                                                    <asp:Button ID="btnEmailLogoRemoveImage" runat="server" Text="Remove" OnClick="btnEmailLogoRemoveImage_Click" CssClass="btn btn-sm btn-solid"/>
                                                                    <br />
                                                                    <asp:Image ID="imgEmailLogo" runat="server" Width="200px" /><br /><br />
                                                                </div>
                                                                <div class="col-md-12">
                                                                    <br />
                                                                    <label>Banner Upload</label>
                                                                    <asp:FileUpload ID="fuBannerImage" runat="server" />
                                                                    <asp:Button ID="btnBannerUpload" runat="server" Text="Upload" OnClick="btnBannerUpload_Click" CssClass="btn btn-sm btn-solid" />
                                                                    <asp:Button ID="btnBannerResetImage" runat="server" Text="Reset" OnClick="btnBannerResetImage_Click" CssClass="btn btn-sm btn-solid" />
                                                                    <asp:Button ID="btnBannerRemoveImage" runat="server" Text="Remove" OnClick="btnBannerRemoveImage_Click" CssClass="btn btn-sm btn-solid"/>
                                                                    <br />
                                                                    <asp:Image ID="imgBanner" runat="server" Width="200px" /><br /><br />
                                                                </div>
                                                                <div class="col-md-12">
                                                                    <br />
                                                                    <label>Default Size Chart</label>
                                                                    <asp:FileUpload ID="fuSizeChartImage" runat="server" />
                                                                    <asp:Button ID="btnSizeChartUpload" runat="server" Text="Upload" OnClick="btnSizeChartUpload_Click" CssClass="btn btn-sm btn-solid" />
                                                                    <asp:Button ID="btnSizeChartResetImage" runat="server" Text="Reset" OnClick="btnSizeChartResetImage_Click" CssClass="btn btn-sm btn-solid" />
                                                                    <asp:Button ID="btnSizeChartRemoveImage" runat="server" Text="Remove" OnClick="btnSizeChartRemoveImage_Click" CssClass="btn btn-sm btn-solid"/>
                                                                    <br />
                                                                    <asp:Image ID="imgSizeChart" runat="server" Width="200px" /><br /><br />
                                                                </div>
                                                                <div class="col-md-12">
                                                                    <label>Allow Name Change</label>&nbsp;
                                                                    <asp:CheckBox ID="chkAllowNameChange" runat="server" />
                                                                </div>
                                                                <div class="col-md-12">
                                                                    <label>Enable Password Reset</label>&nbsp;
                                                                    <asp:CheckBox ID="chkEnablePasswordReset" runat="server" />
                                                                </div>

                                                                <div class="col-md-12">
                                                                    <label>Disallow Back Order</label>&nbsp;
                                                                    <asp:CheckBox ID="chkDisallowBackOrder" runat="server" />
                                                                </div>
                                                                
                                                                <div class="col-md-12">
                                                                    <label>Allow Back Order For All Items</label>&nbsp;
                                                                    <asp:CheckBox ID="chkAllowBackOrderForAllItems" runat="server" />
                                                                </div>

                                                                <div class="col-md-12">
                                                                    <label>Hide User Email</label>&nbsp;
                                                                    <asp:CheckBox ID="chkHideEmail" runat="server" />
                                                                </div>
                                                                <div class="col-md-12">
                                                                    <label>Enable Zendesk</label>&nbsp;
                                                                    <asp:CheckBox ID="chkEnableZendesk" runat="server" />
                                                                </div>
                                                                
                                                                <div class="col-md-12">
                                                                    <label>Hide Order Report (My Account)</label>&nbsp;
                                                                    <asp:CheckBox ID="chkHideMyAccountOrderReport" runat="server" />
                                                                </div>
                                                                <div class="col-md-12">
                                                                    <label>Hide Order Approval (My Account)</label>&nbsp;
                                                                    <asp:CheckBox ID="chkHideMyAccountOrderApproval" runat="server" />
                                                                </div>
                                                                <div class="col-md-12">
                                                                    <label>Hide Order Approval (Admin)</label>&nbsp;
                                                                    <asp:CheckBox ID="chkHideAdminOrderApproval" runat="server" />
                                                                </div>


                                                                <div class="col-md-12">
                                                                    <label>Is Punchout</label>&nbsp;
                                                                    <asp:CheckBox ID="chkIsPunchOut" runat="server" />
                                                                </div>

                                                                
                                                                <div class="col-md-12">
                                                                    <label>Discount (Per Item)</label>
                                                                    <asp:TextBox ID="txtDiscountPerItem" runat="server" CssClass="form-control" placeholder="amount" onKeyPress="if (event.keyCode!=46 && event.keyCode!=45 && (event.keyCode<48 || event.keyCode>57)) event.returnValue=false"></asp:TextBox>
                                                                </div>

                                                                <div class="col-md-12">
                                                                    <label>Display Employee Permission</label>&nbsp;
                                                                    <asp:DropDownList ID="ddlDisplayUserPermission" runat="server" CssClass="form-control">
                                                                        <asp:ListItem Selected="True" Value=""></asp:ListItem>
                                                                        <asp:ListItem Value="All">All</asp:ListItem>
                                                                        <asp:ListItem Value="All With Account Change">All With Account Change</asp:ListItem>
                                                                        <asp:ListItem Value="Store Only">Store Only</asp:ListItem>
                                                                    </asp:DropDownList>                 
                                                                    <br />
                                                                </div>

                                                                <div class="col-md-12">
                                                                    <label>Enable Localize</label>&nbsp;
                                                                    <asp:CheckBox ID="CheckBox1" runat="server" />
                                                                </div>
                                                                <div class="col-md-12">
                                                                    <label>Enable Email Opt In</label>&nbsp;
                                                                    <asp:CheckBox ID="chkEnableEmailOptIn" runat="server" />
                                                                </div>
                                                                <div class="col-md-12">
                                                                    <label>Enable SMS Opt In</label>&nbsp;
                                                                    <asp:CheckBox ID="chkEnableSMSOptIn" runat="server" />
                                                                </div>


                                                                <div class="col-md-12">
                                                                    <label>Enable Localize</label>&nbsp;
                                                                    <asp:CheckBox ID="chkEnableLocalize" runat="server" />
                                                                </div>

                                                                
                                                                <div class="col-md-12">
                                                                    <label>Display New Account Setup Form</label>&nbsp;
                                                                    <asp:CheckBox ID="chkDisplayNewAccountSetupForm" runat="server" />
                                                                </div>
                                                                                                                                
                                                                <div class="col-md-12">
                                                                    <label>Suggested Selling</label>&nbsp;
                                                                    <asp:CheckBox ID="chkSuggestedSelling" runat="server" />
                                                                </div>

                                                                <div class="col-md-12">
                                                                    <label>Banner HTML</label>&nbsp;
                                                                    <asp:TextBox ID="txtBannerHTML" runat="server" TextMode="MultiLine" Width="100%" Height="100px"></asp:TextBox>
                                                                </div>
                                                                <div class="col-md-12">
                                                                    <label>Featured Product HTML</label>&nbsp;
                                                                    <asp:TextBox ID="txtFeaturedProductHTML" runat="server" TextMode="MultiLine" Width="100%" Height="100px"></asp:TextBox>
                                                                </div>

                                                                <div class="col-md-12">
                                                                    <asp:Button ID="btnSave" runat="server" Text="Save" OnClick="btnSave_Click" CssClass="btn btn-sm btn-solid" ValidationGroup="Submit" />
                                                                    <a id="aCancel" runat="server" href="/admin/websiteoverview.aspx" class="btn btn-sm btn-solid">Cancel</a>
                                                                </div>
                                                            </div>
                                                        </div>
                                                    </div>
                                                </div>
                                            </div>
                                        </div>
                                        <div class="tab-pane fade" id="top_2" role="tabpanel" aria-labelledby="top_2_tab">
                                            <div class="row">
                                                <div class="col-12">
                                                    <div class="card dashboard-table mt-0">
                                                        <div class="card-body table-responsive-sm">
                                                            <div class="top-sec">
                                                                <h3></h3>
                                                                <a href="/admin/group.aspx" class="btn btn-sm btn-solid">+ add new</a>
                                                            </div>
                                                            <div class="table-responsive-xl">
                                                                <asp:GridView runat="server" ID="gvWebsiteGroup" AutoGenerateColumns="False" CssClass="table cart-table order-table" HeaderStyle-CssClass="table-head" GridLines="None" Width="100%" AllowSorting="true" CellSpacing="0" CellPadding="0">
                                                                    <Columns>
                                                                        <asp:BoundField DataField="GroupName" HeaderText="Group Name" HeaderStyle-HorizontalAlign="Left" ItemStyle-HorizontalAlign="Left" />
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
                                                                                <a href='/admin/group.aspx?id=<%# Eval("WebsiteGroupID") %>'>
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
                                        <div class="tab-pane fade" id="top_3" role="tabpanel" aria-labelledby="top_3_tab">
                                            <div class="row">
                                                <div class="col-12">
                                                    <div class="card dashboard-table mt-0">
                                                        <div class="card-body table-responsive-sm">
                                                            <div class="top-sec">
                                                                <h3></h3>
                                                                <a href="/admin/userwebsite.aspx" class="btn btn-sm btn-solid">+ add new</a>
                                                            </div>
                                                            <div class="table-responsive-xl">
                                                                <asp:GridView runat="server" ID="gvUserWebsite" AutoGenerateColumns="False" CssClass="table cart-table order-table" HeaderStyle-CssClass="table-head" GridLines="None" Width="100%" AllowSorting="true" CellSpacing="0" CellPadding="0"
                                                                    OnRowCommand="gvUserWebsite_RowCommand">
                                                                    <Columns>
                                                                        <asp:BoundField DataField="UserInfo.FirstName" HeaderText="First Name" HeaderStyle-HorizontalAlign="Left" ItemStyle-HorizontalAlign="Left" />
                                                                        <asp:BoundField DataField="UserInfo.LastName" HeaderText="LastName" HeaderStyle-HorizontalAlign="Left" ItemStyle-HorizontalAlign="Left" />
                                                                        <asp:BoundField DataField="UserInfo.EmailAddress" HeaderText="Email" HeaderStyle-HorizontalAlign="Left" ItemStyle-HorizontalAlign="Left" />
                                                                        <asp:BoundField DataField="IsAdmin" HeaderText="Is Store Admin" HeaderStyle-HorizontalAlign="Left" ItemStyle-HorizontalAlign="Left" />
                                                                        <asp:BoundField DataField="IsPendingApproval" HeaderText="Pending Approval" HeaderStyle-HorizontalAlign="Left" ItemStyle-HorizontalAlign="Left" />
                                                                        <asp:TemplateField HeaderText="Created Date" HeaderStyle-HorizontalAlign="Center" ItemStyle-HorizontalAlign="Center">
                                                                            <ItemTemplate>
                                                                                <%# Eval("CreatedOn") %>
                                                                            </ItemTemplate>
                                                                        </asp:TemplateField>
                                                                        <asp:TemplateField HeaderText="VIEW">
                                                                            <ItemTemplate>
                                                                                <a href='/admin/userwebsite.aspx?id=<%# Eval("UserWebsiteID") %>'>
                                                                                    <i class="fa fa-eye text-theme"></i>
                                                                                </a>
                                                                            </ItemTemplate>
                                                                        </asp:TemplateField>
                                                                        <asp:TemplateField HeaderStyle-HorizontalAlign="Center" ItemStyle-HorizontalAlign="Center" Visible="false">
                                                                            <ItemTemplate>
                                                                                <asp:Button runat="server" ID="btnLineRemove" Text="Remove" CommandName="LineRemove" CommandArgument='<%# Eval("UserWebsiteID") %>' CausesValidation="false"/>
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
                                        <div class="tab-pane fade" id="top_4" role="tabpanel" aria-labelledby="top_4_tab">
                                            <div class="col-12">
                                                <div class="card dashboard-table mt-0">
                                                    <div class="card-body table-responsive-sm">
                                                        <div class="top-sec">
                                                            <h3></h3>
                                                            <a href="/admin/tab.aspx" class="btn btn-sm btn-solid">+ add new</a>
                                                        </div>
                                                        <div class="table-responsive-xl">
                                                            <asp:TreeView ID="tvWebsiteTab" runat="server" OnSelectedNodeChanged="tvWebsiteTab_SelectedNodeChanged"></asp:TreeView>
                                                        </div>
                                                    </div>
                                                </div>
                                            </div>
                                        </div>
                                        <div class="tab-pane fade" id="top_5" role="tabpanel" aria-labelledby="top_5_tab">
                                            <div class="col-12">
                                                <div class="card dashboard-table mt-0">
                                                    <div class="card-body table-responsive-sm">
                                                        <div class="top-sec">
                                                            <h3></h3>
                                                            <a href="/admin/websitecountry.aspx" class="btn btn-sm btn-solid">+ add new</a>
                                                        </div>
                                                        <div class="table-responsive-xl">
                                                            <asp:GridView runat="server" ID="gvWebsiteCountry" AutoGenerateColumns="False" CssClass="table cart-table order-table" HeaderStyle-CssClass="table-head" GridLines="None" Width="100%" AllowSorting="true" CellSpacing="0" CellPadding="0"
                                                                OnRowCommand="gvWebsiteCountry_RowCommand">
                                                                <Columns>
                                                                    <asp:BoundField DataField="CountryCode" HeaderText="Country Code" HeaderStyle-HorizontalAlign="Left" ItemStyle-HorizontalAlign="Left" />
                                                                    <asp:TemplateField HeaderText="Exclude" HeaderStyle-HorizontalAlign="Center" ItemStyle-HorizontalAlign="Center">
                                                                        <ItemTemplate>
                                                                            <asp:CheckBox ID="chkExclude" runat="server" Checked='<%# Eval("Exclude") %>' Enabled="false" /> 
                                                                        </ItemTemplate>
                                                                    </asp:TemplateField>
                                                                    <asp:TemplateField HeaderStyle-HorizontalAlign="Center" ItemStyle-HorizontalAlign="Center">
                                                                        <ItemTemplate>
                                                                            <asp:LinkButton id="btnLineRemove" runat="server" CommandArgument='<%# Eval("WebsiteCountryID") %>' CommandName="LineRemove"><i class="ti-trash" style="font-size:x-large"></i></asp:LinkButton>
                                                                        </ItemTemplate>
                                                                    </asp:TemplateField>
                                                                </Columns>
                                                            </asp:GridView>
                                                        </div>
                                                    </div>
                                                </div>
                                            </div>
                                        </div>

                                        <div class="tab-pane fade" id="top_6" role="tabpanel" aria-labelledby="top_6_tab">
                                            <div class="col-12">
                                                <div class="card dashboard-table mt-0">
                                                    <div class="card-body table-responsive-sm">
                                                        <div class="top-sec">
                                                            <h3></h3>
                                                            <a href="/admin/itempersonalizationapproved.aspx" class="btn btn-sm btn-solid">+ add new</a>
                                                            <asp:LinkButton ID="btnExportItemPersonalizationApproved" runat="server" class="btn btn-sm btn-solid" OnClick="btnExportItemPersonalizationApproved_Click">export</asp:LinkButton>
                                                        </div>
                                                        <div class="table-responsive-xl">
                                                            <asp:GridView runat="server" ID="gvItemPersonalizationApproved" AutoGenerateColumns="False" CssClass="table cart-table order-table" HeaderStyle-CssClass="table-head" GridLines="None" Width="100%" AllowSorting="true" CellSpacing="0" CellPadding="0"
                                                                OnRowCommand="gvItemPersonalizationApproved_RowCommand">
                                                                <Columns>
                                                                    <asp:BoundField DataField="ItemPersonalizationName" HeaderText="Label" HeaderStyle-HorizontalAlign="Left" ItemStyle-HorizontalAlign="Left" />
                                                                    <asp:BoundField DataField="ItemPersonalizationApprovedValue" HeaderText="Value" HeaderStyle-HorizontalAlign="Left" ItemStyle-HorizontalAlign="Left" />
                                                                    <asp:TemplateField HeaderStyle-HorizontalAlign="Center" ItemStyle-HorizontalAlign="Center">
                                                                        <ItemTemplate>
                                                                            <asp:LinkButton id="btnLineRemove" runat="server" CommandArgument='<%# Eval("ItemPersonalizationValueApprovedID") %>' CommandName="LineRemove"><i class="ti-trash" style="font-size:x-large"></i></asp:LinkButton>
                                                                        </ItemTemplate>
                                                                    </asp:TemplateField>
                                                                </Columns>
                                                            </asp:GridView>
                                                            <uc1:Pager runat="server" ID="ucPager" PagingMode="Redirect" PageSize="20" PagingRecordText="Value" />
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
