<%@ Page Language="C#" MasterPageFile="~/MasterPage.Master" AutoEventWireup="true" CodeBehind="ReturnAuthorizationConfirmation.aspx.cs" Inherits="ImageSolutionsWebsite.ReturnAuthorizationConfirmation" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="cphHeader" runat="server">
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="cphBody" runat="server">
        <section class="section-b-space light-layout">
        <div class="container">
            <div class="row">
                <div class="col-md-12">
                    <div class="success-text">
                        <h2>Return</h2>

                        <p>Your Return Authorization Number is <h4><asp:Label ID="lblRMAID" runat="server"></asp:Label></h4></p>

                        <p>Please include your RMA number on your return label. Packages missing RMA numbers can not be processed.</p>

                        <asp:Panel ID="pnlLabelInstruction" runat ="server">
                            <p style="text-align:left;">Click <asp:HyperLink ID="hlnkShippingLabel" runat="server" Target="_blank" Text="here"></asp:HyperLink>for shipping label</></p>

                            <p style="text-align:left;">                         
1. Fold the printed page along the horizontal line
                            <br />
2. Affix label to your shipment so that the barcode is visible.
                            <br />
3. Write Return Authorization # on outside of package
                            </p>
                        </asp:Panel>




                        <asp:Panel ID="pnlUseMyOwn" runat="server" Visible="false">
                            <p style="text-align:left;">                         
1. Get a label from a carrier of your choice, we recommend using a carrier that will generate a tracking number.  Image Solutions will not be responsible for lost packages. Please ship back to<br />                          
Image Solutions - Returns<br />
4692 Brate Drive<br />
Suite 300<br />
Westchester, OH, 45011<br />
<br />
2. Affix label to your shipment so that the barcode is visible.
<br />
3. Write Return Authorization # on outside of package
                            </p>
                        </asp:Panel>

                    </div>
                </div>
            </div>
        </div>
    </section>
</asp:Content>

