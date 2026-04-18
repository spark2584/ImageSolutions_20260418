<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="Loading.ascx.cs" Inherits="ImageSolutionsWebsite.Control.Loading" %>
<!--modal popup start-->
<div class="modal fade bd-example-modal-lg theme-modal show" id="exampleModal" runat="server" tabindex="-1" role="dialog" 
    style="pointer-events: none; display: block;" aria-modal="true">
    <div class="modal-dialog modal-lg modal-dialog-centered" role="document">
        <asp:Image ID="imgLoading" runat="server" ImageUrl="~/assets/company/logo/DiscountTire/logo.png" />
        <%--<div class="modal-content" style="display:none">
            <div class="modal-body modal1">
                <div class="container-fluid p-0">
                    <div class="row">
                        <div class="col-12">
                            <div class="modal-bg">
                                
                            </div>
                        </div>
                    </div>
                </div>
            </div>
        </div>--%>
    </div>
</div>
<!--modal popup end-->