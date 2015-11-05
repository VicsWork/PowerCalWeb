<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="WebForm1.aspx.cs" Inherits="WebApplication1.WebForm1" %>

<%@ Register assembly="System.Web.DataVisualization, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35" namespace="System.Web.UI.DataVisualization.Charting" tagprefix="asp" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
</head>
<body>
    <form id="form1" runat="server">
    <div>
    
        <asp:DropDownList ID="DropDownList1" runat="server" AutoPostBack="True" DataSourceID="SqlDataSource2" DataTextField="name" DataValueField="id" OnDataBinding="DropDownList1_DataBinding" OnDataBound="DropDownList1_DataBound" OnSelectedIndexChanged="DropDownList1_SelectedIndexChanged">
        </asp:DropDownList>
        <asp:SqlDataSource ID="SqlDataSource2" runat="server" ConnectionString="<%$ ConnectionStrings:PowerCalibrationConnectionString %>" SelectCommand="SELECT * FROM [Machines]"></asp:SqlDataSource>
    
    </div>
        <asp:Chart ID="Chart1" runat="server" DataSourceID="SqlDataSource1" Height="560px" Width="982px">
            <series>
                <asp:Series Name="Series1" XValueMember="timestamp" YValueMembers="voltage_gain">
                </asp:Series>
            </series>
            <chartareas>
                <asp:ChartArea Name="ChartArea1">
                </asp:ChartArea>
            </chartareas>
        </asp:Chart>
        <asp:SqlDataSource ID="SqlDataSource1" runat="server" ConnectionString="<%$ ConnectionStrings:PowerCalibrationConnectionString %>" SelectCommand="SELECT * FROM [Results] WHERE ([machine_id] = @machine_id)">
            <SelectParameters>
                <asp:FormParameter DefaultValue="1" FormField="DropDownList1.ValueField" Name="machine_id" Type="Int32" />
            </SelectParameters>
        </asp:SqlDataSource>
    </form>
</body>
</html>
