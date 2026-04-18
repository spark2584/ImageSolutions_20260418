<%@ Page Language="C#" MasterPageFile="~/MasterPage.Master" AutoEventWireup="true" CodeBehind="Passcode.aspx.cs" Inherits="ImageSolutionsWebsite.Passcode" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <style type="text/css">
    .auto-style2 {
        height: 27px;
    }
</style>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="cphHeader" runat="server">
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="cphBody" runat="server">
    <section class="login-page section-b-space">
        <div class="container">
            <div class="row">
                <div class="col-lg-12">
                    <h3>Login</h3>
                    <div class="theme-card">
                        <div class="theme-form">
                            <div class="form-group">
                                <asp:Label ID="lblMessage" runat="server" ForeColor="Red" EnableViewState="false"></asp:Label>
                            </div>
                            <div class="form-group">
                                <label for="email">Email</label>
                                <asp:TextBox ID="txtEmail" runat="server" ValidationGroup="login" CssClass="form-control" placeholder="Email" required="" OnTextChanged="txtEmail_TextChanged"></asp:TextBox>
                                <asp:CustomValidator ID="custValidLogin" runat="server" ErrorMessage="" ValidationGroup="login" onservervalidate="custValidLogin_ServerValidate"></asp:CustomValidator>
                                <asp:LinkButton ID="btnSendPasscode" runat="server" Text="Send Passcode" OnClick="btnSendPasscode_Click" CssClass="btn btn-solid" />                         
                            </div>
                            <div class="form-group">
                                <label for="review">Passcode</label>
                                <asp:TextBox ID="txtPasscode" runat="server" TextMode="Password" ValidationGroup="login" CssClass="form-control" placeholder="Enter passcode" required=""></asp:TextBox>
                                <asp:Button ID="btnLogin" runat="server" Text="Login" OnClick="btnLogin_Click" ValidationGroup="login" CssClass="btn btn-solid"/>
                            </div>
                        </div>
                    </div>
                </div>
            </div>
        </div>
    </section>
</asp:Content> 