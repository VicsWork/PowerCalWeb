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
        static DataTable _table_machies_db = new DataTable();

        protected void Page_Load(object sender, EventArgs e)
        {
            _db_connect_str = new SqlConnectionStringBuilder(
                "Data Source=a1040.centralite.com;Initial Catalog=PowerCalibration;Integrated Security=True");

            if (!IsPostBack)
            {
                using (SqlConnection con = new SqlConnection(_db_connect_str.ConnectionString))
                {
                    con.Open();
                    SqlCommand cmd = new SqlCommand("select * from Machines", con);
                    using (SqlDataAdapter adp = new SqlDataAdapter(cmd))
                        adp.Fill(_table_machies_db);
                }
                CheckBoxListMachines.DataTextField = "name";
                CheckBoxListMachines.DataValueField = "id";
                CheckBoxListMachines.DataSource = _table_machies_db;
                CheckBoxListMachines.DataBind();
                foreach (ListItem item in CheckBoxListMachines.Items)
                    item.Selected = true;

                DateTime start = DateTime.Now - new TimeSpan(5, 0, 0, 0);
                DateTime end = DateTime.Now;
                txtDateTimeStart.Text = start.ToString();
                txtDateTimeEnd.Text = end.ToString();

                updateGrpahBydate();
            }
        }

        protected void ButtonShowTable_Click(object sender, EventArgs e)
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
            updateGrpahBydate();
        }

        void updateGrpahBydate()
        {
            DataTable table_results_db = new DataTable();
            using (SqlConnection con = new SqlConnection(_db_connect_str.ConnectionString))
            {
                con.Open();

                SqlCommand cmd;

                string selectstr = string.Format(
                    "select * from Results where timestamp >= '{0}' and timestamp < '{1}'", txtDateTimeStart.Text, txtDateTimeEnd.Text);

                if (CheckBoxListMachines.SelectedIndex >= 0)
                {
                    selectstr += " and machine_id in (";
                    for (int i = 0; i < CheckBoxListMachines.Items.Count; i++)
                        if (CheckBoxListMachines.Items[i].Selected)
                            selectstr += CheckBoxListMachines.Items[i].Value + ',';
                    selectstr = selectstr.TrimEnd(new char[] { ',' });
                    selectstr += ")";
                }
                selectstr += " order by timestamp";

                cmd = new SqlCommand(selectstr, con);
                using (SqlDataAdapter adp = new SqlDataAdapter(cmd))
                    adp.Fill(table_results_db);
            }

            //GridView1.DataSource = table_results_db;
            //GridView1.DataBind();
            //return;

            var q = from r in table_results_db.AsEnumerable()
                    join m in _table_machies_db.AsEnumerable() on r.Field<int>("machine_id") equals m.Field<int>("id")
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
            Chart1.ChartAreas["ChartArea1"].AxisY.Maximum = 2.0;

            GridView1.DataSource = table_graph;
            //GridView1.DataSource = q;
            GridView1.DataBind();


            //for(int h = 0; h < 23

        }

    }
}