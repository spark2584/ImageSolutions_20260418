<%@ Page Title="" Language="C#" MasterPageFile="~/MasterPage.Master" AutoEventWireup="true" CodeBehind="ImageSolutions.aspx.cs" Inherits="ImageSolutionsWebsite.HomePage.ImageSolutions" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="cphHeader" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="cphBody" runat="server">
    <div class="breadcrumb-section">
        <div class="container">
            <div class="row">
                <div class="col-sm-12" style="text-align:center;">
                    <h4><b>WELCOME TO IMAGE SOLUTIONS BRAND SHOP</b></h4>
                </div>
            </div>
        </div>
    </div>

    <%--<!-- about section start -->
    <section class="about-page section-b-space">
        <div class="container">
            <div class="row">
                <div class="col-lg-12">
                    <div class="banner-section"><img src="../assets/images/about/about-us.jpg"
                            class="img-fluid blur-up lazyload" alt=""></div>
                </div>
            </div>
        </div>
    </section>
    <!-- about section end -->--%>

    <asp:Literal ID="litBannerHTML" runat="server"></asp:Literal>

    <asp:Literal ID="litFeaturedProduct" runat="server"></asp:Literal>
    
</asp:Content>
