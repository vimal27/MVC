using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using System.IO.Compression;
using ICSharpCode.SharpZipLib.Tar;
using System.Net;
using System.Security.Cryptography;

public partial class FL_SourceCodeViewer : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
        if (!IsPostBack == true)
        {
            ProtectedBlock.Visible = false;
            AuthenticationBlock.Visible = true;
        }
    }
    protected void UnlockCode(object sender, EventArgs e)
    {
        try
        {
            if (txt_Password.Text == Decrypt("JHTqfQxUpS1UGSWZb/xm2R/sFqoc6t100dCViWKKtG8="))
            {
                ProtectedBlock.Visible = true;
                AuthenticationBlock.Visible = false;
            }
            else
            {
                ProtectedBlock.Visible = false;
                AuthenticationBlock.Visible = true;
            }
        }
        catch (Exception ex)
        {

        }
    }
    protected void LockCode(object sender, EventArgs e)
    {
        ProtectedBlock.Visible = false;
        AuthenticationBlock.Visible = true;
        ResultBlock.InnerText = string.Empty;
    }
    protected void ViewCode(object sender, EventArgs e)
    {
        try
        {
            string FilePath = Server.MapPath("~") + txt_FilePath.Text;
            string inputString = string.Empty;
            using (StreamReader streamReader = File.OpenText(FilePath))
            {
                inputString = streamReader.ReadLine();
                while (inputString != null)
                {
                    if (FilePath.Contains(".aspx.cs"))
                        ResultBlock.InnerHtml += inputString + "<br />";
                    else
                    {
                        ResultBlock.InnerText += inputString + Environment.NewLine;
                    }
                    inputString = streamReader.ReadLine();
                }
            }

        }
        catch (Exception ex)
        {

        }
    }

    protected void CompressAndDownload(object sender, EventArgs e)
    {
        string dirpath = Server.MapPath("~") + txt_FilePath.Text;
        DirectoryInfo di = new DirectoryInfo(dirpath);
        //foreach (FileInfo fi in di.GetFiles())
        //{
        //    Compress(fi);
        //}
        string sCompressedFile = Server.MapPath("~") + "FL/Exceldownload";
        //CompressDirectory(dirpath, sCompressedFile);
        string FileName = DateTime.Now.ToString("dd_MM_yyyy_hh_mm_ss") + ".tar.gz";
        CreateTar(dirpath, sCompressedFile, FileName);
        DownloadFile(sCompressedFile + "/" + FileName, FileName);
    }
    public static string CreateTar(string directoryToCompress, string destPath, string tarFile)
    {
        string destDrive = destPath.Substring(0, destPath.IndexOf(@"\") + 1);
        Directory.SetCurrentDirectory(destDrive);
        string tarFilePath = Path.Combine(destPath, tarFile);
        using (Stream fs = new FileStream(tarFilePath, FileMode.OpenOrCreate))
        {
            using (TarArchive ta = TarArchive.CreateOutputTarArchive(fs))
            {
                string[] files = Directory.GetFiles(directoryToCompress);
                foreach (string file in files)
                {
                    string entry = file.Substring(file.IndexOf(@"\") + 1);
                    TarEntry te = TarEntry.CreateEntryFromFile(entry);
                    ta.WriteEntry(te, false);
                }
            }
        }
        return tarFilePath;
    }
    private void DownloadFile(string FilePath, string FileName)
    {
        try
        {
            if (File.Exists(FilePath))
            {
                WebClient req = new WebClient();
                HttpResponse response = HttpContext.Current.Response;
                response.Clear();
                response.ClearContent();
                response.ClearHeaders();
                response.Buffer = true;
                response.AddHeader("Content-Disposition", "attachment;filename=\"" + FileName + "\"");
                byte[] data = req.DownloadData(FilePath);
                response.BinaryWrite(data);
                response.End();
                File.Delete(FilePath);
            }
        }
        catch (Exception ex)
        {
            File.Delete(FilePath);
        }
    }
    /// <summary>
    /// Encryption For Login Password with Encrypt key : $321SeCiVrEsLaTiGiDZiPaL
    /// </summary>
    /// <param name="clearText"></param>
    /// <returns></returns>
    private string Encrypt(string clearText)
    {
        string EncryptionKey = "$321SeCiVrEsLaTiGiDZiPaL";
        byte[] clearBytes = Encoding.Unicode.GetBytes(clearText);
        using (Aes encryptor = Aes.Create())
        {
            Rfc2898DeriveBytes pdb = new Rfc2898DeriveBytes(EncryptionKey, new byte[] { 0x49, 0x76, 0x61, 0x6e, 0x20, 0x4d, 0x65, 0x64, 0x76, 0x65, 0x64, 0x65, 0x76 });
            encryptor.Key = pdb.GetBytes(32);
            encryptor.IV = pdb.GetBytes(16);
            using (MemoryStream ms = new MemoryStream())
            {
                using (CryptoStream cs = new CryptoStream(ms, encryptor.CreateEncryptor(), CryptoStreamMode.Write))
                {
                    cs.Write(clearBytes, 0, clearBytes.Length);
                    cs.Close();
                }
                clearText = Convert.ToBase64String(ms.ToArray());
            }
        }
        return clearText;
    }
    /// <summary>
    /// Decryption For Login Password
    /// </summary>
    /// <param name="cipherText"></param>
    /// <returns></returns>
    private string Decrypt(string cipherText)
    {
        string EncryptionKey = "$321SeCiVrEsLaTiGiDZiPaL";
        byte[] cipherBytes = Convert.FromBase64String(cipherText);
        using (Aes encryptor = Aes.Create())
        {
            Rfc2898DeriveBytes pdb = new Rfc2898DeriveBytes(EncryptionKey, new byte[] { 0x49, 0x76, 0x61, 0x6e, 0x20, 0x4d, 0x65, 0x64, 0x76, 0x65, 0x64, 0x65, 0x76 });
            encryptor.Key = pdb.GetBytes(32);
            encryptor.IV = pdb.GetBytes(16);
            using (MemoryStream ms = new MemoryStream())
            {
                using (CryptoStream cs = new CryptoStream(ms, encryptor.CreateDecryptor(), CryptoStreamMode.Write))
                {
                    cs.Write(cipherBytes, 0, cipherBytes.Length);
                    cs.Close();
                }
                cipherText = Encoding.Unicode.GetString(ms.ToArray());
            }
        }
        return cipherText;
    }
}