<%@ Page Language="C#" MasterPageFile="~/MasterPage.Master" AutoEventWireup="true" CodeBehind="BudgetAssignmentAdjustment.aspx.cs" Inherits="ImageSolutionsWebsite.Admin.BudgetAssignmentAdjustment" %>
<%@ Register Src="~/Control/AdminNavigation.ascx" TagPrefix="uc" TagName="AdminNavigation" %>

<asp:Content ID="cHead" ContentPlaceHolderID="head" runat="server">
</asp:Content>
<asp:Content ID="cHeader" ContentPlaceHolderID="cphHeader" runat="server">
</asp:Content>
<asp:Content ID="cBody" ContentPlaceHolderID="cphBody" runat="server">
    <!--  dashboard section start -->
    <section class="dashboard-section section-b-space user-dashboard-section">
        <div class="container">
            <div class="row">
                <uc:AdminNavigation runat="server" id="AdminNavigation" />
                <div class="col-lg-9">
                    <div class="faq-content tab-content" id="top-tabContent">
                        <div class="tab-pane fade show active" id="orders">
                            <div class="row">
                                <div class="col-12">
                                    <div class="card dashboard-table mt-0">
                                        <div class="card-body table-responsive-sm">
                                            <div class="top-sec">
                                                <h3>Budget Assignment Adjustment</h3> 
                                            </div>
                                            <div class="table-responsive-xl">
                                                <div class="theme-form">
                                                    <div class="form-row row">
                                                        <div class="col-md-12">
                                                            <label>Current Balance</label>
                                                            <asp:TextBox ID="txtCurrentBalance" runat="server" CssClass="form-control" Enabled="false"></asp:TextBox>
                                                            <br />
                                                            <br />
                                                        </div>
                                                        <div class="col-md-12">
                                                            <label>Adjustment Amount</label>
                                                            <asp:TextBox ID="txtAmount" runat="server" CssClass="form-control" placeholder="Adjustment Amount" required=""></asp:TextBox>
                                                            <asp:RequiredFieldValidator ID="valAmount" runat="server" ControlToValidate="txtAmount" ValidationGroup="Submit" ErrorMessage="*" ForeColor="Red"></asp:RequiredFieldValidator>
                                                        </div>

                                                        <asp:Panel ID="pnlSubmitPayroll" runat="server">
                                                            <div class="col-md-12">
                                                                <label>Submit Payroll</label>
                                                                <asp:CheckBox ID="chkSubmitPayroll" runat="server"/>
                                                                <br />
                                                            </div>
                                                        </asp:Panel>

                                                        <div class="col-md-12">
                                                            <label>Reason</label>
                                                            <asp:Panel ID="pnlReason" runat="server">
                                                                <asp:TextBox ID="txtReason" runat="server" CssClass="form-control" placeholder="Reason" required=""></asp:TextBox>
                                                                <asp:RequiredFieldValidator ID="valReason" runat="server" ControlToValidate="txtReason" ValidationGroup="Submit" ErrorMessage="*" ForeColor="Red"></asp:RequiredFieldValidator>
                                                            </asp:Panel>
                                                            <asp:Panel ID="pnlEnterpriseReason" runat="server" Visible="false">
                                                                <asp:DropDownList ID="ddlEnterpriseReason" runat="server" CssClass="form-control" AutoPostBack="true" OnSelectedIndexChanged="ddlEnterpriseReason_SelectedIndexChanged">
                                                                    <asp:ListItem Selected="True" Value=""></asp:ListItem>
                                                                    <asp:ListItem Value="Promotion">Promotion</asp:ListItem>
                                                                    <asp:ListItem Value="Refresh">Refresh</asp:ListItem>
                                                                    <asp:ListItem Value="Item Replacement">Item Replacement</asp:ListItem>
                                                                    <asp:ListItem Value="Other">Other Reason</asp:ListItem>
                                                                </asp:DropDownList>
                                                                <br />
                                                                <asp:TextBox ID="txtEnterpriseOtherReason" runat="server" CssClass="form-control" placeholder="Reason" Visible="false"></asp:TextBox>
                                                                <br />
                                                            </asp:Panel>
                                                        </div>
                                                        <div class="col-md-12">
                                                            <asp:Button ID="btnSave" runat="server" Text="Save" OnClick="btnSave_Click" CssClass="btn btn-sm btn-solid" ValidationGroup="Submit" />
                                                            <a id="aCancel" runat="server" href="/Admin/BudgetOverview.aspx" class="btn btn-sm btn-solid">Cancel</a>
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