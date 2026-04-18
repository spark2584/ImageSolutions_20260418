<%@ Page Language="C#" MasterPageFile="~/MasterPage.Master" AutoEventWireup="true" CodeBehind="CreditCard.aspx.cs" Inherits="ImageSolutionsWebsite.Admin.CreditCard" %>
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
                                                <a class="nav-link active" id="top_1_tab" runat="server" data-bs-toggle="tab"
                                                    href="#cphBody_top_1" role="tab" aria-selected="true">
                                                    <i class="icofont icofont-ui-home"></i>Credit Card Information
                                                </a>
                                                <div class="material-border"></div>
                                            </li>
                                            <li class="nav-item">
                                                <a class="nav-link" id="top_2_tab" runat="server" data-bs-toggle="tab"
                                                    href="#cphBody_top_2" role="tab" aria-selected="false">
                                                    <i class="icofont icofont-man-in-glasses"></i>Assigned Users
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
                                                                    <label>Nickname</label>
                                                                    <asp:TextBox ID="txtNickname" runat="server" CssClass="form-control" placeholder="Nickname" required=""></asp:TextBox>
                                                                    <asp:RequiredFieldValidator ID="valNickname" runat="server" ControlToValidate="txtNickname" ValidationGroup="Submit" ErrorMessage="*" ForeColor="Red"></asp:RequiredFieldValidator>
                                                                </div>
                                                                <div class="col-md-12">
                                                                    <label>Full Name</label>
                                                                    <asp:TextBox ID="txtFullName" runat="server" CssClass="form-control" placeholder="Full Name" required=""></asp:TextBox>
                                                                    <asp:RequiredFieldValidator ID="valFullName" runat="server" ControlToValidate="txtFullName" ValidationGroup="Submit" ErrorMessage="*" ForeColor="Red"></asp:RequiredFieldValidator>
                                                                </div>
                                                                <div class="col-md-12">
                                                                    <label>Credit Card Number</label>
                                                                    <asp:TextBox ID="txtCreditCardNumber" runat="server" CssClass="form-control" placeholder="Credit Card Number" required="" OnTextChanged="txtCreditCardNumber_TextChanged" AutoPostBack="true"></asp:TextBox>
                                                                    <asp:RequiredFieldValidator ID="valCreditCardNumber" runat="server" ControlToValidate="txtCreditCardNumber" ValidationGroup="Submit" ErrorMessage="*" ForeColor="Red"></asp:RequiredFieldValidator>
                                                                </div>
                                                                <div class="col-md-12">
                                                                    <label>Credit Card Type</label>
                                                                    <asp:TextBox ID="txtCreditCardType" runat="server" CssClass="form-control" Enabled="false"></asp:TextBox>
                                                                    <br />
                                                                </div>
                                                                <div class="col-md-3">
                                                                    <label>Expiration Month</label>
                                                                    <asp:DropDownList ID="ddlExpirationMonth" runat="server" CssClass="form-control"></asp:DropDownList>
                                                                    <asp:RequiredFieldValidator ID="valExpirationMonth" runat="server" ControlToValidate="ddlExpirationMonth" ValidationGroup="Submit" ErrorMessage="*" ForeColor="Red"></asp:RequiredFieldValidator>
                                                                </div>
                                                                <div class="col-md-3">
                                                                    <label>Expiration Year</label>
                                                                    <asp:DropDownList ID="ddlExpirationYear" runat="server" CssClass="form-control"></asp:DropDownList>
                                                                    <asp:RequiredFieldValidator ID="valExpirationYear" runat="server" ControlToValidate="ddlExpirationYear" ValidationGroup="Submit" ErrorMessage="*" ForeColor="Red"></asp:RequiredFieldValidator>
                                                                </div>
                                                                <div class="col-md-3"></div>
                                                                <asp:Panel ID="pnlCVV" runat="server">
                                                                    <div class="col-md-2">
                                                                        <label>CVV</label>
                                                                        <asp:TextBox ID="txtCVV" runat="server" CssClass="form-control" placeholder="CVV" required="" MaxLength="4"></asp:TextBox>
                                                                        <asp:RequiredFieldValidator ID="valCVV" runat="server" ControlToValidate="txtCVV" ValidationGroup="Submit" ErrorMessage="*" ForeColor="Red"></asp:RequiredFieldValidator>
                                                                    </div>
                                                                    <div class="col-md-10"></div>
                                                                </asp:Panel>

                                                                <div class="col-md-12">
                                                                    <h3>Billing Address</h3>
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
                                                                    <label>Address</label>
                                                                    <asp:TextBox ID="txtAddress" runat="server" ValidationGroup="login" CssClass="form-control" placeholder="Address" required=""></asp:TextBox>
                                                                    <asp:RequiredFieldValidator ID="RequiredFieldValidator4" runat="server" ControlToValidate="txtAddress" ValidationGroup="Submit" ErrorMessage="*" ForeColor="Red"></asp:RequiredFieldValidator>
                                                                </div>
                                                                <div class="col-md-12">
                                                                    <label>Address 2</label>
                                                                    <asp:TextBox ID="txtAddress2" runat="server" ValidationGroup="login" CssClass="form-control" placeholder="Address 2"></asp:TextBox>
                                                                </div>
                                                                <div class="col-md-12">
                                                                    <label>City</label>
                                                                     <asp:TextBox ID="txtCity" runat="server" ValidationGroup="login" CssClass="form-control" placeholder="City" required=""></asp:TextBox>
                                                                    <asp:RequiredFieldValidator ID="RequiredFieldValidator5" runat="server" ControlToValidate="txtCity" ValidationGroup="Submit" ErrorMessage="*" ForeColor="Red"></asp:RequiredFieldValidator>
                                                                </div>
                                                                <div class="col-md-12">
                                                                    <label>State</label>
                                                                    <asp:TextBox ID="txtState" runat="server" ValidationGroup="login" CssClass="form-control" placeholder="State" required=""></asp:TextBox>
                                                                    <asp:RequiredFieldValidator ID="RequiredFieldValidator6" runat="server" ControlToValidate="txtState" ValidationGroup="Submit" ErrorMessage="*" ForeColor="Red"></asp:RequiredFieldValidator>
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
                                                                    <asp:Button ID="btnSave" runat="server" Text="Save" OnClick="btnSave_Click" CssClass="btn btn-sm btn-solid" ValidationGroup="Submit" />
                                                                    <a id="aCancel" runat="server" href="/Admin/CreditCardOverview.aspx" class="btn btn-sm btn-solid">Cancel</a>
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
                                                                <a href="/Admin/UserCreditCard.aspx?creditcardid=<%= _CreditCard.CreditCardID %>" class="btn btn-sm btn-solid">+ add new</a>
                                                                <asp:LinkButton ID="btnImport" runat="server" class="btn btn-sm btn-solid" OnClick="btnImport_Click">import</asp:LinkButton>
                                                                <asp:LinkButton ID="btnExport" runat="server" class="btn btn-sm btn-solid" OnClick="btnExport_Click">export</asp:LinkButton>
                                                            </div>
                                                            <div class="table-responsive-xl">                                                                <asp:GridView runat="server" ID="gvUserCreditCard" AutoGenerateColumns="False" CssClass="table cart-table order-table" HeaderStyle-CssClass="table-head" GridLines="None" Width="100%" AllowSorting="true" CellSpacing="0" CellPadding="0" >
                                                                    <Columns>
                                                                        <asp:BoundField DataField="UserInfo.FullName" HeaderText="User" HeaderStyle-HorizontalAlign="Left" ItemStyle-HorizontalAlign="Left" />
                                                                        <asp:BoundField DataField="UserInfo.EmailAddress" HeaderText="Email" HeaderStyle-HorizontalAlign="Left" ItemStyle-HorizontalAlign="Left" />
                                                                        <asp:BoundField DataField="ResetDayOfTheMonth" HeaderText="Reset Day Of The Month" HeaderStyle-HorizontalAlign="Left" ItemStyle-HorizontalAlign="Left" />
                                                                        <%--<asp:BoundField DataField="Limit" HeaderText="Limit" HeaderStyle-HorizontalAlign="Left" ItemStyle-HorizontalAlign="Left" />--%>
                                                                        <asp:TemplateField HeaderText="Limit" HeaderStyle-HorizontalAlign="Left" ItemStyle-HorizontalAlign="Left">
                                                                            <ItemTemplate>
                                                                                <%# Eval("Limit") == null ? "" : "$" + Eval("Limit").ToString() %>
                                                                            </ItemTemplate>
                                                                        </asp:TemplateField>
                                                                        <%--<asp:BoundField DataField="RemainingBalance" HeaderText="Remaining Balance" HeaderStyle-HorizontalAlign="Left" ItemStyle-HorizontalAlign="Left" />--%>
                                                                        <asp:TemplateField HeaderText="RemainingBalance" HeaderStyle-HorizontalAlign="Left" ItemStyle-HorizontalAlign="Left">
                                                                            <ItemTemplate>
                                                                                <%# Eval("RemainingBalance") == null ? "" : "$" + Eval("RemainingBalance").ToString() %>
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
                                                                                <a href='/admin/UserCreditCard.aspx?id=<%# Eval("UserCreditCardID") %>'>
                                                                                    <i class="fa fa-eye text-theme"></i>
                                                                                </a>
                                                                            </ItemTemplate>
                                                                        </asp:TemplateField>
                                                                    </Columns>
                                                                </asp:GridView>
                                                                <uc1:Pager runat="server" ID="ucUserCreditCardPager" PagingMode="PostBack" PageSize="20" PagingRecordText="Users" OnPostBackPageIndexChanging="ucUserCreditCardPager_PostBackPageIndexChanging"/>
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