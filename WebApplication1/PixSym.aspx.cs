using System;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.IO;
using System.Configuration;
using System.Reflection;
using ASCII;

namespace WebApplication1
{
    [Serializable]
    internal class PageTools
    {
        public PageTools() { }

        #region Current Directory Location

        // Get default current directory.
        private static string defaultCurrentDirectory = null;

        public static string DefaultCurrentDirectory
        {
            get
            {
                if (defaultCurrentDirectory == null)
                {

                    try
                    {
                        defaultCurrentDirectory = HttpContext.Current.Server.MapPath(".");
                    }
                    catch
                    {

                        try
                        {
                            string asmName = Assembly.GetEntryAssembly().Location;
                            defaultCurrentDirectory = (new FileInfo(asmName)).DirectoryName;
                        }
                        catch
                        {
                            try
                            {
                                string asmName = Assembly.GetExecutingAssembly().Location;
                                defaultCurrentDirectory = (new FileInfo(asmName)).DirectoryName;
                            }
                            catch
                            {
                                try
                                {
                                    defaultCurrentDirectory = Directory.GetCurrentDirectory();
                                }
                                catch
                                {
                                    defaultCurrentDirectory = ".";
                                }
                            }
                        }
                    }
                }
                return (defaultCurrentDirectory);
            }
        }

        // Get or set current directory.
        // Note: This property is NOT the same as the actual system
        //       'current working directory' (CWD).
        private string currentDirectory = null;

        public string CurrentDirectory
        {
            get
            {
                if (currentDirectory == null) currentDirectory = DefaultCurrentDirectory;
                return (currentDirectory);
            }

            set
            {
                if (value != null)
                {
                    value = value.Trim().TrimEnd('\\');
                    if (value != "") currentDirectory = value;
                }
            }
        }

        #endregion

        #region Paths Resolving Methods

        // Get the full path-name of the current directory and relative path.
        public string GetPathName(string relPath)
        {
            return (GetPathName(CurrentDirectory, relPath));
        }

        // Get the full path-name of the given rootPath and relative path.
        public static string GetPathName(string rootPath, string relPath)
        {
            if (relPath == null) return (null);
            string path = relPath.Trim();

            if (path == ".")
            {
                path = rootPath;
            }
            else if (path == "..")
            {
                path = rootPath + "\\..";
            }
            else if (path.StartsWith(".\\"))
            {
                path = rootPath + path.Substring(1);
            }
            else if (path.StartsWith("..\\"))
            {
                path = rootPath + "\\" + path;
            }

            return (path);
        }

        #endregion

        #region .Config File Values Loading Methods

        public static string LoadConfig(string keyName, string defaultValue)
        {
            return (LoadConfig(keyName, defaultValue, true));
        }

        public static string LoadConfig(string keyName, string defaultValue,
                                         bool allowEmptyString)
        {
            try
            {
                string val = ConfigurationSettings.AppSettings[keyName];
                if ((val == "") && !allowEmptyString) return (defaultValue);
                return (val == null ? defaultValue : val);
            }
            catch
            {
                return (defaultValue);
            }
        }

        public static int LoadConfig(string keyName, int defaultValue)
        {
            try
            {
                return (Int32.Parse(ConfigurationSettings.AppSettings[keyName]));
            }
            catch
            {
                return (defaultValue);
            }
        }

        public static uint LoadConfig(string keyName, uint defaultValue)
        {
            try
            {
                return (UInt32.Parse(ConfigurationSettings.AppSettings[keyName]));
            }
            catch
            {
                return (defaultValue);
            }
        }

        public static long LoadConfig(string keyName, long defaultValue)
        {
            try
            {
                return (Int64.Parse(ConfigurationSettings.AppSettings[keyName]));
            }
            catch
            {
                return (defaultValue);
            }
        }

        public static ulong LoadConfig(string keyName, ulong defaultValue)
        {
            try
            {
                return (UInt64.Parse(ConfigurationSettings.AppSettings[keyName]));
            }
            catch
            {
                return (defaultValue);
            }
        }

        public static float LoadConfig(string keyName, float defaultValue)
        {
            try
            {
                return (Single.Parse(ConfigurationSettings.AppSettings[keyName]));
            }
            catch
            {
                return (defaultValue);
            }
        }

        public static double LoadConfig(string keyName, double defaultValue)
        {
            try
            {
                return (Double.Parse(ConfigurationSettings.AppSettings[keyName]));
            }
            catch
            {
                return (defaultValue);
            }
        }

        public static bool LoadConfig(string keyName, bool defaultValue)
        {
            try
            {
                return (Boolean.Parse(ConfigurationSettings.AppSettings[keyName]));
            }
            catch
            {
                return (defaultValue);
            }
        }

