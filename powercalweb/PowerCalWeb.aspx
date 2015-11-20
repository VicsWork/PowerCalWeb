<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="PowerCalWeb.aspx.cs" Inherits="PowerCalibration.WebForm_PowerCalibration" %>

<%@ Register Assembly="System.Web.DataVisualization, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35" Namespace="System.Web.UI.DataVisualization.Charting" TagPrefix="asp" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>Power Calibration Data</title>
</head>
<body>
    <form id="form1" runat="server">
        <div>

            <asp:ScriptManager ID="ScriptManager1" runat="server">
            </asp:ScriptManager>

            <asp:UpdatePanel ID="UpdatePanelGraph" runat="server" UpdateMode="Conditional">
                <ContentTemplate>
                    <asp:CheckBoxList ID="CheckBoxListMachines" runat="server" AutoPostBack="True" OnSelectedIndexChanged="CheckBoxListMachines_SelectedIndexChanged" RepeatDirection="Horizontal">
                        <asp:ListItem>Machine1</asp:ListItem>
                        <asp:ListItem>Machne2</asp:ListItem>
                    </asp:CheckBoxList>
                    <asp:TextBox ID="txtDateTimeStart" runat="server" Width="160px" TextMode="DateTime">2015-11-16 00:00:00</asp:TextBox> To 
                    <asp:TextBox ID="txtDateTimeEnd" runat="server" Width="160px" TextMode="DateTime">2015-11-17 00:00:00</asp:TextBox>
                    <asp:Button ID="ButtonGo" runat="server" OnClick="ButtonGo_Click" Text="Go" />
                    <br />
                    <asp:Button ID="ButtonSubtractDay" runat="server" OnClick="ButtonSubtractDay_Click" Text="&lt;&lt;" ToolTip="Zoom out" />
                    <asp:Button ID="ButtonPreviousDay" runat="server" OnClick="ButtonPreviousDay_Click" Text="&lt;" ToolTip="Previous Day" />
                    <asp:Button ID="ButtonNextDay" runat="server" OnClick="ButtonNextDay_Click" Text="&gt;" ToolTip="Next Day" />
                    <asp:Button ID="ButtonAddDay" runat="server" OnClick="ButtonAddDay_Click" Text="&gt;&gt;" ToolTip="Zoom in" />
                    <br />
                    <asp:Timer ID="Timer1" runat="server" Interval="120000" OnTick="Timer1_Tick">
                    </asp:Timer>
                    <asp:Chart ID="ChartCounts" runat="server" Height="600px" Width="1000px">
                        <Series>
                            <asp:Series Name="Series1" ChartType="StackedColumn">
                            </asp:Series>
                        </Series>
                        <ChartAreas>
                            <asp:ChartArea Name="ChartArea1">
                            </asp:ChartArea>
                        </ChartAreas>
                    </asp:Chart>
                    <div>
                    <asp:Chart ID="ChartGains" runat="server" Height="560px" Width="1000px">
                        <Series>
                            <asp:Series ChartType="Line" Legend="Legend1" Name="SeriesName" XValueType="Time" YValueType="Double">
                                <Points>
                                    <asp:DataPoint XValue="23327.25" YValues="0.9" />
                                    <asp:DataPoint XValue="23327.270833333332" YValues="0.8" />
                                    <asp:DataPoint XValue="23327.291666666668" YValues="0.95" />
                                </Points>
                            </asp:Series>
                        </Series>
                        <MapAreas>
                            <asp:MapArea Coordinates="0,0,0,0" />
                        </MapAreas>
                        <ChartAreas>
                            <asp:ChartArea BorderDashStyle="Solid" BorderWidth="2" Name="ChartArea1">
                                <AxisX>
                                    <ScaleView SizeType="Hours" />
                                </AxisX>
                            </asp:ChartArea>
                        </ChartAreas>
                        <Legends>
                            <asp:Legend Name="Legend1">
                            </asp:Legend>
                        </Legends>
                    </asp:Chart>
                    </div>
                </ContentTemplate>
            </asp:UpdatePanel>

        </div>

        <br />

        <asp:UpdatePanel ID="UpdatePanel1" runat="server" UpdateMode="Conditional">
            <ContentTemplate>
                <br />
                <asp:Button ID="ButtonShowTable" runat="server" OnClick="ButtonShowTable_Click" Text="Show Data" />
                <asp:GridView ID="GridViewCounts" runat="server" Style="margin-top: 17px" Visible="False">
                </asp:GridView>
                <asp:GridView ID="GridViewGains" runat="server" Style="margin-top: 17px" Visible="False">
                </asp:GridView>
                <br />
            </ContentTemplate>
            <Triggers>
                <asp:AsyncPostBackTrigger ControlID="ButtonGo" EventName="Click" />
            </Triggers>
        </asp:UpdatePanel>
        <br />
    </form>
</body>
</html>
