<%@ Page Language="C#" MasterPageFile="~/MasterPage.Master" AutoEventWireup="true" CodeBehind="ItemVariation.aspx.cs" Inherits="ImageSolutionsWebsite.Admin.ItemVariation" %>
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
                                                <h3>Item Variation Management</h3>
                                                <a href="/admin/item.aspx?id=<%= mItemID %>&tab=3" class="btn btn-sm btn-solid">Cancel</a>
                                            </div>
                                            <div class="table-responsive-xl">
                                                <asp:GridView runat="server" ID="gvItemVariation" AutoGenerateColumns="False" CssClass="table cart-table order-table" HeaderStyle-CssClass="table-head" GridLines="None" Width="100%" AllowSorting="true" CellSpacing="0" CellPadding="0" DataKeyNames="VariationID" 
                                                    OnRowCommand="gvItemVariation_RowCommand" OnRowDataBound="gvItemVariation_RowDataBound">
                                                    <Columns>
                                                        <asp:BoundField DataField="VariationID" HeaderText="Variation ID" HeaderStyle-HorizontalAlign="Left" ItemStyle-HorizontalAlign="Left" ItemStyle-Width="10%" />
                                                        <asp:TemplateField HeaderText="Attributes" HeaderStyle-HorizontalAlign="Center" ItemStyle-HorizontalAlign="Center" ItemStyle-Width="30%">
                                                            <ItemTemplate>
                                                                <asp:GridView ID="gvAttributeList" runat="server" AutoGenerateColumns="false" CssClass="table cart-table order-table" HeaderStyle-CssClass="table-head" GridLines="None" Width="100%">
                                                                    <Columns>
                                                                        <asp:BoundField HeaderStyle-HorizontalAlign="Left" ItemStyle-Width="50%" ItemStyle-HorizontalAlign="Left" DataField="Attribute.AttributeName" HeaderText="Attribute" />
                                                                        <%--<asp:BoundField ItemStyle-Width="40%" ItemStyle-HorizontalAlign="Center" DataField="AttributeValueName" HeaderText="Value" />--%>
                                                                        <asp:TemplateField HeaderText="Value" HeaderStyle-HorizontalAlign="Left" ItemStyle-HorizontalAlign="Left" ItemStyle-Width="40%">
                                                                            <ItemTemplate>
                                                                                <asp:Label ID="lblAttributeValue" Text='<%# Eval("Value") %>' runat="server" />
                                                                                <asp:HiddenField ID="hfAttributeValueID" Value='<%# Eval("AttributeValueID") %>' runat="server" />
                                                                            </ItemTemplate>
                                                                        </asp:TemplateField>
                                                                     </Columns>
                                                                </asp:GridView>
                                                            </ItemTemplate>
                                                        </asp:TemplateField>
                                                        <asp:TemplateField HeaderText="Create New Item" HeaderStyle-HorizontalAlign="Center" ItemStyle-HorizontalAlign="Center" ItemStyle-Width="10%">
                                                            <ItemTemplate>
                                                                <asp:Button ID="btnNewItem" runat="server" Text="Create" CommandName="CreateItem" CssClass="btn btn-sm btn-solid" CommandArgument='<%# Eval("VariationID") %>' CausesValidation="false" HeaderStyle-HorizontalAlign="Center"/>
                                                            </ItemTemplate>
                                                        </asp:TemplateField>
                                                        <asp:TemplateField HeaderText="Assign Item" HeaderStyle-HorizontalAlign="Center" ItemStyle-HorizontalAlign="Center" ItemStyle-Width="10%">
                                                            <ItemTemplate>
                                                                <asp:Button ID="btnAssign" runat="server" Text="Assign" CommandName="Assign" CssClass="btn btn-sm btn-solid" CommandArgument='<%# Eval("ItemID") %>' CausesValidation="false" HeaderStyle-HorizontalAlign="Center"/>
                                                                <asp:DropDownList ID="ddlAssignItem" runat="server" Visible ="false"></asp:DropDownList>
                                                                <asp:Button ID="btnAssignItem" runat="server" Text="Assign Item" CommandName="AssignItem" CssClass="btn btn-sm btn-solid" CommandArgument='<%# Eval("ItemID") %>' CausesValidation="false" HeaderStyle-HorizontalAlign="Center" Visible="false"/>
                                                            </ItemTemplate>
                                                        </asp:TemplateField>
                                                        <asp:TemplateField HeaderText="Item Number" HeaderStyle-HorizontalAlign="Left" ItemStyle-HorizontalAlign="Left" ItemStyle-Width="30%">
                                                            <ItemTemplate>
                                                                <asp:TextBox ID="txtItemNumber" runat="server" Width="100%"></asp:TextBox>
                                                            </ItemTemplate>
                                                        </asp:TemplateField>
                                                        <asp:TemplateField HeaderText="Base Price" HeaderStyle-HorizontalAlign="Center" ItemStyle-HorizontalAlign="Center" ItemStyle-Width="10%">
                                                            <ItemTemplate>
                                                                <asp:TextBox ID="txtBasePrice" runat="server" onKeyPress="if (event.keyCode!=46 && event.keyCode!=45 && (event.keyCode<48 || event.keyCode>57)) event.returnValue=false" Width="100%"></asp:TextBox>
                                                            </ItemTemplate>
                                                        </asp:TemplateField>
                                                        <asp:TemplateField HeaderText="Purchase Price" HeaderStyle-HorizontalAlign="Center" ItemStyle-HorizontalAlign="Center" ItemStyle-Width="10%">
                                                            <ItemTemplate>
                                                                <asp:TextBox ID="txtPurchasePrice" runat="server" onKeyPress="if (event.keyCode!=46 && event.keyCode!=45 && (event.keyCode<48 || event.keyCode>57)) event.returnValue=false" Width="100%"></asp:TextBox>
                                                            </ItemTemplate>
                                                        </asp:TemplateField>
                                                        <asp:TemplateField HeaderText="Update" HeaderStyle-HorizontalAlign="Center" ItemStyle-HorizontalAlign="Center" ItemStyle-Width="10%">
                                                            <ItemTemplate>
                                                                <asp:Button ID="btnUpdate" runat="server" Text="Update" CommandName="LineUpdate" CssClass="btn btn-sm btn-solid" CommandArgument='<%# Eval("ItemID") %>' CausesValidation="false" HeaderStyle-HorizontalAlign="Center"/>
                                                            </ItemTemplate>
                                                        </asp:TemplateField>
                                                        <asp:TemplateField HeaderText="Update" HeaderStyle-HorizontalAlign="Center" ItemStyle-HorizontalAlign="Center" ItemStyle-Width="10%">
                                                            <ItemTemplate>
                                                                <asp:Button ID="btnRemove" runat="server" Text="Remove" CommandName="Remove" CssClass="btn btn-sm btn-solid" CommandArgument='<%# Eval("ItemID") %>' CausesValidation="false" HeaderStyle-HorizontalAlign="Center"/>
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
    </section>
    <!--  dashboard section end -->
</asp:Content>
