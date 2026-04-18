<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="ImageModal.ascx.cs" Inherits="ImageSolutionsWebsite.Control.ImageModal" %>
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
                                <button type="button" class="btn-close" data-bs-dismiss="modal" aria-label="Close"><span aria-hidden="true">&times;</span></button>
                                <asp:Image ID="imgDialogImage" runat="server" Width="100%" />
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
            $('#imageModal').modal('show');
        }, 500);
    });
</script>
<!--modal popup end-->