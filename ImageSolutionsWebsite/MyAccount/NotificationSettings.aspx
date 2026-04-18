<%@ Page Title="" Language="C#" MasterPageFile="~/MasterPage.Master" AutoEventWireup="true" CodeBehind="NotificationSettings.aspx.cs" Inherits="ImageSolutionsWebsite.MyAccount.NotificationSettings" %>
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
                                                <h3>Notification Settings</h3> 
                                            </div>
                                            <div class="table-responsive-xl">
                                                <div class="theme-form">
                                                    <div class="form-row row">
                                                        <div class="col-md-12">
                                                            <b>Want to stay up to date?</b><br />
                                                            Sign up to get order confirmation, order tracking & budget updates!<br />
                                                            <asp:CustomValidator ID="cvdOptIn" runat="server" ForeColor="Red" Display="Dynamic" OnServerValidate="cvdOptIn_ServerValidate"></asp:CustomValidator>
                                                        </div>

                                                        <asp:PlaceHolder ID="phEnableEmailOptIn" runat="server">
                                                            <div class="col-md-12 pt-4">
                                                                <label>Notification Email</label><asp:RegularExpressionValidator ID="reqEmail" runat="server" ControlToValidate="txtNotificationEmail" ValidationExpression="^[\w-\.]+@([\w-]+\.)+[\w-]{2,4}$" ErrorMessage="Invalid email address" Display="Dynamic" ForeColor="Red"></asp:RegularExpressionValidator>
                                                                <asp:TextBox ID="txtNotificationEmail" runat="server" CssClass="form-control" placeholder="Enter Email" /><br />
                                                            </div>
                                                        </asp:PlaceHolder>

                                                        <asp:PlaceHolder ID="phEnableSMSOptIn" runat="server">
                                                            <div class="col-md-6 pt-4">
                                                                <label>Mobile Phone Number</label><asp:RegularExpressionValidator ID="regMobileNumber" runat="server" ControlToValidate="txtMobileNumber" ValidationExpression="^[0-9]{10}$" ForeColor="Red" ErrorMessage="Please enter a 10-digit phone number." />
                                                                <div class="input-group">
                                                                    <span class="input-group-text">+1 (US)</span>
                                                                    <asp:TextBox ID="txtMobileNumber" runat="server" MaxLength="10" CssClass="form-control" placeholder="Enter Phone Number" />
                                                                </div><br />
                                                            </div>
                                                        </asp:PlaceHolder>
                                                        <div class="col-md-6 pt-4"></div>
                                                        <div class="col-md-12 pt-4">
                                                            <asp:Button ID="btnSave" runat="server" Text="Save" OnClick="btnSave_Click" CssClass="btn btn-sm btn-solid" />
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

<asp:Content ID="Content4" ContentPlaceHolderID="cphFooter" runat="server">
</asp:Content>
