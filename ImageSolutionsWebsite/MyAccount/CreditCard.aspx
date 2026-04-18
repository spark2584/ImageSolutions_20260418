<%@ Page Title="" Language="C#" MasterPageFile="~/MasterPage.Master" AutoEventWireup="true" CodeBehind="CreditCard.aspx.cs" Inherits="ImageSolutionsWebsite.MyAccount.CreditCard" %>
<%@ Register Src="~/Control/MyAccountNavigation.ascx" TagPrefix="uc1" TagName="MyAccountNavigation" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="cphHeader" runat="server">
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="cphBody" runat="server">
    <!--  dashboard section start -->
    <section class="dashboard-section section-b-space user-dashboard-section">
        <div class="container">
            <div class="row">
                <uc1:MyAccountNavigation runat="server" id="MyAccountNavigation" />
                <div class="col-lg-9">
                    <div class="faq-content tab-content" id="top-tabContent">
                        <div class="tab-pane fade show active" id="payment">
                            <div class="row">
                                <div class="col-12">
                                    <div class="card mt-0">
                                        <div class="card-body">
                                            <div class="top-sec">
                                                <h3>Saved Cards</h3>
                                                <a href="/MyAccount/creditcardeditmode.aspx" class="btn btn-sm btn-solid">+ add new</a>
                                            </div>
                                            <div class="address-book-section">
                                                <div class="row g-4">
                                                    <asp:Repeater ID="rptCreditCard" runat="server" OnItemDataBound="rptCreditCard_ItemDataBound" OnItemCommand="rptCreditCard_ItemCommand">
                                                        <ItemTemplate>
                                                            <div class="select-box active col-xl-4 col-md-6">
                                                                <div class="address-box">
                                                                    <div class="bank-logo">
                                                                        <asp:Image ID="imgLogo" runat="server" class="network-logo"/>
                                                                        <asp:HiddenField ID="hfCreditCardType" runat="server" Value='<%# Eval("CreditCard.CreditCardType")%>' Visible="false" />
                                                                    </div>
                                                                    <div class="card-number">
                                                                        <h6>Card Number</h6>
                                                                        <h5>****-****-****-<%# Eval("CreditCard.LastFourDigit")%></h5>
                                                                    </div>
                                                                    <div class="name-validity">
                                                                        <div class="left">
                                                                            <h6>name on card</h6>
                                                                            <h5><%# Eval("CreditCard.FullName")%></h5>
                                                                        </div>
                                                                        <div class="right">
                                                                            <h6>validity</h6>
                                                                            <h5><%# Eval("CreditCard.ExpirationMonth") %> /<%# Eval("CreditCard.ExpirationYear") %></h5>
                                                                        </div>
                                                                    </div>
                                                                    <div class="bottom">
                                                                        <a href="/myaccount/creditcardeditmode.aspx?id=<%# Eval("CreditCardID") %>" class="bottom_btn">edit</a>
                                                                        <asp:LinkButton ID="btnRemove" runat="server" CommandArgument='<%# Eval("UserCreditCardID") %>' CommandName="Remove" CssClass="bottom_btn" Text="remove"></asp:LinkButton>
                                                                    </div>
                                                                </div>
                                                            </div>
                                                        </ItemTemplate>
                                                    </asp:Repeater>
                                                    <%--<div class="select-box active col-xl-4 col-md-6">
                                                        <div class="address-box">
                                                            <div class="bank-logo">
                                                                <img src="../assets/images/bank-logo.png"
                                                                    class="bank-logo">
                                                                <img src="../assets/images/amex.png"
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
                                                                <img src="../assets/images/discover.png"
                                                                    class="bank-logo">
                                                                <img src="../assets/images/mastercard.png"
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
                                                    </div>--%>
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
