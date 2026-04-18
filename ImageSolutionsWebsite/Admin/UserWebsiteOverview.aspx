<%@ Page Language="C#" MasterPageFile="~/MasterPage.Master" AutoEventWireup="true" CodeBehind="UserWebsiteOverview.aspx.cs" Inherits="ImageSolutionsWebsite.Admin.UserWebsiteOverview" %>
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
                                                <h3>User Management</h3> 
                                                
                                                <a href="/Admin/userwebsite.aspx" class="btn btn-sm btn-solid">+ add new</a>
                                                <a href="/Admin/userwebsiteimport.aspx" class="btn btn-sm btn-solid">+ import</a>
                                                <asp:LinkButton ID="btnExport" runat="server" class="btn btn-sm btn-solid" OnClick="btnExport_Click">export</asp:LinkButton>
                                                <asp:LinkButton ID="btnImportBudgetProgram" runat="server" class="btn btn-sm btn-solid" OnClick="btnImportBudgetProgram_Click" Visible="false">Import Budget Program</asp:LinkButton>
                                            </div>
                                            <asp:Panel ID="pnlAdminFunction" runat="server" CssClass="top-sec" Visible ="false">
                                                <div class="col-lg-12 text-end">
                                                    <ul class="header-dropdown">
                                                        <li class="onhover-dropdown">
                                                            <i class="fa fa-gear" aria-hidden="true"></i>
                                                            <ul class="onhover-show-div">
                                                                <li style="display:flex; width:150px;"><a href="/Admin/AdminUserWebsiteImport.aspx">admin - import user</a></li>
                                                                <li style="display:flex; width:150px;"><a href="/Admin/AdminAddressImport.aspx">admin - import address</a></li>
                                                            </ul>
                                                        </li>
                                                    </ul>      
                                                </div> 
                                            </asp:Panel>

                                            <div class="top-sec" id="filter" runat="server" visible="true">

                                                <div class="col-md-3">
                                                    <div>
                                                        First Name/Org:
                                                    </div>
                                                    <div>
                                                        <asp:TextBox ID="txtFirstName" runat="server" placeholder="first name" required="" AutoPostBack="true" OnTextChanged="txtFilter_TextChanged"></asp:TextBox>&nbsp;
                                                    </div>
                                                </div>

                                                <div class="col-md-3">
                                                    <div>
                                                        Last Name/Store#:
                                                    </div>
                                                    <div>
                                                        <asp:TextBox ID="txtLastName" runat="server" placeholder="last name" required="" AutoPostBack="true" OnTextChanged="txtFilter_TextChanged"></asp:TextBox>&nbsp;
                                                    </div>
                                                </div>

                                                <div class="col-md-3">
                                                    <div>
                                                        <asp:Label ID="lblEmail" runat="server" Text="Email:"></asp:Label>
                                                    </div>
                                                    <div>
                                                        <asp:TextBox ID="txtEmail" runat="server" placeholder="email/employee id" required="" AutoPostBack="true" OnTextChanged="txtFilter_TextChanged"></asp:TextBox>&nbsp;
                                                    </div>
                                                </div>

                                                <div class="col-md-3">
                                                    <div>
                                                        &nbsp;&nbsp;
                                                    </div>
                                                    <div>
                                                        Pending Approval:&nbsp;&nbsp;<asp:CheckBox ID="chkPendingApproval" runat="server" AutoPostBack="true" OnCheckedChanged="txtFilter_TextChanged"/>
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
                                                    <asp:Panel ID="pnlSearchMessage" runat="server">
                                                        <span style="font-size:small">click on magnifying glass to search your account</span>
                                                    </asp:Panel>
                                                    <br />
                                                </div>
                                                <div class="col-md-3">
                                                    <asp:LinkButton ID="btnAccountSearch" runat="server" OnClick="btnAccountSearch_Click" Enabled="false"><i class="ti-search" style="font-size:large"></i></asp:LinkButton>
                                                    <asp:LinkButton ID="btnAccountRemove" runat="server" OnClick="btnAccountRemove_Click" Enabled="false"><i class="ti-trash" style="font-size:large"></i></asp:LinkButton>
                                                </div>
                                            </asp:Panel>


                                            <div class="table-responsive-xl">
                                                <asp:GridView runat="server" ID="gvUserWebsites" AutoGenerateColumns="False" CssClass="table cart-table order-table" HeaderStyle-CssClass="table-head" GridLines="None" Width="100%" AllowSorting="true" CellSpacing="0" CellPadding="0" OnRowDataBound="gvUserWebsites_RowDataBound">
                                                    <Columns>
                                                        <asp:BoundField DataField="FirstName" HeaderText="First Name" HeaderStyle-HorizontalAlign="Left" ItemStyle-HorizontalAlign="Left" />
                                                        <asp:BoundField DataField="LastName" HeaderText="LastName" HeaderStyle-HorizontalAlign="Left" ItemStyle-HorizontalAlign="Left" />
                                                        <asp:BoundField DataField="EmailAddress" HeaderText="Email" HeaderStyle-HorizontalAlign="Left" ItemStyle-HorizontalAlign="Left" />
                                                        <asp:BoundField DataField="EmployeeID" HeaderText="Employee ID" HeaderStyle-HorizontalAlign="Left" ItemStyle-HorizontalAlign="Left" />
                                                        <%--<asp:BoundField DataField="Division" HeaderText="Division" HeaderStyle-HorizontalAlign="Left" ItemStyle-HorizontalAlign="Left" />
                                                        <asp:BoundField DataField="Military" HeaderText="Military" HeaderStyle-HorizontalAlign="Left" ItemStyle-HorizontalAlign="Left" />--%>
                                                        <asp:BoundField DataField="IsAdmin" HeaderText="Is Account Admin" HeaderStyle-HorizontalAlign="Left" ItemStyle-HorizontalAlign="Left" />
                                                        <asp:BoundField DataField="IsPendingApproval" HeaderText="Pending Approval" HeaderStyle-HorizontalAlign="Left" ItemStyle-HorizontalAlign="Left" />
                                                        <asp:TemplateField HeaderText="Created Date" HeaderStyle-HorizontalAlign="Center" ItemStyle-HorizontalAlign="Center">
                                                            <ItemTemplate>
                                                                <%# Eval("CreatedOn") %>
                                                            </ItemTemplate>
                                                        </asp:TemplateField>
                                                        <asp:TemplateField HeaderText="VIEW">
                                                            <ItemTemplate>
                                                                <a href='/admin/userwebsite.aspx?id=<%# Eval("UserWebsiteID") %>'>
                                                                    <i class="fa fa-eye text-theme"></i>
                                                                </a>
                                                            </ItemTemplate>
                                                        </asp:TemplateField>
                                                    </Columns>
                                                </asp:GridView>
                                                <uc1:Pager runat="server" ID="ucPager" PagingMode="Redirect" PageSize="20" PagingRecordText="User" />
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

