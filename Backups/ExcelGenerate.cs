using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.IO;
using NPOI.HSSF.UserModel;
using NPOI.HPSF;
using NPOI.POIFS.FileSystem;
using NPOI.SS.UserModel;
using System.Text;
using NPOI.SS.Util;
using System.Data;
using System.Configuration;

/// <summary>
/// Summary description for ExcelGenerate
/// </summary>
public class ExcelGenerate
{      
	public ExcelGenerate()
	{        
	}
    public string ReportGeneration(DataTable dt, string filename, string headername, string sheetname, string Username,bool InitializeRequired=false)
    {
        try
        {
            if (dt != null)
            {
                if (dt.Rows.Count > 0)
                {
                    if(InitializeRequired==true)
                        InitializeWorkbook(headername, Username);
                    GenerateData(headername, sheetname, dt, Username);
                    WriteToFile(filename);
                    return "Excel Downloaded";
                }
                else
                {
                    return "No Records Found";
                }
            }
            else
            {
                return "Object is null";
            }
        }
        catch(Exception ee)
        {
            return Convert.ToString(ee.Message); 
        }
    }
    static HSSFWorkbook hssfworkbook;

    static void WriteToFile(string filename)
    {

        string filePath = HttpContext.Current.Server.MapPath(".") + "\\Exceldownload\\";
        if (!Directory.Exists(filePath))
            Directory.CreateDirectory(filePath + "\\Exceldownload\\");
        string excelpath = filePath + "\\" + filename;
        if (File.Exists(excelpath))
            File.Delete(excelpath);
        FileStream file = new FileStream(excelpath, FileMode.Create);
        hssfworkbook.Write(file);
        file.Close();
        //DownloadFile(filename);
        
    }
    
