<%@ Page Language="C#" MasterPageFile="~/MasterPage.Master" AutoEventWireup="true" CodeBehind="ReturnPolicy.aspx.cs" Inherits="ImageSolutionsWebsite.ReturnPolicy" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <style>
        ul{
            display: block;
        }

        ul li {
            display: list-item;
        }
    </style>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="cphHeader" runat="server">
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="cphBody" runat="server">
<section class="contact-page section-b-space">
    <div class="container">
        <div class="row section-b-space">
            <div class="col-lg-1">
            </div>
            <div class="col-lg-10">
                <h2 id="title">IMAGE SOLUTIONS 30 DAY RETURN POLICY</h2>
                <br />

                <p>We want to provide you with a great experience and will gladly accept returns on non-custom items you are not completely satisfied with. The refund amount would be for the cost of the item and does not include shipping. We will pay the return shipping costs if the return is a result of our error. There are a few important things to keep in mind when making a return:</p>
                <ul style="list-style-position: inside;">
                    <li style="list-style-position: inside;">Returns must be within 30 days of the receipt date</li>
                    <li style="list-style-position: inside;">Items that have been worn or washed will not be accepted for return.</li>
                    <li style="list-style-position: inside;">Custom items (made just for you), will not be accepted.</li>
                </ul>
                <br />

                <h3>STEPS TO RETURN AN ORDER</h3>
                <p>1. Receive a Return Authorization Number (RA #)</p>
                <ul style="list-style-position: inside; padding-left:50px;">
                    <li style="list-style-position: inside;">Contact Image Solutions Customer Service team at 888.756.9898</li>
                    <li style="list-style-position: inside;">Please be prepared with your sales order #.</li>
                    <li style="list-style-position: inside;">A customer service representative will issue you a return authorization number.</li>
                </ul>
                <br />
                <br />
                <p>2. Return the Items</p>
                <ul style="list-style-position: inside; padding-left:50px;">
                    <li style="list-style-position: inside;">You will be responsible for shipping the returned items to Image Solutions. We recommend using a carrier that will generate a tracking number. Image Solutions will not be responsible for lost packages.</li>
                    <li style="list-style-position: inside;">Reference your return authorization # on the outside of the package.</li>
                    <li style="list-style-position: inside;">Your return will be processed within 4 business days of receipt.</li>
                </ul>
                <br />
                <br />

                <p>If you have any other order correction needs, please contact Image Solutions Customer Service Team toll free at 888-756-9898 for assistance.</p>
                <p>Thank you!</p>
                <p>Image Solutions</p>
            </div>
            <div class="col-lg-1">
            </div>            
        </div>
    </div>
    
</section>
</asp:Content>