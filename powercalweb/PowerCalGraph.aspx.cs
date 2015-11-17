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

            if (!IsPostBack)
            {
                updateGrpahBydate(DateTime.MinValue, DateTime.MaxValue);
            }
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
            DateTime date_start = DateTime.Parse(txtDateTimeStart.Text);
            DateTime date_end = DateTime.Parse(txtDateTimeEnd.Text);
            updateGrpahBydate(date_start, date_end);

        }

        void updateGrpahBydate(DateTime start, DateTime end){

            DataTable table_results_db = new DataTable();
            DataTable table_machies_db = new DataTable();
            using (SqlConnection con = new SqlConnection(_db_connect_str.ConnectionString))
            {
                con.Open();

                string date_start_str, date_end_str;

                SqlCommand cmd;
                if (start == DateTime.MinValue)
                {

                    cmd = new SqlCommand("select top 1 timestamp from Results order by timestamp asc", con);
                    date_start_str = cmd.ExecuteScalar().ToString();

                }
                else
                {
                    date_start_str = start.ToString();
                }
                if (end == DateTime.MaxValue)
                {

                    cmd = new SqlCommand("select top 1 timestamp from Results order by timestamp desc", con);
                    date_end_str = cmd.ExecuteScalar().ToString();

                }
                else
                {
                    date_end_str = end.ToString();
                }

                if (!IsPostBack)
                {
                    this.txtDateTimeStart.Text = date_start_str;
                    this.txtDateTimeEnd.Text = date_end_str;
                }

                string selectstr = string.Format(
                    "select * from Results where (timestamp >= '{0}' and timestamp < '{1}') order by timestamp", date_start_str, date_end_str);
                cmd = new SqlCommand(selectstr, con);

                using (SqlDataAdapter adp = new SqlDataAdapter(cmd))
                    adp.Fill(table_results_db);

                cmd = new SqlCommand("select * from Machines", con);
                using (SqlDataAdapter adp = new SqlDataAdapter(cmd))
                    adp.Fill(table_machies_db);
            }

            //GridView1.DataSource = table_results_db;
            //GridView1.DataBind();
            //return;

            //CheckBoxListMachines.DataMember = "name";
            //CheckBoxListMachines.DataSource = table_machies_db;
            //CheckBoxListMachines.DataBind();

            var q = from r in table_results_db.AsEnumerable() 
                    join m in table_machies_db.AsEnumerable() on r.Field<int>("machine_id") equals m.Field<int>("id")
                    select new 
                    {
                        timestamp = r.Field<DateTime>("timestamp"),
                        voltage_gain = r.Field<Int32>("voltage_gain"),
                        current_gain = r.Field<Int32>("current_gain"),
                        machine = m.Field<string>("name")
                    };


            DataTable table_graph = new DataTable();
            table_graph.Columns.Add("timestamp", typeof(DateTime));
            table_graph.Columns.Add("voltage_gain", typeof(double));
            table_graph.Columns.Add("current_gain", typeof(double));
            foreach (var r in q)
            {
                DataRow rowg = table_graph.NewRow();

                rowg["timestamp"] = r.timestamp;

                double voltage_gain = Convert.ToDouble(r.voltage_gain) / 0x400000;
                double current_gain = Convert.ToDouble(r.current_gain) / 0x400000;

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

            Chart1.ChartAreas.Clear();
            Chart1.ChartAreas.Add("ChartArea1");
            Chart1.ChartAreas["ChartArea1"].AxisX.ScaleView.SizeType = DateTimeIntervalType.Hours;
            Chart1.ChartAreas["ChartArea1"].AxisY.Maximum = 4.0;
            
            //GridView1.DataSource = table_graph;
            GridView1.DataSource = q;
            GridView1.DataBind();
        

            //for(int h = 0; h < 23

        }

    }
}