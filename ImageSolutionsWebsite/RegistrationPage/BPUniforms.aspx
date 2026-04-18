<%@ Page Language="C#" MasterPageFile="~/MasterPage.Master" AutoEventWireup="true" CodeBehind="BPUniforms.aspx.cs" Inherits="ImageSolutionsWebsite.RegistrationPage.BPUniforms" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="cphHeader" runat="server">
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="cphBody" runat="server">
    <section class="login-page section-b-space">
        <div class="container">
            <div class="row">
                <div class="col-lg-12">
                    <h3>User Account Registration</h3>
                    <div class="theme-card">
                        <div class="theme-form">
                            <div class="form-group">
                                <label for="ddlAccount">Banner</label>
                                <asp:DropDownList ID="ddlAccount" runat="server" DataValueField="AccountID" DataTextField="AccountName" CssClass="form-control form-select-sm form-select"></asp:DropDownList>
                                <asp:RequiredFieldValidator ID="RequiredFieldValidator6" runat="server" ControlToValidate="ddlAccount" ValidationGroup="register" ErrorMessage="*" ForeColor="Red"></asp:RequiredFieldValidator>     
                            </div>
                            <div class="form-group">
                                <label for="txtFirstName">First Name</label>
                                <asp:TextBox ID="txtFirstName" runat="server" ValidationGroup="register" CssClass="form-control" placeholder="First Name" required=""></asp:TextBox>
                                <asp:RequiredFieldValidator ID="RequiredFieldValidator3" runat="server" ControlToValidate="txtFirstName" ValidationGroup="register" ErrorMessage="*" ForeColor="Red"></asp:RequiredFieldValidator>     
                            </div>
                            <div class="form-group">
                                <label for="txtLastName">Last Name</label>
                                <asp:TextBox ID="txtLastName" runat="server" ValidationGroup="register" CssClass="form-control" placeholder="Last Name" required=""></asp:TextBox>
                                <asp:RequiredFieldValidator ID="RequiredFieldValidator1" runat="server" ControlToValidate="txtLastName" ValidationGroup="register" ErrorMessage="*" ForeColor="Red"></asp:RequiredFieldValidator>     
                            </div>
                            <div class="form-group">
                                <label for="txtEmail">Email Address</label> <%--<span style="margin-left:20px; color:red">( Personal email address only; do not use work email )</span>--%> 
                                <asp:TextBox ID="txtEmail" runat="server" ValidationGroup="register" CssClass="form-control" placeholder="Email" required=""></asp:TextBox>
                                <asp:RequiredFieldValidator ID="RequiredFieldValidator2" runat="server" ControlToValidate="txtEmail" ValidationGroup="register" ErrorMessage="*" ForeColor="Red"></asp:RequiredFieldValidator>     
                                <asp:RegularExpressionValidator ID="valEmail" runat="server" ControlToValidate="txtEmail" ValidationGroup="register" ErrorMessage="Invalid Email" ValidationExpression="\w+([-+.]\w+)*@\w+([-.]\w+)*\.\w+([-.]\w+)*" ForeColor="Red" />
                            </div>
                            <div class="form-group">
                                <label for="txtPassword">Password</label>
                                <asp:TextBox ID="txtPassword" runat="server" TextMode="Password" CssClass="form-control" placeholder="Password" required=""></asp:TextBox>
                                <asp:RequiredFieldValidator ID="RequiredFieldValidator4" runat="server" ControlToValidate="txtPassword" ValidationGroup="register" ErrorMessage="*" ForeColor="Red"></asp:RequiredFieldValidator>
                                <asp:RegularExpressionValidator ID="valPassword" runat="server" ControlToValidate="txtPassword" ValidationGroup="register" ErrorMessage="Password must be between 6 to 20 characters" ValidationExpression=".{6,20}.*" ForeColor="Red" />
                            </div>
                            <div class="form-group">
                                <label for="txtPasswordConfirm">Confirm Password</label>
                                <asp:TextBox ID="txtPasswordConfirm" runat="server" TextMode="Password" CssClass="form-control" placeholder="Password Confirm" required=""></asp:TextBox>
                                <asp:RequiredFieldValidator ID="RequiredFieldValidator5" runat="server" ControlToValidate="txtPasswordConfirm" ValidationGroup="register" ErrorMessage="*" ForeColor="Red"></asp:RequiredFieldValidator>
                                <asp:CompareValidator ID="CompareValidator1" runat="server" ControlToCompare="txtPassword" ControlToValidate="txtPasswordConfirm" ValidationGroup="register" ForeColor="Red" Display="Dynamic" ErrorMessage="Passwords do not Match"></asp:CompareValidator>
                            </div>
                            <asp:Button ID="btnRegister" runat="server" Text="Register" OnClick="btnRegister_Click" ValidationGroup="register" CssClass="btn btn-solid"/>
                        </div>
                    </div>
                </div>
            </div>
        </div>
    </section>
</asp:Content>