<%@ Page Title="" Language="C#" MasterPageFile="~/MasterPage.Master" AutoEventWireup="true" CodeBehind="ProductDetail.aspx.cs" Inherits="ImageSolutionsWebsite.ProductDetail" %>
<%@ Register src="Control/SuperceedingItem.ascx" tagname="SuperceedingItem" tagprefix="uc1" %>
<%@ Register src="Control/ImageModal.ascx" tagname="ImageModal" tagprefix="uc2"  %>
<%@ Register Src="Control/LeftPanelNavigation.ascx" tagname="LeftPanelNavigation" tagprefix="uc3" %>
<%@ Register src="Control/AccountSearchModal.ascx" tagname="AccountSearchModal" tagprefix="uc4"  %>
<%@ Register Src="~/Control/Pager.ascx" TagPrefix="uc1" TagName="Pager" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <style>
        /* ── Layout ── */
        .pd-section { padding: 32px 0 60px; background: #fff; }
        .pd-image-col { padding-right: 32px; }

        /* ── Main image ── */
        .pd-main-image-wrap { position: relative; border-radius: 4px; overflow: hidden; margin-bottom: 12px; }
        .pd-main-image-wrap img { width: 100%; height: auto; display: block; }
        .pd-img-nav { position: absolute; top: 50%; transform: translateY(-50%); background: rgba(255,255,255,.85); border: 1px solid #ddd; border-radius: 50%; width: 36px; height: 36px; display: flex; align-items: center; justify-content: center; cursor: pointer; font-size: 20px; color: #555; z-index: 2; transition: background .2s; }
        .pd-img-nav:hover { background: #fff; }
        .pd-img-prev { left: 10px; }
        .pd-img-next { right: 10px; }

        /* ── Thumbnails ── */
        .pd-thumbnails { display: flex; gap: 8px; justify-content: center; }
        .pd-thumb { border: 1px solid #ddd; border-radius: 4px; overflow: hidden; cursor: pointer; width: 90px; height: 90px; flex-shrink: 0; }
        .pd-thumb img { width: 100%; height: 100%; object-fit: cover; }
        .pd-thumb.active { border-color: #333; }

        /* ── Category header bar ── */
        .pd-category-header { background: #f2f2f2; padding: 18px 0 14px; text-align: center; width: 100%; margin-bottom: 0; padding-bottom: 20px;}
        .pd-category-title { font-size: 28px; font-weight: 400; letter-spacing: 2px; color: #222; margin: 0 0 4px; text-transform: uppercase; font-family: Lato, sans-serif; }
        .pd-breadcrumb { font-size: 13px; color: #222; margin: 0; }
        .pd-breadcrumb a { color: #222; text-decoration: none; pointer-events: none; cursor: default; }

        /* ── Details right col ── */
        .pd-details-col { padding-left: 24px; }
        .pd-title { font-size: 24px; font-weight: 700; color: #222 !important; margin: 0 0 8px; font-family: Lato, sans-serif; }
        .pd-title, .pd-title * { color: #222 !important; font-size: 24px !important; text-transform: none !important; }
        #cphBody_lblHeader { color: #222 !important; font-size: 24px !important; }
        .pd-item-num { font-size: 13px; color: #222; margin: 0 0 6px; font-family: Lato, sans-serif; }
        .pd-price { font-size: 22px; font-weight: 700; color: #222; margin: 10px 0 14px; font-family: Lato, sans-serif; }

        /* ── Non-inventory note ── */
        .pd-ni-note { background: #fff8f8; border: 1px solid #f5c2c2; border-radius: 4px; padding: 10px 14px; margin-bottom: 16px; font-size: 12px; color: #c00; font-family: Lato, sans-serif; line-height: 1.6; }

        /* ── Single-unit link ── */
        .pd-single-unit { font-size: 13px; margin-bottom: 12px; font-family: Lato, sans-serif; }
        .pd-single-unit a { color: #C01F2F; }

        /* ── Attribute section label ── */
        .pd-attr-label { font-size: 13px; font-weight: 600; color: #333; margin-bottom: 8px; font-family: Lato, sans-serif; display: flex; align-items: center; gap: 8px; }
        .pd-attr-label .pd-attr-value { font-weight: 400; color: #555; }

        /* ── Color swatches ── */
        .pd-color-section { margin-bottom: 18px; }
        .pd-swatches { display: flex; flex-wrap: wrap; gap: 8px; align-items: center; }
        .pd-swatches li { list-style: none; margin: 0; padding: 0; background: none !important; border: none !important; }
        .pd-swatch-btn { display: block; width: 32px; height: 32px; border-radius: 50%; border: 1px solid #ccc; outline: none; cursor: pointer; padding: 0; overflow: hidden; }
        .pd-swatch-btn:hover { outline: 2px solid #888; outline-offset: 2px; border-radius: 50%; }
        .pd-swatch-btn.pd-swatch-selected { border: none; outline: 2px solid #000; outline-offset: 2px; border-radius: 50%; }
        .pd-swatch-text { display: flex; align-items: center; justify-content: center; background: #f3f3f3; font-size: 11px; font-weight: 600; color: #333; }

        /* ── Size section ── */
        .pd-size-section { margin-bottom: 18px; }
        .pd-size-guide-link { font-size: 12px; color: #C01F2F; text-decoration: underline; cursor: pointer; }
        .pd-sizes { display: flex; flex-wrap: wrap; gap: 8px; margin-top: 8px; }
        .size-btn { min-width: 48px; padding: 6px 10px; border: 1px solid #ccc; border-radius: 4px; background: #fff; font-size: 13px; font-weight: 500; color: #333; cursor: pointer; transition: border-color .15s, background .15s, color .15s; font-family: Lato, sans-serif; text-align: center; }
        .size-btn:hover:not(.out-of-stock):not(:disabled) { border-color: #333; }
        .size-btn.active { border-color: #333; background: #333; color: #fff; }
        .size-btn.out-of-stock { position: relative; color: #bbb; border-color: #e0e0e0; cursor: not-allowed; text-decoration: line-through; }

        /* size-grid-table: flatten GridView table into flex row */
        .size-grid-table { width: auto !important; border: none !important; }
        .size-grid-table thead { display: none !important; }
        .size-grid-table tbody { display: flex !important; flex-wrap: wrap; gap: 8px; }
        .size-grid-table tr { display: contents; }
        .size-grid-table td { display: block; padding: 0 !important; border: none !important; background: none !important; }
        .size-btn-item { display: inline-block; }
        .size-qty-input { position: absolute; opacity: 0; pointer-events: none; width: 1px; height: 1px; }

        /* ── Shared quantity control ── */
        .pd-qty-section { margin-bottom: 20px; }
        .pd-qty-control { display: inline-flex; align-items: center; border: 1px solid #ccc; border-radius: 4px; overflow: hidden; }
        .pd-qty-btn { width: 38px; height: 40px; border: none; background: #f5f5f5; font-size: 18px; cursor: pointer; color: #444; display: flex; align-items: center; justify-content: center; transition: background .15s; }
        .pd-qty-btn:hover { background: #e8e8e8; }
        .pd-qty-display { width: 52px; height: 40px; border: none; border-left: 1px solid #ccc; border-right: 1px solid #ccc; text-align: center; font-size: 15px; font-weight: 600; color: #222; -moz-appearance: textfield; outline: none; }
        .pd-qty-display::-webkit-inner-spin-button,
        .pd-qty-display::-webkit-outer-spin-button { -webkit-appearance: none; }
        .pd-qty-info { font-size: 12px; color: #777; margin-top: 6px; font-family: Lato, sans-serif; }
        .pd-unit-price { font-size: 13px; color: #444; margin-left: 12px; }

        /* ── Simple qty (no-attribute items) ── */
        .pd-simple-qty { margin-bottom: 20px; }

        /* ── Action buttons ── */
        .pd-actions { display: flex; flex-wrap: wrap; gap: 12px; margin-bottom: 24px; }
        .btn-add-to-bag { background: #C01F2F; color: #fff; border: none; border-radius: 4px; padding: 12px 28px; font-size: 14px; font-weight: 600; letter-spacing: .5px; cursor: pointer; font-family: Lato, sans-serif; transition: background .2s; }
        .btn-add-to-bag:hover:not(:disabled) { background: #a01828; color: #fff; }
        .btn-add-to-bag:disabled, .btn-buy-now:disabled { background: #ccc !important; border-color: #ccc !important; color: #888 !important; cursor: not-allowed; }
        .btn-buy-now { background: #C01F2F; color: #fff; border: 2px solid #C01F2F; border-radius: 4px; padding: 10px 28px; font-size: 14px; font-weight: 600; letter-spacing: .5px; cursor: pointer; font-family: Lato, sans-serif; transition: background .2s, color .2s; }
        .btn-buy-now:hover:not(:disabled) { background: #a01828; border-color: #a01828; color: #fff; }
        .btn-size-chart { background: none; color: #555; border: 1px solid #ccc; border-radius: 4px; padding: 10px 18px; font-size: 13px; cursor: pointer; font-family: Lato, sans-serif; transition: border-color .2s; }
        .btn-size-chart:hover { border-color: #333; color: #222; }
        .btn-add-more { background: none; color: #C01F2F; border: 1px solid #C01F2F; border-radius: 4px; padding: 10px 18px; font-size: 13px; font-weight: 600; cursor: pointer; font-family: Lato, sans-serif; }

        /* ── Accordion ── */
        .pd-accordion { margin-top: 24px; border-top: 1px solid #e8e8e8; }
        .pd-accordion-item { border-bottom: 1px solid #e8e8e8; }
        .pd-accordion-header { display: flex; justify-content: space-between; align-items: center; padding: 14px 0; cursor: pointer; font-size: 15px; font-weight: 600; color: #222; font-family: Lato, sans-serif; user-select: none; }
        .pd-accordion-header:hover { color: #C01F2F; }
        .pd-accordion-icon { font-size: 18px; color: #555; transition: transform .2s; }
        .pd-accordion-body { padding-bottom: 14px; font-size: 13px; color: #555; font-family: Lato, sans-serif; line-height: 1.7; }
        .pd-accordion-body ul { padding-left: 18px; margin: 0; }
        .pd-accordion-body li { margin-bottom: 4px; }

        /* ── Detailed description (inside accordion body) ── */
        #divDetailedDescription ul { font-family: Lato,sans-serif; font-size: 13px; display: block; }
        #divDetailedDescription ul li { font-family: Lato,sans-serif; font-size: 13px; display: list-item; }
        #divDetailedDescription h2, #divDetailedDescription h3, #divDetailedDescription h4 { font-family: Lato,sans-serif; font-size: 13px; text-transform: unset; font-weight: unset; letter-spacing: unset; line-height: 1.5em; }
        #divDetailedDescription p { font-family: Lato,sans-serif; font-size: 13px; margin-bottom: 8px; }
        #divDetailedDescription span { font-family: Lato,sans-serif; font-size: 13px; }

        /* ── Employee / Account ── */
        .pd-employee-section { margin-bottom: 18px; }
        .pd-employee-section .form-group { margin-bottom: 12px; }
        .pd-employee-section label { font-size: 13px; font-weight: 600; color: #333; margin-bottom: 4px; display: block; font-family: Lato, sans-serif; }

        /* ── Dropdown / NoGroup attributes ── */
        .pd-nogroup-attr { margin-bottom: 18px; }
        .pd-nogroup-attr label { font-size: 13px; font-weight: 600; color: #333; margin-bottom: 4px; display: block; font-family: Lato, sans-serif; }

        /* ── Length/Width dropdown ── */
        .pd-lw-section select { max-width: 220px; }

        /* ── Customization ── */
        .pd-customization { border: 1px solid #e8e8e8; border-radius: 6px; padding: 18px; margin-bottom: 20px; }
        .pd-customization h5 { font-size: 15px; font-weight: 700; margin-bottom: 14px; font-family: Lato, sans-serif; }
        .pd-logo-grid { display: flex; flex-wrap: wrap; gap: 10px; background: #f5f5f5; padding: 10px; border-radius: 4px; list-style: none; margin: 0; padding-left: 0; }
        .pd-logo-grid li { list-style: none; }

        /* ── Related items carousel ── */
        .pd-related { margin-top: 60px; padding-top: 30px; border-top: 1px solid #e8e8e8; }
        .pd-related h2 { font-size: 20px; font-weight: 700; text-transform: uppercase; letter-spacing: 1px; margin-bottom: 24px; font-family: Lato, sans-serif; color: #222; text-align: center; }
        .pd-related-carousel { display: flex; align-items: center; gap: 8px; }
        .related-track { flex: 1; overflow: hidden; }
        .related-arrow { background: #f5f5f5; border: 1px solid #ddd; border-radius: 50%; width: 36px; height: 36px; font-size: 22px; cursor: pointer; display: flex; align-items: center; justify-content: center; flex-shrink: 0; color: #444; transition: background .15s; padding-bottom: 6px; }
        .related-arrow:hover { background: #e0e0e0; }
        .pd-related-item .img-wrapper { overflow: hidden; height: 200px; display: flex; align-items: center; justify-content: center; margin-bottom: 10px; }
        .pd-related-item .img-wrapper img { max-height: 100%; max-width: 100%; object-fit: contain; }
        .pd-related-item { text-align: center; }
        .pd-related-item h4 a { font-size: 14px; font-weight: 500; color: #555; text-decoration: none; font-family: Lato, sans-serif; }
        .pd-related-item h4 a:hover { color: #C01F2F; }
        .pd-related-price { font-size: 14px; font-weight: 600; color: #222; font-family: Lato, sans-serif; }

        /* ── Recommended for You (Items.aspx card style) ── */
        .pd-recommended { margin-top: 56px; padding-top: 36px; border-top: 1px solid #e8e8e8; }
        .pd-recommended-title { font-size: 20px; font-weight: 700; text-transform: uppercase; letter-spacing: 1px; margin-bottom: 28px; font-family: Lato, sans-serif; color: #222; }
        .product-card { margin-bottom: 28px; transition: box-shadow .2s; }
        .product-card:hover { box-shadow: 0 4px 16px rgba(0,0,0,.10); border-radius: 4px; }
        .product-card-img { width: 100%; aspect-ratio: 1/1; overflow: hidden; background: #f8f8f8; border-radius: 4px; display: flex; align-items: center; justify-content: center; }
        .product-card-img img { width: 100%; height: 100%; object-fit: contain; }
        .product-card-info { padding: 10px 8px 14px; text-align: center; }
        .product-card-name { font-size: 13px; font-weight: 600; color: #222; margin-bottom: 4px; line-height: 1.4; }
        .product-card-name a { color: inherit; text-decoration: none; }
        .product-card-name a:hover { color: #ff4c3b; }
        .product-card-price { font-size: 13px; font-weight: 600; color: #222; margin-bottom: 8px; }
        .product-card-colors { display: flex; flex-wrap: wrap; gap: 5px; justify-content: center; }
        .color-dot { width: 16px; height: 16px; border-radius: 50%; border: 1px solid #ccc; display: inline-block; cursor: default; }

        .ti-info-alt:before { content: "\0043\0024"; }

        @media (max-width: 767px) {
            .pd-image-col { padding-right: 15px; margin-bottom: 24px; }
            .pd-details-col { padding-left: 15px; }
        }
    </style>
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="cphHeader" runat="server">
</asp:Content>

<asp:Content ID="Content3" ContentPlaceHolderID="cphBody" runat="server">

    <%-- Hidden left panel kept for code-behind compatibility --%>
    <div id="divLeftPanel" runat="server" style="display:none;">
        <uc3:LeftPanelNavigation runat="server" ID="ucLeftPanelNavigation" />
    </div>

    <%-- Category header bar --%>
    <asp:Panel ID="pnlCategoryBreadCrumb" runat="server" CssClass="pd-category-header">
        <div class="pd-category-title" id="divCategoryTitle"></div>
        <div class="pd-breadcrumb" id="divCategoryBreadcrumb">
            <asp:Literal ID="litCategoryBreadCrumb" runat="server"></asp:Literal>
        </div>
    </asp:Panel>

    <section class="pd-section">
        <div class="container">

            <div class="row">

                <%-- ══ LEFT: Image Column ══ --%>
                <div class="col-lg-5 pd-image-col">
                    <div class="pd-main-image-wrap">
                        <button type="button" class="pd-img-nav pd-img-prev" onclick="pdThumbNav(-1)">&#8249;</button>
                        <asp:Image ID="imgItem2" runat="server" alt="" CssClass="pd-main-img" Width="100%" />
                        <button type="button" class="pd-img-nav pd-img-next" onclick="pdThumbNav(1)">&#8250;</button>
                    </div>
                    <%-- Thumbnail strip — populated from main image; hidden original imgItem kept for code-behind compat --%>
                    <asp:Image ID="imgItem" runat="server" alt="" style="display:none;" />
                    <div class="pd-thumbnails" id="divThumbnails">
                        <%-- JS fills these from imgItem2.src after load --%>
                    </div>
                </div>

                <%-- ══ RIGHT: Details Column ══ --%>
                <div class="col-lg-7 pd-details-col">

                    <%-- Product name --%>
                    <h1 class="pd-title"><span id="ItemName"><asp:Label ID="lblHeader" runat="server" Text=""></asp:Label></span></h1>

                    <%-- Item number --%>
                    <p class="pd-item-num">Item#: <span id="ItemNumber"><asp:Literal ID="litItemNumber" runat="server" Text=""></asp:Literal></span></p>

                    <%-- Sales description --%>
                    <asp:Literal ID="litSalesDescription" runat="server"></asp:Literal>

                    <%-- Single-unit link --%>
                    <asp:Panel ID="pnlSingleUnit" runat="server" CssClass="pd-single-unit">
                        For a single unit purchase, &nbsp;<asp:HyperLink ID="btnSingleUnit" runat="server">Click Here</asp:HyperLink>
                    </asp:Panel>

                    <%-- Price --%>
                    <p class="pd-price"><asp:Literal ID="litBasePrice" runat="server"></asp:Literal></p>

                    <%-- Non-inventory message --%>
                    <div id="divNonInventoryUnavailableMessage" runat="server" class="pd-ni-note" visible="false">
                        <strong>PLEASE NOTE:</strong><br />
                        This is a custom-ordered item with a 10-12 day lead time, selecting expedited shipping will NOT expedite the lead time.
                        This item cannot be returned or exchanged due to the custom decoration.
                        We are unable to accept orders for items which are currently unavailable – if the size is greyed out, then we do not have stock.
                        Please check back regularly as stock levels are updated daily.
                    </div>

                    <%-- Employee / Account selectors --%>
                    <asp:PlaceHolder ID="phEmployee" runat="server" Visible="false">
                        <div class="pd-employee-section">
                            <asp:PlaceHolder ID="phEmployeeAccount" runat="server" Visible="false">
                                <div class="form-group">
                                    <label>Store</label>
                                    <div class="d-flex gap-2">
                                        <asp:DropDownList ID="ddlAccount" runat="server" Width="100%" DataValueField="AccountID" DataTextField="AccountName" CssClass="form-control form-select-sm form-select" AutoPostBack="true" OnSelectedIndexChanged="ddlAccount_SelectedIndexChanged"></asp:DropDownList>
                                        <asp:TextBox ID="txtAccount" runat="server" CssClass="form-control" Enabled="false"></asp:TextBox>
                                        <asp:HiddenField ID="hfAccountID" runat="server" />
                                        <asp:LinkButton ID="btnAccountSearch" runat="server" OnClick="btnAccountSearch_Click" CssClass="btn btn-solid btn-sm" style="white-space:nowrap;">Change</asp:LinkButton>
                                    </div>
                                </div>
                            </asp:PlaceHolder>
                            <div class="form-group">
                                <label>Employee</label>
                                <asp:DropDownList ID="ddlUserInfo" runat="server" Width="100%" DataValueField="UserInfoID" DataTextField="FullName" CssClass="form-control form-select-sm form-select" AutoPostBack="true" OnSelectedIndexChanged="ddlUserInfo_SelectedIndexChanged"></asp:DropDownList>
                            </div>
                        </div>
                    </asp:PlaceHolder>

                    <%-- ══ Item Attributes / Quantity (Repeater) ══ --%>
                    <asp:Repeater ID="rptItems" runat="server" DataMember="ItemID" OnItemDataBound="rptItems_ItemDataBound">
                        <ItemTemplate>
                            <asp:HiddenField ID="hfItemID" runat="server" Value='<%# Eval("Item.ItemID")%>' />

                            <%-- ── No attributes: simple quantity box ── --%>
                            <asp:Panel ID="pnlNoAttribute" runat="server">
                                <div class="pd-simple-qty">
                                    <div class="pd-attr-label">Quantity</div>
                                    <div class="d-flex align-items-center gap-3">
                                        <div class="pd-qty-control">
                                            <button type="button" class="pd-qty-btn pd-qty-minus">&minus;</button>
                                            <asp:TextBox ID="txtQuantity" runat="server" type="number" CssClass="pd-qty-display" min="0"></asp:TextBox>
                                            <button type="button" class="pd-qty-btn pd-qty-plus">+</button>
                                        </div>
                                    </div>
                                    <div class="pd-qty-info">
                                        <asp:Label ID="lblQuantityAvailable" runat="server"></asp:Label>
                                        &nbsp;<a id="aSuperceedingItem" runat="server" href='/ItemList.aspx?itemid=<%# Eval("ItemID")%>' visible="false" style="color:#C01F2F;font-size:12px;">Other Options</a>
                                    </div>
                                </div>
                            </asp:Panel>

                            <%-- ── With attributes ── --%>
                            <asp:Panel ID="pnlAttribute" runat="server" Visible="false">

                                <%-- No-group: dropdown per attribute + single qty --%>
                                <asp:Panel ID="pnlNoGroup" runat="server" Visible="false">
                                    <div class="pd-nogroup-attr">
                                        <asp:Repeater ID="rptNoGroupAttributes" runat="server" DataMember="AttributeID" OnItemDataBound="rptAttributes_ItemDataBound">
                                            <ItemTemplate>
                                                <div class="form-group mb-3">
                                                    <label><%# Eval("AttributeName")%></label>
                                                    <asp:DropDownList ID="ddlAttributeValue" runat="server" OnSelectedIndexChanged="ddlAttributeValue_SelectedIndexChanged" AutoPostBack="true" CssClass="form-control form-select-sm form-select"></asp:DropDownList>
                                                    <asp:HiddenField ID="hfAttributeID" runat="server" Value='<%# Eval("AttributeID")%>' />
                                                </div>
                                            </ItemTemplate>
                                        </asp:Repeater>
                                        <div class="pd-attr-label">Quantity</div>
                                        <div class="d-flex align-items-center gap-3">
                                            <div class="pd-qty-control">
                                                <button type="button" class="pd-qty-btn pd-qty-minus">&minus;</button>
                                                <asp:TextBox ID="txtNoGroupAttributeQuantity" runat="server" type="number" CssClass="pd-qty-display" min="0"></asp:TextBox>
                                                <button type="button" class="pd-qty-btn pd-qty-plus">+</button>
                                            </div>
                                        </div>
                                        <div class="pd-qty-info">
                                            <asp:Label ID="lblNoGroupAttributeQuantityAvailable" runat="server"></asp:Label>
                                            &nbsp;<a id="aNoGroupAttributeSuperceedingItem" runat="server" href='/ItemList.aspx?itemid=<%# Eval("ItemID")%>' visible="false" style="color:#C01F2F;font-size:12px;">Other Options</a>
                                        </div>
                                    </div>
                                </asp:Panel>

                                <%-- Grouped (Color + Size) --%>
                                <asp:Panel ID="pnlGroup" runat="server" Visible="false">

                                    <%-- Color swatches --%>
                                    <asp:Repeater ID="rptGroupByAttribute" runat="server" DataMember="AttributeValueID" OnItemDataBound="rptGroupByAttribute_ItemDataBound" OnItemCommand="rptGroupByAttribute_ItemCommand">
                                        <HeaderTemplate>
                                            <div class="pd-color-section">
                                                <div class="pd-attr-label">
                                                    Color:&nbsp;<span class="pd-attr-value" id="spanSelectedColor"></span>
                                                </div>
                                                <ul class="pd-swatches">
                                        </HeaderTemplate>
                                        <ItemTemplate>
                                            <li id="liSelected" runat="server">
                                                <asp:LinkButton ID="lbnAttributeValue" runat="server" CommandArgument='<%#Eval("AttributeValueID")%>' CommandName="Update" CssClass="pd-swatch-btn"></asp:LinkButton>
                                            </li>
                                        </ItemTemplate>
                                        <FooterTemplate>
                                                </ul>
                                            </div>
                                        </FooterTemplate>
                                    </asp:Repeater>

                                    <%-- Length/Width dropdown (when UseLengthAndWidth) --%>
                                    <asp:Panel ID="pnlLengthWidthAttribute" runat="server" Visible="false">
                                        <div class="pd-size-section pd-lw-section">
                                            <div class="pd-attr-label">Size:</div>
                                            <asp:DropDownList ID="ddlLengthWidthAttribute" runat="server" CssClass="form-control form-select-sm form-select" DataValueField="AttributeValueID" DataTextField="Value" OnSelectedIndexChanged="ddlLengthWidthAttribute_SelectedIndexChanged" Width="50%" AutoPostBack="true"></asp:DropDownList>
                                            <div class="pd-qty-info mt-1">
                                                <asp:Label ID="lblLengthWidthAttributeUnitPrice" runat="server" CssClass="pd-unit-price"></asp:Label>
                                                &nbsp;<asp:Label ID="lblLengthWidthAttributeQuantityAvailable" runat="server"></asp:Label>
                                                &nbsp;<asp:LinkButton ID="lbnLengthWidthAttributeSuperceedingItem" runat="server" visible="false" CausesValidation="false" style="color:#C01F2F;font-size:12px;">Other Options</asp:LinkButton>
                                            </div>
                                        </div>
                                        <div class="pd-simple-qty">
                                            <div class="pd-attr-label">Quantity</div>
                                            <div class="pd-qty-control">
                                                <button type="button" class="pd-qty-btn pd-qty-minus">&minus;</button>
                                                <asp:TextBox ID="txtLengthWidthAttributeQuantity" runat="server" type="number" CssClass="pd-qty-display" min="0"></asp:TextBox>
                                                <button type="button" class="pd-qty-btn pd-qty-plus">+</button>
                                            </div>
                                        </div>
                                    </asp:Panel>

                                    <%-- Single-group attribute (size buttons) --%>
                                    <asp:Panel ID="pnlGroupSingleAttribute" runat="server" Visible="false">
                                        <asp:Repeater ID="rptGroupSingleAttributeValue" runat="server" OnItemDataBound="rptGroupSingleAttributeValue_ItemDataBound">
                                            <HeaderTemplate>
                                                <div class="pd-size-section">
                                                    <div class="pd-attr-label">
                                                        <h6 id="hHeader" runat="server" style="margin:0;font-size:13px;font-weight:600;">Size</h6>
                                                        <asp:Button ID="btnSizeChartInline" runat="server" Text="Size Guide" CssClass="pd-size-guide-link" OnClick="btnSizeChart_Click" CausesValidation="false" style="background:none;border:none;padding:0;font-size:12px;color:#C01F2F;text-decoration:underline;cursor:pointer;" Visible="false" />
                                                    </div>
                                                    <div class="pd-sizes sizes-container">
                                            </HeaderTemplate>
                                            <ItemTemplate>
                                                <asp:HiddenField ID="hfAttributeValueID" runat="server" Value='<%# Eval("AttributeValueID")%>' />

                                                <%-- Grid display mode --%>
                                                <asp:Repeater ID="rptGroupAttribute" runat="server" OnItemCommand="rptGroupAttribute_ItemCommand" Visible="false">
                                                    <ItemTemplate>
                                                        <div class="size-btn-item">
                                                            <asp:HiddenField ID="hfListAttributeValueID" runat="server" Value='<%# Eval("AttributeValueID")%>' />
                                                            <button type="button" class="size-btn" onclick="return pdSelectSize(this);"><%# Eval("Value")%></button>
                                                            <asp:TextBox ID="txtGroupAttributeQuantity" runat="server" type="number" CssClass="size-qty-input" min="0"></asp:TextBox>
                                                            <asp:Label ID="lblGroupAttributeUnitPrice" runat="server" CssClass="size-price" style="display:none;"></asp:Label>
                                                            <asp:Label ID="lblGroupAttributeQuantityAvailable" runat="server" CssClass="size-stock" style="display:none;"></asp:Label>
                                                            <asp:LinkButton ID="lbnGroupAttributeSuperceedingItem" runat="server" visible="false" CausesValidation="false" style="display:none;">Other Options</asp:LinkButton>
                                                            <a id="aGroupAttributeSuperceedingItem" runat="server" visible="false" style="display:none;">Other Options</a>
                                                        </div>
                                                    </ItemTemplate>
                                                </asp:Repeater>

                                                <%-- List display mode (GridView styled as size buttons) --%>
                                                <asp:GridView ID="gvGroupAttribute" runat="server" AutoGenerateColumns="false" CssClass="size-grid-table" GridLines="None" CellSpacing="0" CellPadding="0" Visible="false" ShowHeader="false">
                                                    <Columns>
                                                        <asp:TemplateField>
                                                            <ItemTemplate>
                                                                <div class="size-btn-item">
                                                                    <asp:HiddenField ID="hfListAttributeValueID" runat="server" Value='<%# Eval("AttributeValueID")%>' />
                                                                    <button type="button" class="size-btn" onclick="return pdSelectSize(this);"><%# Eval("Value")%></button>
                                                                    <asp:TextBox ID="txtGroupAttributeQuantity" runat="server" type="number" CssClass="size-qty-input" min="0"></asp:TextBox>
                                                                    <asp:Label ID="lblGroupAttributeUnitPrice" runat="server" CssClass="size-price" style="display:none;"></asp:Label>
                                                                    <asp:Label ID="lblGroupAttributeQuantityAvailable" runat="server" CssClass="size-stock" style="display:none;"></asp:Label>
                                                                    <asp:LinkButton ID="lbnGroupAttributeSuperceedingItem" runat="server" visible="false" CausesValidation="false" style="display:none;">Other Options</asp:LinkButton>
                                                                    <a id="aGroupAttributeSuperceedingItem" runat="server" visible="false" style="display:none;">Other Options</a>
                                                                </div>
                                                            </ItemTemplate>
                                                        </asp:TemplateField>
                                                    </Columns>
                                                </asp:GridView>

                                            </ItemTemplate>
                                            <FooterTemplate>
                                                    </div><%-- close pd-sizes --%>
                                                </div><%-- close pd-size-section --%>

                                                <%-- Shared quantity control shown after size is selected --%>
                                                <div class="pd-qty-section shared-qty-section" style="display:none;">
                                                    <div class="pd-attr-label">Quantity</div>
                                                    <div class="d-flex align-items-center gap-3">
                                                        <div class="pd-qty-control">
                                                            <button type="button" class="pd-qty-btn shared-qty-minus">&minus;</button>
                                                            <input type="number" class="pd-qty-display shared-qty-display" value="1" min="1" />
                                                            <button type="button" class="pd-qty-btn shared-qty-plus">+</button>
                                                        </div>
                                                    </div>
                                                    <div class="pd-qty-info selected-size-stock"></div>
                                                </div>
                                            </FooterTemplate>
                                        </asp:Repeater>
                                    </asp:Panel>

                                </asp:Panel><%-- /pnlGroup --%>
                            </asp:Panel><%-- /pnlAttribute --%>
                        </ItemTemplate>
                    </asp:Repeater>

                    <%-- ══ Customization ══ --%>
                    <asp:PlaceHolder ID="phCustomization" runat="server" Visible="false">
                        <div class="pd-customization">
                            <h5>Customize This Item</h5>

                            <asp:PlaceHolder ID="phPersonalization" runat="server">
                                <asp:Repeater ID="rptItemPersonalization" runat="server" DataMember="ItemPersonalizationID" OnItemDataBound="rptItemPersonalization_ItemDataBound">
                                    <ItemTemplate>
                                        <div class="form-group mb-3">
                                            <label><asp:Label ID="lblLabel" runat="server" Text='<%# Eval("Name")%>' ForeColor="Black"></asp:Label></label>
                                            <asp:TextBox ID="txtValue" runat="server" Visible="true" CssClass="form-control"></asp:TextBox>
                                            <asp:Label ID="lblVerifyLabel" runat="server" Visible="false" ForeColor="Black" CssClass="form-label mt-2"></asp:Label>
                                            <asp:TextBox ID="txtVerifyValue" runat="server" Visible="false" CssClass="form-control"></asp:TextBox>
                                            <asp:DropDownList ID="ddlValueList" runat="server" DataTextField="Value" DataValueField="Value" Visible="false" CssClass="form-control form-select-sm form-select" Style="margin-bottom:5px;" OnSelectedIndexChanged="ddlValueList_SelectedIndexChanged" AutoPostBack="true"></asp:DropDownList>
                                            <asp:CheckBox ID="chkBlank" runat="server" Text="&nbsp;&nbsp;&nbsp;No Embroidery" CssClass="form-check-input mt-2" BorderStyle="None" AutoPostBack="true" OnCheckedChanged="chkBlank_CheckedChanged" />
                                            <asp:DropDownList ID="ddlTextOption" runat="server" DataTextField="Label" DataValueField="Value" Visible="false" CssClass="form-control form-select-sm form-select" Style="margin-bottom:5px;" OnSelectedIndexChanged="ddlTextOption_SelectedIndexChanged" AutoPostBack="true"></asp:DropDownList>
                                            <asp:TextBox ID="txtTextOption" runat="server" Visible="false" CssClass="form-control mt-1"></asp:TextBox>
                                            <asp:HiddenField ID="hfItemPersonalizationID" runat="server" Value='<%# Eval("ItemPersonalizationID")%>' />
                                        </div>
                                    </ItemTemplate>
                                </asp:Repeater>
                                <asp:Panel ID="pnlPersonalizationBasePrice" runat="server" Visible="false">
                                    <div class="form-group mb-3">
                                        <label><asp:Label ID="lblPersonalizationBasePrice" runat="server" ForeColor="Black" Text="Personalization Base Price"></asp:Label></label>
                                        <asp:TextBox ID="txtPersonalizationBasePrice" runat="server" CssClass="form-control" Enabled="false"></asp:TextBox>
                                    </div>
                                </asp:Panel>
                            </asp:PlaceHolder>

                            <asp:PlaceHolder ID="phLogo" runat="server">
                                <div class="form-group mb-2">
                                    <label>Please select a logo:</label>
                                    <asp:HiddenField ID="hfSelectedLogo" runat="server" />
                                    <asp:Panel ID="pnlSelectableLogoImage" runat="server" CssClass="mt-2">
                                        <ul class="pd-logo-grid">
                                            <asp:Repeater ID="rptSelectableLogo" runat="server" DataMember="ItemSelectableLogoID" OnItemDataBound="rptSelectableLogo_ItemDataBound" OnItemCommand="rptSelectableLogo_ItemCommand">
                                                <ItemTemplate>
                                                    <li id="liSelected" runat="server">
                                                        <asp:LinkButton ID="lbnSelectableLogo" runat="server" CommandArgument='<%#Eval("ItemSelectableLogoID")%>' CommandName="Update">
                                                            <asp:Image ID="imgLogo" runat="server" />
                                                        </asp:LinkButton>
                                                    </li>
                                                </ItemTemplate>
                                            </asp:Repeater>
                                        </ul>
                                    </asp:Panel>
                                    <div class="mt-2">
                                        <asp:CheckBox ID="chkNoLogo" runat="server" Text="&nbsp;&nbsp;&nbsp;No Logo" CssClass="form-check-input" BorderStyle="None" AutoPostBack="true" OnCheckedChanged="chkNoLogo_CheckedChanged" />
                                    </div>
                                </div>
                                <asp:Panel ID="pnlSelectableLogoYear" runat="server" Visible="false">
                                    <div class="form-group mb-2">
                                        <label><asp:Label ID="lblSelectableLogoYear" runat="server" ForeColor="Black" Text="Year"></asp:Label></label>
                                        <asp:DropDownList ID="ddlSelectableLogoYear" runat="server" CssClass="form-control form-select-sm form-select" Style="max-width:160px;"></asp:DropDownList>
                                    </div>
                                </asp:Panel>
                                <asp:Panel ID="pnlSelectableBasePrice" runat="server" Visible="false">
                                    <div class="form-group mb-2">
                                        <label><asp:Label ID="lblSelectableBasePrice" runat="server" ForeColor="Black" Text="Logo Base Price"></asp:Label></label>
                                        <asp:TextBox ID="txtSelectableBasePrice" runat="server" CssClass="form-control" Enabled="false" style="max-width:160px;"></asp:TextBox>
                                    </div>
                                </asp:Panel>
                                <%-- Hidden file upload kept for code-behind compat --%>
                                <div style="display:none;">
                                    <asp:FileUpload ID="filLogo" runat="server" />
                                    <asp:Image ID="imgUploadedLogo" runat="server" Width="100px" Visible="false" />
                                </div>
                            </asp:PlaceHolder>
                        </div>
                    </asp:PlaceHolder>

                    <%-- ══ Action Buttons ══ --%>
                    <div class="pd-actions">
                        <asp:Button id="btnAddToCart" runat="server" Text="ADD TO CART" CssClass="btn-add-to-bag" OnClick="btnAddToCart_Click" CausesValidation="false" />
                        <asp:Button id="btnBuyNow" runat="server" Text="BUY NOW" CssClass="btn-buy-now" OnClick="btnBuyNow_Click" CausesValidation="false" Visible="false" />
                        <asp:Button id="btnAddMore" runat="server" Text="Add More" CssClass="btn-add-more" OnClick="btnAddMore_Click" CausesValidation="false" Visible="false" />
                    </div>

                    <%-- Size chart button (shown when item has chart) --%>
                    <asp:Button id="btnSizeChart" runat="server" Text="Size Chart" CssClass="btn-size-chart" OnClick="btnSizeChart_Click" CausesValidation="false" style="margin-bottom:20px;" />

                    <%-- ══ Accordion: Product Details + Shipping ══ --%>
                    <div class="pd-accordion">

                        <%-- Product Details entries from backend --%>
                        <asp:Repeater id="rptProductDetail" runat="server">
                            <ItemTemplate>
                                <div class="pd-accordion-item">
                                    <div class="pd-accordion-header" onclick="pdToggleAccordion(this)">
                                        <span><%# Eval("Attribute") %></span>
                                        <span class="pd-accordion-icon">&#8964;</span>
                                    </div>
                                    <div class="pd-accordion-body" style="display:none;">
                                        <ul><%# Eval("ItemDetailValuesInHTML") %></ul>
                                    </div>
                                </div>
                            </ItemTemplate>
                        </asp:Repeater>

                        <%-- Detailed description accordion --%>
                        <div class="pd-accordion-item" id="divDetailedDescriptionAccordion" runat="server">
                            <div class="pd-accordion-header" onclick="pdToggleAccordion(this)">
                                <span>Product Details</span>
                                <span class="pd-accordion-icon">&#8964;</span>
                            </div>
                            <div class="pd-accordion-body" style="display:none;">
                                <div id="divDetailedDescription"><asp:Literal ID="litDetailedDescription" runat="server"></asp:Literal></div>
                            </div>
                        </div>

                        <%-- Shipping accordion --%>
                        <div class="pd-accordion-item">
                            <div class="pd-accordion-header" onclick="pdToggleAccordion(this)">
                                <span>Shipping</span>
                                <span class="pd-accordion-icon">&#8963;</span>
                            </div>
                            <div class="pd-accordion-body">
                                <p>Shipping rates and delivery times are calculated at checkout. Expedited shipping is available but will not reduce production lead times for custom-decorated items.</p>
                            </div>
                        </div>

                    </div><%-- /pd-accordion --%>

                </div><%-- /pd-details-col --%>
            </div><%-- /row --%>

            <%-- ══ Related Items ══ --%>
            <asp:Panel ID="pnlRelatedItems" runat="server" CssClass="pd-related">
                <h2>RECOMMENDED FOR YOU</h2>
                <div class="pd-related-carousel">
                    <button type="button" class="related-arrow related-prev" onclick="pdRelatedNav(-1)" style="display:none;">&#8249;</button>
                    <div class="related-track">
                        <div class="row related-items-row justify-content-center" id="relatedItemsRow">
                            <asp:Repeater ID="rptRelatedItem" runat="server" OnItemCommand="rptRelatedItem_ItemCommand" OnItemDataBound="rptRelatedItem_ItemDataBound">
                                <ItemTemplate>
                                    <div class="col-6 col-md-2 mb-4 pd-related-item">
                                        <div class="img-wrapper">
                                            <a href="/ProductDetail.aspx?id=<%# Eval("Item.ItemID")%>&websitetabid=<%# mWebSiteTabID %>">
                                                <img src='<%# Eval("Item.DisplayImageURL")%>' class="img-fluid" alt="" />
                                            </a>
                                        </div>
                                        <h4><a href="/ProductDetail.aspx?id=<%# Eval("Item.ItemID")%>"><%# Eval("Item.StoreDisplayName")%></a></h4>
                                        <p class="pd-related-price"><%# Eval("Item.PriceRange") %></p>
                                        <div class="product-card-colors">
                                            <asp:Literal ID="litColorSwatches" runat="server"></asp:Literal>
                                        </div>
                                    </div>
                                </ItemTemplate>
                            </asp:Repeater>
                        </div>
                    </div>
                    <button type="button" class="related-arrow related-next" onclick="pdRelatedNav(1)" style="display:none;">&#8250;</button>
                </div>
            </asp:Panel>


        </div><%-- /container --%>
    </section>

    <asp:HiddenField ID="hfSelectedGroupByAttributeValueID" runat="server" />

    <script>
        // ── Accordion ──────────────────────────────────────────────
        function pdToggleAccordion(header) {
            var body = header.nextElementSibling;
            var icon = header.querySelector('.pd-accordion-icon');
            var open = body.style.display !== 'none';
            body.style.display = open ? 'none' : 'block';
            icon.innerHTML = open ? '&#8964;' : '&#8963;';
        }

        // Open Shipping accordion by default (matches reference image)
        document.addEventListener('DOMContentLoaded', function () {
            var shippingHeaders = document.querySelectorAll('.pd-accordion-header');
            shippingHeaders.forEach(function (h) {
                if (h.querySelector('span:first-child').textContent.trim() === 'Shipping') {
                    pdToggleAccordion(h);
                }
            });

            // ── Qty +/- for simple (no-attribute) items ────────────
            document.querySelectorAll('.pd-qty-minus').forEach(function (btn) {
                btn.addEventListener('click', function () {
                    var inp = this.nextElementSibling;
                    var v = parseInt(inp.value) || 0;
                    if (v > 0) inp.value = v - 1;
                });
            });
            document.querySelectorAll('.pd-qty-plus').forEach(function (btn) {
                btn.addEventListener('click', function () {
                    var inp = this.previousElementSibling;
                    var v = parseInt(inp.value) || 0;
                    inp.value = v + 1;
                });
            });

            // ── Color swatch click → re-check buttons ──────────────
            document.querySelectorAll('.pd-swatch-btn').forEach(function (btn) {
                btn.addEventListener('click', function () {
                    document.querySelectorAll('.pd-swatch-btn').forEach(function (b) { b.classList.remove('pd-swatch-selected'); });
                    this.classList.add('pd-swatch-selected');
                    pdCheckEnableButtons();
                });
            });

            // ── Mark out-of-stock size buttons ─────────────────────
            document.querySelectorAll('.size-btn-item').forEach(function (item) {
                var input = item.querySelector('.size-qty-input');
                var btn = item.querySelector('.size-btn');
                if (input && btn && input.disabled) {
                    btn.classList.add('out-of-stock');
                    btn.disabled = true;
                }
            });

            // ── Shared qty +/- ─────────────────────────────────────
            document.querySelectorAll('.shared-qty-minus').forEach(function (btn) {
                btn.addEventListener('click', function () {
                    var display = this.parentElement.querySelector('.shared-qty-display');
                    var v = parseInt(display.value) || 1;
                    if (v > 1) {
                        display.value = v - 1;
                        pdSyncSelectedQty(btn, v - 1);
                    }
                });
            });
            document.querySelectorAll('.shared-qty-plus').forEach(function (btn) {
                btn.addEventListener('click', function () {
                    var display = this.parentElement.querySelector('.shared-qty-display');
                    var v = parseInt(display.value) || 1;
                    display.value = v + 1;
                    pdSyncSelectedQty(btn, v + 1);
                });
            });
            document.querySelectorAll('.shared-qty-display').forEach(function (inp) {
                inp.addEventListener('change', function () {
                    pdSyncSelectedQty(inp, parseInt(inp.value) || 1);
                });
            });

            // ── Thumbnail strip (only if multiple images) ───────────
            var mainImg = document.querySelector('.pd-main-img');
            var strip = document.getElementById('divThumbnails');
            var pdPrev = document.querySelector('.pd-img-prev');
            var pdNext = document.querySelector('.pd-img-next');
            if (mainImg && mainImg.src && strip) {
                // Currently only one real image — show single thumb, hide nav
                var thumb = document.createElement('div');
                thumb.className = 'pd-thumb active';
                var tImg = document.createElement('img');
                tImg.src = mainImg.src;
                tImg.alt = '';
                thumb.appendChild(tImg);
                strip.appendChild(thumb);
                // Hide nav arrows since there's only one image
                if (pdPrev) pdPrev.style.display = 'none';
                if (pdNext) pdNext.style.display = 'none';
            }

            // ── Button enable/disable based on required selections ──
            pdCheckEnableButtons();

            // ── Category header: extract last segment as title, format breadcrumb ──
            var bcLit = document.querySelector('#divCategoryBreadcrumb');
            if (bcLit) {
                var raw = bcLit.innerText || bcLit.textContent || '';
                raw = raw.replace(/ &gt; /g, ' > ');
                var segments = raw.split('>').map(function(s){ return s.trim(); }).filter(function(s){ return s.length > 0; });
                if (segments.length > 0) {
                    var titleEl = document.getElementById('divCategoryTitle');
                    if (titleEl) titleEl.textContent = segments[segments.length - 1].toUpperCase();
                }
                bcLit.innerHTML = bcLit.innerHTML.replace(/ &gt; /g, ' / ').replace(/ > /g, ' / ');
            }

            // ── Related items carousel ──────────────────────────────
            var relatedOffset = 0;
            var relatedPageSize = 5;
            function pdRelatedRender() {
                var items = document.querySelectorAll('#relatedItemsRow .pd-related-item');
                if (!items.length) return;
                items.forEach(function (el, i) {
                    el.style.display = (i >= relatedOffset && i < relatedOffset + relatedPageSize) ? '' : 'none';
                });
                var prevBtn = document.querySelector('.related-prev');
                var nextBtn = document.querySelector('.related-next');
                if (prevBtn) prevBtn.style.display = relatedOffset > 0 ? '' : 'none';
                if (nextBtn) nextBtn.style.display = (relatedOffset + relatedPageSize < items.length) ? '' : 'none';
            }
            window.pdRelatedNav = function(dir) {
                var items = document.querySelectorAll('#relatedItemsRow .pd-related-item');
                relatedOffset = Math.max(0, Math.min(relatedOffset + dir * relatedPageSize, items.length - relatedPageSize));
                pdRelatedRender();
            };
            pdRelatedRender();

            // ── Google Analytics data layer ─────────────────────────
            window.dataLayer = window.dataLayer || [];
            var itemNameEl = document.getElementById('ItemName');
            var itemNumberEl = document.getElementById('ItemNumber');
            if (itemNameEl && itemNumberEl) {
                var rawName = itemNameEl.innerText.trim();
                var itemName = rawName.split(' ').map(function (w) { return w.charAt(0).toUpperCase() + w.slice(1).toLowerCase(); }).join(' ');
                dataLayer.push({ event: 'view_item', ecommerce: { items: { item_id: itemNumberEl.innerText, item_name: itemName } } });

                var addBtn = document.getElementById('cphBody_btnAddToCart');
                if (addBtn) {
                    addBtn.addEventListener('click', function () {
                        var totalQty = 0;
                        document.querySelectorAll('.pd-qty-display, .size-qty-input').forEach(function (inp) { totalQty += parseInt(inp.value) || 0; });
                        dataLayer.push({ event: 'add_to_cart', ecommerce: { item_id: itemNumberEl.innerText, item_name: itemName, quantity: totalQty } });
                    });
                }
            }
        });

        // ── Enable/disable Add to Cart ─────────────────────────────
        function pdCheckEnableButtons() {
            var colorSection = document.querySelector('.pd-color-section');
            var sizesContainer = document.querySelector('.sizes-container');
            var hasAttributes = colorSection || sizesContainer;

            var colorOk = !colorSection || colorSection.querySelector('.pd-swatch-selected') !== null;
            var sizeOk = !sizesContainer || sizesContainer.querySelector('.size-btn.active') !== null;
            var ok = !hasAttributes || (colorOk && sizeOk);

            var addBtn = document.getElementById('<%=btnAddToCart.ClientID%>');
            if (addBtn) addBtn.disabled = !ok;
        }

        // ── Size button selection ───────────────────────────────────
        function pdSelectSize(btn) {
            if (btn.disabled || btn.classList.contains('out-of-stock')) return false;

            var container = btn.closest('.sizes-container');
            if (!container) return false;

            // Deselect all sizes in this container, clear their qty inputs
            container.querySelectorAll('.size-btn').forEach(function (b) { b.classList.remove('active'); });
            container.querySelectorAll('.size-qty-input').forEach(function (inp) { inp.value = ''; });

            btn.classList.add('active');

            // Sync hidden qty input with shared display
            var qtySection = container.closest('.pd-size-section, .pd-group-attr-section') || container.parentElement;
            // Walk up to find the shared qty section sibling
            var parent = container.parentElement;
            while (parent) {
                var sharedSection = parent.querySelector('.shared-qty-section');
                if (sharedSection) {
                    sharedSection.style.display = 'block';
                    var display = sharedSection.querySelector('.shared-qty-display');
                    var selInput = btn.closest('.size-btn-item').querySelector('.size-qty-input');
                    if (display && selInput) {
                        selInput.value = display.value || 1;
                    }
                    // Update main price display and stock
                    var priceEl = btn.closest('.size-btn-item').querySelector('.size-price');
                    var stockEl = btn.closest('.size-btn-item').querySelector('.size-stock');
                    var mainPrice = document.querySelector('.pd-price');
                    if (mainPrice && priceEl && priceEl.textContent.trim()) {
                        mainPrice.textContent = priceEl.textContent.trim();
                    }
                    var stockTarget = sharedSection.querySelector('.selected-size-stock');
                    if (stockTarget && stockEl) stockTarget.textContent = stockEl.textContent;
                    break;
                }
                parent = parent.parentElement;
                if (parent && parent.classList && parent.classList.contains('pd-details-col')) break;
            }

            pdCheckEnableButtons();
            return false;
        }

        function pdSyncSelectedQty(triggerEl, qty) {
            var section = triggerEl.closest('.shared-qty-section');
            if (!section) return;
            // Walk up to find sizes container
            var parent = section.parentElement;
            while (parent) {
                var container = parent.querySelector('.sizes-container');
                if (container) {
                    var activeInput = container.querySelector('.size-btn.active');
                    if (activeInput) {
                        var inp = activeInput.closest('.size-btn-item').querySelector('.size-qty-input');
                        if (inp) inp.value = qty;
                    }
                    break;
                }
                parent = parent.parentElement;
                if (!parent || parent.tagName === 'BODY') break;
            }
        }

        // ── Thumbnail navigation ────────────────────────────────────
        function pdThumbNav(dir) {
            var thumbs = document.querySelectorAll('.pd-thumb');
            if (!thumbs.length) return;
            var active = -1;
            thumbs.forEach(function (t, i) { if (t.classList.contains('active')) active = i; });
            var next = (active + dir + thumbs.length) % thumbs.length;
            thumbs.forEach(function (t) { t.classList.remove('active'); });
            thumbs[next].classList.add('active');
        }
    </script>

</asp:Content>

<asp:Content ID="Content4" ContentPlaceHolderID="cphFooter" runat="server">
    <uc1:SuperceedingItem ID="ucSuperceedingItem" runat="server" />
    <uc2:ImageModal ID="ucImageModal" runat="server" />
    <uc4:AccountSearchModal ID="ucAccountSearchModal" runat="server" />
</asp:Content>
