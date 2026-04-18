<%@ Page Language="C#" MasterPageFile="~/MasterPage.Master" AutoEventWireup="true" CodeBehind="UserWebsiteTab.aspx.cs" Inherits="ImageSolutionsWebsite.Admin.UserWebsiteTab" %>
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
                        <div class="tab-pane fade show active" id="orders">
                            <div class="row">
                                <div class="col-12">
                                    <div class="card dashboard-table mt-0">
                                        <div class="card-body table-responsive-sm">
                                            <div class="top-sec">
                                                <h3>User Tab Management</h3> 
                                            </div>
                                            <div class="table-responsive-xl">
                                                <div class="theme-form">
                                                    <div class="form-row row">

<%--                                                        <div class="col-md-12">
                                                            <label>User</label>
                                                            <asp:DropDownList ID="ddlUserWebsite" runat="server" DataTextField="Description" DataValueField="UserWebsiteID" CssClass="form-control"></asp:DropDownList>
                                                            <asp:RequiredFieldValidator ID="valUser" runat="server" ControlToValidate="ddlUserWebsite" ValidationGroup="User" ErrorMessage="*" ForeColor="Red"></asp:RequiredFieldValidator>
                                                        </div>--%>


                                                        <div class="col-md-12">
                                                            <label>User</label>
                                                        </div>
                                                        <div class="col-md-9">
                                                            <asp:DropDownList ID="ddlUserWebsite" runat="server" DataTextField="Description" DataValueField="UserWebsiteID" CssClass="form-control" Visible="false"></asp:DropDownList>
                                                            <asp:TextBox ID="txtUserWebsite" runat="server" CssClass="form-control" Enabled="false"></asp:TextBox>
                                                            <asp:HiddenField ID="hfUserWebsiteID" runat="server" />
                                                        </div>
                                                        <div class="col-md-3">
                                                            <asp:LinkButton ID="btnUserWebsiteSearch" runat="server" OnClick="btnUserWebsiteSearch_Click" Enabled="false"><i class="ti-search" style="font-size:large"></i></asp:LinkButton>
                                                            <asp:LinkButton ID="btnUserWebsiteRemove" runat="server" OnClick="btnUserWebsiteRemove_Click" Enabled="false"><i class="ti-trash" style="font-size:large"></i></asp:LinkButton>
                                                        </div>



                                                        <div class="col-md-12">
                                                            <br />
                                                            <asp:Button ID="btnSave" runat="server" Text="Save" OnClick="btnSave_Click" CssClass="btn btn-sm btn-solid" ValidationGroup="User" />
                                                            <a id="aCancel" runat="server" href="/Admin/GroupOverview.aspx" class="btn btn-sm btn-solid">Cancel</a>
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


<asp:Content ID="cFooter" ContentPlaceHolderID="cphFooter" runat="server">
    <uc2:UserWebsiteSearchModal ID="ucUserWebsiteSearchModal" runat="server" />
</asp:Content>