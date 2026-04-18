<%@ Page Title="" Language="C#" MasterPageFile="~/MasterPage.Master" AutoEventWireup="true" CodeBehind="UserWebSite.aspx.cs" Inherits="ImageSolutionsWebsite.UserWebSite" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <style>
        th {
            text-align: center;
        }        
    </style>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="cphHeader" runat="server">
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="cphBody" runat="server">

    <div class="container">
        <div class="row">
            <div class="col-s-12" style="overflow-x: scroll; overflow-y: hidden;">
                <asp:GridView ID="gvUserWebSites" runat="server" AutoGenerateColumns="False" DataKeyNames="UserWebSiteID" CssClass="table" HeaderStyle-CssClass="table-head" onrowupdating="gvUserWebSites_RowUpdating" Width="100%" 
                    GridLines="None" AllowSorting="true" CellSpacing="0" CellPadding="0" onsorting="gvUserWebSites_Sorting" OnRowDataBound="gvUserWebSites_RowDataBound">
                    <Columns>
                        <asp:TemplateField HeaderText="LOGO" HeaderStyle-CssClass="" HeaderStyle-HorizontalAlign="Center" ItemStyle-CssClass="" ItemStyle-HorizontalAlign="Center" ItemStyle-VerticalAlign="Middle">
                            <ItemTemplate>
                                <asp:LinkButton ID="lkImage" runat="server" CommandArgument='<%# Eval("UserWebSiteID") %>' CommandName="Update"><img src="<%# Eval("WebSite.LogoPath") %>" style="max-width:250px;" height="100" alt=""></asp:LinkButton>
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="WEBSITE" HeaderStyle-CssClass="" HeaderStyle-HorizontalAlign="Center" ItemStyle-CssClass="" ItemStyle-HorizontalAlign="Center" ItemStyle-VerticalAlign="Middle">
                            <ItemTemplate>
                                <asp:LinkButton ID="lkWebsiteName" runat="server" CommandArgument='<%# Eval("UserWebSiteID") %>' CommandName="Update"><h4><%# Eval("WebSite.Name") %></h4></asp:LinkButton>                                
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="DOMAIN" HeaderStyle-CssClass="table-head" ItemStyle-CssClass="" ItemStyle-HorizontalAlign="Center" HeaderStyle-HorizontalAlign="Center" ItemStyle-VerticalAlign="Middle">
                            <ItemTemplate>
                                <asp:LinkButton ID="lkBtnBusiness" runat="server" CommandArgument='<%# Eval("UserWebSiteID") %>' CommandName="Update"><%# Eval("WebSite.Domain") %></asp:LinkButton>
                            </ItemTemplate>
                        </asp:TemplateField>
                    </Columns>
                </asp:GridView>
            </div>
        </div>
    </div>

<%--     <section class="cart-section section-b-space">
        <div class="container">
            <div class="row">
                <div class="col-sm-12 table-responsive-xs">
                    <asp:GridView ID="gvUserWebSites" runat="server" AutoGenerateColumns="False" DataKeyNames="UserWebSiteID" CssClass="table cart-table" HeaderStyle-CssClass="table-head" onrowupdating="gvUserWebSites_RowUpdating" Width="100%" 
                        GridLines="None" AllowSorting="true" CellSpacing="0" CellPadding="0" onsorting="gvUserWebSites_Sorting" OnRowDataBound="gvUserWebSites_RowDataBound">
                        <rowstyle Height="20px" />
                        <alternatingrowstyle  Height="20px"/>
                        <Columns>
                            <asp:TemplateField HeaderText="LOGO" HeaderStyle-CssClass="" ItemStyle-CssClass="">
                                <ItemTemplate>
                                    <asp:LinkButton ID="lkImage" runat="server" CommandArgument='<%# Eval("UserWebSiteID") %>' CommandName="Update"><img src="<%# Eval("WebSite.LogoPath") %>" alt=""></asp:LinkButton>
                                </ItemTemplate>
                            </asp:TemplateField>
                            <asp:TemplateField HeaderText="WEBSITE" HeaderStyle-CssClass="" ItemStyle-CssClass="">
                                <ItemTemplate>
                                    <h2><%# Eval("WebSite.Name") %></h2>
                                </ItemTemplate>
                            </asp:TemplateField>
                            <asp:TemplateField HeaderText="DOMAIN" HeaderStyle-CssClass="table-head" ItemStyle-CssClass="" ItemStyle-HorizontalAlign="Center" HeaderStyle-HorizontalAlign="Center">
                                <ItemTemplate>
                                    <asp:LinkButton ID="lkBtnBusiness" runat="server" CommandArgument='<%# Eval("UserWebSiteID") %>' CommandName="Update"><%# Eval("WebSite.Domain") %></asp:LinkButton>
                                </ItemTemplate>
                            </asp:TemplateField>
                        </Columns>
                    </asp:GridView>
                </div>
            </div>
        </div>
    </section>--%>
</asp:Content>