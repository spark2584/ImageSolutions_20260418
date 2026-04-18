<%@ Page Language="C#" MasterPageFile="~/MasterPage.Master" AutoEventWireup="true" CodeBehind="ItemPersonalizationApproved.aspx.cs" Inherits="ImageSolutionsWebsite.Admin.ItemPersonalizationApproved" %>
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
                                                <h3>Approved Item Personalization</h3> 
                                            </div>
                                            <div class="table-responsive-xl">
                                                <div class="theme-form">
                                                    <div class="form-row row">
                                                        <div class="col-md-12">
                                                            <label>Name</label>
                                                            <asp:TextBox ID="txtName" runat="server" CssClass="form-control" placeholder="name"></asp:TextBox>
                                                            <asp:RequiredFieldValidator ID="valName" runat="server" ControlToValidate="txtName" ValidationGroup="Save" ErrorMessage="*" ForeColor="Red"></asp:RequiredFieldValidator>
                                                         </div>
                                                        <div class="col-md-12">
                                                            <label>Value</label>
                                                            <asp:TextBox ID="txtValue" runat="server" CssClass="form-control" placeholder="value"></asp:TextBox>
                                                            <asp:RequiredFieldValidator ID="valValue" runat="server" ControlToValidate="txtValue" ValidationGroup="Save" ErrorMessage="*" ForeColor="Red"></asp:RequiredFieldValidator>
                                                        </div>
                                                        <div class="col-md-12">
                                                            <asp:Button ID="btnSave" runat="server" Text="Save" OnClick="btnSave_Click" CssClass="btn btn-sm btn-solid" ValidationGroup="Save" />
                                                            <asp:Button ID="btnCancel" runat="server" Text="Cancel" OnClick="btnCancel_Click" CssClass="btn btn-sm btn-solid" CausesValidation="false"/>
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