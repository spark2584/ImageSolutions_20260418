<%@ Page Language="C#" MasterPageFile="~/MasterPage.Master" AutoEventWireup="true" CodeBehind="AdminUserWebsiteImport.aspx.cs" Inherits="ImageSolutionsWebsite.Admin.AdminUserWebsiteImport" %>
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
                                                <h3>User Import</h3>       
                                                <asp:LinkButton ID="btnDownloadTemplate" runat="server" class="btn btn-sm btn-solid" OnClick="btnDownloadTemplate_Click">Download Template</asp:LinkButton>
                                            </div>
                                            <div class="table-responsive-xl">
                                                <div class="col-md-12">
                                                    <h3>Import File</h3>
                                                    <label>File: </label>
                                                    <asp:FileUpload ID="fuUser" runat="server" />
                                                    <asp:Button ID="btnUpload" runat="server" Text="Upload" CssClass="btn btn-sm btn-solid" OnClick="btnUpload_Click"/>
                                                </div>
                                                <div>
                                                    <asp:GridView runat="server" ID="gvUserUpload" AutoGenerateColumns="False" CssClass="table cart-table order-table" HeaderStyle-CssClass="table-head" GridLines="None" Width="100%" AllowSorting="true" CellSpacing="0" CellPadding="0">
                                                        <Columns>
                                                            <asp:BoundField DataField="FirstName" HeaderText="First Name" HeaderStyle-HorizontalAlign="Left" ItemStyle-HorizontalAlign="Left" />
                                                            <asp:BoundField DataField="LastName" HeaderText="Last Name" HeaderStyle-HorizontalAlign="Left" ItemStyle-HorizontalAlign="Left" />
                                                            <asp:BoundField DataField="Email" HeaderText="Email" HeaderStyle-HorizontalAlign="Left" ItemStyle-HorizontalAlign="Left" />
                                                            <asp:BoundField DataField="Password" HeaderText="Password" HeaderStyle-HorizontalAlign="Left" ItemStyle-HorizontalAlign="Left" />
                                                            <asp:BoundField DataField="EmployeeID" HeaderText="Employee ID" HeaderStyle-HorizontalAlign="Left" ItemStyle-HorizontalAlign="Left" />
                                                            <asp:BoundField DataField="Account" HeaderText="Account" HeaderStyle-HorizontalAlign="Left" ItemStyle-HorizontalAlign="Left" />
                                                            <asp:BoundField DataField="Group" HeaderText="Group" HeaderStyle-HorizontalAlign="Left" ItemStyle-HorizontalAlign="Left" />
                                                            <asp:BoundField DataField="Username" HeaderText="Username" HeaderStyle-HorizontalAlign="Left" ItemStyle-HorizontalAlign="Left" />
                                                        </Columns>
                                                    </asp:GridView>
                                                    <asp:Button ID="btnSubmit" runat="server" Text="Submit" CssClass="btn btn-sm btn-solid" OnClick="btnSubmit_Click" Visible="false"/>
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

    <asp:HiddenField runat="server" ID="hfFilePath" />
</asp:Content>
