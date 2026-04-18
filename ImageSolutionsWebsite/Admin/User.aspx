<%@ Page Language="C#" MasterPageFile="~/MasterPage.Master" AutoEventWireup="true" CodeBehind="User.aspx.cs" Inherits="ImageSolutionsWebsite.Admin.User" %>
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
                                                <a class="nav-link active" id="top_1_tab" data-bs-toggle="tab"
                                                    href="#top_1" role="tab" aria-selected="true">
                                                    <i class="icofont icofont-ui-home"></i>User Info
                                                </a>
                                                <div class="material-border"></div>
                                            </li>
                                            <li class="nav-item">
                                                <a class="nav-link" id="top_2_tab" data-bs-toggle="tab"
                                                    href="#top_2" role="tab" aria-selected="false">
                                                    <i class="icofont icofont-man-in-glasses"></i>
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
                                                        <h3>User Management</h3>
                                                    </div>--%>
                                                    <div class="table-responsive-xl">
                                                        <div class="theme-form">
                                                            <div class="form-row row">
                                                                <div class="col-md-12">
                                                                    <label>Email</label>
                                                                    <asp:TextBox ID="txtEmail" runat="server" CssClass="form-control" placeholder="email address" required=""></asp:TextBox>
                                                                    <asp:RequiredFieldValidator ID="valEmail" runat="server" ControlToValidate="txtEmail" ValidationGroup="Submit" ErrorMessage="*" ForeColor="Red"></asp:RequiredFieldValidator>
                                                                </div>
                                                                <div class="col-md-6">
                                                                    <label>First Name</label>
                                                                    <asp:TextBox ID="txtFirstName" runat="server" CssClass="form-control" placeholder="first name" required="true"></asp:TextBox>
                                                                    <asp:RequiredFieldValidator ID="valFirstName" runat="server" ControlToValidate="txtFirstName" ValidationGroup="Submit" ErrorMessage="*" ForeColor="Red"></asp:RequiredFieldValidator>
                                                                </div>
                                                                <div class="col-md-6">
                                                                    <label>Last Name</label>
                                                                    <asp:TextBox ID="txtLastName" runat="server" CssClass="form-control" placeholder="last name" required="true"></asp:TextBox>
                                                                    <asp:RequiredFieldValidator ID="valLastName" runat="server" ControlToValidate="txtLastName" ValidationGroup="Submit" ErrorMessage="*" ForeColor="Red"></asp:RequiredFieldValidator>
                                                                </div>
                                                                <div class="col-md-12">
                                                                    <label>Is Admin</label>&nbsp;
                                                                    <asp:CheckBox ID="cbIsAdmin" runat="server" />
                                                                </div>
                                                                <div class="col-md-12">
                                                                    <label>Inactive</label>&nbsp;
                                                                    <asp:CheckBox ID="cbInactive" runat="server" />
                                                                </div>
                                                                <div class="col-md-12">
                                                                    <asp:Button ID="btnSave" runat="server" Text="Save" OnClick="btnSave_Click" CssClass="btn btn-sm btn-solid" ValidationGroup="Submit" />
                                                                    <a id="aCancel" runat="server" href="/Admin/UserOverview.aspx" class="btn btn-sm btn-solid">Cancel</a>
                                                                    <asp:Button ID="btnDelete" runat="server" Text="Delete" OnClick="btnDelete_Click" CssClass="btn btn-sm btn-solid" CausesValidation="false" />
                                                                </div>
                                                            </div>
                                                        </div>
                                                    </div>
                                                </div>
                                            </div>
                                        </div>
                                        <div class="tab-pane fade" id="top_2" role="tabpanel" aria-labelledby="top_2_tab"></div>
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
