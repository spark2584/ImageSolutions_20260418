<%@ Page Title="" Language="C#" MasterPageFile="~/MasterPage.Master" AutoEventWireup="true" CodeBehind="Dashboard.aspx.cs" Inherits="ImageSolutionsWebsite.MyAccount.Dashboard" %>
<%@ Register Src="~/Control/MyAccountNavigation.ascx" TagPrefix="uc1" TagName="MyAccountNavigation" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="cphHeader" runat="server">
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="cphBody" runat="server">
    <!--  dashboard section start -->
    <section class="dashboard-section section-b-space user-dashboard-section">
        <div class="container">
<%--            <div class="row">
                <div class="col-lg-12">
                    <p style="background-color:red; color:white; text-align:center; font-size:large; padding: 20px 0px 20px 0px;">
                        Dear Valued Customer,
                        <br>We will be closed Monday September 4th in observance of Labor Day. &nbsp;During this time, all departments, including Customer Service will be closed. &nbsp;To avoid delays due to the extended holiday weekend, please submit your orders before 11am EST Friday September 1st. &nbsp;We will resume normal business hours Tuesday September 5th.
                        <br>Thank you for partnering with Image Solutions!
                    </p>
                </div>
            </div>--%>
            <asp:Panel ID="pnlWebsiteMessage" runat="server" CssClass="row" Visible="false" >
                <div CssClass="col-lg-12">
                    <div style="text-align:center; font-size:x-large"><asp:Literal ID="litWebsiteMessage" runat="server"></asp:Literal></div>
                    <br />
                    <br />
                </div>
            </asp:Panel>
            <div class="row">
                <uc1:MyAccountNavigation runat="server" id="MyAccountNavigation" />
                <div class="col-lg-9">
                    <div class="faq-content tab-content" id="top-tabContent">
                        <div class="tab-pane fade show active" id="info">
                            <div class="counter-section">
                                <div class="box-account box-info">
                                    <div class="row">
                                        <div class="col-sm-12">
                                            <div class="top-banner-wrapper">
                                                <asp:Image ID="imgBanner" runat="server" Width="100%" />
                                            </div>
                                        </div>
                                    </div>
                                </div>
                                <div class="box-account box-info">
                                    <div class="box-head">
                                        <h4>Announcements</h4>
                                    </div>

                                    <div class="row">
                                        <asp:Literal ID="litPackageMessage" runat="server"></asp:Literal>
                                    </div>

                                    <div class="row">
                                        <asp:Repeater ID="rptAnnoucnement" runat="server" OnItemDataBound="rptAnnoucnement_ItemDataBound">
                                            <ItemTemplate>
                                                <asp:HiddenField ID="hfWebsiteMessageID" runat="server" Value='<%# Eval("WebsiteMessageID")%>'/>
                                                <div class="col-sm-12">
                                                    <asp:Label ID="lblDate" runat="server"></asp:Label>
                                                </div>
                                                <div class="col-sm-12">
                                                    <asp:Literal ID="litMessage" runat="server"></asp:Literal>
                                                    <br />
                                                    <br />
                                                </div>
                                            </ItemTemplate>
                                        </asp:Repeater>
                                    </div>


                                </div>

                                <div class="welcome-msg">
                                    <h4>Hello, <asp:Label ID="lblCustomerName" runat="server"></asp:Label></h4>
                                    <p>From your My Account Dashboard you have the ability to view a snapshot of your
                                        recent
                                        account activity and update your account information. Select a link below to
                                        view or
                                        edit information.</p>
                                </div>
                                <div class="row">
                                    <div class="col-md-6">
                                        <div class="counter-box">
                                            <img src="../assets/images/icon/dashboard/sale.png" class="img-fluid">
                                            <div>
                                                <h3><asp:Literal ID="litApprovedOrderCount" runat="server">0</asp:Literal></h3>
                                                <h5>Total Orders</h5>
                                            </div>
                                        </div>
                                    </div>
                                    <div class="col-md-6">
                                        <div class="counter-box">
                                            <img src="../assets/images/icon/dashboard/homework.png" class="img-fluid">
                                            <div>
                                                <h3><asp:Literal ID="litPendingOrderCount" runat="server">0</asp:Literal></h3>
                                                <h5>Pending Orders</h5>
                                            </div>
                                        </div>
                                    </div>
                                </div>

                                <div class="box-account box-info">
                                    <div class="box-head">
                                        <h4>Account Information</h4>
                                    </div>
                                    <div class="row">
                                        <div class="col-sm-12">
                                            <div class="box">
                                                <div class="box-title">
                                                    <h3>Contact Information</h3><a href="/MyAccount/Profile.aspx">Edit</a>
                                                </div>
                                                <div class="box-content">
                                                    <h6 ignore><asp:Label ID="lblName" runat="server"></asp:Label></h6>
                                                    <h6 ignore><asp:Label ID="lblEmail" runat="server"></asp:Label></h6>
                                                    <asp:Panel ID="pnlChangePassword" runat="server">
                                                        <h6><a href="/MyAccount/ChangePassword.aspx">Change Password</a></h6>
                                                    </asp:Panel>
                                                </div>
                                            </div>
                                        </div>
                                    </div>
                                    <asp:Panel ID="pnlManageAddress" runat="server">

                                        <div class="box mt-3">
                                            <div class="box-title">
                                                <h3>Address Book</h3><a id="aManageAddress" runat="server" href="/MyAccount/AddressBook.aspx">Manage Addresses</a>
                                            </div>
                                            <div class="row">
                                                <div class="col-sm-6">
                                                    <h6>Default Billing Address</h6>
                                                    <asp:Literal ID="litBillingAddress" runat="server"></asp:Literal>
                                                    <br>
                                                </div>
                                                <div class="col-sm-6">
                                                    <h6>Default Shipping Address</h6>
                                                    <asp:Literal ID="litShippingAddress" runat="server"></asp:Literal>
                                                    <br>
                                                </div>
                                            </div>
                                        </div>

                                    </asp:Panel>
                                </div>
                            </div>
                        </div>
                      
                        <div class="tab-pane fade" id="orders">
                            <div class="row">
                                <div class="col-12">
                                    <div class="card dashboard-table mt-0">
                                        <div class="card-body table-responsive-sm">
                                            <div class="top-sec">
                                                <h3>My Orders</h3>
                                            </div>
                                            <div class="table-responsive-xl">
                                                <table class="table cart-table order-table">
                                                    <thead>
                                                        <tr class="table-head">
                                                            <th scope="col">image</th>
                                                            <th scope="col">Order Id</th>
                                                            <th scope="col">Product Details</th>
                                                            <th scope="col">Status</th>
                                                            <th scope="col">Price</th>
                                                            <th scope="col">View</th>
                                                        </tr>
                                                    </thead>
                                                    <tbody>
                                                        <tr>
                                                            <td>
                                                                <a href="javascript:void(0)">
                                                                    <img src="../assets/images/pro3/1.jpg"
                                                                        class="blur-up lazyloaded" alt="">
                                                                </a>
                                                            </td>
                                                            <td>
                                                                <span class="mt-0">#125021</span>
                                                            </td>
                                                            <td>
                                                                <span class="fs-6">Purple polo tshirt</span>
                                                            </td>
                                                            <td>
                                                                <span
                                                                    class="badge rounded-pill bg-success custom-badge">Shipped</span>
                                                            </td>
                                                            <td>
                                                                <span class="theme-color fs-6">$49.54</span>
                                                            </td>
                                                            <td>
                                                                <a href="javascript:void(0)">
                                                                    <i class="fa fa-eye text-theme"></i>
                                                                </a>
                                                            </td>
                                                        </tr>

                                                        <tr>
                                                            <td>
                                                                <a href="javascript:void(0)">
                                                                    <img src="../assets/images/pro3/2.jpg"
                                                                        class="blur-up lazyloaded" alt="">
                                                                </a>
                                                            </td>
                                                            <td>
                                                                <span class="mt-0">#125367</span>
                                                            </td>
                                                            <td>
                                                                <span class="fs-6">Sleevless white top</span>
                                                            </td>
                                                            <td>
                                                                <span
                                                                    class="badge rounded-pill bg-danger custom-badge">Pending</span>
                                                            </td>
                                                            <td>
                                                                <span class="theme-color fs-6">$49.54</span>
                                                            </td>
                                                            <td>
                                                                <a href="javascript:void(0)">
                                                                    <i class="fa fa-eye text-theme"></i>
                                                                </a>
                                                            </td>
                                                        </tr>

                                                        <tr>
                                                            <td>
                                                                <a href="javascript:void(0)">
                                                                    <img src="../assets/images/pro3/27.jpg"
                                                                        class="blur-up lazyloaded" alt="">
                                                                </a>
                                                            </td>
                                                            <td>
                                                                <p>#125948</p>
                                                            </td>
                                                            <td>
                                                                <p class="fs-6">multi color polo tshirt</p>
                                                            </td>
                                                            <td>
                                                                <span
                                                                    class="badge rounded-pill bg-success custom-badge">Shipped</span>
                                                            </td>
                                                            <td>
                                                                <p class="theme-color fs-6">$49.54</p>
                                                            </td>
                                                            <td>
                                                                <a href="javascript:void(0)">
                                                                    <i class="fa fa-eye text-theme"></i>
                                                                </a>
                                                            </td>
                                                        </tr>

                                                        <tr>
                                                            <td>
                                                                <a href="javascript:void(0)">
                                                                    <img src="../assets/images/pro3/28.jpg"
                                                                        class="blur-up lazyloaded" alt="">
                                                                </a>
                                                            </td>
                                                            <td>
                                                                <p>#127569</p>
                                                            </td>
                                                            <td>
                                                                <p class="fs-6">Candy red solid tshirt</p>
                                                            </td>
                                                            <td>
                                                                <span
                                                                    class="badge rounded-pill bg-success custom-badge">Shipped</span>
                                                            </td>
                                                            <td>
                                                                <p class="theme-color fs-6">$49.54</p>
                                                            </td>
                                                            <td>
                                                                <a href="javascript:void(0)">
                                                                    <i class="fa fa-eye text-theme"></i>
                                                                </a>
                                                            </td>
                                                        </tr>

                                                        <tr>
                                                            <td>
                                                                <a href="javascript:void(0)">
                                                                    <img src="../assets/images/pro3/33.jpg"
                                                                        class="blur-up lazyloaded" alt="">
                                                                </a>
                                                            </td>
                                                            <td>
                                                                <p>#125753</p>
                                                            </td>
                                                            <td>
                                                                <p class="fs-6">multicolored polo tshirt</p>
                                                            </td>
                                                            <td>
                                                                <span
                                                                    class="badge rounded-pill bg-secondary custom-badge">Canceled</span>
                                                            </td>
                                                            <td>
                                                                <p class="theme-color fs-6">$49.54</p>
                                                            </td>
                                                            <td>
                                                                <a href="javascript:void(0)">
                                                                    <i class="fa fa-eye text-theme"></i>
                                                                </a>
                                                            </td>
                                                        </tr>

                                                        <tr>
                                                            <td>
                                                                <a href="javascript:void(0)">
                                                                    <img src="../assets/images/pro3/34.jpg"
                                                                        class="blur-up lazyloaded" alt="">
                                                                </a>
                                                            </td>
                                                            <td>
                                                                <span>#125021</span>
                                                            </td>
                                                            <td>
                                                                <span class="fs-6">Men's Sweatshirt</span>
                                                            </td>
                                                            <td>
                                                                <span
                                                                    class="badge rounded-pill bg-secondary custom-badge">Canceled</span>
                                                            </td>
                                                            <td>
                                                                <span class="theme-color fs-6">$49.54</span>
                                                            </td>
                                                            <td>
                                                                <a href="javascript:void(0)">
                                                                    <i class="fa fa-eye text-theme"></i>
                                                                </a>
                                                            </td>
                                                        </tr>
                                                    </tbody>
                                                </table>
                                            </div>
                                        </div>
                                    </div>
                                </div>
                            </div>
                        </div>
                        <div class="tab-pane fade" id="wishlist">
                            <div class="row">
                                <div class="col-12">
                                    <div class="card dashboard-table mt-0">
                                        <div class="card-body table-responsive-sm">
                                            <div class="top-sec">
                                                <h3>My Wishlist</h3>
                                            </div>
                                            <div class="table-responsive-xl">
                                                <table class="table cart-table wishlist-table">
                                                    <thead>
                                                        <tr class="table-head">
                                                            <th scope="col">image</th>
                                                            <th scope="col">Order Id</th>
                                                            <th scope="col">Product Details</th>
                                                            <th scope="col">Price</th>
                                                            <th scope="col">Action</th>
                                                        </tr>
                                                    </thead>
                                                    <tbody>
                                                        <tr>
                                                            <td>
                                                                <a href="javascript:void(0)">
                                                                    <img src="../assets/images/pro3/1.jpg"
                                                                        class="blur-up lazyloaded" alt="">
                                                                </a>
                                                            </td>
                                                            <td>
                                                                <span class="mt-0">#125021</span>
                                                            </td>
                                                            <td>
                                                                <span>Purple polo tshirt</span>
                                                            </td>
                                                            <td>
                                                                <span class="theme-color fs-6">$49.54</span>
                                                            </td>
                                                            <td>
                                                                <a href="javascript:void(0)"
                                                                    class="btn btn-xs btn-solid">
                                                                    Move to Cart
                                                                </a>
                                                            </td>
                                                        </tr>

                                                        <tr>
                                                            <td>
                                                                <a href="javascript:void(0)">
                                                                    <img src="../assets/images/pro3/2.jpg"
                                                                        class="blur-up lazyloaded" alt="">
                                                                </a>
                                                            </td>
                                                            <td>
                                                                <span class="mt-0">#125367</span>
                                                            </td>
                                                            <td>
                                                                <span>Sleevless white top</span>
                                                            </td>
                                                            <td>
                                                                <span class="theme-color fs-6">$49.54</span>
                                                            </td>
                                                            <td>
                                                                <a href="javascript:void(0)"
                                                                    class="btn btn-xs btn-solid">
                                                                    Move to Cart
                                                                </a>
                                                            </td>
                                                        </tr>

                                                        <tr>
                                                            <td>
                                                                <a href="javascript:void(0)">
                                                                    <img src="../assets/images/pro3/27.jpg"
                                                                        class="blur-up lazyloaded" alt="">
                                                                </a>
                                                            </td>
                                                            <td>
                                                                <span>#125948</span>
                                                            </td>
                                                            <td>
                                                                <span>multi color polo tshirt</span>
                                                            </td>
                                                            <td>
                                                                <span class="theme-color fs-6">$49.54</span>
                                                            </td>
                                                            <td>
                                                                <a href="javascript:void(0)"
                                                                    class="btn btn-xs btn-solid">
                                                                    Move to Cart
                                                                </a>
                                                            </td>
                                                        </tr>

                                                        <tr>
                                                            <td>
                                                                <a href="javascript:void(0)">
                                                                    <img src="../assets/images/pro3/28.jpg"
                                                                        class="blur-up lazyloaded" alt="">
                                                                </a>
                                                            </td>
                                                            <td>
                                                                <span>#127569</span>
                                                            </td>
                                                            <td>
                                                                <span>Candy red solid tshirt</span>
                                                            </td>
                                                            <td>
                                                                <span class="theme-color fs-6">$49.54</span>
                                                            </td>
                                                            <td>
                                                                <a href="javascript:void(0)"
                                                                    class="btn btn-xs btn-solid">
                                                                    Move to Cart
                                                                </a>
                                                            </td>
                                                        </tr>

                                                        <tr>
                                                            <td>
                                                                <a href="javascript:void(0)">
                                                                    <img src="../assets/images/pro3/33.jpg"
                                                                        class="blur-up lazyloaded" alt="">
                                                                </a>
                                                            </td>
                                                            <td>
                                                                <span>#125753</span>
                                                            </td>
                                                            <td>
                                                                <span>multicolored polo tshirt</span>
                                                            </td>
                                                            <td>
                                                                <span class="theme-color fs-6">$49.54</span>
                                                            </td>
                                                            <td>
                                                                <a href="javascript:void(0)"
                                                                    class="btn btn-xs btn-solid">
                                                                    Move to Cart
                                                                </a>
                                                            </td>
                                                        </tr>

                                                        <tr>
                                                            <td>
                                                                <a href="javascript:void(0)">
                                                                    <img src="../assets/images/pro3/34.jpg"
                                                                        class="blur-up lazyloaded" alt="">
                                                                </a>
                                                            </td>
                                                            <td>
                                                                <span>#125021</span>
                                                            </td>
                                                            <td>
                                                                <span>Men's Sweatshirt</span>
                                                            </td>
                                                            <td>
                                                                <span class="theme-color fs-6">$49.54</span>
                                                            </td>
                                                            <td>
                                                                <a href="javascript:void(0)"
                                                                    class="btn btn-xs btn-solid">
                                                                    Move to Cart
                                                                </a>
                                                            </td>
                                                        </tr>
                                                    </tbody>
                                                </table>
                                            </div>
                                        </div>
                                    </div>
                                </div>
                            </div>
                        </div>
                        <div class="tab-pane fade" id="payment">
                            <div class="row">
                                <div class="col-12">
                                    <div class="card mt-0">
                                        <div class="card-body">
                                            <div class="top-sec">
                                                <h3>Saved Cards</h3>
                                                <a href="#" class="btn btn-sm btn-solid">+ add new</a>
                                            </div>
                                            <div class="address-book-section">
                                                <div class="row g-4">
                                                    <div class="select-box active col-xl-4 col-md-6">
                                                        <div class="address-box">
                                                            <div class="bank-logo">
                                                                <img src="../assets/images/bank-logo.png"
                                                                    class="bank-logo">
                                                                <img src="../assets/images/visa.png"
                                                                    class="network-logo">
                                                            </div>
                                                            <div class="card-number">
                                                                <h6>Card Number</h6>
                                                                <h5>6262 6126 2112 1515</h5>
                                                            </div>
                                                            <div class="name-validity">
                                                                <div class="left">
                                                                    <h6>name on card</h6>
                                                                    <h5>Mark Jecno</h5>
                                                                </div>
                                                                <div class="right">
                                                                    <h6>validity</h6>
                                                                    <h5>XX/XX</h5>
                                                                </div>
                                                            </div>
                                                            <div class="bottom">
                                                                <a href="javascript:void(0)"
                                                                    data-bs-target="#edit-address"
                                                                    data-bs-toggle="modal" class="bottom_btn">edit</a>
                                                                <a href="#" class="bottom_btn">remove</a>
                                                            </div>
                                                        </div>
                                                    </div>
                                                    <div class="select-box col-xl-4 col-md-6">
                                                        <div class="address-box">
                                                            <div class="bank-logo">
                                                                <img src="../assets/images/bank-logo1.png"
                                                                    class="bank-logo">
                                                                <img src="../assets/images/visa.png"
                                                                    class="network-logo">
                                                            </div>
                                                            <div class="card-number">
                                                                <h6>Card Number</h6>
                                                                <h5>6262 6126 2112 1515</h5>
                                                            </div>
                                                            <div class="name-validity">
                                                                <div class="left">
                                                                    <h6>name on card</h6>
                                                                    <h5>Mark Jecno</h5>
                                                                </div>
                                                                <div class="right">
                                                                    <h6>validity</h6>
                                                                    <h5>XX/XX</h5>
                                                                </div>
                                                            </div>
                                                            <div class="bottom">
                                                                <a href="javascript:void(0)"
                                                                    data-bs-target="#edit-address"
                                                                    data-bs-toggle="modal" class="bottom_btn">edit</a>
                                                                <a href="#" class="bottom_btn">remove</a>
                                                            </div>
                                                        </div>
                                                    </div>
                                                </div>
                                            </div>
                                        </div>
                                    </div>
                                </div>
                            </div>
                        </div>
                        <div class="tab-pane fade" id="profile">
                            <div class="row">
                                <div class="col-12">
                                    <div class="card mt-0">
                                        <div class="card-body">
                                            <div class="dashboard-box">
                                                <div class="dashboard-title">
                                                    <h4>profile</h4>
                                                    <a class="edit-link" href="#">edit</a>
                                                </div>
                                                <div class="dashboard-detail">
                                                    <ul>
                                                        <li>
                                                            <div class="details">
                                                                <div class="left">
                                                                    <h6>company name</h6>
                                                                </div>
                                                                <div class="right">
                                                                    <h6>Fashion Store</h6>
                                                                </div>
                                                            </div>
                                                        </li>
                                                        <li>
                                                            <div class="details">
                                                                <div class="left">
                                                                    <h6>email address</h6>
                                                                </div>
                                                                <div class="right">
                                                                    <h6>mark.jecno@gmail.com</h6>
                                                                </div>
                                                            </div>
                                                        </li>
                                                        <li>
                                                            <div class="details">
                                                                <div class="left">
                                                                    <h6>Country / Region</h6>
                                                                </div>
                                                                <div class="right">
                                                                    <h6>Downers Grove, IL</h6>
                                                                </div>
                                                            </div>
                                                        </li>
                                                        <li>
                                                            <div class="details">
                                                                <div class="left">
                                                                    <h6>Year Established</h6>
                                                                </div>
                                                                <div class="right">
                                                                    <h6>2018</h6>
                                                                </div>
                                                            </div>
                                                        </li>
                                                        <li>
                                                            <div class="details">
                                                                <div class="left">
                                                                    <h6>Total Employees</h6>
                                                                </div>
                                                                <div class="right">
                                                                    <h6>101 - 200 People</h6>
                                                                </div>
                                                            </div>
                                                        </li>
                                                        <li>
                                                            <div class="details">
                                                                <div class="left">
                                                                    <h6>category</h6>
                                                                </div>
                                                                <div class="right">
                                                                    <h6>clothing</h6>
                                                                </div>
                                                            </div>
                                                        </li>
                                                        <li>
                                                            <div class="details">
                                                                <div class="left">
                                                                    <h6>street address</h6>
                                                                </div>
                                                                <div class="right">
                                                                    <h6>549 Sulphur Springs Road</h6>
                                                                </div>
                                                            </div>
                                                        </li>
                                                        <li>
                                                            <div class="details">
                                                                <div class="left">
                                                                    <h6>city/state</h6>
                                                                </div>
                                                                <div class="right">
                                                                    <h6>Downers Grove, IL</h6>
                                                                </div>
                                                            </div>
                                                        </li>
                                                        <li>
                                                            <div class="details">
                                                                <div class="left">
                                                                    <h6>zip</h6>
                                                                </div>
                                                                <div class="right">
                                                                    <h6>60515</h6>
                                                                </div>
                                                            </div>
                                                        </li>
                                                    </ul>
                                                </div>
                                                <div class="dashboard-title mt-lg-5 mt-3">
                                                    <h4>login details</h4>
                                                    <a class="edit-link" href="#">edit</a>
                                                </div>
                                                <div class="dashboard-detail">
                                                    <ul>
                                                        <li>
                                                            <div class="details">
                                                                <div class="left">
                                                                    <h6>Email Address</h6>
                                                                </div>
                                                                <div class="right">
                                                                    <h6>mark.jecno@gmail.com <a class="edit-link"
                                                                            href="#">edit</a></h6>
                                                                </div>
                                                            </div>
                                                        </li>
                                                        <li>
                                                            <div class="details">
                                                                <div class="left">
                                                                    <h6>Phone No.</h6>
                                                                </div>
                                                                <div class="right">
                                                                    <h6>+01 4485 5454<a class="edit-link"
                                                                            href="#">Edit</a></h6>
                                                                </div>
                                                            </div>
                                                        </li>
                                                        <li>
                                                            <div class="details">
                                                                <div class="left">
                                                                    <h6>Password</h6>
                                                                </div>
                                                                <div class="right">
                                                                    <h6>******* <a class="edit-link" href="#">Edit</a>
                                                                    </h6>
                                                                </div>
                                                            </div>
                                                        </li>
                                                    </ul>
                                                </div>
                                            </div>
                                        </div>
                                    </div>
                                </div>
                            </div>
                        </div>
                        <div class="tab-pane fade" id="security">
                            <div class="row">
                                <div class="col-12">
                                    <div class="card mt-0">
                                        <div class="card-body">
                                            <div class="dashboard-box">
                                                <div class="dashboard-title">
                                                    <h4>settings</h4>
                                                </div>
                                                <div class="dashboard-detail">
                                                    <div class="account-setting">
                                                        <h5>Notifications</h5>
                                                        <div class="row">
                                                            <div class="col">
                                                                <div class="form-check">
                                                                    <input class="radio_animated form-check-input"
                                                                        type="radio" name="exampleRadios"
                                                                        id="exampleRadios1" value="option1" checked>
                                                                    <label class="form-check-label"
                                                                        for="exampleRadios1">
                                                                        Allow Desktop Notifications
                                                                    </label>
                                                                </div>
                                                                <div class="form-check">
                                                                    <input class="radio_animated form-check-input"
                                                                        type="radio" name="exampleRadios"
                                                                        id="exampleRadios2" value="option2">
                                                                    <label class="form-check-label"
                                                                        for="exampleRadios2">
                                                                        Enable Notifications
                                                                    </label>
                                                                </div>
                                                                <div class="form-check">
                                                                    <input class="radio_animated form-check-input"
                                                                        type="radio" name="exampleRadios"
                                                                        id="exampleRadios3" value="option3">
                                                                    <label class="form-check-label"
                                                                        for="exampleRadios3">
                                                                        Get notification for my own activity
                                                                    </label>
                                                                </div>
                                                                <div class="form-check">
                                                                    <input class="radio_animated form-check-input"
                                                                        type="radio" name="exampleRadios"
                                                                        id="exampleRadios4" value="option4">
                                                                    <label class="form-check-label"
                                                                        for="exampleRadios4">
                                                                        DND
                                                                    </label>
                                                                </div>
                                                            </div>
                                                        </div>
                                                    </div>
                                                    <div class="account-setting">
                                                        <h5>deactivate account</h5>
                                                        <div class="row">
                                                            <div class="col">
                                                                <div class="form-check">
                                                                    <input class="radio_animated form-check-input"
                                                                        type="radio" name="exampleRadios1"
                                                                        id="exampleRadios4" value="option4" checked>
                                                                    <label class="form-check-label"
                                                                        for="exampleRadios4">
                                                                        I have a privacy concern
                                                                    </label>
                                                                </div>
                                                                <div class="form-check">
                                                                    <input class="radio_animated form-check-input"
                                                                        type="radio" name="exampleRadios1"
                                                                        id="exampleRadios5" value="option5">
                                                                    <label class="form-check-label"
                                                                        for="exampleRadios5">
                                                                        This is temporary
                                                                    </label>
                                                                </div>
                                                                <div class="form-check">
                                                                    <input class="radio_animated form-check-input"
                                                                        type="radio" name="exampleRadios1"
                                                                        id="exampleRadios6" value="option6">
                                                                    <label class="form-check-label"
                                                                        for="exampleRadios6">
                                                                        other
                                                                    </label>
                                                                </div>
                                                                <button type="button"
                                                                    class="btn btn-solid btn-xs">Deactivate
                                                                    Account</button>
                                                            </div>
                                                        </div>
                                                    </div>
                                                    <div class="account-setting">
                                                        <h5>Delete account</h5>
                                                        <div class="row">
                                                            <div class="col">
                                                                <div class="form-check">
                                                                    <input class="radio_animated form-check-input"
                                                                        type="radio" name="exampleRadios3"
                                                                        id="exampleRadios7" value="option7" checked>
                                                                    <label class="form-check-label"
                                                                        for="exampleRadios7">
                                                                        No longer usable
                                                                    </label>
                                                                </div>
                                                                <div class="form-check">
                                                                    <input class="radio_animated form-check-input"
                                                                        type="radio" name="exampleRadios3"
                                                                        id="exampleRadios8" value="option8">
                                                                    <label class="form-check-label"
                                                                        for="exampleRadios8">
                                                                        Want to switch on other account
                                                                    </label>
                                                                </div>
                                                                <div class="form-check">
                                                                    <input class="radio_animated form-check-input"
                                                                        type="radio" name="exampleRadios3"
                                                                        id="exampleRadios9" value="option9">
                                                                    <label class="form-check-label"
                                                                        for="exampleRadios9">
                                                                        other
                                                                    </label>
                                                                </div>
                                                                <button type="button"
                                                                    class="btn btn-solid btn-xs">Delete Account</button>
                                                            </div>
                                                        </div>
                                                    </div>
                                                </div>
                                            </div>
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
    <!--  dashboard section end -->
</asp:Content>
