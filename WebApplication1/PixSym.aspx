<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="PixSym.aspx.cs" Inherits="WebApplication1.PixSym" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <meta http-equiv="Content-Type" content="text/html; charset=utf-8"/>
    <title>PIXSym - Главная</title>
    <link rel="shortcut icon" href="Images/logo.ico" type="image/x-icon">
    <link rel="stylesheet" type="text/css" href="css/index.css">
</head>
<body>
    <div id="topPanel">
        <table id="topTable">
            <tr>
                <td class="logo">
                    <a href="/PixSym.aspx"><img src="css/images/logo.png"></a>
                </td>
                <td id="name">
                    <a href="/PixSym.aspx"><img src="css/images/name.png"></a>
                </td>
            </tr>
        </table>
    </div>
    <div id="signContent">
        <form id="Form1" method="post" runat="server">
            <table id="mainTable">
                <tr>
                    <td colspan="2">
                        <h1>Настройки</h1>
                    </td>
                </tr>
                <tr>
                    <td class="leftcol">URL картинки:&nbsp;</td>
                    <td class="rightcol">&nbsp;
                        <asp:textbox CssClass="textView" id="txtImageUrl" runat="server" Width="76%"></asp:textbox>
                        <asp:button CssClass="btn btn-1 btn-1e" id="btnSetImage" runat="server" Width="20%" Text="Просмотр" onclick="btnSetImage_Click"></asp:button>
                    </td>
                </tr>
                <tr>
                    <td class="leftcol">Загрузить файл:&nbsp;</td>
                    <td class="rightcol">&nbsp;&nbsp;<input id="txtImageFile" type="file" name="txtImageFile" runat="server"></td>
                </tr>
                <tr>
                    <td class="leftcol">Использовать алфавит:&nbsp;</td>
                    <td class="rightcol">&nbsp;<i><asp:checkbox CssClass="chk" id="chkUseAlpha" runat="server" Text="(A-Z, a-z)"></asp:checkbox></i></td>
                </tr>
                <tr>
                    <td class="leftcol">Использовать цифры:&nbsp;</td>
                    <td class="rightcol">&nbsp;<i><asp:checkbox CssClass="chk" id="chkUseNum" runat="server" Text="(0-9)"></asp:checkbox></i></td>
                </tr>
                <tr>
                    <td class="leftcol">Использовать базовые символы:&nbsp;</td>
                    <td class="rightcol">&nbsp;<i><asp:checkbox CssClass="chk" id="chkUseBasic" runat="server" Text="(не Unicode символы и независимая яркость)"></asp:checkbox></i></td>
                </tr>
                <tr>
                    <td class="leftcol">Использовать расширенные символы:&nbsp;</td>
                    <td class="rightcol">&nbsp;<i><asp:checkbox CssClass="chk" id="chkUseExtended" runat="server" Text="(не Unicode символы и зависимая яркость)"></asp:checkbox></i></td>
                </tr>
                <tr>
                    <td class="leftcol">Использовать блочные символы:&nbsp;</td>
                    <td class="rightcol">&nbsp;<i><asp:checkbox CssClass="chk" id="chkUseBlock" runat="server" Text="(Unicode символы: блоки, пайпы, и т.д.)"></asp:checkbox></i></td>
                </tr>
                <tr>
                    <td class="leftcol">Использовать фиксированные символы:&nbsp;</td>
                    <td class="rightcol">&nbsp;<i><asp:checkbox CssClass="chk" id="chkUseFixed" runat="server" Text="(использовать только записанные в набор ниже)"></asp:checkbox></i></td>
                </tr>
                <tr>
                    <td class="leftcol">Фиксированный набор символов:&nbsp;</td>
                    <td class="rightcol">&nbsp;
                        <asp:textbox CssClass="textView" id="txtFixedChars" runat="server" Width="76%"></asp:textbox>
                        <asp:button CssClass="btn btn-1 btn-1e" id="btnResetFixedChars" runat="server" Width="20%" Text="Сброс" onclick="btnResetFixedChars_Click"></asp:button>
                    </td>
                </tr>
                <tr>
                    <td class="leftcol">Размер шрифта:&nbsp;</td>
                    <td class="rightcol">&nbsp;&nbsp;<asp:textbox CssClass="textView" id="txtFontSize" runat="server" Width="7%"></asp:textbox></td>
                </tr>
                <tr>
                    <td class="leftcol">Цвет заднего фона:&nbsp;</td>
                    <td class="rightcol">&nbsp;&nbsp;<asp:textbox CssClass="textView" id="txtBackColor" runat="server" Width="13%"></asp:textbox></td>
                </tr>
                <tr>
                    <td class="leftcol"><asp:radiobutton id="radSingleColor" runat="server" Text="Единый цвет шрифта:&nbsp;" GroupName="ColorType"></asp:radiobutton></td>
                    <td class="rightcol">&nbsp;&nbsp;<asp:textbox CssClass="textView" id="txtForeColor" runat="server" Width="13%"></asp:textbox></td>
                </tr>
                <tr>
                    <td class="leftcol"><asp:radiobutton id="radMultiColor" runat="server" Text="Многоцветовой шрифт:&nbsp;" GroupName="ColorType"
                            Checked="True"></asp:radiobutton></td>
                    <td class="rightcol">&nbsp<asp:checkbox CssClass="chk" id="chkGrayScale" runat="server" Text="Использовать ч/б цвета "></asp:checkbox><i>(Генерируются автоматически)</i></td>
                </tr>
                <tr>
                    <td class="leftcol">&nbsp;<asp:radiobutton id="radScale" runat="server" Text="Уменьшить масштаб картинки:&nbsp;" GroupName="SizingType" Checked="True"></asp:radiobutton></td>
                    <td class="rightcol">
                        <table>
                            <tr>
                                <td>
                                    <asp:radiobutton id="radScale1" runat="server" Text="нет" GroupName="ScaleFactor" Checked="True" Width="50px"></asp:radiobutton>
                                </td>
                                <td>
                                    <asp:radiobutton id="radScale2" runat="server" Text="x2" GroupName="ScaleFactor" Width="50px"></asp:radiobutton>
                                </td>
                                <td>
                                    <asp:radiobutton id="radScale3" runat="server" Text="x3" GroupName="ScaleFactor" Width="50px"></asp:radiobutton>
                                </td>
                                <td>
                                    <asp:radiobutton id="radScale4" runat="server" Text="x4" GroupName="ScaleFactor" Width="50px"></asp:radiobutton>
                                </td>
                                <td>
                                    <asp:radiobutton id="radScale5" runat="server" Text="x5" GroupName="ScaleFactor" Width="50px"></asp:radiobutton>
                                </td>
                                <td>
                                    <asp:radiobutton id="radScale6" runat="server" Text="x6" GroupName="ScaleFactor" Width="50px"></asp:radiobutton>
                                </td>
                                <td>
                                    <asp:radiobutton id="radScale7" runat="server" Text="x7" GroupName="ScaleFactor" Width="50px"></asp:radiobutton>
                                </td>
                                <td>
                                    <asp:radiobutton id="radScale8" runat="server" Text="x8" GroupName="ScaleFactor" Width="50px"></asp:radiobutton>
                                </td>
                            </tr>
                            <tr>
                                <td>
                                    <asp:radiobutton id="radScale9" runat="server" Text="x9" GroupName="ScaleFactor" Width="50px"></asp:radiobutton>
                                </td>
                                <td>
                                    <asp:radiobutton id="radScale10" runat="server" Text="x10" GroupName="ScaleFactor" Width="50px"></asp:radiobutton>
                                </td>
                                <td>
                                    <asp:radiobutton id="radScale11" runat="server" Text="x11" GroupName="ScaleFactor" Width="50px"></asp:radiobutton>
                                </td>
                                <td>
                                    <asp:radiobutton id="radScale12" runat="server" Text="x12" GroupName="ScaleFactor" Width="50px"></asp:radiobutton>
                                </td>
                                <td>
                                    <asp:radiobutton id="radScale13" runat="server" Text="x13" GroupName="ScaleFactor" Width="50px"></asp:radiobutton>
                                </td>
                                <td>
                                    <asp:radiobutton id="radScale14" runat="server" Text="x14" GroupName="ScaleFactor" Width="50px"></asp:radiobutton>                                                       
                                </td>
                                <td>
                                    <asp:radiobutton id="radScale15" runat="server" Text="x15" GroupName="ScaleFactor" Width="50px"></asp:radiobutton>
                                </td>
                                <td>
                                    <asp:radiobutton id="radScale16" runat="server" Text="x16" GroupName="ScaleFactor" Width="50px"></asp:radiobutton>
                                </td>
                            </tr>
                        </table>
                    </td>
                </tr>
                <tr>
                    <td class="leftcol"><asp:radiobutton id="radSize" runat="server" Text="Пользовательский размер картинки:&nbsp;" GroupName="SizingType"></asp:radiobutton></td>
                    <td class="rightcol">&nbsp; Ширина:
                        <asp:textbox CssClass="textView" id="txtWidth" runat="server" Width="142px">100</asp:textbox>&nbsp; Высота:
                        <asp:textbox CssClass="textView" id="txtHeight" runat="server" Width="142px">100</asp:textbox>
                    </td>
                </tr>
                <tr>
                    <td class="leftcol">Вывод в текстовом виде:&nbsp;</td>
                    <td class="rightcol">&nbsp;<i><asp:checkbox CssClass="chk" id="chkTextOnly" runat="server" Text="(без HTML форматирования)"></asp:checkbox></i></td>
                </tr>
                <tr>
                    <td class="leftcol">Скачать ASCII-картинку:&nbsp;</td>
                    <td class="rightcol">&nbsp;<i><asp:checkbox CssClass="chk" id="chkDownload" runat="server" Text="(Скачать ASCII-картинку в формате HTML)"></asp:checkbox></i></td>
                </tr>
            </table>
            <table id="actionTable">
                <tr>
                    <td colSpan="2">
                        <asp:button CssClass="btn btn-1 btn-1e" id="btnAsciiFy" runat="server" Width="49%" Text="Конвертация картинки" onclick="btnConversion_Click"></asp:button>
                        <asp:button CssClass="btn btn-1 btn-1e" id="btnClear" runat="server" Width="49%" Text="Сброс настроек" onclick="btnClear_Click"></asp:button>
                    </td>
                </tr>
            </table>
            <table id="resultTable">
                <tr>
                    <td colSpan="2">
                        <h1>Исходное изображение</h1>
                    </td>
                </tr>
                <tr>
                    <td colSpan="2">
                        <asp:label id="lblError" runat="server" EnableViewState="False" Visible="False" Font-Bold="True"
                            ForeColor="DarkRed"></asp:label><asp:image id="imgOrigImage" runat="server" AlternateText="Невозможно отобразить изображение"></asp:image>
                    </td>
                </tr>
                <tr>
                    <td colSpan="2">
                        <h1>Результат</h1>
                    </td>
                </tr>
                <tr>
                    <td colSpan="2" title="ASCII Image"><asp:label id="lblAsciiImage" runat="server" EnableViewState="False" Text="Конвертация еще не была выполнена"></asp:label></td>
                </tr>
                <tr>
                    <td>
                        &nbsp;
                    </td>
                </tr>
                <tr>
                    <td>
                        &nbsp;
                    </td>
                </tr>
            </table>
        </form>
    </div>
    <div id="bottomPanel">
        <table id="bottomTable">
            <tr>
                <td id="copyright">
                    (C)2016 Happymilk™
                </td>
                <td>
                    <a href="terms.html">Правила</a>
                </td>
                <td>
                    <a href="help.html">Помощь</a>
                </td>
            </tr>
        </table>
    </div>
</body>
</html>
