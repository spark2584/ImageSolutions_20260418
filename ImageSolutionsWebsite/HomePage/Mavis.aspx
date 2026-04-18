<%@ Page Language="C#" MasterPageFile="~/MasterPage.Master" AutoEventWireup="true" CodeBehind="Mavis.aspx.cs" Inherits="ImageSolutionsWebsite.HomePage.Mavis" %>


<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <style>
        div#slides div{
           background-size: 100% !important;
        }
        div#slides2 div{
           background-size: 100% !important;
        }


        .item-desktop {
		    display: block;
	    }

        .item-mobile {
	        display: none;
		}
	
	    @media all and (max-width: 768px) {
		    .item-desktop {
			    display: none;
		    }

		    .item-mobile {
			    display: block;
		    }
	    }
    </style>
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="cphHeader" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="cphBody" runat="server">

    <div class="breadcrumb-section">
        <div class="container">
            <div class="row">
                <div class="col-sm-12" style="text-align:center;">
                    <h3 style="text-transform: unset;"><b><asp:Literal ID="litHeader" runat="server"></asp:Literal></b></h3>
                    <h4 style="text-transform: unset;"><b><asp:Literal ID="litHeader2" runat="server"></asp:Literal></b></h4>
                </div>
            </div>
        </div>
    </div>

    <!-- Home slider -->

    <section class="p-0">
        <div class="item-desktop">
            <div class="slide-1 home-slider">
                <div>
                    <div class="home text-center">
                        <asp:Image ID="imgHomePage" runat="server" ImageUrl="../assets/company/Mavis/Carousel/20250504/MavisBanner-LG-02-Mavis.png" AlternateText="" Width="100%" />
                        <%--<img src="../assets/company/DiscountTire/Carousel/Store_Header_DT.png" alt="" class="bg-img blur-up lazyload">--%>
                        <div class="container">
                            <div class="row">
                                <div class="col">
                                    <div class="slider-contain">
                                        <div>
                                            <%--<h4>Upgrade Your Outerwear</h4>--%>
                                            <%--<h1 style="color:white;">Upgrade Your Outerwear</h1>
                                            <a href="/items.aspx?WebsiteTabID=25" class="btn btn-solid">Shop Zip Up Vests</a>--%>
                                        </div>
                                    </div>
                                </div>
                            </div>
                        </div>
                    </div>
                </div>               
            </div>
        </div>

        <div class="item-mobile">
            <div class="slide-1 home-slider">
                <div>
                    <div class="home text-center">
                        <asp:Image ID="imgHomePageMobile" runat="server" ImageUrl="../assets/company/Mavis/Carousel/20250504/MavisBanner-SM-08-Mavis.png" AlternateText="" Width="100%" />
                        <div class="container">
                            <div class="row">
                                <div class="col">
                                    <div class="slider-contain">
                                        <div>
                                        </div>
                                    </div>
                                </div>
                            </div>
                        </div>
                    </div>
                </div>                
            </div>
        </div>
    </section>
    <!-- Home slider end -->

    
    <!-- Paragraph-->
<%--    <div class="title1 section-t-space">
        <h2 class="title-inner1">Shop The Latest Trends</h2>
    </div>--%>
    <!-- Paragraph end -->


    <!-- Product slider -->
    <%--<section class="section-b-space pt-0 ratio_asos">
        <div class="container">
            <div class="row">
                <div class="col">
                    <div class="product-4 product-m no-arrow">
                        <div class="product-box">
                            <div class="img-wrapper">
                                <div class="front">
                                    <a href="/ProductDetail.aspx?id=714"><img src="https://acct88641.app.netsuite.com/core/media/media.nl?id=11667079&c=ACCT88641&h=SL_FVQXYR_VsqGFurR4VmqZ0iBOZgP5zP5yXFX_z67N7tK29"
                                            class="img-fluid blur-up lazyload bg-img" alt=""></a>
                                </div>
                            </div>
                            <div class="product-detail">
                                <a href="/ProductDetail.aspx?id=714">
                                    <h5 style="word-wrap: break-word;">NEW ERA® HERITAGE BLEND 3/4-SLEEVE BASEBALL RAGLAN TEE</h5>
                                </a>
                            </div>
                        </div>
                        <div class="product-box">
                            <div class="img-wrapper">
                                <div class="front">
                                    <a href="/ProductDetail.aspx?id=335"><img src="https://acct88641.app.netsuite.com/core/media/media.nl?id=7447582&c=ACCT88641&h=CJce9TRRSUntLsj9cQ-z0rx_q985OdS5OnXEwq16q7gRu1S9"
                                            class="img-fluid blur-up lazyload bg-img" alt=""></a>
                                </div>
                            </div>
                            <div class="product-detail">
                                <a href="/ProductDetail.aspx?id=335">
                                    <h5 style="word-wrap: break-word;">Port Authority ® City Stretch Shirt</h5>
                                </a>
                            </div>
                        </div>
                        <div class="product-box">
                            <div class="img-wrapper">
                                <div class="front">
                                    <a href="/ProductDetail.aspx?id=773"><img src="https://acct88641.app.netsuite.com/core/media/media.nl?id=7447750&c=ACCT88641&h=d3iIHnDFTkKqnOyVmMscp3yEtM8rHz7ljFyg62JBf9MPl2Jl"
                                            class="img-fluid blur-up lazyload bg-img" alt=""></a>
                                </div>
                            </div>
                            <div class="product-detail">
                                <a href="/ProductDetail.aspx?id=773">
                                    <h5 style="word-wrap: break-word;">Port Authority® Ladies Concept Stretch V-Neck Tee</h5>
                                </a>
                            </div>
                        </div>
                        <div class="product-box">
                            <div class="img-wrapper">
                                <div class="front">
                                    <a href="/ProductDetail.aspx?id=19"><img src="https://acct88641.app.netsuite.com/core/media/media.nl?id=11667055&c=ACCT88641&h=RrM3TueC01J4c5XlJRCPRIJ8nkJpyRoUuD3lYNcblebvtUl7"
                                            class="img-fluid blur-up lazyload bg-img" alt=""></a>
                                </div>
                            </div>
                            <div class="product-detail">
                                <a href="/ProductDetail.aspx?id=19">
                                    <h5 style="word-wrap: break-word;">District® Women’s Perfect Tri® Fleece V-Neck Sweatshirt</h5>
                                </a>
                            </div>
                        </div>
                    </div>
                </div>
            </div>
        </div>
    </section>--%>
    <!-- Product slider end -->
    
</asp:Content>
