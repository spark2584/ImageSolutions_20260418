<%@ Page Title="" Language="C#" MasterPageFile="~/MasterPage.Master" AutoEventWireup="true" CodeBehind="Order.aspx.cs" Inherits="ImageSolutionsWebsite.Admin.Order" %>
<%@ Register Src="~/Control/AdminNavigation.ascx" TagPrefix="uc" TagName="AdminNavigation" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
        <script type="text/javascript">    
           function ConfirmOnReject() {
            if (confirm("Are you sure you want to reject this order?") == true)
                return true;
            else
                return false;
           }
        </script>

    <style type="text/css">
        .space input[type="checkbox"] {
          margin-right: 8px; /* Control space manually */
        }

    </style>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="cphHeader" runat="server">
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="cphBody" runat="server">
    <!--  dashboard section start -->
    <section class="dashboard-section section-b-space user-dashboard-section">
        <div class="container">
            <div class="row">
                <uc:AdminNavigation runat="server" id="AdminNavigation" />
                <div class="col-lg-9">
                    <div class="faq-content tab-content" id="top-tabContent">
                        <div class="tab-pane fade show active" id="websites">
                            <div class="row">
                                <div class="col-sm-12 col-lg-12">
                                    <section class="tab-product m-0">
                                        <ul class="nav nav-tabs nav-material" id="top-tab" role="tablist">
                                            <li class="nav-item">
                                                <a class="nav-link active" id="top_1_tab" runat="server" data-bs-toggle="tab"
                                                    href="#top_1" role="tab" aria-selected="true">
                                                    <i class="icofont icofont-ui-home"></i>Order Detail
                                                </a>
                                                <div class="material-border"></div>
                                            </li>
                                        </ul>
                                    </section><br />
                                    <div class="tab-content nav-material">
                                        <div class="tab-pane fade show active" id="top_1" role="tabpanel" aria-labelledby="top_1_tab">
                                            <div class="card dashboard-table mt-0">
                                                <div class="card-body table-responsive-sm">
                                                    <%--<div class="top-sec">
                                                        <h3>Shipping Service</h3> 
                                                    </div>--%>
                                                    <div class="table-responsive-xl">
                                                        <div class="theme-form">
                                                            <div class="form-row row">
                                                                
                                                                <asp:Panel ID="pnlEditPersonalization" runat="server" BorderStyle="Solid" style="padding: 20px; margin:10px" Visible="false">
                                                                    <div class="col-md-12">
                                                                        <asp:Label ID="lblSelectedItem" runat="server" Enabled="false"></asp:Label>                                                                        
                                                                        <asp:HiddenField ID="hfSalesOrderLineID" runat="server" /> 
                                                                        <br />
                                                                    </div>
                                                                    <asp:Repeater ID="rptItemPersonalization" runat="server" DataMember="ItemPersonalizationID">
                                                                        <ItemTemplate>
                                                                            <div class="row" style="margin:10px;">
                                                                                <asp:Label ID="lblLabel" runat="server" Text='<%# Eval("ItemPersonalization.Name")%>' ForeColor="Black"></asp:Label>
                                                                                <asp:TextBox ID="txtValue" runat="server" Text='<%# Eval("Value")%>' CssClass="form-control"></asp:TextBox>
                                                                                <asp:HiddenField ID="hfItemPersonalizationValueID" runat="server" Value='<%# Eval("ItemPersonalizationValueID")%>'/>                                                                                                                                                                                        <br />
                                                                            </div>
                                                                        </ItemTemplate>
                                                                    </asp:Repeater>
                                                                    <div class="col-md-12">
                                                                        <asp:Button ID="btnUpdatePersonalization" runat="server" Text="Save" OnClick="btnUpdatePersonalization_Click" CssClass="btn btn-sm btn-solid" ValidationGroup="Submit" />
                                                                        <asp:Button ID="btnCancelPersonalization" runat="server" Text="Cancel" OnClick="btnCancelPersonalization_Click" CssClass="btn btn-sm btn-solid" ValidationGroup="Submit" />
                                                                    </div>
                                                                </asp:Panel>
                                                                <br />
                                                                <div class="col-md-12">
                                                                    <asp:GridView ID="gvSalesOrderLine" runat="server" AutoGenerateColumns="False" DataKeyNames="SalesOrderLineID" CssClass="table cart-table" HeaderStyle-CssClass="table-head" Width="100%" 
                                                                        GridLines="None" AllowSorting="true" CellSpacing="0" CellPadding="0" OnRowCommand="gvSalesOrderLine_RowCommand" OnRowDataBound="gvSalesOrderLine_RowDataBound">
                                                                        <Columns>

                                       <%--                                     <asp:TemplateField HeaderText="Item" ItemStyle-HorizontalAlign="Center" HeaderStyle-HorizontalAlign="Center">
                                                                                <ItemTemplate>
                                                                                    <%# Eval("Item.ItemNumber")%> <%# Eval("UserInfo") == null ? string.Empty : "(" + Eval("UserInfo.FullName") + ")" %>
                                                                                </ItemTemplate>
                                                                            </asp:TemplateField>
                                                                            <asp:TemplateField HeaderText="Description" ItemStyle-HorizontalAlign="Center" HeaderStyle-HorizontalAlign="Center">
                                                                                <ItemTemplate>
                                                                                    <%# Eval("Item.SalesDescription")%>
                                                                                </ItemTemplate>
                                                                            </asp:TemplateField>--%>


                                                                            <asp:TemplateField HeaderText="Product Name" HeaderStyle-HorizontalAlign="Center" ItemStyle-HorizontalAlign="Left">
                                                                                <ItemTemplate>
                                                                                    <%# Eval("Item.ItemNumber")%><br /><%# Eval("Item.SalesDescription")%><br />
                                                                                    <%# (!string.IsNullOrEmpty(Convert.ToString(Eval("LogosDescription"))) ? "<font style='color:blue;'> " + Eval("LogosDescription") + " </font><br/>" : String.Empty) %>                                                                                    
                                                                                </ItemTemplate>
                                                                            </asp:TemplateField>


                                                                            <asp:TemplateField HeaderText="Personalization" ItemStyle-HorizontalAlign="Center" HeaderStyle-HorizontalAlign="Center">
                                                                                <ItemTemplate>
                                                                                    <%# string.IsNullOrEmpty(Convert.ToString(Eval("UserInfo"))) ? string.Empty : "Employee: " + Eval("UserInfo.FullName") + "<br />" %>
                                                                                    <%# Eval("PersonalizationListHTML")%>
                                                                                    <asp:LinkButton ID="btnEditPersonalization" runat="server" CommandName="EditPersonalization" CommandArgument='<%# Eval("SalesOrderLineID") %>'><i class="ti-pencil" style="font-size:medium"></i></asp:LinkButton>
                                                                                </ItemTemplate>
                                                                            </asp:TemplateField>
                                                                            <asp:TemplateField HeaderText="Options" ItemStyle-HorizontalAlign="Left" HeaderStyle-HorizontalAlign="Left" Visible="false">
                                                                                <ItemTemplate>
                                                                                    <%# string.IsNullOrEmpty(Convert.ToString(Eval("UserInfo"))) ? string.Empty : "User: " + Eval("UserInfo.FullName") + "<br />" %>
                                                                                    <%# string.IsNullOrEmpty(Convert.ToString(Eval("CustomListValueID_1"))) ? string.Empty : Eval("CustomListValue_1.CustomList.ListName") + ": " + Eval("CustomListValue_1.ListValue") + "<br />" %>
                                                                                    <%# string.IsNullOrEmpty(Convert.ToString(Eval("CustomListValueID_2"))) ? string.Empty : Eval("CustomListValue_2.CustomList.ListName") + ": " + Eval("CustomListValue_2.ListValue") + "<br />" %>
                                                                                </ItemTemplate>
                                                                            </asp:TemplateField>
                                                                            <asp:TemplateField HeaderText="Price" ItemStyle-HorizontalAlign="Center" HeaderStyle-HorizontalAlign="Center">
                                                                                <ItemTemplate>
                                                                                    <%# Convert.ToBoolean(Eval("SalesOrder.DisplayTariffCharge")) ? string.Format("{0:c}", Eval("OnlinePrice")) : string.Format("{0:c}", Eval("UnitPrice"))%>
                                                                                </ItemTemplate>
                                                                            </asp:TemplateField>
                                                                            <asp:TemplateField HeaderText="Tariff Surcharge" Visible="false" ItemStyle-HorizontalAlign="Center" HeaderStyle-HorizontalAlign="Center">
                                                                                <ItemTemplate>
                                                                                    <%# string.Format("{0:c}", Eval("TariffCharge"))%>
                                                                                </ItemTemplate>
                                                                            </asp:TemplateField>
                                                                            <asp:TemplateField HeaderText="Quantity" ItemStyle-HorizontalAlign="Center" HeaderStyle-HorizontalAlign="Center">
                                                                                <ItemTemplate>
                                                                                    <%# Eval("Quantity")%>
                                                                                </ItemTemplate>
                                                                            </asp:TemplateField>
                                                                        </Columns>
                                                                    </asp:GridView>
                                                                 </div>
                                                                
                                                                <asp:PlaceHolder ID="phPackage" runat="server">
                                                                    <div class="col-md-12">
                                                                        <label>Package</label>
                                                                        <asp:TextBox ID="txtPackage" runat="server" CssClass="form-control" Enabled="false"></asp:TextBox>
                                                                        <br />
                                                                    </div>
                                                                    <div class="col-md-12">
                                                                        <%--<label>Exclude Optional</label>--%>
                                                                        <asp:CheckBox ID="chkExcludeOptional" runat="server" CssClass="space" Enabled="false"/>
                                                                        <br />
                                                                        <br />
                                                                    </div>
                                                                </asp:PlaceHolder>

                                                                <div class="col-md-12">
                                                                    <label>User</label>
                                                                    <asp:TextBox ID="txtUser" runat="server" CssClass="form-control" Enabled="false"></asp:TextBox>
                                                                    <br />
                                                                </div>
                                                                <div class="col-md-12">
                                                                    <label>Order Number</label>
                                                                    <asp:TextBox ID="txtOrderNumber" runat="server" CssClass="form-control" Enabled="false"></asp:TextBox>
                                                                    <br />
                                                                </div>

                                                                <asp:Panel ID="pnlPONumber" runat="server">
                                                                    <div class="col-md-12">
                                                                        <label>PO Number</label>
                                                                        <asp:TextBox ID="txtPONumber" runat="server" CssClass="form-control" Enabled="false"></asp:TextBox>
                                                                        <br />
                                                                    </div>
                                                                </asp:Panel>

                                                                <div class="col-md-12">
                                                                    <label>Order Date</label>
                                                                    <asp:TextBox ID="txtOrderDate" runat="server" CssClass="form-control" Enabled="false"></asp:TextBox>
                                                                    <br />
                                                                </div>
                                                                <div class="col-md-12">
                                                                    <label>Subtotal</label>
                                                                    <asp:TextBox ID="txtSubtotal" runat="server" CssClass="form-control" Enabled="false"></asp:TextBox>
                                                                    <br />
                                                                </div>
                                                                <div class="col-md-12">
                                                                    <label>Shipping</label>
                                                                    <asp:TextBox ID="txtShipping" runat="server" CssClass="form-control" Enabled="false"></asp:TextBox>
                                                                    <br />
                                                                </div>
                                                                <div class="col-md-12">
                                                                    <label>Tax</label>
                                                                    <asp:TextBox ID="txtTax" runat="server" CssClass="form-control" Enabled="false"></asp:TextBox>
                                                                    <br />
                                                                </div>
                                                                <div class="col-md-12">
                                                                    <label>Total</label>
                                                                    <asp:TextBox ID="txtTotal" runat="server" CssClass="form-control" Enabled="false"></asp:TextBox>
                                                                    <br />
                                                                </div>

                                                                <div class="col-md-12">
                                                                    <label>Payment Method</label>
                                                                    <asp:TextBox ID="txtPayment" runat="server" CssClass="form-control" Enabled="false"></asp:TextBox>
                                                                    <br />
                                                                </div>
                                                                
                                                                <asp:Panel ID="pnlBudgetUsed" runat="server">
                                                                    <div class="col-md-12">
                                                                        <label>Credit Used:</label>
                                                                        <asp:TextBox ID="txtBudgetUsed" runat="server" CssClass="form-control" Enabled="false"></asp:TextBox>
                                                                        <br />
                                                                    </div>
                                                                </asp:Panel>
                                                                
                                                                <div class="col-md-12">
                                                                    <label>Shipping Method</label>
                                                                    <asp:TextBox ID="txtShippingMethod" runat="server" CssClass="form-control" Enabled="false"></asp:TextBox>
                                                                    <br />
                                                                </div>

                                                                <div class="col-md-12">
                                                                    <label>Shipping Address</label>
                                                                    <asp:TextBox ID="txtShippingAddress" runat="server" CssClass="form-control" Enabled="false" TextMode="MultiLine" Height="150px"></asp:TextBox>
                                                                    <br />
                                                                </div>

                                                                <asp:Repeater ID="rptCustomField" runat="server">
                                                                    <ItemTemplate>
                                                                        <div class="col-md-12">
                                                                            <label><asp:Label ID="lblLabel" runat="server" Text='<%# Eval("CustomField.Name")%>'></asp:Label></label>
                                                                            <asp:TextBox ID="txtCustomValue" runat="server" CssClass="form-control" Text='<%# Eval("Value")%>' Enabled="false"></asp:TextBox>
                                                                            <br />
                                                                        </div>
                                                                    </ItemTemplate>
                                                                </asp:Repeater>


                                                                <div class="col-md-12">
                                                                    <label>Is Pending Personalization Approval</label>
                                                                    <asp:CheckBox ID="chkIsPendingPersonalziationApproval" runat="server" Enabled="false"/>
                                                                    <br />
                                                                </div>
                                                                <div class="col-md-12">
                                                                    <label>Is Pending Order Approval</label>
                                                                    <asp:CheckBox ID="chkIsPendingApproval" runat="server" Enabled="false"/>
                                                                    <br />
                                                                </div>

                                                                <div class="col-md-12">
                                                                    <asp:Button ID="btnSave" runat="server" Text="Save" OnClick="btnSave_Click" CssClass="btn btn-sm btn-solid" ValidationGroup="Submit" />
                                                                    <a id="aCancel" runat="server" href="/Admin/OrderOverview.aspx" class="btn btn-sm btn-solid">Back</a>
                                                                    <asp:Button ID="btnDelete" runat="server" Text="Delete" OnClick="btnDelete_Click" CssClass="btn btn-sm btn-solid" CausesValidation="false"/>
                                                                </div>

                                                                
                                                                <div class="col-md-12">
                                                                    <br />
                                                                    <label>Rejection Reason</label>
                                                                    <asp:TextBox ID="txtRejectionReason" runat="server" CssClass="form-control" TextMode="MultiLine" Enabled="true"></asp:TextBox>
                                                                    <br />
                                                                </div>
                                                                <div class="col-md-12">
                                                                    <asp:Button ID="btnReject" runat="server" Text="Reject" OnClick="btnReject_Click" CssClass="btn btn-sm btn-solid" CausesValidation="false" OnClientClick="return ConfirmOnReject();"/>                                                                
                                                                </div>


                                                                <div class="col-lg-12">
                                                                    <br />
                                                                    <asp:Label ID="lblReturns" runat="server" Text="Returns"></asp:Label>
                                                                    <br />
                                                                    <asp:GridView ID="gvRMA" runat="server" AutoGenerateColumns="False" DataKeyNames="RMAID" CssClass="table cart-table" HeaderStyle-CssClass="table-head" Width="100%" 
                                                                        GridLines="None" AllowSorting="true" CellSpacing="0" CellPadding="0" OnRowDataBound="gvRMA_RowDataBound" OnRowCommand="gvRMA_RowCommand">
                                                                        <Columns>
                                                                            <asp:TemplateField HeaderText="Date Requested" ItemStyle-HorizontalAlign="Left" HeaderStyle-HorizontalAlign="Left">
                                                                                <ItemTemplate>
                                                                                    <%# string.Format("{0:MM/dd/yyyy}", Eval("CreatedOn"))%>
                                                                                </ItemTemplate>
                                                                            </asp:TemplateField>
                                                                            <asp:TemplateField HeaderText="RMA ID" ItemStyle-HorizontalAlign="Left" HeaderStyle-HorizontalAlign="Left">
                                                                                <ItemTemplate>
                                                                                    <%# Eval("RMAID")%>
                                                                                </ItemTemplate>
                                                                            </asp:TemplateField>
                                                                            <asp:TemplateField HeaderText="Label">
                                                                                <ItemTemplate>
                                                                                    <asp:Panel ID="pnlShippingLabel" runat="server">
                                                                                        <a href='<%# Eval("ShippingLabelPath") %>' target="_blank">
                                                                                            <i class="fa fa-envelope text-theme"></i>
                                                                                        </a>
                                                                                    </asp:Panel>
                                                                                </ItemTemplate>
                                                                            </asp:TemplateField>


                                                                            <asp:TemplateField HeaderText="Shiphawk ID">
                                                                                <ItemTemplate>
                                                                                    <%# Eval("ReferenceNumber") %>
                                                                                </ItemTemplate>
                                                                            </asp:TemplateField>

                                                                            <asp:TemplateField HeaderText="Delete">
                                                                                <ItemTemplate>
                                                                                    <asp:LinkButton id="btnDelete" runat="server" CommandArgument='<%# Eval("RMAID") %>' CommandName="DeleteRMA"><i class="ti-trash" style="font-size:x-large"></i></asp:LinkButton>
                                                                                </ItemTemplate>
                                                                            </asp:TemplateField>

                                                                        </Columns>
                                                                    </asp:GridView>

                                                                    <asp:HiddenField ID="hfDeleteMessage" runat="server" />
                                                                </div>

                                                            </div>
                                                        </div>
                                                    </div>
                                                </div>
                                            </div>
                                        </div>
                                    </div>
                                </div>
                            </div>
                        </div>
                    </div>
                </div>
            </div>
        </div>
    </section>
    <!--  dashboard section end -->
</asp:Content>
