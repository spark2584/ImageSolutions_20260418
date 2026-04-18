<%@ Page Title="" Language="C#" MasterPageFile="~/MasterPage.Master" AutoEventWireup="true" CodeBehind="WebsiteTab.aspx.cs" Inherits="ImageSolutionsWebsite.WebsiteTab" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="cphHeader" runat="server">
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="cphBody" runat="server">
     <table width="100%">
      <tr>
         <td>Website Tab</td>
      </tr>
           <tr>
         <asp:GridView ID="gvWebsiteTab" runat="server" AutoGenerateColumns="False" DataKeyNames="WebsiteTabID" CssClass="GridStyle" Width="85%" 
            GridLines="Both" AllowSorting="true" CellSpacing="0" CellPadding="0" OnRowCommand="gvWebsiteTab_RowCommand"  >
             <Columns>
                 <asp:TemplateField HeaderText="TabName" HeaderStyle-HorizontalAlign="Center" ItemStyle-HorizontalAlign="Left">
                     <ItemTemplate>
                         <asp:Button id="btnWebsiteTab" runat="server" Text='<%# Eval("TabName")%>' CommandArgument='<%# Eval("WebsiteTabID") %>' CommandName="OpenWebsiteTab" />
                     </ItemTemplate>
                 </asp:TemplateField>
                   <asp:TemplateField HeaderText="Expand" HeaderStyle-HorizontalAlign="Center" ItemStyle-HorizontalAlign="Center">
                    <ItemTemplate>
                        <asp:Button id="btnExpand" runat="server" Text="+" CommandArgument='<%# Eval("WebsiteTabID") %>' CommandName="expandWebsiteTab" />
                    </ItemTemplate>
                </asp:TemplateField>
                     <asp:TemplateField HeaderText="Retract" HeaderStyle-HorizontalAlign="Center" ItemStyle-HorizontalAlign="Center">
                    <ItemTemplate>
                        <asp:Button id="btnRetract" runat="server" Text="-" CommandArgument='<%# Eval("WebsiteTabID") %>' CommandName="retractWebsiteTab" />
                    </ItemTemplate>
                </asp:TemplateField>
             </Columns>
         </asp:GridView>
      </tr>
    </table>
</asp:Content>