        #endregion
    }

    public partial class PixSym : System.Web.UI.Page
    {
        #region Fields

        public MainLogic AsciiArt = null;

        #endregion

        #region Properties

        public string defaultImageFile = null;

        public string DefaultImageFile
        {
            get
            {
                if (defaultImageFile == null)
                    defaultImageFile = PageTools.LoadConfig("DefaultImageFile", "");
                
                return (defaultImageFile);
            }
        }

        public string imageFolderName = null;

        public string ImageFolderName
        {
            get
            {
                if (imageFolderName == null) 
                    imageFolderName = PageTools.LoadConfig("ImageSubFolder", "").Trim().TrimEnd(new char[] { '\\', '/' });
                
                return (imageFolderName);
            }
        }

        public string imageFolderPath = null;

        public string ImageFolderPath
        {
            get
            {
                if (imageFolderPath == null)
                {
                    string folder = Server.MapPath(".\\") + ImageFolderName;
                    
                    if (ImageFolderName != "") 
                        folder += "\\";
                    
                    imageFolderPath = folder;
                }

                return (imageFolderPath);
            }
        }

        public string ImageUrlPath
        {
            get
            {
                string url = CurrentUrlPath + ImageFolderName;
                
                if (ImageFolderName != "")
                    url += "/";
                
                return (url);
            }
        }

        public string CurrentUrlPath
        {
            get
            {
                string url = Request.Url.ToString();
                int idx = url.IndexOf('?');
                
                if (idx >= 0) 
                    url = url.Substring(0, idx);
                
                idx = url.LastIndexOf('/');
                
                return (url.Substring(0, idx + 1));
            }
        }

        #endregion

        #region Methods

        #region Other Methods

        public void ShowError(string msg)
        {
            if (msg == null) 
                return;
            
            lblError.Text += "Ошибка: " + msg + "<br>\n";
            lblError.Visible = true;
        }

        public string GetPostedFileName() //хранит файл и извлекает имя
        {
            if (txtImageFile.PostedFile == null) 
                return (null);
            
            string fileType = txtImageFile.PostedFile.ContentType;
            string fileName = txtImageFile.PostedFile.FileName.Trim();
            int index = fileName.LastIndexOfAny(new char[] { '/', '\\' });
            
            fileName = fileName.Substring(index + 1);
            
            if (fileName == "") 
                return (null);

            if (!fileType.ToLower().StartsWith("image/"))
            {
                ShowError("Неверный тип файла!");
                return (null);
            }

            try
            {
                txtImageFile.PostedFile.SaveAs(ImageFolderPath + fileName);
                return (fileName);
            }
            catch (Exception e)
            {
                ShowError(e.Message);
                return (null);
            }
        }

        public string LocalUrlToLocalPath(string url)
        {
            if (url == null) 
                return (null);
            
            string baseUrl = CurrentUrlPath;

            if (url.StartsWith(baseUrl))
                return (Server.MapPath(url.Substring(baseUrl.Length)));
            else
                return (null);
        }

        public static string GetFileName(string filePath) //извлекает имя из пути или урла
        {
            if (filePath == null) 
                return (null);
            
            int index = filePath.LastIndexOf('?');
            
            if (index >= 0) 
                filePath = filePath.Substring(0, index);
            
            index = filePath.LastIndexOfAny(new char[] { '/', '\\' });
            filePath = filePath.Substring(index + 1);
            
            return (filePath == "" ? null : filePath);
        }

        #endregion

        #region Methods for Creating/Destroying Image To ASCII Converter

        public bool CreateImageConverter(string imagePath, bool useCurrentSettings, bool resetFixedChars)
        {
            try
            {
                string path = LocalUrlToLocalPath(imagePath);
                
                if (path == null) 
                    path = imagePath;
                
                AsciiArt = new MainLogic(path);
            }
            catch (FileNotFoundException)
            {
                ShowError("Невозможно найти файл по ссылке: " + txtImageUrl.Text);
                return (false);
            }
            catch (Exception exc)
            {
                ShowError(exc.Message);
                return (false);
            }

            if (useCurrentSettings)
            {
                AsciiArt.UseAlpabets = chkUseAlpha.Checked;
                AsciiArt.UseNumbers = chkUseNum.Checked;
                AsciiArt.UseBasicSymbols = chkUseBasic.Checked;
                AsciiArt.UseExtendedSymbols = chkUseExtended.Checked;
                AsciiArt.UseBlockSymbols = chkUseBlock.Checked;
                AsciiArt.UseFixedChars = chkUseFixed.Checked;
                
                if (!resetFixedChars) 
                    AsciiArt.FixedChars = txtFixedChars.Text.ToCharArray();

                try
                {
                    AsciiArt.FontSize = (int)UInt16.Parse(txtFontSize.Text);
                }
                catch
                {
                    txtFontSize.Text = AsciiArt.FontSize.ToString();
                    ShowError("Неверный размер шрифта! Сброс до стандартного значения.");
                }

                AsciiArt.BackgroundColor = txtBackColor.Text;
                AsciiArt.FontColor = txtForeColor.Text;
                AsciiArt.UseColors = radMultiColor.Checked;
                AsciiArt.IsGrayScale = chkGrayScale.Checked;
                AsciiArt.IsHtmlOutput = !chkTextOnly.Checked;
            }

            return (true);
        }

