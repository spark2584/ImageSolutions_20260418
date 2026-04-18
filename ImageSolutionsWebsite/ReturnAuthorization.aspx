<%@ Page Language="C#" MasterPageFile="~/MasterPage.Master" AutoEventWireup="true" CodeBehind="ReturnAuthorization.aspx.cs" Inherits="ImageSolutionsWebsite.ReturnAuthorization" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="cphHeader" runat="server">
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="cphBody" runat="server">

    <!-- order-detail section start -->
    <section class="section-b-space">
        <div class="container">
            <div class="row">         
                <div class="col-lg-12">
                    <h3>Return</h3>
                </div>

                <div class="col-lg-12">
                    <div class="order-success-sec">
                        <div class="row">
                            <div class="col-sm-6">
                                <h4>summary</h4>
                                <ul class="order-detail">
                                    <li>Order Number: <asp:Label ID="lblOrderNumber" runat="server"></asp:Label></li>
                                    <li>Order Date: <asp:Label ID="lblOrderDate" runat="server"></asp:Label></li>
                                    <%--<li>Subtotal: <asp:Label ID="lblSubtotal" runat="server"></asp:Label></li>
                                    <li>Shipping: <asp:Literal ID="litShipping" runat="server"></asp:Literal></li>
                                    <li>Tax: <asp:Literal ID="litTax" runat="server"></asp:Literal></li>
                                    <li>Order Total: <asp:Label ID="lblTotal" runat="server"></asp:Label></li>
                                    <li>Status: <asp:Label ID="lblStatus" runat="server"></asp:Label></li>
                                    <li>Reference Number: <asp:Label ID="lblReferenceNumber" runat="server"></asp:Label></li>
                                    <asp:Panel ID="pnlPONumber" runat="server">
                                        <li>PO Number: <asp:Label ID="lblPONumber" runat="server"></asp:Label></li>
                                    </asp:Panel>--%>
                                </ul>
                                <br />
                            </div>
                            <div class="col-sm-6">
                                <h4>delivery address</h4>
                                <ul class="order-detail">
                                    <li id="liShippingName" runat="server"></li>
                                    <li id="liShippingAddress" runat="server"></li>
                                    <li id="liShippingAddress2" runat="server"></li>
                                    <li id="liShippingCityandState" runat="server"></li>
                                    <li id="liShippingNumber" runat="server"></li>
                                </ul>
                            </div>
<%--                            <div class="col-sm-6">
                                <h4>payment method</h4>
                                <p><asp:Label ID="lblPayment" runat="server"></asp:Label></p>
                            </div>--%>
<%--                            <div class="col-sm-6">
                                <div style="display:none;">
                                    <h4>billing address</h4>
                                    <ul class="order-detail">
                                        <li id="liBillingName" runat="server"></li>
                                        <li id="liBillingAddress" runat="server"></li>
                                        <li id="liBillingCityandState" runat="server"></li>
                                        <li id="liBillingNumber" runat="server"></li>
                                    </ul>
                                 </div>
                            </div>--%>
<%--                            <div class="col-sm-6">
                                <h4>shipping method</h4>
                                <p><asp:Label ID="lblShippingMethod" runat="server"></asp:Label></p>
                            </div>--%>
                        </div>
                    </div>
                </div>


                <div class="col-lg-12">
                    <asp:GridView ID="gvSalesOrderLine" runat="server" AutoGenerateColumns="False" DataKeyNames="SalesOrderLineID" CssClass="table cart-table" HeaderStyle-CssClass="table-head" Width="100%" 
                        GridLines="None" AllowSorting="true" CellSpacing="0" CellPadding="0" OnRowCommand="gvSalesOrderLine_RowCommand">
                        <Columns>
                            <asp:TemplateField HeaderText="Item" ItemStyle-HorizontalAlign="Left" HeaderStyle-HorizontalAlign="Left">
                                <ItemTemplate>
                                    <asp:Label ID="lblItemNumber" runat="server" Text='<%# Eval("Item.ItemNumber")%>'></asp:Label>
                                </ItemTemplate>
                            </asp:TemplateField>
                            <asp:TemplateField HeaderText="Description" ItemStyle-HorizontalAlign="Left" HeaderStyle-HorizontalAlign="Left">
                                <ItemTemplate>
                                    <li ID="ItemDescription">
                                        <%# Eval("Item.SalesDescription")%>
                                    </li>
                                </ItemTemplate>
                            </asp:TemplateField>
                            <asp:TemplateField HeaderText="Options" ItemStyle-HorizontalAlign="Left" HeaderStyle-HorizontalAlign="Left">
                                <ItemTemplate>
                                    <%# Eval("Item.ItemNumber")%><br /><%# Eval("Item.SalesDescription")%><br />
                                    <%# (!string.IsNullOrEmpty(Convert.ToString(Eval("CustomDesignName"))) ? "<font style='color:blue;'>(This is a personalized item, Name = " + Eval("CustomDesignName") + ")</font><br/>" : String.Empty) %>
                                    <%# (!string.IsNullOrEmpty(Convert.ToString(Eval("LogosDescription"))) ? "<font style='color:blue;'> " + Eval("LogosDescription") + " </font><br/>" : String.Empty) %>
                                    <%# (!string.IsNullOrEmpty(Convert.ToString(Eval("PersonalizationDescription"))) ? "<font style='color:blue;'> " + Eval("PersonalizationDescription") + " </font><br/>" : String.Empty) %>

                                </ItemTemplate>
                            </asp:TemplateField>
                            <asp:TemplateField HeaderText="Price" ItemStyle-HorizontalAlign="Left" HeaderStyle-HorizontalAlign="Left">
                                <ItemTemplate>
                                    <li ID="ItemPrice"> 
                                        <%# string.Format("{0:c}", Eval("UnitPrice"))%>
                                    </li>
                                </ItemTemplate>
                            </asp:TemplateField>
                            <asp:TemplateField HeaderText="Quantity" ItemStyle-HorizontalAlign="Left" HeaderStyle-HorizontalAlign="Left">
                                <ItemTemplate>
                                    <li ID="ItemQuantity">
                                        <%# Eval("RMAAvailableQuantity")%>
                                    </li>
                                </ItemTemplate>
                            </asp:TemplateField>

                            <asp:TemplateField HeaderText="Return Quantity" ItemStyle-HorizontalAlign="Center" HeaderStyle-HorizontalAlign="Center">
                                <ItemTemplate>
                                    <div class="qty-box">
                                        <div class="input-group">
                                            <asp:TextBox ID="txtReturnQuantity" runat="server" type="number" CssClass="form-control input-number" min="0"></asp:TextBox>
                                        </div>
                                    </div>
                                </ItemTemplate>
                            </asp:TemplateField>

                        </Columns>
                    </asp:GridView>
                    <br />
                </div>
                <div class="col-lg-6" style="text-align: right;">
                </div>
                <div class="col-lg-3" style="text-align: right;">
                    <%--Reason:--%>
                </div> 
                <div class="col-lg-3" style="text-align: right;">
                    <asp:DropDownList ID="ddlReason" AutoPostBack="true" OnSelectedIndexChanged="ddlReason_SelectedIndexChanged" CssClass="form-control" runat="server">           
