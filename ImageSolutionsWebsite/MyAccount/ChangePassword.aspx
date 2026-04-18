<%@ Page Language="C#" MasterPageFile="~/MasterPage.Master" AutoEventWireup="true" CodeBehind="ChangePassword.aspx.cs" Inherits="ImageSolutionsWebsite.MyAccount.ChangePassword" %>
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
                                                <h3>Change Password</h3> 
                                            </div>
                                            <div class="table-responsive-xl">
                                                <div class="theme-form">
                                                    <div class="form-row row">
                                                        <div class="col-md-12">
                                                            <label>Current Password</label>
                                                            <asp:TextBox ID="txtPassword" runat="server" CssClass="form-control" placeholder="Current Password" TextMode="Password" required=""></asp:TextBox>
                                                            <asp:RequiredFieldValidator ID="valPassword" runat="server" ControlToValidate="txtPassword" ValidationGroup="Submit" ErrorMessage="*" ForeColor="Red"></asp:RequiredFieldValidator>
                                                        </div>
                                                        <div class="col-md-12">
                                                            <label>New Password</label>
                                                            <asp:TextBox ID="txtNewPassword" runat="server" CssClass="form-control" placeholder="New Password" TextMode="Password" required=""></asp:TextBox>
                                                            <asp:RequiredFieldValidator ID="valNewPassword" runat="server" ControlToValidate="txtNewPassword" ValidationGroup="Submit" ErrorMessage="*" ForeColor="Red"></asp:RequiredFieldValidator>
                                                        </div>
                                                        <div class="col-md-12">
                                                            <label>Confirm New Password</label>
                                                            <asp:TextBox ID="txtConfirmNewPassword" runat="server" CssClass="form-control" placeholder="Confirm New Password" TextMode="Password" required=""></asp:TextBox>
                                                            <asp:RequiredFieldValidator ID="valConfirmNewPassword" runat="server" ControlToValidate="txtConfirmNewPassword" ValidationGroup="Submit" ErrorMessage="*" ForeColor="Red"></asp:RequiredFieldValidator>
                                                            <asp:CompareValidator ID="compvalConfirmNewPassword" runat="server" ControlToCompare="txtNewpassword" ControlToValidate="txtConfirmNewPassword" ValidationGroup="Submit" ErrorMessage="*" ForeColor="Red"></asp:CompareValidator>
                                                        </div>
                                                        <div class="col-md-12">
                                                            <asp:Button ID="btnSave" runat="server" Text="Save" OnClick="btnSave_Click" CssClass="btn btn-sm btn-solid" ValidationGroup="Submit" />
                                                            <a id="aCancel" runat="server" href="/myaccount/dashboard.aspx" class="btn btn-sm btn-solid">Cancel</a>
                                                        </div>
                                                        <div class="col-md-12">
                                                            <asp:Label ID="lblMessage" runat="server" ForeColor="Red"></asp:Label>
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