<%@ Page Language="C#" MasterPageFile="~/MasterPage.Master" AutoEventWireup="true" CodeBehind="Item.aspx.cs" Inherits="ImageSolutionsWebsite.Admin.Item" %>
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
                                                    href="#cphBody_top_1" role="tab" aria-selected="true">
                                                    <i class="icofont icofont-ui-home"></i>Item Info
                                                </a>
                                                <div class="material-border"></div>
                                            </li>
                                            <li class="nav-item" id="navAttribute" runat="server">
                                                <a class="nav-link" id="top_2_tab" runat="server" data-bs-toggle="tab"
                                                    href="#cphBody_top_2" role="tab" aria-selected="false">
                                                    <i class="icofont icofont-man-in-glasses"></i>Attributes
                                                </a>
                                                <div class="material-border"></div>
                                            </li>
                                            <li class="nav-item" id="navVariations" runat="server">
                                                <a class="nav-link" id="top_3_tab" runat="server" data-bs-toggle="tab"
                                                    href="#cphBody_top_3" role="tab" aria-selected="false">
                                                    <i class="icofont icofont-contacts"></i>Variations
                                                </a>
                                                <div class="material-border"></div>
                                            </li>
                                            <li class="nav-item" id="navItemPricing" runat="server">
                                                <a class="nav-link" id="top_4_tab" runat="server" data-bs-toggle="tab"
                                                    href="#cphBody_top_4" role="tab" aria-selected="false">
                                                    <i class="icofont icofont-contacts"></i>Pricing
                                                </a>
                                                <div class="material-border"></div>
                                            </li>
                                            <li class="nav-item" id="navItemDetail" runat="server">
                                                <a class="nav-link" id="top_5_tab" runat="server" data-bs-toggle="tab"
                                                    href="#cphBody_top_5" role="tab" aria-selected="false">
                                                    <i class="icofont icofont-contacts"></i>Product Detail
                                                </a>
                                                <div class="material-border"></div>
                                            </li>
                                            <li class="nav-item" id="navSuperceedingItems" runat="server">
                                                <a class="nav-link" id="top_6_tab" runat="server" data-bs-toggle="tab"
                                                    href="#cphBody_top_6" role="tab" aria-selected="false">
                                                    <i class="icofont icofont-contacts"></i>Superceding Items
                                                </a>
                                                <div class="material-border"></div>
                                            </li>
                                            <li class="nav-item" id="navPersonalizations" runat="server">
                                                <a class="nav-link" id="top_7_tab" runat="server" data-bs-toggle="tab"
                                                    href="#cphBody_top_7" role="tab" aria-selected="false">
                                                    <i class="icofont icofont-contacts"></i>Customizations
                                                </a>
                                                <div class="material-border"></div>
                                            </li>
                                            <li class="nav-item" id="navTabs" runat="server">
                                                <a class="nav-link" id="top_8_tab" runat="server" data-bs-toggle="tab"
                                                    href="#cphBody_top_8" role="tab" aria-selected="false">
                                                    <i class="icofont icofont-contacts"></i>Tabs
                                                </a>
                                                <div class="material-border"></div>
                                            </li>
                                            <li class="nav-item" id="navGroups" runat="server">
                                                <a class="nav-link" id="top_9_tab" runat="server" data-bs-toggle="tab"
                                                    href="#cphBody_top_9" role="tab" aria-selected="false">
                                                    <i class="icofont icofont-contacts"></i>Groups
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
                                                                    <label>Item Number</label>
                                                                    <asp:TextBox ID="txtItemNumber" runat="server" CssClass="form-control" placeholder="item number" required=""></asp:TextBox>
                                                                    <asp:RequiredFieldValidator ID="valItemNumber" runat="server" ControlToValidate="txtItemNumber" ValidationGroup="Submit" ErrorMessage="*" ForeColor="Red"></asp:RequiredFieldValidator>     
                                                                </div>
                                                                <div class="col-md-12">
                                                                    <label>Item Name</label>
                                                                    <asp:TextBox ID="txtItemName" runat="server" CssClass="form-control" placeholder="item name" required=""></asp:TextBox>
                                                                    <asp:RequiredFieldValidator ID="valItemName" runat="server" ControlToValidate="txtItemName" ValidationGroup="Submit" ErrorMessage="*" ForeColor="Red"></asp:RequiredFieldValidator>
                                                                </div>
                                                                <div class="col-md-12" style="display:none;">
                                                                    <label>Item Type</label>
                                                                    <asp:DropDownList ID="ddlItemType" runat="server" CssClass="form-control">
                                                                        <asp:ListItem Selected="True" Value="_nonInventoryItem">Non-Inventory</asp:ListItem>
                                                                        <asp:ListItem Value="_inventoryItem">Inventory</asp:ListItem>
                                                                    </asp:DropDownList>
                                                                    <asp:RequiredFieldValidator ID="valItemType" runat="server" ControlToValidate="ddlItemType" ValidationGroup="Submit" ErrorMessage="*" ForeColor="Red"></asp:RequiredFieldValidator>
                                                                </div>
                                                                <div class="col-md-12">
                                                                    <label>Store Display Name</label>
                                                                    <asp:TextBox ID="txtStoreDisplayName" runat="server" CssClass="form-control" placeholder="store display name" required=""></asp:TextBox>
                                                                    <asp:RequiredFieldValidator ID="valStoreDisplayName" runat="server" ControlToValidate="txtStoreDisplayName" ValidationGroup="Submit" ErrorMessage="*" ForeColor="Red"></asp:RequiredFieldValidator>
                                                                </div>
                                                                <div class="col-md-12">
                                                                    <label>Sales Description</label>
                                                                    <asp:TextBox ID="txtSalesDescription" runat="server" CssClass="form-control" placeholder="sales description"></asp:TextBox>
                                                                </div>
                                                                <div class="col-md-12">
                                                                    <label>Detailed Description</label>
                                                                    <asp:TextBox ID="txtDetailedDescription" runat="server" CssClass="form-control" TextMode="MultiLine" placeholder="Detailed Description"></asp:TextBox>
                                                                </div>
                                                                <div class="col-md-12">
                                                                    <label>Base Price</label>
                                                                    <asp:TextBox ID="txtBasePrice" runat="server" CssClass="form-control" onKeyPress="if (event.keyCode!=46 && event.keyCode!=45 && (event.keyCode<48 || event.keyCode>57)) event.returnValue=false"></asp:TextBox>
                                                                </div>
                                                                <div class="col-md-12">
                                                                    <label>Purchase Price</label>
                                                                    <asp:TextBox ID="txtPurchasePrice" runat="server" CssClass="form-control" onKeyPress="if (event.keyCode!=46 && event.keyCode!=45 && (event.keyCode<48 || event.keyCode>57)) event.returnValue=false"></asp:TextBox>
                                                                </div>
                                                                <div class="col-md-12">
                                                                    <label>Quantity Available</label>
                                                                    <asp:TextBox ID="txtQuantityAvailable" runat="server" CssClass="form-control" onKeyPress="if (event.keyCode!=46 && event.keyCode!=45 && (event.keyCode<48 || event.keyCode>57)) event.returnValue=false"></asp:TextBox>
                                                                    <asp:RequiredFieldValidator ID="RequiredFieldValidator2" runat="server" ControlToValidate="txtQuantityAvailable" ValidationGroup="Submit" ErrorMessage="*" ForeColor="Red"></asp:RequiredFieldValidator>
                                                                    <asp:RangeValidator id="Range1" ControlToValidate="txtQuantityAvailable" MinimumValue="0" MaximumValue="2147483647" Type="Integer" Text="Qantity available must be integer and greater or equal than 0" runat="server" ValidationGroup="Submit" ForeColor="Red"/>
                                                                </div>
                                                                <div class="col-md-12">
                                                                    <label>Attribute Display Type</label>
                                                                    <asp:DropDownList ID="ddlAttributeDisplayType" runat="server" CssClass="form-control" OnSelectedIndexChanged="ddlAttributeDisplayType_SelectedIndexChanged" AutoPostBack="true">
                                                                        <asp:ListItem Value=""></asp:ListItem>
                                                                        <asp:ListItem Value="dropdown">Drop Down</asp:ListItem>
                                                                        <asp:ListItem Value="list">List</asp:ListItem>
                                                                    </asp:DropDownList>
                                                                </div>
                                                                <asp:Panel ID="pnlGroupByAttribute" runat="server" Visible="false">
                                                                    <div class="col-md-12">
                                                                        <label>Group By Attribute</label>
                                                                        <asp:DropDownList ID="ddlGroupByAttribute" runat="server" CssClass="form-control" DataTextField="AttributeName" DataValueField="AttributeID"></asp:DropDownList>
                                                                        <asp:RequiredFieldValidator ID="RequiredFieldValidator1" runat="server" ControlToValidate="ddlGroupByAttribute" ValidationGroup="Submit" ErrorMessage="*" ForeColor="Red"></asp:RequiredFieldValidator>
                                                                    </div>
                                                                </asp:Panel>
                                                                <div class="col-md-12">
                                                                    <br />
                                                                    <label>Image Upload</label>
                                                                    <br />
                                                                    <asp:FileUpload ID="fuItemImage" runat="server" />
                                                                    <asp:Button ID="btnUpload" runat="server" Text="Upload" OnClick="btnUpload_Click" CssClass="btn btn-sm btn-solid" />
                                                                    <asp:Button ID="btnResetImage" runat="server" Text="Reset" OnClick="btnResetImage_Click" CssClass="btn btn-sm btn-solid" />
                                                                    <asp:Button ID="btnRemoveImage" runat="server" Text="Remove" OnClick="btnRemoveImage_Click" CssClass="btn btn-sm btn-solid"/>
                                                                    <br />
                                                                    <asp:Image ID="imgItem" runat="server" Width="200px" /><br /><br />
                                                                </div>
                                                                <div class="col-md-12">
                                                                    <br />
                                                                    <label>Logo Image Upload</label>
                                                                    <br />
                                                                    <asp:FileUpload ID="fuLogoItemImage" runat="server" />
                                                                    <asp:Button ID="btnUploadLogoImage" runat="server" Text="Upload" OnClick="btnUploadLogoImage_Click" CssClass="btn btn-sm btn-solid" />
                                                                    <asp:Button ID="btnResetLogoImage" runat="server" Text="Reset" OnClick="btnResetLogoImage_Click" CssClass="btn btn-sm btn-solid" />
                                                                    <asp:Button ID="btnRemoveLogoImage" runat="server" Text="Remove" OnClick="btnRemoveLogoImage_Click" CssClass="btn btn-sm btn-solid"/>
                                                                    <br />
                                                                    <asp:Image ID="imgItemLogo" runat="server" Width="200px" /><br /><br />
                                                                </div>
                                                                <div class="col-md-12">
                                                                    <br />
                                                                    <label>Size Chart Image Upload</label>
                                                                    <br />
                                                                    <asp:FileUpload ID="fuSizeChartImage" runat="server" />
                                                                    <asp:Button ID="btnUploadSizeChartImage" runat="server" Text="Upload" OnClick="btnUploadSizeChartImage_Click" CssClass="btn btn-sm btn-solid" />
                                                                    <asp:Button ID="btnResetSizeChartImage" runat="server" Text="Reset" OnClick="btnResetSizeChartImage_Click" CssClass="btn btn-sm btn-solid" />
                                                                    <asp:Button ID="btnRemoveSizeChartImage" runat="server" Text="Remove" OnClick="btnRemoveSizeChartImage_Click" CssClass="btn btn-sm btn-solid"/>
                                                                    <br />
                                                                    <asp:Image ID="imgSizeChart" runat="server" Width="200px" /><br /><br />
                                                                </div>
                                                                <div class="col-md-12">
                                                                    <label>Hide Size Chart</label>&nbsp;
                                                                    <asp:CheckBox ID="chkHideSizeChart" runat="server" />
                                                                </div>
                                                                <div class="col-md-12">
                                                                    <label>Is NonInventory</label>&nbsp;
                                                                    <asp:CheckBox ID="chkIsNonInventory" runat="server" />
                                                                </div>
                                                                <div class="col-md-12">
                                                                    <label>Unit Item</label>
                                                                    <asp:DropDownList ID="ddlUnitItem" runat="server" CssClass="form-control"></asp:DropDownList>
                                                                    <br />
                                                                </div>

                                                                <div class="col-md-12">
                                                                    <label>Quantity Per Person</label>
                                                                    <asp:TextBox ID="txtQuantityPerPerson" runat="server" CssClass="form-control" onKeyPress="if (event.keyCode!=46 && event.keyCode!=45 && (event.keyCode<48 || event.keyCode>57)) event.returnValue=false"></asp:TextBox>
                                                                </div>
                                                                <div class="col-md-12">
                                                                    <label>Hide Detailed Description</label>&nbsp;
                                                                    <asp:CheckBox ID="chkHideDetailedDescription" runat="server" />
                                                                </div>

                                                                <div class="col-md-12">
                                                                    <label>Enable Peronalization</label>&nbsp;
                                                                    <asp:CheckBox ID="chkEnablePersonalization" runat="server" />
                                                                </div>
                                                                <div class="col-md-12">
                                                                    <label>Enable Selectable Logo</label>&nbsp;
                                                                    <asp:CheckBox ID="chkEnableSelectableLogo" runat="server" />
                                                                </div>
                                                                <div class="col-md-12">
                                                                    <label>Require Logo Selection</label>&nbsp;
                                                                    <asp:CheckBox ID="chkRequireLogoSelection" runat="server" />
                                                                </div>
                                                                <div class="col-md-12">
                                                                    <label>Exclude Display User</label>&nbsp;
                                                                    <asp:CheckBox ID="chkExcludeDisplayUser" runat="server" />
                                                                </div>

                                                                <div class="col-md-12">
                                                                    <label>Company Invoiced</label>&nbsp;
                                                                    <asp:CheckBox ID="cbCompanyInvoiced" runat="server" />
                                                                </div>
                                                                <div class="col-md-12">
                                                                    <label>Allow Backorder</label>&nbsp;
                                                                    <asp:CheckBox ID="cbAllowBackorder" runat="server" />
                                                                </div>
                                                                <div class="col-md-12">
                                                                    <label>Require Approval</label>&nbsp;
                                                                    <asp:CheckBox ID="chkRequireApproval" runat="server" />
                                                                </div>
                                                                <div class="col-md-12">
                                                                    <label>Do Not Display NI Message</label>&nbsp;
                                                                    <asp:CheckBox ID="chkDoNotDisplayNIMessage" runat="server" />
                                                                </div>
                                                                <div class="col-md-12">
                                                                    <label>Is Online</label>&nbsp;
                                                                    <asp:CheckBox ID="cbIsOnline" runat="server" />
                                                                </div>
                                                                <div class="col-md-12">
                                                                    <label>Inactive</label>&nbsp;
                                                                    <asp:CheckBox ID="cbInactive" runat="server" />
                                                                </div>
                                                                <div class="col-md-12">
                                                                    <asp:Button ID="btnSave" runat="server" Text="Save" OnClick="btnSave_Click" CssClass="btn btn-sm btn-solid" ValidationGroup="Submit" />
                                                                    <a id="aCancel" runat="server" href="/Admin/ItemOverview.aspx" class="btn btn-sm btn-solid">Cancel</a>
                                                                    <asp:Button ID="btnDelete" runat="server" Text="Delete" OnClick="btnDelete_Click" CssClass="btn btn-sm btn-solid" CausesValidation="false"/>
                                                                    <a id="a1" runat="server" href="/Admin/ItemCustomization.aspx" class="btn btn-sm btn-solid">Customize Item</a>
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
                                                                <a href="/Admin/ItemAttribute.aspx?itemid=<%= mItemID %>" class="btn btn-sm btn-solid">+ add new</a>
                                                            </div>
                                                            <div class="table-responsive-xl">                                                                <asp:GridView runat="server" ID="gvAttributes" AutoGenerateColumns="False" CssClass="table cart-table order-table" HeaderStyle-CssClass="table-head" GridLines="None" Width="100%" AllowSorting="true" CellSpacing="0" CellPadding="0">
                                                                    <Columns>
                                                                        <asp:BoundField DataField="AttributeName" HeaderText="Attribute" HeaderStyle-HorizontalAlign="Left" ItemStyle-HorizontalAlign="Left" />
                                                                        <asp:BoundField DataField="Sort" HeaderText="Sort" HeaderStyle-HorizontalAlign="Center" ItemStyle-HorizontalAlign="Center" />
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
                                                                                <a href='/admin/itemattribute.aspx?id=<%# Eval("AttributeID") %>'>
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
                                                                <a href="/Admin/ItemVariation.aspx?itemid=<%= mItemID %>" class="btn btn-sm btn-solid">+ configure variations</a>
                                                            </div>
                                                            <div class="table-responsive-xl">
                                                                <asp:GridView runat="server" ID="gvVariations" AutoGenerateColumns="False" CssClass="table cart-table order-table" HeaderStyle-CssClass="table-head" GridLines="None"  Width="100%" AllowSorting="true" CellSpacing="0" CellPadding="0">
                                                                    <Columns>
                                                                        <asp:BoundField DataField="ItemID" HeaderText="Item ID" HeaderStyle-HorizontalAlign="Left" ItemStyle-HorizontalAlign="Left" />
                                                                        <asp:BoundField DataField="ItemNumber" HeaderText="Item Number" HeaderStyle-HorizontalAlign="Left" ItemStyle-HorizontalAlign="Left" />
                                                                        <asp:BoundField DataField="ItemName" HeaderText="Item Name" HeaderStyle-HorizontalAlign="Left" ItemStyle-HorizontalAlign="Left" />
                                                                        <asp:TemplateField HeaderText="Parent" HeaderStyle-HorizontalAlign="Left" ItemStyle-HorizontalAlign="Left">
                                                                            <ItemTemplate>
                                                                                <%# Eval("ParentItem.ItemNumber") %>
                                                                            </ItemTemplate>
                                                                        </asp:TemplateField>
                                                                        <asp:TemplateField HeaderText="Is Variation" HeaderStyle-HorizontalAlign="Center" ItemStyle-HorizontalAlign="Center">
                                                                            <ItemTemplate>
                                                                                <asp:CheckBox ID="chkIsVariation" runat="server" Checked='<%# Eval("IsVariation") %>' Enabled="false" /> 
                                                                            </ItemTemplate>
                                                                        </asp:TemplateField>
                                                                        <%--<asp:HyperLinkField DataTextField="ItemNumber" DataNavigateUrlFields="ItemID" DataNavigateUrlFormatString="Item.aspx?id={0}" HeaderText="ItemNumber" />--%>
                                                                        <asp:TemplateField HeaderText="View" HeaderStyle-HorizontalAlign="Center" ItemStyle-HorizontalAlign="Center">
                                                                            <ItemTemplate>
                                                                                <a href='/admin/item.aspx?id=<%# Eval("ItemID") %>&parentid=<%# Eval("ParentID") %>'>
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
                                        <div class="tab-pane fade" id="top_4" runat="server" role="tabpanel" aria-labelledby="top_4_tab">
                                            <div class="row">
                                                <div class="col-12">
                                                    <div class="card dashboard-table mt-0">
                                                        <div class="card-body table-responsive-sm">
                                                            <div class="top-sec">
                                                                <h3></h3>
                                                                <a href="/Admin/ItemPricing.aspx?itemid=<%= mItemID %>" class="btn btn-sm btn-solid">+ add new</a>
                                                            </div>
                                                            <div class="table-responsive-xl">                                                                <asp:GridView runat="server" ID="gvItemPricing" AutoGenerateColumns="False" CssClass="table cart-table order-table" HeaderStyle-CssClass="table-head" GridLines="None" Width="100%" AllowSorting="true" CellSpacing="0" CellPadding="0">
                                                                    <Columns>
                                                                        <asp:TemplateField HeaderText="Group" HeaderStyle-HorizontalAlign="Center" ItemStyle-HorizontalAlign="Center">
                                                                            <ItemTemplate>
                                                                                <%# Eval("WebsiteGroup.GroupName") %>
                                                                            </ItemTemplate>
                                                                        </asp:TemplateField>
                                                                        <asp:TemplateField HeaderText="Price" HeaderStyle-HorizontalAlign="Center" ItemStyle-HorizontalAlign="Center">
                                                                            <ItemTemplate>
                                                                                <%# string.Format("{0:c}", Eval("Price")) %>
                                                                            </ItemTemplate>
                                                                        </asp:TemplateField>
                                                                        <asp:TemplateField HeaderText="VIEW">
                                                                            <ItemTemplate>
                                                                                <a href='/admin/itempricing.aspx?id=<%# Eval("ItemPricingID") %>'>
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
                                                                <asp:Button ID="btnExportItemDetail" runat="server" class="btn btn-sm btn-solid" Text="Export" OnClick="btnExportItemDetail_Click" />
                                                                <a href="/Admin/ItemDetail.aspx?itemid=<%= mItemID %>" class="btn btn-sm btn-solid">+ add new</a>
                                                            </div>
                                                            <div class="table-responsive-xl">                                                                <asp:GridView runat="server" ID="gvItemDetails" AutoGenerateColumns="False" CssClass="table cart-table order-table" HeaderStyle-CssClass="table-head" GridLines="None" Width="100%" AllowSorting="true" CellSpacing="0" CellPadding="0">
                                                                    <Columns>
                                                                        <asp:BoundField DataField="Attribute" HeaderText="Attribute" HeaderStyle-HorizontalAlign="Left" ItemStyle-HorizontalAlign="Left" />
                                                                        <asp:BoundField DataField="Sort" HeaderText="Sort" HeaderStyle-HorizontalAlign="Center" ItemStyle-HorizontalAlign="Center" />
                                                                        <asp:TemplateField HeaderText="Created Date" HeaderStyle-HorizontalAlign="Center" ItemStyle-HorizontalAlign="Center">
                                                                            <ItemTemplate>
                                                                                <%# Eval("CreatedOn") %>
                                                                            </ItemTemplate>
                                                                        </asp:TemplateField>
                                                                        <asp:TemplateField HeaderText="VIEW">
                                                                            <ItemTemplate>
                                                                                <a href='/admin/itemdetail.aspx?id=<%# Eval("ItemDetailID") %>'>
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
                                        <div class="tab-pane fade" id="top_6" runat="server" role="tabpanel" aria-labelledby="top_6_tab">
                                            <div class="row">
                                                <div class="col-12">
                                                    <div class="card dashboard-table mt-0">
                                                        <div class="card-body table-responsive-sm">
                                                            <div class="top-sec">
                                                                <h3></h3>
                                                                <a href="/Admin/SuperceedingItem.aspx?itemid=<%= mItemID %>" class="btn btn-sm btn-solid">+ add new</a>
                                                            </div>
                                                                <asp:UpdatePanel ID="UpdatePanel1" runat="server">
                                                                    <ContentTemplate>
                                                                    <div class="table-responsive-xl">
                                                                        <asp:GridView ID="gvSuperceedingItem" runat="server" AutoGenerateColumns="False" CssClass="table cart-table order-table" HeaderStyle-CssClass="table-head" GridLines="None" Width="100%" AllowSorting="true" CellSpacing="0" 
                                                                            DataKeyNames="SuperceedingItemID" CellPadding="0" OnRowCommand="gvSuperceedingItem_RowCommand" OnRowDataBound="gvSuperceedingItem_RowDataBound">
                                                                            <Columns>
                                                                                <asp:BoundField DataField="ReplacementItem.ItemName" HeaderText="Item Name" HeaderStyle-HorizontalAlign="Left" ItemStyle-HorizontalAlign="Left" />
                                                                                <asp:BoundField DataField="ReplacementItem.ItemNumber" HeaderText="Item Number" HeaderStyle-HorizontalAlign="Left" ItemStyle-HorizontalAlign="Left" />
                                                                                <asp:BoundField DataField="ReplacementItem.QuantityAvailable" HeaderText="Quantity Available" HeaderStyle-HorizontalAlign="Left" ItemStyle-HorizontalAlign="Left" />
                                                                                <asp:TemplateField HeaderText="VIEW">
                                                                                    <ItemTemplate>
                                                                                        <a href='/admin/superceedingitem.aspx?itemid=<%= mItemID %>&id=<%# Eval("SuperceedingItemID") %>'>
                                                                                            <i class="fa fa-eye text-theme"></i>
                                                                                        </a>
                                                                                    </ItemTemplate>
                                                                                </asp:TemplateField>
                                                                                <asp:TemplateField>
                                                                                    <ItemTemplate>
                                                                                        <asp:HiddenField ID="hfWebsiteTabItemID" runat="server" Value='<%# Eval("SuperceedingItemID") %>' />
                                                                                        <asp:Button ID="btnUp" runat="server" Text="Up" CausesValidation="false" CommandArgument='<%# Eval("SuperceedingItemID") %>' CommandName="MoveUp" />
                                                                                        <asp:Button ID="btnDown" runat="server" Text="Down" CausesValidation="false" CommandArgument='<%# Eval("SuperceedingItemID") %>' CommandName="MoveDown"/>
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
                                        <div class="tab-pane fade" id="top_7" runat="server" role="tabpanel" aria-labelledby="top_7_tab">
                                            <div class="row">
                                                <div class="col-12">
                                                    <div class="card dashboard-table mt-0">
                                                        <div class="card-body table-responsive-sm">
                                                            <div class="top-sec">
                                                                <h3>Personalization</h3>
                                                                <a href="/Admin/ItemPersonalization.aspx?itemid=<%= mItemID %>" class="btn btn-sm btn-solid">+ add new</a>
                                                            </div>
                                                                <asp:UpdatePanel ID="UpdatePanel2" runat="server">
                                                                    <ContentTemplate>
                                                                    <div class="table-responsive-xl">
                                                                        <asp:GridView ID="gvItemPersonalization" runat="server" AutoGenerateColumns="False" CssClass="table cart-table order-table" HeaderStyle-CssClass="table-head" GridLines="None" Width="100%" AllowSorting="true" CellSpacing="0" 
                                                                            DataKeyNames="ItemPersonalizationID" CellPadding="0" OnRowCommand="gvItemPersonalization_RowCommand" OnRowDataBound="gvItemPersonalization_RowDataBound">
                                                                            <Columns>
                                                                                <asp:BoundField DataField="Name" HeaderText="Name" HeaderStyle-HorizontalAlign="Left" ItemStyle-HorizontalAlign="Left" />
                                                                                <asp:BoundField DataField="Type" HeaderText="Type" HeaderStyle-HorizontalAlign="Left" ItemStyle-HorizontalAlign="Left" />
                                                                                <asp:CheckBoxField DataField="InActive" HeaderText="InActive" HeaderStyle-HorizontalAlign="Center" ItemStyle-HorizontalAlign="Center"/>
                                                                                <asp:TemplateField HeaderText="VIEW">
                                                                                    <ItemTemplate>
                                                                                        <a href='/admin/itempersonalization.aspx?itemid=<%= mItemID %>&id=<%# Eval("ItemPersonalizationID") %>'>
                                                                                            <i class="fa fa-eye text-theme"></i>
                                                                                        </a>
                                                                                    </ItemTemplate>
                                                                                </asp:TemplateField>
                                                                                <asp:TemplateField>
                                                                                    <ItemTemplate>
                                                                                        <asp:HiddenField ID="hfItemPersonalizationID" runat="server" Value='<%# Eval("ItemPersonalizationID") %>' />
                                                                                        <asp:Button ID="btnUp" runat="server" Text="Up" CommandArgument='<%# Eval("ItemPersonalizationID") %>' CommandName="MoveUp" />
                                                                                        <asp:Button ID="btnDown" runat="server" Text="Down" CommandArgument='<%# Eval("ItemPersonalizationID") %>' CommandName="MoveDown"/>
                                                                                    </ItemTemplate>
                                                                                </asp:TemplateField>
                                                                                <asp:TemplateField HeaderText="Sort #" HeaderStyle-Width="60px" ItemStyle-Width="60px">
                                                                                    <ItemTemplate>
                                                                                        <asp:TextBox ID="txtSortPersonalization" runat="server" CssClass="form-control" Width="60px"></asp:TextBox>
                                                                                    </ItemTemplate>
                                                                                </asp:TemplateField>
                                                                            </Columns>
                                                                        </asp:GridView>
                                                                    </div>
                                                                    <div class="top-sec">
                                                                        <h3></h3>
                                                                        <asp:LinkButton ID="btnUpdateSortPersonalization" runat="server" OnClick="btnUpdateSortPersonalization_Click" class="btn btn-sm btn-solid" Text="Update Sort"></asp:LinkButton>
                                                                    </div>
                                                                </ContentTemplate>
                                                            </asp:UpdatePanel>

                                                            <div>
                                                                <br />
                                                                <br />
                                                            </div>

                                                            <div class="top-sec">
                                                                <h3>Selectable Logo</h3>
                                                                <a href="/Admin/ItemSelectableLogo.aspx?itemid=<%= mItemID %>" class="btn btn-sm btn-solid">+ add new</a>
                                                            </div>
                                                                <asp:UpdatePanel ID="UpdatePanel3" runat="server">
                                                                    <ContentTemplate>
                                                                    <div class="table-responsive-xl">
                                                                        <asp:GridView ID="gvItemSelectableLogo" runat="server" AutoGenerateColumns="False" CssClass="table cart-table order-table" HeaderStyle-CssClass="table-head" GridLines="None" Width="100%" AllowSorting="true" CellSpacing="0" 
                                                                            DataKeyNames="ItemSelectableLogoID" CellPadding="0" OnRowCommand="gvItemSelectableLogo_RowCommand" OnRowDataBound="gvItemSelectableLogo_RowDataBound">
                                                                            <Columns>
                                                                                <asp:BoundField DataField="SelectableLogo.Name" HeaderText="Name" HeaderStyle-HorizontalAlign="Left" ItemStyle-HorizontalAlign="Left" />
                                                                                <asp:CheckBoxField DataField="SelectableLogo.Inactive" HeaderText="InActive" HeaderStyle-HorizontalAlign="Center" ItemStyle-HorizontalAlign="Center"/>
                                                                                <asp:TemplateField HeaderText="VIEW">
                                                                                    <ItemTemplate>
                                                                                        <a href='/admin/itemselectablelogo.aspx?itemid=<%= mItemID %>&id=<%# Eval("ItemSelectableLogoID") %>'>
                                                                                            <i class="fa fa-eye text-theme"></i>
                                                                                        </a>
                                                                                    </ItemTemplate>
                                                                                </asp:TemplateField>
                                                                            </Columns>
                                                                        </asp:GridView>
                                                                    </div>
                                                                </ContentTemplate>
                                                            </asp:UpdatePanel>

                                                        </div>
                                                    </div>
                                                </div>
                                            </div>
                                        </div>
                                        <div class="tab-pane fade" id="top_8" runat="server" role="tabpanel" aria-labelledby="top_8_tab">
                                            <div class="row">
                                                <div class="col-12">
                                                    <div class="card dashboard-table mt-0">
                                                        <div class="card-body table-responsive-sm">
                                                            <div class="top-sec">
                                                                <h3></h3>
                                                                <!--<a href="/Admin/tabitem.aspx?itemid=<%= mItemID %>" class="btn btn-sm btn-solid">+ add new</a>-->
                                                            </div>
                                                            <div class="table-responsive-xl">                                                                <asp:GridView runat="server" ID="gvItemTab" AutoGenerateColumns="False" CssClass="table cart-table order-table" HeaderStyle-CssClass="table-head" GridLines="None" Width="100%" AllowSorting="true" CellSpacing="0" CellPadding="0">
                                                                    <Columns>
                                                                        <asp:BoundField DataField="WebsiteTab.TabPath" HeaderText="Tab Path" HeaderStyle-HorizontalAlign="Left" ItemStyle-HorizontalAlign="Left" />
                                                                        <asp:TemplateField HeaderText="Created Date" HeaderStyle-HorizontalAlign="Center" ItemStyle-HorizontalAlign="Center">
                                                                            <ItemTemplate>
                                                                                <%# Eval("CreatedOn") %>
                                                                            </ItemTemplate>
                                                                        </asp:TemplateField>
                                                                        <asp:TemplateField HeaderText="VIEW">
                                                                            <ItemTemplate>
                                                                                <a href='/admin/tabitem.aspx?id=<%# Eval("WebSiteTabItemID") %>'>
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

                                        <div class="tab-pane fade" id="top_9" runat="server" role="tabpanel" aria-labelledby="top_9_tab">
                                            <div class="row">
                                                <div class="col-12">
                                                    <div class="card dashboard-table mt-0">
                                                        <div class="card-body table-responsive-sm">
                                                            <div class="top-sec">
                                                                <h3>Include</h3>
                                                                <a href="/Admin/WebsiteGroupItem.aspx?itemid=<%= mItemID %>" class="btn btn-sm btn-solid">+ add new</a>
                                                            </div>
                                                            <div class="table-responsive-xl">                                                                <asp:GridView runat="server" ID="gvWebsiteGroupItem" AutoGenerateColumns="False" CssClass="table cart-table order-table" HeaderStyle-CssClass="table-head" GridLines="None" Width="100%" AllowSorting="true" CellSpacing="0" CellPadding="0">
                                                                    <Columns>
                                                                        <asp:BoundField DataField="GroupName" HeaderText="Group" HeaderStyle-HorizontalAlign="Left" ItemStyle-HorizontalAlign="Left" />
                                                                        <asp:TemplateField HeaderText="VIEW">
                                                                            <ItemTemplate>
                                                                                <a href='/admin/websitegroupitem.aspx?id=<%# Eval("WebsiteGroupItemID") %>'>
                                                                                    <i class="fa fa-eye text-theme"></i>
                                                                                </a>
                                                                            </ItemTemplate>
                                                                        </asp:TemplateField>
                                                                    </Columns>
                                                                </asp:GridView>
                                                            </div>


                                                            <div>
                                                                <br />
                                                                <br />
                                                            </div>

                                                            <div class="top-sec">
                                                                <h3>Exclude</h3>
                                                                <a href="/Admin/WebsiteGroupItemExclude.aspx?itemid=<%= mItemID %>" class="btn btn-sm btn-solid">+ add new</a>
                                                            </div>
                                                            <div class="table-responsive-xl">                                                                <asp:GridView runat="server" ID="gvWebsiteGroupItemExclude" AutoGenerateColumns="False" CssClass="table cart-table order-table" HeaderStyle-CssClass="table-head" GridLines="None" Width="100%" AllowSorting="true" CellSpacing="0" CellPadding="0">
                                                                    <Columns>
                                                                        <asp:BoundField DataField="GroupName" HeaderText="Group" HeaderStyle-HorizontalAlign="Left" ItemStyle-HorizontalAlign="Left" />
                                                                        <asp:TemplateField HeaderText="VIEW">
                                                                            <ItemTemplate>
                                                                                <a href='/admin/websitegroupitemexclude.aspx?id=<%# Eval("WebsiteGroupItemExcludeID") %>'>
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

