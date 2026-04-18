<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="MyAccountNavigation.ascx.cs" Inherits="ImageSolutionsWebsite.Control.MyAccountNavigation" %>
<div class="col-lg-3">
    <div class="dashboard-sidebar">
        <div class="profile-top">
            <div class="profile-image" style="display:none;">
                <img src="../assets/images/avtar.jpg" alt="" class="img-fluid">
            </div>
            <div class="profile-detail" ignore>
                <h5><asp:Label ID="lblFullName" runat="server"></asp:Label></h5>
                <h6><asp:Label ID="lblEmail" runat="server"></asp:Label></h6>
            </div>
        </div>
        <div class="faq-tab">
            <ul class="nav nav-tabs" id="top-tab" role="tablist">
                <li class="nav-item"><a id="aDashboard" runat="server" class="nav-link" href="/MyAccount/Dashboard.aspx">My Dashboard</a></li>
                <li class="nav-item"><a id="aAddressBook" runat="server" class="nav-link" href="/MyAccount/AddressBook.aspx">Address Book</a></li>
                <li class="nav-item"><a id="aMyOrders" runat="server" class="nav-link" href="/MyAccount/Orders.aspx">My Orders</a></li>
                <li class="nav-item" id="liOrderApproval" runat="server"><a id="aOrderApproval" runat="server" class="nav-link" href="/Admin/OrderPendingApproval.aspx">Order Approval</a></li>
                <li class="nav-item" id="liOrderReport" runat="server"><a id="aOrderReport" runat="server" class="nav-link" href="/Admin/OrderOverview.aspx">Order Report</a></li>
                <li class="nav-item"><a id="aBudgets" runat="server" class="nav-link" href="/MyAccount/Budgets.aspx">My Employee Credits</a></li>
                <li class="nav-item" runat="server" id="liCreditCard"><a id="aCreditCard" runat="server" class="nav-link" href="/MyAccount/CreditCard.aspx">Credit Cards</a></li>
                <li class="nav-item"><a id="aProfile" runat="server" class="nav-link" href="/MyAccount/Profile.aspx">Profile</a></li>
                <li class="nav-item" runat="server" id="liNotificationSettings"><a id="aNotificationSettings" runat="server" class="nav-link" href="/MyAccount/NotificationSettings.aspx">Notification Settings</a></li>
                <%--<li class="nav-item"><a href="https://wkf.ms/3GvR43z" class="nav-link" target="_blank">Customer Request Form</a></li>--%>
                <li class="nav-item"><a class="nav-link" href="/logout.aspx">Log Out</a></li>
            </ul>
        </div>
    </div>
</div>