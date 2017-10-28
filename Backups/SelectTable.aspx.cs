using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Globalization;

public partial class FL_SelectTable : System.Web.UI.Page
{
    clsDataControl objData = new clsDataControl();
    ExcelGenerate xlsGen = new ExcelGenerate();
    protected void Page_Load(object sender, EventArgs e)
    {

    }
    protected void btn_Search_Click(object sender, EventArgs e)
    {

        grd_Data.DataSource = getDataResult();
        grd_Data.DataBind();
    }
    protected void btn_Download_Click(object sender, EventArgs e)
    {
        DataTable dt_toExport = getDataResult();
        xlsGen.ReportGeneration(dt_toExport, "Result.xls", "Result", "Result", "Result");
        string filePath = HttpContext.Current.Server.MapPath(".") + "\\Exceldownload\\";
        string excelname = "Result.xls";
        StringWriter sw = new StringWriter();
        HtmlTextWriter htm = new HtmlTextWriter(sw);
        FileInfo file = new FileInfo(filePath + excelname);
        Response.Clear();
        Response.AddHeader("Content-Length", file.Length.ToString(CultureInfo.InvariantCulture));
        Response.AddHeader("Content-Disposition", "attachment;filename=\"" + (excelname) + "\"");
        Response.ContentType = "application/octet-stream";
        Response.Flush();
        Response.TransmitFile(filePath + (excelname));
        Response.End();
    }
    private DataTable getDataResult()
    {
        DataTable dt_Table = new DataTable();
        string Query = Convert.ToString(txt_Query.Text);
        dt_Table = objData.Getdata(Query);
        return dt_Table;
    }
    protected void btn_Insert_Click(object sender, EventArgs e)
    {
        bool Res = objData.InsertOrUpdateData(Convert.ToString(txt_Query.Text));
    }
}