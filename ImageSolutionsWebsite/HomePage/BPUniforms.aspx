<%@ Page Language="C#" MasterPageFile="~/HomePageMasterPage.Master" AutoEventWireup="true" CodeBehind="BPUniforms.aspx.cs" Inherits="ImageSolutionsWebsite.HomePage.BPUniforms" %>

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
        <asp:Panel ID="pnlBP" runat="server">
	        <div class="item-desktop">
                <div class="slide-1 home-slider" id="slides">
                    <div>
                        <div class="home text-center">
                            <asp:Image ID="imgBP1" runat="server" ImageUrl="../assets/company/BPUniforms/Carousel/20250821/Banner_1.jpg" class="bg-img blur-up lazyload" Width="100%" />
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
                            <asp:Image ID="imgBP2" runat="server" ImageUrl="../assets/company/BPUniforms/Carousel/20250821/Banner_2.jpg" class="bg-img blur-up lazyload" Width="100%" />
                            <div class="container">
                                <div class="row">
                                    <div class="col">
                                        <div class="slider-contain">
                                            <div style="width:100%;">

                                            </div>
                                        </div>
                                    </div>
                                </div>
                            </div>
                        </div>
                    </div>

                    <div>
                        <div class="home text-center">
                            <asp:Image ID="Image3" runat="server" ImageUrl="../assets/company/BPUniforms/Carousel/20250821/Banner_3.jpg" class="bg-img blur-up lazyload" Width="100%" />
                            <div class="container">
                                <div class="row">
                                    <div class="col">
                                        <div class="slider-contain">
                                            <div style="width:100%;">

                                            </div>
                                        </div>
                                    </div>
                                </div>
                            </div>
                        </div>
                    </div>

                    <div>
                        <div class="home text-center">
                            <asp:Image ID="Image4" runat="server" ImageUrl="../assets/company/BPUniforms/Carousel/20250821/Banner_4.jpg" class="bg-img blur-up lazyload" Width="100%" />
                            <div class="container">
                                <div class="row">
                                    <div class="col">
                                        <div class="slider-contain">
                                            <div style="width:100%;">

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
                            <asp:Image ID="Image1" runat="server" ImageUrl="../assets/company/BPUniforms/Carousel/20250821/Banner_Mobile_5.jpg" class="bg-img blur-up lazyload" Width="100%" />
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
                            <asp:Image ID="Image2" runat="server" ImageUrl="../assets/company/BPUniforms/Carousel/20250821/Banner_Mobile_6.jpg" class="bg-img blur-up lazyload" Width="100%" />
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
                            <asp:Image ID="Image5" runat="server" ImageUrl="../assets/company/BPUniforms/Carousel/20250821/Banner_Mobile_7.jpg" class="bg-img blur-up lazyload" Width="100%" />
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
                            <asp:Image ID="Image6" runat="server" ImageUrl="../assets/company/BPUniforms/Carousel/20250821/Banner_Mobile_8.jpg" class="bg-img blur-up lazyload" Width="100%" />
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
        </asp:Panel>
        <asp:Panel ID="pnlAMPM" runat="server" Visible="false">
            <div class="item-desktop">
                <div class="slide-1 home-slider" id="slides">

		    <a href="/items.aspx?WebsiteTabID=1669"> 
                        <div>
                            <div class="home text-center">
                                <asp:Image ID="imgAMPM3" runat="server" ImageUrl="../assets/company/BPUniforms/Carousel/20260206/AMPM_Banner.jpg" class="bg-img blur-up lazyload" Width="100%" />
                                <div class="container">
                                    <div class="row">
                                        <div class="col">
                                            <div class="slider-contain">
                                                <div style="width:100%;">

                                                </div>
                                            </div>
                                        </div>
                                    </div>
                                </div>
                            </div>
                        </div>
                    </a>


                    <div>
                        <div class="home text-center">
                            <asp:Image ID="imgAMPM2" runat="server" ImageUrl="../assets/company/BPUniforms/Carousel/20250325/ampm_desktop_2.png" class="bg-img blur-up lazyload" Width="100%" />
                            <div class="container">
                                <div class="row">
                                    <div class="col">
                                        <div class="slider-contain">
                                            <div style="width:100%;">

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

		    <a href="/items.aspx?WebsiteTabID=1669"> 
                        <div>
                            <div class="home text-center">
                                <asp:Image ID="Image9" runat="server" ImageUrl="../assets/company/BPUniforms/Carousel/20260206/AMPM_Mobile.jpg" class="bg-img blur-up lazyload" Width="100%" />
                                <div class="container">
                                    <div class="row">
                                        <div class="col">
                                            <div class="slider-contain">
                                                <div style="width:100%;">

                                                </div>
                                            </div>
                                        </div>
                                    </div>
                                </div>
                            </div>
                        </div>
                    </a>


                    <div>
                        <div class="home text-center">
                            <asp:Image ID="Image8" runat="server" ImageUrl="../assets/company/BPUniforms/Carousel/20250325/ampm_mobile_2.png" class="bg-img blur-up lazyload" Width="100%" />
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
        </asp:Panel>
    </section>
</asp:Content>