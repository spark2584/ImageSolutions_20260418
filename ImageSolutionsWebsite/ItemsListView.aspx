<%@ Page Language="C#" MasterPageFile="~/MasterPage.Master" AutoEventWireup="true" CodeBehind="ItemsListView.aspx.cs" Inherits="ImageSolutionsWebsite.ItemsListView" %>
<%--<%@ Register Src="~/Control/Pager.ascx" TagPrefix="uc1" TagName="Pager" %>--%>

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
                                    <div class="product-top-filter">
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
                                                    <div class="col-3"><asp:HiddenField ID="hfItemID" runat="server" Value='<%# Eval("Item.ItemID")%>'/></div>
                                                    <div class="col-6">
                                                        <div class="product-box">
                                                            <div class="img-wrapper">
                                                                <div class="front">
                                                                    <a href="#"><img src='<%# Eval("Item.ImageURL") %>' class="img-fluid blur-up lazyload bg-img" alt=""></a>
                                                                </div>
                                                                <div class="back" style="display:none;">
                                                                    <a href="#"><img src='<%# Eval("Item.ImageURL") %>' class="img-fluid blur-up lazyload bg-img" alt=""></a>
                                                                </div>
                                                                <div class="cart-info cart-wrap"></div>
                                                            </div>
                                                            <div class="product-detail">
                                                                <div>
                                                                    <%--<div class="rating"><i class="fa fa-star"></i> <i
                                                                            class="fa fa-star"></i> <i class="fa fa-star"></i>
                                                                        <i class="fa fa-star"></i> <i class="fa fa-star"></i>
                                                                    </div><a href="product-page(no-sidebar).html"></a>--%>
                                                                    <h6><%# Eval("Item.ItemName") %></h6>
                                                                    
                                                                    <p><%# Eval("Item.SalesDescription") %></p>
                                                                    <h4><%# string.Format("{0:c}", Eval("Item.BasePrice")) %></h4>


                                                                    <asp:Panel ID="pnlNoAttribute" runat="server">
                                                                        <table width="100%">
                                                                            <tr>
                                                                                <td>
                                                                                    Quantity
                                                                                </td>
                                                                                <td>
                                                                                    <asp:TextBox ID="txtQuantity" runat="server"></asp:TextBox>
                                                                                </td>
                                                                            </tr>
                                                                        </table>
                                                                    </asp:Panel>
                                                                    <asp:Panel ID="pnlAttribute" runat="server" Visible="false">
                                                                        <table width="100%">
                                                                            <tr>
                                                                                <td>
                                                                                    <asp:GridView ID="gvAttributes" AutoGenerateColumns="False" GridLines="None" Width="100%" AllowSorting="true" CellSpacing="0" CellPadding="0" 
                                                                                        DataKeyNames="AttributeID" runat="server" OnRowDataBound="gvAttributes_RowDataBound">
                                                                                        <Columns>
                                                                                            <asp:TemplateField ItemStyle-HorizontalAlign="Left" ItemStyle-Width="100%">
                                                                                                <ItemTemplate>                                                                
                                                                                                    <%# Eval("AttributeName")%> : <asp:DropDownList ID="ddlAttributeValue" runat="server" OnSelectedIndexChanged="ddlAttributeValue_SelectedIndexChanged" AutoPostBack="true"></asp:DropDownList>
                                                                                                    <asp:HiddenField ID="hfAttributeID" runat="server" Value='<%# Eval("AttributeID")%>'/>
                                                                                                </ItemTemplate>
                                                                                            </asp:TemplateField>
                                                                                        </Columns>
                                                                                    </asp:GridView>
                                                                                </td>
                                                                            </tr>
                                                                            <tr>
                                                                                <td>
                                                                                    <table width="100%">
                                                                                        <tr>
                                                                                            <td>
                                                                                                <asp:Panel ID="pnlNoGroup" runat="server" Visible="false">
                                                                                                    <table width="100%">
                                                                                                        <tr>
                                                                                                            <td>
                                                                                                                Quantity
                                                                                                            </td>
                                                                                                            <td>
                                                                                                                <asp:TextBox ID="txtNoGroupAttributeQuantity" runat="server"></asp:TextBox>
                                                                                                            </td>
                                                                                                        </tr>
                                                                                                    </table>
                                                                                                </asp:Panel>
                                                                                                <asp:Panel ID="pnlGroup" runat="server" Visible="false">
                                                                                                    <table width="100%">
                                                                                                        <tr>
                                                                                                            <asp:Repeater ID="rptGroupAttribute" runat="server">
                                                                                                                <ItemTemplate>
                                                                                                                    <td>
                                                                                                                        <table width="100%">
                                                                                                                            <tr>
                                                                                                                                <td>
                                                                                                                                    <%# Eval("Value")%> 
                                                                                                                                    <asp:HiddenField ID="hfAttributeValueID" runat="server" Value='<%# Eval("AttributeValueID")%>'/>
                                                                                                                                </td>
                                                                                                                            </tr>
                                                                                                                            <tr>
                                                                                                                                <td>
                                                                                                                                    <asp:TextBox ID="txtGroupAttributeQuantity" runat="server"></asp:TextBox>
                                                                                                                                </td>
                                                                                                                            </tr>
                                                                                                                        </table>
                                                                                                                    </td>
                                                                                                                </ItemTemplate>
                                                                                                            </asp:Repeater>
                                                                                                        </tr>
                                                                                                    </table>
                                                                                                </asp:Panel>                                                            
                                                                                            </td>
                                                                                        </tr>
                                                                                    </table>
                                                                                </td>
                                                                            </tr>
                                                                        </table>
                                                                    </asp:Panel> 


                                                                    <%--<ul class="color-variant">
                                                                        <li class="bg-light0"></li>
                                                                        <li class="bg-light1"></li>
                                                                        <li class="bg-light2"></li>
                                                                    </ul>--%>
                                                                </div>
                                                            </div>
                                                        </div>
                                                    </div>
                                                    <div class="col-3"></div>
                                                </ItemTemplate>
                                            </asp:Repeater>
                                        </div>
                                        <div class="row">
                                            <div class="col-3"></div>
                                            <div class="col-3"></div>
                                            <div class="col-3"><asp:Button ID="btnAddToCart" runat="server" Text="Add To Cart" OnClick="btnAddToCart_Click" CssClass="btn btn-sm btn-solid" /></div>
                                            <div class="col-3"></div>
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


    <%--<asp:GridView ID="gvItem" runat="server" AutoGenerateColumns="False" GridLines="None" Width="100%" AllowSorting="true" CellSpacing="0" CellPadding="0" 
        DataKeyNames="ItemID" OnRowCommand="gvItems_RowCommand" OnRowDataBound="gvItems_RowDataBound" ShowHeader="false">
        <Columns>
            <asp:TemplateField ItemStyle-HorizontalAlign="Center" ItemStyle-Width="100%">
                <ItemTemplate>
                    <table Width="100%">
                        <tr>
                            <td style="width:30%">
                                <%# Eval("Item.ItemNumber")%>
                                <asp:Image ID="imgItem" runat="server" />
                            </td>
                            <td style="width:70%">
                                <asp:Panel ID="pnlNoAttribute" runat="server">
                                    <table width="100%">
                                        <tr>
                                            <td>
                                                Quantity
                                            </td>
                                            <td>
                                                <asp:TextBox ID="txtQuantity" runat="server"></asp:TextBox>
                                            </td>
                                        </tr>
                                    </table>
                                </asp:Panel>
                                <asp:Panel ID="pnlAttribute" runat="server" Visible="false">
                                    <table width="100%">
                                        <tr>
                                            <td>
                                                <asp:GridView ID="gvAttributes" AutoGenerateColumns="False" GridLines="None" Width="100%" AllowSorting="true" CellSpacing="0" CellPadding="0" 
                                                    DataKeyNames="AttributeID" runat="server" OnRowDataBound="gvAttributes_RowDataBound">
                                                    <Columns>
                                                        <asp:TemplateField ItemStyle-HorizontalAlign="Left" ItemStyle-Width="100%">
                                                            <ItemTemplate>                                                                
                                                                <%# Eval("AttributeName")%> : <asp:DropDownList ID="ddlAttributeValue" runat="server" OnSelectedIndexChanged="ddlAttributeValue_SelectedIndexChanged" AutoPostBack="true"></asp:DropDownList>
                                                                <asp:HiddenField ID="hfAttributeID" runat="server" Value='<%# Eval("AttributeID")%>'/>
                                                            </ItemTemplate>
                                                        </asp:TemplateField>
                                                    </Columns>
                                                </asp:GridView>
                                            </td>
                                        </tr>
                                        <tr>
                                            <td>
                                                <table width="100%">
                                                    <tr>
                                                        <td>
                                                            <asp:Panel ID="pnlNoGroup" runat="server" Visible="false">
                                                                <table width="100%">
                                                                    <tr>
                                                                        <td>
                                                                            Quantity
                                                                        </td>
                                                                        <td>
                                                                            <asp:TextBox ID="txtNoGroupAttributeQuantity" runat="server"></asp:TextBox>
                                                                        </td>
                                                                    </tr>
                                                                </table>
                                                            </asp:Panel>
                                                            <asp:Panel ID="pnlGroup" runat="server" Visible="false">
                                                                <table width="100%">
                                                                    <tr>
                                                                        <asp:Repeater ID="rptGroupAttribute" runat="server">
                                                                            <ItemTemplate>
                                                                                <td>
                                                                                    <table width="100%">
                                                                                        <tr>
                                                                                            <td>
                                                                                                <%# Eval("Value")%> 
                                                                                                <asp:HiddenField ID="hfAttributeValueID" runat="server" Value='<%# Eval("AttributeValueID")%>'/>
                                                                                            </td>
                                                                                        </tr>
                                                                                        <tr>
                                                                                            <td>
                                                                                                <asp:TextBox ID="txtGroupAttributeQuantity" runat="server"></asp:TextBox>
                                                                                            </td>
                                                                                        </tr>
                                                                                    </table>
                                                                                </td>
                                                                            </ItemTemplate>
                                                                        </asp:Repeater>
                                                                    </tr>
                                                                </table>
                                                            </asp:Panel>                                                            
                                                        </td>
                                                    </tr>
                                                </table>
                                            </td>
                                        </tr>
                                    </table>
                                </asp:Panel>                                
                            </td>
                            <td style="width:10%">
                            </td>
                        </tr>
                    </table>
                </ItemTemplate>
            </asp:TemplateField>
        </Columns>
    </asp:GridView>--%>

</asp:Content>