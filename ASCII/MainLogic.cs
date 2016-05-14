using System;
using System.Data;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.IO;
using System.Net;
using System.Text;

namespace ASCII
{
    public class MainLogic
    {
        public const string DefaultAsciiDataFile = @".\ASCII.xml";
        public const string DefaultAsciiDataEntryName = "ascii";

        private const float R = 0.2125f;
        private const float G = 0.7154f;
        private const float B = 0.0721f;

        public string FontFamily = "Courier New, Courier, monospace";
        public string FontColor = "#000000";
        public string BackgroundColor = "#FFFFFF";
        public int FontSize = 12;

        public bool UseAlpabets = true;
        public bool UseNumbers = true;
        public bool UseBasicSymbols = true;
        public bool UseExtendedSymbols = false;
        public bool UseBlockSymbols = false;
        public bool UseFixedChars = false;
        public bool UseColors = true;
        public bool IsGrayScale = false;
        public bool IsHtmlOutput = true;

        private MemoryStream ImageStream = null;
        private Image ImageFile = null;

        private string asciiDataFile = null;
        public string AsciiDataFile
        {
            get
            {
                if (asciiDataFile == null)
                    asciiDataFile = Tools.GetPathName(Tools.DefaultCurrentDirectory, DefaultAsciiDataFile);

                return (asciiDataFile);
            }

            set
            {
                if (value == null)
                    value = ".";
                else
                    value = value.Trim().TrimEnd('\\', '/');

                if (value == "") 
                    value = ".";
                
                asciiDataFile = Tools.GetPathName(Tools.DefaultCurrentDirectory, value);
            }
        }

        private string asciiDataEntryName = DefaultAsciiDataEntryName;
        public string AsciiDataEntryName
        {
            get { return (asciiDataEntryName); }
            set { if (value != null) asciiDataEntryName = value; }
        }

        public string BlockStart 
        {
            get
            {
                if (IsHtmlOutput)
                {
                    int height = FontSize * 3 / 5;
                    
                    if ((FontSize % 5) % 2 == 1) 
                        height++;

                    return ("<pre id='AsciiImage' style='letter-spacing: 0px; font-family: "
                            + FontFamily + "; color: " + FontColor +
                            "; background-color: " + BackgroundColor + "; font-size: "
                            + FontSize + "px; line-height: " + height + "px'>");
                }
                else
                    return ("<xmp id='AsciiImage' style='color: " + FontColor +
                            "; background-color: " + BackgroundColor + "; font-size: "
                            + FontSize + "px'>");
            }
        }
        public string BlockEnd
        {
            get { return (IsHtmlOutput ? "</pre>" : "</xmp>"); }
        }

        private string htmlNewLine = "\n";
        public string HtmlNewLine
        {
            get { return (htmlNewLine); }
            set { if ((value != null) && (value != "")) htmlNewLine = value; }
        }

        private string textNewLine = "\r\n";
        public string TextNewLine
        {
            get { return (textNewLine); }
            set { if ((value != null) && (value != "")) textNewLine = value; }
        }

        private char[] fixedChars = new char[] { 'a', 'i', 'L', 'D', ' ', '.', ':', ',', ';', '+', '#' };
        public char[] FixedChars
        {
            get { return (fixedChars); }
            set { if (value != null) fixedChars = value; }
        }
   
        public int ImageWidth
        {
            get { return (ImageFile == null ? -1 : ImageFile.Width); }
        }
        public int ImageHeight
        {
            get { return (ImageFile == null ? -1 : ImageFile.Height); }
        }

        private DataTable asciiDataTable = null;
        private object AsciiDataTableMutex = new object();
        private DataTable AsciiDataTable
        {
            get
            {
                if (asciiDataTable == null)
                    lock (AsciiDataTableMutex)
                        if (asciiDataTable == null)
                        {  
                            DataSet ds = new DataSet();
                            ds.ReadXml(AsciiDataFile);
                            DataTable dt = ds.Tables[AsciiDataEntryName];
                            dt.CaseSensitive = true;
                            asciiDataTable = dt;
                        }

                return (asciiDataTable);
            }
        }

