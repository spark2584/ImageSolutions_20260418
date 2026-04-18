<%@ Page Title="" Language="C#" MasterPageFile="~/MasterPage.Master" AutoEventWireup="true" CodeBehind="ShoppingCart.aspx.cs" Inherits="ImageSolutionsWebsite.ShoppingCart" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server"></asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="cphHeader" runat="server"></asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="cphBody" runat="server">
    <!--section start-->
    <section class="cart-section section-b-space">
        <div class="container">
            <div class="row">
                <div class="col-sm-12 table-responsive-xs">
                    <asp:GridView ID="gvShoppingCartLine" runat="server" AutoGenerateColumns="False" DataKeyNames="ShoppingCartLineID, Quantity, UserInfoID, CustomListID_1, CustomListValueID_1, CustomListID_2, CustomListValueID_2" CssClass="table cart-table" HeaderStyle-CssClass="table-head" Width="100%" 
                        GridLines="None" BorderWidth="0" AllowSorting="true" CellSpacing="0" CellPadding="0" OnRowCommand="gvShoppingCart_RowCommand" OnRowDataBound="gvShoppingCartLine_RowDataBound">
                        <Columns>
                            <asp:TemplateField HeaderText="Image" HeaderStyle-HorizontalAlign="Center" ItemStyle-HorizontalAlign="Left">
                                <ItemTemplate>
                                    <a href="/ProductDetail.aspx?id=<%# (!string.IsNullOrEmpty(Convert.ToString(Eval("Item.ParentID"))) ? Convert.ToString(Eval("Item.ParentID")) : Eval("Item.ItemID")) %>"><asp:Image id="imgItemImage" runat="server" /></a>
                                </ItemTemplate>
                            </asp:TemplateField>
                            <asp:TemplateField HeaderText="Product Name" HeaderStyle-HorizontalAlign="Center" ItemStyle-HorizontalAlign="Left">
                                <ItemTemplate>
                                    <%--<%# Eval("Item.ItemNumber")%><br /><%# Eval("Item.StoreDisplayName")%><br />--%>
                                    <%# Eval("Item.ItemNumber")%><br /><%# Eval("Item.SalesDescription")%>
