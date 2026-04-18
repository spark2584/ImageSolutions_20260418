<%@ Page Title="" Language="C#" MasterPageFile="~/MasterPage.Master" AutoEventWireup="true" CodeBehind="CheckOut.aspx.cs" Inherits="ImageSolutionsWebsite.CheckOut" %>

<%@ Register Src="~/Control/Loading.ascx" TagPrefix="uc1" TagName="Loading" %>
<%@ Register src="Control/AccountSearchModal.ascx" tagname="AccountSearchModal" tagprefix="uc2"  %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <style type="text/css">
        .input-group > .form-control,
        .input-group > .aspNetDisabled,
        .input-group > input[type="text"] {
            display: block;
            flex: 1 1 auto !important;
            width: 1% !important; /* key: prevents full-width expansion */
            min-width: 0 !important;
        }
        .input-group > .input-group-text {
            display: flex !important;
            align-items: center !important;
            white-space: nowrap !important;
        }
    </style>

    <script src="/assets/js/jquery-3.3.1.min.js"></script>
    <script type="text/javascript">  
        if (performance.navigation.type == 2) {
            location.reload(true);
        }
    </script>
   <%-- <div id="ps_container_id"></div>  

    <asp:Literal ID="litPaystandScript" runat="server">
        <script
            type="text/javascript"
            id="ps_checkout"
            src="https://checkout.paystand.co/v4/js/paystand.checkout.js"
            ps-env="sandbox"
            ps-publishableKey="t4f464u6sbjtljj2w4wrrm1a"
            ps-containerId="ps_container_id"
            ps-mode="modal"
            ps-viewFunds="card"
            ps-checkoutType="checkout_payment"
            ps-paymentAmount="1.00"
            ps-fixedAmount="true"
            ps-payerName="Default"
            ps-payerEmail="default@imageinc.com"
            ps-payerAddressStreet="123 Street"
            ps-payerAddressCity="Torrance"
            ps-payerAddressPostal="92844"
            ps-payerAddressCountry="USA"
            ps-payerAddressState="CA"
            ps-customReceipt="Thank you for your order!<br>Your order confirmation number is ##"
        >
        </script>
    </asp:Literal>--%>

    <%--<script type="text/javascript">
        $(document).ready(function () {
            $("#ps_checkout").ready(function () {

                psCheckout.onceComplete((psEvent) => {
                    console.log('psEvent', psEvent);

                    const isClose = psEvent.response && psEvent.response.action == 'closeModal' && psEvent.response.type == 'checkoutEvent"';
                    if (isClose) {
                        return;
                    }

                    const isComplete = psEvent.response && psEvent.response.flow == 'payment' && psEvent.response.type == 'flowComplete';
                    if (isComplete) {
                        document.getElementById('<%= hfPaystandData.ClientID %>').value = JSON.stringify(psEvent.response);
                        document.getElementById('<%= btnPlaceOrderHidden.ClientID %>').click();
                    }

                    //} else {
                    //    console.log('ps once', status, isComplete, psEvent);
                    //}
                });

                //psCheckout.reboot({
                //    "checkoutType": "checkout_payment",
                //    "mode": "modal",
                //    // "viewFunds" : 'card',
                //    "paymentAmount": 1.00,
                //    "fixedAmount": "true",
                //    payerName: "Steve Park",
                //    payerEmail: "steve@imageinc.com",
                //    paymentExtId: "1",
                //    payerAddressStreet: "9912 Bria Ln",
                //    payerAddressCity: "Garden Grove",
                //    payerAddressState: "CA",
                //    payerAddressPostal: "92844",
                //    payerAddressCountry: 'US',
                //    customReceipt: 'Thank you for your order!<br>Your order confirmation number is ' + '1' + '<br> An email receipt will be sent to you from noreply@paystand.com.'

                //});
            })
        })
    </script>--%>
          
        
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="cphHeader" runat="server">
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="cphBody" runat="server">

    <!-- section start -->
    <section class="section-b-space">
        <div class="container">
            <div class="checkout-page">
                <div class="checkout-form">
                    <div class="row">
                        <div class="col-lg-6 col-sm-12 col-xs-12">
                            <asp:Repeater ID="rptCustomField" runat="server" DataMember="CustomFieldID" OnItemDataBound="rptCustomField_ItemDataBound">
                                <ItemTemplate>
                                    <div class="form-group col-md-12 col-sm-12 col-xs-12">

                                        <%--<div class="field-label"><asp:Label ID="lblLabel" runat="server" Text='<%# Eval("Name")%>'></asp:Label></div>--%>
                                        <asp:Panel ID="pnlLabel" runat="server" CssClass="field-label">
                                            <asp:Label ID="lblLabel" runat="server" Text='<%# Eval("Name")%>'></asp:Label>
                                        </asp:Panel>
                                        <asp:Literal ID="litLabelHTML" runat="server"></asp:Literal>

                                        <asp:TextBox ID="txtCustomValue" runat="server" Visible="true"></asp:TextBox>
                                        <asp:DropDownList ID="ddlCustomValueList" runat="server" DataTextField="Value" DataValueField="Value" Visible="false"></asp:DropDownList>
                                        <h6><asp:Label ID="lblHint" runat="server"></asp:Label></h6>
                                        <asp:HiddenField ID="hfCustomFieldID" runat="server" Value='<%# Eval("CustomFieldID")%>'/>
                                    </div>
                                </ItemTemplate>
                            </asp:Repeater>

                            <asp:Panel ID="pnlNotificationEmail" runat="server" class="form-group col-md-12 col-sm-12 col-xs-12" Visible="false">
                                <div CssClass="field-label">
                                    <asp:Label runat="server" ID="lblNotificationEmail" Text="Notification Email"></asp:Label>
                                </div>
                                <asp:TextBox ID="txtNotificationEmail" runat="server" Visible="true"></asp:TextBox>
                            </asp:Panel>


                            <asp:Panel ID="pnlShippingAddressMain" runat="server">
                                <div class="checkout-title">
                                    <h3>Shipping Address &nbsp;&nbsp;<%--<span style="font-size: medium;" title="Please note, we do not ship to PO Boxes"><i class="ti-info-alt" ></i> </span>--%> </h3>      
                                     <div style="color: red;">
                                        Please note that we are unable to ship orders to P.O. Boxes. <%--We are unable to ship to PO BOX--%>
                                    </div>
                                </div>
                               
                                <asp:Panel ID="pnlShipToAccount" runat="server" Visible ="false">
                                    <div class="col-md-12">
                                        <br />
                                        <h6 class="product-title">Store</h6>                                        
                                    </div>
                                    <div class="row">
                                        <div class="col-md-9">
                                            <%--<asp:DropDownList ID="ddlAccount" runat="server" Width="100%" DataValueField="AccountID" DataTextField="AccountName" CssClass="form-control form-select-sm form-select" AutoPostBack="true" OnSelectedIndexChanged="ddlAccount_SelectedIndexChanged"></asp:DropDownList>--%>
                                            <asp:TextBox ID="txtAccount" runat="server" CssClass="form-control" Enabled="false"></asp:TextBox>
                                            <asp:HiddenField ID="hfAccountID" runat="server" />
                                        </div>
                                        <div class="col-md-3" style="display: flex;align-items: center;">
                                            <asp:LinkButton ID="btnAccountSearch" runat="server" OnClick="btnAccountSearch_Click"><i class="ti-search" style="font-size:large"></i></asp:LinkButton>
                                        </div>
                                    </div>
                                </asp:Panel>

                                <asp:Panel ID="pnlExistingShippingAddress" runat="server" Visible="false" CssClass="row check-out">
                                    <div class="form-group col-md-12 col-sm-12 col-xs-12">
                                        <div class="field-label">Shipping Addresses</div>
                                        <asp:DropDownList ID="ddlShippingAddress" runat="server" OnSelectedIndexChanged="ddlShippingAddress_SelectedIndexChanged" AutoPostBack="true"></asp:DropDownList>
                                    </div>
                                    <div class="form-group col-md-12 col-sm-12 col-xs-12">
                                        <asp:Button ID="btnCancelShippingAddressSelect" runat="server" Text="Cancel" CssClass="btn btn-solid" OnClick="btnCancelShippingAddressSelect_Click" />
                                    </div>
                                </asp:Panel>
                                <asp:Panel ID="pnlShippingAddress" runat="server" Visible="false" CssClass="row check-out">
                                    <div class="form-group col-md-12 col-sm-12 col-xs-12" ignore>
                                        <asp:Literal ID="litShippingAddress" runat="server"></asp:Literal>                                    
                                    </div>
                                    <div class="form-group col-md-12 col-sm-12 col-xs-12">
                                    </div>
                                </asp:Panel>
                                <asp:Panel ID="pnlShippingAddressButton" runat="server" Visible="true" CssClass="row check-out">
                                    <div class="form-group col-md-12 col-sm-12 col-xs-12">
                                        <asp:Button ID="btnEditShippingAddress" runat="server" Text="Edit" CssClass="btn btn-solid" OnClick="btnEditShippingAddress_Click" />
                                        <asp:Button ID="btnNewShippingAddress" runat="server" Text="Enter New Address" CssClass="btn btn-solid" OnClick="btnNewShippingAddress_Click" />
                                        <asp:Button ID="btnChooseExistingShippingAddress" runat="server" Text="Choose Existing Address" CssClass="btn btn-solid" OnClick="btnChooseExistingShippingAddress_Click" />
                                    </div>
                                </asp:Panel>
                            </asp:Panel>

                            <asp:Panel ID="pnlEditShippingAddress" runat="server" Visible="false" CssClass="row check-out">
                                <div class="form-group col-md-12 col-sm-12 col-xs-12">
                                    <div class="field-label">Company Name</div>
                                    <input id="txtAddressLabel" runat="Server" type="text" name="field-name" value="">
                                </div>
                                <div class="form-group col-md-6 col-sm-6 col-xs-12">
                                    <div class="field-label">First Name</div>
                                    <input id="txtFirstName" runat="Server" type="text" name="field-name" value="" placeholder="" required="required">
                                </div>
                                <div class="form-group col-md-6 col-sm-6 col-xs-12">
                                    <div class="field-label">Last Name</div>
                                    <input id="txtLastName" runat="Server" type="text" name="field-name" value="" placeholder="">
                                </div>

                                <div class="form-group col-md-12 col-sm-12 col-xs-12">
                                    <div class="field-label">Phone</div>
                                    <input id="txtPhone" runat="Server" type="text" name="field-name" value="" placeholder="" required="">
                                </div>
                                <%--<div class="form-group col-md-6 col-sm-6 col-xs-12">
                                    <div class="field-label">Email Address</div>
                                    <input id="txtEmailAddress" runat="Server" type="text" name="field-name" value="" placeholder="">
                                </div>--%>
                                <div class="form-group col-md-12 col-sm-12 col-xs-12">
                                    <div class="field-label">Address</div>
                                    <input id="txtAddress" runat="Server" type="text" name="field-name" value="" placeholder="Street address" >
                                </div>
                                <div class="form-group col-md-12 col-sm-12 col-xs-12">
                                    <div class="field-label">Address 2 (Suite or Apt #)</div>
                                    <input id="txtAddress2" runat="Server" type="text" name="field-name" value="" placeholder="Street address 2" >
                                </div>
                                <div class="form-group col-md-12 col-sm-12 col-xs-12">
                                    <div class="field-label">Town/City</div>
                                    <input id="txtCity" runat="Server" type="text" name="field-name" value="" placeholder="">
                                </div>
                                <div class="form-group col-md-12 col-sm-6 col-xs-12">
                                    <div class="field-label">State</div>
                                    <input id="txtState" runat="Server" type="text" name="field-name" value="" placeholder="">
                                </div>
                                <div class="form-group col-md-12 col-sm-6 col-xs-12">
                                    <div class="field-label">Postal Code</div>
                                    <input id="txtPostalCode" runat="Server" type="text" name="field-name" value="" placeholder="">
                                </div>
                                <div class="form-group col-md-12 col-sm-6 col-xs-12">
                                    <div class="field-label">Country</div>
                                    <asp:DropDownList ID="ddlCountry" runat="server" DataTextField="Name" DataValueField="Alpha2Code" CssClass="form-control"></asp:DropDownList>
                                </div>
                                <asp:Panel ID="pnlSkipShippingAddressValidation" runat="server" Visible="false">
                                    <div class="form-group col-md-12 col-sm-12 col-xs-12">
                                        <div class="field-label">Skip Address Validation <asp:CheckBox ID="cbSkipShippingAddressValidation" runat="server" /></div>                                            
                                    </div>
                                </asp:Panel>
                                <div class="form-group col-md-12 col-sm-12 col-xs-12">
                                    <asp:Button ID="btnSaveShippingAddress" runat="server" Text="Save" CssClass="btn btn-solid" OnClick="btnSaveShippingAddress_Click" />
                                    <asp:Button ID="btnCancelShippingAddressSave" runat="server" Text="Cancel" CssClass="btn btn-solid" OnClick="btnCancelShippingAddressSave_Click" UseSubmitBehavior="false" CausesValidation="false"/>
                                </div>
                            </asp:Panel>
                            <br />
                            <asp:Panel ID="pnlBillingAddressWrap" runat="server" Visible="true">
                                <br />
                                <div class="checkout-title">
                                    <h3>Billing Address</h3>
                                </div>
                                <div class="row check-out">
                                    <div class="form-group col-md-12 col-sm-12 col-xs-12">
                                        <div class="field-label"><asp:CheckBox ID="cbSameAsShippingAddress" runat="server" OnCheckedChanged="cbSameAsShippingAddress_CheckedChanged" AutoPostBack="true"/>  Same as Shipping Address</div>                                            
                                    </div>
                                </div>
                                <asp:Panel ID="pnlExistingBillingAddress" runat="server" Visible="false" CssClass="row check-out">
                                    <div class="form-group col-md-12 col-sm-12 col-xs-12">
                                        <div class="field-label">Shipping Addresses</div>
                                        <asp:DropDownList ID="ddlBillingAddress" runat="server" OnSelectedIndexChanged="ddlBillingAddress_SelectedIndexChanged" AutoPostBack="true"></asp:DropDownList>
                                    </div>
                                    <div class="form-group col-md-12 col-sm-12 col-xs-12">
                                        <asp:Button ID="btnCancelBillingAddressSelect" runat="server" Text="Cancel" CssClass="btn btn-solid" OnClick="btnCancelBillingAddressSelect_Click" />
                                    </div>
                                </asp:Panel>
                                <asp:Panel ID="pnlBillingAddress" runat="server" Visible="false" CssClass="row check-out">
                                    <div class="form-group col-md-12 col-sm-12 col-xs-12" ignore>
                                        <asp:Literal ID="litBillingAddress" runat="server"></asp:Literal>                                    
                                    </div>
                                    <div class="form-group col-md-12 col-sm-12 col-xs-12">
                                    </div>
                                </asp:Panel>
                                <asp:Panel ID="pnlBillingAddressButton" runat="server" Visible="true" CssClass="row check-out">
                                    <div class="form-group col-md-12 col-sm-12 col-xs-12">
                                        <asp:Button ID="btnEditBillingAddress" runat="server" Text="Edit" CssClass="btn btn-solid" OnClick="btnEditBillingAddress_Click" />
                                        <asp:Button ID="btnNewBillingAddress" runat="server" Text="Enter New Address" CssClass="btn btn-solid" OnClick="btnNewBillingAddress_Click" />
                                        <asp:Button ID="btnChooseExistingBillingAddress" runat="server" Text="Choose Existing Address" CssClass="btn btn-solid" OnClick="btnChooseExistingBillingAddress_Click" />
                                    </div>
                                </asp:Panel>
                                <asp:Panel ID="pnlEditBillingAddress" runat="server" Visible="false" CssClass="row check-out">
                                    <div class="form-group col-md-12 col-sm-12 col-xs-12">
                                        <div class="field-label">Billing Addresses</div>
                                    </div>
                                    <div class="form-group col-md-12 col-sm-12 col-xs-12">
                                        <div class="field-label">Company Name</div>
                                        <input id="txtBillingAddressLabel" runat="Server" type="text" name="field-name" value="">
                                    </div>
                                    <div class="form-group col-md-6 col-sm-6 col-xs-12">
                                        <div class="field-label">First Name</div>
                                        <input id="txtBillingFirstName" runat="Server" type="text" name="field-name" value="" placeholder="" required="required">
                                    </div>
                                    <div class="form-group col-md-6 col-sm-6 col-xs-12">
                                        <div class="field-label">Last Name</div>
                                        <input id="txtBillingLastName" runat="Server" type="text" name="field-name" value="" placeholder="">
                                    </div>
                                    <div class="form-group col-md-12 col-sm-12 col-xs-12">
                                        <div class="field-label">Phone</div>
                                        <input id="txtBillingPhone" runat="Server" type="text" name="field-name" value="" placeholder="" required="">
                                    </div>
                                    <%--<div class="form-group col-md-6 col-sm-6 col-xs-12">
                                        <div class="field-label">Email Address</div>
                                        <input id="txtBillingEmailAddress" runat="Server" type="text" name="field-name" value="" placeholder="">
                                    </div>--%>
                                    <div class="form-group col-md-12 col-sm-12 col-xs-12">
                                        <div class="field-label">Address</div>
                                        <input id="txtBillingAddress" runat="Server" type="text" name="field-name" value="" placeholder="Street address" >
                                    </div>
                                    <div class="form-group col-md-12 col-sm-12 col-xs-12">
                                        <div class="field-label">Address 2</div>
                                        <input id="txtBillingAddress2" runat="Server" type="text" name="field-name" value="" placeholder="Street address 2" >
                                    </div>
                                    <div class="form-group col-md-12 col-sm-12 col-xs-12">
                                        <div class="field-label">Town/City</div>
                                        <input id="txtBillingCity" runat="Server" type="text" name="field-name" value="" placeholder="">
                                    </div>
                                    <div class="form-group col-md-12 col-sm-6 col-xs-12">
                                        <div class="field-label">State</div>
                                        <input id="txtBillingState" runat="Server" type="text" name="field-name" value="" placeholder="">
                                    </div>
                                    <div class="form-group col-md-12 col-sm-6 col-xs-12">
                                        <div class="field-label">Postal Code</div>
                                        <input id="txtBillingPostalCode" runat="Server" ype="text" name="field-name" value="" placeholder="">
                                    </div>
                                    <div class="form-group col-md-12 col-sm-6 col-xs-12">
                                        <div class="field-label">Country</div>
                                        <asp:DropDownList ID="ddlBillingCountry" runat="server" DataTextField="Name" DataValueField="Alpha2Code" CssClass="form-control"></asp:DropDownList>
                                    </div>
                                    <asp:Panel ID="pnlSkipBillingAddressValidation" runat="server" Visible="false">
                                        <div class="form-group col-md-12 col-sm-12 col-xs-12">
                                            <div class="field-label">Skip Address Validation <asp:CheckBox ID="cbSkipBillingAddressValidation" runat="server" /></div>                                            
                                        </div>
                                    </asp:Panel>
                                    <div class="form-group col-md-12 col-sm-12 col-xs-12">
                                        <asp:Button ID="btnSaveBillingAddress" runat="server" Text="Save" CssClass="btn btn-solid" OnClick="btnSaveBillingAddress_Click" />
                                        <asp:Button ID="btnCancelBillingAddressSave" runat="server" Text="Cancel" CssClass="btn btn-solid" OnClick="btnCancelBillingAddressSave_Click" UseSubmitBehavior="false" CausesValidation="false" />
                                    </div>
                                </asp:Panel>
                            </asp:Panel>
                            <br />
                            <asp:Panel ID="pnlCreateAccount" runat="server" Visible="false">
                                <br />
                                <div class="checkout-title">
                                    <h3>Create An Account? <asp:CheckBox ID="chkRegister" runat="server" AutoPostBack="true" OnCheckedChanged="chkRegister_CheckedChanged" /></h3>
                                </div>
                                <div id="divCreateAccountForm" runat="server" visible="false">
                                    <div class="form-group col-md-12 col-sm-12 col-xs-12">
                                        <div class="field-label">Email Address</div>
                                        <asp:TextBox ID="txtEmailAddress" runat="server" CssClass="form-control" placeholder="Email"></asp:TextBox>
                                        <asp:RegularExpressionValidator ID="RegularExpressionValidator1" runat="server" ControlToValidate="txtEmailAddress" ErrorMessage="Invalid Email Address" ValidationExpression="\w+([-+.]\w+)*@\w+([-.]\w+)*\.\w+([-.]\w+)*" ForeColor="Red" />
                                    </div>
                                    <div class="form-group col-md-6 col-sm-6 col-xs-12">
                                        <div class="field-label">Password</div>
                                        <asp:TextBox ID="txtPassword" runat="server" TextMode="Password" CssClass="form-control" placeholder="Enter your password"></asp:TextBox>
                                        <asp:RegularExpressionValidator ID="valPassword" runat="server" ControlToValidate="txtPassword" ValidationGroup="register" ErrorMessage="Password must be between 6 to 20 characters" ValidationExpression=".{6,20}.*" ForeColor="Red" />
                                    </div>
                                </div>
                            </asp:Panel>
                        </div>
                        <div class="col-lg-6 col-sm-12 col-xs-12">
                            <div class="checkout-details">                               
                                <div class="order-box">
                                    <div class="title-box">
                                        <div>Order Summary <span id="spanOrderTotal" runat="server">Order Total</span><asp:Literal ID="litPackage" runat="server"></asp:Literal></div>
                                    </div>
                                    <ul class="qty">
                                        <asp:Repeater ID="rptShoppingCart" runat="server">
                                            <ItemTemplate>
                                                <li>
                                                    <div class="row">
                                                        <div class="form-group col-9">
                                                            <%--<%# string.IsNullOrEmpty(Convert.ToString(Eval("Item.StoreDisplayName"))) ? Convert.ToString(Eval("Item.ItemName")) : Convert.ToString(Eval("Item.StoreDisplayName")) %> × <%# Eval("Quantity") %>--%>
                                                            <%# string.IsNullOrEmpty(Convert.ToString(Eval("Item.SalesDescription"))) ? Convert.ToString(Eval("Item.ItemName")) : Convert.ToString(Eval("Item.SalesDescription")) %> × <%# Eval("Quantity") %>
                                                            <%# string.IsNullOrEmpty(Convert.ToString(Eval("UserInfo"))) ? string.Empty : "<br />Employee: " + Eval("UserInfo.FullName") + "<br/>" %>
                                                            <%# string.IsNullOrEmpty(Convert.ToString(Eval("CustomListValueID_1"))) ? string.Empty : "<br />" + Eval("CustomListValue_1.CustomList.ListName") + ": " + Eval("CustomListValue_1.ListValue") %>
                                                            <%# string.IsNullOrEmpty(Convert.ToString(Eval("CustomListValueID_2"))) ? string.Empty : "<br />" + Eval("CustomListValue_2.CustomList.ListName") + ": " + Eval("CustomListValue_2.ListValue") %>
                                                            <%# (!string.IsNullOrEmpty(Convert.ToString(Eval("PersonalizationDescription"))) ? "<font style='color:blue;'> " + Eval("PersonalizationDescription") + " </font><br/>" : String.Empty) %>                                   
                                                            <%--<%# (Convert.ToBoolean(Eval("Item.IsNonInventory")) || !string.IsNullOrEmpty(Convert.ToString(Eval("CustomListValueID_1"))) || !string.IsNullOrEmpty(Convert.ToString(Eval("CustomListValueID_2"))) ? "<br/><font style='color:red;'>(This is a custom item. You will receive it in 12 business days.)</font>" : String.Empty) %>--%>
                                                            
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
                                                        </div>
                                                        <div class="form-group col-3">
                                                            <span id="spanLineTotal" runat="server"><%# string.Format("{0:c}", Eval("LineTotal"))%></span>
                                                        </div>
                                                    </div>

                                                </li>
                                            </ItemTemplate>
                                        </asp:Repeater>
                                    </ul>

                                    <asp:Panel ID="pnlCompanyInvoice" runat="server" Visible="false">
                                        <ul class="qty">
                                            <li><asp:Literal ID="litCompanyInvoicedTotalList" runat="server" Text="Company Invoiced:"></asp:Literal></li>
                                            <asp:Repeater ID="rptShoppingCartCompanyInvoice" runat="server">
                                                <ItemTemplate>
                                                    <li>
                                                        <div class="row">
                                                            <div class="form-group col-9">
                                                                <%--<%# string.IsNullOrEmpty(Convert.ToString(Eval("Item.StoreDisplayName"))) ? Convert.ToString(Eval("Item.ItemName")) : Convert.ToString(Eval("Item.StoreDisplayName")) %> × <%# Eval("Quantity") %>--%>
                                                                <%# string.IsNullOrEmpty(Convert.ToString(Eval("Item.SalesDescription"))) ? Convert.ToString(Eval("Item.ItemName")) : Convert.ToString(Eval("Item.SalesDescription")) %> × <%# Eval("Quantity") %>
                                                                <%# string.IsNullOrEmpty(Convert.ToString(Eval("UserInfo"))) ? string.Empty : "<br />User: " + Eval("UserInfo.FullName") %>
                                                                <%# string.IsNullOrEmpty(Convert.ToString(Eval("CustomListValueID_1"))) ? string.Empty : "<br />" + Eval("CustomListValue_1.CustomList.ListName") + ": " + Eval("CustomListValue_1.ListValue") %>
                                                                <%# string.IsNullOrEmpty(Convert.ToString(Eval("CustomListValueID_2"))) ? string.Empty : "<br />" + Eval("CustomListValue_2.CustomList.ListName") + ": " + Eval("CustomListValue_2.ListValue") %>
                                                                <%# (Convert.ToBoolean(Eval("Item.IsNonInventory")) || !string.IsNullOrEmpty(Convert.ToString(Eval("CustomListValueID_1"))) || !string.IsNullOrEmpty(Convert.ToString(Eval("CustomListValueID_2"))) ? "<br/><font style='color:red;'>(This is a custom item. You will receive it in 12 business days.)</font>" : String.Empty) %>
                                                            </div>
                                                            <div class="form-group col-3">
                                                                <span><%# string.Format("{0:c}", Eval("LineTotal"))%></span>                                                        
                                                            </div>
                                                        </div>
                                                    </li>
                                                </ItemTemplate>
                                            </asp:Repeater>
                                        </ul>
                                    </asp:Panel>

                                    <ul class="sub-total" id="ulSubTotal" runat="server">
                                        <li>Subtotal <span class="count"><asp:Literal ID="litSubtotal" runat="server"></asp:Literal></span></li>
                                        <asp:Panel ID="pnlShippingAndTaxes" runat="server">
                                            <asp:Panel ID="pnlParitalShippingOption" runat="server" >
                                                <li style="display:none;">
                                                    Partial Shipping&nbsp;&nbsp;<span class="count"><asp:CheckBox ID="chkPartialShipping" runat="server" OnCheckedChanged="chkPartialShipping_CheckedChanged" AutoPostBack="true"/></span> <span style="font-size: medium;" title="Multiple shipments&#013;By checking this box, items will ship as soon as they’re available, which may increase shipping costs due to multiple shipments.&#013;By not checking this box, items will be held to ship together, which may result in fewer shipments and lower shipping costs."><i class="ti-info-alt" ></i> </span>                                                  
                                                </li>
                                            </asp:Panel>
                                            
                                            <asp:Panel ID="pnlShippingMethod" runat="server">
                                                <li>Shipping Method
                                                    <div class="shipping">
                                                        <asp:DropDownList ID="ddlWebsiteShippingService" runat="server" DataValueField="WebsiteShippingServiceID" DataTextField="Description" OnSelectedIndexChanged="ddlWebsiteShippingService_SelectedIndexChanged" AutoPostBack="true"></asp:DropDownList>
                                                    </div>
                                                </li>
                                            </asp:Panel>

                                            <asp:Panel ID="pnlShippingBreakdown" runat="server" Visible="false">
                                                <asp:Repeater ID="rptShippingBreakdown" runat="server" Visible="false">
                                                    <ItemTemplate>
                                                        <li> <%# string.Format("{0}", Eval("Label"))%><span class="count"><%# string.Format("{0:c}", Eval("Amount"))%></span></li>
                                                    </ItemTemplate>
                                                </asp:Repeater>
                                            </asp:Panel>

                                            <li>Shipping <span class="count"><asp:Literal ID="litShippingAmount" runat="server"></asp:Literal></span></li>
                                            <li>Taxes <span class="count"><asp:Literal ID="litTaxAmount" runat="server"></asp:Literal></span></li>
                                            <li id="liIPDCharge" runat="server" style="display:none">IPD Duties & Taxes<span class="count"><asp:Literal ID="litIPDAmount" runat="server" Text="0"></asp:Literal></span></li>
                                        </asp:Panel>

  

                                        <asp:Panel ID="pnlBudgetAppliedSummary" runat="server" Visible="false">
                                            <li><asp:Literal ID="litBudgetAppliedLabel" runat="server" Text="Budget Applied"></asp:Literal> <span class="count"><asp:Literal ID="litBudgetAppliedAmount" runat="server"></asp:Literal></span></li>

                                            <asp:Panel ID="pnlBudgetShippingAndTaxes" runat="server" Visible="false">
                                                <li><asp:Literal ID="litBudgetShippingLabel" runat="server" Text="Shipping (Paid by Employer)"></asp:Literal> <span class="count"><asp:Literal ID="litBudgetShipping" runat="server"></asp:Literal></span></li>
                                                <li><asp:Literal ID="litBudgetTaxLabel" runat="server" Text="Tax (Paid by Employer)"></asp:Literal> <span class="count"><asp:Literal ID="litBudgetTax" runat="server"></asp:Literal></span></li>
                                            </asp:Panel>
                                            
                                        </asp:Panel>
                                        <asp:Panel ID="pnlCompanyInvoicedAmount" runat="server" Visible="false">
                                            <li><asp:Literal ID="litCompanyInvoicedTotal" runat="server" Text="Company Invoiced"></asp:Literal> <span class="count"><asp:Literal ID="litCompanyInvoiced" runat="server"></asp:Literal></span></li>
                                        </asp:Panel>

                                        <asp:Panel ID="pnlDiscountAmount" runat="server" Visible="false">
                                            <li><asp:Literal ID="litDiscountLabel" runat="server" Text="Discount"></asp:Literal> <span class="count"><asp:Literal ID="litDiscountAmount" runat="server"></asp:Literal></span></li>
                                        </asp:Panel>

                                        <li id="liPromo" runat="server">Promo <span class="count"><asp:Literal ID="litPromo" runat="server"></asp:Literal></span></li>
                                    </ul>
                                    <ul class="total" id="ulTotal" runat="server">
                                        <li><asp:Literal ID="litOrderTotal" runat="server" Text="Order Total"></asp:Literal> <span class="count" style="font-weight:bold;"><asp:Literal ID="litTotal" runat="server"></asp:Literal><asp:Literal ID="litTotalCurrencyConvert" runat="server"></asp:Literal><asp:Literal ID="litTotalWithBudget" runat="server" Visible="false"></asp:Literal><asp:Literal ID="litTotalWithBudgetCurrencyeConvert" runat="server" Visible="false"></asp:Literal></span></li>
                                    </ul>
                                    <div class="col-12">
                                        <asp:Label ID="lblOrderMessage" runat="server" Text ="Please review your order carefully before hitting submit.  Once your order has been placed, we will not be able to update or change" ForeColor="Red"></asp:Label>
                                    </div>
                                </div>
                               
                                <asp:Panel ID="pnlPromotion" runat="server">
                                    <div class="title-box">
                                        <div><b>Promotional Code</b></div>
                                    </div>
                                    <div class="form-group col-12">
                                        <asp:GridView ID="gvPromotions" runat="server" AutoGenerateColumns="false" ShowHeader="false" ShowFooter="false" GridLines="None" DataKeyNames="PromotionID" 
                                            Width="100%" onrowdeleting="gvPromotions_RowDeleting">
                                            <Columns>
                                                <asp:TemplateField ItemStyle-Width="20px">
                                                    <ItemTemplate>
                                                        <asp:LinkButton ID="lbnDeletePromotion" runat="server" CausesValidation="false" CommandName="Delete" ForeColor="Red">X</asp:LinkButton>
                                                    </ItemTemplate>
                                                </asp:TemplateField>
                                                <asp:TemplateField>
                                                    <ItemTemplate>
                                                        <%#Eval("PromotionName") %>
                                                    </ItemTemplate>
                                                </asp:TemplateField>
                                            </Columns>
                                        </asp:GridView>
                                    </div>
                                    <asp:Panel ID="pnlPromotionForm" runat="server">
                                        <div class="form-group col-12">
                                            <asp:TextBox ID="txtPromotionCode" runat="server" MaxLength="50"></asp:TextBox>
                                        </div>
                                        <div class="form-group col-12">
                                            <asp:Button ID="btnApplyPromotion" runat="server" OnClick="btnApplyPromotion_Click" CssClass="btn btn-solid" Text="Apply" /><br />
                                            <asp:CustomValidator ID="custValidPromotion" runat="server" Display="Dynamic" ControlToValidate="txtPromotionCode" Text="" onservervalidate="custValidPromotion_ServerValidate" SetFocusOnError="true"></asp:CustomValidator>
                                        </div>                                        
                                    </asp:Panel>
                                </asp:Panel>


                                <asp:PlaceHolder ID="phBudgetOption" runat="server" Visible="false">
                                    <div class="payment-box">
                                        <div class="upper-box">
                                            <div class="field-label"><asp:CheckBox ID="chkApplyBudget" runat="server" OnCheckedChanged="chkApplyBudget_CheckedChanged" AutoPostBack="true"/>  <asp:Label ID="lblMyEmployeeCredit" runat="server" Text=" My Employee Credit"></asp:Label></div> 
                                            <asp:PlaceHolder ID="phBudget" runat="server" Visible="false">
                                                <div><asp:DropDownList ID="ddlMyBudgetAssignment" runat="server" DataValueField="BudgetAssignmentID" DataTextField="Description" OnSelectedIndexChanged="ddlWebsiteShippingService_SelectedIndexChanged" AutoPostBack="true"></asp:DropDownList></div>
                                                <div class="order-box" style="display:none;">
                                                    <ul class="total">
                                                        <li>Remaining Balance Due <span class="count" style="font-weight:bold;"><asp:Literal ID="litRemainingTotal" runat="server"></asp:Literal></span></li>
                                                    </ul>
                                                </div>
                                            </asp:PlaceHolder>
                                        </div>
                                    </div>                                                
                                </asp:PlaceHolder>  

                                <asp:PlaceHolder ID="pnlPaymentOption" runat="server">
                                    <div class="payment-box">
                                        <div class="upper-box">
                                            <div class="payment-options">
                                                <ul>     
                                                    <%--<asp:PlaceHolder ID="phPackagePayment" runat="server" Visible="false">
                                                        <li>
                                                            <div class="radio-option">
                                                                <asp:RadioButton ID="rbnPackage" runat="server" GroupName="Payment" Text="Package" AutoPostBack="true" OnCheckedChanged="rbnPackage_CheckedChanged" /><br /><br />
                                                                <asp:DropDownList ID="ddlPackage" runat="server" DataValueField="PackageID" DataTextField="Name" OnSelectedIndexChanged="ddlPackage_SelectedIndexChanged" AutoPostBack="true"></asp:DropDownList>
                                                            </div>
                                                        </li>
                                                    </asp:PlaceHolder>--%>

                                                    <asp:PlaceHolder ID="phInvoicePayment" runat="server" Visible="false">
                                                        <li>
                                                            <div class="radio-option">
                                                                <asp:RadioButton ID="rbnInvoicePayment" runat="server" GroupName="Payment" Text="Invoice Payment" AutoPostBack="true" OnCheckedChanged="rbnUserCreditCard_CheckedChanged" /><br /><br />
                                                                <asp:PlaceHolder ID="phInvoicePaymentMessage" runat="server" Visible="false">
                                                                    <asp:Label ID="lblInvoicePayment" runat="server"></asp:Label>

                                                                    <asp:PlaceHolder ID="phInvoicePaymentNumber" runat="server" Visible="false">                                                                    
                                                                        <div class="col-md-12">
                                                                            <asp:Label ID="lblInvoicePaymnetNumber" runat="server" Text="PO Number :"></asp:Label>
                                                                            <asp:TextBox ID="txtInvoicePaymentNumber" runat="server"></asp:TextBox>
                                                                        </div>
                                                                    </asp:PlaceHolder>

                                                                    <asp:PlaceHolder ID="phInvoiceStoreNumber" runat="server" Visible="false">                                                                    
                                                                        <div class="col-md-12">
                                                                            <asp:Label ID="lblInvoiceStoreNumber" runat="server" Text="Store Number :"></asp:Label>
                                                                            <asp:TextBox ID="txtInvoiceStoreNumber" runat="server"></asp:TextBox>
                                                                        </div>
                                                                    </asp:PlaceHolder>


                                                                </asp:PlaceHolder>
 
                                                            </div>
                                                        </li>
                                                    </asp:PlaceHolder>
                                                    <asp:PlaceHolder ID="phEnableCreditCard" runat="server" Visible="false">
                                                        <asp:PlaceHolder ID="phUserCreditCardOption" runat="server" Visible="false">
                                                            <li>
                                                                <div class="radio-option">
                                                                    <asp:RadioButton ID="rbnUserCreditCard" runat="server" GroupName="Payment" Text="Use Saved Credit Card" AutoPostBack="true" OnCheckedChanged="rbnUserCreditCard_CheckedChanged" /><br /><br />
                                                                    <asp:PlaceHolder ID="phUserCreditCard" runat="server" Visible="false">
                                                                        <asp:DropDownList ID="ddlUserCreditCard" runat="server" DataValueField="CreditCardID" DataTextField="Description" OnSelectedIndexChanged="ddlWebsiteShippingService_SelectedIndexChanged" AutoPostBack="true"></asp:DropDownList>
                                                                        <asp:PlaceHolder ID="pnlUserCreditCardMessage" runat="server" Visible="false">
                                                                            <div class="col-md-12">
                                                                                <asp:Label ID="lblUserCreditCard" runat="server" Text="Credit Card functions are currently connected to the testing environment" ForeColor="Red"></asp:Label>
                                                                            </div>
                                                                        </asp:PlaceHolder>
                                                                    </asp:PlaceHolder>
                                                                </div>
                                                            </li>
                                                        </asp:PlaceHolder>
                                                        <li>
                                                            <div class="radio-option">
                                                                <asp:RadioButton ID="rbnNewCreditCard" runat="server" GroupName="Payment" Text="Enter New Credit Card" AutoPostBack="true" OnCheckedChanged="rbnUserCreditCard_CheckedChanged" /><br />
                                                                <asp:PlaceHolder ID="phNewCreditCardMessage" runat="server" Visible="false">
                                                                    <asp:Label ID="lblNewCreditCard" runat="server" Text="You will be prompted to enter your credit card information when you click Submit Order" hidden></asp:Label>
                                                                </asp:PlaceHolder>
                                                                <%--<span class="image"><img src="../assets/images/paypal.png" alt=""></span>--%>
                                                                <asp:PlaceHolder ID="phNewCreditCard" runat="server" Visible="false">
                                                                    <div class="theme-form">
                                                                        <div class="form-row row">

                                                                            <div class="col-md-12">
                                                                                Name on Card
                                                                                <asp:TextBox ID="txtCCFullName" runat="server" CssClass="form-control" placeholder="name on card"></asp:TextBox>
                                                                                <asp:RequiredFieldValidator ID="valEmail" runat="server" ControlToValidate="txtCCFullName" ValidationGroup="Submit" ErrorMessage="*" ForeColor="Red"></asp:RequiredFieldValidator>
                                                                            </div>
                                                                            <div class="col-md-12">
                                                                                Card Number
                                                                                <asp:TextBox ID="txtCCCardNumber" runat="server" CssClass="form-control" placeholder="card number" AutoPostBack="true" OnTextChanged="txtCCCardNumber_TextChanged"></asp:TextBox>
                                                                                <asp:RequiredFieldValidator ID="valFirstName" runat="server" ControlToValidate="txtCCCardNumber" ValidationGroup="Submit" ErrorMessage="*" ForeColor="Red"></asp:RequiredFieldValidator>
                                                                            </div>
                                                                            <div class="col-md-12">
                                                                                Credit Card Type
                                                                                <asp:TextBox ID="txtCCType" runat="server" Enabled="false" CssClass="form-control" placeholder="credit card type"></asp:TextBox>
                                                                                <asp:RequiredFieldValidator ID="valCCType" runat="server" ControlToValidate="txtCCType" ValidationGroup="Submit" ErrorMessage="*" ForeColor="Red"></asp:RequiredFieldValidator>
                                                                            </div>
                                                                            <div class="col-md-4">
                                                                                Month
                                                                                <asp:DropDownList ID="ddlCCMonth" runat="server">
                                                                                    <asp:ListItem Value="01">01</asp:ListItem>
                                                                                    <asp:ListItem Value="02">02</asp:ListItem>
                                                                                    <asp:ListItem Value="03">03</asp:ListItem>
                                                                                    <asp:ListItem Value="04">04</asp:ListItem>
                                                                                    <asp:ListItem Value="05">05</asp:ListItem>
                                                                                    <asp:ListItem Value="06">06</asp:ListItem>
                                                                                    <asp:ListItem Value="07">07</asp:ListItem>
                                                                                    <asp:ListItem Value="08">08</asp:ListItem>
                                                                                    <asp:ListItem Value="09">09</asp:ListItem>
                                                                                    <asp:ListItem Value="10">10</asp:ListItem>
                                                                                    <asp:ListItem Value="11">11</asp:ListItem>
                                                                                    <asp:ListItem Value="12">12</asp:ListItem>
                                                                                </asp:DropDownList>
                                                                                <asp:RequiredFieldValidator ID="valLastName" runat="server" ControlToValidate="ddlCCMonth" ValidationGroup="Submit" ErrorMessage="*" ForeColor="Red"></asp:RequiredFieldValidator>
                                                                            </div>
                                                                            <div class="col-md-4">
                                                                                Year
                                                                                <asp:DropDownList ID="ddlCCYear" runat="server"></asp:DropDownList>
                                                                                <asp:RequiredFieldValidator ID="valCCYear" runat="server" ControlToValidate="ddlCCYear" ValidationGroup="Submit" ErrorMessage="*" ForeColor="Red"></asp:RequiredFieldValidator>
                                                                            </div>
                                                                            <div class="col-md-4">
                                                                                CVV
                                                                                <asp:TextBox ID="txtCCCVV" runat="server" MaxLength="4" CssClass="form-control" placeholder="CVV"></asp:TextBox>
                                                                                <asp:RequiredFieldValidator ID="valCVV" runat="server" ControlToValidate="txtCCCVV" ValidationGroup="Submit" ErrorMessage="*" ForeColor="Red"></asp:RequiredFieldValidator>
                                                                            </div>
                                                                            <div class="col-md-12">
                                                                                <asp:CheckBox ID="cbSaveCard" runat="server" AutoPostBack="true" OnCheckedChanged="cbSaveCard_CheckedChanged"/> &nbsp; &nbsp;
                                                                                <asp:Label ID="lblSaveCard" runat="server" Text="Save this card for future transactions"></asp:Label>
                                                                            </div>
                                                                            <asp:PlaceHolder ID="phCreditCardNickname" runat="server" Visible="false">
                                                                                <div class="col-md-12">
                                                                                    Nickname
                                                                                    <asp:TextBox ID="txtNickname" runat="server" CssClass="form-control" placeholder="nickname"></asp:TextBox>
                                                                                    <asp:RequiredFieldValidator ID="valNickname" runat="server" ControlToValidate="txtNickname" ValidationGroup="Submit" ErrorMessage="*" ForeColor="Red"></asp:RequiredFieldValidator>
                                                                                </div>
                                                                            </asp:PlaceHolder>
                                                                        </div>
                                                                    </div>

                                                                    <asp:PlaceHolder ID="pnlNewCreditCardMessage" runat="server" Visible="false">
                                                                        <div class="col-md-12">
                                                                            <asp:Label ID="lblNewCreditCardMessage" runat="server" Text="Credit Card functions are currently connected to the testing environment" ForeColor="Red"></asp:Label>
                                                                        </div>
                                                                    </asp:PlaceHolder>
                                                                        <div class="col-md-12">
                                                                            <asp:Label ID="lblBillingAddressNotification" runat="server" Text="If your Billing address is different than your shipping address, please enter billing as new address, do not edit or update the existing address file" ForeColor="Red"></asp:Label>
                                                                        </div>
                                                                </asp:PlaceHolder>
                                                            </div>
                                                        </li>
                                                    </asp:PlaceHolder>
                                                </ul>
                                            </div>
                                        </div>
                                    </div>
                                </asp:PlaceHolder>

<%--                                <asp:Panel ID="pnlSMSOptIn" runat="server">
                                    <div class="title-box">
                                        <div><b>Text Me Order Notifications at <asp:TextBox ID="txtMobileNumber" runat="server" MaxLength="20"></asp:TextBox></b></div>
                                    </div>
                                    <div class="form-group col-12">
                                        <asp:CheckBox ID="chkSMSOptIn" runat="server" /> Sign up for automated marketing text messages about new items, great savings and more.
                                        <br /><br />
                                        SMS consent is not required to make a purchase. Message and data rates may apply. Message frequency varies. Wireless Carriers are not liable for delayed or undelivered messages. Text HELP for help and STOP to cancel. For questions, please contact us. 
                                    </div>
                                </asp:Panel>--%>

<%--                                <asp:Panel ID="pnlOptIn" runat="server">
                                    <div class="title-box">
                                        <div><b>Want to stay up to date?</b><br />Sign up to get order confirmation, order tracking & budget updates!<br />
                                            <asp:Panel ID="pnlEmailOptIn" runat="server"><asp:TextBox ID="txtNotificationEmail2" runat="server" placeholder="Enter Email Address"></asp:TextBox><br /></asp:Panel>
                                            <asp:Panel ID="pnlSMSOptIn" runat="server"><asp:TextBox ID="txtMobileNumber" runat="server" MaxLength="10" placeholder="Enter Phone Number"></asp:TextBox><br /></asp:Panel>
                                        </div>
                                    </div>
                                    <div class="form-group col-12" style="display:none;">
                                        <asp:CheckBox ID="chkSMSOptIn" runat="server" Checked="true" /> Sign up to get SMS order confirmation, tracking & budget updates!
                                    </div>
                                </asp:Panel>--%>

                                <asp:Panel ID="pnlOptIn" runat="server">
                                    <div id="OptIn" tabindex="-1" role="dialog" aria-hidden="true" style="padding-top:20px;">
                                        <div class="modal-content">
                                            <div class="modal-body modal1">
                                                <div class="container-fluid p-0" style="background-color: white;">
                                                    <div class="row">
                                                        <div class="col-12">
                                                            <div class="modal-bg">
                                                                <div class="offer-content text-center">
                                                                    <h3>Want to stay up to date?</h3><br />
                                                                    <div class="form-group mx-sm-3">
                                                                        <label>Sign up to get order confirmation, order tracking & budget updates!</label><br /><br />
                                                                        
                                                                        <asp:Panel ID="pnlEmailOptIn" runat="server">
                                                                            <asp:RegularExpressionValidator ID="reqEmail" runat="server" ControlToValidate="txtNotificationEmail2" ValidationExpression="^[\w-\.]+@([\w-]+\.)+[\w-]{2,4}$" ErrorMessage="Invalid email address" Display="Dynamic" ForeColor="Red"></asp:RegularExpressionValidator>
                                                                            <asp:TextBox ID="txtNotificationEmail2" runat="server" CssClass="form-control" placeholder="Enter Email" /><br />
                                                                        </asp:Panel>
                                                                        <asp:Panel ID="pnlSMSOptIn" runat="server">
                                                                            <asp:RegularExpressionValidator ID="regMobileNumber" runat="server" ControlToValidate="txtMobileNumber" ValidationExpression="^[0-9]{10}$" ErrorMessage="Please enter a 10-digit phone number." ForeColor="Red" />
                                                                            <div class="input-group">
                                                                                <span class="input-group-text">+1 (US)</span>
                                                                                <asp:TextBox ID="txtMobileNumber" runat="server" MaxLength="10" CssClass="form-control" placeholder="Enter Phone Number"/>
                                                                            </div>
                                                                        </asp:Panel>
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


                                <div class="text-end">
                                    <br />
                                    <%--<button id="btnPlaceOrderPaystand" runat="server" class="ps-button btn btn-solid" ps-checkoutType="checkout_payment" type="button" >Submit Order</button>--%>
                                    <asp:Button ID="btnPlaceOrder" runat="server" CssClass="btn btn-solid" Text="Submit Order" OnClick="btnPlaceOrder_Click" OnClientClick="this.disabled='true'; this.value='Please Wait..';" UseSubmitBehavior="false"/>           
                                </div>
                                <div>
                                    <asp:Label ID="lblMinimumRequirementMessage" runat="server" ForeColor="Red"></asp:Label>
                                </div>
                            </div>
                        </div>
                    </div>
                </div>
            </div>
        </div>

        <uc1:Loading runat="server" ID="ucLoading" Visible="false" />
    </section>
    <!-- section end -->

<%--    <asp:Button ID="btnPlaceOrderHidden" runat="server" style="visibility: hidden; display: none;" OnClick="btnPlaceOrderPaystand_Click"/>
    <asp:HiddenField ID="hfPaystandData" runat="server" />--%>

    <asp:HiddenField ID="hfUserGUID" runat="server" />
    <asp:HiddenField ID="hfUserWebsiteID" runat="server" />
    <asp:HiddenField ID="hfShippingPOBoxMessage" runat="server" />
    <asp:HiddenField ID="hfBudgetMessage" runat="server" />
    <asp:HiddenField ID="hfEnterpriseMessage" runat="server" />

    
</asp:Content>

<asp:Content ID="Content4" ContentPlaceHolderID="cphFooter" runat="server">
    <uc2:AccountSearchModal ID="ucAccountSearchModal" runat="server" />
</asp:Content>