        private DataView asciiDataHtml = null;
        private object AsciiDataHtmlMutex = new object();
        private DataView AsciiDataHtml
        {
            get
            {
                lock (AsciiDataHtmlMutex) //create DataView
                {
                    if (asciiDataHtml == null) 
                        asciiDataHtml = new DataView(AsciiDataTable);
                    
                    SetupAsciiData(asciiDataHtml, true);
                    
                    return (asciiDataHtml);
                }
            }
        }

        private DataView asciiDataText = null;
        private object AsciiDataTextMutex = new object();
        private DataView AsciiDataText
        {
            get
            {
                lock (AsciiDataTextMutex) //create DataView
                {
                    if (asciiDataText == null) 
                        asciiDataText = new DataView(AsciiDataTable);
                    
                    SetupAsciiData(asciiDataText, false);
                    
                    return (asciiDataText);
                }
            }
        }

        private void SetupAsciiData(DataView asciiData, bool isHtml)
        {
            string filter = "";

            if (UseFixedChars)
            {
                filter = "htmlchar in (";

                foreach (char c in FixedChars) 
                    filter += "'" + CharToHtml(c) + "',";
                
                filter = filter.Substring(0, filter.Length - 1) + ")";
            }
            else
            {
                if (UseAlpabets) 
                    filter += "type = 1 OR ";
                if (UseNumbers) 
                    filter += "type = 2 OR ";
                if (UseBasicSymbols) 
                    filter += "type = 3 OR ";
                if (UseExtendedSymbols) 
                    filter += "type = 4 OR ";
                if (UseBlockSymbols && isHtml) 
                    filter += "type = 5 OR ";

                if (filter.Length > 0)
                {
                    filter = filter.Substring(0, filter.Length - 4);
                    
                    if (!isHtml) 
                        filter = "(" + filter + ")";
                }
                else
                    filter = "type = 0";
            }

            if (!isHtml) 
                filter += " AND id > 31 AND id < 127";
            if (asciiData.RowFilter != filter) 
                asciiData.RowFilter = filter;

            string sort;

            switch (FontSize)
            {
                case 13: 
                    sort = "c13"; 
                    break;
                case 16: 
                    sort = "c16"; 
                    break;
                default: 
                    sort = "c20"; 
                    break;
            }

            if (asciiData.Sort != sort) 
                asciiData.Sort = sort;
        }

        public MainLogic(string imageFile)
        {
            if (imageFile.ToLower().StartsWith("http"))
                try
                {
                    HttpWebRequest req = (HttpWebRequest)WebRequest.Create(imageFile);
                    req.Credentials = CredentialCache.DefaultCredentials;
                    req.Proxy = WebProxy.GetDefaultProxy();
                    
                    if (req.Proxy.Credentials == null)
                        req.Proxy.Credentials = CredentialCache.DefaultCredentials;
                    
                    req.UserAgent = "User-Agent: Mozilla/5.0 (compatible; MSIE 8.0)";
                    req.Method = "GET";

                    WebResponse resp = req.GetResponse();
                    byte[] data = new byte[resp.ContentLength];
                    Stream rawStream = resp.GetResponseStream();
                    int total = 0;

                    while (total < data.Length)
                        total += rawStream.Read(data, total, data.Length - total);

                    rawStream.Close();
                    
                    ImageStream = new MemoryStream(data, 0, total);
                    ImageFile = Image.FromStream(ImageStream);
                }
                catch
                {
                    if (ImageStream != null) 
                        ImageStream.Close();
                    
                    throw new ArgumentException("Невозможно открыть изображение по данному URL!");
                }
            else
                ImageFile = Image.FromFile(imageFile);
        }
        public MainLogic(byte[] image)
        {
            ImageStream = new MemoryStream(image);
            ImageFile = Image.FromStream(ImageStream);
        }