    void GenerateData(string headername, string sheetname, DataTable dt,string Username)
    {
        ISheet sheet1 = hssfworkbook.CreateSheet(sheetname);
        sheet1.DisplayGridlines = true;
        CellRangeAddress region = new CellRangeAddress(0, 0, 0, dt.Columns.Count - 1);
        IRow row = sheet1.CreateRow(0);
        row.HeightInPoints = 20;

        ICell cell = row.CreateCell(0);
        ICellStyle style1 = hssfworkbook.CreateCellStyle();        
        style1.FillForegroundColor = NPOI.HSSF.Util.HSSFColor.PaleBlue.Index;
        style1.FillPattern = FillPattern.SolidForeground;
        style1.Alignment = HorizontalAlignment.Center;
		style1.VerticalAlignment = VerticalAlignment.Center;
        style1.WrapText = true;
        //style1.ShrinkToFit = true;
        HSSFFont hFont = (HSSFFont)hssfworkbook.CreateFont();
        hFont.FontHeightInPoints = 11;
        hFont.FontName = "Calibri";
        hFont.Boldweight = (short)NPOI.SS.UserModel.FontBoldWeight.Bold;

        HSSFFont hFont2 = (HSSFFont)hssfworkbook.CreateFont();
        hFont2.FontHeightInPoints = 11;
        hFont2.FontName = "Calibri";
      

        HSSFCellStyle hStyle = (HSSFCellStyle)hssfworkbook.CreateCellStyle();
       
        hStyle.FillForegroundColor = NPOI.HSSF.Util.HSSFColor.LightGreen.Index;
        hStyle.FillPattern = FillPattern.SolidForeground;
        hStyle.Alignment = HorizontalAlignment.Left;
        hStyle.BorderBottom = NPOI.SS.UserModel.BorderStyle.Thin;
        hStyle.BottomBorderColor = NPOI.HSSF.Util.HSSFColor.Black.Index;
        hStyle.BorderLeft = NPOI.SS.UserModel.BorderStyle.Thin;
        hStyle.LeftBorderColor = NPOI.HSSF.Util.HSSFColor.Green.Index;
        hStyle.BorderRight = NPOI.SS.UserModel.BorderStyle.Thin;
        hStyle.RightBorderColor = NPOI.HSSF.Util.HSSFColor.Blue.Index;
        hStyle.BorderTop = NPOI.SS.UserModel.BorderStyle.Thin;
        hStyle.TopBorderColor = NPOI.HSSF.Util.HSSFColor.Orange.Index;
        hStyle.SetFont(hFont);
        hStyle.WrapText = true;
        //hStyle.ShrinkToFit = true;

        
        HSSFCellStyle hStyle2 = (HSSFCellStyle)hssfworkbook.CreateCellStyle();
      
        
        hStyle2.FillForegroundColor = NPOI.HSSF.Util.HSSFColor.PaleBlue.Index;
        hStyle2.FillPattern = FillPattern.SolidForeground;
        hStyle2.Alignment = HorizontalAlignment.Center;
		hStyle2.VerticalAlignment = VerticalAlignment.Center;
        hStyle2.BorderBottom = NPOI.SS.UserModel.BorderStyle.Thin;
        hStyle2.BottomBorderColor = NPOI.HSSF.Util.HSSFColor.Black.Index;
        hStyle2.BorderLeft = NPOI.SS.UserModel.BorderStyle.Thin;
        hStyle2.LeftBorderColor = NPOI.HSSF.Util.HSSFColor.Green.Index;
        hStyle2.BorderRight = NPOI.SS.UserModel.BorderStyle.Thin;
        hStyle2.RightBorderColor = NPOI.HSSF.Util.HSSFColor.Blue.Index;
        hStyle2.BorderTop = NPOI.SS.UserModel.BorderStyle.Thin;        
        hStyle2.TopBorderColor = NPOI.HSSF.Util.HSSFColor.Orange.Index;
        hStyle2.SetFont(hFont);
        hStyle.WrapText = true;
       // hStyle.ShrinkToFit = true;

        IFont font = hssfworkbook.CreateFont();
        font.FontHeight = 15 * 15;
        font.Boldweight = 2;
        cell.CellStyle = hStyle2;
        
        cell.SetCellValue(headername);


        string organizationanme=Convert.ToString(ConfigurationManager.AppSettings["Client"]);

        sheet1.Header.Right = sheetname;
        sheet1.Footer.Left = "Copyright @" + organizationanme;
        sheet1.Footer.Right = "Created by " + Convert.ToString(Username);
     

      

        row = sheet1.CreateRow(1);

        for (int i = 0; i <= dt.Columns.Count-1; i++)
        {
			hStyle.Alignment = HorizontalAlignment.Left;
			hStyle.VerticalAlignment = VerticalAlignment.Center;
            cell = row.CreateCell(i); cell.CellStyle = hStyle; cell.SetCellValue(Convert.ToString(i==0?"Column Name":"Column Value"));
            //cell.CellStyle.ShrinkToFit = true;
            cell.CellStyle.WrapText = true;
            
        }
		       
        hStyle = (HSSFCellStyle)hssfworkbook.CreateCellStyle();        
        hStyle.FillPattern = FillPattern.SolidForeground;
        hStyle.Alignment = HorizontalAlignment.Left;
        hStyle.BorderBottom = NPOI.SS.UserModel.BorderStyle.Thin;
        hStyle.BottomBorderColor = NPOI.HSSF.Util.HSSFColor.Black.Index;
        hStyle.BorderLeft = NPOI.SS.UserModel.BorderStyle.Thin;
        hStyle.LeftBorderColor = NPOI.HSSF.Util.HSSFColor.Green.Index;
        hStyle.BorderRight = NPOI.SS.UserModel.BorderStyle.Thin;
        hStyle.RightBorderColor = NPOI.HSSF.Util.HSSFColor.Blue.Index;
        hStyle.BorderTop = NPOI.SS.UserModel.BorderStyle.Thin;
        hStyle.TopBorderColor = NPOI.HSSF.Util.HSSFColor.Orange.Index;
        hStyle.FillForegroundColor = NPOI.HSSF.Util.HSSFColor.White.Index;
        hStyle.FillPattern = FillPattern.SolidForeground;
        hStyle.SetFont(hFont2);
       // hStyle.ShrinkToFit = true;
        hStyle.WrapText = true;
		hStyle.Alignment = HorizontalAlignment.Center;
		hStyle.VerticalAlignment = VerticalAlignment.Center;
		style1.Alignment = HorizontalAlignment.Center;
		style1.VerticalAlignment = VerticalAlignment.Center;
		string CurrentName="";
		string previousName="";
		int Start = 0;
		int End = 0;
        if (dt != null)
        {
            int x = 2;
            for (int i = 3; i < dt.Rows.Count; i++)
            {
                row = sheet1.CreateRow(x);
                for (int j = 0; j <= dt.Columns.Count - 1; j++)
                {
                    //if (j == 0)
                    //{
                    //    cell = row.CreateCell(j); cell.CellStyle = hStyle; cell.SetCellValue(Convert.ToString(dt.Rows[i][j]));
                    //    //cell.CellStyle.ShrinkToFit = true;
                    //    cell.CellStyle.WrapText = true;
                    //}
                    //else if(j==1)
                    //{
                    //    cell = row.CreateCell(j); cell.CellStyle = hStyle; cell.SetCellValue(Convert.ToString(dt.Rows[i][j]));
                    //    CurrentName = cell.ToString();
                    //    if (CurrentName == previousName) {
                    //        Start = i;
                    //    } 
                    //    else 
                    //    {
                    //        End = i;
                    //    }
                    //    previousName = cell.ToString();
                    //    if(End!=0)
                    //    {
                    //        sheet1.AddMergedRegion (new CellRangeAddress(Start,End,1,1));
                    //    }
                    //   // cell.CellStyle.ShrinkToFit = true;
                    //}
                    //else
                    //{
                    hStyle.Alignment = HorizontalAlignment.Left;
                    hStyle.VerticalAlignment = VerticalAlignment.Center;
                        cell = row.CreateCell(j); cell.CellStyle = hStyle; cell.SetCellValue(Convert.ToString(dt.Rows[i][j]));
                      //  cell.CellStyle.ShrinkToFit = true;
                        cell.CellStyle.WrapText = true;
                    //}

                }
                x++;
            }
        }
        for (int j = 0; j <= dt.Columns.Count - 1; j++)
                {
        sheet1.AutoSizeColumn(j);  
        }
        sheet1.AddMergedRegion(region);
        ((HSSFSheet)sheet1).SetEnclosedBorderOfRegion(region, NPOI.SS.UserModel.BorderStyle.Double, NPOI.HSSF.Util.HSSFColor.DarkTeal.Index);


    }

    void InitializeWorkbook(string headername, string Username)
    {
        hssfworkbook = new HSSFWorkbook();
        ////create a entry of DocumentSummaryInformation
        DocumentSummaryInformation dsi = PropertySetFactory.CreateDocumentSummaryInformation();

        string organizationanme = Convert.ToString(ConfigurationManager.AppSettings["Client"]);
        string portalname = Convert.ToString(ConfigurationManager.AppSettings["portalname"]);

        dsi.Company = "@" + organizationanme + " - " + portalname;
        hssfworkbook.DocumentSummaryInformation = dsi;
        SummaryInformation si = PropertySetFactory.CreateSummaryInformation();
        si.Subject = headername;
        si.Author = Convert.ToString(Username);
        si.Comments = headername + " Created On " + DateTime.Now.ToString("dd-MM-yyyy") + " " + DateTime.Now.ToString("hh:mm:ss tt") + " @System generated Report";
        hssfworkbook.SummaryInformation = si;
    }

    //public string ReportGeneration(DataSet ds, string p, string p_2, string p_3, string p_4)
    //{
    //    throw new NotImplementedException();
    //}
}