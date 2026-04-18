<%@ Page Title="" Language="C#" MasterPageFile="~/MasterPage.Master" AutoEventWireup="true" CodeBehind="AddressBook.aspx.cs" Inherits="ImageSolutionsWebsite.MyAccount.AddressBook" %>

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
                        <div class="tab-pane fade show active" id="orders">
                            <div class="row">
                                <div class="col-12">
                                    <div class="card dashboard-table mt-0">
                                        <div class="card-body table-responsive-sm">
                                            <div class="top-sec">
                                                <h3>Address Book</h3>
                                                <a href="/myaccount/AddressBookEditMode.aspx" class="btn btn-sm btn-solid">+ add new</a>
                                            </div>
                                            <div>                                                                                                
                                                <a href="/MyAccount/AddressBookImport.aspx" class="btn btn-sm btn-solid">import</a>
                                                <asp:LinkButton ID="btnExport" runat="server" class="btn btn-sm btn-solid" OnClick="btnExport_Click">export</asp:LinkButton>
                                            </div>
                                            <asp:GridView ID="gvAddressBook" runat="server" AutoGenerateColumns="False" DataKeyNames="AddressBookID" CssClass="table cart-table wishlist-table" HeaderStyle-CssClass="table-head" Width="100%" 
                                                GridLines="None" AllowSorting="true" CellSpacing="0" CellPadding="0" OnRowCommand="gvAddressBook_RowCommand" OnRowDataBound="gvAddressBook_RowDataBound" ignore>
                                                <Columns>
                                                    <asp:TemplateField HeaderText="Company Name" ItemStyle-HorizontalAlign="Center" HeaderStyle-HorizontalAlign="Center">
                                                        <ItemTemplate>
                                                            <%# string.Format("{0:c}", Eval("AddressLabel"))%>
                                                        </ItemTemplate>
                                                    </asp:TemplateField>
                                                    <asp:TemplateField HeaderText="Name" ItemStyle-HorizontalAlign="Center" HeaderStyle-HorizontalAlign="Center">
                                                        <ItemTemplate>
                                                            <%# string.Format("{0:c}", Eval("FullName"))%>
                                                        </ItemTemplate>
                                                    </asp:TemplateField>
                                                    <asp:TemplateField HeaderText="Address" HeaderStyle-HorizontalAlign="Center" ItemStyle-HorizontalAlign="Center">
                                                        <ItemTemplate>
                                                            <%# string.Format("{0:c}", Eval("AddressLine1"))%>
                                                        </ItemTemplate>
                                                    </asp:TemplateField>
                                                    <asp:TemplateField HeaderText="City" HeaderStyle-HorizontalAlign="Center" ItemStyle-HorizontalAlign="Center">
                                                        <ItemTemplate>
                                                                <%# string.Format("{0:c}", Eval("City"))%>
                                                        </ItemTemplate>
                                                    </asp:TemplateField>
                                                    <asp:TemplateField HeaderText="State" HeaderStyle-HorizontalAlign="Center" ItemStyle-HorizontalAlign="Center">
                                                        <ItemTemplate>
                                                                <%# string.Format("{0:c}", Eval("State"))%>
                                                        </ItemTemplate>
                                                    </asp:TemplateField>
                                                    <asp:TemplateField HeaderText="Zip" HeaderStyle-HorizontalAlign="Center"  ItemStyle-HorizontalAlign="Center">
                                                        <ItemTemplate>
                                                                <%# string.Format("{0:c}", Eval("PostalCode"))%>
                                                        </ItemTemplate>
                                                    </asp:TemplateField>
                                                    <asp:TemplateField HeaderText="Country" HeaderStyle-HorizontalAlign="Center"  ItemStyle-HorizontalAlign="Center">
                                                        <ItemTemplate>
                                                                <%# string.Format("{0:c}", Eval("CountryCode"))%>
                                                        </ItemTemplate>
                                                    </asp:TemplateField>
                                                    <asp:TemplateField HeaderText="Default Shipping" HeaderStyle-HorizontalAlign="Center"  ItemStyle-HorizontalAlign="Center">
                                                        <ItemTemplate>
                                                            <asp:CheckBox ID="cbDefaultShipping" runat="server" Enabled="false"/>
                                                        </ItemTemplate>
                                                    </asp:TemplateField>
                                                    <asp:TemplateField HeaderText="Default Billing" HeaderStyle-HorizontalAlign="Center"  ItemStyle-HorizontalAlign="Center">
                                                        <ItemTemplate>
                                                            <asp:CheckBox ID="cbDefaultBilling" runat="server" Enabled="false"/>
                                                        </ItemTemplate>
                                                    </asp:TemplateField>
                                                    <asp:TemplateField HeaderText="EDIT">
                                                        <ItemTemplate>
                                                            <a href='/myaccount/addressbookeditmode.aspx?AddressBookID=<%# Eval("AddressBookID") %>'>
                                                                <i class="fa fa-edit text-theme"></i>
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
    </section>
    <!--  dashboard section end -->

</asp:Content>
