<%@ Page Language="C#" MasterPageFile="~/MasterPage.Master" AutoEventWireup="true" CodeBehind="CreateUser.aspx.cs" Inherits="ImageSolutionsWebsite.Admin.CreateUser" %>
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
                        <div class="tab-pane fade show active" id="orders">
                            <div class="row">
                                <div class="col-12">
                                    <div class="card dashboard-table mt-0">
                                        <div class="card-body table-responsive-sm">
                                            <div class="top-sec">
                                                <h3>User Management</h3> 
                                            </div>
                                            <div class="table-responsive-xl">
                                                <div class="theme-form">
                                                    <div class="form-row row">
                                                        <div class="col-md-12">
                                                            <label>Email</label>
                                                            <asp:TextBox ID="txtEmail" runat="server" CssClass="form-control" placeholder="Email" required=""></asp:TextBox>
                                                            <asp:RequiredFieldValidator ID="RequiredFieldValidator1" runat="server" ControlToValidate="txtEmail" ValidationGroup="Submit" ErrorMessage="*" ForeColor="Red"></asp:RequiredFieldValidator>
                                                        </div>
                                                        <div class="col-md-12">
                                                            <label>Email</label>
                                                            <asp:TextBox ID="TextBox1" runat="server" CssClass="form-control" placeholder="Email" required=""></asp:TextBox>
                                                            <asp:RequiredFieldValidator ID="RequiredFieldValidator2" runat="server" ControlToValidate="txtEmail" ValidationGroup="Submit" ErrorMessage="*" ForeColor="Red"></asp:RequiredFieldValidator>
                                                        </div>
                                                        <div class="col-md-12">
                                                            <label>Email</label>
                                                            <asp:TextBox ID="TextBox2" runat="server" CssClass="form-control" placeholder="Email" required=""></asp:TextBox>
                                                            <asp:RequiredFieldValidator ID="RequiredFieldValidator3" runat="server" ControlToValidate="txtEmail" ValidationGroup="Submit" ErrorMessage="*" ForeColor="Red"></asp:RequiredFieldValidator>
                                                        </div>
                                                        <div class="col-md-12">
                                                            <asp:Button ID="btnEdit" runat="server" Text="Save" OnClick="btnEdit_Click" CssClass="btn btn-sm btn-solid" ValidationGroup="Submit" />
                                                            <asp:Button ID="btnCancel" runat="server" Text="Cancel" OnClick="btnCancel_Click" CssClass="btn btn-sm btn-solid" CausesValidation="false"/>
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

    <table style="width:100%">
        <tr>
            <td colspan="2">
                <h1>Create New User</h1>
            </td>
        </tr>
        <tr>
            <td>
                <asp:Button ID="" runat="server" Text="Back" OnClick="btnCancel_Click" />
            </td>
        </tr>
        <tr>
            <td>
                Email
            </td>
            <td>
                <asp:TextBox ID="" runat="server"></asp:TextBox>
                <asp:RequiredFieldValidator ID="valEmail" runat="server" ControlToValidate="txtEmail" ValidationGroup="Submit" ErrorMessage="*" ForeColor="Red"></asp:RequiredFieldValidator>
            </td>
        </tr>
        <tr>
            <td>
                First Name
            </td>
            <td>
                <asp:TextBox ID="txtFirstName" runat="server"></asp:TextBox>
                <asp:RequiredFieldValidator ID="valFirstName" runat="server" ControlToValidate="txtFirstName" ValidationGroup="Submit" ErrorMessage="*" ForeColor="Red"></asp:RequiredFieldValidator>
            </td>
        </tr>
        <tr>
            <td>
                Last Name
            </td>
            <td>
                <asp:TextBox ID="txtLastName" runat="server"></asp:TextBox>
                <asp:RequiredFieldValidator ID="valLastName" runat="server" ControlToValidate="txtLastName" ValidationGroup="Submit" ErrorMessage="*" ForeColor="Red"></asp:RequiredFieldValidator>
            </td>
        </tr>
        <tr>
            <td>
                Password
            </td>
            <td>
                <asp:TextBox ID="txtPassword" runat="server" TextMode="Password"></asp:TextBox>
                <asp:RequiredFieldValidator ID="valPassword" runat="server" ControlToValidate="txtPassword" ValidationGroup="Submit" ErrorMessage="*" ForeColor="Red"></asp:RequiredFieldValidator>
            </td>
        </tr>
        <tr>
            <td>
                Confirm Password
            </td>
            <td>
                <asp:TextBox ID="txtConfirmPassword" runat="server" TextMode="Password"></asp:TextBox>
                <asp:CompareValidator ID="valConfirmPassword" runat="server" ControlToValidate="txtConfirmPassword" ControlToCompare="txtPassword" ValidationGroup="Submit" ErrorMessage="*" ForeColor="Red"></asp:CompareValidator>
            </td>
        </tr>
        <tr>
            <td>
                IsAdmin
            </td>
            <td>
                <asp:CheckBox ID="cbIsAdmin" runat="server" />
            </td>
        </tr>
        <tr>
            <td colspan="2">
                <asp:Button ID="" runat="server" Text="Create" OnClick="btnCreate_Click" ValidationGroup="Submit" />
            </td>
        </tr>
    </table>
</asp:Content>
