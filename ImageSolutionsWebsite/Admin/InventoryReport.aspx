<%@ Page Language="C#" MasterPageFile="~/MasterPage.Master" AutoEventWireup="true" CodeBehind="InventoryReport.aspx.cs" Inherits="ImageSolutionsWebsite.Admin.InventoryReport" %>
<%@ Register Src="~/Control/AdminNavigation.ascx" TagPrefix="uc" TagName="AdminNavigation" %>

<%@ Register Src="~/Control/Pager.ascx" TagPrefix="uc1" TagName="Pager" %>

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
                                                <h3>Inventory Report</h3> 
                                                <asp:LinkButton ID="btnExport" runat="server" Text="Export" class="btn btn-sm btn-solid" OnClick="btnExport_Click"></asp:LinkButton>
                                            </div>
                                            <div class="table-responsive-xl">
                                                <asp:GridView runat="server" ID="gvInventoryReport" AutoGenerateColumns="False" CssClass="table cart-table order-table" HeaderStyle-CssClass="table-head" GridLines="None" Width="100%" AllowSorting="true" CellSpacing="0" CellPadding="0">
                                                    <Columns>
                                                        <asp:BoundField DataField="VendorCode" HeaderText="Vendor Code" HeaderStyle-HorizontalAlign="Left" ItemStyle-HorizontalAlign="Left" />
                                                        <%--<asp:BoundField DataField="ItemNumber" HeaderText="Item Number" HeaderStyle-HorizontalAlign="Left" ItemStyle-HorizontalAlign="Left" />--%>
                                                        <asp:BoundField DataField="Name" HeaderText="Name" HeaderStyle-HorizontalAlign="Left" ItemStyle-HorizontalAlign="Left" />
                                                        <asp:BoundField DataField="Color" HeaderText="Color" HeaderStyle-HorizontalAlign="Left" ItemStyle-HorizontalAlign="Left" />
                                                        <asp:BoundField DataField="Size" HeaderText="Size" HeaderStyle-HorizontalAlign="Left" ItemStyle-HorizontalAlign="Left" />
                                                        <asp:BoundField DataField="AvailableInventory" HeaderText="Available" HeaderStyle-HorizontalAlign="Left" ItemStyle-HorizontalAlign="Left" />                                                        
                                                    </Columns>
                                                </asp:GridView>
                                                <uc1:Pager runat="server" ID="ucPager" PagingMode="Redirect" PageSize="20" PagingRecordText="Inventory" />
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