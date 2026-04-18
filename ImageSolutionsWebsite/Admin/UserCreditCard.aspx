<%@ Page Language="C#" MasterPageFile="~/MasterPage.Master" AutoEventWireup="true" CodeBehind="UserCreditCard.aspx.cs" Inherits="ImageSolutionsWebsite.Admin.UserCreditCard" %>
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
                                                <h3>User Credit Card</h3>
                                            </div>
                                            <div class="table-responsive-xl">
                                                <div class="theme-form">
                                                    <div class="form-row row">
                                                        <div class="col-md-12">
                                                            <b>Filter</b><hr />
                                                        </div>
                                                        <div class="col-md-4">
                                                            <label>Email</label>
                                                            <asp:TextBox ID="txtEmailAddress" runat="server" CssClass="form-control" AutoPostBack="true" OnTextChanged="txtUserFilter_TextChanged"></asp:TextBox>
                                                        </div>
                                                        <div class="col-md-4">
                                                            <label>First</label>
                                                            <asp:TextBox ID="txtFirstName" runat="server" CssClass="form-control" AutoPostBack="true" OnTextChanged="txtUserFilter_TextChanged"></asp:TextBox>
                                                        </div>
                                                        <div class="col-md-4">
                                                            <label>Last</label>
                                                            <asp:TextBox ID="txtLastName" runat="server" CssClass="form-control" AutoPostBack="true" OnTextChanged="txtUserFilter_TextChanged"></asp:TextBox>
                                                        </div>
                                                        <div><br />
                                                            <hr />
                                                        </div>
                                                        <div class="col-md-12">
                                                            <label>User</label>
                                                            <asp:DropDownList ID="ddlUser" runat="server" DataTextField="Description" DataValueField="UserInfoID" CssClass="form-control"></asp:DropDownList>
                                                            <asp:RequiredFieldValidator ID="valUser" runat="server" ControlToValidate="ddlUser" ValidationGroup="Submit" ErrorMessage="*" ForeColor="Red"></asp:RequiredFieldValidator>
                                                        </div>
                                                        <div class="col-md-12">
                                                            <label>Reset Day of the Month</label>
                                                            <asp:DropDownList ID="ddlDay" runat="server" CssClass="form-control">
                                                                <asp:ListItem Selected="True" Value="1">1</asp:ListItem>
                                                                <asp:ListItem Value="2">2</asp:ListItem>
                                                                <asp:ListItem Value="3">3</asp:ListItem>
                                                                <asp:ListItem Value="4">4</asp:ListItem>
                                                                <asp:ListItem Value="5">5</asp:ListItem>
                                                                <asp:ListItem Value="6">6</asp:ListItem>
                                                                <asp:ListItem Value="7">7</asp:ListItem>
                                                                <asp:ListItem Value="8">8</asp:ListItem>
                                                                <asp:ListItem Value="9">9</asp:ListItem>
                                                                <asp:ListItem Value="10">10</asp:ListItem>
                                                                <asp:ListItem Value="11">11</asp:ListItem>
                                                                <asp:ListItem Value="12">12</asp:ListItem>
                                                                <asp:ListItem Value="13">13</asp:ListItem>
                                                                <asp:ListItem Value="14">14</asp:ListItem>
                                                                <asp:ListItem Value="15">15</asp:ListItem>
                                                                <asp:ListItem Value="16">16</asp:ListItem>
                                                                <asp:ListItem Value="17">17</asp:ListItem>
                                                                <asp:ListItem Value="18">18</asp:ListItem>
                                                                <asp:ListItem Value="19">19</asp:ListItem>
                                                                <asp:ListItem Value="20">20</asp:ListItem>
                                                                <asp:ListItem Value="21">21</asp:ListItem>
                                                                <asp:ListItem Value="22">22</asp:ListItem>
                                                                <asp:ListItem Value="23">23</asp:ListItem>
                                                                <asp:ListItem Value="24">24</asp:ListItem>
                                                                <asp:ListItem Value="25">25</asp:ListItem>
                                                                <asp:ListItem Value="26">26</asp:ListItem>
                                                                <asp:ListItem Value="27">27</asp:ListItem>
                                                                <asp:ListItem Value="28">28</asp:ListItem>
                                                                <asp:ListItem Value="29">29</asp:ListItem>
                                                                <asp:ListItem Value="30">30</asp:ListItem>
                                                                <asp:ListItem Value="31">31</asp:ListItem>
                                                            </asp:DropDownList>
                                                        </div>
                                                        <div class="col-md-12">
                                                            <label>Limit $</label>
                                                            <asp:TextBox ID="txtLimit" runat="server" CssClass="form-control" placeholder="$" onKeyPress="if (event.keyCode!=46 && event.keyCode!=45 && (event.keyCode<48 || event.keyCode>57)) event.returnValue=false"></asp:TextBox>
                                                            <br />
                                                        </div>
                                                        <div class="col-md-12">
                                                            <asp:Button ID="btnSave" runat="server" Text="Save" OnClick="btnSave_Click" CssClass="btn btn-sm btn-solid" ValidationGroup="Submit" />
                                                            <a id="aCancel" runat="server" href="/Admin/CreditCardOverview.aspx" class="btn btn-sm btn-solid">Cancel</a>
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
    </section>
    <!--  dashboard section end -->
</asp:Content>
