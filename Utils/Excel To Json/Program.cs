using System;
using System.IO;
using System.Runtime.InteropServices;
using Excel = Microsoft.Office.Interop.Excel;

namespace Excel_To_Json
{
    class Program
    {
        static void Main(string[] args)
        {
            string path = Path.Combine(Environment.CurrentDirectory, "Data.xlsx");

            Excel.Application app = new Excel.Application();
            Excel.Workbook workBook = app.Workbooks.Open(path);

            try
            {
                foreach (Excel.Worksheet sheet in workBook.Worksheets)
                {
                    string name = sheet.Name;
                    string filename = name + ".json";
                    string contents = "";
                    Excel.Range range = sheet.UsedRange;

                    Console.WriteLine($"Start Parsing {name.ToUpper()}");
                    contents = string.Format(JsonFormat.jsonFormat, ParseBasicData(name, range));
                    File.WriteAllText(Path.Combine(Environment.CurrentDirectory, filename), contents);
                }

                workBook.Close(true);
                app.Quit();
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
            finally
            {
                ReleaseObject(app);
                ReleaseObject(workBook);
            }

            path = Path.Combine(Environment.CurrentDirectory, "Round.xlsx");

            app = new Excel.Application();
            workBook = app.Workbooks.Open(path);

            try
            {
                string filename = "Round.json";
                string contents = "";
                for (int i = 1; i <= workBook.Worksheets.Count; i++)
                {
                    Excel.Worksheet sheet = workBook.Worksheets[i];

                    string name = sheet.Name;
                    Excel.Range range = sheet.UsedRange;

                    Console.WriteLine($"Start Parsing {name.ToUpper()}");
                    contents += string.Format(JsonFormat.contentsFormat, ParseRoundData(name, range));

                    if (i < workBook.Worksheets.Count) contents += ",\n";
                }
                contents = string.Format(JsonFormat.jsonFormat, contents);
                File.WriteAllText(Path.Combine(Environment.CurrentDirectory, filename), contents);
                workBook.Close(true);
                app.Quit();
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
            finally
            {
                ReleaseObject(app);
                ReleaseObject(workBook);
            }
        }


        private static string ParseBasicData(string dataName, Excel.Range range)
        {
            string contents = "";
            int count = 1;

            // 하나의 row는 하나의 데이터를 나타내고 있음.
            for (int row = 2; row <= range.Rows.Count; row++)
            {
                string datas = "";

                for (int column = 1; column <= range.Columns.Count; column++)
                {
                    object o = (range.Cells[row, column] as Excel.Range).Value2;
                    string type = (range.Cells[1, column] as Excel.Range).Value2;
                    string tmp = ParseValue(type.ToLower(), o);
                    if (tmp != null)
                    {
                        datas += tmp + ",\n";
                    }
                }
                if (datas != "")
                {
                    datas = datas.Remove(datas.Length - 2);
                    if (contents != "") contents += ",\n";
                    contents += string.Format(JsonFormat.contentsFormat, datas);
                }
                Console.WriteLine($"Progress {dataName.ToUpper()} {count++}");
            }
            return contents;
        }

        private static string ParseRoundData(string mapName, Excel.Range range)
        {
            string contents = "";
            string datas = "";
            contents = ParseValue("mapName", mapName) + ",";
            for (int row = 2; row <= range.Rows.Count; row++)
            {
                // column 1: units
                // column 2: amounts
                string data = "";

                for (int column = 1; column <= 2; column++)
                {
                    string type = (range.Cells[1, column] as Excel.Range).Value2;
                    object o = (range.Cells[row, column] as Excel.Range).Value2;
                    if (o == null) continue;
                    data += string.Format(JsonFormat.listFormat, type, o.ToString());
                    if (column < 2) data += ",";
                }
                datas += string.Format(JsonFormat.contentsFormat, data);
                if (row < range.Rows.Count) datas += ",\n";
            }
            contents += string.Format(JsonFormat.listFormat, "data", datas);
            return contents;
        }
        private static string ParseValue(string type, object value)
        {
            if (value == null) return null;

            string s = value.ToString();
            int i;
            float f, x, y;
            bool b;
            // 타입에 대한 체크
            if (int.TryParse(s, out i))
            {
                s = i.ToString();
            }
            else if (float.TryParse(s, out f))
            {
                s = f.ToString();
            }
            // bool
            else if (bool.TryParse(s, out b))
            {
                s = b.ToString().ToLower();
            }
            // vector2
            else if (TryParseVector2(s, out x, out y))
            {
                s = string.Format(JsonFormat.vector2Format, x, y);
            }
            // string
            else
            {
                s = "\"" + s + "\"";
            }
            return string.Format(JsonFormat.valueFormat, type, s);
        }
        private static bool TryParseVector2(string s, out float x, out float y)
        {
            string[] xy = s.Split(",");
            bool xb = float.TryParse(xy[0], out x);
            bool yb = float.TryParse(xy[0], out y);

            if (xb && yb) return true;
            return false;
        }

        private static void ReleaseObject(object obj)
        {
            try
            {
                if (obj != null)
                {
                    Marshal.ReleaseComObject(obj);  // 액셀 객체 해제
                    obj = null;
                }
            }
            catch (Exception ex)
            {
                obj = null;
                throw ex;
            }
            finally
            {
                GC.Collect();   // 가비지 수집
            }
        }
    }
}
