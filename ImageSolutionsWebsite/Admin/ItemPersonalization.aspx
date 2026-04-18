<%@ Page Language="C#" MasterPageFile="~/MasterPage.Master" AutoEventWireup="true" CodeBehind="ItemPersonalization.aspx.cs" Inherits="ImageSolutionsWebsite.Admin.ItemPersonalization" %>
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
                                                <h3>Item Personalization</h3> 
                                            </div>
                                            <div class="table-responsive-xl">
                                                <div class="theme-form">
                                                    <div class="form-row row">
                                                        <div class="col-md-12">
                                                            <label>Name</label>
                                                            <asp:TextBox ID="txtName" runat="server" CssClass="form-control" placeholder="name" required=""></asp:TextBox>
                                                            <asp:RequiredFieldValidator ID="valName" runat="server" ControlToValidate="txtName" ValidationGroup="Submit" ErrorMessage="*" ForeColor="Red"></asp:RequiredFieldValidator>
                                                        </div>
                                                        <div class="col-md-12">
                                                            <label>Type</label>
                                                                <asp:DropDownList ID="ddlType" runat="server" CssClass="form-control" placeholder="type" required="" AutoPostBack="true" OnSelectedIndexChanged="ddlType_SelectedIndexChanged">
                                                                    <asp:ListItem Selected="True" Value="text">Text</asp:ListItem>
                                                                    <asp:ListItem Value="dropdown">Dropdown</asp:ListItem>
                                                            </asp:DropDownList>
                                                            <br />
                                                        </div>
                                                        <asp:PlaceHolder ID="pnlDefaultValue" runat="server" Visible="false">
                                                            <div class="col-md-12">
                                                                <label>Default Value</label>
                                                                    <asp:DropDownList ID="ddlDefaultValue" runat="server" CssClass="form-control" placeholder="type" AutoPostBack="true">
                                                                        <asp:ListItem Selected="True" Value=""></asp:ListItem>
                                                                        <asp:ListItem Value="First Name">First Name</asp:ListItem>
                                                                </asp:DropDownList>
                                                                <br />
                                                            </div>
                                                        </asp:PlaceHolder>

                                                        <asp:PlaceHolder ID="pnlAllowBlank" runat="server" Visible="false">
                                                            <div class="col-md-12">
                                                                <label>Allow Blank</label>
                                                                <asp:CheckBox ID="chkAllowBlank" runat="server" />
                                                                <br />
                                                            </div>
                                                        </asp:PlaceHolder>

                                                        <asp:PlaceHolder ID="phDropdownValue" runat="server" Visible="false">
                                                            <div class="col-md-12">
                                                            <div class="top-sec">
                                                                <h3>Drop-down Values</h3>
                                                                <a href="/Admin/ItemPersonalizationValueList.aspx?itemid=<%= mItemID %>&itempersonalizationid=<%= mItemPersonalizationID %>" class="btn btn-sm btn-solid">+ add new</a>
                                                            </div>
                                                            <div class="table-responsive-xl"
                                                                <asp:GridView ID="gvItemPersonalizationValueList" runat="server" AutoGenerateColumns="False" CssClass="table cart-table order-table" HeaderStyle-CssClass="table-head" GridLines="None" Width="100%" AllowSorting="true" CellSpacing="0" 
                                                                    DataKeyNames="ItemPersonalizationValueListID" CellPadding="0" OnRowCommand="gvItemPersonalizationValueList_RowCommand" OnRowDataBound="gvItemPersonalizationValueList_RowDataBound">
                                                                    <Columns>
                                                                        <asp:BoundField DataField="Value" HeaderText="Value" HeaderStyle-HorizontalAlign="Left" ItemStyle-HorizontalAlign="Left" />
                                                                        <asp:TemplateField HeaderText="VIEW">
                                                                            <ItemTemplate>
                                                                                <a href='/admin/itempersonalizationvaluelist.aspx?itemid=<%= mItemID %>&itempersonalizationid=<%= mItemPersonalizationID %>&id=<%# Eval("ItemPersonalizationValueListID") %>'>
                                                                                    <i class="fa fa-eye text-theme"></i>
                                                                                </a>
                                                                            </ItemTemplate>
                                                                        </asp:TemplateField>
                                                                    </Columns>
                                                                </asp:GridView>
                                                            </div>
                                                        </div>
                                                        </asp:PlaceHolder>

                                                        <asp:PlaceHolder ID="phRequireVerification" runat="server">
                                                            <div class="col-md-12">
                                                                <label>Require Verification</label>
                                                                <asp:CheckBox ID="chkRequireVerification" runat="server" />
                                                                <br />
                                                            </div>
                                                        </asp:PlaceHolder>
                                                        
                                                        <div class="col-md-12">
                                                            <label>Require Approval</label>
                                                            <asp:CheckBox ID="chkRequireApproval" runat="server" />
                                                            <br />
                                                        </div>

                                                        <div class="col-md-12">
                                                            <label>Inactive</label>
                                                            <asp:CheckBox ID="chkInActive" runat="server" />
                                                            <br />
                                                        </div>
                                                        <div class="col-md-12">
                                                            <asp:Button ID="btnSave" runat="server" Text="Save" OnClick="btnSave_Click" CssClass="btn btn-sm btn-solid" ValidationGroup="Submit" />
                                                            <a id="aCancel" runat="server" href="/Admin/Item.aspx?id=<%= mItemID %>" class="btn btn-sm btn-solid">Cancel</a>
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
