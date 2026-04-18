<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="WebsiteMessage.ascx.cs" Inherits="ImageSolutionsWebsite.Control.WebsiteMessage" %>
<!--modal popup start-->
<div class="modal fade bd-example-modal-lg theme-modal" id="exampleModal" tabindex="-1" role="dialog" style="pointer-events:none;"
    aria-hidden="true">
    <div class="modal-dialog modal-lg modal-dialog-centered" role="document">
        <div class="modal-content">
            <div class="modal-body modal1">
                <div class="container-fluid p-0">
                    <div class="row">
                        <div class="col-12">
                            <div class="modal-bg">
                                <button type="button" class="btn-close" data-bs-dismiss="modal" aria-label="Close"><span aria-hidden="true">&times;</span></button>
                              <%--  <asp:Label ID="lblMessage" runat="server"></asp:Label>--%>
<%--                                <asp:Literal ID="litMessage" runat="server"></asp:Literal>--%>
                                <asp:Repeater ID="rptAnnoucnement" runat="server" OnItemDataBound="rptAnnoucnement_ItemDataBound">
                                    <ItemTemplate>
                                    <asp:HiddenField ID="hfWebsiteMessageID" runat="server" Value='<%# Eval("WebsiteMessageID")%>'/>
                                        <div class="col-sm-12">
                                            <asp:Label ID="lblDate" runat="server"></asp:Label>
                                        </div>
                                        <div class="col-sm-12">
                                            <asp:Literal ID="litMessage" runat="server"></asp:Literal>
                                            <br />
                                            <br />
                                        </div>
                                    </ItemTemplate>
                                </asp:Repeater>
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