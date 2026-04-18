<%@ Page Title="" Language="C#" MasterPageFile="~/MasterPage.Master" AutoEventWireup="true" CodeBehind="AddressBookEditMode.aspx.cs" Inherits="ImageSolutionsWebsite.MyAccount.AddressBookEditMode" %>
<%@ Register Src="~/Control/MyAccountNavigation.ascx" TagPrefix="uc1" TagName="MyAccountNavigation" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="cphHeader" runat="server">
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="cphBody" runat="server">
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
                                                <h3>Address Book</h3> 
                                            </div>
                                            <div class="table-responsive-xl">
                                                <div class="theme-form">
                                                    <div class="form-row row">
                                                        <div class="col-md-12">
                                                            <label>Company Name</label>
                                                            <asp:TextBox ID="txtAddressLabel" runat="server" ValidationGroup="login" CssClass="form-control" placeholder="addressee"></asp:TextBox>
                                                        </div>
                                                        <div class="col-md-12">
                                                            <label>First Name</label>
                                                            <asp:TextBox ID="txtFirstName" runat="server" ValidationGroup="login" CssClass="form-control" placeholder="First Name" required=""></asp:TextBox>
                                                            <asp:RequiredFieldValidator ID="RequiredFieldValidator1" runat="server" ControlToValidate="txtFirstName" ValidationGroup="Submit" ErrorMessage="*" ForeColor="Red"></asp:RequiredFieldValidator>
                                                        </div>
                                                        <div class="col-md-12">
                                                            <label>Last Name</label>
                                                            <asp:TextBox ID="txtLastName" runat="server" ValidationGroup="login" CssClass="form-control" placeholder="Last Name" required=""></asp:TextBox>
                                                            <asp:RequiredFieldValidator ID="RequiredFieldValidator2" runat="server" ControlToValidate="txtLastName" ValidationGroup="Submit" ErrorMessage="*" ForeColor="Red"></asp:RequiredFieldValidator>
                                                        </div>
                                                        <div class="col-md-12">
                                                            <label>Address</label>
                                                            <asp:TextBox ID="txtAddress" runat="server" ValidationGroup="login" CssClass="form-control" placeholder="Address" required=""></asp:TextBox>
                                                        </div>
                                                        <div class="col-md-12">
                                                            <label>Address 2 (Suite or Apt #)</label>
                                                            <asp:TextBox ID="txtAddress2" runat="server" ValidationGroup="login" CssClass="form-control" placeholder="Address 2"></asp:TextBox>
                                                        </div>
                                                        <div class="col-md-12">
                                                            <label>City</label>
                                                             <asp:TextBox ID="txtCity" runat="server" ValidationGroup="login" CssClass="form-control" placeholder="City" required=""></asp:TextBox>
                                                        </div>
                                                        <div class="col-md-12">
                                                            <label>State</label>
                                                            <asp:TextBox ID="txtState" runat="server" ValidationGroup="login" CssClass="form-control" placeholder="State"></asp:TextBox>
                                                        </div>
                                                        <div class="col-md-12">
                                                            <label>Zip</label>
                                                             <asp:TextBox ID="txtZip" runat="server" ValidationGroup="login" CssClass="form-control" placeholder="Zip" required=""></asp:TextBox>
                                                        </div>
                                                        <div class="col-md-12">
                                                            <label>Country</label>
                                                            <asp:DropDownList ID="ddlCountry" runat="server" DataTextField="Name" DataValueField="Alpha2Code" CssClass="form-control"></asp:DropDownList>
                                                            <asp:RequiredFieldValidator ID="RequiredFieldValidator3" runat="server" ControlToValidate="ddlCountry" ValidationGroup="Submit" ErrorMessage="*" ForeColor="Red"></asp:RequiredFieldValidator>
                                                        </div>
                                                        <div class="col-md-12">
                                                            <label>Phone Number</label>
                                                            <asp:TextBox ID="txtPhone" runat="server" ValidationGroup="login" CssClass="form-control" placeholder="phone number"></asp:TextBox>
                                                        </div>
                                                        <div class="col-md-12">
                                                            <label>Default Shipping</label>
                                                            <asp:CheckBox ID="cbDefaultShipping" runat="server" />
                                                        </div>
                                                        <div class="col-md-12">
                                                            <label>Default Billing</label>
                                                            <asp:CheckBox ID="cbDefaultBilling" runat="server" />
                                                        </div>
                                                        <asp:Panel ID="pnlSkipAddressValidation" runat="server" Visible="false">
                                                            <div class="col-md-12">
                                                                <label>Skip Address Validation</label>
                                                                <asp:CheckBox ID="cbSkipAddressValidation" runat="server" />
                                                            </div>
                                                        </asp:Panel>
                                                        <div class="col-md-12">
                                                            <asp:Button ID="btnSave" runat="server" Text="Save" OnClick="btnSave_Click" CssClass="btn btn-sm btn-solid" ValidationGroup="Submit" />
                                                            <a id="aCancel" runat="server" href="/myaccount/dashboard.aspx" class="btn btn-sm btn-solid">Cancel</a>
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