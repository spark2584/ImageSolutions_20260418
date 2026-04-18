<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="Header.ascx.cs" Inherits="ImageSolutionsWebsite.Controls.Header" %>
<%@ Register Src="~/Control/VirtualMenu.ascx" TagPrefix="uc1" TagName="VirtualMenu" %>
<%@ Register Src="~/Control/VirtualMenuHeader.ascx" TagPrefix="uc1" TagName="VirtualMenuHeader" %>


<!-- header start -->
<header>
    <div class="mobile-fix-option"></div>
    <div class="top-header">
        <div class="container">
            <div class="row">
                <div class="col-lg-6">
                    <div class="header-contact">
                        <ul>
                            <li><asp:Literal ID="litWebsiteName" runat="server"></asp:Literal></li>
                            <li style="display:none;"><i class="fa fa-phone" aria-hidden="true"></i>Call Us: 123 - 456 - 7890</li>
                        </ul>
                    </div>
                </div>
                <div class="col-lg-6 text-end">
                    <ul class="header-dropdown">
                        <li class="mobile-wishlist" style="display:none;"><a href="#"><i class="fa fa-heart" aria-hidden="true"></i></a></li>
                        <asp:Label ID="lblUserName" runat="server"></asp:Label>
                        <li class="onhover-dropdown mobile-account" id="liMyAccount" runat="server">
                            <i class="fa fa-user" aria-hidden="true"></i> My Account
                            <ul class="onhover-show-div">
                                <asp:PlaceHolder ID="phLogin" runat="server" Visible="false">
                                    <li><a href="/login.aspx">Login</a></li>
                                    <%--<li><a href="/register.aspx">register</a></li>--%>
                                </asp:PlaceHolder>
                                <asp:PlaceHolder ID="phLogout" runat="server" Visible="false">
                                    <li><a href="/myaccount/dashboard.aspx">My Dashboard</a></li>
                                    <li><a id="aUserWebsite" runat="server" href="/userwebsite.aspx">Websites</a></li>
                                    <li><a id="aUserAccount" runat="server" href="/UserAccount.aspx">My Accounts</a></li>
                                    <li><a id="aAdmin" runat="server" href="/Admin/">Admin</a></li>
                                    <%--<li><asp:HyperLink ID="lnkReturn" runat="server" Text="Return"></asp:HyperLink></li>--%>
                                    <li><asp:LinkButton ID="lnkReturn" runat="server" Text="Return" OnClick="lnkReturn_Click"></asp:LinkButton></li>
                                    <li><a id="aLogout" runat="server" href="/logout.aspx">Logout</a></li>
                                    <li><asp:LinkButton ID="btnReturn" runat="server" OnClick="btnReturn_Click" Text="Return As Admin" Visible="false"></asp:LinkButton></li>
                                </asp:PlaceHolder>
                            </ul>
                        </li>
                    </ul>
                </div>
            </div>
        </div>
    </div>

    <div class="container">
        <div class="row">
            <div class="col-sm-12">
                <div class="main-menu">
                    <div class="menu-left">
                        <div class="navbar">
                            <a href="javascript:void(0)" onclick="openNav()">
                                <div id="divSideMenu" runat="server" class="bar-style"> <i class="fa fa-bars sidebar-bar" aria-hidden="true"></i>
                                </div>
                            </a>
                            <div id="mySidenav" class="sidenav">
                                <a href="javascript:void(0)" class="sidebar-overlay" onclick="closeNav()"></a>
                                <nav>
                                    <div onclick="closeNav()">
                                        <div class="sidebar-back text-start">
                                            <i class="fa fa-angle-left pe-2" aria-hidden="true"></i> Back
                                        </div>
                                    </div>
                                    <uc1:VirtualMenu runat="server" id="VirtualMenu" />
                                </nav>
                            </div>
                        </div>
                        <div class="brand-logo">
                            <a href="/"> <img id="imgLogo" runat="server" src="/assets/images/icon/logo.png" style="max-width:280px;max-height:100px;"
                                    class="img-fluid blur-up lazyload" alt=""></a>
                        </div>
                    </div>
                    <div class="menu-right pull-right">
                        <div>
                            <nav id="main-nav">
                                <div class="toggle-nav"><i class="fa fa-bars sidebar-bar"></i></div>
                                <uc1:VirtualMenuHeader runat="server" id="VirtualMenuHeader" />
                            </nav>
                        </div>
                        <div>
                            <div class="icon-nav">
                                <ul>
                                    <li class="onhover-div mobile-search">
                                        <div><img src="/assets/images/icon/search.png" onclick="openSearch()"
                                                class="img-fluid blur-up lazyload" style="max-width: 50px;" alt="">
                                            <i class="ti-search" onclick="openSearch()"></i>
                                        </div>
                                        <div id="search-overlay" class="search-overlay">
                                            <div>
                                                <span class="closebtn" onclick="closeSearch()"
                                                    title="Close Overlay">×</span>
                                                <div class="overlay-content">
                                                    <div class="container">
                                                        <div class="row">
                                                            <div class="col-xl-12">
                                                                    <div class="form-group">
                                                                        <asp:TextBox ID="txtSearchText" runat="server" placeholder="Search a Product" CssClass="form-control" AutoPostBack="true" OnTextChanged="txtSearchText_TextChanged"></asp:TextBox>
                                                                    </div>
                                                                    <button type="submit" class="btn btn-primary"><i
                                                                            class="fa fa-search"></i></button>
                                                            </div>
                                                        </div>
                                                    </div>
                                                </div>
                                            </div>
                                        </div>
                                    </li>
                                    <li class="onhover-div mobile-setting" style="display:none;">
                                        <div><img src="/assets/images/icon/setting.png"
                                                class="img-fluid blur-up lazyload" alt="">
                                            <i class="ti-settings"></i>
                                        </div>
                                        <div class="show-div setting">
                                            <h6>language</h6>
                                            <ul>
                                                <li><a href="#">english</a> </li>
                                                <li><a href="#">french</a> </li>
                                            </ul>
                                            <h6>currency</h6>
                                            <ul class="list-inline">
                                                <li><a href="#">euro</a> </li>
                                                <li><a href="#">rupees</a> </li>
                                                <li><a href="#">pound</a> </li>
                                                <li><a href="#">doller</a> </li>
                                            </ul>
                                        </div>
                                    </li>
                                    <li class="onhover-div mobile-cart" id="liCart">
                                        <div><a href="/shoppingcart.aspx"><img src="/assets/images/icon/cart-1.png"
                                                class="img-fluid blur-up lazyload" alt=""></a>
                                            <i class="ti-shopping-cart"></i>
                                        </div>
                                        <span class="cart_qty_cls"><asp:Label ID="lblShoppingCartQuantity" runat="server" Text="0"></asp:Label></span>
                                        
                                        <ul class="show-div shopping-cart" style="min-width:300px;" id="ulShoppingCart" runat="server">
                                            <asp:Repeater ID="rptShoppingCart" runat="server" OnItemCommand="rptShoppingCart_ItemCommand">
                                                <ItemTemplate>
                                                    <li>
                                                        <div class="media">
                                                            <a href="#"><img class="me-3" src='<%# string.IsNullOrEmpty(Convert.ToString(Eval("CustomDesignImagePath"))) ? Eval("Item.DisplayImageURL") : Eval("CustomDesignImagePath") %>' alt="Generic placeholder image"></a>
                                                            <div class="media-body">
                                                                <a href="#">
                                                                    <h5><%# Eval("Item.SalesDescription") %></h5>
                                                                </a>
                                                                <h4><span><%# Eval("Quantity") %> x <%# string.Format("{0:c}", Eval("UnitPrice"))%></span></h4>
                                                            </div>
                                                        </div>
                                                        <div class="close-circle">
                                                            <asp:LinkButton id="btnDelete" runat="server" CommandArgument='<%# Eval("ShoppingCartLineID") %>' CommandName="DeleteLine"><i class="fa fa-times" aria-hidden="true"></i></asp:LinkButton>
                                                        </div>
                                                    </li>
                                                </ItemTemplate>
                                            </asp:Repeater>
                                            <li>
                                                <div class="total">
                                                    <h5>subtotal : <span><asp:Label id="lblShoppingCartTotal" runat="server"></asp:Label></span></h5>
                                                </div>
                                            </li>
                                            <li>
                                                <div class="buttons">
                                                    <a href="/shoppingcart.aspx" class="view-cart">view cart</a>
                                                    <a href="/checkout.aspx" class="checkout">checkout</a>
                                                </div>
                                            </li>
                                        </ul>
                                    </li>
                                </ul>
                            </div>
                        </div>
                    </div>
                </div>
            </div>
        </div>
    </div>
</header>
<!-- header end -->