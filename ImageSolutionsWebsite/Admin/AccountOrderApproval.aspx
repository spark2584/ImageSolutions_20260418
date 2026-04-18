<%@ Page Language="C#" MasterPageFile="~/MasterPage.Master" AutoEventWireup="true" CodeBehind="AccountOrderApproval.aspx.cs" Inherits="ImageSolutionsWebsite.Admin.AccountOrderApproval" %>
<%@ Register Src="~/Control/AdminNavigation.ascx" TagPrefix="uc" TagName="AdminNavigation" %>
<%@ Register src="~/Control/UserWebsiteSearchModal.ascx" tagname="UserWebsiteSearchModal" tagprefix="uc2"  %>

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
                                                    <i class="icofont icofont-ui-home"></i>Order Approval
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
                                                                    <label>Approver</label>
                                                                </div>
                                                                <div class="col-md-9">
                                                                    <asp:DropDownList ID="ddlUserWebsite" runat="server" DataTextField="Description" DataValueField="UserWebsiteID" CssClass="form-control" Visible="false"></asp:DropDownList>
                                                                    <%--<asp:RequiredFieldValidator ID="valUser" runat="server" ControlToValidate="ddlUserWebsite" ValidationGroup="Submit" ErrorMessage="*" ForeColor="Red"></asp:RequiredFieldValidator>--%>
                                                                    <asp:TextBox ID="txtUserWebsite" runat="server" CssClass="form-control" Enabled="false"></asp:TextBox>
                                                                    <asp:HiddenField ID="hfUserWebsiteID" runat="server" />                                                                    
                                                                </div>
                                                                <div class="col-md-3">
                                                                    <asp:LinkButton ID="btnUserWebsiteSearch" runat="server" OnClick="btnUserWebsiteSearch_Click"><i class="ti-search" style="font-size:large"></i></asp:LinkButton>
                                                                    <asp:LinkButton ID="btnUserWebsiteRemove" runat="server" OnClick="btnUserWebsiteRemove_Click"><i class="ti-trash" style="font-size:large"></i></asp:LinkButton>
                                                                </div>
                                                                <div class="col-md-12">
                                                                    <label>Amount</label>
                                                                </div>
                                                                <div class="col-md-12">
                                                                    <asp:TextBox ID="txtAmount" runat="server" CssClass="form-control" placeholder="amount" required=""></asp:TextBox>
                                                                    <asp:RequiredFieldValidator ID="valPrice" runat="server" ControlToValidate="txtAmount" ValidationGroup="Submit" ErrorMessage="*" ForeColor="Red"></asp:RequiredFieldValidator>
                                                                </div>
                                                                <div class="col-md-12">
                                                                    <asp:Button ID="btnSave" runat="server" Text="Save" OnClick="btnSave_Click" CssClass="btn btn-sm btn-solid" ValidationGroup="Submit" />
                                                                    <a id="aCancel" runat="server" href="/Admin/Account.aspx?id=<%= mAccountID %>" class="btn btn-sm btn-solid">Cancel</a>
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

<asp:Content ID="cFooter" ContentPlaceHolderID="cphFooter" runat="server">
    <uc2:UserWebsiteSearchModal ID="ucUserWebsiteSearchModal" runat="server" />
</asp:Content>