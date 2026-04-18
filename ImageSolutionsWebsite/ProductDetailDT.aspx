<%@ Page Title="" Language="C#" MasterPageFile="~/MasterPage.Master" AutoEventWireup="true" CodeBehind="ProductDetailDT.aspx.cs" Inherits="ImageSolutionsWebsite.ProductDetailDT" %>
<%@ Register src="Control/SuperceedingItem.ascx" tagname="SuperceedingItem" tagprefix="uc1" %>
<%@ Register src="Control/ImageModal.ascx" tagname="ImageModal" tagprefix="uc2"  %>
<%@ Register Src="Control/LeftPanelNavigation.ascx" tagname="LeftPanelNavigation" tagprefix="uc3" %>
<%@ Register src="Control/AccountSearchModal.ascx" tagname="AccountSearchModal" tagprefix="uc4"  %>

<%@ Register Src="~/Control/Pager.ascx" TagPrefix="uc1" TagName="Pager" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <style>
        div#divDetailedDescription ul{
            font-family: Lato,sans-serif;
            font-size:12px;
            display: block;
        }

        /* add style only to the 'li' elements inside the id="one" div. this means 'li' inside the 'ul' inside the 'div' which its id="one" */
        div#divDetailedDescription ul li {
            font-family: Lato,sans-serif;
            font-size:12px;
            display: list-item;
        }

        div#divDetailedDescription h2{
            font-family: Lato,sans-serif;
            font-size:12px;
            text-transform: unset;
            font-weight:unset;
            letter-spacing:unset;
            line-height:1.5em;
        }

	    div#divDetailedDescription h3{
            font-family: Lato,sans-serif;
            font-size:12px;
            text-transform: unset;
            font-weight:unset;
            letter-spacing:unset;
            line-height:1.5em;
        }

        div#divDetailedDescription h4{
            font-family: Lato,sans-serif;
            font-size:12px;
            text-transform: unset;
            font-weight:unset;
            letter-spacing:unset;
            line-height:1.5em;
        }

        div#divDetailedDescription p{
            font-family: Lato,sans-serif;
            font-size:12px;
	        margin-bottom: 10px;
        }

        div#divDetailedDescription span{
            font-family: Lato,sans-serif;
            font-size:12px;
        }

        div#divDetailedDescription body{
            font-family: Lato,sans-serif;
            font-size:12px;
        }
        
        .ti-info-alt:before {
           content: "\0043\0024";
        }
    </style>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="cphHeader" runat="server">
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="cphBody" runat="server">
    <section class="section-b-space ratio_asos">
        <div class="collection-wrapper">
            <div class="container">
                <div class="row">
                    <div class="col-sm-3 collection-filter" id="divLeftPanel" runat="server" visible="false">
                        <uc3:LeftPanelNavigation runat="server" ID="ucLeftPanelNavigation" />                                                   
                    </div>
                     <div class="collection-content col">
                        <div class="page-main-content">
                            <asp:Panel ID="pnlCategoryBreadCrumb" runat="server">
                                <div class="collection-product-wrapper" style="font-size:larger">
                                    <asp:Literal ID="litCategoryBreadCrumb" runat="server"></asp:Literal>
                                </div>
                            </asp:Panel>

                            <div class="collection-wrapper">
                                <div class="container">

                                    <div class="row">
                                        <div class="col-lg-1 col-sm-2 col-xs-12" style="display:none">
                                            <div class="row">
                                                <div class="col-12 p-0">
                                                    <div class="slider-right-nav">
                                                        <div><asp:Image ID="imgItem" runat="server" alt="" class="img-fluid blur-up lazyload"/></div>
                                                    <div><asp:Image ID="imgItem2_old" runat="server" alt="" Visible="false" class="img-fluid blur-up lazyload image_zoom_cls-0" Width="100%"/></div>
                                                
                                                    </div>
                                                </div>
                                            </div>
                                        </div>
                                        <div class="col-lg-5 col-sm-10 col-xs-12 order-up">
                                            <div class="product-slick">
                                                <div><img ID="imgItem1" runat="server" src="" alt=""
                                                        class="img-fluid blur-up lazyload image_zoom_cls-0"></div>
                                                <div id="divImgItem2" runat="server" visible="false"><img ID="imgItem2" runat="server" src="" alt=""
                                                        class="img-fluid blur-up lazyload image_zoom_cls-1"></div>
                                                <div id="divImgItem3" runat="server" visible="false"><img ID="imgItem3" runat="server" src="" alt=""
                                                        class="img-fluid blur-up lazyload image_zoom_cls-2"></div>
                                                <%--<div><img src="/assets/images/sample/MM2000_nightnavy_model_front.jpg" alt=""
                                                        class="img-fluid blur-up lazyload image_zoom_cls-1"></div>--%>
                                            </div>
                                            <div class="row">
                                                <div class="col-12 p-0">
                                                    <div class="slider-nav">
                                                        <div><img id="imgItem1_swatch" runat="server" src="" alt=""
                                                                class="img-fluid blur-up lazyload image_zoom_cls-0"></div>
                                                        <div id="divImgItem2_swatch" runat="server" visible="false"><img id="imgItem2_swatch" runat="server" src="" alt=""
                                                                class="img-fluid blur-up lazyload image_zoom_cls-1"></div>
                                                        <div id="divImgItem3_swatch" runat="server" visible="false"><img id="imgItem3_swatch" runat="server" src="" alt=""
                                                                class="img-fluid blur-up lazyload image_zoom_cls-2"></div>
                                                        <%--<div><img src="/assets/images/sample/MM2000_nightnavy_model_front.jpg" alt=""
                                                                class="img-fluid blur-up lazyload"></div>--%>
                                                    </div>
                                                </div>
                                            </div>
                                        </div>
                                        <div class="col-lg-6 rtl-text">
                                            <div class="product-right">
                                                <div class="product-count" style="display:none;">
                                                    <ul>
                                                        <li>
                                                            <img src="../assets/images/fire.gif" class="img-fluid" alt="image">
                                                            <span class="p-counter">37</span>
                                                            <span class="lang">orders in last 24 hours</span>
                                                        </li>
                                                        <li>
                                                            <img src="../assets/images/person.gif" class="img-fluid user_img" alt="image">
                                                            <span class="p-counter">44</span>
                                                            <span class="lang">active view this</span>
                                                        </li>
                                                    </ul>
                                                </div>
                                                <h2><asp:Label ID="lblHeader" runat="server" Text=""></asp:Label></h2>
                                                <h4>Item#: <span id="ItemNumber" style="color:black;"><asp:Literal ID="litItemNumber" runat="server" Text=""></asp:Literal></span></h4>
                                                <h4><asp:Literal ID="litSalesDescription" runat="server"></asp:Literal></h4>

                                                <asp:Panel ID="pnlSingleUnit" runat="server">
                                                    For a single unit purchase, &nbsp;&nbsp;<asp:HyperLink ID="btnSingleUnit" runat="server">Click Here</asp:HyperLink>
                                                </asp:Panel>

                                                <div class="rating-section" style="display:none;">
                                                    <div class="rating"><i class="fa fa-star"></i> <i class="fa fa-star"></i> <i
                                                            class="fa fa-star"></i> <i class="fa fa-star"></i> <i class="fa fa-star"></i>
                                                    </div>
                                                    <h6>120 ratings</h6>
                                                </div>
                                                <div class="label-section" style="display:none;">
                                                    <span class="badge badge-grey-color">#1 Best seller</span>
                                                    <span class="label-text">in fashion</span>
                                                </div>
                                                <h3 class="price-detail" style="display:none;">$<asp:Literal ID="litBasePrice" runat="server"></asp:Literal> </h3>
                                                <ul class="color-variant" style="display:none;">
                                                    <li class="bg-light0 active"></li>
                                                    <li class="bg-light1"></li>
                                                    <li class="bg-light2"></li>
                                                </ul>

                                                
                                                    <asp:PlaceHolder ID="phEmployee" runat="server" Visible="false">

                                                        <asp:PlaceHolder ID="phEmployeeAccount" runat="server" Visible="false">
                                                            <div class="col-md-12">
                                                                <br />
                                                                <h6 class="product-title">Store</h6>                                        
                                                            </div>
                                                            <div class="row">
                                                                <div class="col-md-9">
                                                                    <asp:DropDownList ID="ddlAccount" runat="server" Width="100%" DataValueField="AccountID" DataTextField="AccountName" CssClass="form-control form-select-sm form-select" AutoPostBack="true" OnSelectedIndexChanged="ddlAccount_SelectedIndexChanged"></asp:DropDownList>
                                                                    <asp:TextBox ID="txtAccount" runat="server" CssClass="form-control" Enabled="false"></asp:TextBox>
                                                                    <asp:HiddenField ID="hfAccountID" runat="server" />
                                                                </div>
                                                                <div class="col-md-3">
                                                                    <asp:LinkButton ID="btnAccountSearch" runat="server" OnClick="btnAccountSearch_Click" CssClass="btn btn-solid" >Change<%--<i class="ti-search" style="font-size:large"></i>--%></asp:LinkButton>
    <%--                                                                <asp:LinkButton ID="btnAccountRemove" runat="server" OnClick="btnAccountRemove_Click" Enabled="false"><i class="ti-trash" style="font-size:large"></i></asp:LinkButton>--%>
                                                                </div>
                                                            </div>
                                                        </asp:PlaceHolder>

                                                        <div class="col-md-12">
                                                            <br />
                                                            <h6 class="product-title">Employee</h6>                                        
                                                        </div>
                                                        <div class="col-md-12">
                                                            <asp:DropDownList ID="ddlUserInfo" runat="server" Width="100%" DataValueField="UserInfoID" DataTextField="FullName" CssClass="form-control form-select-sm form-select" AutoPostBack="true" OnSelectedIndexChanged="ddlUserInfo_SelectedIndexChanged"></asp:DropDownList>
                                                        </div>
                                                        <br />
                                                    </asp:PlaceHolder>

                                                <div id="selectSize" class="addeffect-section border-product">
                                                    <h6 class="product-title size-text" style="display:none;">select size <span></span></h6>
                                                    <div class="modal fade" id="sizemodal" tabindex="-1" role="dialog"
                                                        aria-labelledby="exampleModalLabel" aria-hidden="true">
                                                        <div class="modal-dialog modal-dialog-centered" role="document">
                                                            <div class="modal-content">
                                                                <div class="modal-header">
                                                                    <h5 class="modal-title" id="exampleModalLabel"></h5>
                                                                    <button type="button" class="btn-close" data-bs-dismiss="modal"
                                                                        aria-label="Close"><span aria-hidden="true">&times;</span></button>
                                                                </div>
                                                                <div class="modal-body" style="display:none;"><img src="../assets/images/size-chart.jpg" alt=""
                                                                        class="img-fluid blur-up lazyload"></div>
                                                            </div>
                                                        </div>
                                                    </div>

                                                    <asp:Repeater ID="rptItems" runat="server" DataMember="ItemID" OnItemDataBound="rptItems_ItemDataBound">
                                                        <ItemTemplate>
                                                            <asp:HiddenField ID="hfItemID" runat="server" Value='<%# Eval("Item.ItemID")%>'/>
                                                            <asp:Panel ID="pnlNoAttribute" runat="server">
                                                                <table>
                                                                    <tr>
                                                                        <td style="min-width:100px;">Quantity</td>
                                                                        <td style="text-align:center;">
                                                                            <div class="qty-box">
                                                                                <div class="input-group">
                                                                                    <asp:TextBox ID="txtQuantity" runat="server" Width="80px" type="number" CssClass="form-control input-number" min="0"></asp:TextBox>
                                                                                </div>
                                                                            </div>
                                                                        </td>
                                                                    </tr>
                                                                    <tr>
                                                                        <td></td>
                                                                        <td style="text-align:center;"><asp:Label ID="lblUnitPrice" runat="server"></asp:Label></td>
                                                                    </tr>
                                                                    <tr>
                                                                        <td></td>
                                                                        <td style="text-align:center; font-size:11px;"><u><asp:Label ID="lblQuantityAvailable" runat="server"></asp:Label></u><br /><a id="aSuperceedingItem" runat="server" href='/ItemList.aspx?itemid=<%# Eval("ItemID")%>' visible="false">Other Options</a></td>
                                                                    </tr>
                                                                </table>
                                                                <p></p>
                                                            </asp:Panel>
                                                            <asp:Panel ID="pnlAttribute" runat="server" Visible="false">
                                                                <asp:Panel ID="pnlNoGroup" runat="server" Visible="false">
                                                                    <table>
                                                                        <asp:Repeater ID="rptNoGroupAttributes" runat="server" DataMember="AttributeID" OnItemDataBound="rptAttributes_ItemDataBound">
                                                                            <ItemTemplate>
                                                                                <tr>
                                                                                    <td style="min-width:100px;"><%# Eval("AttributeName")%></td>
                                                                                    <td><asp:DropDownList ID="ddlAttributeValue" runat="server" OnSelectedIndexChanged="ddlAttributeValue_SelectedIndexChanged" AutoPostBack="true"></asp:DropDownList><asp:HiddenField ID="hfAttributeID" runat="server" Value='<%# Eval("AttributeID")%>'/></td>
                                                                                </tr>
                                                                            </ItemTemplate>
                                                                        </asp:Repeater>
                                                                        <tr>
                                                                            <td>Quantity</td>
                                                                            <td style="text-align:center;">
                                                                                <div class="qty-box">
                                                                                    <div class="input-group">
                                                                                        <asp:TextBox ID="txtNoGroupAttributeQuantity" runat="server" Width="80px" type="number" CssClass="form-control input-number" min="0"></asp:TextBox>
                                                                                    </div>
                                                                                </div>
                                                                            </td>
                                                                        </tr>
                                                                        <tr>
                                                                            <td></td>
                                                                            <td style="text-align:center; font-size:10px;"><asp:Label ID="lblNoGroupAttributeUnitPrice" runat="server"></asp:Label></td>
                                                                        </tr>
                                                                        <tr>
                                                                            <td></td>
                                                                            <td style="text-align:center; font-size:10px;"><u><asp:Label ID="lblNoGroupAttributeQuantityAvailable" runat="server"></asp:Label></u><br /><a id="aNoGroupAttributeSuperceedingItem" runat="server" href='/ItemList.aspx?itemid=<%# Eval("ItemID")%>' visible="false">Other Options</a></td>
                                                                        </tr>
                                                                    </table>
                                                                </asp:Panel>                                                                        
                                                                <asp:Panel ID="pnlGroup" runat="server" Visible="false">
                                                                    <asp:Repeater ID="rptGroupByAttribute" runat="server" DataMember="AttributeValueID" OnItemDataBound="rptGroupByAttribute_ItemDataBound" OnItemCommand="rptGroupByAttribute_ItemCommand">
                                                                        <HeaderTemplate>
                                                                            <h6 class="product-title">select color</h6>
                                                                            <div class="size-box">
                                                                                <ul>
                                                                        </HeaderTemplate>
                                                                        <ItemTemplate>
                                                                            <li id="liSelected" runat="server"><asp:LinkButton ID="lbnAttributeValue" runat="server" CommandArgument='<%#Eval("AttributeValueID")%>' CommandName="Update"></asp:LinkButton></li>
                                                                            <%--<li><a href="javascript:void(0)"> <%#Eval("Abbreviation").ToString()[0]%></a></li>--%>
                                                                        </ItemTemplate>
                                                                        <FooterTemplate>
                                                                                </ul>
                                                                            </div>
                                                                            <br />
                                                                        </FooterTemplate>
                                                                    </asp:Repeater>

                                                                    <asp:Panel ID="pnlLengthWidthAttribute" runat="server" Visible="false">
                                                                        <div class="form-row row">
                                                                            <div class="col-12" style="text-align:left;">
                                                                                <label>Size:</label>
                                                                                <asp:DropDownList ID="ddlLengthWidthAttribute" runat="server" CssClass="form-control form-select-sm form-select" DataValueField="AttributeValueID" DataTextField="Value" OnSelectedIndexChanged="ddlLengthWidthAttribute_SelectedIndexChanged" Width="50%" AutoPostBack="true"></asp:DropDownList>    
                                                                                &nbsp;&nbsp;<asp:Label ID="lblLengthWidthAttributeUnitPrice" runat="server"></asp:Label><br />
                                                                                &nbsp;&nbsp;<u style="font-size:10px;"><asp:Label ID="lblLengthWidthAttributeQuantityAvailable" runat="server"></asp:Label></u><br />
                                                                                &nbsp;&nbsp;<asp:LinkButton ID="lbnLengthWidthAttributeSuperceedingItem" runat="server" visible="false" CausesValidation="false">Other Options</asp:LinkButton>
                                                                                <br />
                                                                            </div>
                                                                            <div class="col-12" style="text-align:left;">
                                                                                <label>Quantity:</label>
                                                                                <asp:TextBox ID="txtLengthWidthAttributeQuantity" runat="server" Width="80px" type="number" CssClass="form-control input-number" min="0"></asp:TextBox>
                                                                                <br />

                                                                            </div>
                                                                        </div>
                                                                    </asp:Panel>

                                                                    <asp:Panel ID="pnlGroupSingleAttribute" runat="server" Visible="false">
                                                                        <%--<h6 class="product-title">select size</h6>--%>

                                                                        <asp:Repeater ID="rptGroupSingleAttributeValue" runat="server" OnItemDataBound="rptGroupSingleAttributeValue_ItemDataBound">
                                                                            <HeaderTemplate>
                                                                                <h6 class="product-title" id="hHeader" runat="server">select size</h6>
                                                                            </HeaderTemplate>
                                                                            <ItemTemplate>
                                                                                <div class="row">
                                                                                    <%--<div class="col-2" style="margin-top:100px; vertical-align:top;">
                                                                                        <%# Eval("Value")%>
                                                                                    </div>--%>
                                                                                    <div class="col-12" style="text-align:center;">
                                                                                        <div class="row">
                                                                                            <asp:HiddenField ID="hfAttributeValueID" runat="server" Value='<%# Eval("AttributeValueID")%>'/>
                                                                                            <asp:GridView ID="gvGroupAttribute" runat="server" AutoGenerateColumns="false" CssClass="table cart-table order-table" HeaderStyle-CssClass="table-head" GridLines="None"  Width="100%" CellSpacing="0" CellPadding="0" Visible="false" OnRowCommand="gvGroupAttribute_RowCommand">
                                                                                                <Columns>
                                                                                                    <%--<asp:TemplateField HeaderText="Color">
                                                                                                        <ItemTemplate>
                                                                                                            <%# SelectedGroupAttributeValue.Value %>
                                                                                                        </ItemTemplate>
                                                                                                    </asp:TemplateField>--%>
                                                                                                    <asp:TemplateField HeaderText="Size">
                                                                                                        <ItemTemplate>
                                                                                                            <%# Eval("Value")%>
                                                                                                            <asp:HiddenField ID="hfListAttributeValueID" runat="server" Value='<%# Eval("AttributeValueID")%>'/>
                                                                                                        </ItemTemplate>
                                                                                                    </asp:TemplateField>
                                                                                                    <asp:TemplateField HeaderText="Stock">
                                                                                                        <ItemTemplate>
                                                                                                            <u style="font-size:10px;"><asp:Label ID="lblGroupAttributeQuantityAvailable" runat="server"></asp:Label></u><br /><asp:LinkButton ID="lbnGroupAttributeSuperceedingItem" runat="server" visible="false" CausesValidation="false">Other Options</asp:LinkButton><a id="aGroupAttributeSuperceedingItem" runat="server" visible="false">Other Options</a>
                                                                                                        </ItemTemplate>
                                                                                                    </asp:TemplateField>
                                                                                                    <asp:TemplateField HeaderText="Price">
                                                                                                        <ItemTemplate>
                                                                                                            <asp:Label ID="lblGroupAttributeUnitPrice" runat="server"></asp:Label>
                                                                                                            <asp:Literal ID="litDiscountPrice" runat="server"></asp:Literal>
                                                                                                        </ItemTemplate>
                                                                                                    </asp:TemplateField>
                                                                                                    <asp:TemplateField HeaderText="Quantity">
                                                                                                        <ItemTemplate>
                                                                                                            <div class="qty-box">
                                                                                                                <div class="input-group">
                                                                                                                    <asp:TextBox ID="txtGroupAttributeQuantity" runat="server" Width="80px" type="number" CssClass="form-control input-number" min="0"></asp:TextBox>
                                                                                                                </div>
                                                                                                            </div>
                                                                                                        </ItemTemplate>
                                                                                                    </asp:TemplateField>
                                                                                                </Columns>
                                                                                            </asp:GridView>
                                                                                            <asp:Repeater ID="rptGroupAttribute" runat="server" OnItemCommand="rptGroupAttribute_ItemCommand" Visible="false">
                                                                                                <ItemTemplate>
                                                                                                    <div class="col-2" style="text-align:center;">
                                                                                                        <%# Eval("Value")%>
                                                                                                        <asp:HiddenField ID="hfListAttributeValueID" runat="server" Value='<%# Eval("AttributeValueID")%>'/>
                                                                                                        <div class="qty-box">
                                                                                                            <div class="input-group">
                                                                                                                <asp:TextBox ID="txtGroupAttributeQuantity" runat="server" Width="80px" type="number" CssClass="form-control input-number" min="0"></asp:TextBox>
                                                                                                            </div>
                                                                                                        </div>
                                                                                                        <asp:Label ID="lblGroupAttributeUnitPrice" runat="server"></asp:Label>
                                                                                                        <asp:Literal ID="litDiscountPrice" runat="server"></asp:Literal><br />
                                                                                                        <u style="font-size:10px;"><asp:Label ID="lblGroupAttributeQuantityAvailable" runat="server"></asp:Label></u><br /><asp:LinkButton ID="lbnGroupAttributeSuperceedingItem" runat="server" visible="false" CausesValidation="false">Other Options</asp:LinkButton><a id="aGroupAttributeSuperceedingItem" runat="server" visible="false">Other Options</a>
                                                                                                        <br /><br />
                                                                                                    </div>
                                                                                                </ItemTemplate>
                                                                                            </asp:Repeater>
                                                                                        </div>
                                                                                    </div>
                                                                                </div>
                                                                            </ItemTemplate>
                                                                        </asp:Repeater>              
                                                                    </asp:Panel>
                                                                </asp:Panel>                                                                            
                                                            </asp:Panel> 
                                                        </ItemTemplate>
                                                    </asp:Repeater>

                                                    <%--<div class="size-box" style="display:none;">
                                                        <ul>
                                                            <li><a href="javascript:void(0)">s</a></li>
                                                            <li><a href="javascript:void(0)">m</a></li>
                                                            <li><a href="javascript:void(0)">l</a></li>
                                                            <li><a href="javascript:void(0)">xl</a></li>
                                                        </ul>
                                                    </div>--%>
                                                    <h6 class="product-title" style="display:none;">quantity</h6>
                                                    <div class="qty-box" style="display:none;">
                                                        <div class="input-group"><span class="input-group-prepend"><button type="button"
                                                                    class="btn quantity-left-minus" data-type="minus" data-field=""><i
                                                                        class="ti-angle-left"></i></button> </span>
                                                            <input type="text" name="quantity" class="form-control input-number" value="1">
                                                            <span class="input-group-prepend"><button type="button"
                                                                    class="btn quantity-right-plus" data-type="plus" data-field=""><i
                                                                        class="ti-angle-right"></i></button></span>
                                                        </div>
                                                    </div>


                                                    <asp:PlaceHolder ID="phCustomization" runat="server" Visible="false">
                                                        <div class="col-md-12">
                                                            <br />
                                                            <h3>Customize This Item</h3>                                        
                                                        </div>

                                                        <asp:PlaceHolder ID="phPersonalization" runat="server">
                                                            <asp:Repeater ID="rptItemPersonalization" runat="server" DataMember="ItemPersonalizationID" OnItemDataBound="rptItemPersonalization_ItemDataBound">
                                                                <ItemTemplate>
                                                                    <div class="row" style="margin:10px;">
                                                                        <h4><asp:Label ID="lblLabel" runat="server" Text='<%# Eval("Name")%>' ForeColor="Black"></asp:Label>:</h4>
                                                                        <asp:TextBox ID="txtValue" runat="server" Visible="true" CssClass="form-control"></asp:TextBox>
                                                                        <h4><asp:Label ID="lblVerifyLabel" runat="server" Visible="false" ForeColor="Black"></asp:Label></h4>
                                                                        <asp:TextBox ID="txtVerifyValue" runat="server" Visible="false" CssClass="form-control"></asp:TextBox>
                                                                        <asp:DropDownList ID="ddlValueList" runat="server" DataTextField="Value" DataValueField="Value" Visible="false" CssClass="form-control form-select-sm form-select" Style="margin-bottom:5px;" OnSelectedIndexChanged="ddlValueList_SelectedIndexChanged" AutoPostBack="true"></asp:DropDownList>
                                                                        <asp:CheckBox ID="chkBlank" runat="server" Text="&nbsp;&nbsp;&nbsp;No Embroidery" CssClass="form-control" BorderStyle="None" AutoPostBack="true" OnCheckedChanged="chkBlank_CheckedChanged" />

                                                                        <asp:DropDownList ID="ddlTextOption" runat="server" DataTextField="Label" DataValueField="Value" Visible="false" CssClass="form-control form-select-sm form-select" Style="margin-bottom:5px;" OnSelectedIndexChanged="ddlTextOption_SelectedIndexChanged" AutoPostBack="true"></asp:DropDownList>
                                                                        <asp:TextBox ID="txtTextOption" runat="server" Visible="false" CssClass="form-control"></asp:TextBox>

                                                                        <asp:HiddenField ID="hfItemPersonalizationID" runat="server" Value='<%# Eval("ItemPersonalizationID")%>' />                                                                                                                                                                                        <br />
                                                                    </div>
                                                                </ItemTemplate>
                                                            </asp:Repeater>

                                                            <asp:Panel ID="pnlPersonalizationBasePrice" runat="server" Visible="false">
                                                                <div class="row" style="margin:10px;  margin-bottom:30px;">
                                                                    <h4><asp:Label ID="lblPersonalizationBasePrice" runat="server" ForeColor="Black" Text="Personalization Base Price"></asp:Label></h4>
                                                                    <asp:TextBox ID="txtPersonalizationBasePrice" runat="server" CssClass="form-control" Enabled="false"></asp:TextBox>
                                                                </div>
                                                            </asp:Panel>

                                                        </asp:PlaceHolder>

                                                        <asp:PlaceHolder ID="phLogo" runat="server">
                                                            <div class="col-md-12">
                                            
                    <%--                                            <asp:DropDownList ID="ddlLogo" runat="server" AutoPostBack="true" OnSelectedIndexChanged="ddlLogo_SelectedIndexChanged" Visible="false">
                                                                    <asp:ListItem></asp:ListItem>
                                                                    <asp:ListItem Value="Logo4">Circle Logo</asp:ListItem>
                                                                    <asp:ListItem Value="Logo5">CMYK Logo</asp:ListItem>
                                                                </asp:DropDownList>--%>
                                         

                                                                <div class="row" style="margin:10px;">
                                                                    <h4><label>Please select a logo: </label></h4>
                    <%--                                                <asp:DropDownList ID="ddlItemSelectableLogo" runat="server" AutoPostBack="true" DataValueField="ItemSelectableLogoID" DataTextField="Description" OnSelectedIndexChanged="ddlItemSelectableLogo_SelectedIndexChanged" CssClass="form-control"></asp:DropDownList>--%>
                                                                    <asp:HiddenField ID="hfSelectedLogo" runat="server" />
                                                                    <asp:Panel ID="pnlSelectableLogoImage" runat="server" BackColor="LightGray">
                                                                        <ul>
                                                                            <asp:Repeater ID="rptSelectableLogo" runat="server" DataMember="ItemSelectableLogoID" OnItemDataBound="rptSelectableLogo_ItemDataBound" OnItemCommand="rptSelectableLogo_ItemCommand">
                                                                                <ItemTemplate>
                                                                                    <li id="liSelected" runat="server" style="padding: 15px;"><asp:LinkButton ID="lbnSelectableLogo" runat="server" CommandArgument='<%#Eval("ItemSelectableLogoID")%>' CommandName="Update"><asp:Image ID="imgLogo" runat="server" /></asp:LinkButton></li>
                                                                                </ItemTemplate>
                                                                            </asp:Repeater>
                                                                        </ul>
                                                                    </asp:Panel>
                                                                    <br />
                                                                    <asp:CheckBox ID="chkNoLogo" runat="server" Text="&nbsp;&nbsp;&nbsp;No Logo" CssClass="form-control" BorderStyle="None" AutoPostBack="true" OnCheckedChanged="chkNoLogo_CheckedChanged" />
                                                                </div>

                                                                <asp:Panel ID="pnlSelectableLogoYear" runat="server" Visible="false">
                                                                    <div class="row" style="margin:10px;">
                                                                        <h4><asp:Label ID="lblSelectableLogoYear" runat="server" ForeColor="Black" Text="Year"></asp:Label></h4>
                                                                        <asp:DropDownList ID="ddlSelectableLogoYear" runat="server" CssClass="form-control form-select-sm form-select" Style="margin-bottom:5px;"></asp:DropDownList>
                                                                    </div>
                                                                </asp:Panel>
                                                                
                                                                <asp:Panel ID="pnlSelectableBasePrice" runat="server" Visible="false">
                                                                    <div class="row" style="margin:10px;">
                                                                        <h4><asp:Label ID="lblSelectableBasePrice" runat="server" ForeColor="Black" Text="Logo Base Price"></asp:Label></h4>
                                                                        <asp:TextBox ID="txtSelectableBasePrice" runat="server" CssClass="form-control" Enabled="false"></asp:TextBox>
                                                                    </div>
                                                                </asp:Panel>


                                                            </div>
                                                            <div class="col-md-12" style="display:none;">
                                                                <h3>Upload Your Own Logo</h3>
                                                                <label>Logo File</label>
                                                                <asp:FileUpload ID="filLogo" runat="server" />
                    <%--                                            <asp:Button ID="btnGenerate" runat="server" Text="Upload Logo" CssClass="btn btn-sm btn-solid" OnClick="btnGenerate_Click" />
                                                                <asp:RequiredFieldValidator ID="RequiredFieldValidator5" runat="server" ControlToValidate="filLogo" Display="Dynamic" ErrorMessage="*" ForeColor="Red"></asp:RequiredFieldValidator>
                                                                <br />--%>
                                                            </div>
                                                            <div class="col-md-12" style="display:none;">
                                                                <label>Logo File</label>
                                                                <asp:Image ID="imgUploadedLogo" runat="server" Width="100px" Visible="false" /><br />
                                                            </div>

                                                                <%--<div class="col-md-12" style="display:none;">
                                                                    <p></p>
                                                                    <asp:Image ID="imgResult" runat="server" Width="400px" />
                                                                    <p></p>
                                                                    <p></p>
                                                                </div>
                                                                <div class="col-md-2" style="display:none;">
                                                                    <label>Position</label>
                                                                </div>
                                                                <div class="col-md-10" style="display:none;">
                                                                    <label>Position of the logo: </label>
                                                                    <asp:DropDownList ID="ddlPosition" runat="server" AutoPostBack="true" CausesValidation="false" OnSelectedIndexChanged="ddlPosition_SelectedIndexChanged">
                                                                        <asp:ListItem Value="left-chest">Right Chest</asp:ListItem>
                                                                        <asp:ListItem Value="right-chest" Selected="True">Left Chest</asp:ListItem>
                                                                        <asp:ListItem Value="back">Back Center</asp:ListItem>
                                                                        <asp:ListItem Value="left-waist">Right Waist</asp:ListItem>
                                                                        <asp:ListItem Value="right-waist">Left Waist</asp:ListItem>
                                                                    </asp:DropDownList>
                                                                </div>
                                                                <div class="col-md-10" style="display:none;">
                                                                    <br />
                                                                    <label>Full Name: </label>
                                                                        <asp:TextBox ID="txtCustomDesignName" runat="server" Width="200px"></asp:TextBox>
                                                                    </div>
                                                                <div class="col-md-2" style="display:none;">
                                                                    <label>Ratio</label>
                                                                </div>
                                                                <div class="col-md-10" style="display:none;">
                                                                    <asp:Button ID="btnRatioMinus" runat="server" CausesValidation="false" Text="-" OnClick="btnRatio_Click" CommandArgument="Minus" />
                                                                        <asp:DropDownList ID="ddlRatio" runat="server" AutoPostBack="true" CausesValidation="false" OnSelectedIndexChanged="ddlRatio_SelectedIndexChanged">
                                                                        <asp:ListItem Value="0.1">10%</asp:ListItem>
                                                                        <asp:ListItem Value="0.2">20%</asp:ListItem>
                                                                        <asp:ListItem Value="0.3">30%</asp:ListItem>
                                                                        <asp:ListItem Value="0.4">40%</asp:ListItem>
                                                                        <asp:ListItem Value="0.5">50%</asp:ListItem>
                                                                        <asp:ListItem Value="0.6" Selected="True">60%</asp:ListItem>
                                                                        <asp:ListItem Value="0.7">70%</asp:ListItem>
                                                                        <asp:ListItem Value="0.8">80%</asp:ListItem>
                                                                        <asp:ListItem Value="0.9">90%</asp:ListItem>
                                                                        <asp:ListItem Value="1.0">100%</asp:ListItem>
                                                                        <asp:ListItem Value="1.1">110%</asp:ListItem>
                                                                        <asp:ListItem Value="1.2">120%</asp:ListItem>
                                                                        <asp:ListItem Value="1.3">130%</asp:ListItem>
                                                                        <asp:ListItem Value="1.4">140%</asp:ListItem>
                                                                        <asp:ListItem Value="1.5">150%</asp:ListItem>
                                                                        <asp:ListItem Value="1.6">160%</asp:ListItem>
                                                                        <asp:ListItem Value="1.7">170%</asp:ListItem>
                                                                        <asp:ListItem Value="1.8">180%</asp:ListItem>
                                                                        <asp:ListItem Value="1.9">190%</asp:ListItem>
                                                                        <asp:ListItem Value="2.0">200%</asp:ListItem>
                                                                    </asp:DropDownList>
                                                                    <asp:Button ID="btnRatioPlus" runat="server" CausesValidation="false" Text="+" OnClick="btnRatio_Click" CommandArgument="Plus" />
                                                                </div>
                                                                <div class="col-md-2" style="display:none;">
                                                                    <label>Left Margin</label>
                                                                </div>
                                                                <div class="col-md-10" style="display:none;">
                                                                    <asp:Button ID="btnLeftMarginMinus" runat="server" CausesValidation="false" Text="-" OnClick="btnLeftMargin_Click" CommandArgument="Minus" />
                                                                        <asp:DropDownList ID="ddlLeftMargin" runat="server" AutoPostBack="true" CausesValidation="false" OnSelectedIndexChanged="ddlLeftMargin_SelectedIndexChanged">
                                                                        <asp:ListItem Value="1.9">10%</asp:ListItem>
                                                                        <asp:ListItem Value="1.8">20%</asp:ListItem>
                                                                        <asp:ListItem Value="1.7">30%</asp:ListItem>
                                                                        <asp:ListItem Value="1.6">40%</asp:ListItem>
                                                                        <asp:ListItem Value="1.5">50%</asp:ListItem>
                                                                        <asp:ListItem Value="1.4">60%</asp:ListItem>
                                                                        <asp:ListItem Value="1.3">70%</asp:ListItem>
                                                                        <asp:ListItem Value="1.2">80%</asp:ListItem>
                                                                        <asp:ListItem Value="1.1">90%</asp:ListItem>
                                                                        <asp:ListItem Value="1.0" Selected="True">100%</asp:ListItem>
                                                                        <asp:ListItem Value="0.9">110%</asp:ListItem>
                                                                        <asp:ListItem Value="0.8">120%</asp:ListItem>
                                                                        <asp:ListItem Value="0.7">130%</asp:ListItem>
                                                                        <asp:ListItem Value="0.6">140%</asp:ListItem>
                                                                        <asp:ListItem Value="0.5">150%</asp:ListItem>
                                                                        <asp:ListItem Value="0.4">160%</asp:ListItem>
                                                                        <asp:ListItem Value="0.3">170%</asp:ListItem>
                                                                        <asp:ListItem Value="0.2">180%</asp:ListItem>
                                                                        <asp:ListItem Value="0.1">190%</asp:ListItem>
                                                                        <asp:ListItem Value="0.0">200%</asp:ListItem>
                                                                    </asp:DropDownList>
                                                                    <asp:Button ID="btnLeftMarginPlus" runat="server" CausesValidation="false" Text="+" OnClick="btnLeftMargin_Click" CommandArgument="Plus" />
                                                                </div>
                                                                <div class="col-md-2" style="display:none;">
                                                                    <label>Top Margin</label>
                                                                </div>
                                                                <div class="col-md-10" style="display:none;">
                                                                    <asp:Button ID="btnTopMarginMinus" runat="server" CausesValidation="false" Text="-" OnClick="btnTopMargin_Click" CommandArgument="Minus" />
                                                                        <asp:DropDownList ID="ddlTopMargin" runat="server" AutoPostBack="true" CausesValidation="false" OnSelectedIndexChanged="ddlTopMargin_SelectedIndexChanged">
                                                                        <asp:ListItem Value="0.1">10%</asp:ListItem>
                                                                        <asp:ListItem Value="0.2">20%</asp:ListItem>
                                                                        <asp:ListItem Value="0.3">30%</asp:ListItem>
                                                                        <asp:ListItem Value="0.4">40%</asp:ListItem>
                                                                        <asp:ListItem Value="0.5">50%</asp:ListItem>
                                                                        <asp:ListItem Value="0.6">60%</asp:ListItem>
                                                                        <asp:ListItem Value="0.7">70%</asp:ListItem>
                                                                        <asp:ListItem Value="0.8">80%</asp:ListItem>
                                                                        <asp:ListItem Value="0.9">90%</asp:ListItem>
                                                                        <asp:ListItem Value="1.0" Selected="True">100%</asp:ListItem>
                                                                        <asp:ListItem Value="1.1">110%</asp:ListItem>
                                                                        <asp:ListItem Value="1.2">120%</asp:ListItem>
                                                                        <asp:ListItem Value="1.3">130%</asp:ListItem>
                                                                        <asp:ListItem Value="1.4">140%</asp:ListItem>
                                                                        <asp:ListItem Value="1.5">150%</asp:ListItem>
                                                                        <asp:ListItem Value="1.6">160%</asp:ListItem>
                                                                        <asp:ListItem Value="1.7">170%</asp:ListItem>
                                                                        <asp:ListItem Value="1.8">180%</asp:ListItem>
                                                                        <asp:ListItem Value="1.9">190%</asp:ListItem>
                                                                        <asp:ListItem Value="2.0">200%</asp:ListItem>
                                                                    </asp:DropDownList>
                                                                    <asp:Button ID="btnTopMarginPlus" runat="server" CausesValidation="false" Text="+" OnClick="btnTopMargin_Click" CommandArgument="Plus" />
                                                                </div>
                                                                <br />--%>
                                                            </asp:PlaceHolder>
                                                            
                                                        </asp:PlaceHolder>
                                                    <div class="product-buttons">
                                                        <div class="row" style="margin:10px;">
                                                            <div class="col-md-6" style="padding:10px">
                                                                <asp:Button id="btnAddToCart" runat="server" Text="Add To Cart" CssClass="btn btn-solid hover-solid btn-animation" OnClick="btnAddToCart_Click" CausesValidation="false"/>
                                                            </div>
                                                            <div class="col-md-6" style="padding:10px">
                                                                <asp:Button id="btnAddMore" runat="server" Text="Add More" CssClass="btn btn-solid hover-solid btn-animation" OnClick="btnAddMore_Click" CausesValidation="false" Visible="false"/>
                                                            </div>
                                                        </div>
                                                    </div>

                                                    <div id="divDetailedDescription"><asp:Literal ID="litDetailedDescription" runat="server"></asp:Literal></div>
                                                    <div id="divNonInventoryUnavailableMessage" runat="server" style="margin-top:10px" visible="false">
                                                        <span style="font-family: Lato,sans-serif; font-size:12px; color:red;"><strong>
