<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="AdminNavigation.ascx.cs" Inherits="ImageSolutionsWebsite.Control.AdminNavigation" %>
<div class="col-lg-3">
    <div class="dashboard-sidebar">
        <div class="profile-top">
            <div class="profile-image" style="display:none;">
                <img src="../assets/images/avtar.jpg" alt="" class="img-fluid">
            </div>
            <div class="profile-detail">
                <h5><asp:Literal ID="litUser" runat="server"></asp:Literal></h5>
                <h6><asp:Literal ID="litEmailAddress" runat="server"></asp:Literal></h6>
            </div>
        </div>
        <div class="faq-tab">
            <ul class="nav nav-tabs" id="top-tab" role="tablist">
                <li class="nav-item"><a id="aWebsite" runat="server" class="nav-link" href="/Admin/WebsiteOverview.aspx">Website Management</a></li>
                <li class="nav-item"><a id="aAccount" runat="server" class="nav-link" href="/Admin/Account.aspx">Account Management</a></li>
                <li class="nav-item"><a id="aSubAccount" runat="server" class="nav-link" href="/Admin/AccountOverview.aspx">Sub Account Management</a></li>
                <li class="nav-item"><a id="aUser" runat="server" class="nav-link" href="/Admin/UserWebsiteOverview.aspx">User Management</a></li>
                <li class="nav-item"><a id="aGroup" runat="server" class="nav-link" href="/Admin/GroupOverview.aspx">Group Management</a></li>
                <li class="nav-item"><a id="aTab" runat="server" class="nav-link" href="/Admin/TabOverview.aspx">Tab Management</a></li>
                <li class="nav-item"><a id="aItem" runat="server" class="nav-link" href="/Admin/ItemOverview.aspx">Item Management</a></li>
                <li class="nav-item"><a id="aInventoryReport" runat="server" class="nav-link" href="/Admin/InventoryReport.aspx">Inventory Report</a></li>
                <li class="nav-item"><a id="aBudget" runat="server" class="nav-link" href="/Admin/BudgetOverview.aspx">Employee Credit Management</a></li>
                <li class="nav-item"><a id="aOrder" runat="server" class="nav-link" href="/Admin/OrderOverview.aspx">Order Management</a></li>
                <li class="nav-item"><a id="aOrderApproval" runat="server" class="nav-link" href="/Admin/OrderPendingApproval.aspx">Order Approval</a></li>
                <li class="nav-item"><a id="aCreditCard" runat="server" class="nav-link" href="/Admin/CreditCardOverview.aspx">Credit Card Management</a></li>
                <li class="nav-item"><a id="aPromotion" runat="server" class="nav-link" href="/Admin/PromotionOverview.aspx">Promotions</a></li>
                <li class="nav-item"><a id="aShipping" runat="server" class="nav-link" href="/Admin/ShippingOverview.aspx">Shipping Management</a></li>
                <li class="nav-item"><a id="aMessage" runat="server" class="nav-link" href="/Admin/MessageOverview.aspx">Message Management</a></li>
                <li class="nav-item"><a id="aCustomization" runat="server" class="nav-link" href="/Admin/ItemCustomization.aspx">Item Customization</a></li>
                <li class="nav-item"><a id="aCustomList" runat="server" class="nav-link" href="/Admin/CustomListOverview.aspx">Custom List Management</a></li>
            </ul>
        </div>
    </div>
</div>