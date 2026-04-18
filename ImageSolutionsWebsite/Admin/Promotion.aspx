<%@ Page Language="C#"  MasterPageFile="~/MasterPage.Master" AutoEventWireup="true" CodeBehind="Promotion.aspx.cs" Inherits="ImageSolutionsWebsite.Admin.Promotion" %>
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
                        <div class="tab-pane fade show active" id="orders">
                            <div class="row">
                                <div class="col-12">

                                    <section class="tab-product m-0">
                                        <ul class="nav nav-tabs nav-material" id="top-tab" role="tablist">
                                            <li class="nav-item">
                                                <a class="nav-link active" id="top_1_tab" runat="server" data-bs-toggle="tab"
                                                    href="#cphBody_top_1" role="tab" aria-selected="false">
                                                    <i class="icofont icofont-ui-home"></i>Promotion
                                                </a>
                                                <div class="material-border"></div>
                                            </li>
                                            <li class="nav-item">
                                                <a class="nav-link" id="top_2_tab" runat="server" data-bs-toggle="tab"
                                                    href="#cphBody_top_2" role="tab" aria-selected="true">
                                                    <i class="icofont icofont-man-in-glasses"></i>Orders
                                                </a>
                                                <div class="material-border"></div>
                                            </li>
                                        </ul>
                                    </section><br />
                                    <div class="tab-content nav-material">
                                        <div class="tab-pane fade show active" id="top_1" runat="server" role="tabpanel" aria-labelledby="top_1_tab">


                                            <div class="card dashboard-table mt-0">
                                                <div class="card-body table-responsive-sm">
                                                    <div class="top-sec">
                                                        <h3>Promotion</h3>
                                                    </div>
                                                    <div class="table-responsive-xl">
                                                        <div class="theme-form">
                                                            <div class="form-row row">                                                        
                                                                <asp:UpdatePanel ID="UpdatePanel1" runat="server">
                                                                    <ContentTemplate>
                                                                        <asp:Label ID="lblMessage" runat="server" EnableViewState="false" ForeColor="Red"></asp:Label>
                                                                        <fieldset style="padding:5px; text-align:left;">
                                                                            <div class="col-md-12">
                                                                                <asp:Label ID="lblPromotionDiscount" runat="server" EnableViewState="false" ForeColor="Red"></asp:Label>
                                                                            </div>
                                                                            <div class="col-md-12">
                                                                                <label>Promotion Code</label>
                                                                                <asp:TextBox ID="txtPromotionCode" runat="server" CssClass="form-control" placeholder="promotion code" required=""></asp:TextBox>
                                                                                <asp:RequiredFieldValidator ID="RequiredFieldValidator2" runat="server" ControlToValidate="txtPromotionCode" Display="Dynamic" ErrorMessage="*"></asp:RequiredFieldValidator>
                                                                            </div>
                                                                            <div class="col-md-12">
                                                                                <label>Promotion Name</label>
                                                                                <asp:TextBox ID="txtPromotionName" runat="server" CssClass="form-control" placeholder="promotion name" required=""></asp:TextBox>
                                                                                <asp:RequiredFieldValidator ID="RequiredFieldValidator1" runat="server" ControlToValidate="txtPromotionName" Display="Dynamic" ErrorMessage="*"></asp:RequiredFieldValidator>
                                                                            </div>
                                                                            <div class="col-md-12">
                                                                                <label>Discount Percentage</label>
                                                                                <asp:TextBox ID="txtDiscountPercent" runat="server" CssClass="form-control" placeholder="discount percent"></asp:TextBox>
                                                                                <asp:CompareValidator ID="CompareValidator4" ControlToValidate="txtDiscountPercent" runat="server" ErrorMessage="Invalid discount percentage" Operator="DataTypeCheck" Type="Double" Display="Dynamic"></asp:CompareValidator>
                                                                            </div>
                                                                            <div class="col-md-12">
                                                                                <label>Discount Amount</label>
                                                                                <asp:TextBox ID="txtDiscountAmount" runat="server" CssClass="form-control" placeholder="discount amount"></asp:TextBox>
                                                                                <asp:CompareValidator ID="CompareValidator1" ControlToValidate="txtDiscountAmount" runat="server" ErrorMessage="Invalid discount amount" Operator="DataTypeCheck" Type="Currency" Display="Dynamic"></asp:CompareValidator>
                                                                            </div>
                                                                            <div class="col-md-12">
                                                                                <label>Minimum Order Amount</label>
                                                                                <asp:TextBox ID="txtMinOrderAmount" runat="server" CssClass="form-control" placeholder="minimum amount"></asp:TextBox>
                                                                                <asp:CompareValidator ID="CompareValidator2" ControlToValidate="txtMinOrderAmount" runat="server" ErrorMessage="Invalid minimum order amount" Operator="DataTypeCheck" Type="Currency" Display="Dynamic"></asp:CompareValidator>
                                                                            </div>
                                                                            <div class="col-md-12">
                                                                                <label>Maximum Order Amount</label>
                                                                                <asp:TextBox ID="txtMaxOrderAmount" runat="server" CssClass="form-control" placeholder="maximum amount"></asp:TextBox>
                                                                                <asp:CompareValidator ID="CompareValidator3" ControlToValidate="txtMaxOrderAmount" runat="server" ErrorMessage="Invalid maximum order amount" Operator="DataTypeCheck" Type="Currency" Display="Dynamic"></asp:CompareValidator>
                                                                            </div>
                                                                            <div class="col-md-12">
                                                                                <label>From Date</label>
                                                                                <asp:TextBox ID="txtFromDate" runat="server" CssClass="form-control" placeholder="from date"></asp:TextBox>
                                                                                <asp:CompareValidator ID="CompareValidator5" ControlToValidate="txtFromDate" runat="server" ErrorMessage="Invalid from date format" Operator="DataTypeCheck" Type="Date" Display="Dynamic"></asp:CompareValidator>
                                                                            </div>
                                                                            <div class="col-md-12">
                                                                                <label>To Date</label>
                                                                                <asp:TextBox ID="txtToDate" runat="server" CssClass="form-control" placeholder="from date"></asp:TextBox>
                                                                                <asp:CompareValidator ID="CompareValidator6" ControlToValidate="txtToDate" runat="server" ErrorMessage="Invalid to date format" Operator="DataTypeCheck" Type="Date" Display="Dynamic"></asp:CompareValidator>
                                                                            </div>
                                                                            <div class="col-md-12">
                                                                                <label>Maximum Usage Count</label>
                                                                                <asp:TextBox ID="txtMaxUsageCount" runat="server" CssClass="form-control" placeholder="maximum usage count"></asp:TextBox>
                                                                                <asp:CompareValidator ID="CompareValidator7" ControlToValidate="txtMaxUsageCount" runat="server" ErrorMessage="Invalid maximum usage count" Operator="DataTypeCheck" Type="Integer" Display="Dynamic"></asp:CompareValidator>
                                                                            </div>
                                                                            <div class="col-md-12">
                                                                                <label>Usage Count</label>
                                                                                <asp:Label ID="lblUsageCount" runat="server"></asp:Label>
                                                                            </div>
                                                                            <div class="col-md-12">
                                                                                <label>Free Shipping Service</label>
                                                                                <asp:DropDownList ID="ddlWebsiteShippingService" runat="server" DataValueField="ShippingServiceID" DataTextField="Description"></asp:DropDownList>
                                                                            </div>
        <%--                                                                    <div class="col-md-12">
                                                                                <label>Can Be Combined</label>
                                                                                <asp:CheckBox ID="chkCanBeCombined" runat="server" />
                                                                            </div>
                                                                            <div class="col-md-12">
                                                                                <label>Exclude Sale Item</label>
                                                                                <asp:CheckBox ID="chkExcludeSaleItem" runat="server" />
                                                                            </div>
                                                                            <div class="col-md-12">
                                                                                <label>Sales Tax Exempt</label>
                                                                                <asp:CheckBox ID="chkIsSalesTaxExempt" runat="server" />
                                                                            </div>--%>
                                                                            <div class="col-md-12">
                                                                                <label>Is Active</label>
                                                                                <asp:CheckBox ID="chkIsActive" runat="server" />
                                                                            </div>
                                                                            <div class="col-md-12">
                                                                                <asp:Button ID="btnSave" runat="server" Text="Save" OnClick="btnSave_Click" CssClass="btn btn-sm btn-solid" ValidationGroup="Submit" />
                                                                                <a id="aCancel" runat="server" href="/admin/promotionoverview.aspx" class="btn btn-sm btn-solid">Cancel</a>
                                                                            </div>
                                                                        </fieldset>

                                                                        <%--<table cellpadding="0" cellspacing="0" width="100%" border="0" align="center">
                                                                            <tr> 
                                                                                <td>
                                                                                    <fieldset style="padding:5px; text-align:left;">
                                                                                        <legend style="padding:5px;" class="GridHeader2">Promotion Detail</legend>
                                                                                        <asp:Label ID="lblPromotionDiscount" runat="server" EnableViewState="false" ForeColor="Red"></asp:Label>
                                                                                        <table style="margin-left:5px; margin-right:5px;" cellspacing="5">
                                                                                            <tr>
                                                                                                <td>Promotion Code</td>
                                                                                                <td><asp:TextBox ID="txtPromotionCode" runat="server"></asp:TextBox>
                                                                                                    <asp:RequiredFieldValidator ID="RequiredFieldValidator2" runat="server" ControlToValidate="txtPromotionCode" Display="Dynamic" ErrorMessage="*"></asp:RequiredFieldValidator>
                                                                                                </td>
                                                                                            </tr>
                                                                                            <tr>
                                                                                                <td>Customer Number</td>
                                                                                                <td><asp:TextBox ID="txtCustomerNumber" runat="server"></asp:TextBox> <asp:HyperLink ID="hypCustomer" runat="server">View Customer</asp:HyperLink></td>
                                                                                            </tr>
                                                                                            <tr>
                                                                                                <td>Promotion Name</td>
                                                                                                <td><asp:TextBox ID="txtPromotionName" runat="server"></asp:TextBox>
                                                                                                    <asp:RequiredFieldValidator ID="RequiredFieldValidator1" runat="server" ControlToValidate="txtPromotionName" Display="Dynamic" ErrorMessage="*"></asp:RequiredFieldValidator>
                                                                                                </td>
                                                                                            </tr>
                                                                                            <tr>
                                                                                                <td>Discount Percentage</td>
                                                                                                <td><asp:TextBox ID="txtDiscountPercent" runat="server"></asp:TextBox>%
                                                                                                    <asp:CompareValidator ID="CompareValidator4" ControlToValidate="txtDiscountPercent" runat="server" ErrorMessage="Invalid discount percentage" Operator="DataTypeCheck" Type="Double" Display="Dynamic"></asp:CompareValidator>
                                                                                                </td>
                                                                                            </tr>
                                                                                            <tr>
                                                                                                <td>Discount Amount</td>
                                                                                                <td><asp:TextBox ID="txtDiscountAmount" runat="server"></asp:TextBox>$
                                                                                                    <asp:CompareValidator ID="CompareValidator1" ControlToValidate="txtDiscountAmount" runat="server" ErrorMessage="Invalid discount amount" Operator="DataTypeCheck" Type="Currency" Display="Dynamic"></asp:CompareValidator>
                                                                                                </td>
                                                                                            </tr>
                                                                                            <tr>
                                                                                                <td>Minimum Order Amount</td>
                                                                                                <td><asp:TextBox ID="txtMinOrderAmount" runat="server"></asp:TextBox>
                                                                                                    <asp:CompareValidator ID="CompareValidator2" ControlToValidate="txtMinOrderAmount" runat="server" ErrorMessage="Invalid minimum order amount" Operator="DataTypeCheck" Type="Currency" Display="Dynamic"></asp:CompareValidator>
                                                                                                </td>
                                                                                            </tr>
                                                                                            <tr>
                                                                                                <td>Maximum Order Amount</td>
                                                                                                <td><asp:TextBox ID="txtMaxOrderAmount" runat="server"></asp:TextBox>    
                                                                                                    <asp:CompareValidator ID="CompareValidator3" ControlToValidate="txtMaxOrderAmount" runat="server" ErrorMessage="Invalid maximum order amount" Operator="DataTypeCheck" Type="Currency" Display="Dynamic"></asp:CompareValidator>
                                                                                                </td>
                                                                                            </tr>
                                                                                            <tr>
                                                                                                <td>From Date</td>
                                                                                                <td><asp:TextBox ID="txtFromDate" runat="server"></asp:TextBox>
                                                                                                    <asp:CompareValidator ID="CompareValidator5" ControlToValidate="txtFromDate" runat="server" ErrorMessage="Invalid from date format" Operator="DataTypeCheck" Type="Date" Display="Dynamic"></asp:CompareValidator>
                                                                                                </td>
                                                                                            </tr>
                                                                                            <tr>
                                                                                                <td>To Date</td>
                                                                                                <td><asp:TextBox ID="txtToDate" runat="server"></asp:TextBox>
                                                                                                    <asp:CompareValidator ID="CompareValidator6" ControlToValidate="txtToDate" runat="server" ErrorMessage="Invalid to date format" Operator="DataTypeCheck" Type="Date" Display="Dynamic"></asp:CompareValidator>
                                                                                                </td>
                                                                                            </tr>
                                                                                            <tr>
                                                                                                <td>Maximum Usage Count</td>
                                                                                                <td><asp:TextBox ID="txtMaxUsageCount" runat="server"></asp:TextBox>
                                                                                                    <asp:CompareValidator ID="CompareValidator7" ControlToValidate="txtMaxUsageCount" runat="server" ErrorMessage="Invalid maximum usage count" Operator="DataTypeCheck" Type="Integer" Display="Dynamic"></asp:CompareValidator>
                                                                                                </td>
                                                                                            </tr>
                                                                                            <tr>
                                                                                                <td>Usage Count</td>
                                                                                                <td><asp:Label ID="lblUsageCount" runat="server"></asp:Label></td>
                                                                                            </tr>
                                                                                            <tr>
                                                                                                <td>Can Be Combined</td>
                                                                                                <td><asp:CheckBox ID="chkCanBeCombined" runat="server" /></td>
                                                                                            </tr>
                                                                                            <tr>
                                                                                                <td>Exclude Sale Item</td>
                                                                                                <td><asp:CheckBox ID="chkExcludeSaleItem" runat="server" /></td>
                                                                                            </tr>
                                                                                            <tr>
                                                                                                <td>Sales Tax Exempt</td>
                                                                                                <td><asp:CheckBox ID="chkIsSalesTaxExempt" runat="server" /></td>
                                                                                            </tr>
                                                                                            <tr>
                                                                                                <td>Is Active</td>
                                                                                                <td><asp:CheckBox ID="chkIsActive" runat="server" /></td>
                                                                                            </tr>
                                                                                            <tr>
                                                                                                <td></td>
                                                                                                <td><asp:Button ID="btnSave" runat="server" onclick="btnSave_Click" Text="Save" /></td>
                                                                                            </tr>
                                                                                        </table>
                                                                                    </fieldset>
                                                                                </td>
                                                                            </tr>
                                                                        </table>
                                                                        <br />--%>
                                                                        <%--<table cellpadding="0" cellspacing="0" width="100%" border="0" align="center">
                                                                            <tr> 
                                                                                <td>
                                                                                    <fieldset style="padding:5px; text-align:left;">
                                                                                        <legend style="padding:0px;" class="GridHeader2">Promotion Buy <asp:CheckBox ID="chkIsOrPromotionBuy" runat="server" Text="Apply Or Logic" AutoPostBack="True" OnCheckedChanged="chkIsOrPromotionBuy_CheckedChanged" /></legend>
                                                                                        <asp:Label ID="lblPromotionBuyDescription" runat="server" ForeColor="Red"></asp:Label>
                                                                                        <asp:GridView ID="gvPromotionBuy" AutoGenerateColumns="false" 
                                                                                            GridLines="None" runat="server" Width="100%" ShowFooter="true" ShowHeader="true" 
                                                                                            DataKeyNames="PromotionBuyID, ItemCategoryID, ItemID, ItemDetailID" 
                                                                                            CssClass="GridStyle" HeaderStyle-CssClass="HeaderStyle" RowStyle-CssClass="RowStyle" EditRowStyle-CssClass="RowEditStyle" FooterStyle-CssClass="FooterStyle" PagerStyle-CssClass="PagerStyle" 
                                                                                            onrowdatabound="gvPromotionBuy_RowDataBound" 
                                                                                            onrowcommand="gvPromotionBuy_RowCommand" 
                                                                                            onrowediting="gvPromotionBuy_RowEditing" 
                                                                                            onrowcancelingedit="gvPromotionBuy_RowCancelingEdit" 
                                                                                            onrowupdating="gvPromotionBuy_RowUpdating" 
                                                                                            onrowdeleting="gvPromotionBuy_RowDeleting" >
                                                                                            <Columns>
                                                                                                <asp:TemplateField HeaderText="Item Category" HeaderStyle-HorizontalAlign="Left">
                                                                                                    <ItemTemplate>
                                                                                                        <asp:Repeater ID="rptItemCategory" runat="server">
                                                                                                            <HeaderTemplate>
                                                                                                                <asp:Label ID="lkBtItemCategoryRoot" runat="server">Root</asp:Label>
                                                                                                            </HeaderTemplate>
                                                                                                            <ItemTemplate>
                                                                                                                > <asp:Label ID="lkBtnItemCategory" runat="server"><%#Eval("CategoryName") %></asp:Label>
                                                                                                            </ItemTemplate>
                                                                                                        </asp:Repeater>
                                                                                                    </ItemTemplate>
                                                                                                    <EditItemTemplate>
                                                                                                        <asp:Repeater ID="rptItemCategory" runat="server">
                                                                                                            <HeaderTemplate>
                                                                                                                <asp:LinkButton ID="lkBtnItemCategoryRoot" OnClick="lkBtnItemCategory_Click" CausesValidation="false" CommandArgument="" runat="server">Root</asp:LinkButton>
                                                                                                            </HeaderTemplate>
                                                                                                            <ItemTemplate>
                                                                                                                > <asp:LinkButton ID="lkBtnItemCategory" OnClick="lkBtnItemCategory_Click" CausesValidation="false" CommandArgument='<%#Eval("ItemCategoryID") %>' runat="server"><%#Eval("CategoryName") %></asp:LinkButton>
                                                                                                            </ItemTemplate>
                                                                                                        </asp:Repeater>
                                                                                                        :&nbsp<asp:DropDownList ID="ddlSubcategories" runat="server" DataTextField="CategoryName" DataValueField="ItemCategoryID" AutoPostBack="True" onselectedindexchanged="ddlSubcategories_SelectedIndexChanged"></asp:DropDownList><br />
                                                                                                        <asp:Repeater ID="rptSubcategories" runat="server" Visible="false">
                                                                                                            <ItemTemplate>
                                                                                                                <%# Container.ItemIndex + 1 %>. <asp:LinkButton ID="lkBtnItemCategory" OnClick="lkBtnItemCategory_Click" CausesValidation="false" CommandArgument='<%#Eval("ItemCategoryID") %>' runat="server"><%#Eval("CategoryName") %></asp:LinkButton><br />
                                                                                                            </ItemTemplate>
                                                                                                        </asp:Repeater>
                                                                                                    </EditItemTemplate>
                                                                                                    <FooterTemplate>
                                                                                                        <asp:Repeater ID="rptItemCategory" runat="server">
                                                                                                            <HeaderTemplate>
                                                                                                                <asp:LinkButton ID="lkBtnItemCategoryRoot" OnClick="lkBtnItemCategory_Click" CausesValidation="false" CommandArgument="" runat="server">Root</asp:LinkButton>
                                                                                                            </HeaderTemplate>
                                                                                                            <ItemTemplate>
                                                                                                                > <asp:LinkButton ID="lkBtnItemCategory" OnClick="lkBtnItemCategory_Click" CausesValidation="false" CommandArgument='<%#Eval("ItemCategoryID") %>' runat="server"><%#Eval("CategoryName") %></asp:LinkButton>
                                                                                                            </ItemTemplate>
                                                                                                        </asp:Repeater>
                                                                                                        :&nbsp<asp:DropDownList ID="ddlSubcategories" runat="server" DataTextField="CategoryName" DataValueField="ItemCategoryID" AutoPostBack="True" onselectedindexchanged="ddlSubcategories_SelectedIndexChanged"></asp:DropDownList><br />
                                                                                                        <asp:Repeater ID="rptSubcategories" runat="server" Visible="false">
                                                                                                            <ItemTemplate>
                                                                                                                <%# Container.ItemIndex + 1 %>. <asp:LinkButton ID="lkBtnItemCategory" OnClick="lkBtnItemCategory_Click" CausesValidation="false" CommandArgument='<%#Eval("ItemCategoryID") %>' runat="server"><%#Eval("CategoryName") %></asp:LinkButton><br />
                                                                                                            </ItemTemplate>
                                                                                                        </asp:Repeater>
                                                                                                    </FooterTemplate>
                                                                                                </asp:TemplateField>
                                                                                                <asp:TemplateField HeaderText="Item" HeaderStyle-HorizontalAlign="Left">
                                                                                                    <ItemTemplate>
                                                                                                        <%#Eval("Item.ItemName") %>
                                                                                                    </ItemTemplate>
                                                                                                    <EditItemTemplate>
                                                                                                        <asp:DropDownList ID="ddlItem" runat="server" DataTextField="ItemNumber" DataValueField="ItemID"></asp:DropDownList>
                                                                                                    </EditItemTemplate>
                                                                                                    <FooterTemplate>
                                                                                                        <asp:DropDownList ID="ddlItem" runat="server" DataTextField="ItemNumber" DataValueField="ItemID"></asp:DropDownList>
                                                                                                    </FooterTemplate>
                                                                                                </asp:TemplateField>
                                                                                                <asp:TemplateField HeaderText="Quantity" HeaderStyle-HorizontalAlign="Left">
                                                                                                    <ItemTemplate>
                                                                                                        <%#Eval("Quantity") %>
                                                                                                    </ItemTemplate>
                                                                                                    <EditItemTemplate>
                                                                                                        <asp:TextBox ID="txtQuantity" runat="server" Width="60px" Text='<%#Eval("Quantity") %>'></asp:TextBox>
                                                                                                        <asp:CompareValidator ID="CompareValidator7" ControlToValidate="txtQuantity" runat="server" ErrorMessage="Invalid minimum quantity" Operator="DataTypeCheck" Type="Integer" Display="Dynamic"></asp:CompareValidator>
                                                                                                    </EditItemTemplate>
                                                                                                    <FooterTemplate>
                                                                                                        <asp:TextBox ID="txtQuantity" runat="server" Width="60px"></asp:TextBox>
                                                                                                        <asp:CompareValidator ID="CompareValidator7" ControlToValidate="txtQuantity" runat="server" ErrorMessage="Invalid minimum quantity" Operator="DataTypeCheck" Type="Integer" Display="Dynamic"></asp:CompareValidator>
                                                                                                    </FooterTemplate>
                                                                                                </asp:TemplateField>
                                                                                                <asp:CommandField HeaderText="" FooterStyle-HorizontalAlign="Center" HeaderStyle-HorizontalAlign="Center" ItemStyle-HorizontalAlign="Center" ShowEditButton="True" />
                                                                                                <asp:TemplateField HeaderText="" FooterStyle-HorizontalAlign="Center" HeaderStyle-HorizontalAlign="Center" ItemStyle-HorizontalAlign="Center">
                                                                                                    <ItemTemplate>
                                                                                                        <asp:LinkButton ID="lkBtnDelete" CommandName="Delete" CausesValidation="false" runat="server">Delete</asp:LinkButton>
                                                                                                    </ItemTemplate>
                                                                                                    <FooterTemplate>
                                                                                                        <asp:LinkButton ID="lkBtnAdd" CommandName="Add" runat="server">Add</asp:LinkButton>
                                                                                                    </FooterTemplate>
                                                                                                </asp:TemplateField>
                                                                                            </Columns>
                                                                                        </asp:GridView>
                                                                                    </fieldset>
                                                                                </td>
                                                                            </tr>
                                                                        </table>
                                                                        <br />
                                                                        <table cellpadding="0" cellspacing="0" width="100%" border="0" align="center">
                                                                            <tr> 
                                                                                <td>
                                                                                    <fieldset style="padding:5px; text-align:left;">
                                                                                        <legend style="padding:0px;" class="GridHeader2">Promotion Get <asp:CheckBox ID="chkIsOrPromotionGet" runat="server" Text="Apply Or Logic" AutoPostBack="True" OnCheckedChanged="chkIsOrPromotionGet_CheckedChanged" /></legend>
                                                                                        <asp:Label ID="lblPromotionGetDescription" runat="server" ForeColor="Red"></asp:Label>
                                                                                        <asp:GridView ID="gvPromotionGet" AutoGenerateColumns="false" 
                                                                                            GridLines="None" runat="server" Width="100%" ShowFooter="true" ShowHeader="true" 
                                                                                            DataKeyNames="PromotionGetID, ItemCategoryID, ItemID, ItemDetailID" 
                                                                                            CssClass="GridStyle" HeaderStyle-CssClass="HeaderStyle" RowStyle-CssClass="RowStyle" EditRowStyle-CssClass="RowEditStyle" FooterStyle-CssClass="FooterStyle" PagerStyle-CssClass="PagerStyle" 
                                                                                            onrowdatabound="gvPromotionGet_RowDataBound" 
                                                                                            onrowcommand="gvPromotionGet_RowCommand" 
                                                                                            onrowediting="gvPromotionGet_RowEditing" 
                                                                                            onrowcancelingedit="gvPromotionGet_RowCancelingEdit" 
                                                                                            onrowupdating="gvPromotionGet_RowUpdating" 
                                                                                            onrowdeleting="gvPromotionGet_RowDeleting" >
                                                                                            <Columns>
                                                                                                <asp:TemplateField HeaderText="Item Category" HeaderStyle-HorizontalAlign="Left">
                                                                                                    <ItemTemplate>
                                                                                                        <asp:Repeater ID="rptItemCategory" runat="server">
                                                                                                            <HeaderTemplate>
                                                                                                                <asp:Label ID="lkBtItemCategoryRoot" runat="server">Root</asp:Label>
                                                                                                            </HeaderTemplate>
                                                                                                            <ItemTemplate>
                                                                                                                > <asp:Label ID="lkBtnItemCategory" runat="server"><%#Eval("CategoryName") %></asp:Label>
                                                                                                            </ItemTemplate>
                                                                                                        </asp:Repeater>
                                                                                                    </ItemTemplate>
                                                                                                    <EditItemTemplate>
                                                                                                        <asp:Repeater ID="rptItemCategory" runat="server">
                                                                                                            <HeaderTemplate>
                                                                                                                <asp:LinkButton ID="lkBtnItemCategoryRoot" OnClick="lkBtnItemCategory_Click" CausesValidation="false" CommandArgument="" runat="server">Root</asp:LinkButton>
                                                                                                            </HeaderTemplate>
                                                                                                            <ItemTemplate>
                                                                                                                > <asp:LinkButton ID="lkBtnItemCategory" OnClick="lkBtnItemCategory_Click" CausesValidation="false" CommandArgument='<%#Eval("ItemCategoryID") %>' runat="server"><%#Eval("CategoryName") %></asp:LinkButton>
                                                                                                            </ItemTemplate>
                                                                                                        </asp:Repeater>
                                                                                                        :&nbsp<asp:DropDownList ID="ddlSubcategories" runat="server" DataTextField="CategoryName" DataValueField="ItemCategoryID" AutoPostBack="True" onselectedindexchanged="ddlSubcategories_SelectedIndexChanged"></asp:DropDownList><br />
                                                                                                        <asp:Repeater ID="rptSubcategories" runat="server" Visible="false">
                                                                                                            <ItemTemplate>
                                                                                                                <%# Container.ItemIndex + 1 %>. <asp:LinkButton ID="lkBtnItemCategory" OnClick="lkBtnItemCategory_Click" CausesValidation="false" CommandArgument='<%#Eval("ItemCategoryID") %>' runat="server"><%#Eval("CategoryName") %></asp:LinkButton><br />
                                                                                                            </ItemTemplate>
                                                                                                        </asp:Repeater>
                                                                                                    </EditItemTemplate>
                                                                                                    <FooterTemplate>
                                                                                                        <asp:Repeater ID="rptItemCategory" runat="server">
                                                                                                            <HeaderTemplate>
                                                                                                                <asp:LinkButton ID="lkBtnItemCategoryRoot" OnClick="lkBtnItemCategory_Click" CausesValidation="false" CommandArgument="" runat="server">Root</asp:LinkButton>
                                                                                                            </HeaderTemplate>
                                                                                                            <ItemTemplate>
                                                                                                                > <asp:LinkButton ID="lkBtnItemCategory" OnClick="lkBtnItemCategory_Click" CausesValidation="false" CommandArgument='<%#Eval("ItemCategoryID") %>' runat="server"><%#Eval("CategoryName") %></asp:LinkButton>
                                                                                                            </ItemTemplate>
                                                                                                        </asp:Repeater>
                                                                                                        :&nbsp<asp:DropDownList ID="ddlSubcategories" runat="server" DataTextField="CategoryName" DataValueField="ItemCategoryID" AutoPostBack="True" onselectedindexchanged="ddlSubcategories_SelectedIndexChanged"></asp:DropDownList><br />
                                                                                                        <asp:Repeater ID="rptSubcategories" runat="server" Visible="false">
                                                                                                            <ItemTemplate>
                                                                                                                <%# Container.ItemIndex + 1 %>. <asp:LinkButton ID="lkBtnItemCategory" OnClick="lkBtnItemCategory_Click" CausesValidation="false" CommandArgument='<%#Eval("ItemCategoryID") %>' runat="server"><%#Eval("CategoryName") %></asp:LinkButton><br />
                                                                                                            </ItemTemplate>
                                                                                                        </asp:Repeater>
                                                                                                    </FooterTemplate>
                                                                                                </asp:TemplateField>
                                                                                                <asp:TemplateField HeaderText="Item" HeaderStyle-HorizontalAlign="Left">
                                                                                                    <ItemTemplate>
                                                                                                        <%#Eval("Item.ItemName") %>
                                                                                                    </ItemTemplate>
                                                                                                    <EditItemTemplate>
                                                                                                        <asp:DropDownList ID="ddlItem" runat="server" DataTextField="ItemNumber" DataValueField="ItemID"></asp:DropDownList>
                                                                                                    </EditItemTemplate>
                                                                                                    <FooterTemplate>
                                                                                                        <asp:DropDownList ID="ddlItem" runat="server" DataTextField="ItemNumber" DataValueField="ItemID"></asp:DropDownList>
                                                                                                    </FooterTemplate>
                                                                                                </asp:TemplateField>
                                                                                                <asp:TemplateField HeaderText="Quantity" HeaderStyle-HorizontalAlign="Left">
                                                                                                    <ItemTemplate>
                                                                                                        <%#Eval("Quantity") %>
                                                                                                    </ItemTemplate>
                                                                                                    <EditItemTemplate>
                                                                                                        <asp:TextBox ID="txtQuantity" runat="server" Width="60px" Text='<%#Eval("Quantity") %>'></asp:TextBox>
                                                                                                        <asp:CompareValidator ID="CompareValidator7" ControlToValidate="txtQuantity" runat="server" ErrorMessage="Invalid minimum quantity" Operator="DataTypeCheck" Type="Integer" Display="Dynamic"></asp:CompareValidator>
                                                                                                    </EditItemTemplate>
                                                                                                    <FooterTemplate>
                                                                                                        <asp:TextBox ID="txtQuantity" runat="server" Width="60px"></asp:TextBox>
                                                                                                        <asp:CompareValidator ID="CompareValidator7" ControlToValidate="txtQuantity" runat="server" ErrorMessage="Invalid minimum quantity" Operator="DataTypeCheck" Type="Integer" Display="Dynamic"></asp:CompareValidator>
                                                                                                    </FooterTemplate>
                                                                                                </asp:TemplateField>
                                                                                                <asp:CommandField HeaderText="" FooterStyle-HorizontalAlign="Center" HeaderStyle-HorizontalAlign="Center" ItemStyle-HorizontalAlign="Center" ShowEditButton="True" />
                                                                                                <asp:TemplateField HeaderText="" FooterStyle-HorizontalAlign="Center" HeaderStyle-HorizontalAlign="Center" ItemStyle-HorizontalAlign="Center">
                                                                                                    <ItemTemplate>
                                                                                                        <asp:LinkButton ID="lkBtnDelete" CommandName="Delete" CausesValidation="false" runat="server">Delete</asp:LinkButton>
                                                                                                    </ItemTemplate>
                                                                                                    <FooterTemplate>
                                                                                                        <asp:LinkButton ID="lkBtnAdd" CommandName="Add" runat="server">Add</asp:LinkButton>
                                                                                                    </FooterTemplate>
                                                                                                </asp:TemplateField>
                                                                                            </Columns>
                                                                                        </asp:GridView>
                                                                                    </fieldset>
                                                                                </td>
                                                                            </tr>
                                                                        </table>
                                                                        <br />--%>
                                                                    </ContentTemplate>
                                                                </asp:UpdatePanel>
                                                       
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
                                                            </div>
                                                            <div class="table-responsive-xl">
                                                                <asp:GridView ID="gvPayments" runat="server" AutoGenerateColumns="False" DataKeyNames="SalesOrderID" CssClass="table cart-table wishlist-table" HeaderStyle-CssClass="table-head" Width="100%" 
                                                                    GridLines="None" AllowSorting="true" CellSpacing="0" CellPadding="0">
                                                                    <Columns>
                                                                        <asp:TemplateField HeaderText="Order Number" ItemStyle-HorizontalAlign="Center" HeaderStyle-HorizontalAlign="Center">
                                                                            <ItemTemplate>
                                                                                <%# string.Format("{0:c}", Eval("SalesOrder.SalesOrderID"))%>
                                                                            </ItemTemplate>
                                                                        </asp:TemplateField>
                                                                        <asp:TemplateField HeaderText="Account" ItemStyle-HorizontalAlign="Center" HeaderStyle-HorizontalAlign="Center">
                                                                            <ItemTemplate>
                                                                                <%# Eval("SalesOrder.Account.AccountName")%>
                                                                            </ItemTemplate>
                                                                        </asp:TemplateField>
                                                                        <asp:TemplateField HeaderText="User" HeaderStyle-HorizontalAlign="Center" ItemStyle-HorizontalAlign="Center">
                                                                            <ItemTemplate>
                                                                                <%# Eval("SalesOrder.UserWebsite.Description")%>
                                                                            </ItemTemplate>
                                                                        </asp:TemplateField>
                                                                        <asp:TemplateField HeaderText="Total" HeaderStyle-HorizontalAlign="Center" ItemStyle-HorizontalAlign="Center">
                                                                            <ItemTemplate>
                                                                                <%# string.Format("{0:c}", Eval("SalesOrder.Total"))%>
                                                                            </ItemTemplate>
                                                                            </asp:TemplateField>
                                                                        <asp:TemplateField HeaderText="CreatedOn" HeaderStyle-HorizontalAlign="Center" ItemStyle-HorizontalAlign="Center">
                                                                            <ItemTemplate>
                                                                                <%# string.Format("{0:d}", Eval("CreatedOn"))%>
                                                                            </ItemTemplate>
                                                                        </asp:TemplateField>
                                                                    </Columns>
                                                                </asp:GridView>
                                                                <uc1:Pager runat="server" ID="ucPager" PagingMode="Redirect" PageSize="20" PagingRecordText="Rows"/>
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