<%--                                    <%# (
                                            (
                                                Convert.ToString(Eval("ShoppingCart.UserWebsite.WebsiteID")) == "2"
                                                &&
                                                (
                                                    Convert.ToBoolean(Eval("Item.IsNonInventory")) && (Convert.ToDecimal(Eval("Item.VendorInventory")) < Convert.ToDecimal(Eval("Quantity")))
                                                    ||
                                                    !Convert.ToBoolean(Eval("Item.IsNonInventory")) && (Convert.ToDecimal(Eval("Item.QuantityAvailable")) < Convert.ToDecimal(Eval("Quantity")))
                                                )   
                                            )
                                                //? "&nbsp;&nbsp;<span style='font-size: medium; color: red;' title='" + Eval("Item.SalesDescription") + " is temporarily out of stock but can be backordered. You will be charged at the time of purchase for both the product and shipping. Please check the home page announcement for the estimated restock date.'><i class='ti-info-alt' ></i> </span>"
                                                ? "&nbsp;&nbsp;<span style='font-size: medium; color: red;' title='This item is temporarily out of stock but can be backordered. You will be charged at the time of purchase for both the product and shipping. Please check the home page announcement for the estimated restock date.'><i class='ti-info-alt' ></i> </span>"
                                            : String.Empty) %>--%>

                                    <br />
                                    <%# (!string.IsNullOrEmpty(Convert.ToString(Eval("EmployeeDescription"))) ? "<font style='color:blue;'>"+ Eval("EmployeeDescription") + "</font><br/>" : String.Empty) %>
                                    <%# (!string.IsNullOrEmpty(Convert.ToString(Eval("CustomDesignName"))) ? "<font style='color:blue;'>(This is a personalized item, Name = " + Eval("CustomDesignName") + ")</font><br/>" : String.Empty) %>
                                    <%# (!string.IsNullOrEmpty(Convert.ToString(Eval("LogosDescription"))) ? "<font style='color:blue;'> " + Eval("LogosDescription") + " </font><br/>" : String.Empty) %>
                                    <%# (!string.IsNullOrEmpty(Convert.ToString(Eval("PersonalizationDescription"))) ? "<font style='color:blue;'> " + Eval("PersonalizationDescription") + " </font><br/>" : String.Empty) %>

                                    <%# (Eval("Item.DiscountAmount") != null && Convert.ToDecimal(Eval("Item.DiscountAmount")) > 0 ? "<font style='color:blue;'> Discount: " + Eval("Item.DiscountAmount", "{0:C2}") + " </font><br/>" : String.Empty) %>

                                    <%# (
                                            (
                                                Convert.ToBoolean(Eval("Item.IsNonInventory")) 
                                                || !string.IsNullOrEmpty(Convert.ToString(Eval("CustomListValueID_1"))) 
                                                || !string.IsNullOrEmpty(Convert.ToString(Eval("CustomListValueID_2"))) 
                                                || Convert.ToBoolean(Eval("HasCusomtization")) 
                                            ) && ( Convert.ToString(Eval("ShoppingCart.UserWebsite.WebsiteID")) != "9" || Convert.ToString(Eval("Item.ItemType")) != "_inventoryItem" )
                                                ? "<font style='color:red;'>(This is a custom item. You will receive it in 12 business days.)</font>" 
                                                : String.Empty) %>
                                    
                                    <%# (
                                            (
                                                Convert.ToString(Eval("ShoppingCart.UserWebsite.WebsiteID")) == "2"
                                                &&
                                                (
                                                    Convert.ToBoolean(Eval("Item.IsNonInventory")) && (Convert.ToDecimal(Eval("Item.VendorInventory")) < Convert.ToDecimal(Eval("Quantity")))
                                                    ||
                                                    !Convert.ToBoolean(Eval("Item.IsNonInventory")) && (Convert.ToDecimal(Eval("Item.QuantityAvailable")) < Convert.ToDecimal(Eval("Quantity")))
                                                )
                                            )
                                                ? "<font style='color:red;'> This item is temporarily out of stock but can be backordered. You will be charged at the time of purchase for both the product and shipping. Please check the home page announcement for the estimated restock date.</font>" 
                                            : String.Empty) %>
                                    
                                    

                                    <div class="mobile-cart-content row">
                                        <div class="col">
                                            <div class="qty-box">
                                                <div class="input-group">
                                                    <asp:TextBox ID="txtQuantityMobile" runat="server" type="number" CssClass="form-control input-number" Text='<%# Eval("Quantity")%>' min="0"></asp:TextBox>
                                                </div>
                                            </div>
                                        </div>
                                        <div class="col">
                                            <%--<h2 class="td-color"><%# string.Format("{0:c}", Eval("UnitPrice"))%></h2>--%>
                                            <h2 class="td-color"><%# string.Format("{0:c}", Eval("UnitTotal"))%></h2>
                                        </div>
                                        <div class="col">
                                            <h2 class="td-color"><asp:LinkButton id="btnDeleteMobile" runat="server" CommandArgument='<%# Eval("ShoppingCartLineID") %>' CommandName="DeleteLine"><i class="ti-close"></i></asp:LinkButton></h2>
                                        </div>
                                    </div>
                                </ItemTemplate>
                            </asp:TemplateField>
                            <asp:TemplateField HeaderText="Unit Price" ItemStyle-HorizontalAlign="Center" HeaderStyle-HorizontalAlign="Center">
                                <ItemTemplate>
                                    <h2><%# string.Format("{0:c}", Eval("UnitTotal"))%></h2>
                                </ItemTemplate>
                            </asp:TemplateField>
                            <asp:TemplateField HeaderText="Tariff Surcharge" ItemStyle-HorizontalAlign="Center" HeaderStyle-HorizontalAlign="Center">
                                <ItemTemplate>
                                    <h2><%# string.Format("{0:c}", Eval("TariffCharge"))%></h2>
                                </ItemTemplate>
                            </asp:TemplateField>
                            <asp:TemplateField HeaderText="Action" Visible="false" HeaderStyle-HorizontalAlign="Center" ItemStyle-HorizontalAlign="Center">
                                <ItemTemplate>
                                    <asp:Button id="btnAdd" runat="server" Text="+" CommandArgument='<%# Eval("ShoppingCartLineID") %>' CommandName="AddQty" />
                                </ItemTemplate>
                            </asp:TemplateField>
                            <asp:TemplateField HeaderText="Quantity" ItemStyle-HorizontalAlign="Center" HeaderStyle-HorizontalAlign="Center">
                                <ItemTemplate>
                                    <div class="qty-box">
                                        <div class="input-group">
                                            <asp:TextBox ID="txtQuantity" runat="server" type="number" CssClass="form-control input-number" Text='<%# Eval("Quantity")%>'></asp:TextBox>
                                        </div>
                                    </div>
                                </ItemTemplate>
                            </asp:TemplateField>
                            <asp:TemplateField HeaderText="Employee" Visible="false" HeaderStyle-HorizontalAlign="Center" ItemStyle-HorizontalAlign="Center">
                                <ItemTemplate>
                                    <div class="row" id="rowOptions" runat="server" visible="false">
                                        <%--<div class="col" id="divCustomList_1" runat="server" visible="false" style="padding-bottom:10px;">
                                            Division: <asp:DropDownList ID="ddlCustomListValue_1" runat="server" Width="120px" DataValueField="CustomListValueID" DataTextField="ListValue" AutoPostBack="true" OnSelectedIndexChanged="ddlOptions_SelectedIndexChanged"></asp:DropDownList>
                                        </div>
                                        <div class="col" id="divCustomList_2" runat="server" visible="false" style="padding-bottom:10px;">
                                            Military: <asp:DropDownList ID="ddlCustomListValue_2" runat="server" Width="120px" DataValueField="CustomListValueID" DataTextField="ListValue" AutoPostBack="true" OnSelectedIndexChanged="ddlOptions_SelectedIndexChanged"></asp:DropDownList>
                                        </div>--%>
                                        <div class="col" id="divEmployee" runat="server" visible="false">
                                            <%--Employee: --%><asp:DropDownList ID="ddlUserInfo" runat="server" Width="100%" DataValueField="UserInfoID" DataTextField="FullName"  CssClass="form-control form-select-sm form-select"></asp:DropDownList>
                                        </div>
                                    </div>                               
                                </ItemTemplate>
                            </asp:TemplateField>
                            <asp:TemplateField HeaderText="Action" Visible="false" HeaderStyle-HorizontalAlign="Center" ItemStyle-HorizontalAlign="Center">
                                <ItemTemplate>
                                    <asp:Button id="btnSub" runat="server" Text="-" CommandArgument='<%# Eval("ShoppingCartLineID") %>' CommandName="SubQty" />
                                </ItemTemplate>
                            </asp:TemplateField>
                            <asp:TemplateField HeaderText="Remove Item" ItemStyle-HorizontalAlign="Center" HeaderStyle-HorizontalAlign="Center" Visible="false">
                                <ItemTemplate>
                                    <asp:LinkButton id="btnDelete" runat="server" CommandArgument='<%# Eval("ShoppingCartLineID") %>' CommandName="DeleteLine"><i class="ti-trash" style="font-size:x-large"></i></asp:LinkButton>
                                </ItemTemplate>
                            </asp:TemplateField>
                            <asp:TemplateField HeaderText="Action" ItemStyle-HorizontalAlign="Center" HeaderStyle-HorizontalAlign="Center">
                                <ItemTemplate>
                                    <asp:LinkButton id="btnBatchDelete" runat="server" CommandArgument='<%# Eval("ShoppingCartLineID") %>' CommandName="DeleteLine">REMOVE</asp:LinkButton>&nbsp;|&nbsp;
                                    <asp:LinkButton id="btnBatchCopy" runat="server" CommandArgument='<%# Eval("ShoppingCartLineID") %>' CommandName="CopyLine">COPY</asp:LinkButton>
                                </ItemTemplate>
                            </asp:TemplateField>
                            <asp:TemplateField HeaderText="Item Total" ItemStyle-HorizontalAlign="Center" HeaderStyle-HorizontalAlign="Center">
                                <ItemTemplate>
                                    <h2 class="td-color"><%# string.Format("{0:c}", Eval("LineTotal")) %></h2>
                                </ItemTemplate>
                            </asp:TemplateField>
                        </Columns>
                    </asp:GridView>
                    <div class="table-responsive-md">
                        <table class="table cart-table">
                            <tfoot>
                                <asp:Panel ID="pnlCompanyInvoicedAmount" runat="server">
                                    <tr>
                                        <td><asp:Literal ID="litCompanyInvoiced" runat="server" Text="Company Invoiced :"></asp:Literal></td>
                                        <td>
                                            <h2><asp:Label ID="lblCompanyInvoicedAmount" runat="server"></asp:Label></h2>
                                        </td>
                                    </tr>
                                </asp:Panel>

                                <asp:Panel ID="pnlDiscount" runat="server">
                                    <tr>
                                        <td><asp:Literal ID="litDiscountLabel" runat="server" Text="Discount :"></asp:Literal></td>
                                        <td>
                                            <h2><asp:Label ID="lblDisocuntAmount" runat="server"></asp:Label></h2>
                                        </td>
                                    </tr>
                                </asp:Panel>
                                <tr>

                                <tr>
                                    <td><asp:LIteral ID="litOrderTotal" runat="server" Text="Order Total :"></asp:LIteral></td>
                                    <td>
                                        <h2><asp:Label ID="lblTotal" runat="server"></asp:Label></h2>
                                    </td>
                                </tr>

                                <asp:Panel ID="pnlEstimatedShipping" runat="server" Visible ="false">
                                    <tr>
                                        <td style="text-align:right;" colspan="2">
                                            <asp:Literal ID="litEstimatedShipping" runat="server" Text="Estimated Shipping :"></asp:Literal>
                                        </td>
                                    </tr>
                                </asp:Panel>


                                <tr>
                                    <td></td>
                                    <td style="display:none">
                                        <asp:CheckBox ID="chkAdvanced" runat="server" Text="Batch/Group Order" EnableViewState="true" AutoPostBack="true" OnCheckedChanged="chkAdvanced_CheckedChanged" />
                                    </td>
                                </tr>
                            </tfoot>
                        </table>
                    </div>
                </div>
            </div>
            <div class="col-12">
                <asp:Label ID="lblOrderMessage" runat="server" Text ="Please review your order carefully before hitting submit.  Once your order has been placed, we will not be able to update or change" ForeColor="Red"></asp:Label>
            </div>
            <div class="row cart-buttons">

                <div class="col-6"><asp:Button id="btnContinueShopping" runat="server" CssClass="btn btn-solid" Text="Continue Shopping" OnClick="btnContinueShopping_Click" CausesValidation="false" /></div>
                <div class="col-6"><asp:Button id="btnUpdateCart" runat="server" CssClass="btn btn-solid" Text="Update Cart" OnClick="btnUpdateCart_Click" CausesValidation="false" /> <asp:Button id="btnCheckOut" runat="server" CssClass="btn btn-solid" Text="Check Out" OnClick="btnCheckOut_Click" CausesValidation="false" /> <asp:Button id="btnTransfer" runat="server" CssClass="btn btn-solid" Text="Transfer" OnClick="btnTransfer_Click" CausesValidation="false" Visible="false"/></div>
            </div>
        </div>
    </section>
    <!--section end-->
</asp:Content>