<%@ Page Language="C#" MasterPageFile="~/MasterPage.Master" AutoEventWireup="true" CodeBehind="BudgetOverview.aspx.cs" Inherits="ImageSolutionsWebsite.Admin.BudgetOverview" %>
<%@ Register Src="~/Control/AdminNavigation.ascx" TagPrefix="uc" TagName="AdminNavigation" %>
<%@ Register src="~/Control/UserWebsiteSearchModal.ascx" tagname="UserWebsiteSearchModal" tagprefix="uc2"  %>
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
                                                <h3>Employee Credit Management</h3> 
                                                <asp:LinkButton ID="btnExport" runat="server" Text="Export" class="btn btn-sm btn-solid" OnClick="btnExport_Click"></asp:LinkButton>
                                                <a href="/admin/BudgetImport.aspx" id="aBudgetImport" runat="server" class="btn btn-sm btn-solid">import</a>
                                                <a href="/admin/Budget.aspx" id="aBudgetAdd" runat="server" class="btn btn-sm btn-solid">+ add new</a>
                                            </div>
                                            <div class="row" id="filter" runat="server" visible="true">
                                                <div class="col-md-3">
                                                    <div>
                                                        Budget Name:
                                                    </div>
                                                    <div>
                                                        <asp:TextBox ID="txtBudgetName" runat="server" placeholder="budget name" required="" AutoPostBack="true" OnTextChanged="txtBudgetName_TextChanged" Width="100%"></asp:TextBox>
                                                    </div>
                                                </div>
                                                <div class="col-md-3">
                                                    <div>
                                                        <asp:Label ID="lblEmailFilter" runat="server" Text="Email:"></asp:Label>
                                                    </div>
                                                    <div>
                                                        <asp:TextBox ID="txtEmail" runat="server" required="" AutoPostBack="true" OnTextChanged="txtEmail_TextChanged" Width="100%"></asp:TextBox>
                                                    </div>                                               
                                                </div>
                                                <div class="col-md-3">
                                                    <div>
                                                        Division:
                                                    </div>
                                                    <div>
                                                        <asp:TextBox ID="txtDivision" runat="server" placeholder="email" required="" AutoPostBack="true" OnTextChanged="txtDivision_TextChanged" Width="100%"></asp:TextBox>
                                                    </div>                                               
                                                </div>
                                                <div class="col-md-3">
                                                    <div>
                                                        &nbsp;&nbsp;
                                                    </div>
                                                    <div>
                                                        Active Only:&nbsp;&nbsp;<asp:CheckBox ID="chkActiveOnly" runat="server" AutoPostBack="true" OnCheckedChanged="chkActiveOnly_CheckedChanged"/>
                                                    </div>
                                                </div>

                                            </div>
                                            <asp:Panel ID="pnlApproverFilter" runat="server" CssClass="row">
                                                <div class="col-md-12">
                                                    <br />
                                                    <label>Approvers:</label>
                                                </div>
                                                <div class="col-md-9">
                                                    <asp:DropDownList ID="ddlUserWebsite" runat="server" DataTextField="Description" DataValueField="UserWebsiteID" CssClass="form-control" Visible="false"></asp:DropDownList>
                                                    <asp:TextBox ID="txtUserWebsite" runat="server" CssClass="form-control" Enabled="false"></asp:TextBox>
                                                    <asp:HiddenField ID="hfUserWebsiteID" runat="server" />
                                                    <br />
                                                </div>
                                                <div class="col-md-3">
                                                    <asp:LinkButton ID="btnUserWebsiteSearch" runat="server" OnClick="btnUserWebsiteSearch_Click" Enabled="false"><i class="ti-search" style="font-size:large"></i></asp:LinkButton>
                                                    <asp:LinkButton ID="btnUserWebsiteRemove" runat="server" OnClick="btnUserWebsiteRemove_Click" Enabled="false"><i class="ti-trash" style="font-size:large"></i></asp:LinkButton>
                                                </div>
                                            </asp:Panel>
                                            <div class="table-responsive-xl">
                                                <asp:GridView runat="server" ID="gvBudget" AutoGenerateColumns="False" CssClass="table cart-table order-table" HeaderStyle-CssClass="table-head" GridLines="None" Width="100%" AllowSorting="true" CellSpacing="0" CellPadding="0"
                                                    DataKeyNames="BudgetID" OnRowDataBound="gvBudget_RowDataBound" >
                                                    <Columns>
                                                        <asp:BoundField DataField="BudgetName" HeaderText="Budget" HeaderStyle-HorizontalAlign="Left" ItemStyle-HorizontalAlign="Left" />
                                                        <asp:TemplateField HeaderText="Budget Amount" HeaderStyle-HorizontalAlign="Center" ItemStyle-HorizontalAlign="Center">
                                                            <ItemTemplate>
                                                                <%# string.Format("{0:c}", Eval("BudgetAmount")) %>
                                                            </ItemTemplate>
                                                        </asp:TemplateField>
                                                        <asp:TemplateField HeaderText="Start Date" HeaderStyle-HorizontalAlign="Center" ItemStyle-HorizontalAlign="Center">
                                                            <ItemTemplate>
                                                                <%# string.Format("{0:d}", Eval("StartDate")) %>
                                                            </ItemTemplate>
                                                        </asp:TemplateField>
                                                        <asp:TemplateField HeaderText="End Date" HeaderStyle-HorizontalAlign="Center" ItemStyle-HorizontalAlign="Center">
                                                            <ItemTemplate>
                                                                <%# string.Format("{0:d}", Eval("EndDate")) %>
                                                            </ItemTemplate>
                                                        </asp:TemplateField>
                                                        <%--<asp:TemplateField HeaderText="Created By" HeaderStyle-HorizontalAlign="Center" ItemStyle-HorizontalAlign="Center">
                                                            <ItemTemplate>
                                                                <%# Eval("CreatedByUser.FullName") %>
                                                            </ItemTemplate>
                                                        </asp:TemplateField>--%>
                                                        <asp:TemplateField HeaderText="Created Date" HeaderStyle-HorizontalAlign="Center" ItemStyle-HorizontalAlign="Center">
                                                            <ItemTemplate>
                                                                <%# Eval("CreatedOn") %>
                                                            </ItemTemplate>
                                                        </asp:TemplateField>
                                                        <asp:TemplateField HeaderText="View" HeaderStyle-HorizontalAlign="Center" ItemStyle-HorizontalAlign="Center" ItemStyle-Width="20%">
                                                            <ItemTemplate>
<%--                                                                 <a href='/admin/Budget.aspx?id=<%# Eval("BudgetID") %>'>
                                                                    <i class="fa fa-eye text-theme"></i>
                                                                </a>--%>
                                                                <asp:HyperLink ID="hlnkBudget" runat="server"><i class="fa fa-eye text-theme"></i></asp:HyperLink>
                                                            </ItemTemplate>
                                                        </asp:TemplateField>
                                                    </Columns>
                                                </asp:GridView>
                                                <uc1:Pager runat="server" ID="ucPager" PagingMode="Redirect" PageSize="20" PagingRecordText="Budgets" />
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
    <uc2:UserWebsiteSearchModal ID="ucUserWebsiteSearchModal" runat="server" />
</asp:Content>