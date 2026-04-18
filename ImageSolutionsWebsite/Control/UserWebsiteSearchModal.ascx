<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="UserWebsiteSearchModal.ascx.cs" Inherits="ImageSolutionsWebsite.Control.UserWebsiteSearchModal" %>
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
                                                First Name:&nbsp;<asp:TextBox ID="txtFirstName" runat="server" placeholder="first name" required="" AutoPostBack="true" OnTextChanged="txtFirstName_TextChanged"></asp:TextBox>&nbsp;
                                                Last Name:&nbsp;<asp:TextBox ID="txtLastName" runat="server" placeholder="last name" required="" AutoPostBack="true" OnTextChanged="txtLastName_TextChanged"></asp:TextBox>&nbsp;
                                                <asp:Label ID="lblEmail" runat="server" Text="Email:"></asp:Label>&nbsp;<asp:TextBox ID="txtEmail" runat="server" placeholder="email" required="" AutoPostBack="true" OnTextChanged="txtEmail_TextChanged"></asp:TextBox>&nbsp;
                                            </div>
                                            <div class="table-responsive-xl">
                                                <asp:GridView runat="server" ID="gvUserWebsites" AutoGenerateColumns="False" CssClass="table cart-table order-table" HeaderStyle-CssClass="table-head" GridLines="None" Width="100%" AllowSorting="true" CellSpacing="0" CellPadding="0" OnRowCommand="gvUserWebsites_RowCommand"
                                                    OnRowDataBound="gvUserWebsites_RowDataBound">
                                                    <Columns>
                                                        <asp:BoundField DataField="UserInfo.FirstName" HeaderText="First Name" HeaderStyle-HorizontalAlign="Left" ItemStyle-HorizontalAlign="Left" />
                                                        <asp:BoundField DataField="UserInfo.LastName" HeaderText="LastName" HeaderStyle-HorizontalAlign="Left" ItemStyle-HorizontalAlign="Left" />
                                                        <asp:BoundField DataField="UserInfo.EmailAddress" HeaderText="Email" HeaderStyle-HorizontalAlign="Left" ItemStyle-HorizontalAlign="Left" />
                                                        <asp:BoundField DataField="UserInfo.UserName" HeaderText="UserName" HeaderStyle-HorizontalAlign="Left" ItemStyle-HorizontalAlign="Left" />
                                                        <asp:TemplateField HeaderText="Select">
                                                            <ItemTemplate>
                                                                <asp:LinkButton ID="btnSelect" runat="server" CommandArgument='<%# Eval("UserWebsiteID") %>' CommandName="SelectLine"><i class="fa fa-edit me-2 font-success"></i></asp:LinkButton>
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