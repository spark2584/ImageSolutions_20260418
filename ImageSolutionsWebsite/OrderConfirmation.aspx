<%@ Page Title="" Language="C#" MasterPageFile="~/MasterPage.Master" AutoEventWireup="true" CodeBehind="OrderConfirmation.aspx.cs" Inherits="ImageSolutionsWebsite.OrderConfirmation" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="cphHeader" runat="server">
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="cphBody" runat="server">
    <!-- thank-you section start -->
    <section class="section-b-space light-layout">
        <div class="container">
            <div class="row">
                <div class="col-md-12">
                    <div class="success-text">
                        <div class="checkmark">
                            <svg class="star" height="19" viewBox="0 0 19 19" width="19"
                                xmlns="http://www.w3.org/2000/svg">
                                <path
                                    d="M8.296.747c.532-.972 1.393-.973 1.925 0l2.665 4.872 4.876 2.66c.974.532.975 1.393 0 1.926l-4.875 2.666-2.664 4.876c-.53.972-1.39.973-1.924 0l-2.664-4.876L.76 10.206c-.972-.532-.973-1.393 0-1.925l4.872-2.66L8.296.746z">
                                </path>
                            </svg>
                            <svg class="star" height="19" viewBox="0 0 19 19" width="19"
                                xmlns="http://www.w3.org/2000/svg">
                                <path
                                    d="M8.296.747c.532-.972 1.393-.973 1.925 0l2.665 4.872 4.876 2.66c.974.532.975 1.393 0 1.926l-4.875 2.666-2.664 4.876c-.53.972-1.39.973-1.924 0l-2.664-4.876L.76 10.206c-.972-.532-.973-1.393 0-1.925l4.872-2.66L8.296.746z">
                                </path>
                            </svg>
                            <svg class="star" height="19" viewBox="0 0 19 19" width="19"
                                xmlns="http://www.w3.org/2000/svg">
                                <path
                                    d="M8.296.747c.532-.972 1.393-.973 1.925 0l2.665 4.872 4.876 2.66c.974.532.975 1.393 0 1.926l-4.875 2.666-2.664 4.876c-.53.972-1.39.973-1.924 0l-2.664-4.876L.76 10.206c-.972-.532-.973-1.393 0-1.925l4.872-2.66L8.296.746z">
                                </path>
                            </svg>
                            <svg class="star" height="19" viewBox="0 0 19 19" width="19"
                                xmlns="http://www.w3.org/2000/svg">
                                <path
                                    d="M8.296.747c.532-.972 1.393-.973 1.925 0l2.665 4.872 4.876 2.66c.974.532.975 1.393 0 1.926l-4.875 2.666-2.664 4.876c-.53.972-1.39.973-1.924 0l-2.664-4.876L.76 10.206c-.972-.532-.973-1.393 0-1.925l4.872-2.66L8.296.746z">
                                </path>
                            </svg>
                            <svg class="star" height="19" viewBox="0 0 19 19" width="19"
                                xmlns="http://www.w3.org/2000/svg">
                                <path
                                    d="M8.296.747c.532-.972 1.393-.973 1.925 0l2.665 4.872 4.876 2.66c.974.532.975 1.393 0 1.926l-4.875 2.666-2.664 4.876c-.53.972-1.39.973-1.924 0l-2.664-4.876L.76 10.206c-.972-.532-.973-1.393 0-1.925l4.872-2.66L8.296.746z">
                                </path>
                            </svg>
                            <svg class="star" height="19" viewBox="0 0 19 19" width="19"
                                xmlns="http://www.w3.org/2000/svg">
                                <path
                                    d="M8.296.747c.532-.972 1.393-.973 1.925 0l2.665 4.872 4.876 2.66c.974.532.975 1.393 0 1.926l-4.875 2.666-2.664 4.876c-.53.972-1.39.973-1.924 0l-2.664-4.876L.76 10.206c-.972-.532-.973-1.393 0-1.925l4.872-2.66L8.296.746z">
                                </path>
                            </svg>
                            <svg class="checkmark__check" height="36" viewBox="0 0 48 36" width="48"
                                xmlns="http://www.w3.org/2000/svg">
                                <path
                                    d="M47.248 3.9L43.906.667a2.428 2.428 0 0 0-3.344 0l-23.63 23.09-9.554-9.338a2.432 2.432 0 0 0-3.345 0L.692 17.654a2.236 2.236 0 0 0 .002 3.233l14.567 14.175c.926.894 2.42.894 3.342.01L47.248 7.128c.922-.89.922-2.34 0-3.23">
                                </path>
                            </svg>
                            <svg class="checkmark__background" height="115" viewBox="0 0 120 115" width="120"
                                xmlns="http://www.w3.org/2000/svg">
                                <path
                                    d="M107.332 72.938c-1.798 5.557 4.564 15.334 1.21 19.96-3.387 4.674-14.646 1.605-19.298 5.003-4.61 3.368-5.163 15.074-10.695 16.878-5.344 1.743-12.628-7.35-18.545-7.35-5.922 0-13.206 9.088-18.543 7.345-5.538-1.804-6.09-13.515-10.696-16.877-4.657-3.398-15.91-.334-19.297-5.002-3.356-4.627 3.006-14.404 1.208-19.962C10.93 67.576 0 63.442 0 57.5c0-5.943 10.93-10.076 12.668-15.438 1.798-5.557-4.564-15.334-1.21-19.96 3.387-4.674 14.646-1.605 19.298-5.003C35.366 13.73 35.92 2.025 41.45.22c5.344-1.743 12.628 7.35 18.545 7.35 5.922 0 13.206-9.088 18.543-7.345 5.538 1.804 6.09 13.515 10.696 16.877 4.657 3.398 15.91.334 19.297 5.002 3.356 4.627-3.006 14.404-1.208 19.962C109.07 47.424 120 51.562 120 57.5c0 5.943-10.93 10.076-12.668 15.438z">
                                </path>
                            </svg>
                        </div>
                        <h2>thank you</h2>
                        <p><asp:Literal ID="litPaymentMessage" runat="server">Payment is successfully processed and your order is on the way</asp:Literal></p>
                        <p class="font-weight-bold"><asp:Literal ID="litTransactionID" runat="server"></asp:Literal></p>
                    </div>
                </div>
            </div>
        </div>
    </section>
    <!-- Section ends -->

    <!-- order-detail section start -->
    <section class="section-b-space">
        <div class="container">
            <div class="row">
                <div class="col-lg-6">
                    <asp:GridView ID="gvSalesOrderLine" runat="server" AutoGenerateColumns="False" DataKeyNames="SalesOrderLineID" CssClass="table cart-table" HeaderStyle-CssClass="table-head" Width="100%" 
                        GridLines="None" AllowSorting="true" CellSpacing="0" CellPadding="0" OnRowCommand="gvSalesOrderLine_RowCommand">
                        <Columns>
                            <asp:TemplateField HeaderText="Item" ItemStyle-HorizontalAlign="Left" HeaderStyle-HorizontalAlign="Left">
                                <ItemTemplate>
                                    <li ID="ItemNumber">
                                        <%--<%# string.Format("{0:c}", Eval("Item.ItemNumber"))%> <%# Eval("UserInfo") == null ? string.Empty : "(" + Eval("UserInfo.FullName") + ")" %>--%>
                                        <%# (!string.IsNullOrEmpty(Convert.ToString(Eval("SKU"))) ? Eval("SKU") : Eval("Item.ItemNumber")) %> <%# Eval("UserInfo") == null ? string.Empty : "(" + Eval("UserInfo.FullName") + ")" %>

                                    </li>
                                </ItemTemplate>
                            </asp:TemplateField>
                            <asp:TemplateField HeaderText="Description" ItemStyle-HorizontalAlign="Left" HeaderStyle-HorizontalAlign="Left">
                                <ItemTemplate>
                                    <li ID="ItemDescription">
                                       <%-- <%# Eval("Item.SalesDescription")%>--%>
                                        <%# (!string.IsNullOrEmpty(Convert.ToString(Eval("Description"))) ? Eval("Description") : Eval("Item.SalesDescription")) %>
                                    </li>
                                </ItemTemplate>
                            </asp:TemplateField>
                            <asp:TemplateField HeaderText="Options" ItemStyle-HorizontalAlign="Left" HeaderStyle-HorizontalAlign="Left">
                                <ItemTemplate>
                                 <%--   <%# string.IsNullOrEmpty(Convert.ToString(Eval("UserInfo"))) ? string.Empty : "User: " + Eval("UserInfo.FullName") + "<br />" %>
                                    <%# string.IsNullOrEmpty(Convert.ToString(Eval("CustomListValueID_1"))) ? string.Empty : Eval("CustomListValue_1.CustomList.ListName") + ": " + Eval("CustomListValue_1.ListValue") + "<br />" %>
                                    <%# string.IsNullOrEmpty(Convert.ToString(Eval("CustomListValueID_2"))) ? string.Empty : Eval("CustomListValue_2.CustomList.ListName") + ": " + Eval("CustomListValue_2.ListValue") + "<br />" %>
                                    <%# (Convert.ToBoolean(Eval("Item.IsNonInventory")) || !string.IsNullOrEmpty(Convert.ToString(Eval("CustomListValueID_1"))) || !string.IsNullOrEmpty(Convert.ToString(Eval("CustomListValueID_2"))) ? "<font style='color:red;'>(This is a custom item. You will receive it in 12 business days.)</font>" : String.Empty) %>
