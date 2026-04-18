<%@ Page Language="C#" MasterPageFile="~/MasterPage.Master"  AutoEventWireup="true" CodeBehind="ItemDetail.aspx.cs" Inherits="ImageSolutionsWebsite.Admin.ItemDetail" %>
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
                        <div class="tab-pane fade show active" id="websites">
                            <div class="row">
                                <div class="col-sm-12 col-lg-12">
                                    <section class="tab-product m-0">
                                        <ul class="nav nav-tabs nav-material" id="top-tab" role="tablist">
                                            <li class="nav-item">
                                                <a class="nav-link active" id="top_1_tab" runat="server" data-bs-toggle="tab"
                                                    href="#cphBody_top_1" role="tab" aria-selected="false">
                                                    <i class="icofont icofont-ui-home"></i>Product Attribute Information
                                                </a>
                                                <div class="material-border"></div>
                                            </li>
                                            <li class="nav-item">
                                                <a class="nav-link" id="top_2_tab" runat="server" data-bs-toggle="tab"
                                                    href="#cphBody_top_2" role="tab" aria-selected="true">
                                                    <i class="icofont icofont-man-in-glasses"></i>Product Attribute Values
                                                </a>
                                                <div class="material-border"></div>
                                            </li>
                                        </ul>
                                    </section><br />
                                    <div class="tab-content nav-material">
                                        <div class="tab-pane fade show active" id="top_1" runat="server" role="tabpanel" aria-labelledby="top_1_tab">
                                            <div class="card dashboard-table mt-0">
                                                <div class="card-body table-responsive-sm">
                                                    <%--<div class="top-sec">
                                                        <h3>User Website Management</h3> 
                                                    </div>--%>
                                                    <div class="table-responsive-xl">
                                                        <div class="theme-form">
                                                            <div class="form-row row">
                                                                <div class="col-md-12">
                                                                    <label>Product Attribute Name</label>
                                                                    <asp:TextBox ID="txtAttribute" runat="server" CssClass="form-control" placeholder="attribute name" required=""></asp:TextBox>
                                                                    <asp:RequiredFieldValidator ID="valName" runat="server" ControlToValidate="txtAttribute" ValidationGroup="Submit" ErrorMessage="*" ForeColor="Red"></asp:RequiredFieldValidator>
                                                                </div>
                                                                <div class="col-md-12" style="display:none;">
                                                                    <label>Sort</label>
                                                                    <asp:TextBox ID="txtSort" runat="server" CssClass="form-control" placeholder="sort" required="" onkeydown = "return (!(event.keyCode>=65) && event.keyCode!=32);" Text="0" ValidationGroup="Submit"></asp:TextBox>
                                                                </div>
                                                                <div class="col-md-12">
                                                                    <asp:Button ID="btnSave" runat="server" Text="Save" OnClick="btnSave_Click" CssClass="btn btn-sm btn-solid" ValidationGroup="Submit" />
                                                                    <a id="aCancel" runat="server" href="/Admin/Item.aspx?id=<%= mItemID %>" class="btn btn-sm btn-solid">Cancel</a>
                                                                    <asp:Button ID="btnDelete" runat="server" Text="Delete" OnClick="btnDelete_Click" CssClass="btn btn-sm btn-solid" CausesValidation="false"/>
                                                                </div>
                                                            </div>
                                                        </div>
                                                    </div>
                                                </div>
                                            </div>
                                        </div>
                                        <div class="tab-pane fade" id="top_2" runat="server" role="tabpanel" aria-labelledby="top_2_tab">
                                            <div class="row">
                                                <div class="col-12">
                                                    <div class="card dashboard-table mt-0">
                                                        <div class="card-body table-responsive-sm">
                                                            <div class="top-sec">
                                                                <h3></h3>
                                                                <a href="/admin/itemdetailvalue.aspx?itemdetailid=<%= mItemDetailID %>" class="btn btn-sm btn-solid">+ add new</a>
                                                            </div>
                                                            <div class="table-responsive-xl">
                                                                <asp:GridView runat="server" ID="gvItemDetailValue" AutoGenerateColumns="False" CssClass="table cart-table order-table" HeaderStyle-CssClass="table-head" GridLines="None" Width="100%" AllowSorting="true" CellSpacing="0" CellPadding="0">
                                                                    <Columns>
                                                                        <asp:BoundField DataField="Value" HeaderText="Value" HeaderStyle-HorizontalAlign="Left" ItemStyle-HorizontalAlign="Left" />
                                                                        <asp:TemplateField HeaderText="Created By" HeaderStyle-HorizontalAlign="Center" ItemStyle-HorizontalAlign="Center">
                                                                            <ItemTemplate>
                                                                                <%# Eval("CreatedByUser.FullName") %>
                                                                            </ItemTemplate>
                                                                        </asp:TemplateField>
                                                                        <asp:TemplateField HeaderText="Created Date" HeaderStyle-HorizontalAlign="Center" ItemStyle-HorizontalAlign="Center">
                                                                            <ItemTemplate>
                                                                                <%# Eval("CreatedOn") %>
                                                                            </ItemTemplate>
                                                                        </asp:TemplateField>
                                                                        <asp:TemplateField HeaderText="VIEW">
                                                                            <ItemTemplate>
                                                                                <a href='/admin/itemdetailvalue.aspx?id=<%# Eval("ItemDetailValueID") %>'>
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
                    </div>
                </div>
            </div>
        </div>
    </section>
    <!--  dashboard section end -->
</asp:Content>