        ~MainLogic()
        {
            if (ImageFile != null)
            {
                ImageFile.Dispose();
                ImageFile = null;
            }

            if (ImageStream != null)
            {
                ImageStream.Close();
                ImageStream = null;
            }
        }
        public void Dispose()
        {
            if (ImageFile != null)
            {
                ImageFile.Dispose();
                ImageFile = null;
            }

            if (ImageStream != null)
            {
                ImageStream.Close();
                ImageStream = null;
            }
        }

        public static char HtmlToChar(string htmlEntity)
        {
            if ((htmlEntity == null) || (htmlEntity == "")) 
                return (Char.MinValue);

            if (htmlEntity[0] != '&')
            {
                if (htmlEntity.Length > 1) 
                    return (Char.MinValue);
                
                return (htmlEntity[0]);
            }

            if (htmlEntity.Length == 1) 
                return ('&');
            if (htmlEntity[htmlEntity.Length - 1] != ';') 
                return (Char.MinValue);

            if (htmlEntity[1] == '#')
            {
                string code = htmlEntity.Substring(2, htmlEntity.Length - 3);

                try
                {
                    return ((char)Int32.Parse(code));
                }
                catch { }
            }

            if (htmlEntity == "&amp;") return ('&');
            if (htmlEntity == "&lt;") return ('<');
            if (htmlEntity == "&gt;") return ('>');

            return (Char.MinValue);
        }
        public static string CharToHtml(char character)
        {
            int code = (int)character;
            
            if ((code < 32) || (code > 126)) 
                return ("&#" + code + ";");

            switch (character)
            {
                case '&': 
                    return ("&amp;");
                case '<': 
                    return ("&lt;");
                case '>': 
                    return ("&gt;");
                default: 
                    return (character.ToString());
            }
        }

        private static ColorMatrix GetColorMatrix(float brightness, float contrast, float saturation, float gamma)
        {
            if (brightness < -1f) // brightness = -1 to 1 (default = 0)
                brightness = -1f;
            if (brightness > 1f) 
                brightness = 1f;
            if (contrast < 0f) // contrast = 0 to 4 (default = 1, pure-gray = 0)
                contrast = 0f;
            if (saturation < 0f) // saturation = 0 to 3 (default = 1, grayscale = 0)
                saturation = 0f;
            if (gamma < 0f) // gamma = 0 to 4 (Default = 1)
                gamma = 0f;

            float Wf = (1f - contrast) / 2f + brightness;
            float Rf = (1f - saturation) * R * contrast;
            float Gf = (1f - saturation) * G * contrast;
            float Bf = (1f - saturation) * B * contrast;
            float Rf2 = Rf + saturation * contrast;
            float Gf2 = Gf + saturation * contrast;
            float Bf2 = Bf + saturation * contrast;

            return (new ColorMatrix(new float[][] {
                new float[] {Rf2, Rf, Rf, 0f, 0f},
                new float[] {Gf, Gf2, Gf, 0f, 0f},
                new float[] {Bf, Bf, Bf2, 0f, 0f},
                new float[] {0f, 0f, 0f, 1f, 0f},
                new float[] {Wf, Wf, Wf, 0f, gamma}
            }));
        }

        private static Bitmap GetResizedImage(Image image, int width, int height, Rectangle section)
        {
            Bitmap newImage = new Bitmap(width, height);

            try
            {
                using (Graphics g = Graphics.FromImage(newImage))
                {
                    g.InterpolationMode = InterpolationMode.HighQualityBicubic;

                    g.DrawImage(image, new Rectangle(0, 0, width, height), section.X, section.Y, 
                        section.Width, section.Height, GraphicsUnit.Pixel);
                }
            }
            catch
            {
                newImage.Dispose();
                
                throw;
            }

            return (newImage);
        }
        private static Bitmap GetTransformedImage(Image image, ColorMatrix matrix)
        {
            Bitmap newImage = new Bitmap(image.Width, image.Height);

            try
            {
                using (ImageAttributes info = new ImageAttributes())
                {
                    info.SetGamma(matrix.Matrix44);
                    matrix.Matrix44 = 1f;
                    info.SetColorMatrix(matrix);

                    using (Graphics g = Graphics.FromImage(newImage))
                    {
                        g.DrawImage(image, new Rectangle(0, 0, image.Width, image.Height),
                            0, 0, image.Width, image.Height, GraphicsUnit.Pixel, info);
                    }
                }
            }
            catch
            {
                newImage.Dispose();
                
                throw;
            }

            return (newImage);
        }

