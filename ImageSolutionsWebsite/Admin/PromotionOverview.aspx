<%@ Page Language="C#"  MasterPageFile="~/MasterPage.Master" AutoEventWireup="true" CodeBehind="PromotionOverview.aspx.cs" Inherits="ImageSolutionsWebsite.Admin.PromotionOverview" %>
<%@ Register Src="~/Control/AdminNavigation.ascx" TagPrefix="uc" TagName="AdminNavigation" %>

<%@ Register Src="~/Control/Pager.ascx" TagPrefix="uc1" TagName="Pager" %>
<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="asp" %>

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
                                            <div class="row">
                                                <div class="col-lg-6"><h3>Promotions</h3> </div>
                                                <div class="col-lg-6 text-end">
                                                    <ul class="header-dropdown">
                                                        <li class="onhover-dropdown">
                                                            <i class="fa fa-gear" aria-hidden="true"></i>
                                                            <ul class="onhover-show-div">
                                                                <li style="display:flex"><a href="/admin/Promotion.aspx">+ Add New</a></li>
                                                                <li style="display:flex"><a href="/admin/PromotionImport.aspx">Import</a></li>
                                                                <li style="display:flex"><asp:LinkButton ID="btnExport" runat="server" Text="Export" OnClick="btnExport_Click"></asp:LinkButton></li>
                                                            </ul>
                                                        </li>
                                                    </ul>      
                                                </div>                                                                                                                                         
                                            </div>
                                            <div class="row">
                                                <div class="top-sec" id="filter" runat="server" visible="true">
                                                    Promotion Code:&nbsp;<asp:TextBox ID="txtPromotionCode" runat="server" placeholder="promotion code" AutoPostBack="true" OnTextChanged="txtPromotionCode_TextChanged"></asp:TextBox>&nbsp;
                                                    Promotion Name:&nbsp;<asp:TextBox ID="txtPromotionName" runat="server" placeholder="promotion name" AutoPostBack="true" OnTextChanged="txtPromotionName_TextChanged"></asp:TextBox>&nbsp;
                                                    Active Only:&nbsp;<asp:CheckBox ID="chkActiveOnly" runat="server" AutoPostBack="true" OnCheckedChanged="chkActiveOnly_CheckedChanged"/>&nbsp;
                                                </div>
                                            </div>
                                            <div class="table-responsive-xl">
