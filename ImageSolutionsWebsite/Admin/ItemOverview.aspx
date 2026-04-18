<%@ Page Language="C#" MasterPageFile="~/MasterPage.Master" AutoEventWireup="true" CodeBehind="ItemOverview.aspx.cs" Inherits="ImageSolutionsWebsite.Admin.ItemOverview" %>
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
                                                <div class="col-lg-6"><h3>Item Management</h3></div> 
                                                <div class="col-lg-6 text-end">
                                                    <ul class="header-dropdown">
                                                        <li class="onhover-dropdown">
                                                            <i class="fa fa-gear" aria-hidden="true"></i>
                                                            <ul class="onhover-show-div">
                                                                <li style="display:flex; width:150px;"><a href="/Admin/Item.aspx">+ add new</a></li>
                                                                <li style="display:flex; width:150px;"><a href="/Admin/ItemDetailImport.aspx">import product detail</a></li>
                                                            </ul>
                                                        </li>
                                                    </ul>      
                                                </div> 

                                                
                                            </div>
                                            <div class="top-sec" id="filter" runat="server" visible="true">
                                                Item Number:&nbsp;<asp:TextBox ID="txtItemNumber" runat="server" placeholder="item number" required="" AutoPostBack="true" OnTextChanged="txtFilter_TextChanged"></asp:TextBox>&nbsp;
                                                Item Name:&nbsp;<asp:TextBox ID="txtItemName" runat="server" placeholder="item name" required="" AutoPostBack="true" OnTextChanged="txtFilter_TextChanged"></asp:TextBox>&nbsp;
                                            </div>
                                            <div class="table-responsive-xl">
                                                <asp:GridView runat="server" ID="gvItems" AutoGenerateColumns="False" CssClass="table cart-table order-table" HeaderStyle-CssClass="table-head" GridLines="None"  Width="100%" AllowSorting="true" CellSpacing="0" CellPadding="0">
                                                    <Columns>
                                                        <asp:BoundField DataField="ItemID" HeaderText="Item ID" HeaderStyle-HorizontalAlign="Left" ItemStyle-HorizontalAlign="Left" />
                                                        <asp:BoundField DataField="ItemNumber" HeaderText="Item Number" HeaderStyle-HorizontalAlign="Left" ItemStyle-HorizontalAlign="Left" />
                                                        <asp:BoundField DataField="ItemName" HeaderText="Item Name" HeaderStyle-HorizontalAlign="Left" ItemStyle-HorizontalAlign="Left" />
                                                        <asp:TemplateField HeaderText="Parent" HeaderStyle-HorizontalAlign="Center" ItemStyle-HorizontalAlign="Center">
                                                            <ItemTemplate>
                                                                <%# Eval("Item.ParentItem.ItemNumber") %>
                                                            </ItemTemplate>
                                                        </asp:TemplateField>
                                                        <asp:TemplateField HeaderText="Is NonInventory" HeaderStyle-HorizontalAlign="Center" ItemStyle-HorizontalAlign="Center">
                                                            <ItemTemplate>
                                                                <asp:CheckBox ID="chkIsNonInventory" runat="server" Checked='<%# Eval("Item.IsNonInventory") %>' Enabled="false" /> 
                                                            </ItemTemplate>
                                                        </asp:TemplateField>
                                                        <asp:TemplateField HeaderText="Is Variation" HeaderStyle-HorizontalAlign="Center" ItemStyle-HorizontalAlign="Center">
                                                            <ItemTemplate>
                                                                <asp:CheckBox ID="chkIsVariation" runat="server" Checked='<%# Eval("Item.IsVariation") %>' Enabled="false" /> 
                                                            </ItemTemplate>
                                                        </asp:TemplateField>
                                                        <%--<asp:HyperLinkField DataTextField="ItemNumber" DataNavigateUrlFields="ItemID" DataNavigateUrlFormatString="Item.aspx?id={0}" HeaderText="ItemNumber" />--%>
                                                        <asp:TemplateField HeaderText="View" HeaderStyle-HorizontalAlign="Center" ItemStyle-HorizontalAlign="Center">
                                                            <ItemTemplate>
                                                                <a href='/admin/item.aspx?id=<%# Eval("ItemID") %>'>
                                                                    <i class="fa fa-eye text-theme"></i>
                                                                </a>
                                                            </ItemTemplate>
                                                        </asp:TemplateField>
                                                    </Columns>
                                                </asp:GridView>
                                                <uc1:Pager runat="server" ID="ucPager" PagingMode="Redirect" PageSize="20" PagingRecordText="Items" />
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