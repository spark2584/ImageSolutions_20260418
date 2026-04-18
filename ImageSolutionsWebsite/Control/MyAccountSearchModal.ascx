<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="MyAccountSearchModal.ascx.cs" Inherits="ImageSolutionsWebsite.Control.MyAccountSearchModal" %>
<!--modal popup start-->
<div class="modal fade" id="imageModal" tabindex="-1" role="dialog" style="pointer-events:none;"
    aria-hidden="true">
    <div class="modal-dialog modal-lg modal-dialog-centered" role="document">
        <div class="modal-content">
            <div class="modal-body modal1">
                <div class="container-fluid p-0">
                    <div class="row">
                        <div class="col-12">
                            <div class="modal-bg">
                                <div class="col-12">
                                    <div class="card dashboard-table mt-0">
                                        <div class="card-body table-responsive-sm">
                                            <div class="top-sec" id="filter" runat="server" visible="true">
                                                Account/Store Number:&nbsp;<asp:TextBox ID="txtAccountName" runat="server" placeholder="account name" AutoPostBack="true" OnTextChanged="txtAccountName_TextChanged"></asp:TextBox>&nbsp;
                                                <%--Email:--%>&nbsp;<asp:TextBox ID="txtEmail" runat="server" placeholder="email" AutoPostBack="true" OnTextChanged="txtEmail_TextChanged" Visible="false"></asp:TextBox>&nbsp;
                                            </div>
                                            <div class="table-responsive-xl">
                                                <asp:GridView runat="server" ID="gvAccounts" AutoGenerateColumns="False" CssClass="table cart-table order-table" HeaderStyle-CssClass="table-head" GridLines="None" Width="100%" AllowSorting="true" CellSpacing="0" CellPadding="0" OnRowCommand="gvAccounts_RowCommand">
                                                    <Columns>
                                                        <asp:BoundField DataField="AccountNamePath" HeaderText="Account Name" HeaderStyle-HorizontalAlign="Left" ItemStyle-HorizontalAlign="Left" />
                                                        <asp:TemplateField HeaderText="Select">
                                                            <ItemTemplate>
                                                                <asp:LinkButton ID="btnSelect" runat="server" CommandArgument='<%# Eval("AccountID") %>' CommandName="SelectLine"><i class="fa fa-edit me-2 font-success"></i></asp:LinkButton>
                                                            </ItemTemplate>
                                                        </asp:TemplateField>
                                                    </Columns>
                                                </asp:GridView>
                                            </div>
                                        </div>
                                    </div>
                                </div>
                                <button type="button" class="btn-close" data-bs-dismiss="modal" aria-label="Close"><span aria-hidden="true">&times;</span></button>                                
                            </div>
                        </div>
                    </div>
                </div>
            </div>
        </div>
    </div>
</div>
<asp:HiddenField ID="hfWebsiteID" runat="server" />
<asp:HiddenField ID="hfUserWebsiteID" runat="server" />
<script>
    $(window).on('load', function () {
        setTimeout(function () {
            $('#imageModal').modal('show');
        }, 500);
    });
</script>
<!--modal popup end-->