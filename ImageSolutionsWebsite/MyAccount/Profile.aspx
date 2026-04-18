<%@ Page Title="" Language="C#" MasterPageFile="~/MasterPage.Master" AutoEventWireup="true" CodeBehind="Profile.aspx.cs" Inherits="ImageSolutionsWebsite.MyAccount.Profile" %>
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
                                                <h3>Profile</h3> 
                                            </div>
                                            <div class="table-responsive-xl">
                                                <div class="theme-form">
                                                    <div class="form-row row">
                                                        <div class="col-md-12">
                                                            <label>First Name</label>
                                                            <asp:TextBox ID="txtFirstName" runat="server" CssClass="form-control" placeholder="first name" required=""></asp:TextBox>
                                                            <asp:RequiredFieldValidator ID="RequiredFieldValidator1" runat="server" ControlToValidate="txtFirstName" ValidationGroup="Submit" ErrorMessage="*" ForeColor="Red"></asp:RequiredFieldValidator>
                                                        </div>
                                                        <div class="col-md-12">
                                                            <label>Last Name</label>
                                                            <asp:TextBox ID="txtLastName" runat="server" CssClass="form-control" placeholder="last name" required=""></asp:TextBox>
                                                            <asp:RequiredFieldValidator ID="RequiredFieldValidator2" runat="server" ControlToValidate="txtLastName" ValidationGroup="Submit" ErrorMessage="*" ForeColor="Red"></asp:RequiredFieldValidator>
                                                        </div>
                                                        <asp:Panel ID="pnlEmail" runat="server">
                                                            <div class="col-md-12">
                                                                <label>Email Address</label>
                                                                <asp:TextBox ID="txtEmailAddress" runat="server" CssClass="form-control" placeholder="email" required="" Enabled="false"></asp:TextBox>
                                                                <asp:RequiredFieldValidator ID="RequiredFieldValidator3" runat="server" ControlToValidate="txtEmailAddress" ValidationGroup="Submit" ErrorMessage="*" ForeColor="Red"></asp:RequiredFieldValidator>
                                                            </div>
                                                        </asp:Panel>
                                                        <div class="col-md-12">
                                                            <label>Notification Email Address (if different from Email Address)</label>
                                                            <asp:TextBox ID="txtNotificationEmailAddress" runat="server" CssClass="form-control" placeholder="notification email"></asp:TextBox>
                                                            <asp:RegularExpressionValidator ID="valNotificaitonEmail" runat="server" ValidationExpression="\w+([-+.]\w+)*@\w+([-.]\w+)*\.\w+([-.]\w+)*" ValidationGroup="Submit" ControlToValidate="txtNotificationEmailAddress" ErrorMessage="*" ForeColor="Red"></asp:RegularExpressionValidator>
                                                        </div>
                                                        <div class="col-md-12">
                                                            <asp:Button ID="btnSave" runat="server" Text="Save" OnClick="btnSave_Click" CssClass="btn btn-sm btn-solid" ValidationGroup="Submit" />
                                                            <a id="aCancel" runat="server" href="/myaccount/dashboard.aspx" class="btn btn-sm btn-solid">Cancel</a>
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
