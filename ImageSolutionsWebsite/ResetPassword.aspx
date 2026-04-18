<%@ Page Language="C#" MasterPageFile="~/MasterPage.Master" AutoEventWireup="true" CodeBehind="ResetPassword.aspx.cs" Inherits="ImageSolutionsWebsite.ResetPassword" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="cphHeader" runat="server">
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="cphBody" runat="server">
    
    <!--section start-->
    <section class="pwd-page section-b-space">
        <div class="container">
            <div class="row">
                <div class="col-lg-6 m-auto">
                    <h2>Reset Your Password</h2>
                    <form class="theme-form">
                        <div class="form-group">
                            <label for="review">New Password</label>
                            <asp:TextBox ID="txtPassword" runat="server" TextMode="Password" ValidationGroup="password" CssClass="form-control" placeholder="New Password"></asp:TextBox>
                            <asp:RequiredFieldValidator ID="valPassword" runat="server" ControlToValidate="txtPassword" ErrorMessage="*" ForeColor="Red" ValidationGroup="password"></asp:RequiredFieldValidator>
                        </div>
                        <div class="form-group">
                            <label for="review">Confirm Password</label>
                            <asp:TextBox ID="txtConfirmPassword" runat="server" TextMode="Password" ValidationGroup="password" CssClass="form-control" placeholder="Confirm Password"></asp:TextBox>
                            <asp:CompareValidator ID="valConfirmPassword" ControlToCompare="txtPassword" ControlToValidate="txtConfirmPassword" runat="server" ErrorMessage="Password does not match" ForeColor="Red" ValidationGroup="password"></asp:CompareValidator>
                        </div>
                        <asp:Button ID="btnResetPassword" runat="server" Text="Reset" OnClick="btnResetPassword_Click" CssClass="btn btn-solid" ValidationGroup="password"/>

                        <div class="form-group">
                            <asp:Label ID="lblMessage" runat="server"></asp:Label>
                        </div>
                    </form>
                </div>
            </div>
        </div>
    </section>
    <!--Section ends-->
</asp:Content>
