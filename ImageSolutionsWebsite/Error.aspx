<%@ Page Language="C#" MasterPageFile="~/MasterPage.Master"  AutoEventWireup="true" CodeBehind="Error.aspx.cs" Inherits="ImageSolutionsWebsite.Error" %>


<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="cphHeader" runat="server">
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="cphBody" runat="server">
    <section class="contact-page section-b-space">
        <div class="container">
            <section class="p-0">
                <div class="container">
                    <div class="row">
                        <div class="col-sm-12">
                            <div class="error-section">
                                <%--<h1>500</h1>--%>
                                <asp:Image ID="imgError" runat="server" ImageUrl="~/assets/images/minus-black.png" />
                                <h2>Oops, something went wrong</h2>
                            </div>
                        </div>
                    </div>
                </div>
            </section>
        </div> 
    </section>
</asp:Content>