PLEASE NOTE:
<br />
This is a custom-ordered item with a 10-12 day lead time, selecting expedited shipping will NOT expedite the lead time.
This item cannot be returned or exchanged due to the custom decoration.
We are unable to accept orders for items which are currently unavailable – if the size is greyed out, then we do not have stock.
Please check back regularly as stock levels are updated daily.
                                                        </strong></span>
                                                    </div>

                                                    <asp:Repeater id="rptProductDetail" runat="server">
                                                        <ItemTemplate>
                                                            <div class="border-product">
                                                                <h6 class="product-title"><%# Eval("Attribute") %></h6>
                                                                <ul class="shipping-info">
                                                                    <%# Eval("ItemDetailValuesInHTML") %>
                                                                </ul>
                                                            </div>
                                                        </ItemTemplate>
                                                    </asp:Repeater>

                                                    <div class="product-buttons">
                                                        <div class="row" style="margin:10px;">
                                                            <div class="col-md-3">
                                                                <asp:Button id="btnSizeChart" runat="server" Text="Size Chart" CssClass="btn btn-solid hover-solid btn-animation" OnClick="btnSizeChart_Click" CausesValidation="false"/>
                                                            </div>
                                                        </div>
                                                    </div>
                                                </div>
                                            </div>
                                        </div>
                                    </div>
                                </div>

                                <asp:HiddenField ID="hfSelectedGroupByAttributeValueID" runat="server" />
                            </div>

                        </div>
                     </div>
                </div>   
                
                <asp:Panel ID="pnlRelatedItems" runat="server">            
                    <div class="collection-product-wrapper" style="margin:8%;">
                        <div class="row">
                            <div class="col-12 product-related">
                                <h2>related items</h2>
                            </div>
                        </div>
                        <div class="product-wrapper-grid">
                            <div class="row margin-res">

                                <asp:Repeater ID="rptRelatedItem" runat="server" OnItemCommand="rptRelatedItem_ItemCommand" OnItemDataBound="rptRelatedItem_ItemDataBound">
                                    <ItemTemplate>
                                        <div class="col-xl-2 col-md-4 col-6">
                                            <div class="product-box" >
                                                <div class="img-wrapper" style="text-align:center; height:200px;">
                                                    <div class="front">
                                                        <a href="/ProductDetail.aspx?id=<%# Eval("Item.ItemID")%>&websitetabid=<%# mWebSiteTabID %>"><img src='<%# Eval("Item.DisplayImageURL")%>' class="img-fluid blur-up lazyload" alt="" ></a>
                                                    </div>
                                                </div>
                                                <div class="product-detail">
                                                    <div style="text-align:left;">
                                                        <div style="height:50px;display: flex;">
                                                            <h4 style="align-self: flex-end;"><a href="/ProductDetail.aspx?id=<%# Eval("Item.ItemID")%>" style="font-size:16px;font-weight:normal;text-decoration:none;color:#777777"><%# Eval("Item.StoreDisplayName")%></a></h4>
                                                        </div>
                                                        <br />
                                                        <h4><%# string.Format("{0:c}", Eval("Item.PriceRange")) %></h4>
                                                        <h1><asp:Label ID="lblGroupAttributeUnitPrice" runat="server"></asp:Label></h1>

            
            <%--                                            <asp:Repeater ID="rptColors" runat="server">
                                                            <ItemTemplate>
                                                                    <ul">
                                                                    <li class="color-item" style="background-color:#<%# Eval("BackgroundColor")%>; width:20px; height: 20px; margin-right: 8px;">
                                                                    </ul>
                                                            </ItemTemplate>
                                                        </asp:Repeater>--%>

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
    </section>

    <script>
        window.dataLayer = window.dataLayer || [];

        itemName = (function () {
            function capitalizeWords(str) {
                return str.split(' ').map(function (word) {
                    return word.charAt(0).toUpperCase() + word.slice(1).toLowerCase();
                }).join(' ');
            }
            var text = document.getElementById('ItemName').innerText.trim();
            // Capitalize the first letter of each word
            return capitalizeWords(text);
        })(),
            dataLayer.push({
                event: 'view_item',
                ecommerce: {
                    items: {
                        item_id: document.getElementById('ItemNumber').innerText,
                        item_name: itemName,
                    }
                }
            });
        document.getElementById('cphBody_btnAddToCart').addEventListener('click', function () {
            var inputs = document.querySelectorAll('.form-control.input-number');
            var totalQuantity = 0;
            inputs.forEach(function (input) {
                var quantity = parseInt(input.value) || 0;
                totalQuantity += quantity;
            })
            dataLayer.push({
                event: 'add_to_cart',
                ecommerce: {
                    item_id: document.getElementById('ItemNumber').innerText,
                    item_name: itemName,
                    quantity: totalQuantity
                }
            });
        });
    </script>


 
</asp:Content>
<asp:Content ID="Content4" ContentPlaceHolderID="cphFooter" runat="server">
    <uc1:SuperceedingItem ID="ucSuperceedingItem" runat="server" />
    <uc2:ImageModal ID="ucImageModal" runat="server" />
    <uc4:AccountSearchModal ID="ucAccountSearchModal" runat="server" />
</asp:Content>
