<%@ Page Title="" Language="C#" MasterPageFile="~/MasterPage.Master" AutoEventWireup="true" CodeBehind="Login.aspx.cs" Inherits="ImageSolutionsWebsite.Login" %>
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
<%--            <div class="row">
                <div class="col-lg-12">
                    <p style="background-color:red; color:white; text-align:center; font-size:large; padding: 20px 0px 20px 0px;">
                        Dear Valued Customer,
                        <br>We will be closed Monday September 4th in observance of Labor Day. &nbsp;During this time, all departments, including Customer Service will be closed. &nbsp;To avoid delays due to the extended holiday weekend, please submit your orders before 11am EST Friday September 1st. &nbsp;We will resume normal business hours Tuesday September 5th.
                        <br>Thank you for partnering with Image Solutions!
                    </p>
                </div>
            </div>--%>
            <div class="row">
                <asp:Panel ID="pnlWebsiteMessage" runat="server" CssClass="col-lg-12" Visible="false" >
                    <div style="text-align:center; font-size:x-large"><asp:Literal ID="litWebsiteMessage" runat="server"></asp:Literal></div>
                    <br />
                    <br />
                </asp:Panel>
                <div class="col-lg-6">
                    <asp:Panel ID="pnlLogin" runat="server" DefaultButton="btnLogin">
                        <h3><asp:Label ID="lblLoginHeader" runat="server" Text="Returning Customer Login"></asp:Label></h3>
                        <div class="theme-card">
                            <div class="theme-form">
                                <div class="form-group">
                                    <asp:Label ID="lblMessage" runat="server" ForeColor="Red" EnableViewState="false"></asp:Label>
                                    <label for="email">Username / Email Address</label> <asp:RequiredFieldValidator ID="reqValidUsername" runat="server" ControlToValidate="txtEmail" ValidationGroup="login" ErrorMessage="*" ForeColor="Red"></asp:RequiredFieldValidator>
                                    <asp:TextBox ID="txtEmail" runat="server" ValidationGroup="login" CssClass="form-control" placeholder="Email"></asp:TextBox>
                                    <asp:CustomValidator ID="custValidLogin" runat="server" ErrorMessage="" ValidationGroup="login" onservervalidate="custValidLogin_ServerValidate"></asp:CustomValidator>
                                </div>
                                <div class="form-group">
                                    <label for="review">Password</label> &emsp;&emsp; <asp:RequiredFieldValidator ID="reqValidPassword" runat="server" ControlToValidate="txtPassword" ValidationGroup="login" ErrorMessage="*" ForeColor="Red"></asp:RequiredFieldValidator>
                                    <asp:Panel ID="pnlForgotPassword" runat="server"> Forget Password? <a href="/forgetpassword.aspx">Reset here</a> <asp:LinkButton ID="btnLoginPasscode" runat="server" Text=" | Email one-time passcode" OnClick="btnLoginPasscode_Click" ValidationGroup="passcode" CausesValidation="false"/> </asp:Panel> 
                                    <asp:TextBox ID="txtPassword" runat="server" TextMode="Password" ValidationGroup="login" CssClass="form-control" placeholder="Enter your password"></asp:TextBox>
                                    
                                </div>

                                <asp:Button ID="btnLogin" runat="server" Text="Login" OnClick="btnLogin_Click" ValidationGroup="login" CssClass="btn btn-solid"/>
                                <asp:Button ID="btnTestPayment" runat="server" Text="Test" OnClick="btnTestPayment_Click" CssClass="btn btn-solid" Visible="false"/>

                            <!-- 新增的 SAML SSO 登录按钮 -->
                                <asp:Button ID="btnSamlLogin" runat="server" Text="Login with SAML SSO" OnClick="btnSamlLogin_Click" CssClass="btn btn-solid" Visible="false" CausesValidation="false"/>            

                                <asp:Panel ID="pnlPasswordHint" runat="server" class="form-group" Visible="false">
                                    <div><asp:Literal ID="litPasswordHint" runat="server"></asp:Literal></div>
                                </asp:Panel>
                            </div>
                        </div>
                    </asp:Panel>
                </div>
                <div class="col-lg-6 right-login" id="divRegistartion" runat="server">
                    <h3>Registration</h3>
                    <div class="theme-card authentication-right">
                        <asp:Panel runat="server" ID="pnlUserRegistrationEnabled" Visible="false">
                            <h6 class="title-font"><asp:Label ID="lblCreateUser" runat="server" Text="Create A User"></asp:Label></h6>
                            <%--<p>Sign up for a free user account. Registration is quick and easy. It allows you to be
                                able to order from our shop. To start shopping click register./p>--%>
                            <a href="/UserRegistration.aspx" id="btnUserRegister" runat="server" class="btn btn-solid" ><asp:Label ID="lblBtnCreateUser" runat="server" Text="Create A User"></asp:Label></a>
                            <br /><br /><br />
                        </asp:Panel>
                        <asp:Panel runat="server" ID="pnlAccountRegistrationEnabled" Visible="false">
                            <h6 class="title-font"><asp:Label ID="lblCreateAccount" runat="server" Text="Create An Account"></asp:Label></h6>
                            <%--<p>Sign up for a free sub-account.</p>--%>
                            <a href="/AccountRegistration.aspx" id="btnAccountRegister" runat="server"
                                class="btn btn-solid" ><asp:Label ID="lblBtnCreateAccount" runat="server" Text="Create An Account"></asp:Label></a>
                        </asp:Panel>
                        <asp:Panel runat="server" ID="pnlRegistrationFormEnabled" Visible="false">
                            <h6 class="title-font"><asp:Label ID="lblRegistrationForm" runat="server" Text="Registration Form"></asp:Label></h6>
                            <a id="aRegistrationForm" runat="server" class="btn btn-solid" target="_blank"><asp:Label ID="lblBtunRegistrationForm" runat="server" Text="New Account Setup Form"></asp:Label></a>
                        </asp:Panel>                        
                    </div>
                </div>
                
                <div class="col-lg-6 right-login" id="divAlternativeLogin" runat="server" visible="false">
                    <h3><asp:Label ID="lblAlternativeSSOLoginHeader" runat="server" Text="SSO Login"></asp:Label></h3>
                    <div class="theme-card authentication-right">
                        <asp:Button ID="btnAlternativeSSOButton" runat="server" Text="Login with SAML SSO" OnClick="btnSamlLogin_Click" CssClass="btn btn-solid" CausesValidation="false"/>    
                    </div>
                </div>

            </div>
        </div>
    </section>
</asp:Content>