        public void DestroyImageConverter()
        {
            if (AsciiArt != null)
            {
                AsciiArt.Dispose();
                AsciiArt = null;
            }
        }

        #endregion

        #region Methods for Image To ASCII Conversions

        public string GetAsciiImage(bool useBlock) //conversion
        {
            if (AsciiArt == null) 
                return (null); ;
            
            int width = -1;
            int height = -1;
            int scale = 1;

            if (txtWidth.Text != "")
            {
                string val = txtWidth.Text;
                bool isPercent = false;

                if (val.EndsWith("%"))
                {
                    isPercent = true;
                    val = val.Substring(0, val.Length - 1);
                }

                try
                {
                    width = (int)UInt16.Parse(val);

                    if (isPercent)
                    {
                        width = AsciiArt.ImageWidth * width / 100;
                        txtWidth.Text = width.ToString();
                    }
                }
                catch
                {
                    width = AsciiArt.ImageWidth;
                    txtWidth.Text = width.ToString();
                    ShowError("Неверная ширина изображения! Сброс к стандартному значению.");
                }
            }

            if (txtHeight.Text != "")
            {
                string val = txtHeight.Text;
                bool isPercent = false;

                if (val.EndsWith("%"))
                {
                    isPercent = true;
                    val = val.Substring(0, val.Length - 1);
                }

                try
                {
                    height = (int)UInt16.Parse(val);

                    if (isPercent)
                    {
                        height = AsciiArt.ImageHeight * height / 100;
                        txtHeight.Text = height.ToString();
                    }
                }
                catch
                {
                    height = AsciiArt.ImageHeight;
                    txtHeight.Text = height.ToString();
                    ShowError("Неверная высота изображения! Сброс до стандартного значения.");
                }
            }

            if ((txtWidth.Text == "") && (txtHeight.Text == ""))
            {
                width = AsciiArt.ImageWidth;
                height = AsciiArt.ImageHeight;
                txtWidth.Text = width.ToString();
                txtHeight.Text = height.ToString();
            }
            else if (txtWidth.Text == "")
            {
                width = height * AsciiArt.ImageWidth / AsciiArt.ImageHeight;
                txtWidth.Text = width.ToString();
            }
            else if (txtHeight.Text == "")
            {
                height = width * AsciiArt.ImageHeight / AsciiArt.ImageWidth;
                txtHeight.Text = height.ToString();
            }

            foreach (Control comp in Form1.Controls)
            {
                RadioButton radScale = comp as RadioButton;
                
                if (radScale == null) 
                    continue;
                if (!radScale.Checked) 
                    continue;
                if (radScale.ID.Length < 9) 
                    continue;
                if (!radScale.ID.StartsWith("radScale")) 
                    continue;

                try
                {
                    scale = Int32.Parse(radScale.ID.Substring(8));
                    break;
                }
                catch { }
            }

            try
            {
                if (radScale.Checked)
                {
                    return (AsciiArt.GetAsciiImage(scale, useBlock));
                }
                else
                {
                    return (AsciiArt.GetAsciiImage(width, height, useBlock));
                }
            }
            catch (Exception exc)
            {
                ShowError(exc.Message);
                return (null);
            }
        }

        #endregion

        #region Form Events

