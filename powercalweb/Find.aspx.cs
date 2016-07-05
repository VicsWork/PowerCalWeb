using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

using System.Data;
using System.Data.SqlClient;
using System.Data.SqlTypes;

namespace PowerCalibration
{
    public partial class Find : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            this.SqlDataSourceRS.ConnectionString = Properties.Settings.Default.PowerCalibrationConnectionString;
            SqlDataSourceRS.SelectCommandType = SqlDataSourceCommandType.Text;

        }

        protected void ButtonFind_Click(object sender, EventArgs e)
        {
            string eui = TextBoxEUI.Text;
            if(eui.StartsWith("0x")){
                int id = Convert.ToInt32(eui,16);
                eui = id.ToString();
            }
            SqlDataSourceRS.SelectCommand =
                string.Format("select * from CalibrationResults where EuiId={0}", eui);
            GridViewResults.DataSourceID = SqlDataSourceRS.ID;
        }

        protected void GridViewResults_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                for (int r = 1; r < 4; r++ )
                {
                    string s = e.Row.Cells[r].Text;
                    int d = Convert.ToInt32(s);
                    e.Row.Cells[r].Text = string.Format("{0:X}", d);
                }
            }
        }
    }
}