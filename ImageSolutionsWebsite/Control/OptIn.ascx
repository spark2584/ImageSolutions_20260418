<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="OptIn.ascx.cs" Inherits="ImageSolutionsWebsite.Control.OptIn" %>
<!--modal popup start-->
<div class="modal fade bd-example-modal-md light-modal" id="OptIn" tabindex="-1" role="dialog"
    aria-hidden="true">
    <div class="modal-dialog modal-md modal-dialog-centered" role="document">
        <div class="modal-content">
            <div class="modal-body modal1">
                <div class="container-fluid p-0" style="background-color: white;">
                    <div class="row">
                        <div class="col-12">
                            <div class="modal-bg">
                                <asp:UpdatePanel ID="UpdatePanel1" runat="server">
                                    <ContentTemplate>
                                        <button type="button" class="btn-close" data-bs-dismiss="modal" aria-label="Close"><span aria-hidden="true">&times;</span></button>
                                        <div class="offer-content text-center">
                                            <br /><br />
                                            <div class="form-group mx-sm-3" id="divForm" runat="server">
                                                <h3>Want to stay up to date?</h3><br />
                                                <label>Sign up to get order confirmation, order tracking & budget updates!</label><br /><br />
                                                <asp:CustomValidator ID="cvdOptIn" runat="server" ValidationGroup="OptIn" ForeColor="Red" Display="Dynamic" OnServerValidate="cvdOptIn_ServerValidate"></asp:CustomValidator>
                                                
                                                <asp:PlaceHolder ID="phEnableEmailOptIn" runat="server">
                                                    <asp:RegularExpressionValidator ID="reqEmail" runat="server" ControlToValidate="txtNotificationEmail" ValidationExpression="^[\w-\.]+@([\w-]+\.)+[\w-]{2,4}$" ErrorMessage="Invalid email address" Display="Dynamic" ForeColor="Red" ValidationGroup="OptIn"></asp:RegularExpressionValidator>
                                                    <asp:TextBox ID="txtNotificationEmail" runat="server" CssClass="form-control" placeholder="Enter Email" /><br />
                                                </asp:PlaceHolder>

                                                <asp:PlaceHolder ID="phEnableSMSOptIn" runat="server">
                                                    <asp:RegularExpressionValidator ID="regMobileNumber" runat="server" ControlToValidate="txtMobileNumber" ValidationExpression="^[0-9]{10}$" ErrorMessage="Please enter a 10-digit phone number." ValidationGroup="OptIn" ForeColor="Red" />
                                                    <div class="input-group">
                                                        <span class="input-group-text">+1 (US)</span>
                                                        <asp:TextBox ID="txtMobileNumber" runat="server" MaxLength="10" CssClass="form-control" placeholder="Enter Phone Number"/>
                                                    </div><br />
                                                </asp:PlaceHolder>

                                                <asp:Button ID="btnOptIn" runat="server" Text="SIGN ME UP" OnClick="btnOptIn_Click" CssClass="btn btn-md btn-solid" ValidationGroup="OptIn" /><br /><br />
                                                <p><a href="#" data-bs-dismiss="modal">No Thanks</a></p>
                                            </div>
                                            <div class="form-group mx-sm-3" id="divThankyou" runat="server" visible="false">
                                                <label><asp:Literal ID="litThankyou" runat="server">You're all set! Thanks for opting in! We'll send your order confirmations, tracking updates and budget alerts by text/email. Reply STOP/UNSUBSCRIBE to opt out or HELP for help.</asp:Literal></label><br /><br />
                                                <p><a href="#" data-bs-dismiss="modal">Close</a></p>
                                            </div>
                                        </div>
                                    </ContentTemplate>
                                </asp:UpdatePanel>
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
            $('#OptIn').modal('show');
        }, 500);
    });
</script>
<!--modal popup end-->