        public void Page_Load(object sender, System.EventArgs e)
        {
            bool isReset = (Request[btnClear.ID] != null);
            bool isResetFixedChars = (Request[btnResetFixedChars.ID] != null);
            bool useCurrentSettings = (IsPostBack && !isReset);

            if (isReset)
            {
                txtImageUrl.Text = ImageUrlPath + DefaultImageFile;
            }
            else
            {
                string fileName = GetPostedFileName();

                if (fileName != null)
                {
                    txtImageUrl.Text = ImageUrlPath + fileName;
                }
                else if (txtImageUrl.Text == "")
                {
                    txtImageUrl.Text = ImageUrlPath + DefaultImageFile;
                }
                else
                {
                    txtImageUrl.Text = txtImageUrl.Text.Trim();
                }
            }

            imgOrigImage.ImageUrl = txtImageUrl.Text;

            if (!CreateImageConverter(txtImageUrl.Text, useCurrentSettings, isResetFixedChars)) 
                return;
            if (isResetFixedChars)
                txtFixedChars.Text = new String(AsciiArt.FixedChars);

            if (!useCurrentSettings)
            {  
                chkUseAlpha.Checked = AsciiArt.UseAlpabets;
                chkUseNum.Checked = AsciiArt.UseNumbers;
                chkUseBasic.Checked = AsciiArt.UseBasicSymbols;
                chkUseExtended.Checked = AsciiArt.UseExtendedSymbols;
                chkUseBlock.Checked = AsciiArt.UseBlockSymbols;
                chkUseFixed.Checked = AsciiArt.UseFixedChars;
                txtFixedChars.Text = new String(AsciiArt.FixedChars);
                txtFontSize.Text = AsciiArt.FontSize.ToString();
                txtBackColor.Text = AsciiArt.BackgroundColor;
                txtForeColor.Text = AsciiArt.FontColor;

                foreach (Control comp in Form1.Controls)
                {
                    RadioButton radio = comp as RadioButton;
                    if (radio != null) radio.Checked = false;
                }

                radSingleColor.Checked = !AsciiArt.UseColors;
                radMultiColor.Checked = AsciiArt.UseColors;
                chkGrayScale.Checked = AsciiArt.IsGrayScale;
                radScale.Checked = true;
                radScale1.Checked = true;
                txtWidth.Text = AsciiArt.ImageWidth.ToString();
                txtHeight.Text = AsciiArt.ImageHeight.ToString();
                chkTextOnly.Checked = !AsciiArt.IsHtmlOutput;
                chkDownload.Checked = false;
            }
        }

        public void btnConversion_Click(object sender, System.EventArgs e)
        {
            bool isHtml = !chkTextOnly.Checked;
            bool isView = !chkDownload.Checked;
            string ascii = GetAsciiImage(isHtml || isView);
            string newLine = (isHtml ? AsciiArt.HtmlNewLine : AsciiArt.TextNewLine);
            int size = AsciiArt.FontSize;
            DestroyImageConverter();
            if (ascii == null) return;

            if (isView)
            {
                lblAsciiImage.Text = ascii;
            }
            else
            {
                string fileName = GetFileName(txtImageUrl.Text);
                Response.Clear();
                Response.ContentType = (isHtml ? "text/html" : "text/plain");
                Response.AddHeader("Content-Disposition", "attachment; filename=\"" + fileName + (isHtml ? ".html" : ".txt") + "\"");
                Response.Flush();

                if (isHtml)
                {
                    Response.Output.WriteLine("<html>{0}<head>{0}<title>ASCII Image of {1}</title>{0}"
                         + "<script language='javascript'>{0}  function ChangeSize() {{{0}"
                         + "    var size = document.all['ImageSize'].selectedIndex + 1;{0}"
                         + "    var height = Math.floor (size * 3 / 5);{0}"
                         + "    if ((size % 5) % 2 == 1) height++;{0}"
                         + "    document.all['AsciiImage'].style.fontSize = size + 'px';{0}"
                         + "    document.all['AsciiImage'].style.lineHeight = height + 'px';"
                         + "{0}  }}{0}</script>{0}</head>{0}<body bgcolor='{2}' text='{3}'>"
                         + "{0}<div align='center'><br>", newLine, fileName, txtBackColor.Text, txtForeColor.Text);
                }

                Response.Output.WriteLine(ascii);

                if (isHtml)
                {
                    Response.Output.WriteLine
                        ("<b><i>ASCII Image of {1}</i></b>{0}<form>{0}<b>(Size:{0}"
                         + "<select id='ImageSize' onchange='ChangeSize()'>", newLine, fileName);

                    for (int i = 1; i <= 20; i++)
                    {
                        string selected = (size == i ? " selected='selected'" : "");
                        Response.Output.WriteLine("  <option value='{0}'{1}>{0}</option>", i, selected);
                    }

                    Response.Output.WriteLine("</select>{0})</b>{0}</form>{0}</div>{0}</body>{0}</html>", newLine);
                }

                Response.Flush();
                Response.End();
            }
        }

        public void btnClear_Click(object sender, System.EventArgs e)
        {
            DestroyImageConverter();
        }

        public void btnSetImage_Click(object sender, System.EventArgs e)
        {
            DestroyImageConverter();
        }

        public void btnResetFixedChars_Click(object sender, System.EventArgs e)
        {
            DestroyImageConverter();
        }

        #endregion

        #endregion
    }
}