<%--                                                <asp:GridView ID="gvPromotions" CssClass="GridStyle" runat="server" Width="100%" 
                                                    GridLines="None" AutoGenerateColumns="False" DataKeyNames="PromotionID, PromotionCode" 
                                                    HeaderStyle-CssClass="HeaderStyle" RowStyle-CssClass="RowStyle" AllowSorting="true" 
                                                    EditRowStyle-CssClass="RowEditStyle" FooterStyle-CssClass="FooterStyle" PagerStyle-CssClass="PagerStyle" 
                                                    CellSpacing="0" CellPadding="0" onsorting="gvPromotions_Sorting" 
                                                    onrowcommand="gvPromotions_RowCommand">--%>
                                                    <asp:GridView ID="gvPromotions" runat="server" Width="100%"  CssClass="table cart-table wishlist-table"
                                                        GridLines="None" AutoGenerateColumns="False" DataKeyNames="PromotionID, PromotionCode" 
                                                        AllowSorting="true"                                                        
                                                        CellSpacing="0" CellPadding="0" onsorting="gvPromotions_Sorting" >
                                                    <Columns>
                                                        <asp:TemplateField HeaderText="Promotion Code" HeaderStyle-HorizontalAlign="Left" ItemStyle-HorizontalAlign="Left">
                                                            <HeaderTemplate>
                                                                <asp:LinkButton runat="server" CommandName="Sort" CommandArgument="PromotionCode" ForeColor="Black">Promotion Code</asp:LinkButton>
                                                                <%--<asp:TextBox ID="txtPromotionCode" runat="server" AutoPostBack="true" OnTextChanged="FilterSearchedChanged"></asp:TextBox>--%>
                                                                <%--<asp:TextBoxWatermarkExtender ID="TextBoxWatermarkExtender1" runat="server" TargetControlID="txtPromotionCode" WatermarkText="Search..."></asp:TextBoxWatermarkExtender>--%>
                                                            </HeaderTemplate>
                                                            <ItemTemplate>
                                                                <asp:HyperLink ID="hypPromotion" runat="server" Text='<%# Eval("PromotionCode")%>' NavigateUrl='<%#WebUtility.PageSURL("Admin/Promotion.aspx?id=" + Eval("PromotionID")) %>'></asp:HyperLink>
                                                            </ItemTemplate>
                                                        </asp:TemplateField>
                                                        <asp:TemplateField HeaderText="Promotion Name" HeaderStyle-HorizontalAlign="Left" ItemStyle-HorizontalAlign="Left">
                                                            <HeaderTemplate>
                                                                <asp:LinkButton runat="server" CommandName="Sort" CommandArgument="PromotionName" ForeColor="Black">Promotion Name</asp:LinkButton>
                                                                <%--<asp:TextBox ID="txtPromotionName" runat="server" AutoPostBack="true" OnTextChanged="FilterSearchedChanged"></asp:TextBox>--%>
                                                                <%--<asp:TextBoxWatermarkExtender ID="TextBoxWatermarkExtender2" runat="server" TargetControlID="txtPromotionName" WatermarkText="Search..."></asp:TextBoxWatermarkExtender>--%>
                                                            </HeaderTemplate>
                                                            <ItemTemplate>
                                                                <%# Eval("PromotionName")%>
                                                            </ItemTemplate>
                                                        </asp:TemplateField>
                                                        <%--<asp:TemplateField HeaderText="Customer Email" HeaderStyle-HorizontalAlign="Left" ItemStyle-HorizontalAlign="Left">
                                                            <HeaderTemplate>
                                                                <p>Customer Email</p>
                                                                <asp:TextBox ID="txtCustomerEmail" runat="server" AutoPostBack="true" OnTextChanged="FilterSearchedChanged"></asp:TextBox><asp:TextBoxWatermarkExtender ID="TextBoxWatermarkExtender3" runat="server" TargetControlID="txtCustomerEmail" WatermarkText="Search..."></asp:TextBoxWatermarkExtender>
                                                            </HeaderTemplate>
                                                            <ItemTemplate>
                                                                <%# Eval("Customer.Email")%>
                                                            </ItemTemplate>
                                                        </asp:TemplateField>
                                                        <asp:TemplateField HeaderText="First Name" HeaderStyle-HorizontalAlign="Left" ItemStyle-HorizontalAlign="Left">
                                                            <HeaderTemplate>
                                                                <p>First Name</p>
                                                                <asp:TextBox ID="txtCustomerFirstName" runat="server" AutoPostBack="true" OnTextChanged="FilterSearchedChanged"></asp:TextBox><asp:TextBoxWatermarkExtender ID="TextBoxWatermarkExtender4" runat="server" TargetControlID="txtCustomerFirstName" WatermarkText="Search..."></asp:TextBoxWatermarkExtender>
                                                            </HeaderTemplate>
                                                            <ItemTemplate>
                                                                <%# Eval("Customer.FirstName")%>
                                                            </ItemTemplate>
                                                        </asp:TemplateField>
                                                        <asp:TemplateField HeaderText="Last Name" HeaderStyle-HorizontalAlign="Left" ItemStyle-HorizontalAlign="Left">
                                                            <HeaderTemplate>
                                                                <p>First Name</p>
                                                                <asp:TextBox ID="txtCustomerLastName" runat="server" AutoPostBack="true" OnTextChanged="FilterSearchedChanged"></asp:TextBox><asp:TextBoxWatermarkExtender ID="TextBoxWatermarkExtender5" runat="server" TargetControlID="txtCustomerLastName" WatermarkText="Search..."></asp:TextBoxWatermarkExtender>
                                                            </HeaderTemplate>
                                                            <ItemTemplate>
                                                                <%# Eval("Customer.LastName")%>
                                                            </ItemTemplate>
                                                        </asp:TemplateField>--%>
                                                        <asp:TemplateField HeaderText="Discount %" ItemStyle-HorizontalAlign="Left" HeaderStyle-HorizontalAlign="Left">
                                                            <ItemTemplate>
                                                                <%# String.Format("{0:P}", Eval("DiscountPercent"))%>
                                                            </ItemTemplate>
                                                        </asp:TemplateField>
                                                        <asp:TemplateField HeaderText="Discount $" ItemStyle-HorizontalAlign="Left" HeaderStyle-HorizontalAlign="Left">
                                                            <ItemTemplate>
                                                                <%# String.Format("{0:c}", Eval("DiscountAmount"))%>
                                                            </ItemTemplate>
                                                        </asp:TemplateField>
                                                        <asp:TemplateField HeaderText="From Date" ItemStyle-HorizontalAlign="Center" HeaderStyle-HorizontalAlign="Center">
                                                            <ItemTemplate>
                                                                <%# String.Format("{0:MM/dd/yyyy}", Eval("FromDate"))%>
                                                            </ItemTemplate>
                                                        </asp:TemplateField>
                                                        <asp:TemplateField HeaderText="To Date" ItemStyle-HorizontalAlign="Center" HeaderStyle-HorizontalAlign="Center">
                                                            <ItemTemplate>
                                                                <%# String.Format("{0:MM/dd/yyyy}", Eval("ToDate"))%>
                                                            </ItemTemplate>
                                                        </asp:TemplateField>
                                                        <asp:TemplateField HeaderText="Usage Count" ItemStyle-HorizontalAlign="Center" HeaderStyle-HorizontalAlign="Center">
                                                            <ItemTemplate>
                                                                <%# Eval("UsageCount")%>
                                                            </ItemTemplate>
                                                        </asp:TemplateField>
                                                        <asp:TemplateField HeaderText="Is Active" ItemStyle-HorizontalAlign="Center" HeaderStyle-HorizontalAlign="Center">
                                                            <ItemTemplate>
                                                                <asp:CheckBox ID="chkIsActive" runat="server" Enabled="false" Checked='<%#Eval("IsActive") == DBNull.Value ? false : Eval("IsActive") %>' />
                                                            </ItemTemplate>
                                                        </asp:TemplateField>
<%--                                                        <asp:TemplateField HeaderText="Usage Summary" ItemStyle-HorizontalAlign="Center" HeaderStyle-HorizontalAlign="Center">
                                                            <ItemTemplate>
                                                                <asp:LinkButton ID="lbnReport" runat="server" CommandName="Report" CausesValidation="false">Report</asp:LinkButton>
                                                            </ItemTemplate>
                                                        </asp:TemplateField>--%>
                                                        <%--<asp:TemplateField HeaderText="Created Date" ItemStyle-HorizontalAlign="Center" HeaderStyle-HorizontalAlign="Center">
                                                            <ItemTemplate>
                                                                <%# String.Format("{0:MM/dd/yyyy}", Eval("CreatedOn"))%>
                                                            </ItemTemplate>
                                                        </asp:TemplateField>--%>
                                                    </Columns>
                                                </asp:GridView>
                                                <uc1:Pager runat="server" ID="ucPagerBottom" PagingMode="Redirect" PageSize="20" PagingRecordText="Budgets" />
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


