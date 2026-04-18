<%@ Page Title="" Language="C#" MasterPageFile="~/MasterPage.Master" AutoEventWireup="true" CodeBehind="UserRegistration.aspx.cs" Inherits="ImageSolutionsWebsite.UserRegistration" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="cphHeader" runat="server">
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="cphBody" runat="server">
    <section class="login-page section-b-space">
        <div class="container">
            <div class="row">
                <div class="col-lg-12">
                    <h3>User Account Registration</h3>
                    <div class="theme-card">
                        <div class="theme-form">
                            <asp:Repeater ID="rptCustomField" runat="server" DataMember="CustomFieldID" OnItemDataBound="rptCustomField_ItemDataBound">
                                <ItemTemplate>
                                    <div class="form-group">
                                        <asp:Label ID="lblLabel" runat="server" Text='<%# Eval("Name")%>' Font-Bold="true"></asp:Label>
                                        <asp:TextBox ID="txtCustomValue"  runat="server" CssClass="form-control" Visible="true"></asp:TextBox>
                                        <asp:DropDownList ID="ddlCustomValueList" runat="server" DataTextField="Value" DataValueField="Value" CssClass="form-control" Visible="false"></asp:DropDownList>
                                        <asp:HiddenField ID="hfCustomFieldID" runat="server" Value='<%# Eval("CustomFieldID")%>'/>
                                    </div>
                                    <br />
                                </ItemTemplate>
                            </asp:Repeater>

                            <div class="form-group" id="divRegistrationKey" runat="server">
                                <label for="txtRegistrationKey"><asp:Label ID="lblRegistrationKey" runat="server" Text="Registration Key"></asp:Label></label>
                                <asp:TextBox ID="txtRegistrationKey" runat="server" ValidationGroup="register" CssClass="form-control" placeholder="Registration Key" required=""></asp:TextBox>
                                <asp:RequiredFieldValidator ID="RequiredFieldValidator6" runat="server" ControlToValidate="txtRegistrationKey" ValidationGroup="register" ErrorMessage="*" ForeColor="Red"></asp:RequiredFieldValidator>     
                            </div>
                            <div class="form-group">
                                <label for="txtFirstName">First Name</label>
                                <asp:TextBox ID="txtFirstName" runat="server" ValidationGroup="register" CssClass="form-control" placeholder="First Name" required=""></asp:TextBox>
                                <asp:RequiredFieldValidator ID="RequiredFieldValidator3" runat="server" ControlToValidate="txtFirstName" ValidationGroup="register" ErrorMessage="*" ForeColor="Red"></asp:RequiredFieldValidator>     
                            </div>
                            <div class="form-group">
                                <label for="txtLastName">Last Name</label>
                                <asp:TextBox ID="txtLastName" runat="server" ValidationGroup="register" CssClass="form-control" placeholder="Last Name" required=""></asp:TextBox>
                                <asp:RequiredFieldValidator ID="RequiredFieldValidator1" runat="server" ControlToValidate="txtLastName" ValidationGroup="register" ErrorMessage="*" ForeColor="Red"></asp:RequiredFieldValidator>     
                            </div>
                            <div class="form-group">
                                <label for="txtEmail">Email Address</label>
                                <asp:TextBox ID="txtEmail" runat="server" ValidationGroup="register" CssClass="form-control" placeholder="Email" required=""></asp:TextBox>
                                <asp:RequiredFieldValidator ID="RequiredFieldValidator2" runat="server" ControlToValidate="txtEmail" ValidationGroup="register" ErrorMessage="*" ForeColor="Red"></asp:RequiredFieldValidator>     
                                <asp:RegularExpressionValidator ID="valEmail" runat="server" ControlToValidate="txtEmail" ValidationGroup="register" ErrorMessage="Invalid Email" ValidationExpression="\w+([-+.]\w+)*@\w+([-.]\w+)*\.\w+([-.]\w+)*" ForeColor="Red" />
                            </div>
                            <div class="form-group">
                                <label for="txtPassword">Password</label>
                                <asp:TextBox ID="txtPassword" runat="server" TextMode="Password" CssClass="form-control" placeholder="Password" required=""></asp:TextBox>
                                <asp:RequiredFieldValidator ID="RequiredFieldValidator4" runat="server" ControlToValidate="txtPassword" ValidationGroup="register" ErrorMessage="*" ForeColor="Red"></asp:RequiredFieldValidator>
                                <asp:RegularExpressionValidator ID="valPassword" runat="server" ControlToValidate="txtPassword" ValidationGroup="register" ErrorMessage="Password must be between 6 to 20 characters" ValidationExpression=".{6,20}.*" ForeColor="Red" />
                            </div>
                            <div class="form-group">
                                <label for="txtPasswordConfirm">Confirm Password</label>
                                <asp:TextBox ID="txtPasswordConfirm" runat="server" TextMode="Password" CssClass="form-control" placeholder="Password Confirm" required=""></asp:TextBox>
                                <asp:RequiredFieldValidator ID="RequiredFieldValidator5" runat="server" ControlToValidate="txtPasswordConfirm" ValidationGroup="register" ErrorMessage="*" ForeColor="Red"></asp:RequiredFieldValidator>
                                <asp:CompareValidator ID="CompareValidator1" runat="server" ControlToCompare="txtPassword" ControlToValidate="txtPasswordConfirm" ValidationGroup="register" ForeColor="Red" Display="Dynamic" ErrorMessage="Passwords do not Match"></asp:CompareValidator>
                            </div>
                            <asp:Button ID="btnRegister" runat="server" Text="Register" OnClick="btnRegister_Click" ValidationGroup="register" CssClass="btn btn-solid"/>
                        </div>
                    </div>
                </div>
            </div>
        </div>
    </section>
</asp:Content>
