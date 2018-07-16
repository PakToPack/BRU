using System;
using System.Text;
using HtmlAgilityPack;
using System.Net;
using System.Text.RegularExpressions;
using FixTricks.constructors;
using SQLite;
using Syncfusion.XlsIO;
using System.IO;
using System.Collections.Generic;

namespace FixTricks.scripts
{
    class ReloadData
    {
        public string[] user { get; set; } = null;
        public string parseLine { get; set; }
        public string[,] parsed_str { get; set; }
        public string[] rates { get; set; }
        protected string studak { get; set; }
        public string report { get; set; }
        public int parsed_str_i { get; set; }
        public int parsed_str_j { get; set; }
        public string exc_err { get; set; }

        public bool LoadDB()
        {
            DBControl dbc = new DBControl();
            dbc.DestroyDB();
            dbc.CreateDB();
            return true;
        }

        public string[,] LoadCourses()
        {
            string[,] _string;
            string html = @"http://bru.by/content/student/timetable";
            HtmlDocument htmlDoc = new HtmlDocument();

            string page;

            try
            {
                using (WebClient client = new WebClient())
                {
                    client.Encoding = Encoding.UTF8;
                    page = Regex.Replace(Regex.Replace(Regex.Replace(Regex.Replace(Regex.Replace(client.DownloadString(html), "(?i)<strong[^>]*>", ""), "(?i)<br[^>]*>", "\n"), "&#8209;", "-"), "&nbsp;", " "), "\nДата", " ");
                }
                htmlDoc.LoadHtml(page);
                HtmlNodeCollection linkParent = htmlDoc.DocumentNode.SelectNodes("//*[text()[contains(., 'Инженерно-экономический факультет')]]");
                var node = linkParent[0];
                HtmlNode ul = node.ParentNode.ChildNodes.FindFirst("ul");
                HtmlNodeCollection li_s = ul.SelectNodes("li");
                int count = 0;
                foreach (HtmlNode a in li_s)
                    count++;
                _string = new string[count, 2];
                for (int i = 0; i < count; i++)
                {
                    HtmlNode a = li_s[i].FirstChild;
                    _string[i, 0] = a.InnerText;
                    _string[i, 1] = a.Attributes["href"].Value;
                }
                return _string;
            }
            catch
            {

            }
            return null;
        }

        public bool ReloadRate(string studakk, int mygroup, string _group = "")
        {
            SQLiteConnection db = new SQLiteConnection(SysPath.DBPath);
            DBControl dbc = new DBControl();
            dbc.ReloadRates();

            string html = @"http://vuz2.bru.by/rate/" + studakk + @"/";
            HtmlDocument htmlDoc = new HtmlDocument();

            string page;

            try
            {
                using (WebClient client = new WebClient())
                {
                    client.Encoding = Encoding.GetEncoding(1251);
                    page = Regex.Replace(Regex.Replace(Regex.Replace(Regex.Replace(Regex.Replace(client.DownloadString(html), "(?i)<strong[^>]*>", ""), "(?i)<br[^>]*>", "\n"), "&#8209;", "-"), "&nbsp;", " "), "\nДата", " ");
                }
                htmlDoc.LoadHtml(page);
                var Name = htmlDoc.GetElementbyId("user").FirstChild.FirstChild.FirstChild.FirstChild.InnerHtml;
                string[] prom = new string[12];

                var nodes = htmlDoc.GetElementbyId("user").ChildNodes;
                parsed_str = new string[nodes.Count - 2, nodes[1].ChildNodes.Count - 1];
                parsed_str_i = nodes.Count - 2;
                parsed_str_j = nodes[1].ChildNodes.Count - 1;
                string[] ratex = new string[nodes[1].ChildNodes.Count - 1];
                for (int i = 1; i < nodes.Count - 1; i++)
                {
                    var th_ = nodes[i].ChildNodes;
                    ratex[i - 1] = Regex.Replace(Regex.Replace(th_[0].InnerHtml, "(?i)<font[^>]*>", ""), "(?i)</font[^>]*>", "");
                    for (int j = 1; j < th_.Count; j++)
                    {
                        parsed_str[i - 1, j - 1] = Regex.Replace(Regex.Replace(th_[j].InnerHtml, "(?i)<font[^>]*>", ""), "(?i)</font[^>]*>", "");
                        parseLine += "\n" + parsed_str[i - 1, j - 1];
                    }
                }
                rates = ratex;

                if (Name != null)
                {
                    string[] user = Name.Split(' ');
                    this.user = user;
                    this.report = "success";
                }
                else
                {
                    this.report = "No user";
                    this.user = null;
                }
            }
            catch (Exception e)
            {
                this.report = "\n" + e.Message + "\n" + e.Source;
            }
            //Trying to save data
            try
            {
                if (this.report == "success")
                {
                    var User = new Users() { group = _group, name = user[0], surname = user[1], podgroup = mygroup, studak = studakk };
                    db.Insert(User);
                    var arr_rates = parsed_str;
                    string[] arr_raten = rates;
                    for (int j = 0; j < parsed_str_j; j++)
                    {
                        for (int i = 0; i < parsed_str_i; i++)
                        {
                            if (i == 0)
                                db.Insert(new DBDisciplines() { name = arr_rates[i, j] });
                            else
                                db.Insert(new AllMyRates() { discipline = arr_rates[0, j], mark = arr_rates[i, j], rate = arr_raten[i] });
                        }
                    }
                    return true;
                }
            }
            catch
            {

            }
            //Something went wrong
            return false;
        }

