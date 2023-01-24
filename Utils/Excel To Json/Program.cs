using System;
using System.Collections.Generic;
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

                    if (name != "EnumData")
                        contents = string.Format(JsonFormat.jsonFormat, ParseBasicData(name, range));
                    else
                    {
                        filename = name + ".cs";
                        contents = string.Format(JsonFormat.enumFileFormat, ParseEnumData(name, range));
                    }
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

        private static string ParseEnumData(string name, Excel.Range range)
        {
            string contents = "";
            List<string> types = new List<string>();
            for (int row = 1; row <= range.Rows.Count; row++)
            {
                // column 1: enumType
                // column 2-: enumData
                string data = "";
                string type = (range.Cells[row, 1] as Excel.Range).Value2;
                types.Add(type);
                int count = 0;
                for (int column = 2; column <= range.Columns.Count; column++)
                {
                    string o = (range.Cells[row, column] as Excel.Range).Value2;
                    if (string.IsNullOrEmpty(o)) continue;
                    data += string.Format(JsonFormat.enumDataFormat, o, count++);
                }
                contents += string.Format(JsonFormat.enumFormat, type, data) + "\n";
            }

            string array = "";
            for (int i = 0; i < types.Count; i++)
            {
                array += string.Format(JsonFormat.enumArrayFormat, types[i]) + "\n";
            }

            contents += string.Format(JsonFormat.enumArrayClassForamt, array);

            return contents;
        }

        private static string ParseValue(string type, object value)
        {
            if (value == null) return null;

            string s = value.ToString();
            int i;
            float f, x, y;
            bool b;
            int[] types;
            float[] values;
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
            // 
            else if (TryParseAbility(s, out types, out values))
            {
                string content = "";
                for (int a =0; a < types.Length; a++)
                {
                    content += string.Format(JsonFormat.abilityFormat, types[a], values[a]);
                    if (a < types.Length - 1) content += ",\n";
                }

                s = string.Format(JsonFormat.listFormat, type, content);
                return s;
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
            x = 0.5f;
            y = 0.5f;
            if (xy.Length != 2) return false;

            bool xb = float.TryParse(xy[0], out x);
            bool yb = float.TryParse(xy[1], out y);

            if (xb && yb) return true;
            return false;
        }

        private static bool TryParseAbility(string s, out int[] types, out float[] values)
        {
            string[] abilities = s.Split(";");
            types = null;
            values = null;
            if (abilities.Length <= 1) return false;

            types = new int[abilities.Length - 1];
            values = new float[abilities.Length - 1];

            for (int i = 0; i < abilities.Length - 1; i++)
            {
                string[] tv = abilities[i].Split(",");
                int.TryParse(tv[0], out types[i]);
                float.TryParse(tv[1], out values[i]);
            }

            return true;
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
