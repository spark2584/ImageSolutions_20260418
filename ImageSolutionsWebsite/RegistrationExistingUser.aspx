<%@ Page Language="C#" MasterPageFile="~/MasterPage.Master" AutoEventWireup="true" CodeBehind="RegistrationExistingUser.aspx.cs" Inherits="ImageSolutionsWebsite.RegistrationExistingUser" %>
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
                        <h2>Registration Complete!</h2>
                        <h3>Website has been added to an existing email</h3>
                        <br /><br />
                        <a href="/login.aspx" class="btn btn-solid">login to your new account</a>
                    </div>
                </div>
            </div>
        </div>
    </section>
    <!-- Section ends -->
</asp:Content>