        public string[] LoadGroups(string link)
        {
            SQLiteConnection db = new SQLiteConnection(SysPath.DBPath);

            WebClient wc = new WebClient();
            if (File.Exists(SysPath.ExcelPath))
                File.Delete(SysPath.ExcelPath);
            var url = new Uri(link);
            wc.DownloadFile(url, SysPath.ExcelPath);

            using (ExcelEngine excelEngine = new ExcelEngine())
            {
                try
                {
                    FileStream fs = new FileStream(SysPath.ExcelPath, FileMode.Open, FileAccess.Read);
                    excelEngine.Excel.DefaultVersion = ExcelVersion.Excel2016;

                    IWorkbook wb = excelEngine.Excel.Workbooks.Open(fs);
                    IWorksheet sheet = wb.Worksheets[0];

                    IRange iRange = sheet.FindFirst("1 подгр.", ExcelFindType.Text);
                    int start_Row = iRange.Row - 1;
                    int start_Column = iRange.Column;
                    IRange used = sheet.UsedRange;
                    int last = used.LastColumn;

                    int gr_count = 0;
                    for (int i = start_Column; i < last; i += 2)
                        if (sheet.Range[start_Row, i].Value != "" && sheet.Range[start_Row, i].Value != null)
                            gr_count++;

                    string[] groups = new string[gr_count];

                    for (int i = start_Column; i < last; i += 2)
                    {
                        if (sheet.Range[start_Row, i].Value == "" || sheet.Range[start_Row, i].Value == null)
                            continue;
                        groups[(i-start_Column) / 2] = sheet.Range[start_Row, i].Value;
                    }
                    //Закрываем чтение потока
                    fs.Close();
                    //Закрываем движок Excel
                    wb.Close();
                    excelEngine.Dispose();
                    return groups;
                }
                catch
                {
                }
            }
            return null;
        }


        public string TransformString(string str)
        {
            string res = str;
            string[] keys = { "П Р О Г Р А М М И Р О В А Н И Е", "Ф И З И Ч Е С К А Я К У Л Ь Т У Р А", "М А Т Е М А Т И К А", "Ф И З И К А", "Д И С К Р Е Т Н А Я", "К О М П Ь Ю Т Е Р Н А Я", "Г Р А Ф И К А", "Э Л Е К Т И В Н Ы Е К У Р С Ы П О Ф И З И Ч Е С К О Й К У Л Ь Т У Р Е", "Э К О Н О М И К А", "ОБЪЕКТНО-ОРИЕНТИРОВАННОЕ ПРОГРАММИРОВАНИЕ" };
            string[] values = { "Программирование", "Физра", "Математика", "Физика", "Дискретная", "Компьютерная", "Графика", "Элект.курсы по физре", "Экономика", "ООП" };
            for (int i = 0; i < keys.Length; i++)
            {
                res = res.Replace(keys[i], values[i]);
            }
            return res;
        }


