<%@ Page Language="C#" AutoEventWireup="true" MasterPageFile="~/MasterPage.Master" CodeBehind="Tab.aspx.cs" Inherits="ImageSolutionsWebsite.Admin.Tab" %>
<%@ Register Src="~/Control/AdminNavigation.ascx" TagPrefix="uc" TagName="AdminNavigation" %>

<asp:Content ID="cHead" ContentPlaceHolderID="head" runat="server">
    <script type="text/javascript">    
       function ConfirmOnDelete() {
        if (confirm("Are you sure you want to delete this tab?") == true)
            return true;
        else
            return false;
       }
    </script>
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
                                                    <i class="icofont icofont-ui-home"></i>Tab Information
                                                </a>
                                                <div class="material-border"></div>
                                            </li>
                                            <li class="nav-item">
                                                <a class="nav-link" id="top_2_tab" runat="server" data-bs-toggle="tab"
                                                    href="#cphBody_top_2" role="tab" aria-selected="false">
                                                    <i class="icofont icofont-man-in-glasses"></i>Sub Tabs
                                                </a>
                                                <div class="material-border"></div>
                                            </li>
                                            <li class="nav-item">
                                                <a class="nav-link" id="top_3_tab" runat="server" data-bs-toggle="tab"
                                                    href="#cphBody_top_3" role="tab" aria-selected="false">
                                                    <i class="icofont icofont-contacts"></i>Items
                                                </a>
                                                <div class="material-border"></div>
                                            </li>
                                            <li class="nav-item">
                                                <a class="nav-link" id="top_4_tab" runat="server" data-bs-toggle="tab"
                                                    href="#cphBody_top_4" role="tab" aria-selected="false">
                                                    <i class="icofont icofont-contacts"></i>Group Permission
                                                </a>
                                                <div class="material-border"></div>
                                            </li>
                                            <li class="nav-item">
                                                <a class="nav-link" id="top_5_tab" runat="server" data-bs-toggle="tab"
                                                    href="#cphBody_top_5" role="tab" aria-selected="false">
                                                    <i class="icofont icofont-contacts"></i>User Permission
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
                                                                    <label>Tab Name</label>
                                                                    <asp:TextBox ID="txtName" runat="server" CssClass="form-control" placeholder="tab name" required=""></asp:TextBox>
                                                                    <asp:RequiredFieldValidator ID="valName" runat="server" ControlToValidate="txtName" ValidationGroup="Submit" ErrorMessage="*" ForeColor="Red"></asp:RequiredFieldValidator>
                                                                </div>
                                                                <div class="col-md-12">
                                                                    <label>Display Name</label>
                                                                    <asp:TextBox ID="txtDisplayName" runat="server" CssClass="form-control" placeholder="tab name"></asp:TextBox>
                                                                </div>
                                                                <div class="col-md-12">
                                                                    <label>Sort</label>
                                                                    <asp:TextBox ID="txtTabSort" runat="server" CssClass="form-control" placeholder="tab sort" onKeyPress="if (event.keyCode!=46 && event.keyCode!=45 && (event.keyCode<48 || event.keyCode>57)) event.returnValue=false"></asp:TextBox>
                                                                </div>
                                                                <div class="col-md-12">
                                                                    <label>Parent Tab</label>
                                                                    <asp:DropDownList ID="ddlParentTab" runat="server" DataTextField="TabName" DataValueField="WebsiteTabID" CssClass="form-control"></asp:DropDownList>
                                                                </div>
                                                                <div class="col-md-12">
                                                                    <br />
                                                                    <label>Image Upload From Item</label>
                                                                    <asp:DropDownList ID="ddlTabItem" runat="server" DataTextField="ItemDescription" DataValueField="WebsiteTabItemID" CssClass="form-control" AutoPostBack="true" OnSelectedIndexChanged="ddlTabItem_SelectedIndexChanged"></asp:DropDownList>
                                                                    <br />
                                                                    <label>Image Upload From File</label>
                                                                    <br />
                                                                    <asp:FileUpload ID="fuTabImage" runat="server" />
                                                                    <asp:Button ID="btnUpload" runat="server" Text="Upload" OnClick="btnUpload_Click" CssClass="btn btn-sm btn-solid" />
                                                                    <asp:Button ID="btnResetImage" runat="server" Text="Reset" OnClick="btnResetImage_Click" CssClass="btn btn-sm btn-solid" />
                                                                    <asp:Button ID="btnRemoveImage" runat="server" Text="Remove" OnClick="btnRemoveImage_Click" CssClass="btn btn-sm btn-solid"/>
                                                                    <br />
                                                                    <asp:Image ID="imgTab" runat="server" Width="200px" /><br /><br />
                                                                </div>
                                                                
                                                                <%--<asp:Panel ID="ItemImageSelect" runat="server">
                                                                    <div class="col-md-12">
                                                                        <label>Image</label>
                                                                        <asp:DropDownList ID="DropDownList1" runat="server" DataTextField="TabName" DataValueField="WebsiteTabID" CssClass="form-control"></asp:DropDownList>
                                                                    </div>
                                                                    <br />
                                                                </asp:Panel>--%>

                                                                <div class="col-md-12">
                                                                    <label>Display Employee Permission</label>&nbsp;
                                                                    <asp:DropDownList ID="ddlDisplayUserPermission" runat="server" CssClass="form-control">
                                                                        <asp:ListItem Selected="True" Value=""></asp:ListItem>
                                                                        <asp:ListItem Value="All">All</asp:ListItem>
                                                                        <asp:ListItem Value="All With Account Change">All With Account Change</asp:ListItem>
                                                                        <asp:ListItem Value="Store Only">Store Only</asp:ListItem>
                                                                    </asp:DropDownList>                 
                                                                    <br />
                                                                </div>

                                                                <div class="col-md-12">
                                                                    <label>Message</label>
                                                                    <asp:TextBox ID="txtMessage" runat="server" CssClass="form-control" placeholder="message" TextMode="MultiLine"></asp:TextBox>
                                                                </div>

                                                                <div class="col-md-12">
                                                                    <label>Allow All Groups</label>&nbsp;
                                                                    <asp:CheckBox ID="cbAllowAllGroups" runat="server" />
                                                                </div>



                                                                <div class="col-md-12" style="display:none;">
                                                                    <label>Inactive</label>&nbsp;
                                                                    <asp:CheckBox ID="cbInactive" runat="server" />
                                                                </div>
                                                                <div class="col-md-12">
                                                                    <asp:Button ID="btnSave" runat="server" Text="Save" OnClick="btnSave_Click" CssClass="btn btn-sm btn-solid" ValidationGroup="Submit" />
                                                                    <a id="aCancel" runat="server" href="/Admin/TabOverview.aspx" class="btn btn-sm btn-solid">Cancel</a>
                                                                    <asp:Button ID="btnDelete" runat="server" Text="Delete" OnClick="btnDelete_Click" CssClass="btn btn-sm btn-solid" CausesValidation="false" OnClientClick="return ConfirmOnDelete();"/>
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
                                                                <a href="/Admin/tab.aspx?parentid=<%= mWebsiteTabID %>" class="btn btn-sm btn-solid">+ add new</a>
                                                            </div>
                                                            <div class="table-responsive-xl">
                                                                <asp:GridView runat="server" ID="gvWebsiteTabs" AutoGenerateColumns="False" CssClass="table cart-table order-table" HeaderStyle-CssClass="table-head" GridLines="None" Width="100%" AllowSorting="true" CellSpacing="0" CellPadding="0">
                                                                    <Columns>
                                                                        <asp:BoundField DataField="TabName" HeaderText="Tab" HeaderStyle-HorizontalAlign="Left" ItemStyle-HorizontalAlign="Left" />
                                                                        <asp:CheckBoxField DataField="AllowAllGroups" HeaderText="Allow All Groups" HeaderStyle-HorizontalAlign="Center" ItemStyle-HorizontalAlign="Center" />
                                                                        <asp:CheckBoxField DataField="Inactive" HeaderText="Inactive" HeaderStyle-HorizontalAlign="Center" ItemStyle-HorizontalAlign="Center" />
                                                                        <asp:TemplateField HeaderText="VIEW">
                                                                            <ItemTemplate>
                                                                                <a href='/admin/tab.aspx?id=<%# Eval("WebsiteTabID") %>'>
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
                                        <div class="tab-pane fade" id="top_3" runat="server" role="tabpanel" aria-labelledby="top_3_tab">
                                                <div class="row">
                                                <div class="col-12">
                                                    <div class="card dashboard-table mt-0">
                                                        <div class="card-body table-responsive-sm">
                                                            <div class="top-sec">
                                                                <h3></h3>
                                                                <a href="/Admin/tabitem.aspx?websitetabid=<%= mWebsiteTabID %>" class="btn btn-sm btn-solid">+ add new</a>
                                                            </div>
                                                                <asp:UpdatePanel ID="UpdatePanel1" runat="server">
                                                                    <ContentTemplate>
                                                                    <div class="table-responsive-xl">
                                                                        <asp:GridView ID="gvWebsiteTabItem" runat="server" AutoGenerateColumns="False" CssClass="table cart-table order-table" HeaderStyle-CssClass="table-head" GridLines="None" Width="100%" AllowSorting="true" CellSpacing="0" 
                                                                            DataKeyNames="WebsiteTabItemID" CellPadding="0" OnRowCommand="gvWebsiteTabItem_RowCommand" OnRowDataBound="gvWebsiteTabItem_RowDataBound">
                                                                            <Columns>
                                                                                <asp:BoundField DataField="Item.ItemName" HeaderText="Item Name" HeaderStyle-HorizontalAlign="Left" ItemStyle-HorizontalAlign="Left" />
                                                                                <asp:BoundField DataField="Item.ItemNumber" HeaderText="Item Number" HeaderStyle-HorizontalAlign="Left" ItemStyle-HorizontalAlign="Left" />
                                                                                <asp:BoundField DataField="Item.ParentItem.ItemNumber" HeaderText="Parent" HeaderStyle-HorizontalAlign="Left" ItemStyle-HorizontalAlign="Left" />
                                                                                <asp:TemplateField HeaderText="VIEW">
                                                                                    <ItemTemplate>
                                                                                        <a href='/admin/tabitem.aspx?id=<%# Eval("WebsiteTabItemID") %>&websitetabid=<%# Eval("WebsiteTabID") %>'>
                                                                                            <i class="fa fa-eye text-theme"></i>
                                                                                        </a>
                                                                                    </ItemTemplate>
                                                                                </asp:TemplateField>
                                                                                <asp:TemplateField>
                                                                                    <ItemTemplate>
                                                                                        <asp:HiddenField ID="hfWebsiteTabItemID" runat="server" Value='<%# Eval("WebsiteTabItemID") %>' />
                                                                                        <asp:Button ID="btnUp" runat="server" Text="Up" CommandArgument='<%# Eval("WebsiteTabItemID") %>' CommandName="MoveUp" />
                                                                                        <asp:Button ID="btnDown" runat="server" Text="Down" CommandArgument='<%# Eval("WebsiteTabItemID") %>' CommandName="MoveDown"/>
                                                                                    </ItemTemplate>
                                                                                </asp:TemplateField>
                                                                                <asp:TemplateField HeaderText="Sort #" HeaderStyle-Width="60px" ItemStyle-Width="60px">
                                                                                    <ItemTemplate>
                                                                                        <asp:TextBox ID="txtSort" runat="server" CssClass="form-control" Width="60px"></asp:TextBox>
                                                                                    </ItemTemplate>
                                                                                </asp:TemplateField>
                                                                            </Columns>
                                                                        </asp:GridView>
                                                                    </div>
                                                                    <div class="top-sec">
                                                                        <h3></h3>
                                                                        <asp:LinkButton ID="btnUpdateSort" runat="server" OnClick="btnUpdateSort_Click" class="btn btn-sm btn-solid" Text="Update Sort"></asp:LinkButton>
                                                                    </div>
                                                                </ContentTemplate>
                                                            </asp:UpdatePanel>
                                                        </div>
                                                    </div>
                                                </div>
                                            </div>
                                        </div>
                                        <div class="tab-pane fade" id="top_4" runat="server" role="tabpanel" aria-labelledby="top_4_tab">
                                            <div class="row">
                                                <div class="col-12">
                                                    <div class="card dashboard-table mt-0">
                                                        <div class="card-body table-responsive-sm">
                                                            <div class="top-sec">
                                                                <h3></h3>
                                                                <a href="/Admin/GroupTab.aspx?websitetabid=<%= mWebsiteTabID %>" class="btn btn-sm btn-solid">+ add new</a>
                                                            </div>
                                                            <div class="table-responsive-xl">
                                                                <asp:GridView ID="gvWebsiteGroupTab" runat="server" AutoGenerateColumns="False" CssClass="table cart-table order-table" HeaderStyle-CssClass="table-head" GridLines="None" Width="100%" AllowSorting="true" CellSpacing="0" 
                                                                    CellPadding="0" OnRowCommand="gvWebsiteGroupTab_RowCommand">
                                                                    <Columns>
                                                                        <asp:BoundField DataField="WebsiteGroup.GroupName" HeaderText="Group" ItemStyle-HorizontalAlign="Center"/>
                                                                        <%--<asp:TemplateField HeaderText="Created By" HeaderStyle-HorizontalAlign="Center" ItemStyle-HorizontalAlign="Center">
                                                                            <ItemTemplate>
                                                                                <%# Eval("CreatedByUser.FullName") %>
                                                                            </ItemTemplate>
                                                                        </asp:TemplateField>--%>
                                                                        <asp:TemplateField HeaderText="Created Date" HeaderStyle-HorizontalAlign="Center" ItemStyle-HorizontalAlign="Center">
                                                                            <ItemTemplate>
                                                                                <%# Eval("CreatedOn") %>
                                                                            </ItemTemplate>
                                                                        </asp:TemplateField>
                                                                        <asp:TemplateField HeaderText="VIEW">
                                                                            <ItemTemplate>
                                                                                <a href='/admin/GroupTab.aspx?id=<%# Eval("WebsiteGroupTabID") %>&websitetabid=<%# Eval("WebsiteTabID") %>'>
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

                                        <div class="tab-pane fade" id="top_5" runat="server" role="tabpanel" aria-labelledby="top_5_tab">
                                            <div class="row">
                                                <div class="col-12">
                                                    <div class="card dashboard-table mt-0">
                                                        <div class="card-body table-responsive-sm">
                                                            <div class="top-sec">
                                                                <h3></h3>
                                                                <a href="/Admin/UserWebsiteTab.aspx?websitetabid=<%= mWebsiteTabID %>" class="btn btn-sm btn-solid">+ add new</a>
                                                            </div>
                                                            <div class="table-responsive-xl">
                                                                <asp:GridView ID="gvUserWebsiteTab" runat="server" AutoGenerateColumns="False" CssClass="table cart-table order-table" HeaderStyle-CssClass="table-head" GridLines="None" Width="100%" AllowSorting="true" CellSpacing="0" 
                                                                    CellPadding="0" OnRowCommand="gvUserWebsiteTab_RowCommand">
                                                                    <Columns>
                                                                        <asp:BoundField DataField="UserWebsite.Description" HeaderText="User" ItemStyle-HorizontalAlign="Center"/>
                                                                        <asp:TemplateField HeaderText="Created Date" HeaderStyle-HorizontalAlign="Center" ItemStyle-HorizontalAlign="Center">
                                                                            <ItemTemplate>
                                                                                <%# Eval("CreatedOn") %>
                                                                            </ItemTemplate>
                                                                        </asp:TemplateField>
                                                                        <asp:TemplateField HeaderText="VIEW">
                                                                            <ItemTemplate>
                                                                                <a href='/admin/UserWebsiteTab.aspx?id=<%# Eval("UserWebsiteTabID") %>&websitetabid=<%# Eval("WebsiteTabID") %>'>
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
