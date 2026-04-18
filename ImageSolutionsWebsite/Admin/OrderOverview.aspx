<%@ Page Language="C#" MasterPageFile="~/MasterPage.Master" AutoEventWireup="true" CodeBehind="OrderOverview.aspx.cs" Inherits="ImageSolutionsWebsite.Admin.OrderOverview" %>
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
                                                <h3>Order Management<%-- |&nbsp;&nbsp;--%></h3>
                                                <a href="/Admin/OrderCreate.aspx" class="btn btn-sm btn-solid" id="aOrderCreate" runat="server">+ create order</a>
                                                <%--<a href="/ShoppingCart.aspx?batch=t" class="btn btn-sm btn-solid">+ add new</a>--%>
                                                <asp:LinkButton ID="btnFilter" runat="server" Text="Show Filters" OnClick="btnFilter_Click" Visible="false"></asp:LinkButton>
                                            </div>




                                            <div class="top-sec" id="filter" runat="server" visible="true">

                                                <div class="col-md-4">
                                                    <div>
                                                        Start Date:
                                                    </div>
                                                    <div>
                                                        <asp:TextBox ID="txtOrderStartDate" runat="server" placeholder="start date" required="" AutoPostBack="true" OnTextChanged="txtDate_TextChanged"></asp:TextBox>&nbsp;
                                                        <asp:CompareValidator ID="CompareValidator1" runat="server" ErrorMessage="Inalidate Start Date Format" ControlToValidate="txtOrderStartDate" Display="None" Operator="DataTypeCheck" Type="Date"></asp:CompareValidator>                                                   
                                                    </div>
                                                </div>

                                                <div class="col-md-4">
                                                    <div>
                                                        End Date:
                                                    </div>
                                                    <div>
                                                        <asp:TextBox ID="txtOrderEndDate" runat="server" placeholder="end date" required="" AutoPostBack="true" OnTextChanged="txtDate_TextChanged"></asp:TextBox>&nbsp;
                                                <asp:CompareValidator ID="CompareValidator2" runat="server" ErrorMessage="Inalidate End Date Format" ControlToValidate="txtOrderEndDate" Display="None" Operator="DataTypeCheck" Type="Date"></asp:CompareValidator>
                                                    </div>
                                                </div>

                                                <div class="col-md-4">
                                                    <div>
                                                        &nbsp;&nbsp;
                                                    </div>
                                                    <div>
                                                        Pending Approval:&nbsp;&nbsp;<asp:CheckBox ID="chkPendingApproval" runat="server" AutoPostBack="true" OnCheckedChanged="txtDate_TextChanged"/>
                                                    </div>
                                                </div>
                                            </div>

                                            <asp:Panel ID="pnlAccountFilter" runat="server" CssClass="row">      
                                                <div class="col-md-12">
                                                    <br />
                                                    <label>Account:</label>
                                                </div>
                                                <div class="col-md-9">
                                                    <asp:DropDownList ID="ddlAccount" runat="server" DataTextField="AccountNamePath" DataValueField="AccountID" CssClass="form-control" OnSelectedIndexChanged="ddlAccount_SelectedIndexChanged" Visible="false" AutoPostBack="true"></asp:DropDownList>
                                                    <asp:TextBox ID="txtAccount" runat="server" CssClass="form-control" Enabled="false"></asp:TextBox>
                                                    <asp:HiddenField ID="hfAccountID" runat="server" />
                                                    <br />
                                                </div>
                                                <div class="col-md-3">
                                                    <asp:LinkButton ID="btnAccountSearch" runat="server" OnClick="btnAccountSearch_Click" Enabled="false"><i class="ti-search" style="font-size:large"></i></asp:LinkButton>
                                                    <asp:LinkButton ID="btnAccountRemove" runat="server" OnClick="btnAccountRemove_Click" Enabled="false"><i class="ti-trash" style="font-size:large"></i></asp:LinkButton>
                                                </div>
                                            </asp:Panel>



                                            <asp:Panel ID="pnlOrderNumberSearch" runat="server" Visible="false">
                                                <div class="top-sec">
                                                    Order Number:&nbsp;<asp:TextBox ID="txtOrderNumber" runat="server" placeholder="order number" required="" AutoPostBack="true" OnTextChanged="txtOrderNumber_TextChanged"></asp:TextBox>&nbsp;                                              
                                                </div>
                                            </asp:Panel>
                                            <div class="top-sec">
                                                <table>
                                                    <tr>
                                                        <td><asp:LinkButton ID="lbnDownloadReconciliation" runat="server" CssClass="btn btn-sm btn-solid" OnClick="lbnDownloadReconciliation_Click">reconciliation report</asp:LinkButton></td>
                                                        <td><asp:LinkButton ID="lbnDownload" runat="server" CssClass="btn btn-sm btn-solid" OnClick="lbnDownload_Click">order report</asp:LinkButton></td>
                                                        <td><asp:LinkButton ID="lbnDownloadItem" runat="server" CssClass="btn btn-sm btn-solid" OnClick="lbnDownloadItem_Click">item report</asp:LinkButton></td>
                                                    </tr>
                                                </table>
                                            </div>
                                            <div class="table-responsive-xl">
                                                <asp:UpdatePanel ID="UpdatePanel1" runat="server">
                                                    <Triggers>
                                                        <asp:AsyncPostBackTrigger ControlID="ddlAccount" />
                                                        <asp:AsyncPostBackTrigger ControlID="txtOrderStartDate" />
                                                        <asp:AsyncPostBackTrigger ControlID="txtOrderEndDate" />
                                                    </Triggers>
                                                    <ContentTemplate>
                                                        <asp:GridView ID="gvSalesOrders" runat="server" AutoGenerateColumns="False" DataKeyNames="SalesOrderID" CssClass="table cart-table wishlist-table" HeaderStyle-CssClass="table-head" Width="100%" 
                                                            GridLines="None" AllowSorting="true" CellSpacing="0" CellPadding="0" OnRowDataBound="gvSalesOrders_RowDataBound">
                                                            <Columns>
                                                                <asp:TemplateField HeaderText="Order Number" ItemStyle-HorizontalAlign="Center" HeaderStyle-HorizontalAlign="Center">
                                                                    <ItemTemplate>
                                                                        <%# string.Format("{0}", Eval("SalesOrderID"))%>
                                                                    </ItemTemplate>
                                                                </asp:TemplateField>
                                                                <asp:TemplateField HeaderText="Transaction Date" HeaderStyle-HorizontalAlign="Center" ItemStyle-HorizontalAlign="Center">
                                                                    <ItemTemplate>
                                                                        <%# string.Format("{0:d}", Eval("TransactionDate"))%>
                                                                    </ItemTemplate>
                                                                 </asp:TemplateField>
                                                                <asp:TemplateField HeaderText="Account" ItemStyle-HorizontalAlign="Center" HeaderStyle-HorizontalAlign="Center">
                                                                    <ItemTemplate>
                                                                        <%# Eval("AccountName")%>
                                                                    </ItemTemplate>
                                                                </asp:TemplateField>
                                                                <asp:TemplateField HeaderText="User" HeaderStyle-HorizontalAlign="Center" ItemStyle-HorizontalAlign="Center">
                                                                    <ItemTemplate>
                                                                        <%# Eval("UserDescription")%>
                                                                    </ItemTemplate>
                                                                </asp:TemplateField>
                                                                <asp:CheckBoxField DataField="IsClosed" HeaderText="Closed" HeaderStyle-HorizontalAlign="Center" ItemStyle-HorizontalAlign="Center"/>
                                                                <asp:CheckBoxField DataField="IsPendingApproval" HeaderText="Pending Approval" HeaderStyle-HorizontalAlign="Center" ItemStyle-HorizontalAlign="Center"/>
                                                                <asp:CheckBoxField DataField="IsRejected" HeaderText="Is Rejected" HeaderStyle-HorizontalAlign="Center" ItemStyle-HorizontalAlign="Center"/>
                                                                <asp:TemplateField HeaderText="Total" HeaderStyle-HorizontalAlign="Center" ItemStyle-HorizontalAlign="Center">
                                                                    <ItemTemplate>
                                                                        <%# string.Format("{0:c}", Eval("Total"))%>
                                                                    </ItemTemplate>
                                                                 </asp:TemplateField>
                                   <%--                             <asp:TemplateField HeaderText="CreatedOn" HeaderStyle-HorizontalAlign="Center" ItemStyle-HorizontalAlign="Center">
                                                                    <ItemTemplate>
                                                                        <%# string.Format("{0:d}", Eval("CreatedOn"))%>
                                                                    </ItemTemplate>
                                                                 </asp:TemplateField>--%>

                                                                <asp:TemplateField HeaderText="Invoice File">
                                                                    <ItemTemplate>
                                                                        <asp:HyperLink ID="lnkInvoiceFile" runat="server" Target="_blank" NavigateUrl='<%#Eval("InvoiceFilePath") %>'><i class="fa fa-file text-theme"></i></asp:HyperLink>
                                                                    </ItemTemplate>
                                                                </asp:TemplateField>

                                                                <asp:TemplateField HeaderText="VIEW">
                                                                    <ItemTemplate>
                                                                        <asp:HiddenField ID="hfSalesOrderID" runat="server" Value='<%# Eval("SalesOrderID")%>'/>
                                                                        <a href='/admin/Order.aspx?id=<%# Eval("SalesOrderID") %>'>
                                                                            <i class="fa fa-eye text-theme"></i>
                                                                        </a>
                                                                    </ItemTemplate>
                                                                </asp:TemplateField>
                                                            </Columns>
                                                        </asp:GridView>
                                                         <uc1:Pager runat="server" ID="ucPager" PagingMode="Redirect" PageSize="20" PagingRecordText="Orders" />
                                                    </ContentTemplate>
                                                </asp:UpdatePanel>
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
