<%@ Page Language="C#" MasterPageFile="~/HomePageMasterPage.Master" AutoEventWireup="true" CodeBehind="BrinkerGear.aspx.cs" Inherits="ImageSolutionsWebsite.HomePage.BrinkerGear" %>


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

<%--    <div class="breadcrumb-section">
        <div class="container">
            <div class="row">
                <div class="col-sm-12" style="text-align:center;">
                    <h4><b>WELCOME TO DISCOUNT TIRE UNIFORM STORE</b></h4>
                </div>
            </div>
        </div>
    </div>--%>

    <!-- Home slider -->

    <section class="p-0">
        <div class="item-desktop">
            <div class="slide-1 home-slider" id="slides">

               
                <%--<a href="/items.aspx?WebsiteTabID=1621">  --%>              
                    <div>
                        <div class="home text-center">
                            <asp:Image ID="imgHomePage" runat="server" ImageUrl="../assets/company/BrinkerGear/Carousel/20251207/Brinker_Landing_Page.jpg" AlternateText="" class="bg-img blur-up lazyload" Width="100%" />
                            <div class="container">
                                <div class="row">
                                    <div class="col">
                                        <div class="slider-contain">
                                            <div style="width:100%;">
                                                <%--<h4>Upgrade Your Outerwear</h4>--%>
                                                <%--<h1 style="color:black;">EFFORTLESS LAYER<br /> TO CONQUER THE DAY</h1>--%>
                                                <%--<a href="/items.aspx?WebsiteTabID=1476" class="btn btn-solid" style="color:white; --theme-color:black;">Shop Now</a>--%>
                                                <%--<a href="/items.aspx?WebsiteTabID=1483" class="btn btn-solid">Women's</a>--%>
                                            </div>
                                        </div>
                                    </div>
                                </div>
                            </div>
                        </div>
                    </div>
             <%--   </a>--%>
            
            </div>
        </div>

        <div class="item-mobile">
            <div class="slide-1 home-slider" id="slides2">

                <%--<a href="/items.aspx?WebsiteTabID=1904"> --%>               
                    <div>
                        <div class="home text-center">
                            <asp:Image ID="imgHomePageMobile" runat="server" ImageUrl="../assets/company/BrinkerGear/Carousel/20251207/Brinker_Landing_Page.jpg" AlternateText="" class="bg-img blur-up lazyload" />
                            <%--<img src="../assets/company/DiscountTire/Carousel/20251003/Mobile/New_Arrivals.jpg" alt="" class="bg-img blur-up lazyload">--%>
                            <div class="container">
                                <div class="row">
                                    <div class="col">
                                        <div class="slider-contain">
                                            <div style="width:100%;">
                                                <%--<h4>Upgrade Your Outerwear</h4>--%>
                                                <%--<h1 style="color:black;">EFFORTLESS LAYER<br /> TO CONQUER THE DAY</h1>--%>
                                                <%--<a href="/items.aspx?WebsiteTabID=1476" class="btn btn-solid" style="color:white; --theme-color:black;">Shop Now</a>--%>
                                                <%--<a href="/items.aspx?WebsiteTabID=1483" class="btn btn-solid">Women's</a>--%>
                                            </div>
                                        </div>
                                    </div>
                                </div>
                            </div>
                        </div>
                    </div>
                <%--</a>--%>
          
            </div>
        </div>


        
    </section>
    <!-- Home slider end -->

    
    <!-- Paragraph-->
<%--    <div class="title1 section-t-space">
        <h2 class="title-inner1">Gear Up For Golf Season</h2>
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
                                    <a href="/ProductDetail.aspx?id=1086985"><img src="https://acct88641.app.netsuite.com/core/media/media.nl?id=30947410&c=ACCT88641&h=JaRztLmMj-6dLxbj3w-5wYoUD9H0fYDopq19J6hauqbR9UMk"
                                            class="img-fluid blur-up lazyload bg-img" alt=""></a>
                                </div>
                            </div>
                            <div class="product-detail">
                                <a href="/ProductDetail.aspx?id=1086985">
                                    <h5 style="word-wrap: break-word;">Callaway SuperSoft Golf Balls</h5>
                                </a>
                            </div>
                        </div>
                        <div class="product-box">
                            <div class="img-wrapper">
                                <div class="front">
                                    <a href="/ProductDetail.aspx?id=1086995"><img src="https://acct88641.app.netsuite.com/core/media/media.nl?id=30939461&c=ACCT88641&h=qmZCPCQqTp6SCVVOjSRyAUCMDPF_ms86HWm5fC7YmDKo8V4V"
                                            class="img-fluid blur-up lazyload bg-img" alt=""></a>
                                </div>
                            </div>
                            <div class="product-detail">
                                <a href="/ProductDetail.aspx?id=1086995">
                                    <h5 style="word-wrap: break-word;">Prime Line Woodbury Chrome-Plated Divot Fixer</h5>
                                </a>
                            </div>
                        </div>
                        <div class="product-box">
                            <div class="img-wrapper">
                                <div class="front">
                                    <a href="/ProductDetail.aspx?id=1091008"><img src="https://acct88641.app.netsuite.com/core/media/media.nl?id=32532006&c=ACCT88641&h=avFhAZdZVQx3xxQYAEhNL6FUpQhrJFEIb2GEP6FtSrJGI7hK"
                                            class="img-fluid blur-up lazyload bg-img" alt=""></a>
                                </div>
                            </div>
                            <div class="product-detail">
                                <a href="/ProductDetail.aspx?id=1091008">
                                    <h5 style="word-wrap: break-word;">Pillow Tee Pack</h5>
                                </a>
                            </div>
                        </div>
                        <div class="product-box">
                            <div class="img-wrapper">
                                <div class="front">
                                    <a href="/ProductDetail.aspx?id=1117038"><img src="https://acct88641.app.netsuite.com/core/media/media.nl?id=34151930&c=ACCT88641&h=mdQ1z_65asCICEfDBvFL2sUnRx3VfYWTJh--TzpMwrkRCT5f"
                                            class="img-fluid blur-up lazyload bg-img" alt=""></a>
                                </div>
                            </div>
                            <div class="product-detail">
                                <a href="/ProductDetail.aspx?id=1117038">
                                    <h5 style="word-wrap: break-word;">TravisMathew Approach Storage Cube</h5>
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
