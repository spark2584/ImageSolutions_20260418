<%@ Page Language="C#" MasterPageFile="~/MasterPage.Master" AutoEventWireup="true" CodeBehind="CreateTab.aspx.cs" Inherits="ImageSolutionsWebsite.Admin.CreateTab" %>
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
                                                <h3>Tab Management</h3> 
                                            </div>
                                            <div class="table-responsive-xl">
                                                <div class="theme-form">
                                                    <div class="form-row row">
                                                        <div class="col-md-12">
                                                            <label>Tab Name</label>
                                                            <asp:TextBox ID="txtName" runat="server" CssClass="form-control" placeholder="Tab Name" required=""></asp:TextBox>
                                                            <asp:RequiredFieldValidator ID="RequiredFieldValidator1" runat="server" ControlToValidate="txtName" ValidationGroup="Submit" ErrorMessage="*" ForeColor="Red"></asp:RequiredFieldValidator>
                                                        </div>
                                                        <div class="col-md-12">
                                                            <label>Tab Image</label>&nbsp;
                                                            <asp:FileUpload ID="fuTabImage" runat="server" />
                                                            <asp:Button ID="btnUpload" runat="server" Text="Upload" OnClick="btnUpload_Click" />
                                                            <br />
                                                            <asp:Image ID="imgTab" runat="server" />
                                                            <asp:Button ID="btnRemoveImage" runat="server" Text="Remove" OnClick="btnRemoveImage_Click" />
                                                        </div>
                                                        <div class="col-md-12">
                                                            <label>Allow All Groups</label>&nbsp;
                                                            <asp:CheckBox ID="cbAllowAllGroups" runat="server" />
                                                        </div>
                                                        <div class="col-md-12">
                                                            <label>Inactive</label>&nbsp;
                                                            <asp:CheckBox ID="cbInactive" runat="server" />
                                                        </div>
                                                        <div class="col-md-12">
                                                            <asp:Button ID="btnCreate" runat="server" Text="Save" OnClick="btnCreate_Click" CssClass="btn btn-sm btn-solid" ValidationGroup="Submit" />
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