<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="Pager.ascx.cs" Inherits="ImageSolutionsWebsite.Control.Pager" %>

<div class="product-pagination">
    <div class="theme-paggination-block">
        <div class="row">
            <div class="col-xl-6 col-md-6 col-sm-12">
                <nav aria-label="Page navigation">
                    <ul class="pagination">
                        <asp:Repeater ID="rptRedirect" runat="server">
                            <HeaderTemplate>
                                <li class="page-item"><a id="A1" class="page-link" href='<%# RedirectUrl(1) %>' runat="server" Visible='<%# CurrentPageNumber > 1 %>'>FIRST</a> </li>
                                <li class="page-item"><a id="A2" class="page-link" href='<%# RedirectUrl(CurrentPageNumber - 1) %>' runat="server" Visible='<%# CurrentPageNumber > 1 %>'>PREVIOUS</a> </li>
                                <li class="page-item"><a id="A3" class="page-link" href='<%# RedirectUrl(StartPageNumber - 1) %>' runat="server" Visible='<%# StartPageNumber > 1 %>'>...</a></li>
                            </HeaderTemplate>
                            <ItemTemplate>
                                <li class="page-item"><a id="A4" runat="server" class="page-link" href='<%# RedirectUrl(Container.DataItem) %>' Visible='<%# Container.DataItem.ToString() != CurrentPageNumber.ToString() %>'><%# Container.DataItem %></a></li>
                                <li class="page-item active"><a id="A5" runat="server" class="page-link" Visible='<%# Container.DataItem.ToString() == CurrentPageNumber.ToString() %>'><asp:Literal ID="Literal2" runat="server" Text='<%#Container.DataItem %>'></asp:Literal></a></li>
                            </ItemTemplate>
                            <FooterTemplate>
                                <li class="page-item"><a id="A5" class="page-link" href='<%# RedirectUrl(EndPageNumber + 1) %>' runat="server" Visible='<%# EndPageNumber < TotalPages %>'>...</a></li>
                                <li class="page-item"><a id="A6" class="page-link" href='<%# RedirectUrl(CurrentPageNumber + 1) %>' runat="server" Visible='<%# CurrentPageNumber < TotalPages %>'>NEXT</a> </li>
                                <li class="page-item"><a id="A7" class="page-link" href='<%# RedirectUrl(TotalPages) %>' runat="server" Visible='<%# CurrentPageNumber < TotalPages %>'>LAST</a> </li>
                            </FooterTemplate>
                        </asp:Repeater>
                    </ul>
                </nav>
            </div>
            <div class="col-xl-6 col-md-6 col-sm-12">
                <div class="product-search-count-bottom">
                    <h5><asp:Label ID="lblPagingRecordText" runat="server"></asp:Label>&nbsp;<asp:LinkButton ID="lbnExport" runat="server" OnClick="lbnExport_Click" Visible="false">Export</asp:LinkButton></h5>
                </div>
            </div>
        </div>
    </div>
</div>

<table class="marginBottom20px" width="100%">
    <tr>
        <td></td>
        <td class="pageNumber" align="right">
            <asp:Repeater ID="rptPostback" runat="server" onitemcommand="rptPostback_ItemCommand">
                <HeaderTemplate>
                    PAGE:&nbsp;
                    <asp:LinkButton ID="lbtn1" CommandArgument='<%# 1 %>' runat="server" CausesValidation="false" Visible='<%# CurrentPageNumber > 1 %>'>FIRST | </asp:LinkButton> 
                    <asp:LinkButton ID="lbtn2" CommandArgument='<%# CurrentPageNumber - 1 %>' runat="server" CausesValidation="false" Visible='<%# CurrentPageNumber > 1 %>'>PREVIOUS&nbsp;&nbsp;&nbsp;</asp:LinkButton> 
                    <asp:LinkButton ID="lbtn3" CommandArgument='<%# StartPageNumber - 1 %>' runat="server" CausesValidation="false" Visible='<%# StartPageNumber > 1 %>'>...</asp:LinkButton> 
                    <asp:Literal ID="Literal3" runat="server" Text=" | " Visible='<%# StartPageNumber > 1 %>'></asp:Literal>
                </HeaderTemplate>
                <ItemTemplate><asp:LinkButton ID="lbtn4" CommandArgument='<%# Container.DataItem %>' runat="server" CausesValidation="false" Visible='<%# Container.DataItem.ToString() != CurrentPageNumber.ToString() %>'><%#Container.DataItem %></asp:LinkButton><asp:Literal ID="Literal4" runat="server" Text='<%#Container.DataItem %>' Visible='<%# Container.DataItem.ToString() == CurrentPageNumber.ToString() %>'></asp:Literal> </ItemTemplate>
                <FooterTemplate>
                    <asp:Literal ID="Literal5" runat="server" Text=" | " Visible='<%# EndPageNumber < TotalPages %>'></asp:Literal>
                    <asp:LinkButton ID="lbtn5" CommandArgument='<%# EndPageNumber + 1 %>' runat="server" CausesValidation="false" Visible='<%# EndPageNumber < TotalPages %>'>...</asp:LinkButton> 
                    <asp:LinkButton ID="lbtn6" CommandArgument='<%# CurrentPageNumber + 1 %>' runat="server" CausesValidation="false" Visible='<%# CurrentPageNumber < TotalPages %>'>&nbsp;&nbsp;&nbsp;NEXT</asp:LinkButton> 
                    <asp:LinkButton ID="lbtn7" CommandArgument='<%# TotalPages %>' runat="server" CausesValidation="false" Visible='<%# CurrentPageNumber < TotalPages %>'> | LAST</asp:LinkButton> 
                </FooterTemplate>
            </asp:Repeater>
        </td>
    </tr>
</table>