        public string GetAsciiImage(int downScale, bool usePreBlock)
        {
            int width = ImageFile.Width / downScale;
            int height = ImageFile.Height / downScale;
            Rectangle section = new Rectangle(0, 0, ImageFile.Width, ImageFile.Height);
            Bitmap image = GetResizedImage(ImageFile, width, height, section);
            ColorMatrix gray = GetColorMatrix(0f, 1f, 0f, 1f);

            try
            {
                return (GetAsciiImage(image, null, gray, usePreBlock));
            }
            finally
            {
                image.Dispose();
            }
        }
        public string GetAsciiImage(double scale, bool usePreBlock)
        {
            int width = (int)(ImageFile.Width * scale);
            int height = (int)(ImageFile.Height * scale);
            Rectangle section = new Rectangle(0, 0, ImageFile.Width, ImageFile.Height);
            Bitmap image = GetResizedImage(ImageFile, width, height, section);
            ColorMatrix gray = GetColorMatrix(0f, 1f, 0f, 1f);

            try
            {
                return (GetAsciiImage(image, null, gray, usePreBlock));
            }
            finally
            {
                image.Dispose();
            }
        }
        public string GetAsciiImage(bool usePreBlock)
        {
            Bitmap image = new Bitmap(ImageFile);
            ColorMatrix gray = GetColorMatrix(0f, 1f, 0f, 1f);

            try
            {
                return (GetAsciiImage(image, null, gray, usePreBlock));
            }
            finally
            {
                image.Dispose();
            }
        }
        public string GetAsciiImage(int width, int height, bool usePreBlock)
        {
            Rectangle section = new Rectangle(0, 0, ImageFile.Width, ImageFile.Height);
            Bitmap image = GetResizedImage(ImageFile, width, height, section);
            ColorMatrix gray = GetColorMatrix(0f, 1f, 0f, 1f);

            try
            {
                return (GetAsciiImage(image, null, gray, usePreBlock));
            }
            finally
            {
                image.Dispose();
            }
        }
        public string GetAsciiImage(float brightness, float contrast, float saturation, float gamma, bool usePreBlock)
        {
            Bitmap image = new Bitmap(ImageFile);
            ColorMatrix color = GetColorMatrix(brightness, contrast, saturation, gamma);
            ColorMatrix gray = GetColorMatrix(brightness, contrast, 0f, gamma);

            try
            {
                return (GetAsciiImage(image, color, gray, usePreBlock));
            }
            finally
            {
                image.Dispose();
            }
        }
        public string GetAsciiImage(int width, int height, Rectangle section, bool usePreBlock)
        {
            Bitmap image = GetResizedImage(ImageFile, width, height, section);
            ColorMatrix gray = GetColorMatrix(0f, 1f, 0f, 1f);

            try
            {
                return (GetAsciiImage(image, null, gray, usePreBlock));
            }
            finally
            {
                image.Dispose();
            }
        }
        public string GetAsciiImage(int width, int height, float brightness, float contrast, float saturation, float gamma, bool usePreBlock)
        {
            Rectangle section = new Rectangle(0, 0, ImageFile.Width, ImageFile.Height);
            Bitmap image = GetResizedImage(ImageFile, width, height, section);
            ColorMatrix color = GetColorMatrix(brightness, contrast, saturation, gamma);
            ColorMatrix gray = GetColorMatrix(brightness, contrast, 0f, gamma);

            try
            {
                return (GetAsciiImage(image, color, gray, usePreBlock));
            }
            finally
            {
                image.Dispose();
            }
        }
        public string GetAsciiImage(int width, int height, Rectangle section, float brightness, float contrast, float saturation, float gamma, bool usePreBlock)
        {
            Bitmap image = GetResizedImage(ImageFile, width, height, section);
            ColorMatrix color = GetColorMatrix(brightness, contrast, saturation, gamma);
            ColorMatrix gray = GetColorMatrix(brightness, contrast, 0f, gamma);

            try
            {
                return (GetAsciiImage(image, color, gray, usePreBlock));
            }
            finally
            {
                image.Dispose();
            }
        }
        private string GetAsciiImage(Bitmap image, ColorMatrix colorMatrix, ColorMatrix grayMatrix, bool usePreBlock)
        {
            StringBuilder ascii = new StringBuilder();
            
            if (usePreBlock) 
                ascii.Append(BlockStart);
            
            GetAsciiImage(ascii, image, colorMatrix, grayMatrix);
            
            if (usePreBlock) 
                ascii.Append(BlockEnd);
            
            return (ascii.ToString());
        }
        private void GetAsciiImage(StringBuilder output, Bitmap image, ColorMatrix colorMatrix, ColorMatrix grayMatrix)
        {
            Bitmap colorImage = null;
            Bitmap grayImage = null;

            try
            {
                colorImage = ((colorMatrix == null) || !IsHtmlOutput || !UseColors || IsGrayScale ? image : GetTransformedImage(image, colorMatrix));
                grayImage = (grayMatrix == null ? image : GetTransformedImage(image, grayMatrix));

                DataView asciiData = (IsHtmlOutput ? AsciiDataHtml : AsciiDataText);

                if (asciiData.Count == 0)
                    throw new DataException("Не введено ни одного ASCII-символа!");

                if ((asciiData.Count == 1) && (!UseColors || !IsHtmlOutput))
                    throw new DataException("Мало символов для использования!");

                string[] ascii = new string[asciiData.Count];

                for (int i = 0; i < asciiData.Count; i++)
                {
                    string htmlChar = asciiData[i]["htmlchar"].ToString();
                    ascii[i] = (IsHtmlOutput ? htmlChar : HtmlToChar(htmlChar).ToString());
                }

                string prevColor = null;
                string color;

                for (int y = 0; y < grayImage.Height; y++)
                {
                    for (int x = 0; x < grayImage.Width; x++)
                    {
                        Color pixel = grayImage.GetPixel(x, y);
                        int index = pixel.R * ascii.Length / 256;
                        string ch = ascii[ascii.Length - index - 1];

                        if (IsHtmlOutput && UseColors)
                        {
                            if (!IsGrayScale) 
                                pixel = colorImage.GetPixel(x, y);
                            
                            color = GetHexColor(pixel);

                            if (color != prevColor)
                            {
                                if (prevColor != null) 
                                    output.Append("</font>");
                                
                                prevColor = color;
                                output.Append("<font color='#");
                                output.Append(color);
                                output.Append("'>");
                            }
                        }

                        output.Append(ch);
                    }

                    if (prevColor != null)
                    {
                        output.Append("</font>");
                        prevColor = null;
                    }

                    output.Append(IsHtmlOutput ? HtmlNewLine : TextNewLine);
                }
            }
            finally
            {
                if ((colorImage != null) && (colorImage != image)) 
                    colorImage.Dispose();
                if ((grayImage != null) && (grayImage != image)) 
                    grayImage.Dispose();
            }
        }

        private string GetHexColor(Color color)
        {
            int colorCode = (color.ToArgb() & 0xFFFFFF);
            string hexColor = "00000" + colorCode.ToString("X");
            return (hexColor.Substring(hexColor.Length - 6));
        }
    }
}
