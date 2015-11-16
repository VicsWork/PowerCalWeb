<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="PowerCalGraph.aspx.cs" Inherits="WebApplication1.WebForm1" %>

<%@ Register Assembly="System.Web.DataVisualization, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35" Namespace="System.Web.UI.DataVisualization.Charting" TagPrefix="asp" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
</head>
<body>
    <form id="form1" runat="server">
        <div>

            <asp:ScriptManager ID="ScriptManager1" runat="server">
            </asp:ScriptManager>

            <asp:UpdatePanel ID="UpdatePanel2" runat="server" UpdateMode="Conditional">
                <ContentTemplate>
                    <asp:DropDownList ID="DropDownList1" runat="server" AutoPostBack="True" DataSourceID="SqlDataSource2" DataTextField="name" DataValueField="id" OnDataBinding="DropDownList1_DataBinding" OnDataBound="DropDownList1_DataBound" OnSelectedIndexChanged="DropDownList1_SelectedIndexChanged">
                    </asp:DropDownList>
                    <asp:SqlDataSource ID="SqlDataSource2" runat="server" ConnectionString="<%$ ConnectionStrings:PowerCalibrationConnectionString %>" SelectCommand="SELECT * FROM [Machines]"></asp:SqlDataSource>
                    <asp:Button ID="Button1" runat="server" OnClick="Button1_Click" Text="Button" />
                    <div>
                        <asp:Chart ID="Chart1" runat="server" Height="560px" Width="982px">
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
                    <asp:TextBox ID="txtDateTime" runat="server" Width="120px">2015-11-16 12:00:00</asp:TextBox>
                    <asp:Button ID="ButtonGo" runat="server" OnClick="ButtonGo_Click" Text="Go" />
                </ContentTemplate>
            </asp:UpdatePanel>

        </div>

        <br />

        <asp:UpdatePanel ID="UpdatePanel1" runat="server" UpdateMode="Conditional">
            <ContentTemplate>
                <br />
                <asp:Button ID="ButtonShowTable" runat="server" OnClick="Button2_Click" Text="Show Data" />
                <asp:GridView ID="GridView1" runat="server" Style="margin-top: 17px" Visible="False">
                </asp:GridView>
            </ContentTemplate>
            <Triggers>
                <asp:AsyncPostBackTrigger ControlID="ButtonGo" EventName="Click" />
            </Triggers>
        </asp:UpdatePanel>
        <br />
    </form>
</body>
</html>
