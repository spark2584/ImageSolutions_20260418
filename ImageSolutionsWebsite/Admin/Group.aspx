<%@ Page Language="C#" MasterPageFile="~/MasterPage.Master" AutoEventWireup="true" CodeBehind="Group.aspx.cs" Inherits="ImageSolutionsWebsite.Admin.Group" %>
<%@ Register Src="~/Control/AdminNavigation.ascx" TagPrefix="uc" TagName="AdminNavigation" %>

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
                        <div class="tab-pane fade show active" id="websites">
                            <div class="row">
                                <div class="col-sm-12 col-lg-12">
                                    <section class="tab-product m-0">
                                        <ul class="nav nav-tabs nav-material" id="top-tab" role="tablist">
                                            <li class="nav-item">
                                                <a class="nav-link active" id="top_1_tab" runat="server" data-bs-toggle="tab"
                                                    href="#cphBody_top_1" role="tab" aria-selected="true">
                                                    <i class="icofont icofont-ui-home"></i>Group Management
                                                </a>
                                                <div class="material-border"></div>
                                            </li>
                                            <li class="nav-item">
                                                <a class="nav-link" id="top_2_tab" runat="server" data-bs-toggle="tab"
                                                    href="#cphBody_top_2" role="tab" aria-selected="false">
                                                    <i class="icofont icofont-man-in-glasses"></i>Assigned Users
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
                                                        <h3>User Management</h3>
                                                    </div>--%>
                                                    <div class="table-responsive-xl">
                                                        <div class="theme-form">
                                                            <div class="form-row row">
                                                                <div class="col-md-12">
                                                                    <label>Group Name</label>
                                                                    <asp:TextBox ID="txtName" runat="server" CssClass="form-control" placeholder="group name" required=""></asp:TextBox>
                                                                    <asp:RequiredFieldValidator ID="RequiredFieldValidator1" runat="server" ControlToValidate="txtName" ValidationGroup="Submit" ErrorMessage="*" ForeColor="Red"></asp:RequiredFieldValidator>
                                                                </div>

                                                                <div class="col-md-12">
                                                                    <br />
                                                                    <label>Logo Upload</label>
                                                                    <asp:FileUpload ID="fuLogoImage" runat="server" />
                                                                    <asp:Button ID="btnUpload" runat="server" Text="Upload" OnClick="btnUpload_Click" CssClass="btn btn-sm btn-solid" />
                                                                    <asp:Button ID="btnResetImage" runat="server" Text="Reset" OnClick="btnResetImage_Click" CssClass="btn btn-sm btn-solid" />
                                                                    <asp:Button ID="btnRemoveImage" runat="server" Text="Remove" OnClick="btnRemoveImage_Click" CssClass="btn btn-sm btn-solid"/>
                                                                    <br />
                                                                    <asp:Image ID="imgLogo" runat="server" Width="200px" /><br /><br />
                                                                </div>

                                                                <div class="col-md-12">
                                                                    <br />
                                                                    <label>Home Page Image Upload</label>
                                                                    <asp:FileUpload ID="fuHomePageImageImage" runat="server" />
                                                                    <asp:Button ID="btnHomePageImageUpload" runat="server" Text="Upload" OnClick="btnHomePageImageUpload_Click" CssClass="btn btn-sm btn-solid" />
                                                                    <asp:Button ID="btnHomePageImageResetImage" runat="server" Text="Reset" OnClick="btnHomePageImageResetImage_Click" CssClass="btn btn-sm btn-solid" />
                                                                    <asp:Button ID="btnHomePageImageRemoveImage" runat="server" Text="Remove" OnClick="btnHomePageImageRemoveImage_Click" CssClass="btn btn-sm btn-solid"/>
                                                                    <br />
                                                                    <asp:Image ID="imgHomePageImage" runat="server" Width="200px" /><br /><br />
                                                                </div>

                                                                <div class="col-md-12">
                                                                    <br />
                                                                    <label>Home Page Mobile Image Upload</label>
                                                                    <asp:FileUpload ID="fuHomePageMobileImageImage" runat="server" />
                                                                    <asp:Button ID="btnHomePageMobileImageUpload" runat="server" Text="Upload" OnClick="btnHomePageMobileImageUpload_Click" CssClass="btn btn-sm btn-solid" />
                                                                    <asp:Button ID="btnHomePageMobileImageResetImage" runat="server" Text="Reset" OnClick="btnHomePageMobileImageResetImage_Click" CssClass="btn btn-sm btn-solid" />
                                                                    <asp:Button ID="btnHomePageMobileImageRemoveImage" runat="server" Text="Remove" OnClick="btnHomePageMobileImageRemoveImage_Click" CssClass="btn btn-sm btn-solid"/>
                                                                    <br />
                                                                    <asp:Image ID="imgHomePageMobileImage" runat="server" Width="200px" /><br /><br />
                                                                </div>

                                                                <div class="col-md-12">
                                                                    <asp:Button ID="btnSave" runat="server" Text="Save" OnClick="btnSave_Click" CssClass="btn btn-sm btn-solid" ValidationGroup="Submit" />
                                                                    <a id="aCancel" runat="server" href="/Admin/GroupOverview.aspx" class="btn btn-sm btn-solid">Cancel</a>
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
                                                                <a href="/Admin/useraccount.aspx?websitegroupid=<%=WebsiteGroup.WebsiteGroupID %>" class="btn btn-sm btn-solid">+ add new</a>
                                                            </div>
                                                            <div class="table-responsive-xl">                                                                <asp:GridView runat="server" ID="gvUserAccount" AutoGenerateColumns="False" CssClass="table cart-table order-table" HeaderStyle-CssClass="table-head" GridLines="None" Width="100%" AllowSorting="true" CellSpacing="0" CellPadding="0" >
                                                                    <Columns>
                                                                        <asp:BoundField DataField="UserWebsite.UserInfo.FullName" HeaderText="Full Name" HeaderStyle-HorizontalAlign="Left" ItemStyle-HorizontalAlign="Left" />
                                                                        <asp:BoundField DataField="Account.AccountName" HeaderText="Store" HeaderStyle-HorizontalAlign="Left" ItemStyle-HorizontalAlign="Left" />
                                                                        <asp:BoundField DataField="WebsiteGroup.GroupName" HeaderText="Website Group" HeaderStyle-HorizontalAlign="Center" ItemStyle-HorizontalAlign="Center" />
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
                                                                                <a href='/admin/useraccount.aspx?id=<%# Eval("UserAccountID") %>&websitegroupid=<%# Eval("WebsiteGroupID") %>'>
                                                                                    <i class="fa fa-eye text-theme"></i>
                                                                                </a>
                                                                            </ItemTemplate>
                                                                        </asp:TemplateField>
                                                                    </Columns>
                                                                </asp:GridView>
                                                                <uc1:Pager runat="server" ID="ucPager" PagingMode="Redirect" PageSize="20" PagingRecordText="Users" />
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