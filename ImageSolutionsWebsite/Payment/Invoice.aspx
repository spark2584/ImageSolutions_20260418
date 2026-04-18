<%@ Page Language="C#" MasterPageFile="~/ImageSolutionsMasterPage.Master"  AutoEventWireup="true" CodeBehind="Invoice.aspx.cs" Inherits="ImageSolutionsWebsite.Payment.Invoice" %>

<asp:Content ID="Content3" ContentPlaceHolderID="cphBody" runat="server">
    <section class="cart-section section-b-space">
        <div class="container">
            <div class="row">
                <div class="col-sm-12 table-responsive-xs">
                    <asp:GridView runat="server" ID="gvInvoice" AutoGenerateColumns="False" CssClass="table cart-table" HeaderStyle-CssClass="table-head" HeaderStyle-HorizontalAlign="Center" Width="100%" GridLines="None" BorderWidth="0" AllowSorting="true" CellSpacing="0" CellPadding="0">
                        <Columns>
                            <asp:BoundField DataField="tranDate" DataFormatString="{0:d}" HeaderText="Invoice Date" HeaderStyle-HorizontalAlign="Center" ItemStyle-HorizontalAlign="Center" />
                            <asp:BoundField DataField="tranId" HeaderText="Invoice #" HeaderStyle-HorizontalAlign="Center" ItemStyle-HorizontalAlign="Center" />
                            <asp:BoundField DataField="entity.name" HeaderText="Name" HeaderStyle-HorizontalAlign="Center" ItemStyle-HorizontalAlign="Center" />
                            <%--<asp:BoundField DataField="amountRemaining" HeaderText="Amount" HeaderStyle-HorizontalAlign="Center" ItemStyle-HorizontalAlign="Center" />--%>
                            <asp:TemplateField HeaderText="Amount" HeaderStyle-HorizontalAlign="Center" ItemStyle-HorizontalAlign="Center">
                                <ItemTemplate>
                                    <%# string.Format("{0:c}", Eval("amountRemaining"))%>
                                </ItemTemplate>
                            </asp:TemplateField>
                            <asp:TemplateField HeaderText="Due Date" HeaderStyle-HorizontalAlign="Center" ItemStyle-HorizontalAlign="Center">
                                <ItemTemplate>
                                    <%# string.Format("{0}", Eval("dueDate").Equals(DateTime.MinValue) ? string.Empty : Eval("dueDate", "{0:MM/dd/yyyy}")) %>
                                </ItemTemplate>
                            </asp:TemplateField>
                            <asp:TemplateField HeaderText="Include" HeaderStyle-HorizontalAlign="Center" ItemStyle-HorizontalAlign="Center">
                                <ItemTemplate>
                                    <asp:HiddenField ID="hfInvoiceInternalID" runat="server" Value='<%# Eval("internalId") %>' />
                                    <asp:HiddenField ID="hfCustomerInteralID" runat="server" Value='<%# Eval("entity.internalid") %>' />
                                    <asp:HiddenField ID="hfTranDate" runat="server" Value='<%# Eval("tranDate") %>' />
                                    <asp:HiddenField ID="hfInvoiceNumber" runat="server" Value='<%# Eval("tranId") %>' />
                                    <asp:HiddenField ID="hfAmountRemaining" runat="server" Value='<%# Eval("amountRemaining") %>' />
                                    <asp:HiddenField ID="hfAmountTotal" runat="server" Value='<%# Eval("total") %>' />
                                    <asp:CheckBox ID="chkInclude" runat="server" OnCheckedChanged="chkInclude_CheckedChanged" Checked="true" AutoPostBack="true"/> 
                                </ItemTemplate>
                            </asp:TemplateField>
                        </Columns>
                    </asp:GridView>
                    <div class="table-responsive-md">
                        <table class="table cart-table">
                            <tfoot>
                                <tr>
                                    <td><asp:LIteral ID="litOrderTotal" runat="server" Text="Invoice Total :"></asp:LIteral></td>
                                    <td>
                                        <h2><asp:Label ID="lblTotal" runat="server" Text="$0"></asp:Label></h2>
                                    </td>
                                </tr>
                            </tfoot>
                        </table>
                    </div>
                    <div class="row cart-buttons">
                        <div class="col-12" style="text-align:center;"><asp:Button ID="btnRequestPayment" runat="server" Text="Proceed With Payment" CssClass="btn btn-solid" OnClick="btnRequestPayment_Click" /></div>
                        <%--<div class="col-6"><asp:HyperLink ID="lnkPaymentLink" runat="server" Text="Payment Link" Visible="false"></asp:HyperLink></div>--%>
                    </div>
                </div>
            </div>
        </div>
    </section>
 </asp:Content>