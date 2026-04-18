<%@ Page Title="" Language="C#" MasterPageFile="~/MasterPage.Master" AutoEventWireup="true" CodeBehind="ForgetPassword.aspx.cs" Inherits="ImageSolutionsWebsite.ForgetPassword" %>
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
                    <h2>Forget Your Password</h2>
                    <form class="theme-form">
                        <div class="form-row row">
<%--                            <div class="col-md-12">
                                <input type="text" class="form-control" id="email" placeholder="Enter Your Email"
                                    required="">
                            </div>
                            <a href="#" class="btn btn-solid w-auto">Submit</a>
                            --%>
                            <asp:Panel ID="pnlSendEmail" runat="server">
                                <div class="col-md-12">
                                    <asp:TextBox ID="txtEmail" runat="server" placeholder="Enter Your Email" CssClass="form-control" OnTextChanged="txtEmail_TextChanged"></asp:TextBox>
                                    <asp:RequiredFieldValidator ID="valEmail" runat="server" ControlToValidate="txtEmail" ErrorMessage="Email Required" ForeColor="Red"></asp:RequiredFieldValidator>
                                    <br />
                                </div>
                                <div class="col-md-12">
                                    <asp:Button ID="btnSubmit" runat="server" CssClass="btn btn-solid w-auto" Text="Submit" OnClick="btnSubmit_Click" />
                                </div>
                            </asp:Panel>
                            <asp:Label ID="lblMessage" runat="server"></asp:Label>                           
                        </div>
                        
                        <br />
                        <br />

                        <div class="form-row row">
                            <div class="col-md-12">
                               <b>If you do not receive your email, please check the spam folder</b>
                            </div>
                        </div>
                    </form>
                </div>
            </div>
        </div>
    </section>
    <!--Section ends-->
</asp:Content>
