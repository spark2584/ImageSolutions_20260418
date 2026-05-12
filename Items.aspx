<%@ Page Title="" Language="C#" MasterPageFile="~/MasterPage.Master" AutoEventWireup="true" CodeBehind="Items.aspx.cs" Inherits="ImageSolutionsWebsite.Items" %>

<%@ Register Src="~/Control/Pager.ascx" TagPrefix="uc1" TagName="Pager" %>
<%@ Register Src="~/Control/LeftPanelNavigation.ascx" TagPrefix="uc2" TagName="LeftPanelNavigation"%>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <asp:Literal ID="litStyle" runat="server"></asp:Literal>
    <style>
        /* ── Sidebar ── */
        .sidebar-panel {
            background: #f5f5f5;
            padding: 4px 16px 24px;
            min-height: 200px;
        }

        .sidebar-section { border-bottom: 1px solid #e4e4e4; }
        .sidebar-section:last-child { border-bottom: none; }

        .sidebar-section-header {
            display: flex;
            justify-content: space-between;
            align-items: center;
            padding: 14px 0;
            cursor: pointer;
            user-select: none;
        }
        .sidebar-section-title {
            font-size: 15px;
            font-weight: 700;
            color: #1a1a1a;
            letter-spacing: 0.01em;
        }
        .sidebar-chevron { font-size: 11px; color: #555; transition: transform .2s; }
        .sidebar-section.collapsed .sidebar-chevron { transform: rotate(-90deg); }
        .sidebar-section.collapsed .sidebar-section-content { display: none; }

        .sidebar-section-content { padding-bottom: 14px; }

        /* Filter list */
        .filter-list { list-style: none; padding: 0; margin: 0; }
        .filter-item { display: block; margin-bottom: 3px; }
        .filter-list .form-check { display: flex; align-items: center; gap: 8px; padding: 0; margin-top: 0; min-height: 20px; }
        .filter-list .form-check > span { display: flex; align-items: center; flex-shrink: 0; line-height: 0; height: 16px; position: relative; top: -2px; }

        /* Unchecked: plain white square, no border */
        .filter-list .form-check-input,
        .filter-list input[type="checkbox"] {
            -webkit-appearance: none;
            appearance: none;
            margin: 0;
            flex-shrink: 0;
            display: block;
            width: 16px;
            height: 16px;
            background-color: #fff;
            border: none;
            outline: none;
            box-shadow: none;
            border-radius: 2px;
            cursor: pointer;
            position: relative;
            vertical-align: middle;
        }
        /* Checked: dark fill + white checkmark */
        .filter-list .form-check-input:checked,
        .filter-list input[type="checkbox"]:checked {
            background-color: #222;
            background-image: url("data:image/svg+xml,%3csvg xmlns='http://www.w3.org/2000/svg' viewBox='0 0 20 20'%3e%3cpath fill='none' stroke='%23fff' stroke-linecap='round' stroke-linejoin='round' stroke-width='3' d='M6 10l3 3l6-6'/%3e%3c/svg%3e");
            background-repeat: no-repeat;
            background-size: 80%;
            background-position: center;
        }
        .filter-list .form-check-label { font-size: 13px; color: #333; cursor: pointer; line-height: 1; align-self: center; padding-top:3px; }
        .filter-list .form-check-label:hover { color: #000; }

        /* Scrollable color list */
        .sidebar-scroll { max-height: 190px; overflow-y: auto; padding-right: 4px; }
        .sidebar-scroll::-webkit-scrollbar { width: 4px; }
        .sidebar-scroll::-webkit-scrollbar-track { background: #f0f0f0; }
        .sidebar-scroll::-webkit-scrollbar-thumb { background: #bbb; border-radius: 2px; }

        /* Category nav links */
        .sidebar-cat-link {
            display: block; font-size: 13px; color: #333;
            padding: 3px 0; text-decoration: none;
        }
        .sidebar-cat-link:hover { color: #000; }

        /* ── Price Slider ── */
        .price-range-wrap {
            position: relative;
            height: 22px;
            margin: 10px 0 6px;
        }
        .price-track {
            position: absolute;
            top: 9px; left: 0; right: 0;
            height: 4px; background: #ddd; border-radius: 2px;
        }
        .price-fill {
            position: absolute;
            height: 4px; background: #1a1a1a;
            border-radius: 2px; left: 0; width: 100%;
        }
        .range-thumb {
            position: absolute;
            width: 100%; height: 4px; top: 9px;
            -webkit-appearance: none; appearance: none;
            background: transparent; pointer-events: none;
            outline: none; margin: 0; padding: 0;
        }
        .range-thumb::-webkit-slider-thumb {
            -webkit-appearance: none;
            width: 16px; height: 16px;
            background: #fff; border: 2px solid #1a1a1a;
            border-radius: 50%; cursor: pointer;
            pointer-events: all;
            box-shadow: 0 1px 3px rgba(0,0,0,.2);
        }
        .range-thumb::-moz-range-thumb {
            width: 16px; height: 16px;
            background: #fff; border: 2px solid #1a1a1a;
            border-radius: 50%; cursor: pointer; pointer-events: all;
        }
        .price-labels {
            display: flex; justify-content: space-between;
            font-size: 12px; color: #444; margin-top: 4px;
        }

        /* ── Top filter bar ── */
        .top-filter-bar {
            display: flex; align-items: center;
            justify-content: space-between;
            padding: 10px 14px 10px;
            border-bottom: 1px solid #e8e8e8;
            margin-bottom: 20px;
            flex-wrap: nowrap; gap: 10px;
            background-color: #f5f5f5;
        }
        .top-filter-selects { display: flex; gap: 10px; align-items: center; flex-shrink: 0; }
        .top-filter-select {
            border: 1px solid #ddd; border-radius: 3px;
            padding: 6px 10px; font-size: 13px; color: #333;
            background: #fff; cursor: pointer;
        }
        /* Top pager: hide page nav, show count only */
        .top-pager-wrap { flex: 1; min-width: 0; }
        .top-pager-wrap .pagination { display: none; }
        .top-pager-wrap .product-pagination { border: none; padding: 0; margin: 0; }
        .top-pager-wrap .col-xl-6:first-child { display: none; }
        .top-pager-wrap .col-xl-6:last-child { flex: 0 0 100%; max-width: 100%; }
        .top-pager-wrap .theme-paggination-block > .row { margin: 0; }
        .top-pager-wrap .product-search-count-bottom {
            border: none; padding: 0; text-align: right;
        }
        .top-pager-wrap .product-search-count-bottom h5 {
            font-size: 13px; color: #555; margin: 0;
            font-weight: normal; white-space: nowrap;
        }

        /* ── Product Cards ── */
        .product-card { margin-bottom: 28px; transition: box-shadow .2s; }
        .product-card:hover { box-shadow: 0 4px 16px rgba(0,0,0,.10); border-radius: 4px; }
        .product-card-img {
            width: 100%; aspect-ratio: 1/1; overflow: hidden;
            background: #f8f8f8; border-radius: 4px;
            display: flex; align-items: center; justify-content: center;
        }
        .product-card-img img { width: 100%; height: 100%; object-fit: contain; }
        .product-card-info { padding: 10px 18px 14px; text-align: center; }
        .product-card-name { font-size: 13px; font-weight: 600; color: #222; margin-bottom: 4px; line-height: 1.4; }
        .product-card-name a { color: inherit; text-decoration: none; }
        .product-card-name a:hover { color: #ff4c3b; }
        .product-card-price { font-size: 13px; font-weight: 600; color: #222; margin-bottom: 8px; }
        .product-card-colors { display: flex; flex-wrap: wrap; gap: 5px; justify-content: center; }
        .color-dot { width: 16px; height: 16px; border-radius: 50%; border: 1px solid #ccc; display: inline-block; }

        /* ── Category header bar ── */
        .items-category-header { background: #f2f2f2; padding: 18px 0 14px; text-align: center; width: 100%; margin-bottom: 0; }
        .items-category-title { font-size: 28px; font-weight: 400; letter-spacing: 2px; color: #222; margin: 0 0 4px; text-transform: uppercase; font-family: Lato, sans-serif; }
        .items-breadcrumb { font-size: 13px; color: #222; margin: 0; }
        .items-breadcrumb a { color: #222; text-decoration: none; pointer-events: none; cursor: default; }

        /* ── Category Breadcrumb Bar (matches ProductDetail) ── */
        .pd-breadcrumb-bar { background: #f2f2f2; padding: 10px 0; text-align: center; margin-bottom: 40px; width: 100%; }
        .pd-breadcrumb-bar .pd-breadcrumb { font-size: 13px; color: #666; margin: 0; }
        .pd-breadcrumb-bar a { color: #666; text-decoration: none; pointer-events: none; cursor: default; }

        /* ── Bottom Pager ── */
        /* Container: no border, no background */
        .bottom-pager .product-pagination { border: none !important; background: none; padding: 0; }
        /* Hide the old postback table pager */
        .bottom-pager table { display: none; }

        /* Layout: numbers truly centered, text pinned to right via absolute positioning */
        .bottom-pager .theme-paggination-block .row { display: flex; flex-direction: row; align-items: center; position: relative; flex-wrap: nowrap; }
        /* DOM col 1 = nav (numbers) → fills full width so nav can center across the whole row */
        .bottom-pager .col-xl-6:first-child { order: 1; flex: 1; border: none !important; box-shadow: none !important; padding: 0; }
        /* DOM col 2 = text → absolutely positioned on the right so it doesn't push numbers off-center */
        .bottom-pager .col-xl-6:last-child { order: 2; position: absolute; right: 0; flex: none; width: auto; text-align: right; border: none !important; box-shadow: none !important; padding: 0; }
        .bottom-pager nav { display: flex; justify-content: center; }
        .bottom-pager .product-search-count-bottom { border: none !important; }
        .bottom-pager .product-search-count-bottom h5,
        .bottom-pager .product-search-count-bottom h5 * { color: #333 !important; font-size: 13px; font-weight: normal; margin: 0; border: none !important; }

        /* Page links: kill ALL Bootstrap pagination margins so gap is the only spacing */
        .bottom-pager .pagination { display: flex; flex-wrap: wrap; justify-content: center; gap: 4px; margin: 0; padding: 0; list-style: none; }
        .bottom-pager .page-item,
        .bottom-pager .page-item + .page-item { list-style: none; margin: 0 !important; padding: 0 !important; }
        /* Hide ghost <li> elements that have no <a> inside — ASP.NET renders both active/inactive
           li for every page number and hides the unwanted <a>, leaving an empty li that eats gap space */
        .bottom-pager .page-item:not(:has(a)) { display: none !important; }

        /* Base box style — every numbered page-link shares this exactly */
        .bottom-pager .page-link {
            display: flex !important; align-items: center !important; justify-content: center !important;
            width: 34px !important; height: 34px !important; padding: 0 !important; margin: 0 !important;
            border: 2px solid #d0d0d0 !important;
            border-radius: 0 !important;
            background: #fff !important; color: #333 !important;
            font-size: 13px !important; font-weight: 500 !important;
            text-decoration: none !important;
            box-sizing: border-box !important;
            line-height: 1 !important;
            position: static !important; z-index: auto !important;
            transition: border-color .15s, background .15s, color .15s;
        }
        .bottom-pager .page-link:hover { border-color: #999 !important; color: #111 !important; background: #f5f5f5 !important; }

        /* Active: ONLY change fill + text color — everything else identical to base */
        .bottom-pager .page-item.active .page-link {
            background: #000 !important; color: #fff !important;
            border-color: #d0d0d0 !important; pointer-events: none;
        }

        /* Hide FIRST and LAST links */
        .bottom-pager .pagination > li:first-child,
        .bottom-pager .pagination > li:last-child { display: none; }

        /* PREVIOUS arrow — no box, just the ‹ character */
        .bottom-pager .pagination > li:nth-child(2) .page-link {
            font-size: 0 !important; border: none !important; background: transparent !important;
            width: 28px !important; height: 34px !important;
            position: relative !important; top: -3px !important;
        }
        .bottom-pager .pagination > li:nth-child(2) .page-link::before { content: '\2039'; font-size: 24px; line-height: 1; color: #555; }

        /* NEXT arrow — no box, just the › character */
        .bottom-pager .pagination > li:nth-last-child(2) .page-link {
            font-size: 0 !important; border: none !important; background: transparent !important;
            width: 28px !important; height: 34px !important;
            position: relative !important; top: -3px !important;
        }
        .bottom-pager .pagination > li:nth-last-child(2) .page-link::before { content: '\203A'; font-size: 24px; line-height: 1; color: #555; }

        /* ── Category header bar (identical to ProductDetail) ── */
        .pd-category-header { background: #f2f2f2; padding: 18px 0 14px; text-align: center; width: 100%; margin-bottom: 40px; padding-bottom: 20px; }
        .pd-category-title { font-size: 28px; font-weight: 400; letter-spacing: 2px; color: #222; margin: 0 0 4px; text-transform: uppercase; font-family: Lato, sans-serif; }
        .pd-breadcrumb { font-size: 13px; color: #222; margin: 0; }
        .pd-breadcrumb a { color: #222; text-decoration: none; pointer-events: none; cursor: default; }

        /* ── Category Cards ── */
        .category-card { border-radius: 4px; overflow: hidden; margin-bottom: 24px; text-align: center; transition: box-shadow .2s; }
        .category-card:hover { box-shadow: 0 4px 16px rgba(0,0,0,.10); }
        .category-card-img { width: 100%; aspect-ratio: 1/1; overflow: hidden; background: #f8f8f8; display: flex; align-items: center; justify-content: center; }
        .category-card-img img { width: 100%; height: 100%; object-fit: contain; }
        .category-card-name { padding: 12px; font-size: 13px; font-weight: 600; color: #222; }
        .category-card-name a { color: inherit; text-decoration: none; }
        .category-card-name a:hover { color: #ff4c3b; }
    </style>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="cphHeader" runat="server">
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="cphBody" runat="server">
    <!-- section start -->
    <section class="section-b-space ratio_asos">

        <%-- Category header bar — identical to ProductDetail, full width outside container --%>
        <asp:Panel ID="pnlCategoryBreadCrumb" runat="server" CssClass="pd-category-header">
            <div class="pd-category-title" id="divCategoryTitle"></div>
            <div class="pd-breadcrumb" id="divCategoryBreadcrumb">
                <asp:Literal ID="litCategoryBreadCrumb" runat="server"></asp:Literal>
            </div>
        </asp:Panel>

        <div class="collection-wrapper">
            <div class="container">
                <div class="row">

                    <!--Begin Left Panel-->
                    <div class="col-lg-2 col-sm-3 sidebar-panel" id="divLeftPanel" runat="server">

                        <!-- Category Section -->
                        <asp:Panel ID="pnlSidebarCategories" runat="server" Visible="false">
                            <div class="sidebar-section">
                                <div class="sidebar-section-header" onclick="toggleSidebarSection(this)">
                                    <span class="sidebar-section-title">Category</span>
                                    <i class="fa fa-chevron-up sidebar-chevron"></i>
                                </div>
                                <div class="sidebar-section-content">
                                    <uc2:LeftPanelNavigation runat="server" ID="ucLeftPanelNavigation" />
                                    <asp:Repeater ID="rptSidebarCategories" runat="server">
                                        <ItemTemplate>
                                            <a href="/items.aspx?websitetabid=<%# Eval("WebsiteTabID")%>" class="sidebar-cat-link"><%# Eval("TabName")%></a>
                                        </ItemTemplate>
                                    </asp:Repeater>
                                </div>
                            </div>
                        </asp:Panel>

                        <!-- Color / Size / Price Filters -->
                        <asp:Panel ID="pnlAttributeFilter" runat="server">

                            <!-- Color -->
                            <div class="sidebar-section">
                                <div class="sidebar-section-header" onclick="toggleSidebarSection(this)">
                                    <span class="sidebar-section-title">Color</span>
                                    <i class="fa fa-chevron-up sidebar-chevron"></i>
                                </div>
                                <div class="sidebar-section-content sidebar-scroll">
                                    <asp:ListView ID="lvColor" runat="server" AutoGenerateColumns="false" DataKeyNames="AttributeValue">
                                        <LayoutTemplate>
                                            <div class="filter-list"><asp:PlaceHolder ID="itemPlaceholder" runat="server" /></div>
                                        </LayoutTemplate>
                                        <ItemTemplate>
                                            <div class="filter-item">
                                                <div class="form-check">
                                                    <asp:CheckBox ID="chkColor" runat="server" CssClass="form-check-input" AutoPostBack="true" OnCheckedChanged="chkColor_CheckedChanged" />
                                                    <label class="form-check-label"><%# Eval("AttributeValue") %></label>
                                                </div>
                                            </div>
                                        </ItemTemplate>
                                    </asp:ListView>
                                </div>
                            </div>

                            <!-- Size -->
                            <div class="sidebar-section">
                                <div class="sidebar-section-header" onclick="toggleSidebarSection(this)">
                                    <span class="sidebar-section-title">Size</span>
                                    <i class="fa fa-chevron-up sidebar-chevron"></i>
                                </div>
                                <div class="sidebar-section-content">
                                    <asp:ListView ID="lvSize" runat="server" AutoGenerateColumns="false" DataKeyNames="AttributeValue">
                                        <LayoutTemplate>
                                            <div class="filter-list"><asp:PlaceHolder ID="itemPlaceholder" runat="server" /></div>
                                        </LayoutTemplate>
                                        <ItemTemplate>
                                            <div class="filter-item">
                                                <div class="form-check">
                                                    <asp:CheckBox ID="chkSize" runat="server" CssClass="form-check-input" AutoPostBack="true" OnCheckedChanged="chkSize_CheckedChanged" />
                                                    <label class="form-check-label"><%# Eval("AttributeValue") %></label>
                                                </div>
                                            </div>
                                        </ItemTemplate>
                                    </asp:ListView>
                                </div>
                            </div>

                            <!-- Price -->
                            <div class="sidebar-section">
                                <div class="sidebar-section-header" onclick="toggleSidebarSection(this)">
                                    <span class="sidebar-section-title">Price</span>
                                    <i class="fa fa-chevron-up sidebar-chevron"></i>
                                </div>
                                <div class="sidebar-section-content">
                                    <div class="price-range-wrap">
                                        <div class="price-track">
                                            <div class="price-fill" id="priceFill"></div>
                                        </div>
                                        <input type="range" id="rangeThumbMin" class="range-thumb" min="0" max="1000" value="0" step="1">
                                        <input type="range" id="rangeThumbMax" class="range-thumb" min="0" max="1000" value="1000" step="1">
                                    </div>
                                    <div class="price-labels">
                                        <span id="spnPriceMin">$0</span>
                                        <span id="spnPriceMax">$0</span>
                                    </div>
                                    <asp:HiddenField ID="hfPriceMin" runat="server" Value="0" />
                                    <asp:HiddenField ID="hfPriceMax" runat="server" />
                                    <asp:HiddenField ID="hfPriceAbsMax" runat="server" Value="0" />
                                    <%-- Hidden apply button — triggered by JS on slider release --%>
                                    <asp:Button ID="btnApplyPrice" runat="server" Text="Apply" OnClick="btnApplyPrice_Click" style="display:none;" />
                                </div>
                            </div>

                        </asp:Panel>
                    </div>
                    <!--End Left Panel-->

                    <div class="collection-content col">
                        <div class="page-main-content">

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
                                        <div class="row">
                                            <asp:Repeater ID="rptCategory" runat="server" OnItemCommand="rptCategory_ItemCommand">
                                                <ItemTemplate>
                                                    <div class="col-6 col-md-4 col-lg-3">
                                                        <div class="category-card">
                                                            <div class="category-card-img">
                                                                <a href="/items.aspx?websitetabid=<%# Eval("WebsiteTabID")%>">
                                                                    <img src='<%# Eval("ImageURL")%>' class="img-fluid blur-up lazyload" alt="">
                                                                </a>
                                                            </div>
                                                            <div class="category-card-name">
                                                                <a href="/items.aspx?websitetabid=<%# Eval("WebsiteTabID")%>"><%# Eval("TabName")%></a>
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

                                    <!-- Top filter bar -->
                                    <div class="top-filter-bar">
                                        <div class="top-filter-selects">
                                            <asp:DropDownList ID="ddlSort" runat="server" AutoPostBack="true" OnSelectedIndexChanged="ddlSort_SelectedIndexChanged" CssClass="top-filter-select">
                                                <asp:ListItem Text="Relevance" Value="Relevance" Selected="True"></asp:ListItem>
                                                <asp:ListItem Text="Price: Low to High" Value="Price_ASC"></asp:ListItem>
                                                <asp:ListItem Text="Price: High to Low" Value="Price_DESC"></asp:ListItem>
                                            </asp:DropDownList>
                                            <asp:DropDownList ID="ddlPageSize" runat="server" AutoPostBack="true" OnSelectedIndexChanged="ddlPageSize_SelectedIndexChanged" CssClass="top-filter-select">
                                                <asp:ListItem Text="10 Products" Value="10"></asp:ListItem>
                                                <asp:ListItem Text="25 Products" Value="25" Selected="True"></asp:ListItem>
                                                <asp:ListItem Text="50 Products" Value="50"></asp:ListItem>
                                                <asp:ListItem Text="100 Products" Value="100"></asp:ListItem>
                                            </asp:DropDownList>
                                        </div>
                                        <div class="top-pager-wrap">
                                            <uc1:Pager runat="server" ID="ucPagerTop" PagingMode="Redirect" PageSize="25" PagingRecordText="Items" />
                                        </div>
                                    </div>

                                    <div class="product-wrapper-grid">
                                        <div class="row">
                                            <asp:Repeater ID="rptItems" runat="server" OnItemCommand="rptItems_ItemCommand" OnItemDataBound="rptItems_ItemDataBound">
                                                <ItemTemplate>
                                                    <div class="col-6 col-md-4 col-lg-3">
                                                        <div class="product-card">
                                                            <div class="product-card-img">
                                                                <a href="/ProductDetail.aspx?id=<%# Eval("Item.ItemID")%>&websitetabid=<%# mWebSiteTabID %>">
                                                                    <img src='<%# Eval("Item.DisplayImageURL")%>' class="img-fluid blur-up lazyload" alt="">
                                                                </a>
                                                            </div>
                                                            <div class="product-card-info">
                                                                <h6 class="product-card-name">
                                                                    <a href="/ProductDetail.aspx?id=<%# Eval("Item.ItemID")%>&websitetabid=<%# mWebSiteTabID %>"><%# ToTitleCase(Eval("Item.StoreDisplayName"))%></a>
                                                                </h6>
                                                                <p class="product-card-price"><%# Eval("Item.PriceRange") %></p>
                                                                <div class="product-card-colors">
                                                                    <asp:Literal ID="litColorSwatches" runat="server"></asp:Literal>
                                                                </div>
                                                            </div>
                                                            <asp:Button id="btnAddToCart" runat="server" Visible="false" Text="Add To Cart" CssClass="btn btn-solid" CommandName="AddItem" CommandArgument='<%# Eval("Item.ItemID") %>'/>
                                                        </div>
                                                    </div>
                                                </ItemTemplate>
                                            </asp:Repeater>
                                        </div>
                                    </div>

                                    <div class="bottom-pager">
                                        <uc1:Pager runat="server" ID="ucPager" PagingMode="Redirect" PageSize="25" PagingRecordText="Items" />
                                    </div>
                                </div>
                            </asp:Panel>

                        </div>
                    </div>

                </div>
            </div>
        </div>
    </section>
    <!-- section End -->

    <script>
        // ── Category header: extract last segment as title, replace > with / ──
        document.addEventListener('DOMContentLoaded', function () {
            var bcLit = document.querySelector('#divCategoryBreadcrumb');
            if (bcLit) {
                var raw = bcLit.innerText || bcLit.textContent || '';
                raw = raw.replace(/ &gt; /g, ' > ');
                var segments = raw.split('>').map(function (s) { return s.trim(); }).filter(function (s) { return s.length > 0; });
                if (segments.length > 0) {
                    var titleEl = document.getElementById('divCategoryTitle');
                    if (titleEl) titleEl.textContent = segments[segments.length - 1].toUpperCase();
                }
                bcLit.innerHTML = bcLit.innerHTML.replace(/ &gt; /g, ' / ').replace(/ > /g, ' / ');
            }
        });

        // ── Collapsible sidebar sections ──
        function toggleSidebarSection(header) {
            var section = header.closest('.sidebar-section');
            var chevron = header.querySelector('.sidebar-chevron');
            section.classList.toggle('collapsed');
            // Swap chevron direction
            if (section.classList.contains('collapsed')) {
                chevron.classList.remove('fa-chevron-up');
                chevron.classList.add('fa-chevron-down');
            } else {
                chevron.classList.remove('fa-chevron-down');
                chevron.classList.add('fa-chevron-up');
            }
        }

        // ── Dual-handle price range slider ──
        (function () {
            var thumbMin, thumbMax, fill, spnMin, spnMax, hfMin, hfMax, hfAbsMax, applyBtnId;

            function getAbsMax() {
                var v = parseFloat(hfAbsMax.value);
                return (isNaN(v) || v <= 0) ? 100 : v;
            }
            function clamp(val, lo, hi) { return Math.max(lo, Math.min(hi, val)); }

            function redraw() {
                var lo = parseFloat(thumbMin.value), hi = parseFloat(thumbMax.value), mx = getAbsMax();
                var pct1 = mx > 0 ? (lo / mx) * 100 : 0;
                var pct2 = mx > 0 ? (hi / mx) * 100 : 100;
                fill.style.left = pct1 + '%';
                fill.style.width = (pct2 - pct1) + '%';
                spnMin.textContent = '$' + Math.round(lo);
                spnMax.textContent = '$' + Math.round(hi);
            }

            function submitFilter(minVal, maxVal) {
                hfMin.value = minVal;
                hfMax.value = maxVal;
                // Use __doPostBack for reliable ASP.NET postback
                if (typeof __doPostBack === 'function' && applyBtnId) {
                    __doPostBack(applyBtnId, '');
                }
            }

            window.addEventListener('load', function () {
                thumbMin  = document.getElementById('rangeThumbMin');
                thumbMax  = document.getElementById('rangeThumbMax');
                fill      = document.getElementById('priceFill');
                spnMin    = document.getElementById('spnPriceMin');
                spnMax    = document.getElementById('spnPriceMax');
                hfMin     = document.getElementById('<%=hfPriceMin.ClientID%>');
                hfMax     = document.getElementById('<%=hfPriceMax.ClientID%>');
                hfAbsMax  = document.getElementById('<%=hfPriceAbsMax.ClientID%>');
                applyBtnId = '<%=btnApplyPrice.UniqueID%>';

                if (!thumbMin || !hfAbsMax) return;

                var mx  = getAbsMax();
                var lo  = clamp(parseFloat(hfMin.value) || 0, 0, mx);
                var hi  = clamp((hfMax.value !== '' ? parseFloat(hfMax.value) : mx), 0, mx);
                if (hi <= 0) hi = mx;

                thumbMin.max = mx; thumbMax.max = mx;
                thumbMin.value = lo; thumbMax.value = hi;
                redraw();

                thumbMin.addEventListener('input', function () {
                    if (parseFloat(thumbMin.value) > parseFloat(thumbMax.value))
                        thumbMin.value = thumbMax.value;
                    redraw();
                });
                thumbMax.addEventListener('input', function () {
                    if (parseFloat(thumbMax.value) < parseFloat(thumbMin.value))
                        thumbMax.value = thumbMin.value;
                    redraw();
                });
                // Submit on mouse/touch release
                thumbMin.addEventListener('change', function () {
                    submitFilter(thumbMin.value, thumbMax.value);
                });
                thumbMax.addEventListener('change', function () {
                    submitFilter(thumbMin.value, thumbMax.value);
                });
            });
        })();

        // ── Category header: extract last segment as title ──
        window.addEventListener('load', function () {
            var bcDiv = document.getElementById('divItemsBreadcrumb');
            if (bcDiv) {
                var raw = bcDiv.innerText || bcDiv.textContent || '';
                raw = raw.replace(/ &gt; /g, ' > ');
                var segments = raw.split('>').map(function (s) { return s.trim(); }).filter(function (s) { return s.length > 0; });
                if (segments.length > 0) {
                    var titleEl = document.getElementById('divItemsCategoryTitle');
                    if (titleEl) titleEl.textContent = segments[segments.length - 1].toUpperCase();
                }
                bcDiv.innerHTML = bcDiv.innerHTML.replace(/ &gt; /g, ' / ').replace(/ > /g, ' / ');
            }
        });
    </script>
</asp:Content>
