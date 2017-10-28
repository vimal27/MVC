using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data;
using System.Data.SqlClient;
using System.Configuration;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Threading;
using System.Threading.Tasks;

public class DAL
{
    SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["KoneMOS"].ConnectionString);
    SqlDataAdapter sda;
    public DAL()
    {
    }
    public string Logins(string username, string password)
    {
        string results = string.Empty;
        if (con.State == ConnectionState.Closed) { con.Open(); }
        SqlCommand cmd = new SqlCommand();
        cmd.Connection = con;
        cmd.CommandType = CommandType.Text;
        cmd.CommandText = "select SeqNo from UserMaster where UserName=@UName and UsrPassword=@Pass";
        cmd.Parameters.Add("@UName", SqlDbType.NVarChar).Value = username;
        cmd.Parameters.Add("@Pass", SqlDbType.NVarChar).Value = password;
        int ids = Convert.ToInt32(cmd.ExecuteScalar());
        if (con.State == ConnectionState.Open) { con.Close(); }
        return ids.ToString();
    }
    public string tokenno()
    {
        string tno = string.Empty;
        SqlCommand cmd = new SqlCommand();
        if (con.State == ConnectionState.Closed) { con.Open(); }
        //cmd = new SqlCommand("select top 1 tokenno from tbl_survey_cust order by ticketno desc", con);
        //cmd = new SqlCommand("select max(tokenno) from tbl_survey_cust  where datepart(mm,convert(datetime,updateddate))=datepart(mm,Getdate()) and datepart(yyyy,convert(datetime,updateddate))=datepart(yyyy,Getdate()) and ticketno>=(select ident_current('tbl_survey_cust')) and (RevsionStatus=0 or RevsionStatus is null)", con);
        cmd = new SqlCommand("select max(tokenno) from tbl_survey_cust  where datepart(mm,convert(datetime,updateddate))=datepart(mm,Getdate()) and datepart(yyyy,convert(datetime,updateddate))=datepart(yyyy,Getdate()) and RevsionStatus=0 and ticketno <=(select ident_current('tbl_survey_cust'))", con);
        string tk = Convert.ToString(cmd.ExecuteScalar());
        cmd.Parameters.Clear();
        cmd = new SqlCommand("select getdate()", con);
        DateTime dt = Convert.ToDateTime(cmd.ExecuteScalar());
        if (tk != "")
        {
            if (dt.Day == 1 && dt.Month != Convert.ToInt32(tk.Substring(7, 2)))
                tno = "TRB" + dt.Year + dt.Month.ToString("00") + "00001";
            else
                tno = "TRB" + dt.Year + dt.Month.ToString("00") + (Convert.ToInt32(tk.Substring(tk.Length - 4, 4)) + 1).ToString("00000");
        }
        else
            tno = "TRB" + dt.Year + dt.Month.ToString("00") + "00001";
        if (con.State == ConnectionState.Open) { con.Close(); }
        return tno;
    }



    #region DropDownList Member Function
    public int DropDownLists(DropDownList ddl, string DataTextField, string DataValueField, string Query)
    {
        int Count = 0;
        try
        {
            if (con.State == ConnectionState.Closed) { con.Open(); }
            sda = new SqlDataAdapter(Query, con);
            DataTable dts = new DataTable();
            sda.Fill(dts);
            ddl.Items.Clear();
            ddl.DataSource = null;
            ddl.SelectedValue = null;
            ddl.SelectedIndex = -1;
            if (con.State == ConnectionState.Open) { con.Close(); }
            ddl.DataSource = dts;
            ddl.DataTextField = DataTextField;
            ddl.DataValueField = DataValueField;
            ddl.DataBind();
            Count = dts.Rows.Count;
        }
        catch (Exception Ex) { Count = 0; }
        finally
        {
            sda.Dispose();
            if (con.State == ConnectionState.Open) { con.Close(); }
        }
        return Count;
    }
    public int DropDownLists(DropDownList ddl, string DataTextField, string DataValueField, string Query, DataTable dt)
    {
        int Count = 0;
        try
        {
            dt.Rows.Clear();
            if (con.State == ConnectionState.Closed) { con.Open(); }
            sda = new SqlDataAdapter(Query, con);
            sda.Fill(dt);
            if (con.State == ConnectionState.Open) { con.Close(); }
            if (dt.Rows.Count > 0)
            {
                ddl.DataSource = dt;
                ddl.DataTextField = DataTextField;
                ddl.DataValueField = DataValueField;
                ddl.DataBind();
                Count = dt.Rows.Count;
            }
        }
        catch (Exception Ex) { dt = null; Count = 0; }
        finally
        {
            sda.Dispose();
            if (con.State == ConnectionState.Open) { con.Close(); }
        }
        return Count;
    }

    public DataTable DDL(DropDownList ddl, string DataTextField, string DataValueField, string Query)
    {
        DataTable dts = new DataTable();
        try
        {
            dts.Rows.Clear();
            if (con.State == ConnectionState.Closed) { con.Open(); }
            sda = new SqlDataAdapter(Query, con);
            sda.Fill(dts);
            if (con.State == ConnectionState.Open) { con.Close(); }
            if (dts.Rows.Count > 0)
            {
                ddl.DataSource = dts;
                ddl.DataTextField = DataTextField;
                ddl.DataValueField = DataValueField;
                ddl.DataBind();
            }
        }
        catch (Exception Ex) { dts = null; }
        finally
        {
            sda.Dispose();
            if (con.State == ConnectionState.Open) { con.Close(); }
        }
        return dts;
    }
    public bool DisableVal(DropDownList ddl)
    {
        Boolean dis = false;
        try
        {

            if (ddl.Items.Count == 2)
            {
                ddl.SelectedIndex = 1;
                ddl.Enabled = false;
                dis = true;
            }
            else
            {
                if (ddl.Items.Count > 2)
                {
                    ddl.SelectedIndex = 0;
                    ddl.Enabled = true;
                    dis = false;
                 }
            }
            
        }
        catch (Exception)
        {

           // throw;
        }
        return dis;
    }
    public bool DisableVal_dataflor(DropDownList ddl)
    {
        Boolean dis = false;
        try
        {

            if (ddl.Items.Count == 2)
            {
                ddl.SelectedIndex = 1;
                ddl.Enabled = true;
                dis = true;
            }
            else
            {
                if (ddl.Items.Count > 2)
                {
                    //ddl.SelectedIndex = 0;
                    ddl.Enabled = true;
                    dis = false;
                }
            }

        }
        catch (Exception)
        {

            // throw;
        }
        return dis;
    }
    #endregion

    #region Timedelay Member Function

    public Boolean Timedelay(Int32 TimeOutVale)
    {
        Boolean timeOut = false;

        Int32 QueryTimeOut = TimeOutVale; //in ms
        Int32 result = 0;
        ManualResetEvent wait = new ManualResetEvent(false);
        Thread work = new Thread(new ThreadStart(() =>
        {
            //some long running method requiring synchronization
            result = GetResult();
            wait.Set();
        }));
        work.Start();
        Boolean signal = wait.WaitOne(QueryTimeOut);
        if (!signal)
        {
            work.Abort();
            timeOut = true;
        }
        return timeOut;
    }
    private static int GetResult()
    {
        int result = 0;
        while (++result < 1000)
        {
            Console.WriteLine(result);
            Thread.Sleep(5);
        }
        return result;
    }
    #endregion

    #region GridView Member Function
    public void GirdViews(GridView grdvw, string Query)
    {
        try
        {
            DataTable dts = new DataTable();
            if (con.State == ConnectionState.Closed) { con.Open(); }
            sda = new SqlDataAdapter(Query, con);
            sda.Fill(dts);
            if (con.State == ConnectionState.Open) { con.Close(); }
            grdvw.DataSource = null;
            grdvw.DataSource = dts;
            grdvw.DataBind();
            
        }
        catch (Exception Ex) { }
        finally
        {
            sda.Dispose();
            if (con.State == ConnectionState.Open) { con.Close(); }
        }
    }
    public void GirdViews(GridView grdvw, string Query, DataTable dt)
    {
        try
        {
            dt.Rows.Clear();
            if (con.State == ConnectionState.Closed) { con.Open(); }
            sda = new SqlDataAdapter(Query, con);
            sda.Fill(dt);
            if (con.State == ConnectionState.Open) { con.Close(); }
            grdvw.DataSource = dt;
            grdvw.DataBind();
        }
        catch (Exception Ex) { dt = null; }
        finally
        {
            sda.Dispose();
            if (con.State == ConnectionState.Open) { con.Close(); }
        }
    }
    #endregion

    #region ExecuteNonQuery Member Function
    public string AutoPackage(string packname,string ddlvalue)
    {
        SqlCommand cmd = new SqlCommand();
        string Result = string.Empty;
        try
        {
            cmd = new SqlCommand();
            cmd.Parameters.Clear();
            cmd.Connection = con;
            cmd.CommandType = CommandType.Text;
        }
        catch (Exception)
        {
            
            throw;
        }
        finally
        {
            cmd.Dispose();
            if (con.State == ConnectionState.Open) { con.Close(); }
        }
        return Result;

    }


    public string ExecuteNonQuery(string CommandText, CommandType CommandType, SqlParameter[] SQLParameter)
    {
        SqlCommand cmd = new SqlCommand();
        string Result = string.Empty;
        try
        {
            cmd = new SqlCommand();
            cmd.Parameters.Clear();
            cmd.Connection = con;
            cmd.CommandType = CommandType;
            cmd.CommandText = CommandText;
            cmd.Parameters.AddRange(SQLParameter);
            if (con.State == ConnectionState.Closed) { con.Open(); }
            Result = Convert.ToString(cmd.ExecuteNonQuery());
            if (con.State == ConnectionState.Open) { con.Close(); }
        }
        catch (Exception Ex) { Result = Ex.Message; }
        finally
        {
            cmd.Dispose();
            if (con.State == ConnectionState.Open) { con.Close(); }
        }
        return Result;
    }
    public string ExecuteNonQuery(string CommandText, CommandType CommandType)
    {
        SqlCommand cmd = new SqlCommand();
        string Result = string.Empty;
        try
        {
            cmd = new SqlCommand();
            cmd.Parameters.Clear();
            cmd.Connection = con;
            cmd.CommandType = CommandType;
            cmd.CommandText = CommandText;
            if (con.State == ConnectionState.Closed) { con.Open(); }
            Result = Convert.ToString(cmd.ExecuteNonQuery());
            if (con.State == ConnectionState.Open) { con.Close(); }
        }
        catch (Exception Ex) { Result = Ex.Message; }
        finally
        {
            cmd.Dispose();
            if (con.State == ConnectionState.Open) { con.Close(); }
        }
        return Result;
    }
    public string ExecuteNonQuery(string CommandText, SqlParameter[] SQLParameter)
    {
        SqlCommand cmd = new SqlCommand();
        string Result = string.Empty;
        try
        {
            cmd = new SqlCommand();
            cmd.Parameters.Clear();
            cmd.Connection = con;
            cmd.CommandText = CommandText;
            cmd.Parameters.AddRange(SQLParameter);
            if (con.State == ConnectionState.Closed) { con.Open(); }
            Result = Convert.ToString(cmd.ExecuteNonQuery());
            if (con.State == ConnectionState.Open) { con.Close(); }
        }
        catch (Exception Ex) { Result = Ex.Message; }
        finally
        {
            cmd.Dispose();
            if (con.State == ConnectionState.Open) { con.Close(); }
        }
        return Result;
    }
    public string ExecuteNonQuery(string CommandText)
    {
        SqlCommand cmd = new SqlCommand();
        string Result = string.Empty;
        try
        {
            cmd = new SqlCommand();
            cmd.Parameters.Clear();
            cmd.Connection = con;
            cmd.CommandText = CommandText;
            if (con.State == ConnectionState.Closed) { con.Open(); }
            Result = Convert.ToString(cmd.ExecuteNonQuery());
            if (con.State == ConnectionState.Open) { con.Close(); }
        }
        catch (Exception Ex) { Result = Ex.Message; }
        finally
        {
            cmd.Dispose();
            if (con.State == ConnectionState.Open) { con.Close(); }
        }
        return Result;
    }
    public string ExecuteNonQueryPackLink(string Tokenno,string Packname,string userid)
    {
        SqlCommand cmd = new SqlCommand();
        string Result = string.Empty;
        try
        {
            cmd = new SqlCommand();
            string Sql = "Sp_PackLinkUpdation";
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Connection = con;
            cmd.CommandText = Sql;
            cmd.Parameters.Clear();
            cmd.Parameters.Add("@Tokenno", Tokenno);
            cmd.Parameters.Add("@Currentpackagename", Packname);
            cmd.Parameters.Add("@userid", userid);
            if (con.State == ConnectionState.Closed) { con.Open(); }
            //cmd.CommandTimeout = 3000;
            Result = Convert.ToString(cmd.ExecuteNonQuery());
            if (con.State == ConnectionState.Open) { con.Close(); }
        }
        catch (Exception Ex) { Result = Ex.Message; }
        finally
        {
            cmd.Dispose();
            if (con.State == ConnectionState.Open) { con.Close(); }
        }
        return Result;
    }
    public string ExecuteNonQueryUnwantPackageData(string Tokenno, string Packname)
    {
        SqlCommand cmd = new SqlCommand();
        string Result = string.Empty;
        try
        {
            cmd = new SqlCommand();
            cmd.CommandText="Select tblname from tbl_deletePackage where packname='"+Packname +"'";
            

        }
        catch (Exception)
        {
            
            throw;
        }
        finally
        {
            cmd.Dispose();
            if (con.State == ConnectionState.Open) { con.Close(); }
        }
        return Result;
        
    }
    #endregion

    #region ExecuteScalar Member Function
    public string ExecuteScalar(string CommandText, CommandType CommandType, SqlParameter[] SQLParameter)
    {
        SqlCommand cmd = new SqlCommand();
        string Result = string.Empty;
        try
        {
            cmd = new SqlCommand();
            cmd.Parameters.Clear();
            cmd.Connection = con;
            cmd.CommandType = CommandType;
            cmd.CommandText = CommandText;
            cmd.Parameters.AddRange(SQLParameter);
            if (con.State == ConnectionState.Closed) { con.Open(); }
            Result = Convert.ToString(cmd.ExecuteScalar());
            if (con.State == ConnectionState.Open) { con.Close(); }
        }
        catch (Exception Ex) { Result = Ex.Message; }
        finally
        {
            cmd.Dispose();
            if (con.State == ConnectionState.Open) { con.Close(); }
        }
        return Result;
    }
    public string ExecuteScalar(string CommandText, CommandType CommandType)
    {
        SqlCommand cmd = new SqlCommand();
        string Result = string.Empty;
        try
        {
            cmd = new SqlCommand();
            cmd.Parameters.Clear();
            cmd.Connection = con;
            cmd.CommandType = CommandType;
            cmd.CommandText = CommandText;
            if (con.State == ConnectionState.Closed) { con.Open(); }
            Result = Convert.ToString(cmd.ExecuteScalar());
            if (con.State == ConnectionState.Open) { con.Close(); }
        }
        catch (Exception Ex) { Result = Ex.Message; }
        finally
        {
            cmd.Dispose();
            if (con.State == ConnectionState.Open) { con.Close(); }
        }
        return Result;
    }
    public string ExecuteScalar(string CommandText, SqlParameter[] SQLParameter)
    {
        SqlCommand cmd = new SqlCommand();
        string Result = string.Empty;
        try
        {
            cmd = new SqlCommand();
            cmd.Parameters.Clear();
            cmd.Connection = con;
            cmd.CommandText = CommandText;
            cmd.Parameters.AddRange(SQLParameter);
            if (con.State == ConnectionState.Closed) { con.Open(); }
            Result = Convert.ToString(cmd.ExecuteScalar());
            if (con.State == ConnectionState.Open) { con.Close(); }
        }
        catch (Exception Ex) { Result = Ex.Message; }
        finally
        {
            cmd.Dispose();
            if (con.State == ConnectionState.Open) { con.Close(); }
        }
        return Result;
    }
    public string ExecuteScalar(string CommandText)
    {
        SqlCommand cmd = new SqlCommand();
        string Result = string.Empty;
        try
        {
            cmd = new SqlCommand();
            cmd.Parameters.Clear();
            cmd.Connection = con;
            cmd.CommandText = CommandText;
            if (con.State == ConnectionState.Closed) { con.Open(); }
            Result = Convert.ToString(cmd.ExecuteScalar());
            if (con.State == ConnectionState.Open) { con.Close(); }
        }
        catch (Exception Ex) { Result = Ex.Message; }
        finally
        {
            cmd.Dispose();
            if (con.State == ConnectionState.Open) { con.Close(); }
        }
        return Result;
    }
    #endregion

    #region ExecuteDataSet Members Function
    public DataTable ExecuteDataSet(string CommandText)
    {
        SqlDataAdapter sda = new SqlDataAdapter();
        DataTable dts = new DataTable();
        string Result = string.Empty;
        try
        {
            dts.Rows.Clear();
            if (con.State == ConnectionState.Closed) { con.Open(); }
            sda = new SqlDataAdapter(CommandText, con);
            sda.Fill(dts);
            if (con.State == ConnectionState.Open) { con.Close(); }
            if (dts.Rows.Count == 0) { dts = null; }
        }
        catch (Exception Ex) { dts = null; }
        finally
        {
            sda.Dispose();
            if (con.State == ConnectionState.Open) { con.Close(); }
        }
        return dts;
    }
    public DataTable ExecuteDataSet(string CommandText, CommandType CommandType)
    {
        SqlDataAdapter sda = new SqlDataAdapter();
        DataTable dts = new DataTable();
        SqlCommand cmd = new SqlCommand();
        string Result = string.Empty;
        try
        {
            dts.Rows.Clear();
            cmd = new SqlCommand();
            cmd.Parameters.Clear();
            cmd.Connection = con;
            cmd.CommandType = CommandType;
            cmd.CommandText = CommandText;
            if (con.State == ConnectionState.Closed) { con.Open(); }
            sda = new SqlDataAdapter(cmd);
            sda.Fill(dts);
            if (con.State == ConnectionState.Open) { con.Close(); }
            if (dts.Rows.Count == 0) { dts = null; }
        }
        catch (Exception Ex) { dts = null; }
        finally
        {
            cmd.Dispose();
            sda.Dispose();
            if (con.State == ConnectionState.Open) { con.Close(); }
        }
        return dts;
    }
    public DataTable ExecuteDataSet(string CommandText, CommandType CommandType, SqlParameter[] parameter)
    {
        SqlDataAdapter sda = new SqlDataAdapter();
        DataTable dts = new DataTable();
        SqlCommand cmd = new SqlCommand();
        string Result = string.Empty;
        try
        {
            dts.Rows.Clear();
            cmd = new SqlCommand();
            cmd.Parameters.Clear();
            cmd.Connection = con;
            cmd.CommandType = CommandType;
            cmd.CommandText = CommandText;
            cmd.Parameters.AddRange(parameter);
            if (con.State == ConnectionState.Closed) { con.Open(); }
            sda = new SqlDataAdapter(cmd);
            sda.Fill(dts);
            if (con.State == ConnectionState.Open) { con.Close(); }
            if (dts.Rows.Count == 0) { dts = null; }
        }
        catch (Exception Ex) { dts = null; }
        finally
        {
            cmd.Dispose();
            sda.Dispose();
            if (con.State == ConnectionState.Open) { con.Close(); }
        }
        return dts;
    }

    public DataTable ExecutePackageDescription(string tablename)
    {
        SqlDataAdapter sda = new SqlDataAdapter();
        DataTable dts = new DataTable();
        SqlCommand cmd = new SqlCommand();
        string Result = string.Empty;
        try
        {
            dts.Rows.Clear();
            cmd = new SqlCommand();
            cmd.Parameters.Clear();
            cmd.Connection = con;
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.CommandText = "Sp_createdesc";
            cmd.Parameters.AddWithValue("@tablename", tablename);            
            if (con.State == ConnectionState.Closed) { con.Open(); }
            sda = new SqlDataAdapter(cmd);
            sda.Fill(dts);
            if (con.State == ConnectionState.Open) { con.Close(); }
            if (dts.Rows.Count == 0) { dts = null; }
        }
        catch (Exception Ex) { dts = null; }
        finally
        {
            cmd.Dispose();
            sda.Dispose();
            if (con.State == ConnectionState.Open) { con.Close(); }
        }
        return dts;
    }
    public DataTable ExecuteDataSet(string CommandText, SqlParameter[] parameter)
    {
        SqlDataAdapter sda = new SqlDataAdapter();
        DataTable dts = new DataTable();
        SqlCommand cmd = new SqlCommand();
        string Result = string.Empty;
        try
        {
            dts.Rows.Clear();
            cmd = new SqlCommand();
            cmd.Parameters.Clear();
            cmd.Connection = con;
            cmd.CommandText = CommandText;
            cmd.Parameters.AddRange(parameter);
            if (con.State == ConnectionState.Closed) { con.Open(); }
            sda = new SqlDataAdapter(cmd);
            sda.Fill(dts);
            if (con.State == ConnectionState.Open) { con.Close(); }
            if (dts.Rows.Count == 0) { dts = null; }
        }
        catch (Exception Ex) { dts = null; }
        finally
        {
            cmd.Dispose();
            sda.Dispose();
            if (con.State == ConnectionState.Open) { con.Close(); }
        }
        return dts;
    }
    #endregion
}