--%>



                                    <%--<%# Eval("Item.ItemNumber")%><br /><%# Eval("Item.StoreDisplayName")%><br />--%>
                                    <%--<%# Eval("Item.ItemNumber")%>--%>
                                    <%# (!string.IsNullOrEmpty(Convert.ToString(Eval("SKU"))) ? Eval("SKU") : Eval("Item.ItemNumber")) %><br /><%# (!string.IsNullOrEmpty(Convert.ToString(Eval("Description"))) ? Eval("Description") : Eval("Item.SalesDescription")) %><br />    <%--<%# Eval("Item.SalesDescription")%><br />--%>
                                    <%# (!string.IsNullOrEmpty(Convert.ToString(Eval("CustomDesignName"))) ? "<font style='color:blue;'>(This is a personalized item, Name = " + Eval("CustomDesignName") + ")</font><br/>" : String.Empty) %>
                                    <%# (!string.IsNullOrEmpty(Convert.ToString(Eval("LogosDescription"))) ? "<font style='color:blue;'> " + Eval("LogosDescription") + " </font><br/>" : String.Empty) %>
                                    <%# (!string.IsNullOrEmpty(Convert.ToString(Eval("PersonalizationDescription"))) ? "<font style='color:blue;'> " + Eval("PersonalizationDescription") + " </font><br/>" : String.Empty) %>
                                    <%# (
                                            (
                                                Convert.ToBoolean(Eval("Item.IsNonInventory")) 
                                                || !string.IsNullOrEmpty(Convert.ToString(Eval("CustomListValueID_1"))) 
                                                || !string.IsNullOrEmpty(Convert.ToString(Eval("CustomListValueID_2"))) 
                                                || Convert.ToBoolean(Eval("HasCusomtization")) 
                                            ) && ( Convert.ToString(Eval("SalesOrder.WebsiteID")) != "9" || Convert.ToString(Eval("Item.ItemType")) != "_inventoryItem" )
                                                ? "<font style='color:red;'>(This is a custom item. You will receive it in 12 business days.)</font>" 
                                                : String.Empty) %>

                                </ItemTemplate>
                            </asp:TemplateField>
                            <asp:TemplateField HeaderText="Price" ItemStyle-HorizontalAlign="Left" HeaderStyle-HorizontalAlign="Left">
                                <ItemTemplate>
                                    <li ID="ItemPrice"> 
                                        <%# Convert.ToBoolean(Eval("SalesOrder.DisplayTariffCharge")) ? string.Format("{0:c}", Eval("OnlinePrice")) : string.Format("{0:c}", Eval("UnitPrice"))%>
                                    </li>
                                </ItemTemplate>
                            </asp:TemplateField>
                            <asp:TemplateField HeaderText="Tariff Surcharge" Visible="false" ItemStyle-HorizontalAlign="Left" HeaderStyle-HorizontalAlign="Left">
                                <ItemTemplate>
                                    <li ID="ItemPrice"> 
                                        <%# string.Format("{0:c}", Eval("TariffCharge"))%>
                                    </li>
                                </ItemTemplate>
                            </asp:TemplateField>
                            <asp:TemplateField HeaderText="Quantity" ItemStyle-HorizontalAlign="Left" HeaderStyle-HorizontalAlign="Left">
                                <ItemTemplate>
                                    <li ID="ItemQuantity">
                                        <%# Eval("Quantity")%>
                                    </li>
                                </ItemTemplate>
                            </asp:TemplateField>
                        </Columns>
                    </asp:GridView>
                </div>
                <div class="col-lg-6">
                    <div class="order-success-sec">
                        <div class="row">
                            <div class="col-sm-6">
                                <h4>summary</h4>
                                <ul class="order-detail">
                                    <li>Order Number: <asp:Label ID="lblOrderNumber" runat="server"></asp:Label></li>
                                    <li>Order Date: <asp:Label ID="lblOrderDate" runat="server"></asp:Label></li>
                                    <li>Subtotal: <asp:Label ID="lblSubtotal" runat="server"></asp:Label></li>
                                    <li>Shipping: <%--<asp:Label ID="lblShipping" runat="server"></asp:Label>--%><asp:Literal ID="litShipping" runat="server"></asp:Literal></li>
                                    <li>Tax: <%--<asp:Label ID="lblTax" runat="server"></asp:Label>--%><asp:Literal ID="litTax" runat="server"></asp:Literal></li>
                                    <li>Discount: <asp:Label ID="lblDiscountAmount" runat="server"></asp:Label></li>
                                    <li>Order Total: <asp:Label ID="lblTotal" runat="server"></asp:Label></li>
                                    <li>Status: <asp:Label ID="lblStatus" runat="server"></asp:Label></li>
                                    <li>Reference Number: <asp:Label ID="lblReferenceNumber" runat="server"></asp:Label></li>
                                    <asp:Panel ID="pnlPONumber" runat="server">
                                        <li>PO Number: <asp:Label ID="lblPONumber" runat="server"></asp:Label></li>
                                    </asp:Panel>
                                </ul>
                                <br />
                            </div>
                            <div class="col-sm-6">
                                <h4>shipping address</h4>
                                <ul class="order-detail">
                                    <li id="liShippingName" runat="server"></li>
                                    <li id="liShippingAddress" runat="server"></li>
                                    <li id="liShippingAddress2" runat="server"></li>
                                    <li id="liShippingCityandState" runat="server"></li>
                                    <li id="liShippingNumber" runat="server"></li>
                                </ul>
                            </div>
                            <div class="col-sm-6">
                                <h4>payment method</h4>
                                <p><asp:Label ID="lblPayment" runat="server"></asp:Label></p>
                            </div>
                            <div class="col-sm-6">
                                <div style="display:none;">
                                    <h4>billing address</h4>
                                    <ul class="order-detail">
                                        <li id="liBillingName" runat="server"></li>
                                        <li id="liBillingAddress" runat="server"></li>
                                        <li id="liBillingCityandState" runat="server"></li>
                                        <li id="liBillingNumber" runat="server"></li>
                                    </ul>
                                 </div>
                            </div>
                            <div class="col-sm-6">
                                <h4>shipping method</h4>
                                <p><asp:Label ID="lblShippingMethod" runat="server"></asp:Label></p>
                            </div>
                            <%--<div class="col-md-12">
                                <div class="delivery-sec">
                                    <h3>expected date of delivery: <span>october 22, 2018</span></h3>
                                    <!--<a href="order-tracking.html">track order</a>-->
                                </div>
                            </div>--%>

                            <asp:Panel ID="pnlRejectionReason" runat="server" class="col-sm-6">
                                <h4>Reason for Rejection</h4>
                                <p><asp:Label ID="lblRejectionReason" runat="server"></asp:Label></p>
                            </asp:Panel>
                        </div>
                    </div>
                </div>
                <div class="col-lg-12">
                    <asp:GridView ID="gvFulfillment" runat="server" AutoGenerateColumns="False" DataKeyNames="FulfillmentID" CssClass="table cart-table" HeaderStyle-CssClass="table-head" Width="100%" 
                        GridLines="None" AllowSorting="true" CellSpacing="0" CellPadding="0">
                        <Columns>
                            <asp:TemplateField HeaderText="Date Shipped" ItemStyle-HorizontalAlign="Left" HeaderStyle-HorizontalAlign="Left">
                                <ItemTemplate>
                                    <%# string.Format("{0:MM/dd/yyyy}", Eval("ShipDate"))%>
                                </ItemTemplate>
                            </asp:TemplateField>
                            <asp:TemplateField HeaderText="Shipment Number" ItemStyle-HorizontalAlign="Left" HeaderStyle-HorizontalAlign="Left">
                                <ItemTemplate>
                                    <%# Eval("FulfillmentNumber")%>
                                </ItemTemplate>
                            </asp:TemplateField>
                            <asp:TemplateField HeaderText="Carrier" ItemStyle-HorizontalAlign="Left" HeaderStyle-HorizontalAlign="Left">
                                <ItemTemplate>
                                    <%# Eval("ShippingCarrier")%>
                                </ItemTemplate>
                            </asp:TemplateField>
                            <asp:TemplateField HeaderText="Tracking Number" ItemStyle-HorizontalAlign="Left" HeaderStyle-HorizontalAlign="Left">
                                <ItemTemplate>
                                    <%# Eval("TrackingNumber")%>
                                </ItemTemplate>
                            </asp:TemplateField>
                        </Columns>
                    </asp:GridView>
                </div>

                <div class="col-lg-12">
                    <asp:GridView ID="gvRMA" runat="server" AutoGenerateColumns="False" DataKeyNames="RMAID" CssClass="table cart-table" HeaderStyle-CssClass="table-head" Width="100%" 
                        GridLines="None" AllowSorting="true" CellSpacing="0" CellPadding="0" OnRowDataBound="gvRMA_RowDataBound" OnRowCommand="gvRMA_RowCommand">
                        <Columns>
                            <asp:TemplateField HeaderText="Date Requested" ItemStyle-HorizontalAlign="Left" HeaderStyle-HorizontalAlign="Left">
                                <ItemTemplate>
                                    <%# string.Format("{0:MM/dd/yyyy}", Eval("CreatedOn"))%>
                                </ItemTemplate>
                            </asp:TemplateField>
                            <asp:TemplateField HeaderText="RMA ID" ItemStyle-HorizontalAlign="Left" HeaderStyle-HorizontalAlign="Left">
                                <ItemTemplate>
                                    <%# Eval("RMAID")%>
                                </ItemTemplate>
                            </asp:TemplateField>
                            <asp:TemplateField HeaderText="Label">
                                <ItemTemplate>
                                    <asp:Panel ID="pnlShippingLabel" runat="server">
                                        <a href='<%# Eval("ShippingLabelPath") %>' target="_blank">
                                            <i class="fa fa-envelope text-theme"></i>
                                        </a>
                                    </asp:Panel>
                                </ItemTemplate>
                            </asp:TemplateField>

                            <asp:TemplateField HeaderText="Delete">
                                <ItemTemplate>
                                    <asp:LinkButton id="btnDelete" runat="server" CommandArgument='<%# Eval("RMAID") %>' CommandName="DeleteRMA"><i class="ti-trash" style="font-size:x-large"></i></asp:LinkButton>
                                </ItemTemplate>
                            </asp:TemplateField>

                        </Columns>
                    </asp:GridView>
                </div>

            </div>
        </div>
    </section>

    <asp:HiddenField ID="hfDeleteMessage" runat="server" />

    <!-- Section ends -->

    <script>
        window.dataLayer = window.dataLayer || [];
        dataLayer.push({ ecommerce: null });

        var items = [];

        var itemNumbers = document.querySelectorAll('#ItemNumber');
        var itemNames = document.querySelectorAll('#ItemDescription');
        var itemPrices = document.querySelectorAll('#ItemPrice');
        var itemQuantities = document.querySelectorAll('#ItemQuantity');

        for (var i = 0; i < itemNumbers.length; i++) {
            items.push({
                item_id: itemNumbers[i].innerText.trim(),
                item_name: (function (text) {
                    var firstDashIndex = text.indexOf('-');
                    var lastDashIndex = text.lastIndexOf('-');
                    return (firstDashIndex !== lastDashIndex) ?
                        text.substring(0, text.lastIndexOf('-', text.lastIndexOf('-') - 1)) :
                        text.substring(0, firstDashIndex);
                })(itemNames[i].innerText.trim()),
                price: parseFloat(itemPrices[i].innerText.trim().substring(1)),
                quantity: parseFloat(itemQuantities[i].innerText.trim())
            });
        }

        dataLayer.push({
            event: 'purchase',
            ecommerce: {
                transaction_id: document.getElementById('cphBody_lblOrderNumber').innerText.trim(),
                shipping: parseFloat(document.getElementById('spanShipping').innerText.trim().substring(1)),
                tax: parseFloat(document.getElementById('spanTax').innerText.trim().substring(1)),
                value: parseFloat(document.getElementById('cphBody_lblTotal').innerText.trim().substring(1)),
                currency: 'USD',
                items: items
            }
        });
    </script>

</asp:Content>