<%--                        <asp:ListItem Value="Customer" Selected="True">Customer</asp:ListItem>                                                                    
                        <asp:ListItem Value="ImageSolutions">Image Solutions</asp:ListItem>--%>
                    </asp:DropDownList>
                    <br />
                </div>
                
                <div class="col-lg-12" style="text-align: right;">
                    <ul>
                        <li>
                            <div class="radio-option">
                                <asp:RadioButton ID="rbnUseMyOwn" runat="server" GroupName="Label" Text="Use My Own Return Label" Checked="true" AutoPostBack="true" OnCheckedChanged="rbnUseMyOwn_CheckedChanged" />
                            </div>
                            <div class="radio-option">
                                <br />
                                <asp:RadioButton ID="rbnRequestReturnLabel" runat="server" GroupName="Label" Text="Request Return Label" AutoPostBack="true" OnCheckedChanged="rbnRequestReturnLabel_CheckedChanged" />
<%--                                <asp:Panel ID="pnlRequestReturnLabel" runat="server" Visible="false">
                                    <br />
                                    <asp:Button ID="btnRequestShippingLabel" runat="server" Text="Request Shipping Label" OnClick="btnRequestShippingLabel_Click" CssClass="btn btn-solid" />
                                    <asp:HyperLink ID="hlnkRequestShippingLabel" runat="server" Visible="false" Target="_blank"></asp:HyperLink>
                                    <br />
                                </asp:Panel>--%>
                                <asp:Panel ID="pnlShippingCost" runat="server" Visible="false">
                                    <asp:Literal ID="litShippingCost" runat="server" Text="Shipping Cost: "></asp:Literal>
                                    <br />(+ tax) &nbsp;&nbsp;<span style="font-size: medium;" title="tax is calculated at the time of processing"><i class="ti-info-alt" ></i> </span>
                                </asp:Panel>
                                <asp:HiddenField ID="hfShippingCost" runat="server" />
                                
                            </div>
                        </li>
                    </ul>
                </div>

                <div class="col-lg-12" style="text-align: right;">
                    <br />
                    <asp:Button id="btnSubmit" runat="server" CssClass="btn btn-solid" Text="Submit" OnClick="btnSubmit_Click"/> 
                    <asp:HiddenField ID="hfSubmitMessage" runat="server" />
                </div>

                <asp:Panel ID="pnlUseMyOwn" runat="server" Visible="false">
                    <br />
                    <ul style="list-style-position: inside; padding-left:50px;">
                        <li style="list-style-position: inside;">1. Get a label from a carrier of your choice, we recommend using a carrier that will generate a tracking number.  Image Solutions will not be responsible for lost packages. Please ship back to<br />                          
Image Solutions - Returns<br />
4692 Brate Drive<br />
Suite 300<br />
Westchester, OH, 45011<br />
<br />
                        </li>
                        <li style="list-style-position: inside;">2. Affix label to your shipment so that the barcode is visible.</li>
                        <li style="list-style-position: inside;">3. Write Return Authorization # on outside of package</li>
                    </ul>
                    <br />
                </asp:Panel>


            </div>
        </div>
    </section>

</asp:Content>