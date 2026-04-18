<%@ Page Language="C#" MasterPageFile="~/MasterPage.Master" AutoEventWireup="true" CodeBehind="CreditCardOverview.aspx.cs" Inherits="ImageSolutionsWebsite.Admin.CreditCardOverview" %>
<%@ Register Src="~/Control/AdminNavigation.ascx" TagPrefix="uc" TagName="AdminNavigation" %>

<asp:Content ID="cHead" ContentPlaceHolderID="head" runat="server">
</asp:Content>
<asp:Content ID="cHeader" ContentPlaceHolderID="cphHeader" runat="server">
</asp:Content>
<asp:Content ID="cBody" ContentPlaceHolderID="cphBody" runat="server">
    <!--  dashboard section start -->
    <section class="dashboard-section section-b-space user-dashboard-section">
        <div class="container">
            <div class="row">
                <uc:AdminNavigation runat="server" id="AdminNavigation" />
                <div class="col-lg-9">
                    <div class="faq-content tab-content" id="top-tabContent">
                        <div class="tab-pane fade show active" id="orders">
                            <div class="row">
                                <div class="col-12">
                                    <div class="card dashboard-table mt-0">
                                        <div class="card-body table-responsive-sm">
                                            <div class="top-sec">
                                                <h3>Credit Card Management</h3>
                                                <a href="/admin/creditcard.aspx" class="btn btn-sm btn-solid">+ add new</a>
                                            </div>
                                            <div class="table-responsive-xl">
                                                <asp:GridView runat="server" ID="gvCreditCard" AutoGenerateColumns="False" CssClass="table cart-table order-table" HeaderStyle-CssClass="table-head" Width="100%" AllowSorting="true" GridLines="None" CellSpacing="0" CellPadding="0">
                                                    <Columns>
                                                        <asp:BoundField DataField="CreditCard.Nickname" HeaderText="Nickname" HeaderStyle-HorizontalAlign="Left" ItemStyle-HorizontalAlign="Left" />
                                                        <asp:BoundField DataField="CreditCard.FullName" HeaderText="Full Name" HeaderStyle-HorizontalAlign="Left" ItemStyle-HorizontalAlign="Left" />
                                                        <asp:TemplateField HeaderText="Card Number" HeaderStyle-HorizontalAlign="Center" ItemStyle-HorizontalAlign="Center">
                                                            <ItemTemplate>
                                                                XXXXXXXXXXXX<%# Eval("CreditCard.LastFourDigit") %>
                                                            </ItemTemplate>
                                                        </asp:TemplateField>
                                                        <asp:BoundField DataField="CreditCard.CreditCardType" HeaderText="Credit Card Type" HeaderStyle-HorizontalAlign="Left" ItemStyle-HorizontalAlign="Left" />
                                                        <asp:TemplateField HeaderText="Expiration" HeaderStyle-HorizontalAlign="Center" ItemStyle-HorizontalAlign="Center">
                                                            <ItemTemplate>
                                                                <%# Eval("CreditCard.ExpirationMonth") %> /<%# Eval("CreditCard.ExpirationYear") %>
                                                            </ItemTemplate>
                                                        </asp:TemplateField>
                                                        <asp:TemplateField HeaderText="View" HeaderStyle-HorizontalAlign="Center" ItemStyle-HorizontalAlign="Center">
                                                            <ItemTemplate>
                                                                <a href='/admin/creditcard.aspx?id=<%# Eval("CreditCardID") %>'>
                                                                    <i class="fa fa-eye text-theme"></i>
                                                                </a>
                                                            </ItemTemplate>
                                                        </asp:TemplateField>
                                                    </Columns>
                                                </asp:GridView>
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