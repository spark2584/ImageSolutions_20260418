<%@ Page Language="C#" MasterPageFile="~/MasterPage.Master" AutoEventWireup="true" CodeBehind="Message.aspx.cs" Inherits="ImageSolutionsWebsite.Admin.Message" %>
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
                                                    <i class="icofont icofont-ui-home"></i>Message Management
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
                                                                    <label>Subject</label>
                                                                    <asp:TextBox ID="txtSubject" runat="server" CssClass="form-control" placeholder="Subject" required=""></asp:TextBox>
                                                                    <asp:RequiredFieldValidator ID="RequiredFieldValidator1" runat="server" ControlToValidate="txtSubject" ValidationBudget="Submit" ErrorMessage="*" ForeColor="Red"></asp:RequiredFieldValidator>
                                                                </div>
                                                                <div class="col-md-12">
                                                                    <label>Start Date</label>
                                                                    <asp:TextBox ID="txtStartDate" runat="server" CssClass="datepicker-here form-control digits" placeholder="start date" required=""></asp:TextBox>
                                                                    <asp:RequiredFieldValidator ID="RequiredFieldValidator2" runat="server" ControlToValidate="txtStartDate" ValidationBudget="Submit" ErrorMessage="*" ForeColor="Red"></asp:RequiredFieldValidator>
                                                                </div>
                                                                <div class="col-md-12">
                                                                    <label>End Date</label>
                                                                    <asp:TextBox ID="txtEndDate" runat="server" CssClass="datepicker-here form-control digits" placeholder="end date" required=""></asp:TextBox>
                                                                    <asp:RequiredFieldValidator ID="RequiredFieldValidator3" runat="server" ControlToValidate="txtEndDate" ValidationBudget="Submit" ErrorMessage="*" ForeColor="Red"></asp:RequiredFieldValidator>
                                                                </div>
                                                                    <label>Message</label>
                                                                    <asp:TextBox ID="txtMessage" runat="server" CssClass="form-control" TextMode="MultiLine" placeholder="Subject" required=""></asp:TextBox>
                                                                    <asp:RequiredFieldValidator ID="RequiredFieldValidator4" runat="server" ControlToValidate="txtMessage" ValidationBudget="Submit" ErrorMessage="*" ForeColor="Red"></asp:RequiredFieldValidator>
                                                                </div>
                                                                <div class="col-md-12">
                                                                    <label>Notification</label>&nbsp;
                                                                    <asp:CheckBox ID="cbIsNotification" runat="server" Checked="true"/>
                                                                </div>
                                                                <div class="col-md-12">
                                                                    <label>Announcement</label>&nbsp;
                                                                    <asp:CheckBox ID="cbIsAnnouncement" runat="server" Checked="true"/>
                                                                </div>
                                                                <div class="col-md-12">
                                                                    <asp:Button ID="btnSave" runat="server" Text="Save" OnClick="btnSave_Click" CssClass="btn btn-sm btn-solid" ValidationBudget="Submit" />
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