<%@ Page Language="C#" MasterPageFile="~/MasterPage.Master" AutoEventWireup="true" CodeBehind="ItemList.aspx.cs" Inherits="ImageSolutionsWebsite.ItemList" %>
<%@ Register src="Control/SuperceedingItem.ascx" tagname="SuperceedingItem" tagprefix="uc1" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="cphHeader" runat="server">
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="cphBody" runat="server">
    <!-- section start -->
    <section class="section-b-space ratio_asos">
        <div class="collection-wrapper">
            <div class="collection-content">
                <div class="page-main-content">
                    <div class="container">
                        <div class="row">
                            <div class="col-sm-12">
                                <div class="collection-product-wrapper">
                                    <div class="product-top-filter" style="display:none;">
                                        <div class="row">
                                            <div class="col-12">
                                                <div class="product-filter-content">
                                                    <div class="search-count">
                                                        <h5><asp:Label ID="lblPagingRecordText" runat="server">Showing Products 1-24 of 10 Result</asp:Label></h5>
                                                    </div>
                                                    <div class="collection-view">
                                                        <ul>
                                                            <li><i class="fa fa-th grid-layout-view"></i></li>
                                                            <li><i class="fa fa-list-ul list-layout-view"></i></li>
                                                        </ul>
                                                    </div>
                                                    <div class="collection-grid-view">
                                                        <%--<ul>
                                                            <li><img src="../assets/images/icon/2.png" alt=""
                                                                    class="product-2-layout-view"></li>
                                                            <li><img src="../assets/images/icon/3.png" alt=""
                                                                    class="product-3-layout-view"></li>
                                                            <li><img src="../assets/images/icon/4.png" alt=""
                                                                    class="product-4-layout-view"></li>
                                                            <li><img src="../assets/images/icon/6.png" alt=""
                                                                    class="product-6-layout-view"></li>
                                                        </ul>--%>
                                                    </div>
                                                    <div class="product-page-per-view">
                                                        <%--<select>
                                                            <option value="High to low">24 Products Par Page</option>
                                                            <option value="Low to High">48 Products Par Page</option>
                                                            <option value="Low to High">96 Products Par Page</option>
                                                        </select>--%>
                                                    </div>
                                                </div>
                                            </div>
                                        </div>
                                    </div>
                                    <div class="product-wrapper-grid list-view">
                                        <div class="row">
                                            <asp:Repeater ID="rptItems" runat="server" DataMember="ItemID" OnItemDataBound="rptItems_ItemDataBound">
                                                <ItemTemplate>
                                                    <div class="col-1"></div>
                                                    <div class="col-10">
                                                        <asp:HiddenField ID="hfItemID" runat="server" Value='<%# Eval("Item.ItemID")%>'/>
                                                        <div class="product-box">
                                                            <div class="img-wrapper">
                                                                <div class="front">
                                                                    <a href="ProductDetail.aspx?id=<%# Eval("Item.ItemID") %>&WebsiteTabID=<%# mWebSiteTabID %>"><img src='<%# Eval("Item.ImageURL") %>' class="img-fluid blur-up lazyload bg-img" alt=""></a>
                                                                </div>
                                                                <div class="back" style="display:none;">
                                                                    <a href="ProductDetail.aspx?id=<%# Eval("Item.ItemID") %>&WebsiteTabID=<%# mWebSiteTabID %>"><img src='<%# Eval("Item.ImageURL") %>' class="img-fluid blur-up lazyload bg-img" alt=""></a>
                                                                </div>
                                                                <div class="cart-info cart-wrap"></div>
                                                            </div>
                                                            <div class="product-detail">
                                                                <div>
                                                                    <%--<div class="rating"><i class="fa fa-star"></i> <i
                                                                            class="fa fa-star"></i> <i class="fa fa-star"></i>
                                                                        <i class="fa fa-star"></i> <i class="fa fa-star"></i>
                                                                    </div><a href="product-page(no-sidebar).html"></a>--%>
                                                                    <h6><%# Eval("Item.StoreDisplayName") %></h6>
                                                                    
                                                                    <%# (Convert.ToBoolean(Eval("Item.IsNonInventory")) ? "<font style='color:red;'>(This is a custom item. You will receive in 15 business days)</font>" : String.Empty) %>
                                                                    <p><%# Eval("Item.ItemName") %></p>
                                                                    <%--<h4><%# string.Format("{0:c}", Eval("Item.BasePrice")) %></h4>--%>
                                                                   <%-- <h4><%# string.Format("{0:c}", Eval("Price")) %></h4>--%>

                                                                    <asp:Panel ID="pnlNoAttribute" runat="server">
                                                                        <table>
                                                                            <tr>
                                                                                <td style="min-width:100px;">Quantity</td>
                                                                                <td style="text-align:center;">
                                                                                    <div class="qty-box">
                                                                                        <div class="input-group">
                                                                                            <asp:TextBox ID="txtQuantity" runat="server" Width="80px" type="number" CssClass="form-control input-number"></asp:TextBox>
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
                                                                        <p> </p>
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
                                                                                                <asp:TextBox ID="txtNoGroupAttributeQuantity" runat="server" Width="80px" type="number" CssClass="form-control input-number"></asp:TextBox>
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
                                                                            <asp:Panel ID="pnlGroupSingleAttribute" runat="server" Visible="false">
                                                                                <asp:Repeater ID="rptGroupSingleAttributeValue" runat="server" OnItemDataBound="rptGroupSingleAttributeValue_ItemDataBound">
                                                                                    <ItemTemplate>
                                                                                        <div class="row">
                                                                                            <!--<div class="col-0" style="margin-top:100px; vertical-align:middle;">
                                                                                                <%# Eval("Value")%>
                                                                                                <asp:HiddenField ID="hfAttributeValueID" runat="server" Value='<%# Eval("AttributeValueID")%>'/>
                                                                                            </div>-->
                                                                                            <div class="col-12" style="text-align:center;">
                                                                                                <div class="row">
                                                                                                    <asp:Repeater ID="rptGroupAttribute" runat="server" OnItemCommand="rptGroupAttribute_ItemCommand">
                                                                                                        <ItemTemplate>
                                                                                                            <div class="col-2" style="text-align:center;">
                                                                                                                <%# Eval("Value")%>
                                                                                                                <asp:HiddenField ID="hfListAttributeValueID" runat="server" Value='<%# Eval("AttributeValueID")%>'/>
                                                                                                                                
                                                                                                                <div class="qty-box">
                                                                                                                    <div class="input-group">
                                                                                                                        <asp:TextBox ID="txtGroupAttributeQuantity" runat="server" Width="80px" type="number" CssClass="form-control input-number"></asp:TextBox>
                                                                                                                    </div>
                                                                                                                </div>
                                                                                                                <asp:Label ID="lblGroupAttributeUnitPrice" runat="server"></asp:Label><br />
                                                                                                                <u style="font-size:10px;"><asp:Label ID="lblGroupAttributeQuantityAvailable" runat="server"></asp:Label></u><br /><asp:LinkButton ID="lbnGroupAttributeSuperceedingItem" runat="server" visible="false" CausesValidation="false">Other Options</asp:LinkButton><a id="aGroupAttributeSuperceedingItem" runat="server" visible="false">Other Options</a>
                                                                                                                <br />
                                                                                                            </div>
                                                                                                        </ItemTemplate>
                                                                                                    </asp:Repeater>
                                                                                                </div>
                                                                                            </div>
                                                                                        </div>
                                                                                    </ItemTemplate>
                                                                                </asp:Repeater>              
                                                                            </asp:Panel>
                                                                            <asp:Panel ID="pnlGroupMultipleAttribute" runat="server" Visible="false">
                                                                                
                                                                            <table>
                                                                                <tr>
                                                                                    <td>
                                                                                        <asp:Repeater ID="rptMultipleGroupAttributes" runat="server" DataMember="AttributeID" OnItemDataBound="rptAttributes_ItemDataBound">
                                                                                            <ItemTemplate>                                                                
                                                                                                <%# Eval("AttributeName")%> : <asp:DropDownList ID="ddlAttributeValue" runat="server" OnSelectedIndexChanged="ddlAttributeValue_SelectedIndexChanged" AutoPostBack="true"></asp:DropDownList>
                                                                                                <asp:HiddenField ID="hfAttributeID" runat="server" Value='<%# Eval("AttributeID")%>'/>
                                                                                            </ItemTemplate>
                                                                                        </asp:Repeater>
                                                                                    </td>
                                                                                </tr>
                                                                                <tr>
                                                                                    <td style="text-align:center;">
                                                                                        <table>
                                                                                            <tr>
                                                                                                <asp:Repeater ID="rptMutlipleGroupAttributeValue" runat="server">
                                                                                                    <ItemTemplate>
                                                                                                        <td style="text-align:center;">
                                                                                                            <table width="100%">
                                                                                                                <tr>
                                                                                                                    <td style="text-align:center;">
                                                                                                                        <%# Eval("Value")%> 
                                                                                                                        <asp:HiddenField ID="hfAttributeValueID" runat="server" Value='<%# Eval("AttributeValueID")%>'/>
                                                                                                                    </td>
                                                                                                                </tr>
                                                                                                                <tr>
                                                                                                                    <td style="text-align:center;">
                                                                                                                        <div class="qty-box">
                                                                                                                            <div class="input-group">
                                                                                                                                <asp:TextBox ID="txtGroupAttributeQuantity" type="number" CssClass="form-control input-number" runat="server" Width="80px"></asp:TextBox>
                                                                                                                            </div>
                                                                                                                        </div>
                                                                                                                    </td>
                                                                                                                </tr>
                                                                                                                <tr>
                                                                                                                    <td style="text-align:center;"><asp:Label ID="lblGroupAttributeUnitPrice" runat="server"></asp:Label></td>
                                                                                                                </tr>
                                                                                                                <tr>
                                                                                                                    <td style="text-align:center; font-size:11px;"><u><asp:Label ID="lblGroupAttributeQuantityAvailable" runat="server"></asp:Label></u><br /><asp:LinkButton ID="lbnGroupAttributeSuperceedingItem" runat="server" visible="false" CausesValidation="false">Other Options</asp:LinkButton><a id="aGroupAttributeSuperceedingItem" runat="server" visible="false">Other Options</a></td>
                                                                                                                </tr>
                                                                                                            </table>
                                                                                                        </td>
                                                                                                    </ItemTemplate>
                                                                                                </asp:Repeater>
                                                                                            </tr>
                                                                                        </table>
                                                                                    </td>
                                                                                </tr>
                                                                                </table>
                                                                            </asp:Panel>
                                                                        </asp:Panel>                                                                            
                                                                    </asp:Panel> 
                                                                </div>
                                                            </div>
                                                        </div>
                                                    </div>
                                                    <div class="col-1"></div>
                                                </ItemTemplate>
                                            </asp:Repeater>
                                        </div>
                                        <div class="row">
                                            <div class="col-12" style="text-align:center;"><p></p><asp:Button ID="btnAddToCart" runat="server" Text="Add To Cart" OnClick="btnAddToCart_Click" CssClass="btn btn-sm btn-solid" /></div>
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
    <!-- section End -->
</asp:Content>

<asp:Content ID="Content4" ContentPlaceHolderID="cphFooter" runat="server">
    <uc1:SuperceedingItem ID="ucSuperceedingItem" runat="server" />
</asp:Content>