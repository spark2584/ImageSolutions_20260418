<%@ Page Language="C#" MasterPageFile="~/MasterPage.Master" AutoEventWireup="true" CodeBehind="Account.aspx.cs" Inherits="ImageSolutionsWebsite.Admin.Account" %>
<%@ Register Src="~/Control/AdminNavigation.ascx" TagPrefix="uc" TagName="AdminNavigation" %>
<%@ Register Src="~/Control/Pager.ascx" TagPrefix="uc1" TagName="Pager" %>

<%@ Register src="~/Control/UserWebsiteSearchModal.ascx" tagname="UserWebsiteSearchModal" tagprefix="uc2"  %>
<%@ Register src="~/Control/UserWebsiteSearchModal.ascx" tagname="UserWebsiteSearchModal2" tagprefix="uc2"  %>

<asp:Content ID="cHead" ContentPlaceHolderID="head" runat="server">

       <script type="text/javascript">
           var callTimeout = function (button) {
               setTimeout(function () { button.disabled = false; }, 2000);
           }
       </script>

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
                                                    href="#cphBody_top_1" role="tab" aria-selected="true">
                                                    <i class="icofont icofont-ui-home"></i>Account Management
                                                </a>
                                                <div class="material-border"></div>
                                            </li>
                                            <li class="nav-item">
                                                <a class="nav-link" id="top_2_tab" runat="server" data-bs-toggle="tab"
                                                    href="#cphBody_top_2" role="tab" aria-selected="false" visible="false">
                                                    <i class="icofont icofont-man-in-glasses"></i>Sub Accounts
                                                </a>
                                                <div class="material-border"></div>
                                            </li>
                                            <li class="nav-item">
                                                <a class="nav-link" id="top_3_tab" runat="server" data-bs-toggle="tab"
                                                    href="#cphBody_top_3" role="tab" aria-selected="false">
                                                    <i class="icofont icofont-contacts"></i>Assigned Emails
                                                </a>
                                                <div class="material-border"></div>
                                            </li>
                                            <li class="nav-item">
                                                <a class="nav-link" id="top_4_tab" runat="server" data-bs-toggle="tab"
                                                    href="#cphBody_top_4" role="tab" aria-selected="true">
                                                    <i class="icofont icofont-man-in-glasses"></i>Order Approval
                                                </a>
                                                <div class="material-border"></div>
                                            </li>
                                            <li class="nav-item">
                                                <a class="nav-link" id="top_5_tab" runat="server" data-bs-toggle="tab"
                                                    href="#cphBody_top_5" role="tab" aria-selected="true">
                                                    <i class="icofont icofont-man-in-glasses"></i>Custom Field
                                                </a>
                                                <div class="material-border"></div>
                                            </li>
                                            <li class="nav-item">
                                                <a class="nav-link" id="top_6_tab" runat="server" data-bs-toggle="tab"
                                                    href="#cphBody_top_6" role="tab" aria-selected="true">
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
                                                        <h3>User Management</h3>
                                                    </div>--%>
                                                    <div class="table-responsive-xl">
                                                        <div class="theme-form">
                                                            <div class="form-row row">
                                                                <div class="col-md-12">
                                                                    <label>Account Name</label>
                                                                    <asp:TextBox ID="txtAccountName" runat="server" CssClass="form-control" placeholder="Account Name" required=""></asp:TextBox>
                                                                    <asp:RequiredFieldValidator ID="RequiredFieldValidator1" runat="server" ControlToValidate="txtAccountName" ValidationGroup="Submit" ErrorMessage="*" ForeColor="Red"></asp:RequiredFieldValidator>
                                                                </div>
                                                                <div class="col-md-12">
                                                                    <label>RSI Member Number</label>
                                                                    <asp:TextBox ID="txtRegistrationKey" runat="server" CssClass="form-control" placeholder="RSI Member Number"></asp:TextBox>
                                                                    <asp:RequiredFieldValidator ID="rfvRegistrationKey" runat="server" ControlToValidate="txtRegistrationKey" ValidationGroup="Submit" ErrorMessage="*" ForeColor="Red"></asp:RequiredFieldValidator>
                                                                </div>
                                                                <asp:PlaceHolder ID ="phDefaultWebsiteGroup" runat="server">
                                                                    <div class="col-md-12" id="div1" runat="server">
                                                                        <label>Default Website Group</label>
                                                                        <asp:DropDownList ID="ddlWebsiteGroup" runat="server" DataTextField="GroupName" DataValueField="WebsiteGroupID"></asp:DropDownList>
                                                                    </div>
                                                                </asp:PlaceHolder>
                                                                <div class="col-md-12" id="divParentAccount" runat="server">
                                                                    <label>Parent Account</label>
                                                                    <asp:DropDownList ID="ddlParentAccount" runat="server" DataTextField="AccountNamePath" DataValueField="AccountID" CssClass="form-control"></asp:DropDownList>
                                                                    <br />
                                                                </div>
