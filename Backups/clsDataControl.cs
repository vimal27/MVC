using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Configuration;
using System.Data.SqlClient;
using System.Data;
using System.Web;
using System.IO;
using System.Web.UI;
using System.Net;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;
using System.Collections;
[Serializable]
public class clsDataControl
{
    public clsDataControl()
    {
        DynamicParameters = new Dictionary<string, object>();
        OutputParam = new Dictionary<string, SqlDBType>();
    }

    public Dictionary<string, SqlDBType> OutputParam { get; set; }
    public string con = ConfigurationManager.ConnectionStrings["KoneMOS"].ConnectionString;
    public SqlCommand comm = new SqlCommand();
    public Dictionary<string, object> DynamicParameters { get; set; }
    public enum SqlDBType
    {
        Int,
        Varchar
    }
    public DataTable Getdata(string sqlstr)
    {
        using (SqlConnection connection = new SqlConnection(con))
        {
            connection.Close();
            connection.Open();
            SqlDataAdapter da = new SqlDataAdapter(sqlstr, connection);
            DataTable dt = new DataTable();
            da.SelectCommand.CommandTimeout = 10000;
            da.Fill(dt);
            return dt;
        }
    }
    public bool DataExeuteScalar(string sqlstr)
    {
        using (SqlConnection connection = new SqlConnection(con))
        {
            comm.Connection = connection;
            comm.CommandText = sqlstr;
            connection.Open();
            comm.ExecuteScalar();
            connection.Close();
        }
        return true;
    }
    public void GirdViews(GridView grdvw, string Query)
    {
        using (SqlConnection conn = new SqlConnection(con))
        {
            SqlDataAdapter sda = null;
            try
            {
                DataTable dts = new DataTable();
                if (conn.State == ConnectionState.Closed) { conn.Open(); }
                sda = new SqlDataAdapter(Query, con);
                sda.Fill(dts);
                if (conn.State == ConnectionState.Open) { conn.Close(); }
                grdvw.DataSource = null;
                grdvw.DataSource = dts;
                grdvw.DataBind();
            }
            catch (Exception Ex)
            {

            }
            finally
            {
                sda.Dispose();
                if (conn.State == ConnectionState.Open) { conn.Close(); }
            }
        }

    }
    public string GetSingleData(string sqlstr, bool isParameter = false)
    {
        if (isParameter == true)
        {
            SqlCommand cmd = new SqlCommand();
            SqlConnection connection = new SqlConnection(con);
            cmd.Connection = connection;
            cmd.CommandText = sqlstr;
            foreach (KeyValuePair<string, object> item in DynamicParameters)
                cmd.Parameters.AddWithValue(item.Key, item.Value);
        }
        SqlDataAdapter da = new SqlDataAdapter(sqlstr, con);
        DataTable dt = new DataTable();
        da.Fill(dt);
        if (dt.Rows.Count > 0)
            return dt.Rows[0][0].ToString();
        else
            return "0";
    }

    public DataTable getTable(string query)
    {
        SqlDataAdapter sda = new SqlDataAdapter(query, con);
        DataTable dt = new DataTable();
        sda.Fill(dt);
        return dt;
    }

    public string getSingleCellData(string query)
    {
        using (SqlConnection connection = new SqlConnection(con))
        {
            SqlCommand cmd = new SqlCommand(query, connection);
            connection.Open();
            return Convert.ToString(cmd.ExecuteScalar());
        }
    }

    public bool DataAuthendication(string sqlstr)
    {
        SqlDataAdapter da = new SqlDataAdapter(sqlstr, con);
        DataTable dt = new DataTable();
        da.Fill(dt);
        if (dt.Rows.Count > 0)
            return true;
        else
            return false;
    }
    public bool DataExecute(string sqlstr)
    {
        using (SqlConnection connection = new SqlConnection(con))
        {
            comm.Connection = connection;
            comm.CommandText = sqlstr;
            connection.Open();
            comm.ExecuteNonQuery();
            connection.Close();
        }
        return true;
    }

    public bool InsertOrUpdateData(string argstrQuery, Boolean IsSp = false, Boolean IsParameter = false)
    {
        Boolean IsSaved = false;
        using (SqlConnection connection = new SqlConnection(con))
        {
            try
            {
                connection.Open();
                SqlCommand cmd = new SqlCommand();
                cmd.Connection = connection;
                cmd.CommandTimeout = 0;
                cmd.CommandText = argstrQuery;
                if (IsSp)
                    cmd.CommandType = CommandType.StoredProcedure;
                else
                    cmd.CommandType = CommandType.Text;
                if (IsParameter)
                {
                    if (DynamicParameters.Count > 0)
                    {
                        foreach (KeyValuePair<string, object> item in DynamicParameters)
                            cmd.Parameters.AddWithValue(item.Key, item.Value);
                        int i = cmd.ExecuteNonQuery();
                        if (i != 0)
                            IsSaved = true;
                    }
                }
                else
                {
                    int i = cmd.ExecuteNonQuery();
                    if (i != 0)
                        IsSaved = true;
                }
            }
            catch (SqlException ex)
            {
                if (connection != null) ((IDisposable)connection).Dispose();
            }
        }
        return IsSaved;
    }
    public DataTable GetDetails(string argstrQuery, Boolean IsSp = false, Boolean IsParameter = false, Boolean isCombo = false, string argstrIntialValue = "--Select--")
    {
        DataTable dtData = null;
        using (SqlConnection connection = new SqlConnection(con))
        {
            try
            {
                connection.Open();
                SqlCommand cmd = new SqlCommand();
                cmd.CommandText = argstrQuery;
                cmd.Connection = connection;
                if (IsSp)
                {
                    if (IsParameter)
                    {
                        if (DynamicParameters.Count > 0)
                        {
                            foreach (KeyValuePair<string, object> item in DynamicParameters)
                                cmd.Parameters.AddWithValue(item.Key, item.Value);
                        }
                    }
                    cmd.CommandType = CommandType.StoredProcedure;
                }
                else
                {
                    cmd.CommandType = CommandType.Text;
                    if (IsParameter)
                    {
                        if (DynamicParameters.Count > 0)
                        {
                            foreach (KeyValuePair<string, object> item in DynamicParameters)
                                cmd.Parameters.AddWithValue(item.Key, item.Value);
                        }
                    }
                }
                cmd.CommandTimeout = 10000;
                SqlDataReader dr = cmd.ExecuteReader();
                if (dr.HasRows)
                {
                    dtData = new DataTable();
                    dtData.Load(dr);
                    if (isCombo)
                    {
                        DataRow drs = dtData.NewRow();
                        drs[0] = "0";
                        drs[1] = argstrIntialValue;
                        dtData.Rows.InsertAt(drs, 0);
                    }
                }
            }
            catch (SqlException)
            {
                if (connection != null)
                    ((IDisposable)connection).Dispose();
            }
        }
        return dtData;
    }
    //public DataTable GetDetails(string argstrQuery, Boolean IsSp = false, Boolean IsParameter = false, Boolean isCombo = false, string argstrIntialValue = "--Select--")
    //{
    //    DataTable dtData = null;
    //    using (SqlConnection connection = new SqlConnection(con))
    //    {
    //        try
    //        {
    //            connection.Open();
    //            SqlCommand cmd = new SqlCommand();
    //            cmd.CommandText = argstrQuery;
    //            cmd.Connection = connection;
    //            if (IsSp)
    //            {
    //                if (IsParameter)
    //                {
    //                    if (DynamicParameters.Count > 0)
    //                    {
    //                        foreach (KeyValuePair<string, object> item in DynamicParameters)
    //                            cmd.Parameters.AddWithValue(item.Key, item.Value);
    //                    }
    //                }
    //                cmd.CommandType = CommandType.StoredProcedure;
    //            }
    //            else
    //                cmd.CommandType = CommandType.Text;
    //            cmd.CommandTimeout = 10000;
    //            SqlDataReader dr = cmd.ExecuteReader();
    //            if (dr.HasRows)
    //            {
    //                dtData = new DataTable();
    //                dtData.Load(dr);
    //                if (isCombo)
    //                {
    //                    DataRow drs = dtData.NewRow();
    //                    drs[0] = "0";
    //                    drs[1] = argstrIntialValue;
    //                    dtData.Rows.InsertAt(drs, 0);
    //                }
    //            }
    //        }
    //        catch (SqlException)
    //        {
    //            if (connection != null)
    //                ((IDisposable)connection).Dispose();
    //        }
    //    }
    //    return dtData;
    //}

