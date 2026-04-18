<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="Breadcrumb.ascx.cs" Inherits="ImageSolutionsWebsite.Controls.Breadcrumb" %>
<!-- breadcrumb start -->
<div class="breadcrumb-section"  >
    <div class="container">
        <div class="row">
            <div class="col-sm-6">
                <div class="page-title">
                    <h2><asp:Label ID="lblPageName" runat="server"></asp:Label></h2>
                </div>
            </div>
            <div class="col-sm-6">
                <nav aria-label="breadcrumb" class="theme-breadcrumb">
                    <ol class="breadcrumb">
                        <li class="breadcrumb-item"><a href="/default.aspx">Home</a></li>
                        <asp:PlaceHolder ID="phUserWebSite" runat="server" Visible="false">
                            <li class="breadcrumb-item active" aria-current="page">User Website</li>
                        </asp:PlaceHolder>
                        <asp:PlaceHolder ID="phUserGroup" runat="server" Visible="false">
                            <li class="breadcrumb-item active" aria-current="page">User Group</li>
                        </asp:PlaceHolder>
                        <asp:PlaceHolder ID="phWebsiteTab" runat="server" Visible="false">
                            <li class="breadcrumb-item active" aria-current="page">Website Tab</li>
                        </asp:PlaceHolder>
                        <asp:PlaceHolder ID="phShoppingCart" runat="server" Visible="false">
                            <li class="breadcrumb-item active" aria-current="page">Shopping Cart</li>
                        </asp:PlaceHolder>
                        <asp:PlaceHolder ID="phMyAccount" runat="server" Visible="false">
                            <li class="breadcrumb-item active" aria-current="page">My Dashboard</li>
                        </asp:PlaceHolder>
                        <asp:PlaceHolder ID="phAdmin" runat="server" Visible="false">
                            <li class="breadcrumb-item active" aria-current="page">Admin</li>
                        </asp:PlaceHolder>
                        <asp:PlaceHolder ID="phCheckout" runat="server" Visible="false">
                            <li class="breadcrumb-item active" aria-current="page">Checkout</li>
                        </asp:PlaceHolder>
                        <asp:PlaceHolder ID="phRegistration" runat="server" Visible="false">
                            <li class="breadcrumb-item active" aria-current="page">Sub-Account Registration</li>
                        </asp:PlaceHolder>
                        <asp:PlaceHolder ID="phSearch" runat="server" Visible="false">
                            <li class="breadcrumb-item active" aria-current="page">Search</li>
                        </asp:PlaceHolder>
                        <asp:PlaceHolder ID="phUserAccount" runat="server" Visible="false">
                            <li class="breadcrumb-item active" aria-current="page">Assigned Accounts</li>
                        </asp:PlaceHolder>
                        <asp:PlaceHolder ID="phItemCategory" runat="server" Visible="false">
                            <li class="breadcrumb-item active" aria-current="page">Products</li>
                        </asp:PlaceHolder>
                        <li class="breadcrumb-item active" aria-current="page"><a id="aSecondTier" runat="server"></a></li>
                    </ol>
                </nav>
            </div>
        </div>
    </div>
</div>
<!-- breadcrumb end -->