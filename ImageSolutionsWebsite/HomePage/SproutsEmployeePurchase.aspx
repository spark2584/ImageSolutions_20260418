<%@ Page Language="C#" MasterPageFile="~/HomePageMasterPage.Master" AutoEventWireup="true" CodeBehind="SproutsEmployeePurchase.aspx.cs" Inherits="ImageSolutionsWebsite.HomePage.SproutsEmployeePurchase" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
	<style type="text/css">
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
	<section class="p-0">           
        <div class="item-desktop">
            <div class="slide-1 home-slider" id="slides">
                <div>
                    <div class="home text-center">
                        <asp:Image ID="imgAMPM1" runat="server" ImageUrl="../assets/company/Sprouts/Carousel/Sprouts_1.png" class="bg-img blur-up lazyload" Width="100%" />
                        <div class="container">
                            <div class="row">
                                <div class="col">
                                    <div class="slider-contain">
                                        <div style="margin-left:10%">
                       
                                        </div>
                                    </div>
                                </div>
                            </div>
                        </div>
                    </div>
                </div>
                <div>
                    <div class="home text-center">
                        <asp:Image ID="Image1" runat="server" ImageUrl="../assets/company/Sprouts/Carousel/Sprouts_2.png" class="bg-img blur-up lazyload" Width="100%" />
                        <div class="container">
                            <div class="row">
                                <div class="col">
                                    <div class="slider-contain">
                                        <div style="margin-left:10%">
                       
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
            <div class="slide-1 home-slider" id="slides2">
                <div>
                    <div class="home text-center">
                        <asp:Image ID="Image7" runat="server" ImageUrl="../assets/company/Sprouts/Carousel/Sprouts_1.png" class="bg-img blur-up lazyload" Width="100%" />
                        <div class="container">
                            <div class="row">
                                <div class="col">
                                    <div class="slider-contain">
                                        <div style="margin-left:10%">
                       
                                        </div>
                                    </div>
                                </div>
                            </div>
                        </div>
                    </div>
                </div>
                <div>
                    <div class="home text-center">
                        <asp:Image ID="Image2" runat="server" ImageUrl="../assets/company/Sprouts/Carousel/Sprouts_2.png" class="bg-img blur-up lazyload" Width="100%" />
                        <div class="container">
                            <div class="row">
                                <div class="col">
                                    <div class="slider-contain">
                                        <div style="margin-left:10%">
                       
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

    <!-- Paragraph-->
    <div class="title1 section-t-space">
        <h2 class="title-inner1">Shop The Latest Trends</h2>
    </div>
    <!-- Paragraph end -->

    <!-- Product slider -->
    <section class="section-b-space pt-0 ratio_asos">
        <div class="container">
            <div class="row">
                <div class="col">
                    <div class="product-4 product-m no-arrow">
                        <div class="product-box">
                            <div class="img-wrapper">
                                <div class="front">
                                    <a href="/ProductDetail.aspx?id=1978760"><img src="https://acct88641.app.netsuite.com/core/media/media.nl?id=37453131&c=ACCT88641&h=CybkdTkjKwz7JHILsRAIfyhkIPgHDHG8S-iN7ptG0-nC2VUJ"
                                            class="img-fluid blur-up lazyload bg-img" alt=""></a>
                                </div>
                            </div>
                            <div class="product-detail">
                                <a href="/ProductDetail.aspx?id=1978760">
                                    <h5 style="word-wrap: break-word;">Port Authority Fleece Jacket</h5>
                                </a>
                            </div>
                        </div>
                        <div class="product-box">
                            <div class="img-wrapper">
                                <div class="front">
                                    <a href="/ProductDetail.aspx?id=1978824"><img src="https://acct88641.app.netsuite.com/core/media/media.nl?id=37453135&c=ACCT88641&h=JQgqOUSF77LIrfFfaKgSCPe3mz1dd7CbmQXnhLipxx1_enbh"
                                            class="img-fluid blur-up lazyload bg-img" alt=""></a>
                                </div>
                            </div>
                            <div class="product-detail">
                                <a href="/ProductDetail.aspx?id=1978824">
                                    <h5 style="word-wrap: break-word;">Sport-Tek Full-Zip Wind Jacket</h5>
                                </a>
                            </div>
                        </div>
                        <div class="product-box">
                            <div class="img-wrapper">
                                <div class="front">
                                    <a href="/ProductDetail.aspx?id=1978868"><img src="https://acct88641.app.netsuite.com/core/media/media.nl?id=37613365&c=ACCT88641&h=tjOsv9xadgEG8lKmBE1PuM6C82-r72n5MkuauH0F82IZZHEE"
                                            class="img-fluid blur-up lazyload bg-img" alt=""></a>
                                </div>
                            </div>
                            <div class="product-detail">
                                <a href="/ProductDetail.aspx?id=1978868">
                                    <h5 style="word-wrap: break-word;">Port Authority Long Sleeve Carefree Poplin Shirt</h5>
                                </a>
                            </div>
                        </div>
                        <div class="product-box">
                            <div class="img-wrapper">
                                <div class="front">
                                    <a href="/ProductDetail.aspx?id=1978835"><img src="https://acct88641.app.netsuite.com/core/media/media.nl?id=37612849&c=ACCT88641&h=JV1R1DikxI7RMEwXozyFvYaneEDxcSQ05C5DgnZunj3lUR32"
                                            class="img-fluid blur-up lazyload bg-img" alt=""></a>
                                </div>
                            </div>
                            <div class="product-detail">
                                <a href="/ProductDetail.aspx?id=1978835">
                                    <h5 style="word-wrap: break-word;">Port Authority Core Soft Shell Jacket</h5>
                                </a>
                            </div>
                        </div>
                                                
                    </div>
                </div>
            </div>
        </div>
    </section>
    <!-- Product slider end -->

</asp:Content>
