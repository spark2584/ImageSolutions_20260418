<%@ Page Language="C#" MasterPageFile="~/MasterPage.Master" AutoEventWireup="true" CodeBehind="ItemPricing.aspx.cs" Inherits="ImageSolutionsWebsite.Admin.ItemPricing" %>
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
                                                    <i class="icofont icofont-ui-home"></i>Pricing
                                                </a>
                                                <div class="material-border"></div>
                                            </li>
                                        </ul>
                                    </section><br />
                                    <div class="tab-content nav-material">
                                        <div class="tab-pane fade show active" id="top_1" runat="server" role="tabpanel" aria-labelledby="top_1_tab">
                                            <div class="card dashboard-table mt-0">
                                                <div class="card-body table-responsive-sm">
                                                    <div class="table-responsive-xl">
                                                        <div class="theme-form">
                                                            <div class="form-row row">
                                                                <div class="col-md-12">
                                                                    <label>Group</label>
                                                                    <asp:DropDownList ID="ddlGroup" runat="server" DataTextField="GroupName" DataValueField="WebsiteGroupID" CssClass="form-control"></asp:DropDownList>
                                                                    <asp:RequiredFieldValidator ID="valName" runat="server" ControlToValidate="ddlGroup" ValidationGroup="Submit" ErrorMessage="*" ForeColor="Red"></asp:RequiredFieldValidator>
                                                                </div>
                                                                <div class="col-md-12">
                                                                    <label>Price</label>
                                                                    <asp:TextBox ID="txtPrice" runat="server" CssClass="form-control" placeholder="price" required=""></asp:TextBox>
                                                                    <asp:RequiredFieldValidator ID="valPrice" runat="server" ControlToValidate="txtPrice" ValidationGroup="Submit" ErrorMessage="*" ForeColor="Red"></asp:RequiredFieldValidator>
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
            </div>
        </div>
    </section>
    <!--  dashboard section end -->
</asp:Content>
