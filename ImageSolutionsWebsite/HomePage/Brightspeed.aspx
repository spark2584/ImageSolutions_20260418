<%@ Page Language="C#" MasterPageFile="~/MasterPage.Master" AutoEventWireup="true" CodeBehind="Brightspeed.aspx.cs" Inherits="ImageSolutionsWebsite.HomePage.Brightspeed" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <style>
        div#slides div{
           background-size: 100% !important;
        }
        div#slides2 div{
           background-size: 100% !important;
        }
        .item-desktop {
		    display: block;
	    }
        .item-mobile {
	        display: none;
		}
	    @media all and (max-width: 768px) {
		    .item-desktop {
			    display: none;
		    }

		    .item-mobile {
			    display: block;
		    }
	    }
    </style>
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="cphHeader" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="cphBody" runat="server">
    <asp:Literal ID="litBannerHTML" runat="server"></asp:Literal>
    <asp:Literal ID="litFeaturedProduct" runat="server"></asp:Literal>
</asp:Content>

