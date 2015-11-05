using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace WebApplication1
{
    public partial class WebForm1 : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {

        }

        protected void DropDownList1_SelectedIndexChanged(object sender, EventArgs e)
        {
            string cmd = "SELECT * FROM [Results]";
            if(DropDownList1.SelectedValue != "0")
                cmd +=  string.Format(" WHERE [machine_id] = {0}", DropDownList1.SelectedValue);
            SqlDataSource1.SelectCommand = cmd;
        }

        protected void DropDownList1_DataBinding(object sender, EventArgs e)
        {
        }

        protected void DropDownList1_DataBound(object sender, EventArgs e)
        {
            DropDownList1.Items.Add(new ListItem("All", "0"));

        }
    }
}