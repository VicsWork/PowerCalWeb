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

namespace PowerCalibration
{
    public partial class WebForm_PowerCalibration : System.Web.UI.Page
    {
        static SqlConnectionStringBuilder _db_connect_str;
        static DataTable _table_machies_db;

        protected void Page_Load(object sender, EventArgs e)
        {
            _db_connect_str = new SqlConnectionStringBuilder(Properties.Settings.Default.PowerCalibrationConnectionString);

            if (!IsPostBack)
            {
                _table_machies_db = new DataTable();
                using (SqlConnection con = new SqlConnection(_db_connect_str.ConnectionString))
                {
                    con.Open();
                    SqlCommand cmd = new SqlCommand("select * from TestStationMachines", con);
                    using (SqlDataAdapter adp = new SqlDataAdapter(cmd))
                        adp.Fill(_table_machies_db);
                }
                CheckBoxListMachines.Items.Clear();
                CheckBoxListMachines.DataTextField = "Name";
                CheckBoxListMachines.DataValueField = "Id";
                CheckBoxListMachines.DataSource = _table_machies_db;
                CheckBoxListMachines.DataBind();
                foreach (ListItem item in CheckBoxListMachines.Items)
                {
                    if (item.Text != "A1040" && item.Text != "Unknown")
                        item.Selected = true;
                }

                DateTime start = DateTime.Now.Date;
                DateTime end = DateTime.Now;
                txtDateTimeStart.Text = start.ToString();
                txtDateTimeEnd.Text = end.ToString();

                updateData();
            }
        }

        protected void ButtonShowTable_Click(object sender, EventArgs e)
        {
            if (GridViewGains.Visible)
            {
                GridViewCounts.Visible = false;
                GridViewGains.Visible = false;
                ButtonShowTable.Text = "Show Data";
            }
            else
            {
                GridViewCounts.Visible = true;
                GridViewGains.Visible = true;
                ButtonShowTable.Text = "Hide Data";
            }
        }

        protected void ButtonGo_Click(object sender, EventArgs e)
        {
            updateData();
        }

