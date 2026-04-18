<%@ Page Title="" Language="C#" MasterPageFile="~/MasterPage.Master" AutoEventWireup="true" CodeBehind="Items.aspx.cs" Inherits="ImageSolutionsWebsite.Items" %>

<%@ Register Src="~/Control/Pager.ascx" TagPrefix="uc1" TagName="Pager" %>
<%@ Register Src="~/Control/LeftPanelNavigation.ascx" TagPrefix="uc2" TagName="LeftPanelNavigation"%>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <asp:Literal ID="litStyle" runat="server"></asp:Literal>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="cphHeader" runat="server">
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="cphBody" runat="server">
    <!-- section start -->
    <section class="section-b-space ratio_asos">
        <div class="collection-wrapper">
            <div class="container">
                <div class="row">
                    <!--Begin Left Panel-->
                        
                        
                        <div class="col-sm-3 collection-filter" id="divLeftPanel" runat="server">

                            <uc2:LeftPanelNavigation runat="server" ID="ucLeftPanelNavigation" />

                            <!-- side-bar colleps block stat -->
                            <asp:Panel ID="pnlAttributeFilter" runat="server">
                                <div class="collection-filter-block">
                                    <!-- brand filter start -->
                                    <div class="collection-mobile-back"><span class="filter-back"><i class="fa fa-angle-left" aria-hidden="true"></i> back</span></div>
                                    <%--<div class="collection-collapse-block open">
                                        <h3 class="collapse-block-title">brand</h3>
                                        <div class="collection-collapse-block-content">
                                            <div class="collection-brand-filter">
                                                <div class="form-check collection-filter-checkbox">
                                                    <input type="checkbox" class="form-check-input" id="zara">
                                                    <label class="form-check-label" for="zara">zara</label>
                                                </div>
                                                <div class="form-check collection-filter-checkbox">
                                                    <input type="checkbox" class="form-check-input" id="vera-moda">
                                                    <label class="form-check-label" for="vera-moda">vera-moda</label>
                                                </div>
                                                <div class="form-check collection-filter-checkbox">
                                                    <input type="checkbox" class="form-check-input" id="forever-21">
                                                    <label class="form-check-label" for="forever-21">forever-21</label>
                                                </div>
                                                <div class="form-check collection-filter-checkbox">
                                                    <input type="checkbox" class="form-check-input" id="roadster">
                                                    <label class="form-check-label" for="roadster">roadster</label>
                                                </div>
                                                <div class="form-check collection-filter-checkbox">
                                                    <input type="checkbox" class="form-check-input" id="only">
                                                    <label class="form-check-label" for="only">only</label>
                                                </div>
                                            </div>
                                        </div>
                                    </div>--%>
                                    <!-- color filter start here -->
                                    <%--<div class="collection-collapse-block open">
                                        <h3 class="collapse-block-title">colors</h3>
                                        <div class="collection-collapse-block-content">
                                            <div class="color-selector">
                                                <ul>
                                                    <li class="color-1 active"></li>
                                                    <li class="color-2"></li>
                                                    <li class="color-3"></li>
                                                    <li class="color-4"></li>
                                                    <li class="color-5"></li>
                                                    <li class="color-6"></li>
                                                    <li class="color-7"></li>
                                                </ul>
                                            </div>
                                        </div>
                                    </div>--%>
                                    <div class="collection-collapse-block border-0">
                                        <h3 class="collapse-block-title">color</h3>
                                        <div class="collection-collapse-block-content" style="display: none;">
                                            <div class="collection-brand-filter">
                                                <asp:ListView ID="lvColor" runat="server" AutoGenerateColumns="false" DataKeyNames="AttributeValue">
                                                    <ItemTemplate>
                                                        <div class="form-check collection-filter-checkbox">
                                                            <asp:CheckBox ID="chkColor" runat="server" class="form-check-input" AutoPostBack="true" OnCheckedChanged="chkColor_CheckedChanged" />&nbsp;<label style="padding-top:4px;"><%# Eval("AttributeValue") %></label>
                                                        </div>
                                                    </ItemTemplate>
                                                </asp:ListView>
                                            </div>
                                        </div>
                                    </div>
                                    <!-- size filter start here -->
                                    <div class="collection-collapse-block border-0">
                                        <h3 class="collapse-block-title">size</h3>
                                        <div class="collection-collapse-block-content" style="display: none;">
                                            <div class="collection-brand-filter">
                                                <asp:ListView ID="lvSize" runat="server" AutoGenerateColumns="false" DataKeyNames="AttributeValue">
                                                    <ItemTemplate>
                                                        <div class="form-check collection-filter-checkbox">
                                                            <asp:CheckBox ID="chkSize" runat="server" class="form-check-input" AutoPostBack="true" OnCheckedChanged="chkSize_CheckedChanged" />&nbsp;<label style="padding-top:4px;"><%# Eval("AttributeValue") %></label>
                                                        </div>
                                                    </ItemTemplate>
                                                </asp:ListView>
                                            </div>
                                           <%--<div class="collection-brand-filter">
                                                <div class="form-check collection-filter-checkbox">
                                                    <input type="checkbox" class="form-check-input" id="hundred">
                                                    <label class="form-check-label" for="hundred">s</label>
                                                </div>
                                                <div class="form-check collection-filter-checkbox">
                                                    <input type="checkbox" class="form-check-input" id="twohundred">
                                                    <label class="form-check-label" for="twohundred">m</label>
                                                </div>
                                                <div class="form-check collection-filter-checkbox">
                                                    <input type="checkbox" class="form-check-input" id="threehundred">
                                                    <label class="form-check-label" for="threehundred">l</label>
                                                </div>
                                                <div class="form-check collection-filter-checkbox">
                                                    <input type="checkbox" class="form-check-input" id="fourhundred">
                                                    <label class="form-check-label" for="fourhundred">xl</label>
                                                </div>
                                            </div>--%>
                                        </div>
                                    </div>
                                </div>
                            </asp:Panel>
                            
                            <!-- silde-bar colleps block end here -->
                        </div>
                    <!--End Left Panel-->

                    <div class="collection-content col">
                        <div class="page-main-content">
                            <%--<div class="row">
                                <div class="col-sm-12">
                                    <div class="top-banner-wrapper">
                                        <div class="top-banner-content small-section">
                                            <h4>APPAREL</h4>
                                            <p>The trick to choosing the best wear for yourself is to keep in mind your
                                                body type, individual style, occasion and also the time of day or
                                                weather.
                                                In addition to eye-catching products from top brands, we also offer an
                                                easy 30-day return and exchange policy, free and fast shipping across
                                                all pin codes, cash or card on delivery option, deals and discounts,
                                                among other perks. So, sign up now and shop for westarn wear to your
                                                heart’s content on Multikart. </p>
                                        </div>
                                    </div>
                                </div>
                            </div>--%>
                            <asp:Panel ID="pnlCategoryBreadCrumb" runat="server">
                                <div class="collection-product-wrapper" style="font-size:larger">
                                    <asp:Literal ID="litCategoryBreadCrumb" runat="server"></asp:Literal>
                                </div>
                            </asp:Panel>

                            <asp:Panel ID="pnlMessage" runat="server" Visible="false">
                                <div class="container" style="margin-top:10px;">
                                    <div class="row">
                                        <div class="col-12">
                                            <asp:Literal ID="litMessage" runat="server"></asp:Literal>
                                        </div>
                                    </div>
                                </div>
                            </asp:Panel>

                            <asp:Panel ID="pnlSubCategories" runat="server">
                                <div class="collection-product-wrapper">
                                    <div class="product-wrapper-grid">
                                        <div class="row margin-res">
                                            <asp:Repeater ID="rptCategory" runat="server" OnItemCommand="rptCategory_ItemCommand">
                                                <ItemTemplate>
                                                    <div class="col-xl-3 col-6 col-grid-box">
                                                        <div class="product-box">
                                                            <div class="img-wrapper" style="text-align:center; height:300px;">
                                                                <div class="front">
                                                                    <a href="/items.aspx?websitetabid=<%# Eval("WebsiteTabID")%>"><img src='<%# Eval("ImageURL")%>' class="img-fluid blur-up lazyload" alt="" ></a>
                                                                </div>
                                                            </div>
                                                            <div class="product-detail">
                                                                <div style="text-align:center;">
                                                                    <a href="/items.aspx?websitetabid=<%# Eval("WebsiteTabID")%>"><%# Eval("TabName")%></a>
                                                                </div>
                                                            </div>
                                                        </div>
                                                    </div>
                                                </ItemTemplate>
                                            </asp:Repeater>
                                        </div>
                                    </div>
                                </div>
                            </asp:Panel>
                            
                            <asp:Panel ID="pnlItems" runat="server">            
                                <div class="collection-product-wrapper">
                                    <uc1:Pager runat="server" ID="ucPagerTop" PagingMode="Redirect" PageSize="16" PagingRecordText="Items" />
                                    <div class="product-wrapper-grid">
                                        <div class="row margin-res">

                                            <asp:Repeater ID="rptItems" runat="server" OnItemCommand="rptItems_ItemCommand">
                                                <ItemTemplate>
                                                    <div class="col-xl-3 col-6 col-grid-box">
                                                        <div class="product-box">
                                                            <div class="img-wrapper" style="text-align:center; height:320px; border:1px solid #cccccc; box-shadow: 4px 4px #eeeeee;">
                                                                <div class="front">
                                                                    <a href="/ProductDetail.aspx?id=<%# Eval("Item.ItemID")%>&websitetabid=<%# mWebSiteTabID %>"><img src='<%# Eval("Item.DisplayImageURL")%>' class="img-fluid blur-up lazyload" alt="" style="max-height:240px;" ></a>
                                                                </div>
                                                                <a href="/ProductDetail.aspx?id=<%# Eval("Item.ItemID")%>"><h6 style="font-weight:bold;"><%# Eval("Item.StoreDisplayName")%></h6></a>

                                                                <h6><%# Eval("Item.PriceRange") %></h6>



                                                                <%--<div class="cart-info cart-wrap">
                                                                    <button data-bs-toggle="modal" data-bs-target="#addtocart"
                                                                        title="Add to cart"><i
                                                                            class="ti-shopping-cart"></i></button> <a
                                                                        href="javascript:void(0)" title="Add to Wishlist"><i
                                                                            class="ti-heart" aria-hidden="true"></i></a> <a href="#"
                                                                        data-bs-toggle="modal" data-bs-target="#quick-view"
                                                                        title="Quick View"><i class="ti-search"
                                                                            aria-hidden="true"></i></a> <a href="compare.html"
                                                                        title="Compare"><i class="ti-reload" aria-hidden="true"></i></a>
                                                                </div>--%>
                                                            </div>
                                                            <div class="product-detail">
                                                                <div style="text-align:center;">
                                                                    <%--<div class="rating"><i class="fa fa-star"></i> <i
                                                                            class="fa fa-star"></i> <i class="fa fa-star"></i> <i
                                                                            class="fa fa-star"></i> <i class="fa fa-star"></i>
                                                                    </div>--%>
                                                                    <asp:Button id="btnAddToCart" runat="server" Visible="false" Text="Add To Cart" CssClass="btn btn-solid" CommandName="AddItem" CommandArgument='<%# Eval("Item.ItemID") %>'/>
                                                                    <%--<ul class="color-variant">
                                                                        <li class="bg-light0"></li>
                                                                        <li class="bg-light1"></li>
                                                                        <li class="bg-light2"></li>
                                                                    </ul>--%>
                                                                </div>
                                                            </div>
                                                        </div>
                                                    </div>
                                                </ItemTemplate>
                                            </asp:Repeater>
                                        </div>
                                    </div>
                                    <uc1:Pager runat="server" ID="ucPager" PagingMode="Redirect" PageSize="16" PagingRecordText="Items" />
                                </div>
                            </asp:Panel>
                        </div>
                    </div>
                </div>    
            </div>
        </div>
    </section>
    <!-- section End -->


    <!--Team start-->
    <section id="team" class="team section-b-space slick-default-margin ratio_asos" style="display:none;">
        <div class="container">
            <div class="row">
                <div class="col-sm-12">
                    <h2>Customers Also Viewed</h2>
                    <div class="team-4">
                        <div>
                            <div>
                                <img src="https://acct88641.app.netsuite.com/core/media/media.nl?id=11160547&c=ACCT88641&h=S6lD4JiokABxFrTHgtTTqTCCOa5YgZItd3x-vRLxbkft8oE1" class="img-fluid blur-up lazyload" alt="">
                            </div>
                            <h4>Hileri Keol</h4>
                            <h6>CEo & Founder At Company</h6>
                        </div>
                        <div>
                            <div>
                                <img src="https://acct88641.app.netsuite.com/core/media/media.nl?id=11160547&c=ACCT88641&h=S6lD4JiokABxFrTHgtTTqTCCOa5YgZItd3x-vRLxbkft8oE1" class="img-fluid blur-up lazyload" alt="">
                            </div>
                            <h4>Hileri Keol</h4>
                            <h6>CEo & Founder At Company</h6>
                        </div>
                        <div>
                            <div>
                                <img src="https://acct88641.app.netsuite.com/core/media/media.nl?id=11160547&c=ACCT88641&h=S6lD4JiokABxFrTHgtTTqTCCOa5YgZItd3x-vRLxbkft8oE1" class="img-fluid blur-up lazyload" alt="">
                            </div>
                            <h4>Hileri Keol</h4>
                            <h6>CEo & Founder At Company</h6>
                        </div>
                        <div>
                            <div>
                                <img src="https://acct88641.app.netsuite.com/core/media/media.nl?id=11160547&c=ACCT88641&h=S6lD4JiokABxFrTHgtTTqTCCOa5YgZItd3x-vRLxbkft8oE1" class="img-fluid blur-up lazyload" alt="">
                            </div>
                            <h4>Hileri Keol</h4>
                            <h6>CEo & Founder At Company</h6>
                        </div>
                        <div>
                            <div>
                                <img src="https://acct88641.app.netsuite.com/core/media/media.nl?id=11160547&c=ACCT88641&h=S6lD4JiokABxFrTHgtTTqTCCOa5YgZItd3x-vRLxbkft8oE1" class="img-fluid blur-up lazyload" alt="">
                            </div>
                            <h4>Hileri Keol</h4>
                            <h6>CEo & Founder At Company</h6>
                        </div>
                    </div>
                </div>
            </div>
        </div>
    </section>
    <!--Team ends-->
</asp:Content>
