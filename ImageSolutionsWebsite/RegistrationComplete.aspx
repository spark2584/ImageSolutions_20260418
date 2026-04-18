<%@ Page Title="" Language="C#" MasterPageFile="~/MasterPage.Master" AutoEventWireup="true" CodeBehind="RegistrationComplete.aspx.cs" Inherits="ImageSolutionsWebsite.RegistrationComplete" %>
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
                        <asp:PlaceHolder ID="phRegistrationApproval" runat="server">
                            <h3">You will receive an email notification once the account is reviewed</h3>
                        </asp:PlaceHolder>
                        <br /><br />
                        <a href="/login.aspx" class="btn btn-solid">login to your new account</a>
                    </div>
                </div>
            </div>
        </div>
    </section>
    <!-- Section ends -->
</asp:Content>