        void updateData()
        {
            DataTable table_results_db = new DataTable();
            using (SqlConnection con = new SqlConnection(_db_connect_str.ConnectionString))
            {
                con.Open();

                SqlCommand cmd;

                string selectstr = string.Format(
                    "select * from CalibrationResults where DateCalibrated >= '{0}' and DateCalibrated < '{1}'", txtDateTimeStart.Text, txtDateTimeEnd.Text);

                if (CheckBoxListMachines.SelectedIndex >= 0)
                {
                    selectstr += " and MachineId in (";
                    for (int i = 0; i < CheckBoxListMachines.Items.Count; i++)
                        if (CheckBoxListMachines.Items[i].Selected)
                            selectstr += CheckBoxListMachines.Items[i].Value + ',';
                    selectstr = selectstr.TrimEnd(new char[] { ',' });
                    selectstr += ")";
                }
                selectstr += " order by DateCalibrated";

                cmd = new SqlCommand(selectstr, con);
                using (SqlDataAdapter adp = new SqlDataAdapter(cmd))
                    adp.Fill(table_results_db);
            }

            //GridView1.DataSource = table_results_db;
            //GridView1.DataBind();
            //return;

            var query_graph = from r in table_results_db.AsEnumerable()
                              join m in _table_machies_db.AsEnumerable() on r.Field<int>("MachineId") equals m.Field<int>("id")
                              select new
                              {
                                  timestamp = r.Field<DateTime>("DateCalibrated"),
                                  voltage_gain = r.Field<Int32>("VoltageGain"),
                                  current_gain = r.Field<Int32>("CurrentGain"),
                                  euiid = r.Field<Int32>("EuiId"),
                                  machine = m.Field<string>("Name")
                              };


            DataTable table_graph = new DataTable();
            table_graph.Columns.Add("DateCalibrated", typeof(DateTime));
            table_graph.Columns.Add("VoltageGain", typeof(double));
            table_graph.Columns.Add("CurrentGain", typeof(double));
            table_graph.Columns.Add("EuiId", typeof(int));
            table_graph.Columns.Add("Machine", typeof(string));
            foreach (var r in query_graph)
            {
                DataRow rowg = table_graph.NewRow();

                rowg["DateCalibrated"] = r.timestamp;

                double voltage_gain = Convert.ToDouble(r.voltage_gain) / 0x400000;
                double current_gain = Convert.ToDouble(r.current_gain) / 0x400000;

                rowg["VoltageGain"] = voltage_gain;
                rowg["CurrentGain"] = current_gain;

                rowg["EuiId"] = r.euiid;
                rowg["Machine"] = r.machine;

                table_graph.Rows.Add(rowg);
            }

            ChartGains.Series.Clear();

            string series_name = "VoltageGain";
            ChartGains.Series.Add(series_name);
            ChartGains.Series[series_name].Points.DataBind(table_graph.AsEnumerable(), "DateCalibrated", series_name, "");
            ChartGains.Series[series_name].ChartType = SeriesChartType.Point;

            ChartGains.Series[series_name].XValueType = ChartValueType.Time;
            ChartGains.Series[series_name].YValuesPerPoint = 1;
            ChartGains.Series[series_name].YValueType = ChartValueType.Double;

            //Chart1.Legends[0].Enabled = true;
            //Chart1.Series[y_axis]["ShowMarkerLines"] = "true";

            series_name = "CurrentGain";
            ChartGains.Series.Add(series_name);
            ChartGains.Series[series_name].Points.DataBind(table_graph.AsEnumerable(), "DateCalibrated", series_name, "");
            ChartGains.Series[series_name].ChartType = SeriesChartType.Point;

            ChartGains.Series[series_name].XValueType = ChartValueType.Time;
            ChartGains.Series[series_name].YValuesPerPoint = 1;
            ChartGains.Series[series_name].YValueType = ChartValueType.Double;


            ChartGains.Legends.Add("Lengend");

            ChartGains.ChartAreas.Clear();
            ChartGains.ChartAreas.Add("ChartArea1");
            ChartGains.ChartAreas["ChartArea1"].AxisX.ScaleView.SizeType = DateTimeIntervalType.Hours;
            //Chart1.ChartAreas["ChartArea1"].AxisY.Maximum = 2.0;

            GridViewGains.DataSource = table_graph;
            //GridView1.DataSource = q;
            GridViewGains.DataBind();

            DataTable table_counts = new DataTable();
            table_counts.Columns.Add("DateCalibrated", typeof(DateTime));

            foreach (ListItem machine in CheckBoxListMachines.Items)
                if (machine.Selected)
                    table_counts.Columns.Add(machine.Text, typeof(int));

            DateTime start = DateTime.Parse(this.txtDateTimeStart.Text);
            DateTime end = DateTime.Parse(this.txtDateTimeEnd.Text);
            int count_total = 0;
            for (DateTime s = start.Date; s < end; )
            {
                DateTime e = s + new TimeSpan(1, 0, 0);

                DataRow row_count = table_counts.NewRow();
                row_count["DateCalibrated"] = s;

                int total_count_for_time_slot = 0;
                foreach (ListItem machine in CheckBoxListMachines.Items)
                {
                    if (machine.Selected)
                    {
                        string filter = string.Format("DateCalibrated >= '{0} ' and DateCalibrated < '{1}' and MachineId={2}",
                            s.ToString(), e.ToString(), machine.Value);
                        int count = (int)table_results_db.Compute("Count(id)", filter);
                        if (count > 0)
                        {
                            row_count[machine.Text] = count;
                            count_total += count;
                            total_count_for_time_slot += count;
                        }
                    }
                }
                if (total_count_for_time_slot > 0)
                    table_counts.Rows.Add(row_count);
                s += new TimeSpan(1, 0, 0);
            }

            table_counts.Columns.Add("Total");
            foreach (DataRow r in table_counts.Rows)
            {
                int total = 0;
                foreach (ListItem machine in CheckBoxListMachines.Items)
                {
                    if (machine.Selected)
                    {
                        var v = r[machine.Text];
                        if (v != DBNull.Value)
                            total += Convert.ToInt32(v);
                    }
                }
                r["Total"] = total;
            }

            GridViewCounts.DataSource = table_counts;
            GridViewCounts.DataBind();

            ChartCounts.Series.Clear();
            ChartCounts.Legends.Add("Legend");
            ChartCounts.ChartAreas["ChartArea1"].AxisY.MinorGrid.Enabled = true;

            foreach (ListItem machine in CheckBoxListMachines.Items)
            {
                if (machine.Selected)
                {
                    series_name = machine.Text;
                    ChartCounts.Series.Add(series_name);
                    ChartCounts.Series[series_name].Points.DataBind(table_counts.AsEnumerable(), "DateCalibrated", series_name, "");
                    ChartCounts.Series[series_name].ChartType = SeriesChartType.StackedColumn;
                    ChartCounts.Series[series_name].YValuesPerPoint = 1;
                    ChartCounts.Series[series_name].YValueType = ChartValueType.Int32;
                    ChartCounts.Series[series_name].XValueType = ChartValueType.Time;
                    ChartCounts.Series[series_name].IsValueShownAsLabel = true;
                }
            }
            
            ChartCounts.ToolTip = "Total = " + count_total.ToString();

        }

        protected void ButtonSubtractDay_Click(object sender, EventArgs e)
        {
            DateTime start = DateTime.Parse(txtDateTimeStart.Text);
            start = start.Date - new TimeSpan(1, 0, 0, 0);
            txtDateTimeStart.Text = start.ToString();

            updateData();
        }

        protected void ButtonAddDay_Click(object sender, EventArgs e)
        {
            DateTime start = DateTime.Parse(txtDateTimeStart.Text);
            start = start.Date + new TimeSpan(1, 0, 0, 0);
            txtDateTimeStart.Text = start.ToString();

            updateData();

        }

        protected void ButtonPreviousDay_Click(object sender, EventArgs e)
        {
            DateTime start = DateTime.Parse(txtDateTimeStart.Text);
            start = start.Date - new TimeSpan(1, 0, 0, 0);
            txtDateTimeStart.Text = start.ToString();
            DateTime end = start + new TimeSpan(1, 0, 0, 0);
            txtDateTimeEnd.Text = end.ToString();

            updateData();
        }

        protected void ButtonNextDay_Click(object sender, EventArgs e)
        {
            DateTime start = DateTime.Parse(txtDateTimeStart.Text);
            start = start.Date + new TimeSpan(1, 0, 0, 0);
            txtDateTimeStart.Text = start.ToString();
            DateTime end = start + new TimeSpan(1, 0, 0, 0);
            txtDateTimeEnd.Text = end.ToString();

            updateData();

        }

        protected void Timer1_Tick(object sender, EventArgs e)
        {
            DateTime end = DateTime.Now;
            txtDateTimeEnd.Text = end.ToString();

            updateData();
        }

        protected void CheckBoxListMachines_SelectedIndexChanged(object sender, EventArgs e)
        {
            updateData();
        }


    }
}