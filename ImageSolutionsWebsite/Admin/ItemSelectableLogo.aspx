<%@ Page Language="C#" MasterPageFile="~/MasterPage.Master" AutoEventWireup="true" CodeBehind="ItemSelectableLogo.aspx.cs" Inherits="ImageSolutionsWebsite.Admin.ItemSelectableLogo" %>
<%@ Register Src="~/Control/AdminNavigation.ascx" TagPrefix="uc" TagName="AdminNavigation" %>

<asp:Content ID="cHead" ContentPlaceHolderID="head" runat="server">
</asp:Content>
<asp:Content ID="cHeader" ContentPlaceHolderID="cphHeader" runat="server">
    <script type="text/javascript">    
       function ConfirmOnApplyToAll() {
        if (confirm("Are you sure you want to apply the location and size to all logos for this item?") == true)
            return true;
        else
            return false;
        }
        function ConfirmOnDelete() {
            if (confirm("Are you sure you want to delete?") == true)
                return true;
            else
                return false;
        }
    </script>
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
                                    <section class="tab-product m-0">
                                        <ul class="nav nav-tabs nav-material" id="top-tab" role="tablist">
                                            <li class="nav-item">
                                                <a class="nav-link active" id="top_1_tab" runat="server" data-bs-toggle="tab"
                                                    href="#cphBody_top_1" role="tab" aria-selected="true">
                                                    <i class="icofont icofont-ui-home"></i>Selectable Logo
                                                </a>
                                                <div class="material-border"></div>
                                            </li>
                                            <li class="nav-item">
                                                <a class="nav-link" id="top_2_tab" runat="server" data-bs-toggle="tab"
                                                    href="#cphBody_top_2" role="tab" aria-selected="false">
                                                    <i class="icofont icofont-man-in-glasses"></i>Exclude Attribute
                                                </a>
                                                <div class="material-border"></div>
                                            </li>
                                            <li class="nav-item" id="navGroups" runat="server">
                                                <a class="nav-link" id="top_3_tab" runat="server" data-bs-toggle="tab"
                                                    href="#cphBody_top_3" role="tab" aria-selected="false">
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
                                                    <div class="table-responsive-xl">
                                                        <div class="theme-form">

                                                            <div class="form-row row">
                                                                <div class="col-md-12">
                                                                    <label>Selectable Logo</label>
                                                                    <asp:DropDownList ID="ddlSelectableLogo" runat="server" CssClass="form-control" placeholder="logo" required="" DataValueField="SelectableLogoID" DataTextField="Name" AutoPostBack="true" OnSelectedIndexChanged="ddlSelectableLogo_SelectedIndexChanged"></asp:DropDownList>
                                                                </div>
                                                                <div class="col-md-12">
                                                                    <label>Position X (%)</label>
                                                                    <asp:TextBox ID="txtPositionXPercent" runat="server" CssClass="form-control" placeholder="Position X (%)" onKeyPress="if (event.keyCode!=45 && (event.keyCode<48 || event.keyCode>57)) event.returnValue=false" AutoPostBack="true" OnTextChanged="txtPositionXPercent_TextChanged"></asp:TextBox>
                                                                </div>
                                                                <div class="col-md-12">
                                                                    <label>Position Y (%)</label>
                                                                    <asp:TextBox ID="txtPositionYPercent" runat="server" CssClass="form-control" placeholder="Position Y (%)" onKeyPress="if (event.keyCode!=45 && (event.keyCode<48 || event.keyCode>57)) event.returnValue=false" AutoPostBack="true" OnTextChanged="txtPositionYPercent_TextChanged"></asp:TextBox>
                                                                </div>
                                                                <div class="col-md-12">
                                                                    <label>Width (px)</label>
                                                                    <asp:TextBox ID="txtWidth" runat="server" CssClass="form-control" placeholder="Width (px)" onKeyPress="if (event.keyCode!=45 && (event.keyCode<48 || event.keyCode>57)) event.returnValue=false" AutoPostBack="true" OnTextChanged="txtWidth_TextChanged"></asp:TextBox>
                                                                    <br />
                                                                
                                                                </div>
                                                                <div class="col-md-12">
                                                                    <asp:Button ID="btnSave" runat="server" Text="Save" OnClick="btnSave_Click" CssClass="btn btn-sm btn-solid" ValidationGroup="Submit" />
                                                                    <a id="aCancel" runat="server" href="/Admin/Item.aspx?id=<%= mItemID %>" class="btn btn-sm btn-solid">Cancel</a>
                                                                    <asp:Button ID="btnDelete" runat="server" Text="Delete" OnClick="btnDelete_Click" CssClass="btn btn-sm btn-solid" CausesValidation="false" OnClientClick="return ConfirmOnDelete();"/>
                                                                    <asp:Button ID="btnApplyToAll" runat="server" Text="Apply To All Logos" OnClick="btnApplyToAll_Click" CssClass="btn btn-sm btn-solid" CausesValidation="false" OnClientClick="return ConfirmOnApplyToAll();"/>
                                                                    <br />
                                                                    <br />
                                                                </div>
                                                            </div>

                                                            <asp:Panel ID="pnlPreview" runat="server">
                                                                <div class="top-sec">
                                                                    <h3>Preview</h3> 
                                                                </div>
                                                                <div class="table-responsive-xl">
                                                                    <div class="theme-form">
                                                                        <div class="form-row row">        
                                                                            <div class="col-md-12" style="display:none;">
                                                                                <label>Logo File</label>
                                                                                <asp:Image ID="imgUploadedLogo" runat="server" Width="100px" Visible="false" /><br />
                                                                            </div>
                                                                            <div class="col-md-12">
                                                                                <div class="row">
                                                                                    <div><asp:Image ID="imgItem" runat="server" alt="" class="img-fluid blur-up lazyload" Width="50%"/></div>
                                                                                </div>
                                                                            </div>
                                                                        </div>
                                                                    </div>
                                                                </div>
                                                            </asp:Panel>

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
                                                                <a href="/Admin/itemselectablelogoexcludeattribute.aspx?itemselectablelogoid=<%=_ItemSelectableLogo.ItemSelectableLogoID%>" class="btn btn-sm btn-solid">+ add new</a>
                                                            </div>
                                                            <div class="table-responsive-xl">
                                                                <asp:GridView runat="server" ID="gvItemSelectableLogoExcludeAttribute" AutoGenerateColumns="False" CssClass="table cart-table order-table" HeaderStyle-CssClass="table-head" GridLines="None" Width="100%" AllowSorting="true" CellSpacing="0" CellPadding="0" >
                                                                    <Columns>
                                                                        <asp:BoundField DataField="AttributeValue.Value" HeaderText="Attribute" HeaderStyle-HorizontalAlign="Left" ItemStyle-HorizontalAlign="Left" />
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
                                                                                <a href='/admin/itemselectablelogoexcludeattribute.aspx?id=<%# Eval("ItemSelectableLogoExcludeAttributeID") %>&itemselectablelogoid=<%# Eval("ItemSelectableLogoID") %>'>
                                                                                    <i class="fa fa-eye text-theme"></i>
                                                                                </a>
                                                                            </ItemTemplate>
                                                                        </asp:TemplateField>
                                                                    </Columns>
                                                                </asp:GridView>
                                                                <asp:Button ID="btnExcludeAttributeApplyToAllItems" runat="server" Text="Add Missing Attributes To All Items" CssClass="btn btn-sm btn-solid" OnClick="btnExcludeAttributeApplyToAllItems_Click" />
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
                                                                <a href="/Admin/WebsiteGroupItemSelectableLogo.aspx?itemselectablelogoid=<%= mItemSelectableLogoID %>" class="btn btn-sm btn-solid">+ add new</a>
                                                            </div>
                                                            <div class="table-responsive-xl">
                                                                <asp:GridView runat="server" ID="gvWebsiteGroupItemSelectableLogo" AutoGenerateColumns="False" CssClass="table cart-table order-table" HeaderStyle-CssClass="table-head" GridLines="None" Width="100%" AllowSorting="true" CellSpacing="0" CellPadding="0">
                                                                    <Columns>
                                                                        <asp:BoundField DataField="GroupName" HeaderText="Group" HeaderStyle-HorizontalAlign="Left" ItemStyle-HorizontalAlign="Left" />
                                                                        <asp:TemplateField HeaderText="VIEW">
                                                                            <ItemTemplate>
                                                                                <a href='/admin/websitegroupitemselectablelogo.aspx?id=<%# Eval("WebsiteGroupItemSelectableLogoID") %>'>
                                                                                    <i class="fa fa-eye text-theme"></i>
                                                                                </a>
                                                                            </ItemTemplate>
                                                                        </asp:TemplateField>
                                                                    </Columns>
                                                                </asp:GridView>
                                                                <asp:Button ID="btnGroupApplyToAllItems" runat="server" Text="Add Missing Groups To All Items" CssClass="btn btn-sm btn-solid" OnClick="btnGroupApplyToAllItems_Click" />
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
