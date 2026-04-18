<%@ Page Language="C#" MasterPageFile="~/MasterPage.Master" AutoEventWireup="true" CodeBehind="OrderCreate.aspx.cs" Inherits="ImageSolutionsWebsite.Admin.OrderCreate" %>
<%@ Register Src="~/Control/AdminNavigation.ascx" TagPrefix="uc" TagName="AdminNavigation" %>
<%@ Register src="~/Control/MyAccountSearchModal.ascx" tagname="MyAccountSearchModal" tagprefix="uc2"  %>

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
                                                <h3>Create Order</h3>
                                            </div>
                                            <div class="top-sec">
                                                <table>
                                                    <tr>
                                                        <td>Order File: <a href="/Admin/OrderFiles/Template/ABS.xlsx" target="_blank">[Download Template]</a></td>
                                                        <%--<td>Batch Number: <asp:TextBox ID="txtBatchNumber" runat="server"></asp:TextBox><asp:RequiredFieldValidator ID="reqBatchNumber" runat="server" ControlToValidate="txtBatchNumber" Display="None" ErrorMessage="*" ForeColor="Red"></asp:RequiredFieldValidator></td>--%>
                                                        <td><asp:FileUpload ID="fuOrderFile" runat="server" /><asp:Literal ID="litPostedFileName" runat="server" Visible="false"></asp:Literal></td>
                                                        <td><asp:LinkButton ID="lbnUpload" runat="server" CssClass="btn btn-sm btn-solid" OnClick="lbnUpload_Click" CausesValidation="false">Upload File</asp:LinkButton></td>
                                                    </tr>
                                                    <tr>
                                                        <td>Shipping Method: <asp:DropDownList ID="ddlWebsiteShippingServices" runat="server" DataTextField="Description" DataValueField="WebsiteShippingServiceID"></asp:DropDownList><asp:RequiredFieldValidator ID="reqWebsiteShippingServices" runat="server" ControlToValidate="ddlWebsiteShippingServices" Display="Dynamic" ErrorMessage="Please select shipping method" ForeColor="Red"></asp:RequiredFieldValidator></td>
                                                        <td><asp:CheckBox ID="chkReceivesConfirmation" runat="server" Checked="true" />Receives Confirmation</td>
                                                        <td><asp:LinkButton ID="lbnCreateOrder" runat="server" CssClass="btn btn-sm btn-solid" OnClick="lbnCreateOrder_Click" CausesValidation="true">Create Orders</asp:LinkButton></td>
                                                    </tr>
                                                </table>
                                            </div>
                                            <div class="table-responsive-xl">
                                                <asp:GridView ID="gvSalesOrderLines" runat="server" AutoGenerateColumns="False" DataKeyNames="Reference" CssClass="table cart-table wishlist-table" HeaderStyle-CssClass="table-head" Width="100%" 
                                                    GridLines="None" AllowSorting="true" CellSpacing="0" CellPadding="0">
                                                    <Columns>
                                                        <asp:BoundField HeaderText="Order #" DataField="OrderNumber" />
                                                        <asp:BoundField HeaderText="NetSuite Item" DataField="NetSuiteItem" />
                                                        <asp:BoundField HeaderText="Quantity" DataField="Quantity" />
                                                        <asp:BoundField HeaderText="Gender" DataField="Gender" />
                                                        <asp:BoundField HeaderText="Size" DataField="Size" />
                                                        <asp:BoundField HeaderText="Memo" DataField="Memo" />
                                                        <asp:BoundField HeaderText="Reference" DataField="Reference" />
                                                        <asp:TemplateField HeaderText="Shipping" ItemStyle-HorizontalAlign="Center" HeaderStyle-HorizontalAlign="Center">
                                                            <ItemTemplate>
                                                                <%# Eval("ShippingCarrier")%><br />
                                                                <%# Eval("ShipVia")%><br />
                                                            </ItemTemplate>
                                                        </asp:TemplateField>
                                                        <asp:TemplateField HeaderText="Address" ItemStyle-HorizontalAlign="Center" HeaderStyle-HorizontalAlign="Center">
                                                            <ItemTemplate>
                                                                <%# Eval("ShippingAttention")%><br />
                                                                <%# Eval("ShippingAddressee")%><br />
                                                                <%# Eval("ShippingAddressLine1")%><br />
                                                                <%# Eval("ShippingAddressLine2")%><br />
                                                                <%# Eval("ShippingCity")%>, <%# Eval("ShippingState")%> <%# Eval("ShippingZip")%><br />
                                                            </ItemTemplate>
                                                        </asp:TemplateField>
                                                        <asp:TemplateField HeaderText="NetSuite Customer" ItemStyle-HorizontalAlign="Center" HeaderStyle-HorizontalAlign="Center">
                                                            <ItemTemplate>
                                                                <a href='https://acct88641-sb1.app.netsuite.com/app/common/entity/custjob.nl?id=<%# Eval("InternalID")%>' title='<%# Eval("Name")%>' target="_blank">Link</a>
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

<asp:Content ID="Content4" ContentPlaceHolderID="cphFooter" runat="server">
    <uc2:MyAccountSearchModal ID="ucMyAccountSearchModal" runat="server" />
</asp:Content>