        public bool ReloadExcel(string link)
        {
            List<ParaX> plist = new List<ParaX>();
            SQLiteConnection db = new SQLiteConnection(SysPath.DBPath);
            Users usr = db.Table<Users>().First();
            DBControl dbc = new DBControl();
            dbc.ReloadExcel();
            int podgr = usr.podgroup - 1;
            string group = usr.group;
            try
            {
                WebClient wc = new WebClient();
                if (File.Exists(SysPath.ExcelPath))
                    File.Delete(SysPath.ExcelPath);
                var url = new Uri(link);
                wc.DownloadFile(url, SysPath.ExcelPath);

                using (ExcelEngine excelEngine = new ExcelEngine())
                {
                    FileStream fs = new FileStream(SysPath.ExcelPath, FileMode.Open, FileAccess.Read);
                    excelEngine.Excel.DefaultVersion = ExcelVersion.Excel2016;

                    IWorkbook wb = excelEngine.Excel.Workbooks.Open(fs);
                    IWorksheet sheet = wb.Worksheets[0];

                    IRange iRange = sheet.FindFirst(group, ExcelFindType.Text);
                    int para = 0, dayc = 1;
                    //Верхняя неделя нужной подгруппы
                    for (int i = 2; i < 57; i += 2)
                    {
                        if (para == 5)
                        {
                            para = 1;
                            dayc++;
                        }
                        else
                            para++;
                        string dop = "";

                        if (sheet.Range[iRange.Row + i, iRange.Column + podgr].IsMerged)
                        {

                            string address2;
                            int[] coord2;
                            string xxx2;
                            address2 = Regex.Replace(sheet.Range[iRange.Row + i + 1, iRange.Column + podgr].MergeArea.Address.ToString().Split(':')[0].Replace("$", "").Replace("!", ""), @"'[^>]*'", "");
                            coord2 = GetAddress(address2);
                            xxx2 = sheet.Range[coord2[1], coord2[0]].Value;

                            if (xxx2.IndexOf("а.") != -1 && xxx2.IndexOf("а.") == 0)
                                dop = xxx2;

                            string address;
                            int[] coord;
                            string xxx;
                            
                            address = Regex.Replace(sheet.Range[iRange.Row + i, iRange.Column + podgr].MergeArea.Address.ToString().Split(':')[0].Replace("$", "").Replace("!", ""), @"'[^>]*'", "");
                            coord = GetAddress(address);
                            xxx = sheet.Range[coord[1], coord[0]].Value;
                            if (xxx != null && xxx.Replace(" ", "") != "") plist.Add(new ParaX() { day = dayc, name = TransformString(Regex.Replace(xxx + " " + dop, "[ ]+", " ")), number = para, week = 1 });
                        }
                        else
                        {
                            if (sheet.Range[iRange.Row + i, iRange.Column + podgr].Text != null && (sheet.Range[iRange.Row + i, iRange.Column + podgr].Text).Replace(" ", "") != "")
                                plist.Add(new ParaX() { day = dayc, name = TransformString(Regex.Replace(sheet.Range[iRange.Row + i, iRange.Column + podgr].Text, "[ ]+", " ")), number = para, week = 1 });
                        }
                        dop = "";
                    }
                    //Нижняя неделя нужной подгруппы
                    dayc = 1;
                    para = 0;
                    for (int i = 3; i < 57; i += 2)
                    {
                        if (para == 5)
                        {
                            para = 1;
                            dayc++;
                        }
                        else
                            para++;

                        if (sheet.Range[iRange.Row + i, iRange.Column + podgr].IsMerged)
                        {
                            string bcp;
                            string address;
                            int[] coord;
                            string xxx;

                            address = Regex.Replace(sheet.Range[iRange.Row + i, iRange.Column + podgr].MergeArea.Address.ToString().Split(':')[0].Replace("$", "").Replace("!", ""), @"'[^>]*'", "");
                            coord = GetAddress(address);
                            xxx = sheet.Range[coord[1], coord[0]].Value;
                            int xr;
                            if (((xr = xxx.IndexOf("доц")) != -1 && xr == 0) || ((xr = xxx.IndexOf("а.")) != -1 && xr == 0))
                            {
                                bcp = xxx;
                                address = Regex.Replace(sheet.Range[iRange.Row + i - 1, iRange.Column + podgr].MergeArea.Address.ToString().Split(':')[0].Replace("$", "").Replace("!", ""), @"'[^>]*'", "");
                                coord = GetAddress(address);
                                xxx = sheet.Range[coord[1], coord[0]].Value + " " + bcp;
                            }
                            if (xxx != null && xxx.Replace(" ", "") != "") plist.Add(new ParaX() { day = dayc, name = TransformString(Regex.Replace(xxx, "[ ]+", " ")), number = para, week = 2 });
                        }
                        else
                        {
                            if (sheet.Range[iRange.Row + i, iRange.Column + podgr].Text != null && (sheet.Range[iRange.Row + i, iRange.Column + podgr].Text).Replace(" ", "") != "")
                                plist.Add(new ParaX() { day = dayc, name = TransformString(Regex.Replace(sheet.Range[iRange.Row + i, iRange.Column + podgr].Text, "[ ]+", " ")), number = para, week = 2 });
                        }
                    }

                    foreach (ParaX pxx in plist)
                    {
                        db.Insert(pxx);
                    }

                    DateTime dTime = DateTime.UtcNow.Date;

                    int month = dTime.Month;

                    string[] months = { "", "январь", "февраль", "март", "апрель", "май", "июнь", "июль", "август", "сентябрь", "октябрь", "декабрь" };

                    IRange iRange2 = sheet.FindFirst(months[month], ExcelFindType.Text);

                    int j = 0;
                    while (sheet.Range[iRange2.Row + 4, iRange2.Column + j].Text != null && Regex.Replace(sheet.Range[iRange2.Row + 4, iRange2.Column + j].Text, "[ ]+", " ") != " ")
                    {
                        int nmonth = 0;
                        for(int i = 1; i < months.Length; i++)
                        {
                            if (months[i] == sheet.Range[iRange2.Row, iRange2.Column + j].Text)
                            {
                                nmonth = i;
                                break;
                            }
                        }
                        string verh = Regex.Replace(sheet.Range[iRange2.Row + 4, iRange2.Column + j].Text, "[ ]+", " ");
                        string nizh = Regex.Replace(sheet.Range[iRange2.Row + 5, iRange2.Column + j].Text, "[ ]+", " ");

                        db.Insert(new WeekSelect() { days = verh, month = nmonth, week_t = 1 });
                        db.Insert(new WeekSelect() { days = nizh, month = nmonth, week_t = 2 });
                        j++;
                    }

                    //Закрываем стрим
                    fs.Close();
                    //Закрываем движок Excel
                    wb.Close();
                    excelEngine.Dispose();
                }
                dbc.UpdateAppLink(link);
                return true;
            }
            catch (Exception e)
            {
                this.exc_err = e.Message + "\n" + e.Source;
                return false;
            }
        }

        static int[] GetAddress(string adr)
        {
            char[] alphabet = new char[26] { 'A', 'B', 'C', 'D', 'E', 'F', 'G', 'H', 'I', 'J', 'K', 'L', 'M', 'N', 'O', 'P', 'Q', 'R', 'S', 'T', 'U', 'V', 'W', 'X', 'Y', 'Z' };
            int[] arr = new int[2];
            for (int i = 0; i < alphabet.Length; i++)
            {
                if (adr[0] == alphabet[i])
                {
                    arr[0] = i + 1;
                    break;
                }
            }

            string sec = adr.Replace(adr[0].ToString(), "");
            arr[1] = int.Parse(sec);
            return arr;
        }

    }
}
