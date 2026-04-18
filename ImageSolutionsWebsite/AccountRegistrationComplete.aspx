<%@ Page Title="" Language="C#" MasterPageFile="~/MasterPage.Master" AutoEventWireup="true" CodeBehind="AccountRegistrationComplete.aspx.cs" Inherits="ImageSolutionsWebsite.AccountRegistrationComplete" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="cphHeader" runat="server">
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="cphBody" runat="server">
    <!-- section start -->
    <section class="p-0">
        <div class="container">
            <div class="row">
                <div class="col-sm-12">
                    <div class="error-section">
                        <h2><asp:Label ID="lblHeader" runat="server" Text="Registration Complete!"></asp:Label></h2>
                        <asp:PlaceHolder ID="phRegistrationApproval" runat="server">
                            <h3"><asp:Label ID="lblMessage" runat="server" Text="You will receive an email notification once the account is reviewed"></asp:Label></h3>
                        </asp:PlaceHolder>
                        <br /><br />
                        <a href="/login.aspx" id="btnLogin" runat="server" class="btn btn-solid">login to your new account</a>
                    </div>
                </div>
            </div>
        </div>
    </section>
    <!-- Section ends -->
</asp:Content>
