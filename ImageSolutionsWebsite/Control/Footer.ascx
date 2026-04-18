<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="Footer.ascx.cs" Inherits="ImageSolutionsWebsite.Controls.Footer" %>
<!-- footer start -->
<footer class="footer-light">
    <section class="section-b-space light-layout">
        <div class="container">
            <div class="row footer-theme partition-f">
                <div class="col-lg-4 col-md-6">
                    <div class="footer-title footer-mobile-title">
                        <h4>about</h4>
                    </div>
                    <div class="footer-contant">
                        <div class="footer-logo"><img id="imgLogo" runat="server" src="/assets/images/icon/logo.png" style="max-width:280px;max-height:100px;"
                                    class="img-fluid blur-up lazyload" alt=""><%--<img src="/assets/images/icon/logo.png?1=1" alt="">--%></div>
                        <%--<div class="footer-social">
                            <ul>
                            <li>
                                <a href="#"><img src="/assets/images/icon/visa.png" alt=""></a>
                            </li>
                            <li>
                                <a href="#"><img src="/assets/images/icon/mastercard.png" alt=""></a>
                            </li>
                            <li>
                                <a href="#"><img src="/assets/images/icon/paypal.png" alt=""></a>
                            </li>
                            <li>
                                <a href="#"><img src="/assets/images/icon/american-express.png" alt=""></a>
                            </li>
                            <li>
                                <a href="#"><img src="/assets/images/icon/discover.png" alt=""></a>
                            </li>
                        </ul>
                        </div>--%>
                    </div>
                </div>
                <div class="col offset-xl-1">
                    <div class="sub-title">
                        <div class="footer-title">
                            <h4>My Account</h4>
                        </div>
                        <div class="footer-contant">
                            <ul>
                                <li><a href="/MyAccount/Orders.aspx">Order History</a></li>
                                <li><a href="/MyAccount/Profile.aspx">My Profile</a></li>
                                <%--<li><a href="#">clothing</a></li>
                                <li><a href="#">accessories</a></li>
                                <li><a href="#">featured</a></li>--%>
                            </ul>
                        </div>
                    </div>
                </div>               
                <div class="col" id="divUsefulLinks" runat="server">
                    <div class="sub-title">
                        <div class="footer-title">
                            <h4>Useful Links</h4>
                        </div>
                        <div class="footer-contant">
                            <ul>
                                <asp:Repeater ID="rptUsefulLink" runat="server" DataMember="WebsiteUsefulLinkID">
                                    <ItemTemplate>
                                        <li><a href="<%# Eval("URL") %>" target="_blank"><%# Eval("Name") %></a></li>
                                    </ItemTemplate>
                                </asp:Repeater>
                            </ul>
                        </div>
                    </div>
                </div>
                <div class="col">
                    <div class="sub-title">
                        <div class="footer-title">
                            <h4>Policies</h4>
                        </div>
                        <div class="footer-contant">
                            <ul>
                                <li><a href="/PrivacyPolicy.aspx">Privacy Policy</a></li>
                                <%--<li><a href="/StorePolicy.aspx">Store Policy</a></li>--%>
                                <li><a id="aReturnPolicy" runat="server" href="/ReturnPolicy.aspx">Return Policy</a></li>
                            </ul>
                        </div>
                    </div>
                </div>
                <div class="col">
                    <div class="sub-title">
                        <div class="footer-title">
                            <h4>Contact Us</h4>
                        </div>
                        <div class="footer-contant">
                            <ul class="contact-list">
                                <li runat="server" id="liSupportFAQ"><a href="/SupportFAQ.aspx">Support/FAQ</a></li>
                                <li runat="server" id="liNewAccountSetupForm"><a href="../assets/pdf/NewAccountSetupForm.pdf" target="_blank">New Account Setup Form</a></li>
                                <li runat="server" id="liContactUsAddress"><i class="fa fa-map-marker"></i>4692 Brate Drive<br />Suite 300 West <br />Chester, OH 45069</li>
                                <li runat="server" id="liContactUsPhoneNumber"><i class="fa fa-phone"></i>Call Us: (888) 756-9898<br />Hours: 9:00AM – 6:00PM, EST </li>
                                <li runat="server" id="liContactUsPhoneNumberMavis" visible="false"><i class="fa fa-phone"></i>Call Us: (888) 805-3090<br />Hours: 9:00AM – 6:00PM, EST </li>
                                <li><i class="fa fa-envelope"></i>Customer Service: cs@imageinc.com</li>
                            </ul>
                        </div>
                    </div>
                </div>
            </div>
        </div>
    </section>
    <div class="sub-footer">
        <div class="container">
            <div class="row">
                <%--<div class="col-xl-6 col-md-6 col-sm-12">
                    <div class="footer-end">
                        <p><i class="fa fa-copyright" aria-hidden="true"></i> 2017-18 themeforest powered by pixelstrap</p>
                    </div>
                </div>
                <div class="col-xl-6 col-md-6 col-sm-12">
                    <div class="payment-card-bottom">
                        <ul>
                            <li>
                                <a href="#"><img src="/assets/images/icon/visa.png" alt=""></a>
                            </li>
                            <li>
                                <a href="#"><img src="/assets/images/icon/mastercard.png" alt=""></a>
                            </li>
                            <li>
                                <a href="#"><img src="/assets/images/icon/paypal.png" alt=""></a>
                            </li>
                            <li>
                                <a href="#"><img src="/assets/images/icon/american-express.png" alt=""></a>
                            </li>
                            <li>
                                <a href="#"><img src="/assets/images/icon/discover.png" alt=""></a>
                            </li>
                        </ul>
                    </div>
                </div>--%>
            </div>
        </div>
    </div>
</footer>
<!-- footer end -->
