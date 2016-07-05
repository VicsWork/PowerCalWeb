<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Find.aspx.cs" Inherits="PowerCalibration.Find" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>Power Calibration Data Finder</title>
</head>
<body>
    <form id="form1" runat="server">

            <asp:ScriptManager ID="ScriptManager1" runat="server">
            </asp:ScriptManager>

            <asp:SqlDataSource ID="SqlDataSourceRS" runat="server"></asp:SqlDataSource>
            <asp:Label ID="Label1" runat="server" Text="EUI"></asp:Label>
            <asp:TextBox ID="TextBoxEUI" runat="server"></asp:TextBox>
            <asp:Button ID="ButtonFind" runat="server" OnClick="ButtonFind_Click" Text="Find" />
            <asp:GridView ID="GridViewResults" runat="server" OnRowDataBound="GridViewResults_RowDataBound">
            </asp:GridView>
            <br />
            <br />

    </form>
</body>
</html>