    public void excelReport(DataTable dt)
    {
        try
        {
            string filePath = HttpContext.Current.Server.MapPath(".") + "\\Reports\\";
            if (!Directory.Exists(filePath))
                Directory.CreateDirectory(filePath + "\\Reports\\");
            string excelpath = filePath + "\\" + ("Reports" + ".xls");
            if (File.Exists(excelpath))
                File.Delete(excelpath);

            FileInfo file = new FileInfo(excelpath);
            StreamWriter writter = new StreamWriter(excelpath, true);
            writter.WriteLine("<html>");
            writter.WriteLine("<head>");
            writter.WriteLine("<body>");
            writter.WriteLine("<table border='1'><tr>");
            for (int i = 0; i < dt.Columns.Count; i++)
            {
                writter.WriteLine("<td> <b>" + dt.Columns[i].ToString().ToUpper() + "</b></td>");

            }
            writter.WriteLine("</tr>");
            for (int i1 = 0; i1 < dt.Rows.Count; i1++)
            {
                writter.WriteLine("<tr>");
                for (int i2 = 0; i2 < dt.Columns.Count; i2++)
                    writter.WriteLine("<td >" + dt.Rows[i1][i2].ToString().ToUpper() + "</td>");
                writter.WriteLine("</tr>");
            }
            writter.WriteLine("</table>");
            writter.WriteLine("</body>");
            writter.WriteLine("</head>");
            writter.WriteLine("</html>");
            writter.Close();
            HttpContext.Current.Response.ContentType = "application/ms-excel";
            HttpContext.Current.Response.AddHeader("content-disposition", "inline; filename=Records.xls");
            HttpContext.Current.Response.TransmitFile(excelpath);
            HttpContext.Current.Response.Flush();
        }
        catch (Exception e)
        { }
    }
    public string getJSON(DataTable dt)
    {
        System.Web.Script.Serialization.JavaScriptSerializer serializer = new System.Web.Script.Serialization.JavaScriptSerializer();
        List<Dictionary<string, object>> rows = new List<Dictionary<string, object>>();
        Dictionary<string, object> row;
        foreach (DataRow dr in dt.Rows)
        {
            row = new Dictionary<string, object>();
            foreach (DataColumn col in dt.Columns)
                row.Add(col.ColumnName, dr[col]);
            rows.Add(row);
        }
        return serializer.Serialize(rows);
    }
    public string GetCurrenttime()
    {
        using (SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["conn"].ConnectionString))
        {
            SqlCommand cmd = new SqlCommand("select SUBSTRING(a,12,5) from(select CONVERT(varchar(22), GETDATE(),120)a)a", con);
            con.Open();
            string ReturnValue = Convert.ToString(cmd.ExecuteScalar());
            return ReturnValue;
        }
    }

    public string GetCurrentDateTime()
    {
        using (SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["conn"].ConnectionString))
        {
            SqlCommand cmd = new SqlCommand("select CONVERT(varchar,GETDATE(),103)+' '+LEFT(CONVERT(varchar,GETDATE(),108),5)", con);
            con.Open();
            string ReturnValue = Convert.ToString(cmd.ExecuteScalar());
            return ReturnValue;
        }
    }


    public string Subtract(string Start, string End, string Break, string Extra)
    {
        DateTime t1 = DateTime.ParseExact(Start, "dd/MM/yyyy HH:mm", null);
        TimeSpan t2;
        if (End != "__/__/____ __:__")
            t2 = DateTime.ParseExact(End, "dd/MM/yyyy HH:mm", null).Subtract(t1);
        else
        {
            t2 = DateTime.ParseExact(DateTime.Now.ToString("dd/MM/yyyy HH:mm"), "dd/MM/yyyy HH:mm", null).Subtract(t1);
        }

        TimeSpan T3 = new TimeSpan(0, Convert.ToInt32(Break), 0);
        TimeSpan T4 = new TimeSpan(Convert.ToInt32(Extra.ToString().Split(':')[0]), Convert.ToInt32(Extra.ToString().Split(':')[1]), 0);
        var TotalMinite = ((t2.Subtract(T3)).Subtract(T4)).TotalMinutes;
        string TotalTime = String.Format("{0:00}", Convert.ToDouble((TotalMinite / 60).ToString().Split('.')[0])) + ":" + string.Format("{0:00}", Convert.ToDouble((TotalMinite % 60).ToString().Split('.')[0]));
        return TotalTime;

    }
    public string SubtractMin(string Start, string End, string Break, string Extra)
    {
        DateTime t1 = DateTime.ParseExact(Start, "dd/MM/yyyy HH:mm", null);
        TimeSpan t2 = DateTime.ParseExact(End, "dd/MM/yyyy HH:mm", null).Subtract(t1);

        TimeSpan T3 = new TimeSpan(0, Convert.ToInt32(Break), 0);
        TimeSpan T4 = new TimeSpan(Convert.ToInt32(Extra.ToString().Split(':')[0]), Convert.ToInt32(Extra.ToString().Split(':')[1]), 0);
        var TotalMinite = ((t2.Subtract(T3)).Subtract(T4)).TotalMinutes;
        //string TotalTime = String.Format("{0:00}", Convert.ToDouble((TotalMinite / 60).ToString().Split('.')[0])) + ":" + string.Format("{0:00}", Convert.ToDouble((TotalMinite % 60).ToString().Split('.')[0]));
        return Convert.ToString(TotalMinite);
    }

    public string getSlnoGeneration(string ProjectID, string Department, string Type, string filename)
    {
        try
        {
            int DCNo = 0;
            using (SqlConnection dcscon = new SqlConnection(ConfigurationManager.ConnectionStrings["DCSconn"].ConnectionString))
            {
                if (dcscon.State == ConnectionState.Closed) dcscon.Open();
                SqlCommand cmd = new SqlCommand("Sp_DcsOnlinetest", dcscon);
                cmd.Parameters.Clear();
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@SelectedMonth", Convert.ToString(System.DateTime.Now.ToString("MM")));
                cmd.Parameters.AddWithValue("@CurDate", Convert.ToString(DateTime.Now.ToString("MM/dd/yyyy hh:mm tt")));
                cmd.Parameters.AddWithValue("@DcsDate", Convert.ToString(System.DateTime.Now.ToString("MM/dd/yyyy")));
                cmd.Parameters.AddWithValue("@ProjectID", ProjectID);
                cmd.Parameters.AddWithValue("@Department", Department.ToUpper());
                cmd.Parameters.AddWithValue("@LOLorLDS", Type);
                cmd.Parameters.AddWithValue("@IP", Convert.ToString(System.Environment.UserDomainName) + "^" + Convert.ToString(System.Environment.UserName) + "^" + Convert.ToString(Dns.GetHostName()) + "^" + Convert.ToString(Dns.GetHostEntry(Dns.GetHostName()).AddressList[0]));
                cmd.Parameters.AddWithValue("@Filename", Convert.ToString(filename));

                SqlParameter DcsNO = new SqlParameter();
                DcsNO.Direction = ParameterDirection.Output;
                DcsNO.SqlDbType = SqlDbType.BigInt;
                DcsNO.ParameterName = "DcsNO";
                cmd.Parameters.Add(@DcsNO);
                cmd.ExecuteNonQuery();
                DCNo = Convert.ToInt32(DcsNO.Value);

            }
            string DCSAutono = DCNo.ToString("0000") + " - " + DateTime.Now.ToString("MM") + " - " + DateTime.Now.ToString("yy") + " - T - " + Convert.ToString(Type);
            return DCSAutono;
        }
        catch (Exception e)
        {
            return "Error-" + e.ToString();
        }
    }
    /// <summary>
    /// Generic Method To Generate Excel Report
    /// </summary>
    /// <returns>The excel report.</returns>
    /// <param name="dt">Dt.</param>
    /// <param name="filename">Filename.</param>
    /// <param name="FolderPath">Folder path.</param>
    /// <param name="ReportName">Report name.</param>
    /// <param name="NoOfColumns">No of columns.</param>
    /// <param name="headernames">Headernames.</param>
    public List<string> generateExcelReport(DataTable dt, string filename, string FolderPath, string ReportName, int NoOfColumns, List<string> headernames)
    {
        List<string> ToGenerate = new List<string>();
        string filePath = HttpContext.Current.Server.MapPath(".") + "\\" + FolderPath + "\\";
        if (!Directory.Exists(filePath))
            Directory.CreateDirectory(filePath + "\\" + FolderPath + "\\");
        string excelpath = filePath + (filename + DateTime.Now.ToShortDateString().Replace("/", "") + ".xls");
        if (File.Exists(excelpath))
            File.Delete(excelpath);
        FileInfo file = new FileInfo(excelpath);
        string reportName = ReportName + " as on : " + DateTime.Now.ToString("dd/MM/yyyy");
        StringBuilder sb = new StringBuilder();
        sb.Append("<html>");
        sb.Append("<head>");
        sb.Append("<body>");
        sb.Append("<table border='1' style='font-family:Calibri; font-size:14px;'>");
        sb.Append("<tr><td colspan='" + NoOfColumns + "' style='text-align:center;vertical-align:middle;font-weight:bold;background-color: #ddebf7;'>" + reportName + "</td></tr>");
        sb.Append("<tr>");
        foreach (string header in headernames)
        {
            sb.Append("<td style='text-align:center;vertical-align:middle;font-weight:bold;background-color: #ddebf7;'>" + header + "</td>");
        }
        sb.Append("</tr>");
        foreach (DataRow row in dt.Rows)
        {
            sb.Append("<tr>");
            foreach (DataColumn column in dt.Columns)
            {
                sb.Append("<td style='text-align:center;vertical-align:middle;'>" + row[column].ToString().ToUpper().Replace("__:__", "NA") + "</td>");
            }
            sb.Append("</tr>");
        }
        sb.Append("</table>");
        sb.Append("</body>");
        sb.Append("</head>");
        sb.Append("</html>");
        File.WriteAllText(excelpath, sb.ToString());
        ToGenerate.Add(filePath.ToString());
        ToGenerate.Add(file.ToString());
        ToGenerate.Add(excelpath.ToString());
        return ToGenerate;
    }
    //public void AddDropDownListWithUnits(Panel Panel, string ID, string LabelForControl, Dictionary<string, string> dct_Values = null, string ControlName = "", int withMeasurementUnit = 0, string DropDownValues = "", string TextBoxModes = "", string TextBoxLength = "", string NumericRange = "0,0", int isReadOnly = 0, string page = "", string Measurement = "")
    //{
    //    //New Row
    //    HtmlTableRow tr = new HtmlTableRow();
    //    tr.Attributes.Add("align", "left");
    //    //New Column For Dropdownlist's Label 
    //    HtmlTableCell tc = new HtmlTableCell();
    //    tc.Attributes.Add("align", "left");

    //    //New Label For Dropdownlist
    //    Label lbl_New = new Label();
    //    lbl_New.ID = "lbl_" + ID;
    //    lbl_New.Text = LabelForControl;
    //    if (page != "tbl_v3_lift" && page != "tbl_LWD")
    //        lbl_New.CssClass = "Three-Dee";
    //    lbl_New.Attributes.Add("Font-Size", "Small");
    //    tc.Controls.Add(lbl_New);
    //    tr.Cells.Add(tc);
    //    switch (ControlName.ToLower())
    //    {
    //        case "label":
    //            break;
    //        case "textbox":
    //            //New Column For Dropdownlist
    //            HtmlTableCell txttc1 = new HtmlTableCell();
    //            TextBox txt_New = new TextBox();
    //            txt_New.ID = "txt_" + ID;
    //            txt_New.ClientIDMode = ClientIDMode.Static;
    //            txt_New.Style.Add("Font-Bold", "False");
    //            txt_New.Style.Add("Width", "140px");
    //            txt_New.Style.Add("font-weight", "normal");
    //            txt_New.CssClass = "DynamicControlsValidation";
    //            if (TextBoxModes.ToLower() == "numeric")
    //            {
    //                txt_New.Attributes.Add("onfocusout", "validateImage('" + ID + "','TextBox','" + Convert.ToString(NumericRange).Split('-')[0] + "','" + Convert.ToString(NumericRange).Split('-')[1] + "')");
    //            }
    //            else
    //            {
    //                txt_New.Attributes.Add("onfocusout", "validateImage('" + ID + "','TextBox','NA','NA')");
    //            }
    //            if (TextBoxModes.ToLower() == "multiline")
    //                txt_New.TextMode = TextBoxMode.MultiLine;
    //            else if (TextBoxModes.ToLower() == "indexed")
    //                txt_New.Attributes.Add("Maxlength", TextBoxLength);
    //            if (!string.IsNullOrEmpty(Measurement) || TextBoxModes.ToLower() == "numeric")
    //            {
    //                txt_New.Attributes.Add("onkeypress", "return numbers(this,event)");
    //            }
    //            txt_New.Attributes.Add("_AutoBindField", ID);
    //            txt_New.DataBind();

    //            Label lbl_Meas = new Label();
    //            lbl_Meas.ID = "lbl_" + ID + (Measurement == "" ? "NA" : Measurement);
    //            lbl_Meas.Text = Measurement;
    //            if (page != "tbl_v3_lift")
    //                lbl_Meas.CssClass = "Three-Dee";
    //            lbl_Meas.Attributes.Add("Font-Size", "Small");
    //            txttc1.Controls.Add(txt_New);
    //            txttc1.Controls.Add(lbl_Meas);
    //            tr.Cells.Add(txttc1);
    //            break;
    //        case "dropdownlist":
    //            //New Column For Dropdownlist
    //            HtmlTableCell ddltc1 = new HtmlTableCell();
    //            DropDownList ddl_New = new DropDownList();
    //            ddl_New.ID = "ddl_" + ID;
    //            ddl_New.ClientIDMode = ClientIDMode.Static;
    //            ddl_New.Style.Add("Font-Bold", "False");
    //            ddl_New.Style.Add("Width", "140px");
    //            ddl_New.Style.Add("font-weight", "normal");
    //            ddl_New.Attributes.Add("onchange", "validateImage('" + ID + "','DropDownList')");
    //            ddl_New.CssClass = "DynamicControlsValidation";
    //            ddl_New.Items.Clear();
    //            ddl_New.DataSource = dct_Values;
    //            ddl_New.DataTextField = "Value";
    //            ddl_New.DataValueField = "Key";
    //            ddl_New.DataBind();
    //            Label lbl_MeasD = new Label();
    //            lbl_MeasD.ID = "lbl_" + ID + (Measurement == "" ? "NA" : Measurement);
    //            lbl_MeasD.Text = Measurement;
    //            if (page != "tbl_v3_lift")
    //                lbl_MeasD.CssClass = "Three-Dee";
    //            lbl_MeasD.Attributes.Add("Font-Size", "Small");
    //            ddltc1.Controls.Add(ddl_New);
    //            ddltc1.Controls.Add(lbl_MeasD);
    //            tr.Cells.Add(ddltc1);
    //            break;
    //    }


    //    //New Column For Validation Images
    //    HtmlTableCell tc2 = new HtmlTableCell();

    //    //For Validation Image False
    //    Image img_New = new Image();
    //    img_New.ImageUrl = "../App_Themes/Master_Themes/images/att.png";
    //    img_New.ClientIDMode = ClientIDMode.Static;
    //    img_New.ID = "img_" + ID + "Att";
    //    img_New.Style.Add("display", "none");
    //    img_New.Style.Add("vertical-align", "middle");
    //    //img_New.ToolTip = "Select Dynamic Value";
    //    tc2.Controls.Add(img_New);

    //    //For Validation Image True
    //    Image img_New1 = new Image();
    //    img_New1.ImageUrl = "../App_Themes/Master_Themes/images/suc.png";
    //    img_New1.ClientIDMode = ClientIDMode.Static;
    //    img_New1.ID = "img_" + ID + "Suc";
    //    img_New1.Style.Add("display", "none");
    //    img_New1.Style.Add("vertical-align", "middle");
    //    tc2.Controls.Add(img_New1);
    //    tr.Controls.Add(tc2);
    //    if (withMeasurementUnit == 1)
    //    {
    //        //New Column For TextBox For Unit Of Measurement
    //        HtmlTableCell tc3 = new HtmlTableCell();

    //        //TextBox For Unit Of Measurement
    //        TextBox txt_New = new TextBox();
    //        txt_New.ID = "txt_" + ID + "Units";
    //        txt_New.Style.Add("Height", "23px");
    //        txt_New.ClientIDMode = ClientIDMode.Static;
    //        txt_New.Attributes.Add("MaxLength", "3");
    //        txt_New.Attributes.Add("_AutoBindField", ID + "Units");
    //        txt_New.Attributes.Add("onkeypress", "return Onlynumbers(event,this)");
    //        txt_New.Attributes.Add("onchange", "validateImage('" + ID + "','DropDownList')");
    //        txt_New.Style.Add("display", "block");
    //        txt_New.Style.Add("Width", "55px");
    //        txt_New.Attributes.Add("placeholder", "Units in Month");
    //        txt_New.ToolTip = "Units in Month";
    //        tc3.Controls.Add(txt_New);
    //        tr.Controls.Add(tc3);
    //    }
    //    Panel.Controls.Add(tr);
    //}
    public void AddDropDownListWithUnits(Panel Panel, string ID, string LabelForControl, Dictionary<string, string> dct_Values = null, string ControlName = "", int withMeasurementUnit = 0, string DropDownValues = "", string TextBoxModes = "", string TextBoxLength = "", string NumericRange = "0,0", int isReadOnly = 0, string page = "", string Measurement = "")
    {
        //New Row
        HtmlGenericControl div = new HtmlGenericControl("div");
        div.Attributes.Add("class", "row");
        HtmlTableRow tr = new HtmlTableRow();
        tr.Attributes.Add("align", "left");
        tr.Attributes.Add("class", "row");
        //New Column For Dropdownlist's Label 
        HtmlTableCell tc = new HtmlTableCell();
        tc.Attributes.Add("align", "left");

        //New Label For Dropdownlist
        Label lbl_New = new Label();
        lbl_New.ID = "lbl_" + ID;
        lbl_New.Text = LabelForControl;
        //if (page != "tbl_v3_lift" && page != "tbl_LWD")
        lbl_New.CssClass = "col-md-5 col-lg-5 col-sm-4 col-xs-4";
        lbl_New.Attributes.Add("Font-Size", "Small");
        tc.Controls.Add(lbl_New);
        tr.Cells.Add(tc);
        switch (ControlName.ToLower())
        {
            case "label":
                break;
            case "textbox":
                //New Column For Dropdownlist
                HtmlGenericControl divTextBox = new HtmlGenericControl("div");
                divTextBox.Attributes.Add("class", "col-lg-3 col-md-4 col-sm-4 col-xs-4");
                HtmlTableCell txttc1 = new HtmlTableCell();
                TextBox txt_New = new TextBox();
                txt_New.ID = "txt_" + ID;
                txt_New.ClientIDMode = ClientIDMode.Static;
                txt_New.Style.Add("Font-Bold", "False");
                txt_New.Style.Add("Width", "80%");
                txt_New.Style.Add("font-weight", "normal");
                txt_New.CssClass = "DynamicControlsValidation";
                if (TextBoxModes.ToLower() == "numeric")
                {
                    txt_New.Attributes.Add("onfocusout", "validateImage('" + ID + "','TextBox','" + Convert.ToString(NumericRange).Split('-')[0] + "','" + Convert.ToString(NumericRange).Split('-')[1] + "')");
                }
                else
                {
                    txt_New.Attributes.Add("onfocusout", "validateImage('" + ID + "','TextBox','NA','NA')");
                }
                if (TextBoxModes.ToLower() == "multiline")
                    txt_New.TextMode = TextBoxMode.MultiLine;
                else if (TextBoxModes.ToLower() == "indexed")
                    txt_New.Attributes.Add("Maxlength", TextBoxLength);
                if (!string.IsNullOrEmpty(Measurement) || TextBoxModes.ToLower() == "numeric")
                {
                    txt_New.Attributes.Add("onkeypress", "return numbers(this,event)");
                }
                txt_New.Attributes.Add("_AutoBindField", ID);
                txt_New.DataBind();

                Label lbl_Meas = new Label();
                lbl_Meas.ID = "lbl_" + ID + (Measurement == "" ? "NA" : Measurement);
                lbl_Meas.Text = Measurement;
                lbl_Meas.Attributes.Add("Font-Size", "Small");
                divTextBox.Controls.Add(txt_New);
                divTextBox.Controls.Add(lbl_Meas);
                txttc1.Controls.Add(divTextBox);
                tr.Controls.Add(txttc1);
                break;
            case "dropdownlist":
                //New Column For Dropdownlist
                HtmlTableCell ddltc1 = new HtmlTableCell();
                HtmlGenericControl divDropDownList = new HtmlGenericControl("div");
                divDropDownList.Attributes.Add("class", "col-lg-3 col-md-4 col-sm-4 col-xs-4");
                DropDownList ddl_New = new DropDownList();
                ddl_New.ID = "ddl_" + ID;
                ddl_New.ClientIDMode = ClientIDMode.Static;
                ddl_New.Style.Add("Font-Bold", "False");
                ddl_New.Style.Add("Width", "80%");
                ddl_New.Style.Add("font-weight", "normal");
                ddl_New.Attributes.Add("onchange", "validateImage('" + ID + "','DropDownList')");
                ddl_New.CssClass = "DynamicControlsValidation";
                ddl_New.Items.Clear();
                ddl_New.DataSource = dct_Values;
                ddl_New.DataTextField = "Value";
                ddl_New.DataValueField = "Key";
                ddl_New.DataBind();
                Label lbl_MeasD = new Label();
                lbl_MeasD.ID = "lbl_" + ID + (Measurement == "" ? "NA" : Measurement);
                lbl_MeasD.Text = Measurement;
                lbl_MeasD.Attributes.Add("Font-Size", "Small");
                divDropDownList.Controls.Add(ddl_New);
                divDropDownList.Controls.Add(lbl_MeasD);
                ddltc1.Controls.Add(divDropDownList);
                tr.Controls.Add(ddltc1);
                break;
        }


        //New Column For Validation Images
        HtmlTableCell tc2 = new HtmlTableCell();
        HtmlGenericControl divValidationImg = new HtmlGenericControl("div");
        divValidationImg.Attributes.Add("class", "col-lg-1 col-md-1 col-sm-1 col-xs-1");

        //For Validation Image False
        Image img_New = new Image();
        img_New.ImageUrl = "../App_Themes/Master_Themes/images/att.png";
        img_New.ClientIDMode = ClientIDMode.Static;
        img_New.ID = "img_" + ID + "Att";
        img_New.Style.Add("display", "none");
        img_New.Style.Add("vertical-align", "middle");
        //img_New.ToolTip = "Select Dynamic Value";
        divValidationImg.Controls.Add(img_New);
        tc2.Controls.Add(divValidationImg);

        //For Validation Image True
        Image img_New1 = new Image();
        img_New1.ImageUrl = "../App_Themes/Master_Themes/images/suc.png";
        img_New1.ClientIDMode = ClientIDMode.Static;
        img_New1.ID = "img_" + ID + "Suc";
        img_New1.Style.Add("display", "none");
        img_New1.Style.Add("vertical-align", "middle");
        divValidationImg.Controls.Add(img_New1);
        tc2.Controls.Add(divValidationImg);
        tr.Controls.Add(tc2);
        if (withMeasurementUnit == 1)
        {
            //New Column For TextBox For Unit Of Measurement
            HtmlTableCell tc3 = new HtmlTableCell();

            //TextBox For Unit Of Measurement
            TextBox txt_New = new TextBox();
            txt_New.ID = "txt_" + ID + "Units";
            txt_New.Style.Add("Height", "23px");
            txt_New.ClientIDMode = ClientIDMode.Static;
            txt_New.Attributes.Add("MaxLength", "3");
            txt_New.Attributes.Add("_AutoBindField", ID + "Units");
            txt_New.Attributes.Add("onkeypress", "return Onlynumbers(event,this)");
            txt_New.Attributes.Add("onchange", "validateImage('" + ID + "','DropDownList')");
            txt_New.Style.Add("display", "block");
            txt_New.Style.Add("Width", "55px");
            txt_New.Attributes.Add("placeholder", "Units in Month");
            txt_New.ToolTip = "Units in Month";
            txt_New.CssClass = "col-lg-2 col-md-2 col-sm-2 col-xs-2";
            tc3.Controls.Add(txt_New);
            tr.Controls.Add(tc3);
        }
        div.Controls.Add(tr);
        Panel.Controls.Add(div);
    }
    /// <summary>
    /// Insert Dynamic Values to Database
    /// </summary>
    /// <param name="panel"></param>
    /// <param name="TableName"></param>
    /// <param name="ColumnsExceptional"></param>
    /// <returns></returns>
    public Dictionary<string, string> insertDynamicValues(Panel panel, string TableName = "", string ColumnsExceptional = "")
    {
        Dictionary<string, string> result = new Dictionary<string, string>();
        string QryColumns = "";
        string QryValues = "";
        //To insert Dynamic Control's Valuse
        SqlCommand comm = new SqlCommand();
        if (panel.Controls.Count > 1)
        {
            //DataTable dt_ColumnNames = Getdata("SELECT column_name FROM Konetest.INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME = N'tbl_dynamicvalues' and column_name not in('id','Enquiryno','status','insertedDate','insertedBy','updatedDate','updatedBy')");
            //DataTable dt_ColumnNames = Getdata("SELECT column_name FROM " + DataBaseName + ".INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME = N'tbl_survey_cust' and column_name not in('ticketno','account_name','building_name','city_id','cust_contact_personn','cust_contact_no','region','salesoffice','salesdistrict','user_name','sales_group','pom_statergy','funding_sector','cust_seg','cust_type','lead_source','survey_by','lift_maintained_by','equip_no','Main_company_name','man_serialno','C4LIS1','lift_condition','lift_installed','scrap_to','reson','tokenno','price_variance','price_varience_month','C4LIS_no','reason_others','crm_no','siteloacationid','business_type','mobileno','stdcode','mobilecode','Updateddate','RevsionStatus')");
            DataTable dt_ColumnNames = Getdata("SELECT column_name FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME = N'" + TableName + "' and column_name not in(" + ColumnsExceptional + ")");
            Dictionary<string, string> dct_DynamicValues = new Dictionary<string, string>();
            //To Add All Dynamic Column Names in List
            foreach (Control control in panel.Controls)
            {
                if (control is HtmlGenericControl)
                {
                    if (Convert.ToString(((HtmlTableRow)control.Controls[0] as HtmlTableRow).Cells[1].Controls[0].Controls[0].GetType()) == "System.Web.UI.WebControls.TextBox")
                    {
                        string ID = ((HtmlTableRow)control.Controls[0] as HtmlTableRow).Cells[1].Controls[0].Controls[0].ID.Replace("txt_", "");
                        string Value = ((((HtmlTableRow)control.Controls[0] as HtmlTableRow).Cells[1].Controls[0].Controls[0]) as TextBox).Text;
                        dct_DynamicValues.Add(ID, Value);
                        QryColumns += ID + ",";
                        QryValues += "'" + Value + "',";
                    }
                    else if (Convert.ToString(((HtmlTableRow)control.Controls[0] as HtmlTableRow).Cells[1].Controls[0].Controls[0].GetType()) == "System.Web.UI.WebControls.DropDownList")
                    {
                        string ID = ((HtmlTableRow)control.Controls[0] as HtmlTableRow).Cells[1].Controls[0].Controls[0].ID.Replace("ddl_", "");
                        string Value = ((((HtmlTableRow)control.Controls[0] as HtmlTableRow).Cells[1].Controls[0].Controls[0]) as DropDownList).SelectedValue;
                        dct_DynamicValues.Add(ID, Value);
                        QryColumns += ID + ",";
                        QryValues += "'" + Value + "',";
                    }
                    if (Convert.ToString(((HtmlTableRow)control.Controls[0] as HtmlTableRow).Cells.Count) == "4")
                    {
                        string ID = ((HtmlTableRow)control.Controls[0] as HtmlTableRow).Cells[3].Controls[0].Controls[0].ID.Replace("txt_", "");
                        string Value = ((((HtmlTableRow)control.Controls[0] as HtmlTableRow).Cells[3].Controls[0].Controls[0]) as TextBox).Text;
                        dct_DynamicValues.Add(ID, Value);
                        QryColumns += ID + ",";
                        QryValues += "'" + Value + "',";
                    }
                }
            }
            QryColumns = QryColumns.TrimEnd(',');
            QryValues = QryValues.TrimEnd(',');
            //If Column not exists in Database then create new columns
            foreach (string Item in dct_DynamicValues.Keys)
            {
                bool contains = dt_ColumnNames.AsEnumerable().Any(row => Item == row.Field<String>("column_name"));
                if (contains == false)
                {
                    InsertOrUpdateData("alter table " + TableName + " add " + Item + " varchar(max)");
                    InsertOrUpdateData("EXECUTE sp_addextendedproperty N'MS_Description', '" + Item.Replace('_', ' ') + "', N'user', N'dbo', N'table', N'" + TableName + "', N'column', N'" + Item + "'");
                }
            }
            //If Database column is unused then column dropped
            foreach (DataRow dr in dt_ColumnNames.Rows)
            {

                bool contains = dct_DynamicValues.ContainsKey(Convert.ToString(dr[0]));
                if (contains == false)
                    InsertOrUpdateData("alter table " + TableName + " drop column " + dr[0] + "");
            }
            result.Add(QryColumns, QryValues);
        }
        return result;
    }
    //public Dictionary<string, string> insertDynamicValues(Panel panel, string TableName = "", string ColumnsExceptional = "")
    //{
    //    Dictionary<string, string> result = new Dictionary<string, string>();
    //    string QryColumns = "";
    //    string QryValues = "";
    //    //To insert Dynamic Control's Valuse
    //    SqlCommand comm = new SqlCommand();
    //    if (panel.Controls.Count > 1)
    //    {
    //        //DataTable dt_ColumnNames = Getdata("SELECT column_name FROM Konetest.INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME = N'tbl_dynamicvalues' and column_name not in('id','Enquiryno','status','insertedDate','insertedBy','updatedDate','updatedBy')");
    //        //DataTable dt_ColumnNames = Getdata("SELECT column_name FROM " + DataBaseName + ".INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME = N'tbl_survey_cust' and column_name not in('ticketno','account_name','building_name','city_id','cust_contact_personn','cust_contact_no','region','salesoffice','salesdistrict','user_name','sales_group','pom_statergy','funding_sector','cust_seg','cust_type','lead_source','survey_by','lift_maintained_by','equip_no','Main_company_name','man_serialno','C4LIS1','lift_condition','lift_installed','scrap_to','reson','tokenno','price_variance','price_varience_month','C4LIS_no','reason_others','crm_no','siteloacationid','business_type','mobileno','stdcode','mobilecode','Updateddate','RevsionStatus')");
    //        DataTable dt_ColumnNames = Getdata("SELECT column_name FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME = N'" + TableName + "' and column_name not in(" + ColumnsExceptional + ")");
    //        Dictionary<string, string> dct_DynamicValues = new Dictionary<string, string>();
    //        //To Add All Dynamic Column Names in List
    //        foreach (Control control in panel.Controls)
    //        {
    //            if (control is HtmlTableRow)
    //            {
    //                if (Convert.ToString(((HtmlTableRow)control as HtmlTableRow).Cells[1].Controls[0].GetType()) == "System.Web.UI.WebControls.TextBox")
    //                {
    //                    string ID = ((HtmlTableRow)control as HtmlTableRow).Cells[1].Controls[0].ID.Replace("txt_", "");
    //                    string Value = ((((HtmlTableRow)control as HtmlTableRow).Cells[1].Controls[0]) as TextBox).Text;
    //                    dct_DynamicValues.Add(ID, Value);
    //                    QryColumns += ID + ",";
    //                    QryValues += "'" + Value + "',";
    //                }
    //                else if (Convert.ToString(((HtmlTableRow)control as HtmlTableRow).Cells[1].Controls[0].GetType()) == "System.Web.UI.WebControls.DropDownList")
    //                {
    //                    string ID = ((HtmlTableRow)control as HtmlTableRow).Cells[1].Controls[0].ID.Replace("ddl_", "");
    //                    string Value = ((((HtmlTableRow)control as HtmlTableRow).Cells[1].Controls[0]) as DropDownList).SelectedValue;
    //                    dct_DynamicValues.Add(ID, Value);
    //                    QryColumns += ID + ",";
    //                    QryValues += "'" + Value + "',";
    //                }
    //                if (Convert.ToString(((HtmlTableRow)control as HtmlTableRow).Cells.Count) == "4")
    //                {
    //                    string ID = ((HtmlTableRow)control as HtmlTableRow).Cells[3].Controls[0].ID.Replace("txt_", "");
    //                    string Value = ((((HtmlTableRow)control as HtmlTableRow).Cells[3].Controls[0]) as TextBox).Text;
    //                    dct_DynamicValues.Add(ID, Value);
    //                    QryColumns += ID + ",";
    //                    QryValues += "'" + Value + "',";
    //                }
    //            }
    //        }
    //        QryColumns = QryColumns.TrimEnd(',');
    //        QryValues = QryValues.TrimEnd(',');
    //        //If Column not exists in Database then create new columns
    //        foreach (string Item in dct_DynamicValues.Keys)
    //        {
    //            bool contains = dt_ColumnNames.AsEnumerable().Any(row => Item == row.Field<String>("column_name"));
    //            if (contains == false)
    //            {
    //                InsertOrUpdateData("alter table " + TableName + " add " + Item + " varchar(max)");
    //                InsertOrUpdateData("EXECUTE sp_addextendedproperty N'MS_Description', '" + Item.Replace('_', ' ') + "', N'user', N'dbo', N'table', N'" + TableName + "', N'column', N'" + Item + "'");
    //            }
    //        }
    //        //If Database column is unused then column dropped
    //        foreach (DataRow dr in dt_ColumnNames.Rows)
    //        {

    //            bool contains = dct_DynamicValues.ContainsKey(Convert.ToString(dr[0]));
    //            if (contains == false)
    //                InsertOrUpdateData("alter table " + TableName + " drop column " + dr[0] + "");
    //        }
    //        result.Add(QryColumns, QryValues);
    //    }
    //    return result;
    //}
    /// <summary>
    /// Bind Dynamic Values to Dynamic Controls
    /// </summary>
    /// <param name="panel"></param>
    /// <param name="TokenNo"></param>
    /// <param name="OldValues"></param>
    /// <param name="TableName"></param>
    /// <param name="ColumnsExceptional"></param>
    /// <param name="TokennoColumnName"></param>
    public void setDynamicFieldValues(Panel panel, string TokenNo, bool OldValues = true, string TableName = "", string ColumnsExceptional = "", string TokennoColumnName = "")
    {
        DataTable dt_ColumnNames = Getdata("SELECT column_name FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME = N'" + TableName + "' and column_name not in(" + ColumnsExceptional + ")");
        if (dt_ColumnNames.Rows.Count > 0)
        {
            foreach (DataRow dr in dt_ColumnNames.Rows)
            {
                string DynamicValue = GetSingleData("select " + dr[0] + " from " + TableName + " where " + TokennoColumnName + "='" + TokenNo + "'");
                if (!string.IsNullOrEmpty(DynamicValue))
                {
                    foreach (Control c in panel.Controls)
                    {
                        if (c is HtmlGenericControl)
                        {
                            if ((Convert.ToString(((HtmlTableRow)c.Controls[0] as HtmlTableRow).Cells[1].Controls[0].Controls[0].GetType()) == "System.Web.UI.WebControls.TextBox") && (Convert.ToString(((HtmlTableRow)c.Controls[0] as HtmlTableRow).Cells[1].Controls[0].Controls[0].ID) == "txt_" + dr[0]))
                            {
                                TextBox cloneTextBox = ((((HtmlTableRow)c.Controls[0] as HtmlTableRow).Cells[1].Controls[0].Controls[0]) as TextBox);
                                if (cloneTextBox != null)
                                {
                                    cloneTextBox.Text = DynamicValue;
                                }
                            }
                            else if ((Convert.ToString(((HtmlTableRow)c.Controls[0] as HtmlTableRow).Cells[1].Controls[0].Controls[0].GetType()) == "System.Web.UI.WebControls.DropDownList") && (Convert.ToString(((HtmlTableRow)c.Controls[0] as HtmlTableRow).Cells[1].Controls[0].Controls[0].ID) == "ddl_" + dr[0]))
                            {
                                DropDownList cloneDropDownList = ((((HtmlTableRow)c.Controls[0] as HtmlTableRow).Cells[1].Controls[0].Controls[0]) as DropDownList);
                                if (cloneDropDownList != null)
                                {
                                    cloneDropDownList.SelectedValue = DynamicValue;
                                }
                            }
                            if (Convert.ToString(((HtmlTableRow)c.Controls[0] as HtmlTableRow).Cells.Count) == "4" && (Convert.ToString(((HtmlTableRow)c.Controls[0] as HtmlTableRow).Cells[3].Controls[0].Controls[0].ID) == "txt_" + dr[0]))
                            {
                                TextBox cloneTextBox = ((((HtmlTableRow)c.Controls[0] as HtmlTableRow).Cells[3].Controls[0].Controls[0]) as TextBox);
                                if (cloneTextBox != null)
                                {
                                    cloneTextBox.Text = DynamicValue;
                                }
                            }
                        }
                    }
                }
                else if (OldValues == false)
                {
                    foreach (Control c in panel.Controls)
                    {
                        if (c is HtmlGenericControl)
                        {
                            HtmlTableRow tr = (HtmlTableRow)c.Controls[0] as HtmlTableRow;
                            tr.Style.Add("display", "none");
                        }
                    }
                }
            }
        }
    }
    //public void setDynamicFieldValues(Panel panel, string TokenNo, bool OldValues = true, string TableName = "", string ColumnsExceptional = "", string TokennoColumnName = "")
    //{
    //    DataTable dt_ColumnNames = Getdata("SELECT column_name FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME = N'" + TableName + "' and column_name not in(" + ColumnsExceptional + ")");
    //    if (dt_ColumnNames.Rows.Count > 0)
    //    {
    //        foreach (DataRow dr in dt_ColumnNames.Rows)
    //        {
    //            string DynamicValue = GetSingleData("select " + dr[0] + " from " + TableName + " where " + TokennoColumnName + "='" + TokenNo + "'");
    //            if (!string.IsNullOrEmpty(DynamicValue))
    //            {
    //                foreach (Control c in panel.Controls)
    //                {
    //                    if (c is HtmlTableRow)
    //                    {
    //                        if ((Convert.ToString(((HtmlTableRow)c as HtmlTableRow).Cells[1].Controls[0].GetType()) == "System.Web.UI.WebControls.TextBox") && (Convert.ToString(((HtmlTableRow)c as HtmlTableRow).Cells[1].Controls[0].ID) == "txt_" + dr[0]))
    //                        {
    //                            TextBox cloneTextBox = ((((HtmlTableRow)c as HtmlTableRow).Cells[1].Controls[0]) as TextBox);
    //                            if (cloneTextBox != null)
    //                            {
    //                                cloneTextBox.Text = DynamicValue;
    //                            }
    //                        }
    //                        else if ((Convert.ToString(((HtmlTableRow)c as HtmlTableRow).Cells[1].Controls[0].GetType()) == "System.Web.UI.WebControls.DropDownList") && (Convert.ToString(((HtmlTableRow)c as HtmlTableRow).Cells[1].Controls[0].ID) == "ddl_" + dr[0]))
    //                        {
    //                            DropDownList cloneDropDownList = ((((HtmlTableRow)c as HtmlTableRow).Cells[1].Controls[0]) as DropDownList);
    //                            if (cloneDropDownList != null)
    //                            {
    //                                cloneDropDownList.SelectedValue = DynamicValue;
    //                            }
    //                        }
    //                        if (Convert.ToString(((HtmlTableRow)c as HtmlTableRow).Cells.Count) == "4" && (Convert.ToString(((HtmlTableRow)c as HtmlTableRow).Cells[3].Controls[0].ID) == "txt_" + dr[0]))
    //                        {
    //                            TextBox cloneTextBox = ((((HtmlTableRow)c as HtmlTableRow).Cells[3].Controls[0]) as TextBox);
    //                            if (cloneTextBox != null)
    //                            {
    //                                cloneTextBox.Text = DynamicValue;
    //                            }
    //                        }
    //                    }
    //                }
    //            }
    //            else if (OldValues == false)
    //            {
    //                foreach (Control c in panel.Controls)
    //                {
    //                    if (c is HtmlTableRow)
    //                    {
    //                        HtmlTableRow tr = (HtmlTableRow)c as HtmlTableRow;
    //                        tr.Style.Add("display", "none");
    //                        //if ((Convert.ToString(((HtmlTableRow)c as HtmlTableRow).Cells[1].Controls[0].GetType()) == "System.Web.UI.WebControls.TextBox") && (Convert.ToString(((HtmlTableRow)c as HtmlTableRow).Cells[1].Controls[0].ID) == "txt_" + dr[0]))
    //                        //{
    //                        //    TextBox cloneTextBox = ((((HtmlTableRow)c as HtmlTableRow).Cells[1].Controls[0]) as TextBox);
    //                        //    if (cloneTextBox != null)
    //                        //    {
    //                        //        cloneTextBox.Text = DynamicValue;
    //                        //    }
    //                        //}
    //                        //if (Convert.ToString(((HtmlTableRow)c as HtmlTableRow).Cells.Count) == "4" && (Convert.ToString(((HtmlTableRow)c as HtmlTableRow).Cells[3].Controls[0].ID) == "txt_" + dr[0]))
    //                        //{
    //                        //    TextBox cloneTextBox = ((((HtmlTableRow)c as HtmlTableRow).Cells[3].Controls[0]) as TextBox);
    //                        //    if (cloneTextBox != null)
    //                        //    {
    //                        //        cloneTextBox.Text = DynamicValue;
    //                        //    }
    //                        //}
    //                    }
    //                }
    //            }
    //        }
    //    }
    //}

    public string updateDynamicValues(Panel panel, string TableName, string columnsExceptional)
    {
        string finalString = string.Empty;
        Dictionary<string, string> dct_QueryOperations = new Dictionary<string, string>();
        //To Update Dynamic Control's Valuse
        SqlCommand comm = new SqlCommand();
        if (panel.Controls.Count > 1)
        {
            DataTable dt_ColumnNames = Getdata("SELECT column_name FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME = N'" + TableName + "' and column_name not in(" + columnsExceptional + ")");
            Dictionary<string, string> dct_DynamicValues = new Dictionary<string, string>();

            //To Add All Dynamic Column Names in List
            foreach (Control control in panel.Controls)
            {
                if (control is HtmlTableRow)
                {
                    if (Convert.ToString(((HtmlTableRow)control as HtmlTableRow).Cells[1].Controls[0].GetType()) == "System.Web.UI.WebControls.TextBox")
                    {
                        string ID = ((HtmlTableRow)control as HtmlTableRow).Cells[1].Controls[0].ID.Replace("txt_", "");
                        string Value = ((((HtmlTableRow)control as HtmlTableRow).Cells[1].Controls[0]) as TextBox).Text;
                        finalString += ID + "='" + Value + "',";
                        dct_QueryOperations.Add(ID, Value);
                    }
                    else if (Convert.ToString(((HtmlTableRow)control as HtmlTableRow).Cells[1].Controls[0].GetType()) == "System.Web.UI.WebControls.DropDownList")
                    {
                        string ID = ((HtmlTableRow)control as HtmlTableRow).Cells[1].Controls[0].ID.Replace("ddl_", "");
                        string Value = ((((HtmlTableRow)control as HtmlTableRow).Cells[1].Controls[0]) as DropDownList).SelectedValue;
                        finalString += ID + "='" + Value + "',";
                        dct_QueryOperations.Add(ID, Value);
                    }
                    if (Convert.ToString(((HtmlTableRow)control as HtmlTableRow).Cells.Count) == "4")
                    {
                        string ID = ((HtmlTableRow)control as HtmlTableRow).Cells[3].Controls[0].ID.Replace("txt_", "");
                        string Value = ((((HtmlTableRow)control as HtmlTableRow).Cells[3].Controls[0]) as TextBox).Text;
                        finalString += ID + "='" + Value + "',";
                        dct_QueryOperations.Add(ID, Value);
                    }
                }
            }
            finalString = finalString.TrimEnd(',');
            //If Column not exists in Database then create new columns
            foreach (string Item in dct_QueryOperations.Keys)
            {
                bool contains = dt_ColumnNames.AsEnumerable().Any(row => Item == row.Field<String>("column_name"));
                if (contains == false)
                {
                    InsertOrUpdateData("alter table " + TableName + " add " + Item + " varchar(max)");
                    InsertOrUpdateData("EXECUTE sp_addextendedproperty N'MS_Description', '" + Item.Replace('_', ' ') + "', N'user', N'dbo', N'table', N'" + TableName + "', N'column', N'" + Item + "'");
                }
            }
            //If Database column is unused then column dropped
            foreach (DataRow dr in dt_ColumnNames.Rows)
            {

                bool contains = dct_QueryOperations.ContainsKey(Convert.ToString(dr[0]));
                if (contains == false)
                    InsertOrUpdateData("alter table " + TableName + " drop column " + dr[0] + "");
            }
        }
        return finalString;
    }
    public bool SP_Insert_Update_Sqlcommand(SqlCommand cmdqry)
    {
        Boolean IsSaved = false;
        //try
        //{
        //    if (con.State == ConnectionState.Closed) { con.Open(); }
        //    SqlCommand cmd = new SqlCommand();
        //    cmd = cmdqry;
        //    cmd.Connection = con;
        //    cmd.ExecuteNonQuery();
        //    con.Close();
        //}
        //catch (SqlException ex)
        //{
        //    if (con != null) ((IDisposable)con).Dispose();
        //}
        return IsSaved;
    }
    public Dictionary<string, string> bindPricingDetails(DataTable dt)
    {
        Dictionary<string, string> dct_FinalString = new Dictionary<string, string>();
        Dictionary<string, string> dct_Parameters = new Dictionary<string, string>();
        dct_Parameters.Clear();
        foreach (DataRow dr in dt.Rows)
        {
            string ColumnsToSelect = "select getField from tbl_commonPriceMaster where packageName='" + dr["PackName"] + "'";
            switch (Convert.ToString(dr["PackName"]).ToLower())
            {
                case "modpak":
                    dct_Parameters.Add("WML_TableName", "tbl_WmlPriceModpak");
                    dct_Parameters.Add("Base_TableName", "tbl_BasePriceModpak");
                    dct_Parameters.Add("TokenNo", Convert.ToString(dr["Tokenno"]));
                    dct_Parameters.Add("PackageName", Convert.ToString(dr["PackName"]));
                    dct_Parameters.Add("TableForPackage", "tbl_v3_lift");
                    string ColumnsForBase = ColumnsToSelect + " and tablname='" + dct_Parameters["Base_TableName"] + "'";
                    ColumnsToSelect += " and tablname='" + dct_Parameters["WML_TableName"] + "'";
                    ColumnsToSelect = GetSingleData(ColumnsToSelect);
                    dct_Parameters.Add("FieldsinCalc", ColumnsToSelect);
                    dct_Parameters.Add("FieldsinBase", GetSingleData(ColumnsForBase));
                    dct_Parameters.Add("TokennoColumnName", "TknNo");
                    break;
                case "default":
                    break;
            }

            if (dct_Parameters.Count > 0)
            {
                #region WML Price
                DataTable dt_ValuesinPackage = Getdata("select top 1 " + dct_Parameters["FieldsinCalc"] + " from " + dct_Parameters["TableForPackage"] + " where " + dct_Parameters["TokennoColumnName"] + "='" + dct_Parameters["TokenNo"] + "'");
                string Query = "select * from " + dct_Parameters["WML_TableName"] + " where Liftmake='" + dt_ValuesinPackage.Rows[0]["Liftmake"] + "' and lifttype='" + dt_ValuesinPackage.Rows[0]["Lifttype"] + "' and no_of_floors='" + dt_ValuesinPackage.Rows[0]["no_of_floors"] + "' and Wiring='" + dt_ValuesinPackage.Rows[0]["Wiring"] + "' and mains_to_modpak=convert(int,isNull(Nullif('" + Convert.ToString(dt_ValuesinPackage.Rows[0]["mains_to_modpak"]) + "',''),'0')) and controller_to_modpak=convert(int,isNull(Nullif('" + Convert.ToString(dt_ValuesinPackage.Rows[0]["controller_to_modpak"]) + "',''),'0')) and cable='" + dt_ValuesinPackage.Rows[0]["Cable"] + "' and Truncking_Section='" + dt_ValuesinPackage.Rows[0]["Truncking_Section"] + "'";
                DataTable dt_result = Getdata(Query);
                List<string> lst_FinalID = new List<string>();
                foreach (DataRow res in dt_result.Rows)
                {
                    Dictionary<string, string> dct_CalcCheck = new Dictionary<string, string>();
                    dct_CalcCheck.Add("Travel", Convert.ToString(res["Travel"]));
                    dct_CalcCheck.Add("Pit", Convert.ToString(res["Pit"]));
                    dct_CalcCheck.Add("Head", Convert.ToString(res["Head"]));
                    dct_CalcCheck.Add("modpak_to_machine", Convert.ToString(res["modpak_to_machine"]));
                    foreach (string key in dct_CalcCheck.Keys)
                    {
                        if (Convert.ToString(dct_CalcCheck[key]).Contains("<=") == true)
                        {
                            if (Convert.ToInt32(dt_ValuesinPackage.Rows[0][key]) <= Convert.ToInt32(dct_CalcCheck[key].Replace("<=", "")))
                            {
                                if (lst_FinalID.Contains(Convert.ToString(res["ID"])) == false)
                                    lst_FinalID.Add(Convert.ToString(res["ID"]));
                            }
                            else
                            {
                                if (lst_FinalID.Contains(Convert.ToString(res["ID"])))
                                {
                                    lst_FinalID.Remove(Convert.ToString(res["ID"]));
                                }
                            }

                        }
                        else if (dct_CalcCheck[key].Contains("<") == true)
                        {
                            if (Convert.ToSingle(dt_ValuesinPackage.Rows[0][key]) < Convert.ToSingle(dct_CalcCheck[key].Replace("<=", "")))
                            {
                                if (lst_FinalID.Contains(Convert.ToString(res["ID"])) == false)
                                    lst_FinalID.Add(Convert.ToString(res["ID"]));
                            }
                            else
                            {
                                if (lst_FinalID.Contains(Convert.ToString(res["ID"])))
                                {
                                    lst_FinalID.Remove(Convert.ToString(res["ID"]));
                                }
                            }
                        }
                        else if (dct_CalcCheck[key].Contains("and") == true)
                        {
                            string minValue = dct_CalcCheck[key].Replace("and", ",").Split(',')[0];
                            string maxValue = dct_CalcCheck[key].Replace("and", ",").Split(',')[1];
                            if (Convert.ToInt32(dt_ValuesinPackage.Rows[0][key]) > Convert.ToInt32(minValue) && Convert.ToInt32(dt_ValuesinPackage.Rows[0][key]) < Convert.ToInt32(maxValue))
                            {
                                if (lst_FinalID.Contains(Convert.ToString(res["ID"])) == false)
                                    lst_FinalID.Add(Convert.ToString(res["ID"]));
                            }
                            else
                            {
                                if (lst_FinalID.Contains(Convert.ToString(res["ID"])))
                                {
                                    lst_FinalID.Remove(Convert.ToString(res["ID"]));
                                }
                            }
                        }
                    }
                }
                string finalID = Convert.ToString(lst_FinalID[0]);
                //string WMLQuery = "select * from " + dct_Parameters["WML_TableName"] + " where Liftmake='" + dt_ValuesinPackage.Rows[0]["Liftmake"] + "' and lifttype='" + dt_ValuesinPackage.Rows[0]["Lifttype"] + "' and no_of_floors='" + dt_ValuesinPackage.Rows[0]["no_of_floors"] + "' and Wiring='" + dt_ValuesinPackage.Rows[0]["Wiring"] + "' and mains_to_modpak=convert(int,isNull(Nullif('" + Convert.ToString(dt_ValuesinPackage.Rows[0]["mains_to_modpak"]) + "',''),'0'))/1000 and controller_to_modpak=convert(int,isNull(Nullif('" + Convert.ToString(dt_ValuesinPackage.Rows[0]["controller_to_modpak"]) + "',''),'0'))/1000 and cable='" + dt_ValuesinPackage.Rows[0]["Cable"] + "' and Truncking_Section='" + dt_ValuesinPackage.Rows[0]["Truncking_Section"] + "' and '" + dt_ValuesinPackage.Rows[0]["Travel"] + "'Travel";
                //DataTable dt_WMLPrice = Getdata(WMLQuery);

                #endregion
                #region Base Price
                DataTable dt_ValuesinPackageBase = Getdata("select top 1 " + dct_Parameters["FieldsinBase"] + " from " + dct_Parameters["TableForPackage"] + " where " + dct_Parameters["TokennoColumnName"] + "='" + dct_Parameters["TokenNo"] + "'");
                string QueryBase = "select ID from " + dct_Parameters["Base_TableName"] + " where Liftmake='" + dt_ValuesinPackageBase.Rows[0]["Liftmake"] + "' and lifttype='" + dt_ValuesinPackageBase.Rows[0]["Lifttype"] + "' and ExistingController='" + dt_ValuesinPackageBase.Rows[0]["controller"] + "' and Existing_Winding_unit='" + dt_ValuesinPackageBase.Rows[0]["unit_type"] + "' and Existing_Winding_unit_KW='" + Convert.ToString(dt_ValuesinPackageBase.Rows[0]["unit_KW"]) + "' and Handwheel='" + Convert.ToString(dt_ValuesinPackageBase.Rows[0]["Handwheel"]) + "'";
                DataTable dt_resultBase = Getdata(QueryBase);
                string FinalIDBase = Convert.ToString(dt_resultBase.Rows[0]["ID"]);
                #endregion
                dct_FinalString.Add(finalID, FinalIDBase);
            }
        }
        return dct_FinalString;
    }


    /// <summary>
    /// Check Others Values for Package Interlinking and update them in Child Packages
    /// </summary>
    /// <param name="Tokenno"></param>
    /// <param name="PackageName"></param>
    /// <param name="CurrentUser"></param>
    public void updateOthersLink(string Tokenno, string PackageName, string CurrentUser)
    {
        string checkExceptOthers = string.Empty;
        DataTable dt_OthersValues = Getdata("select * from tbl_packagelink where PackName!='" + PackageName + "' and packname!='Modpak' and tblcolumn in(select distinct columnname from tbl_InterlinkConditions where status=1) and packname in(select packname from tbl_packagelist where tokenno='" + Tokenno + "')");
        foreach (DataRow row in dt_OthersValues.Rows)
        {
            bool isReplace = false;
            #region Dynamic Conversion
            if (Convert.ToString(row["packname"]).ToUpper() == "RESCUE" || Convert.ToString(row["packname"]).ToUpper() == "LWD")
            {
                string checkRep = GetSingleData("select existing_Controller_type from " + row["tblname"] + " where tokenno='" + Tokenno + "'");
                if (checkRep.ToUpper() == "REPLACE")
                    isReplace = true;
            }
            if (isReplace == false)
            {
                DataTable dt_ToCheckOthers = Getdata("select columnname[tblColumn],selectColumn,packname,conditionTable,conditions,OthersColumnName,childTable[tblName],PackageColumnName from tbl_InterlinkConditions where status=1 and Type=1 and packname='" + row["packname"] + "'");
                foreach (DataRow dr in dt_ToCheckOthers.Rows)
                {
                    string Conditions = Convert.ToString(dr["conditions"]);
                    Conditions = Conditions.Replace("'\" + Tokenno + \"'", "'" + Tokenno + "'").Replace("'\" + dr[\"packname\"] + \"'", "'" + dr["packname"] + "'").Replace("+ Convert.ToString(dr[\"tblname\"]) +", Convert.ToString(dr["tblname"])).Replace("+ dr[\"tblname\"] +", Convert.ToString(dr["tblname"])).Replace("+ dr[\"packagecolumnname\"] +", Convert.ToString(dr["packagecolumnname"]));

                    if (Conditions.Contains('"'))
                        Conditions = Conditions.Replace("\"", "").Replace("'\" + dr[\"packagecolumnname\"] + \"'", "'" + dr["packagecolumnname"] + "'");
                    checkExceptOthers = GetSingleData("select " + dr["selectColumn"] + " from " + dr["conditionTable"] + " where " + Conditions);
                    if (checkExceptOthers == "0")
                    {
                        if (!string.IsNullOrEmpty(Convert.ToString(dr["OthersColumnName"])))
                        {
                            DataTable dt_checkOthersColumn = Getdata("select " + dr["OthersColumnName"] + " from " + dr["tblName"] + " where tokenno='" + Tokenno + "'");
                            foreach (DataColumn dc in dt_checkOthersColumn.Columns)
                            {
                                string ifOthersNull = GetSingleData("select isNull(Nullif(" + dc + ",''),'0') from " + dr["tblname"] + " where tokenno='" + Tokenno + "'");
                                if (ifOthersNull == "0")
                                {
                                    InsertOrUpdateData("update tbl_packagelist set statuss=0,w_Status=NULL,completed_date=null,completed_user=null where tokenno='" + Tokenno + "' and PackName='" + dr["packname"] + "'");
                                    goto EndExcept;
                                }

                            }
                        }
                    }
                    else
                    {
                        string checkColumnValue = GetSingleData("select isNull(Nullif(" + dr["tblcolumn"] + ",''),'0') from " + dr["tblname"] + " where tokenno='" + Tokenno + "'");
                        if (checkColumnValue == "0" || (checkColumnValue == "Others" && Convert.ToString(dr["tblcolumn"]).ToLower() != "car_width"))
                            InsertOrUpdateData("update tbl_packagelist set statuss=0,w_Status=NULL,completed_date=null,completed_user=null where tokenno='" + Tokenno + "' and PackName='" + dr["packname"] + "'");
                    }
                EndExcept:
                    string a = "End Current Loop";
                }
                DataTable dt_BindConditions = Getdata("select columnname[tblColumn],selectColumn,packname,conditionTable,conditions,OthersColumnName,PackageColumnName,childTable[tblName] from tbl_InterlinkConditions where status=1 and Type=2 and packname='" + row["packname"] + "'");
                foreach (DataRow dr in dt_BindConditions.Rows)
                {
                    string conditionColumn = GetSingleData("select compareColumns from tbl_Packagelink where packname='" + dr["packname"] + "' and tblcolumn='" + dr["tblColumn"] + "'");
                    //Testing Columns Bindings
                    if (conditionColumn != "0" && !string.IsNullOrEmpty(conditionColumn))
                    {
                        string checkColumn = GetSingleData("select a." + dr["SelectColumn"] + " from " + dr["conditionTable"] + " a," + dr["tblName"] + " b where " + conditionColumn + "b." + dr["tblColumn"] + " and a." + dr["PackageColumnName"] + "='" + dr["packname"] + "'  and b.tokenno='" + Tokenno + "'");
                        if (checkColumn == "0")
                        {
                            string OthersColumnName = Convert.ToString(dr["OthersColumnName"]);
                            if (!string.IsNullOrEmpty(OthersColumnName) && OthersColumnName != "0")
                            {
                                string OthersVal = GetSingleData("select " + OthersColumnName + " from " + dr["tblname"] + " where tokenno='" + Tokenno + "' and " + dr["tblColumn"] + "='Others'");
                                if (string.IsNullOrEmpty(OthersVal))
                                {
                                    InsertOrUpdateData("update tbl_packagelist set statuss=0,w_Status=NULL,completed_date=null,completed_user=null where tokenno='" + Tokenno + "' and PackName='" + dr["packname"] + "'");
                                }
                            }
                            else
                            {
                                InsertOrUpdateData("update tbl_packagelist set statuss=0,w_Status=NULL,completed_date=null,completed_user=null where tokenno='" + Tokenno + "' and PackName='" + dr["packname"] + "'");
                            }
                        }
                    }
                }
            #endregion
            }
        }
    }

    /// <summary>
    ///  To Update Process when Package Interlinking is Done
    /// </summary>
    /// <returns></returns>
    public void _updateChildPackageProcess(string Tokenno, string CurrentPackage)
    {
        DataTable dt_ChildPackages = Getdata("select stage_Table,stage_alias from tbl_Reportmaster where stage_alias in (select packname from tbl_packagelist where tokenno='" + Tokenno + "' and statuss=1 and packname not in('" + CurrentPackage + "'))");
        foreach (DataRow dr in dt_ChildPackages.Rows)
        {
            bool result = false;
            string ProcessColumns = GetSingleData("select distinct Columns = STUFF((SELECT ',' + Ltrim(Rtrim(column_name)) FROM information_schema.columns where table_name='" + Convert.ToString(dr[0]) + "' and column_name like '%_process%' FOR XML PATH('')), 1, 1, '') from information_schema.columns where table_name='" + Convert.ToString(dr[0]) + "' and Column_name like '%_process%'");
            if (ProcessColumns != "0")
            {
                DataTable dt_Values = Getdata("select " + ProcessColumns + " from " + Convert.ToString(dr[0]) + " where tokenno='" + Tokenno + "'");
                for (int i = 0; i < dt_Values.Columns.Count; i++)
                {
                    if (dt_Values.Rows[0][i].ToString().ToLower() == "c")
                    {
                        result = InsertOrUpdateData("update tbl_packagelist set ProcessType='C' where tokenno='" + Tokenno + "' and packname='" + Convert.ToString(dr[1]) + "'");
                        goto Process;
                    }
                }
            Process:
                if (result == false)
                    InsertOrUpdateData("update tbl_packagelist set ProcessType='A' where tokenno='" + Tokenno + "' and packname='" + Convert.ToString(dr[1]) + "'");
            }
        }
    }
    /// <summary>
    /// To Inactive package when atleast one dropdownlist's value is -select-
    /// </summary>
    /// <param name="Tokenno"></param>
    /// <param name="PackageName"></param>
    public void checkInactiveColumns(string Tokenno, string PackageName, List<DropDownList> lst_Controls)
    {
        foreach (Control control in lst_Controls)
        {
            if (control is DropDownList && control.Visible == true)
            {
                if ((control as DropDownList).SelectedValue == "-Select-")
                {
                    bool status = InsertOrUpdateData("update tbl_packagelist set statuss=0 where tokenno='" + Tokenno + "' and packname='" + PackageName + "'");
                    if (status == true)
                        goto endloop;
                }
            }
        }
    endloop:
        int a = 1;
    }
    public Array SPOutputParam(string _procename, bool IsSp = false, bool IsParameter = false, bool IsOutParamater = false)
    {
        SqlCommand comm;
        SqlConnection sqlcn = new SqlConnection(con);
        try
        {


            string _result;
            sqlcn.Open();
            comm = new SqlCommand(_procename, sqlcn);

            if (IsSp == true)
                comm.CommandType = CommandType.StoredProcedure;
            else
                comm.CommandType = CommandType.Text;

            //InputParaMeter
            if (IsParameter)
            {
                if (DynamicParameters.Count > 0)
                {
                    foreach (KeyValuePair<string, object> item in DynamicParameters)
                        comm.Parameters.AddWithValue(item.Key, item.Value);
                }
            }
            if (IsOutParamater)
            {
                if (OutputParam.Count > 0)
                {
                    foreach (KeyValuePair<string, SqlDBType> item in OutputParam)
                    {
                        if (item.Value == SqlDBType.Int)
                            comm.Parameters.Add(item.Key, SqlDbType.Int);
                        else
                            comm.Parameters.Add(item.Key, SqlDbType.VarChar, 50);


                        comm.Parameters[item.Key].Direction = ParameterDirection.Output;
                    }

                }
            }

            comm.ExecuteNonQuery();
            string[,] _temparray = new string[OutputParam.Count, 2];
            if (IsOutParamater)
            {
                if (OutputParam.Count > 0)
                {
                    int i = 0;
                    foreach (KeyValuePair<string, SqlDBType> item in OutputParam)
                    {
                        _result = comm.Parameters[item.Key].Value.ToString();
                        _temparray[i, 0] = item.Key;
                        _temparray[i, 1] = _result;
                        i = i + 1;
                    }
                }
            }

            return _temparray;

        }
        catch (Exception ex)
        {

            throw;
        }
        finally
        {
            sqlcn.Close();
        }
    }


}
