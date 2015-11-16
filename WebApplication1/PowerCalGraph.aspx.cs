using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.DataVisualization.Charting;

using System.Data;
using System.Data.SqlClient;
using System.Data.SqlTypes;

namespace WebApplication1
{
    public partial class WebForm1 : System.Web.UI.Page
    {
        static SqlConnectionStringBuilder _db_connect_str;
        static int _last = 0;

        protected void Page_Load(object sender, EventArgs e)
        {
            _db_connect_str = new SqlConnectionStringBuilder(
                "Data Source=a1040.centralite.com;Initial Catalog=PowerCalibration;Integrated Security=True");

            if(!IsPostBack)
                updateGrpahBydate(DateTime.MaxValue);
        }

        protected void DropDownList1_SelectedIndexChanged(object sender, EventArgs e)
        {
            string cmd = "SELECT * FROM [Results]";
            if (DropDownList1.SelectedValue != "0")
                cmd += string.Format(" WHERE [machine_id] = {0}", DropDownList1.SelectedValue);
        }

        protected void DropDownList1_DataBinding(object sender, EventArgs e)
        {
        }

        protected void DropDownList1_DataBound(object sender, EventArgs e)
        {
            DropDownList1.Items.Add(new ListItem("All", "0"));

        }

        protected void Chart1_DataBinding(object sender, EventArgs e)
        {

        }


        protected void Button1_Click(object sender, EventArgs e)
        {

            SeriesChartType next_type = (SeriesChartType)Enum.GetValues(typeof(SeriesChartType)).GetValue(_last++);
            if (next_type == SeriesChartType.ThreeLineBreak)
            {
                _last += 3;
                next_type = (SeriesChartType)Enum.GetValues(typeof(SeriesChartType)).GetValue(_last);
            }
                
            if (_last >= Enum.GetValues(typeof(SeriesChartType)).Length)
                _last = 0;

            try
            {
                string y_axis = "voltage_gain";
                Chart1.Series[y_axis].ChartType = next_type;
                y_axis = "current_gain";
                Chart1.Series[y_axis].ChartType = next_type;
            }
            catch (Exception ex)
            {
                string msg = ex.Message;
            }
        }

        protected void Button2_Click(object sender, EventArgs e)
        {
            if (GridView1.Visible)
            {
                GridView1.Visible = false;
                ButtonShowTable.Text = "Show Data";
            }
            else
            {
                GridView1.Visible = true;
                ButtonShowTable.Text = "Hide Data";
            }
        }

        protected void ButtonGo_Click(object sender, EventArgs e)
        {
            DateTime date = DateTime.Parse(txtDateTime.Text);
            updateGrpahBydate(date);

        }

        void updateGrpahBydate(DateTime start){
        
            DataTable table_db = new DataTable();
            //string "select * from Results where machine_id=10 and timestamp >= '2015/11/12' order by timestamp"
            using (SqlConnection con = new SqlConnection(_db_connect_str.ConnectionString))
            {
                con.Open();

                SqlCommand cmd;
                if (start == DateTime.MaxValue)
                {
                    cmd = new SqlCommand("select top 1 timestamp from Results where (machine_id=10 or machine_id=9)  order by timestamp desc", con);
                    start = (DateTime)cmd.ExecuteScalar();
                    string selectstr = string.Format(
                        "select * from Results where machine_id=10 and timestamp >= '{0}' order by timestamp", start.Date.ToShortDateString());
                    cmd = new SqlCommand(selectstr, con);
                }
                else
                {
                    string selectstr = string.Format(
                        "select * from Results where machine_id=10 and timestamp >= '{0}' order by timestamp", start.ToString());
                    cmd = new SqlCommand(selectstr, con);

                }

                using (SqlDataAdapter adp = new SqlDataAdapter(cmd))
                    adp.Fill(table_db);
            }

            DataTable table_graph = new DataTable();
            table_graph.Columns.Add("timestamp", typeof(DateTime));
            table_graph.Columns.Add("voltage_gain", typeof(double));
            table_graph.Columns.Add("current_gain", typeof(double));
            foreach (DataRow rowd in table_db.Rows)
            {
                DataRow rowg = table_graph.NewRow();

                rowg["timestamp"] = rowd["timestamp"];

                double voltage_gain = Convert.ToDouble(rowd["voltage_gain"]) / 0x400000;
                double current_gain = Convert.ToDouble(rowd["current_gain"]) / 0x400000;

                rowg["voltage_gain"] = voltage_gain;
                rowg["current_gain"] = current_gain;

                table_graph.Rows.Add(rowg);

                //ScriptManager1.RegisterAsyncPostBackControl(Button2);

            }

            Chart1.Series.Clear();

            string y_axis = "voltage_gain";
            Chart1.Series.Add(y_axis);
            Chart1.Series[y_axis].Points.DataBind(table_graph.AsEnumerable(), "timestamp", y_axis, "");
            Chart1.Series[y_axis].ChartType = SeriesChartType.Point;

            Chart1.Series[y_axis].XValueType = ChartValueType.Time;
            Chart1.Series[y_axis].YValuesPerPoint = 1;
            Chart1.Series[y_axis].YValueType = ChartValueType.Double;

            //Chart1.Legends[0].Enabled = true;
            //Chart1.Series[y_axis]["ShowMarkerLines"] = "true";

            y_axis = "current_gain";
            Chart1.Series.Add(y_axis);
            Chart1.Series[y_axis].Points.DataBind(table_graph.AsEnumerable(), "timestamp", y_axis, "");
            Chart1.Series[y_axis].ChartType = SeriesChartType.Point;

            Chart1.Series[y_axis].XValueType = ChartValueType.Time;
            Chart1.Series[y_axis].YValuesPerPoint = 1;
            Chart1.Series[y_axis].YValueType = ChartValueType.Double;


            Chart1.Legends.Add("Lengend");
            
            
            GridView1.DataSource = table_graph;
            GridView1.DataBind();
        
        }

    }
}