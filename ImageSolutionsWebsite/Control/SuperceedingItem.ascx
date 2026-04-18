<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="SuperceedingItem.ascx.cs" Inherits="ImageSolutionsWebsite.Control.SuperceedingItem" %>
<!--modal popup start-->
<div class="modal fade bd-example-modal-lg theme-modal" id="exampleModal" tabindex="-1" role="dialog"
    aria-hidden="true">
    <div class="modal-dialog modal-lg modal-dialog-centered" role="document">
        <div class="modal-content">
            <div class="modal-body modal1">
                <div class="container-fluid p-0">
                    <div class="row">
                        <div class="col-12">
                            <div class="modal-bg">
                                <button type="button" class="btn-close" data-bs-dismiss="modal" aria-label="Close"><span aria-hidden="true">&times;</span></button>
                                <p>The item you selected is currently out of stock, please choose from the following replacement items</p>
                                <asp:GridView ID="gvSuperceedingItems" runat="server" AutoGenerateColumns="False" DataKeyNames="SuperceedingItemID, ItemID, ReplacementItemID" CssClass="table cart-table" HeaderStyle-CssClass="table-head" Width="100%" 
                                    GridLines="None" AllowSorting="true" CellSpacing="0" CellPadding="0">
                                    <Columns>
                                    <asp:TemplateField HeaderText="Image" HeaderStyle-HorizontalAlign="Center" ItemStyle-HorizontalAlign="Left">
                                            <ItemTemplate>
                                                <a href="/ProductDetail.aspx?id=<%# Eval("ReplacementItem.ItemID")%>"><img src='<%# Eval("ReplacementItem.ImageURL") %>' alt="" style="max-width:40px;"></a>
                                            </ItemTemplate>
                                        </asp:TemplateField>
                                        <asp:TemplateField HeaderText="Product Name" HeaderStyle-HorizontalAlign="Center" ItemStyle-HorizontalAlign="Left">
                                            <ItemTemplate>
                                                <%# Eval("Item.ItemName")%><br /><%# Eval("ReplacementItem.ItemNumber")%>
                                            </ItemTemplate>
                                        </asp:TemplateField>
                                        <asp:TemplateField HeaderText="UnitPrice" ItemStyle-HorizontalAlign="Center" HeaderStyle-HorizontalAlign="Center">
                                            <ItemTemplate>
                                                <%# string.Format("{0:c}", Eval("ReplacementItem.BasePrice"))%>
                                            </ItemTemplate>
                                        </asp:TemplateField>
                                        <asp:TemplateField HeaderText="Quantity" ItemStyle-HorizontalAlign="Right" HeaderStyle-HorizontalAlign="Right">
                                            <ItemTemplate>
                                                <div class="qty-box">
                                                    <div class="input-group">
                                                        <asp:TextBox ID="txtQuantity" runat="server" type="number" CssClass="form-control input-number"></asp:TextBox>
                                                    </div>
                                                </div>
                                            </ItemTemplate>
                                        </asp:TemplateField>
                                    </Columns>
                                </asp:GridView>
                                <div class="row">
                                    <div class="col-12" style="text-align:right;"><p></p><asp:Button ID="btnClose" runat="server" Text="Close" OnClick="btnClose_Click" CausesValidation="false" CssClass="btn btn-sm btn-solid" />&nbsp;&nbsp;<asp:Button ID="btnAddToCart" runat="server" Text="Add To Cart" OnClick="btnAddToCart_Click" CssClass="btn btn-sm btn-solid" /></div>
                                </div>
                            </div>
                        </div>
                    </div>
                </div>
            </div>
        </div>
    </div>
</div>
<script>
    $(window).on('load', function () {
        setTimeout(function () {
            $('#exampleModal').modal('show');
        }, 500);
    });
</script>
<!--modal popup end-->