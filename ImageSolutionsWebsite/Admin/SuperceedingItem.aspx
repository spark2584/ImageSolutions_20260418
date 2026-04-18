<%@ Page Language="C#"  MasterPageFile="~/MasterPage.Master" AutoEventWireup="true" CodeBehind="SuperceedingItem.aspx.cs" Inherits="ImageSolutionsWebsite.Admin.SuperceedingItem" %>
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
                                                <h3>Superceeding Item Management</h3> 
                                            </div>
                                            <div class="table-responsive-xl">
                                                <div class="theme-form">
                                                    <div class="form-row row">

                                                        <div class="col-md-12">
                                                            <b>Filter</b><hr />
                                                        </div>
                                                        <div class="col-md-4">
                                                            <label>Item Number</label>
                                                            <asp:TextBox ID="txtItemNumber" runat="server" CssClass="form-control" AutoPostBack="true" OnTextChanged="Filter_TextChanged"></asp:TextBox>
                                                        </div>
                                                        <div class="col-md-4">
                                                            <label>Item Name</label>
                                                            <asp:TextBox ID="txtItemName" runat="server" CssClass="form-control" AutoPostBack="true" OnTextChanged="Filter_TextChanged"></asp:TextBox>
                                                        </div>
                                                        <div class="col-md-4">
                                                            <label>Sales Description</label>
                                                            <asp:TextBox ID="txtSalesDescription" runat="server" CssClass="form-control" AutoPostBack="true" OnTextChanged="Filter_TextChanged"></asp:TextBox>
                                                        </div>
                                                        <div>
                                                            <br />
                                                            <hr />
                                                        </div>

                                                        <div class="col-md-12">
                                                            <label>Item</label>
                                                            <asp:DropDownList ID="ddlItem" runat="server" DataTextField="Description" DataValueField="ItemID" CssClass="form-control"></asp:DropDownList>
                                                            <asp:RequiredFieldValidator ID="valItem" runat="server" ControlToValidate="ddlItem" ValidationGroup="Item" ErrorMessage="*" ForeColor="Red"></asp:RequiredFieldValidator>
                                                         </div>
                                                        <div class="col-md-12" style="display:none;">
                                                            <label>Inactive</label>
                                                            <asp:CheckBox ID="cbInactive" runat="server" />
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
                            </div>
                        </div>
                    </div>
                </div>
            </div>
        </div>
    </section>
    <!--  dashboard section end -->
</asp:Content>