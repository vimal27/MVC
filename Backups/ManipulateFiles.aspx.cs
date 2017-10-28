using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

public partial class FL_ManipulateFiles : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
    }
    protected void btn_UploadFiles_Click(object sender, EventArgs e)
    {
        WebClient client = new WebClient();
        client.Credentials = CredentialCache.DefaultCredentials;
        string filePath = string.Empty;
        if (upl_files.HasFile)
        {
            try
            {
                //switch (Convert.ToString(rbl_Path.SelectedValue).ToLower())
                //{
                //    case "css":
                //        filePath = Server.MapPath("~") + "App_Themes\\Master_Themes\\css\\" + upl_files.FileName;
                //        break;
                //    case "js":
                //        filePath = Server.MapPath("~") + "App_Themes/Master_Themes/js/" + upl_files.FileName;
                //        break;
                //    case "images":
                //        filePath = Server.MapPath("~") + "App_Themes/Master_Themes/images/" + upl_files.FileName;
                //        break;
                //    case "flmaster":
                //        filePath = Server.MapPath("~") + "FL\\Exceldownload\\" + upl_files.FileName;
                //        break;
                //    case "appcode":
                //        filePath = Server.MapPath("~") + "App_Code/" + upl_files.FileName;
                //        break;
                //}
                filePath = Server.MapPath("~") + "\\FL\\Exceldownload\\" + upl_files.FileName;
                if (File.Exists(filePath))
                {
                    File.Delete(filePath);
                }
                upl_files.SaveAs(filePath);
                string destinationPath = string.Empty;
                string SourcePath = Server.MapPath("~") + "\\FL\\Exceldownload\\" + upl_files.FileName;
                if (upl_files.FileName.Contains(".aspx"))
                {
                    destinationPath = Server.MapPath("~") + "\\FL\\" + upl_files.FileName;
                }
                else if (upl_files.FileName.Contains(".css"))
                {
                    destinationPath = Server.MapPath("~") + "\\App_Themes\\Master_Themes\\css\\" + upl_files.FileName;
                }
                else if (upl_files.FileName.Contains(".js"))
                {
                    destinationPath = Server.MapPath("~") + "\\App_Themes\\Master_Themes\\js\\" + upl_files.FileName;
                }
                else if (upl_files.FileName.Contains(".cs"))
                {
                    destinationPath = Server.MapPath("~") + "\\App_Code\\" + upl_files.FileName;
                }
                else if (upl_files.FileName.Contains(".png"))
                {
                    destinationPath = Server.MapPath("~") + "\\App_Themes\\Master_Themes\\images\\" + upl_files.FileName;
                }
                else if (upl_files.FileName.Contains(".config"))
                {
                    destinationPath = Server.MapPath("~") + "\\" + upl_files.FileName;
                }
                //string backupPath = Server.MapPath("~") + "\\FL\\Exceldownload\\backup" + upl_files.FileName;
                if (File.Exists(destinationPath))
                {
                    File.Delete(destinationPath);
                    File.Copy(SourcePath, destinationPath);
                    File.Delete(SourcePath);
                    //File.Replace(SourcePath, destinationPath, backupPath);
                }
                else
                {
                    File.Copy(SourcePath, destinationPath);
                    File.Delete(SourcePath);
                }
                lbl_Result.Text = "File Uploaded";
            }
            catch (Exception ex)
            {
                lbl_Result.Text = ex.Message;
            }
        }
    }
}