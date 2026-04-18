<%@ Page Language="C#" MasterPageFile="~/MasterPage.Master" AutoEventWireup="true" CodeBehind="ItemCustomization.aspx.cs" Inherits="ImageSolutionsWebsite.Admin.ItemCustomization" %>
<%@ Register Src="~/Control/AdminNavigation.ascx" TagPrefix="uc" TagName="AdminNavigation" %>

<asp:Content ID="cHead" ContentPlaceHolderID="head" runat="server">
</asp:Content>
<asp:Content ID="cHeader" ContentPlaceHolderID="cphHeader" runat="server">
    <%--<!-- Datepicker css-->
    <link rel="stylesheet" type="text/css" href="/assets/css/vendors/date-picker.css">

    <!-- App css-->
    <link rel="stylesheet" type="text/css" href="/assets/css/style.css">

    <!--Datepicker jquery-->
    <script src="/assets/js/datepicker/datepicker.js"></script>
    <script src="/assets/js/datepicker/datepicker.en.js"></script>
    <script src="/assets/js/datepicker/datepicker.custom.js"></script>--%>
</asp:Content>
<asp:Content ID="cBody" ContentPlaceHolderID="cphBody" runat="server">
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
                                                    href="#cphBody_top_1" role="tab" aria-selected="true">
                                                    <i class="icofont icofont-ui-home"></i>Item Customization
                                                </a>
                                                <div class="material-border"></div>
                                            </li>
                                        </ul>
                                    </section><br />
                                    <div class="tab-content nav-material">
                                        <div class="tab-pane fade show active" id="top_1" runat="server" role="tabpanel" aria-labelledby="top_1_tab">
                                            <div class="card dashboard-table mt-0">
                                                <div class="card-body table-responsive-sm">
                                                    <div class="table-responsive-xl">
                                                        <div class="theme-form">
                                                            <div class="form-row row">
                                                                <div class="col-md-12">
                                                                    <h3>Select From Existing</h3>
                                                                    <label>Please select: </label>
                                                                    <asp:DropDownList ID="ddlLogo" runat="server" AutoPostBack="true" OnSelectedIndexChanged="ddlLogo_SelectedIndexChanged">
                                                                        <asp:ListItem></asp:ListItem>
                                                                        <asp:ListItem Value="Logo1">One Love</asp:ListItem>
                                                                        <asp:ListItem Value="Logo2">Cane's Crew</asp:ListItem>
                                                                        <asp:ListItem Value="Logo3">Swirl Logo</asp:ListItem>
                                                                    </asp:DropDownList>
                                                                    <br />
                                                                    <br />
                                                                </div>
                                                                <div class="col-md-12">
                                                                    <h3>Upload Your Own Logo</h3>
                                                                    <label>Logo File</label>
                                                                    <asp:FileUpload ID="filLogo" runat="server" /><asp:Button ID="btnGenerate" runat="server" Text="Upload Logo" CssClass="btn btn-sm btn-solid" OnClick="btnGenerate_Click" /><asp:RequiredFieldValidator ID="RequiredFieldValidator5" runat="server" ControlToValidate="filLogo" Display="Dynamic" ErrorMessage="*" ForeColor="Red"></asp:RequiredFieldValidator><br />
                                                                </div>
                                                                <div class="col-md-12" style="display:none;">
                                                                    <label>Logo File</label>
                                                                    <asp:Image ID="imgUplodaedLogo" runat="server" Width="100px" Visible="false" /><br />
                                                                </div>
                                                                <asp:UpdatePanel ID="UpdatePanel1" runat="server">
                                                                    <ContentTemplate>
                                                                        <div class="col-md-12">
                                                                            <p></p>
                                                                            <asp:Image ID="imgResult" runat="server" Width="400px" />
                                                                            <p></p>
                                                                            <p></p>
                                                                        </div>
                                                                        <div class="col-md-2">
                                                                            <label>Position</label>
                                                                        </div>
                                                                        <div class="col-md-10">
                                                                            <asp:DropDownList ID="ddlPosition" runat="server" AutoPostBack="true" CausesValidation="false" OnSelectedIndexChanged="ddlPosition_SelectedIndexChanged">
                                                                                <asp:ListItem Value="left-chest">Right Chest</asp:ListItem>
                                                                                <asp:ListItem Value="right-chest">Left Chest</asp:ListItem>
                                                                                <asp:ListItem Value="center" Selected="True">Center-Front</asp:ListItem>
                                                                                <asp:ListItem Value="back">Center-Back</asp:ListItem>
                                                                                <asp:ListItem Value="left-waist">Right Waist</asp:ListItem>
                                                                                <asp:ListItem Value="right-waist">Left Waist</asp:ListItem>
                                                                            </asp:DropDownList>
                                                                        </div>
                                                                        <div class="col-md-2">
                                                                            <label>Ratio</label>
                                                                        </div>
                                                                        <div class="col-md-10">
                                                                            <asp:Button ID="btnRatioMinus" runat="server" CausesValidation="false" Text="-" OnClick="btnRatio_Click" CommandArgument="Minus" />
                                                                                <asp:DropDownList ID="ddlRatio" runat="server" AutoPostBack="true" CausesValidation="false" OnSelectedIndexChanged="ddlRatio_SelectedIndexChanged">
                                                                                <asp:ListItem Value="0.1">10%</asp:ListItem>
                                                                                <asp:ListItem Value="0.2">20%</asp:ListItem>
                                                                                <asp:ListItem Value="0.3">30%</asp:ListItem>
                                                                                <asp:ListItem Value="0.4">40%</asp:ListItem>
                                                                                <asp:ListItem Value="0.5">50%</asp:ListItem>
                                                                                <asp:ListItem Value="0.6">60%</asp:ListItem>
                                                                                <asp:ListItem Value="0.7">70%</asp:ListItem>
                                                                                <asp:ListItem Value="0.8">80%</asp:ListItem>
                                                                                <asp:ListItem Value="0.9">90%</asp:ListItem>
                                                                                <asp:ListItem Value="1.0">100%</asp:ListItem>
                                                                                <asp:ListItem Value="1.1">110%</asp:ListItem>
                                                                                <asp:ListItem Value="1.2">120%</asp:ListItem>
                                                                                <asp:ListItem Value="1.3">130%</asp:ListItem>
                                                                                <asp:ListItem Value="1.4">140%</asp:ListItem>
                                                                                <asp:ListItem Value="1.5">150%</asp:ListItem>
                                                                                <asp:ListItem Value="1.6">160%</asp:ListItem>
                                                                                <asp:ListItem Value="1.7">170%</asp:ListItem>
                                                                                <asp:ListItem Value="1.8">180%</asp:ListItem>
                                                                                <asp:ListItem Value="1.9">190%</asp:ListItem>
                                                                                <asp:ListItem Value="2.0" Selected="True">200%</asp:ListItem>
                                                                            </asp:DropDownList>
                                                                            <asp:Button ID="btnRatioPlus" runat="server" CausesValidation="false" Text="+" OnClick="btnRatio_Click" CommandArgument="Plus" />
                                                                        </div>
                                                                        <div class="col-md-2">
                                                                            <label>Left Margin</label>
                                                                        </div>
                                                                        <div class="col-md-10">
                                                                            <asp:Button ID="btnLeftMarginMinus" runat="server" CausesValidation="false" Text="-" OnClick="btnLeftMargin_Click" CommandArgument="Minus" />
                                                                                <asp:DropDownList ID="ddlLeftMargin" runat="server" AutoPostBack="true" CausesValidation="false" OnSelectedIndexChanged="ddlLeftMargin_SelectedIndexChanged">
                                                                                <asp:ListItem Value="1.9">10%</asp:ListItem>
                                                                                <asp:ListItem Value="1.8">20%</asp:ListItem>
                                                                                <asp:ListItem Value="1.7">30%</asp:ListItem>
                                                                                <asp:ListItem Value="1.6">40%</asp:ListItem>
                                                                                <asp:ListItem Value="1.5">50%</asp:ListItem>
                                                                                <asp:ListItem Value="1.4">60%</asp:ListItem>
                                                                                <asp:ListItem Value="1.3">70%</asp:ListItem>
                                                                                <asp:ListItem Value="1.2">80%</asp:ListItem>
                                                                                <asp:ListItem Value="1.1">90%</asp:ListItem>
                                                                                <asp:ListItem Value="1.0" Selected="True">100%</asp:ListItem>
                                                                                <asp:ListItem Value="0.9">110%</asp:ListItem>
                                                                                <asp:ListItem Value="0.8">120%</asp:ListItem>
                                                                                <asp:ListItem Value="0.7">130%</asp:ListItem>
                                                                                <asp:ListItem Value="0.6">140%</asp:ListItem>
                                                                                <asp:ListItem Value="0.5">150%</asp:ListItem>
                                                                                <asp:ListItem Value="0.4">160%</asp:ListItem>
                                                                                <asp:ListItem Value="0.3">170%</asp:ListItem>
                                                                                <asp:ListItem Value="0.2">180%</asp:ListItem>
                                                                                <asp:ListItem Value="0.1">190%</asp:ListItem>
                                                                                <asp:ListItem Value="0.0">200%</asp:ListItem>
                                                                            </asp:DropDownList>
                                                                            <asp:Button ID="btnLeftMarginPlus" runat="server" CausesValidation="false" Text="+" OnClick="btnLeftMargin_Click" CommandArgument="Plus" />
                                                                        </div>
                                                                        <div class="col-md-2">
                                                                            <label>Top Margin</label>
                                                                        </div>
                                                                        <div class="col-md-10">
                                                                            <asp:Button ID="btnTopMarginMinus" runat="server" CausesValidation="false" Text="-" OnClick="btnTopMargin_Click" CommandArgument="Minus" />
                                                                                <asp:DropDownList ID="ddlTopMargin" runat="server" AutoPostBack="true" CausesValidation="false" OnSelectedIndexChanged="ddlTopMargin_SelectedIndexChanged">
                                                                                <asp:ListItem Value="0.1">10%</asp:ListItem>
                                                                                <asp:ListItem Value="0.2">20%</asp:ListItem>
                                                                                <asp:ListItem Value="0.3">30%</asp:ListItem>
                                                                                <asp:ListItem Value="0.4">40%</asp:ListItem>
                                                                                <asp:ListItem Value="0.5">50%</asp:ListItem>
                                                                                <asp:ListItem Value="0.6">60%</asp:ListItem>
                                                                                <asp:ListItem Value="0.7">70%</asp:ListItem>
                                                                                <asp:ListItem Value="0.8">80%</asp:ListItem>
                                                                                <asp:ListItem Value="0.9">90%</asp:ListItem>
                                                                                <asp:ListItem Value="1.0" Selected="True">100%</asp:ListItem>
                                                                                <asp:ListItem Value="1.1">110%</asp:ListItem>
                                                                                <asp:ListItem Value="1.2">120%</asp:ListItem>
                                                                                <asp:ListItem Value="1.3">130%</asp:ListItem>
                                                                                <asp:ListItem Value="1.4">140%</asp:ListItem>
                                                                                <asp:ListItem Value="1.5">150%</asp:ListItem>
                                                                                <asp:ListItem Value="1.6">160%</asp:ListItem>
                                                                                <asp:ListItem Value="1.7">170%</asp:ListItem>
                                                                                <asp:ListItem Value="1.8">180%</asp:ListItem>
                                                                                <asp:ListItem Value="1.9">190%</asp:ListItem>
                                                                                <asp:ListItem Value="2.0">200%</asp:ListItem>
                                                                            </asp:DropDownList>
                                                                            <asp:Button ID="btnTopMarginPlus" runat="server" CausesValidation="false" Text="+" OnClick="btnTopMargin_Click" CommandArgument="Plus" />
                                                                        </div>
                                                                    </ContentTemplate>
                                                                </asp:UpdatePanel>
                                                                <div class="col-md-12">
                                                                    <asp:Button ID="btnSave" runat="server" Text="Save" OnClick="btnSave_Click" CssClass="btn btn-sm btn-solid" ValidationGroup="Submit" />
                                                                    <a id="aCancel" runat="server" href="/Admin/BudgetOverview.aspx" class="btn btn-sm btn-solid">Cancel</a>
                                                                    <asp:Button ID="btnDelete" runat="server" Text="Delete" OnClick="btnDelete_Click" CssClass="btn btn-sm btn-solid" CausesValidation="false"/>
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