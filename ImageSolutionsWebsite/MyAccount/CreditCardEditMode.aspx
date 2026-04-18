<%@ Page Language="C#" MasterPageFile="~/MasterPage.Master" AutoEventWireup="true" CodeBehind="CreditCardEditMode.aspx.cs" Inherits="ImageSolutionsWebsite.MyAccount.CreditCardEditMode" %>
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
                                                <h3>Credit Card</h3> 
                                            </div>
                                            <div class="table-responsive-xl">
                                                <div class="theme-form">
                                                    <div class="form-row row">
                                                        <div class="col-md-12">
                                                            <label>Nickname</label>
                                                            <asp:TextBox ID="txtNickname" runat="server" CssClass="form-control"></asp:TextBox>
                                                            <asp:RequiredFieldValidator ID="valNickname" runat="server" ControlToValidate="txtNickname" ValidationGroup="Submit" ErrorMessage="*" ForeColor="Red"></asp:RequiredFieldValidator>
                                                        </div>
                                                        <div class="col-md-12">
                                                            <label>Full Name</label>
                                                            <asp:TextBox ID="txtFullName" runat="server" CssClass="form-control"></asp:TextBox>
                                                            <asp:RequiredFieldValidator ID="valFullName" runat="server" ControlToValidate="txtFullName" ValidationGroup="Submit" ErrorMessage="*" ForeColor="Red"></asp:RequiredFieldValidator>
                                                        </div>
                                                        <div class="col-md-12">
                                                            <label>Credit Card Number</label>
                                                            <asp:TextBox ID="txtCreditCardNumber" runat="server" OnTextChanged="txtCreditCardNumber_TextChanged" AutoPostBack="true" CssClass="form-control"></asp:TextBox>
                                                            <asp:RequiredFieldValidator ID="valCreditCardNumber" runat="server" ControlToValidate="txtCreditCardNumber" ValidationGroup="Submit" ErrorMessage="*" ForeColor="Red"></asp:RequiredFieldValidator>
                                                        </div>
                                                        <div class="col-md-12">
                                                            <label>Credit Card Type</label>
                                                            <asp:TextBox ID="txtCreditCardType" runat="server" Enabled="false" CssClass="form-control"></asp:TextBox>
                                                            <asp:RequiredFieldValidator ID="valCreditCardType" runat="server" ControlToValidate="txtCreditCardType" ValidationGroup="Submit" ErrorMessage="*" ForeColor="Red"></asp:RequiredFieldValidator>
                                                        </div>
                                                        <div class="col-md-12">
                                                            <label>Expiration Month</label>
                                                            <asp:DropDownList ID="ddlExpirationMonth" runat="server" CssClass="form-control"></asp:DropDownList>
                                                            <asp:RequiredFieldValidator ID="valExpirationMonth" runat="server" ControlToValidate="ddlExpirationMonth" ValidationGroup="Submit" ErrorMessage="*" ForeColor="Red"></asp:RequiredFieldValidator>
                                                        </div>
                                                        <div class="col-md-12">
                                                            <label>Expiration Year</label>
                                                            <asp:DropDownList ID="ddlExpirationYear" runat="server" CssClass="form-control"></asp:DropDownList>
                                                            <asp:RequiredFieldValidator ID="valExpirationYear" runat="server" ControlToValidate="ddlExpirationYear" ValidationGroup="Submit" ErrorMessage="*" ForeColor="Red"></asp:RequiredFieldValidator>
                                                        </div>
                                                        <asp:Panel ID="pnlCVV" runat="server">
                                                            <div class="col-md-12">
                                                                <label>CVV</label>
                                                                <asp:TextBox ID="txtCVV" runat="server" CssClass="form-control"></asp:TextBox>
                                                                <asp:RequiredFieldValidator ID="valCVV" runat="server" ControlToValidate="txtCVV" ValidationGroup="Submit" ErrorMessage="*" ForeColor="Red"></asp:RequiredFieldValidator>
                                                            </div>
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
                                                            <br />
                                                        </div>
                                                        <div class="form-group col-md-12 col-sm-6 col-xs-12">
                                                            <div class="field-label">Country</div>
                                                            <asp:DropDownList ID="ddlCountry" runat="server" DataTextField="Name" DataValueField="Alpha2Code" CssClass="form-control"></asp:DropDownList>
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
                            </div>
                        </div>
                    </div>
                </div>
            </div>
        </div>
    </section>
    <!--  dashboard section end -->
</asp:Content>