<%--                                                                <div class="col-md-12">
                                                                    <label>Is Tax Exempt</label>
                                                                    <asp:CheckBox ID="chkIsTaxExempt" runat="server" />
                                                                    <br />
                                                                </div>--%>
                                                                <asp:Panel runat="server" ID="pnlDisplayAddress" Visible="true">
                                                                    <div class="col-md-12">
                                                                        <label>Shipping Address</label>
                                                                        <asp:CheckBox ID="chkAddress" runat="server" Checked="true" OnCheckedChanged="chkAddress_CheckedChanged" AutoPostBack="true"/>
                                                                        <br />
                                                                    </div>
                                                                </asp:Panel>
                                                                <asp:Panel runat="server" ID="pnlAddress" Visible="true">

                                                                    <div class="col-md-12">
                                                                        <h3>Shipping Address</h3>
                                                                    </div>
                                                                    <div class="col-md-12">
                                                                        <label>First Name</label>
                                                                        <asp:TextBox ID="txtFirstName" runat="server" ValidationGroup="login" CssClass="form-control" placeholder="First Name" required=""></asp:TextBox>
                                                                        <asp:RequiredFieldValidator ID="RequiredFieldValidator2" runat="server" ControlToValidate="txtFirstName" ValidationGroup="Submit" ErrorMessage="*" ForeColor="Red"></asp:RequiredFieldValidator>
                                                                    </div>
                                                                    <div class="col-md-12">
                                                                        <label>Last Name</label>
                                                                        <asp:TextBox ID="txtLastName" runat="server" ValidationGroup="login" CssClass="form-control" placeholder="Last Name" required=""></asp:TextBox>
                                                                        <asp:RequiredFieldValidator ID="RequiredFieldValidator3" runat="server" ControlToValidate="txtLastName" ValidationGroup="Submit" ErrorMessage="*" ForeColor="Red"></asp:RequiredFieldValidator>
                                                                    </div>
                                                                    <div class="col-md-12">
                                                                        <label>Address Line 1</label>
                                                                        <asp:TextBox ID="txtAddress" runat="server" ValidationGroup="login" CssClass="form-control" placeholder="Address" required=""></asp:TextBox>
                                                                        <asp:RequiredFieldValidator ID="RequiredFieldValidator4" runat="server" ControlToValidate="txtAddress" ValidationGroup="Submit" ErrorMessage="*" ForeColor="Red"></asp:RequiredFieldValidator>
                                                                    </div>
                                                                    <div class="col-md-12">
                                                                        <label>Address Line 2</label>
                                                                        <asp:TextBox ID="txtAddress2" runat="server" ValidationGroup="login" CssClass="form-control" placeholder="Address 2"></asp:TextBox>
                                                                    </div>
                                                                    <div class="col-md-12">
                                                                        <label>City</label>
                                                                         <asp:TextBox ID="txtCity" runat="server" ValidationGroup="login" CssClass="form-control" placeholder="City" required=""></asp:TextBox>
                                                                        <asp:RequiredFieldValidator ID="RequiredFieldValidator5" runat="server" ControlToValidate="txtCity" ValidationGroup="Submit" ErrorMessage="*" ForeColor="Red"></asp:RequiredFieldValidator>
                                                                    </div>
                                                                    <div class="col-md-12">
                                                                        <label>State</label>
                                                                        <asp:TextBox ID="txtState" runat="server" ValidationGroup="login" CssClass="form-control" placeholder="State"></asp:TextBox>
                                                                        <%--<asp:RequiredFieldValidator ID="RequiredFieldValidator6" runat="server" ControlToValidate="txtState" ValidationGroup="Submit" ErrorMessage="*" ForeColor="Red"></asp:RequiredFieldValidator>--%>
                                                                    </div>
                                                                    <div class="col-md-12">
                                                                        <label>Zip</label>
                                                                        <asp:TextBox ID="txtZip" runat="server" ValidationGroup="login" CssClass="form-control" placeholder="Zip" required=""></asp:TextBox>
                                                                        <asp:RequiredFieldValidator ID="RequiredFieldValidator7" runat="server" ControlToValidate="txtZip" ValidationGroup="Submit" ErrorMessage="*" ForeColor="Red"></asp:RequiredFieldValidator>
                                                                    </div>             
                                                                    <div class="col-md-12">
                                                                        <label>Country</label>
                                                                        <asp:DropDownList ID="ddlCountry" runat="server" DataTextField="Name" DataValueField="Alpha2Code" CssClass="form-control"></asp:DropDownList>
                                                                        <asp:RequiredFieldValidator ID="requiredFieldValidator8" runat="server" ControlToValidate="ddlCountry" ValidationGroup="Submit" ErrorMessage="*" ForeColor="Red"></asp:RequiredFieldValidator>
                                                                        <br />
                                                                    </div>
                                                                    <div class="col-md-12">
                                                                        <label>Phone Number</label>
                                                                        <asp:TextBox ID="txtPhoneNumber" runat="server" ValidationGroup="login" CssClass="form-control" placeholder="phone number"></asp:TextBox>
                                                                        <br />
                                                                    </div>
                                                                </asp:Panel>

                                                                <div class="col-md-12">
                                                                    <label>Get Sub Account Shipment Notification</label>
                                                                    <asp:CheckBox ID="chkGetSubAccountNotification" runat="server"/>
                                                                    <br />
                                                                </div>
                                                                <div class="col-md-12">
                                                                    <label>Is Pending Approval</label>
                                                                    <asp:CheckBox ID="chkIsPendingApproval" runat="server" Enabled="false"/>
                                                                    <br />
                                                                </div>
                                                                <div class="col-md-12">
                                                                    <asp:Button ID="btnSave" runat="server" Text="Save" OnClick="btnSave_Click" CssClass="btn btn-sm btn-solid" OnClientClick="this.disabled=true; callTimeout(this);" ValidationGroup="Submit"  UseSubmitBehavior="false"/>
                                                                    <a id="aCancel" runat="server" href="/Admin/AccountOverview.aspx" class="btn btn-sm btn-solid">Cancel</a>
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
                                                                <a href="/Admin/Account.aspx?parentid=<%=_Account.AccountID %>" class="btn btn-sm btn-solid">+ add new</a>
                                                            </div>
                                                            <div class="table-responsive-xl">
                                                                <asp:GridView runat="server" ID="gvSubAccounts" AutoGenerateColumns="False" CssClass="table cart-table order-table" HeaderStyle-CssClass="table-head" GridLines="None" Width="100%" AllowSorting="true" CellSpacing="0" CellPadding="0" >
                                                                    <Columns>
                                                                        <asp:BoundField DataField="AccountName" HeaderText="Account Name" HeaderStyle-HorizontalAlign="Left" ItemStyle-HorizontalAlign="Left" />
                                                                        <asp:BoundField DataField="ParentAccount.AccountName" HeaderText="Parent Account Name" HeaderStyle-HorizontalAlign="Left" ItemStyle-HorizontalAlign="Left" />
                                                                        <asp:BoundField DataField="RegistrationKey" HeaderText="Registration Key" HeaderStyle-HorizontalAlign="Left" ItemStyle-HorizontalAlign="Left" />
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
                                                                                <a href='/admin/account.aspx?id=<%# Eval("AccountID") %>'>
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
                                                                <a href="/Admin/UserWebsite.aspx?accountid=<%=_Account.AccountID %>" class="btn btn-sm btn-solid">+ add new</a>
                                                            </div>
                                                            <div class="table-responsive-xl">
                                                                <asp:GridView runat="server" ID="gvUserAccounts" AutoGenerateColumns="False" CssClass="table cart-table order-table" HeaderStyle-CssClass="table-head" GridLines="None" Width="100%" AllowSorting="true" CellSpacing="0" CellPadding="0" >
                                                                    <Columns>
                                                                        <asp:BoundField DataField="UserWebsite.UserInfo.EmailAddress" HeaderText="Email" HeaderStyle-HorizontalAlign="Left" ItemStyle-HorizontalAlign="Left" />
                                                                        <asp:BoundField DataField="UserWebsite.UserInfo.FullName" HeaderText="Full Name" HeaderStyle-HorizontalAlign="Left" ItemStyle-HorizontalAlign="Left" />
                                                                        <asp:TemplateField HeaderText="Is Admin" HeaderStyle-HorizontalAlign="Center" ItemStyle-HorizontalAlign="Center" Visible="false">
                                                                            <ItemTemplate>
                                                                                <asp:CheckBox ID="chkIsAdmin" runat="server" Checked='<%# Eval("UserWebsite.IsAdmin") %>' Enabled="false" /> 
                                                                            </ItemTemplate>
                                                                        </asp:TemplateField>
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
                                                                                <a href='/admin/userwebsite.aspx?id=<%# Eval("UserWebsiteID") %>&accountid=<%# Eval("AccountID") %>'>
                                                                                    <i class="fa fa-eye text-theme"></i>
                                                                                </a>
                                                                            </ItemTemplate>
                                                                        </asp:TemplateField>
                                                                    </Columns>
                                                                </asp:GridView>
                                                                <uc1:Pager runat="server" ID="ucUserAccountPager" PagingMode="PostBack" PageSize="20" PagingRecordText="Users" OnPostBackPageIndexChanging="ucUserAccountPager_PostBackPageIndexChanging" />
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
                                                                <a href="/admin/accountorderapproval.aspx?accountid=<%= mAccountID %>" class="btn btn-sm btn-solid">+ add new</a>
                                                            </div>
                                                            <div class="table-responsive-xl">                                                                <asp:GridView runat="server" ID="gvAccountOrderApproval" AutoGenerateColumns="False" CssClass="table cart-table order-table" HeaderStyle-CssClass="table-head" GridLines="None" Width="100%" AllowSorting="true" CellSpacing="0" CellPadding="0">
                                                                    <Columns>
                                                                        <asp:TemplateField HeaderText="Group" HeaderStyle-HorizontalAlign="Center" ItemStyle-HorizontalAlign="Center">
                                                                            <ItemTemplate>
                                                                                <%# Eval("UserWebsite.UserInfo.EmailAddress") %>
                                                                            </ItemTemplate>
                                                                        </asp:TemplateField>
                                                                        <asp:TemplateField HeaderText="Price" HeaderStyle-HorizontalAlign="Center" ItemStyle-HorizontalAlign="Center">
                                                                            <ItemTemplate>
                                                                                <%# string.Format("{0:c}", Eval("Amount")) %>
                                                                            </ItemTemplate>
                                                                        </asp:TemplateField>
                                                                        <asp:TemplateField HeaderText="VIEW">
                                                                            <ItemTemplate>
                                                                                <a href='/admin/accountorderapproval.aspx?id=<%# Eval("AccountOrderApprovalID") %>'>
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

                                        <div class="tab-pane fade" id="top_5" runat="server" role="tabpanel" aria-labelledby="top_5_tab">
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
                                                                                    <asp:Panel ID="pnlCustomList" runat="server">
                                                                                        <table class="table cart-table order-table" cellspacing="0" cellpadding="0" style="width:100%;border-collapse:collapse;">
                                                                                            <tr class="table-head">
                                                                                                <td>
                                                                                                    Value
                                                                                                </td>
                                                                                                <td>
                                                                                                    Remove
                                                                                                </td>
                                                                                            </tr>
                                                                                            <asp:Repeater ID="rptCustomValueList" runat="server" OnItemCommand="rptCustomValueList_ItemCommand">
                                                                                                <ItemTemplate>
                                                                                                    <tr>
                                                                                                        <td>
                                                                                                            <asp:Label ID="lblCustomValueListValue" Text='<%# Eval("Value") %>' runat="server" />
                                                                                                        </td>
                                                                                                        <td>
                                                                                                            <asp:LinkButton ID="lnkDelete" CommandName="Delete" runat="server" Text="x" PostBackUrl='<%# HttpContext.Current.Request.Url.AbsoluteUri.Contains("tab=") ? HttpContext.Current.Request.Url.AbsoluteUri : String.Format("{0}&tab=5", HttpContext.Current.Request.Url.AbsoluteUri) %>'></asp:LinkButton>
                                                                                                            <asp:HiddenField ID="hfCustomFieldID" runat="server" Value='<%# Eval("CustomFieldID")%>'/>
                                                                                                            <asp:HiddenField ID="hfCustomValueListID" runat="server" Value='<%# Eval("CustomValueListID")%>'/>
                                                                                                        </td>
                                                                                                    </tr>
                                                                                                </ItemTemplate>
                                                                                            </asp:Repeater>
                                                                                            <tr>
                                                                                                <td>
                                                                                                    <asp:TextBox ID="txtCustomValueListValue" runat="server"></asp:TextBox>
                                                                                                </td>
                                                                                                <td>
                                                                                                    <asp:LinkButton ID="btnAddCustomValueListValue" runat="server" Text="Add" class="btn btn-sm btn-solid" OnClick="btnAddCustomValueListValue_Click" CausesValidation="false" PostBackUrl='<%# HttpContext.Current.Request.Url.AbsoluteUri.Contains("tab=") ? HttpContext.Current.Request.Url.AbsoluteUri :String.Format("{0}&tab=5", HttpContext.Current.Request.Url.AbsoluteUri) %>'/>
