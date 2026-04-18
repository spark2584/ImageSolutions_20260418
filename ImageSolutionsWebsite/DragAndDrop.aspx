<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="DragAndDrop.aspx.cs" Inherits="ImageSolutionsWebsite.DragAndDrop" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
</head>
<body>
    <form id="form1" runat="server">
        <asp:ScriptManager ID="ToolkitScriptManager" runat="server" ScriptMode="Release" ScriptPath=""></asp:ScriptManager>
        <table>
            <tr>
                <td>Logo File</td>
                <td><asp:FileUpload ID="filLogo" runat="server" /><asp:RequiredFieldValidator ID="RequiredFieldValidator2" runat="server" ControlToValidate="filLogo" Display="Dynamic" ErrorMessage="*" ForeColor="Red"></asp:RequiredFieldValidator><br />
                    <asp:Image ID="imgUplodaedLogo" runat="server" Width="100px" />
                </td>
            </tr>
            <tr>
                <td></td>
                <td><asp:Button ID="btnGenerate" runat="server" Text="Generate Image" OnClick="btnGenerate_Click" /></td>
            </tr>
        </table>
        <table>
            <tr>
                <td>Image</td>
                <td><asp:Image ID="imgResult" runat="server" Width="400px" /></td>
            </tr>
            <tr>
                <td>Position</td>
                <td>
                    <asp:DropDownList ID="ddlPosition" runat="server" AutoPostBack="true" CausesValidation="false" OnSelectedIndexChanged="ddlPosition_SelectedIndexChanged">
                        <asp:ListItem Value="left-chest">Left Chest</asp:ListItem>
                        <asp:ListItem Value="right-chest">Right Chest</asp:ListItem>
                        <asp:ListItem Value="center" Selected="True">Center</asp:ListItem>
                        <asp:ListItem Value="left-waist">Left Waist</asp:ListItem>
                        <asp:ListItem Value="right-waist">Right Waist</asp:ListItem>
                    </asp:DropDownList>
                </td>
            </tr>
            <tr>
                <td>Ratio</td>
                <td><asp:Button ID="btnRatioMinus" runat="server" CausesValidation="false" Text="-" OnClick="btnRatio_Click" CommandArgument="Minus" />
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
                    <asp:Button ID="btnRatioPlus" runat="server" CausesValidation="false" Text="+" OnClick="btnRatio_Click" CommandArgument="Plus" /></td>
            </tr>
            <tr>
                <td>Left Margin</td>
                <td><asp:Button ID="btnLeftMarginMinus" runat="server" CausesValidation="false" Text="-" OnClick="btnLeftMargin_Click" CommandArgument="Minus" />
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
                </td>
            </tr>
            <tr>
                <td>Top Margin</td>
                <td><asp:Button ID="btnTopMarginMinus" runat="server" CausesValidation="false" Text="-" OnClick="btnTopMargin_Click" CommandArgument="Minus" />
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
                </td>
            </tr>
        </table>
    </form>
</body>
</html>