<%--                                                                                                <asp:Button ID="btnAddCustomValueListValue" runat="server" Text="Add" OnClick="btnAddCustomValueListValue_Click" CausesValidation="false" PostBackUrl='<%# HttpContext.Current.Request.Url.AbsoluteUri.Contains("tab=") ? HttpContext.Current.Request.Url.AbsoluteUri :String.Format("{0}&tab=5", HttpContext.Current.Request.Url.AbsoluteUri) %>'/>--%>
                                                                                                </td>
                                                                                            </tr>
                                                                                        </table>
 <%--                                                                                       <asp:ListBox ID="lbCustomValueList" runat="server"></asp:ListBox>
<%--                                                                                        <asp:Button ID="btnListBoxRemove" runat="server" Text="Remove" OnClick="btnListBoxRemove_Click" PostBackUrl='<%# String.Format("{0}&tab=5", HttpContext.Current.Request.Url.AbsoluteUri) %>'/>--%>
                                                                                    </asp:Panel>
                                                                                    <asp:HiddenField ID="hfCustomFieldID" runat="server" Value='<%# Eval("CustomFieldID")%>'/>
                                                                                    <asp:HiddenField ID="hfCustomValueID" runat="server" />
                                                                                </div>
                                                                                <br />
                                                                            </ItemTemplate>
                                                                        </asp:Repeater>         
                                                                        <div class="col-md-12">
                                                                            <br />
                                                                            <asp:Button ID="btnUpdateCustomField" runat="server" Text="Update" OnClick="btnUpdateCustomField_Click" CssClass="btn btn-sm btn-solid" ValidationGroup="UpdateCustomField" OnClientClick="this.disabled='true'; this.value='Please Wait..';" UseSubmitBehavior="false"/>
                                                                        </div>
                                                                    </div>
                                                                </div>
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
                                                            <div class="table-responsive-xl">
                                                                <div class="theme-form">
                                                                    <div class="form-row row">
                                                                        <div class="col-md-12">
                                                                            <label>Company Internal ID</label>
                                                                            <asp:TextBox ID="txtInternalID" runat="server" type="number" CssClass="form-control" min="0" placeholder="Internal ID" ></asp:TextBox>
                                                                        </div>
                                                                        <div class="col-md-12">
                                                                            <label>Site Number</label>
                                                                            <asp:TextBox ID="txtSiteNumber" runat="server" CssClass="form-control" placeholder="Site Number"></asp:TextBox>
                                                                        </div>
                                                                        <div class="col-md-12">
                                                                            <label>Store Number</label>
                                                                            <asp:TextBox ID="txtStoreNumber" runat="server" CssClass="form-control" placeholder="Store Number"></asp:TextBox>
                                                                        </div>
                                                                        <div class="col-md-12">
                                                                            <label>Personalization Approver</label>
                                                                            <%--<asp:DropDownList ID="ddlUser" runat="server" DataTextField="Description" DataValueField="UserWebsiteID" CssClass="form-control"></asp:DropDownList>--%>
                                                                        </div>
                                                                        <div class="col-md-9">
                                                                            <asp:DropDownList ID="ddlUserWebsite" runat="server" DataTextField="Description" DataValueField="UserWebsiteID" CssClass="form-control" Visible="false"></asp:DropDownList>
                                                                            <asp:TextBox ID="txtUserWebsite" runat="server" CssClass="form-control" Enabled="false"></asp:TextBox>
                                                                            <asp:HiddenField ID="hfUserWebsiteID" runat="server" />
                                                                        </div>
                                                                        <div class="col-md-3">
                                                                            <asp:LinkButton ID="btnUserWebsiteSearch" runat="server" OnClick="btnUserWebsiteSearch_Click" Enabled="false"><i class="ti-search" style="font-size:large"></i></asp:LinkButton>
                                                                            <asp:LinkButton ID="btnUserWebsiteRemove" runat="server" OnClick="btnUserWebsiteRemove_Click" Enabled="false"><i class="ti-trash" style="font-size:large"></i></asp:LinkButton>
                                                                        </div>

                                                                        <div class="col-md-12">
                                                                            <label>Personalization Approver 2</label>
                                                                        </div>
                                                                        <div class="col-md-9">
                                                                            <asp:DropDownList ID="ddlUserWebsite2" runat="server" DataTextField="Description" DataValueField="UserWebsiteID" CssClass="form-control" Visible="false"></asp:DropDownList>
                                                                            <asp:TextBox ID="txtUserWebsite2" runat="server" CssClass="form-control" Enabled="false"></asp:TextBox>
                                                                            <asp:HiddenField ID="hfUserWebsiteID2" runat="server" />
                                                                        </div>
                                                                        <div class="col-md-3">
                                                                            <asp:LinkButton ID="btnUserWebsiteSearch2" runat="server" OnClick="btnUserWebsiteSearch2_Click" Enabled="false"><i class="ti-search" style="font-size:large"></i></asp:LinkButton>
                                                                            <asp:LinkButton ID="btnUserWebsiteRemove2" runat="server" OnClick="btnUserWebsiteRemove2_Click" Enabled="false"><i class="ti-trash" style="font-size:large"></i></asp:LinkButton>
                                                                        </div>



                                                                        <%--<div class="col-md-12">
                                                                            <label>Auto Assign Budget</label>
                                                                            <asp:CheckBox ID="chkAutoAssignBudget" runat="server"/>
                                                                            <br />
                                                                        </div>--%>
                                                                        <div class="col-md-12">
                                                                            <label>Budget Setting</label>&nbsp;
                                                                            <asp:DropDownList ID="ddlBudgetSetting" runat="server" CssClass="form-control">
                                                                                <asp:ListItem Selected="True" Value=""></asp:ListItem>
                                                                                <asp:ListItem Value="No Budget">No Budget</asp:ListItem>
                                                                                <asp:ListItem Value="Full Time/Part Time">Full Time/Part Time</asp:ListItem>
                                                                                <asp:ListItem Value="Warehouse/Facilities">Warehouse/Facilities</asp:ListItem>
                                                                                <asp:ListItem Value="Fleet">Fleet</asp:ListItem>
                                                                            </asp:DropDownList>
                                                                        </div>

                                                                        <div class="col-md-12">
                                                                            <label>Do Not Allow Credit Card</label>
                                                                            <asp:CheckBox ID="chkDoNotAllowCreditCard" runat="server" />
                                                                            <br />
                                                                        </div>


                                                                        <div class="col-md-12">
                                                                            <br />
                                                                            <asp:Button ID="btnSave2" runat="server" Text="Save" OnClick="btnSave_Click" CssClass="btn btn-sm btn-solid" OnClientClick="this.disabled=true; callTimeout(this);" ValidationGroup="Submit"  UseSubmitBehavior="false"/>
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
            </div>
        </div>
    </section>
    <!--  dashboard section end -->
</asp:Content>

<asp:Content ID="Content4" ContentPlaceHolderID="cphFooter" runat="server">
    <uc2:UserWebsiteSearchModal ID="ucUserWebsiteSearchModal" runat="server" />
    <uc2:UserWebsiteSearchModal2 ID="ucUserWebsiteSearchModal2" runat="server" />
</asp:Content>

