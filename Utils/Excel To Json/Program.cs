using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices;
using Excel = Microsoft.Office.Interop.Excel;

namespace Excel_To_Json
{
    class Program
    {
        private static List<string> langContents = new List<string>();
        private static string[] langType = { "KOR", "ENG" };
        static void Main(string[] args)
        {
            string path = Path.Combine(Environment.CurrentDirectory, "Data.xlsx");

            Excel.Application app = new Excel.Application();
            Excel.Workbook workBook = app.Workbooks.Open(path);

            for (int i = 0; i < langType.Length; i++)
            {
                langContents.Add("");
            }
            try
            {
                // 일반 데이터
                foreach (Excel.Worksheet sheet in workBook.Worksheets)
                {
                    string name = sheet.Name;
                    string filename = name + ".json";
                    string contents = "";
                    string directory = "";
                    Excel.Range range = sheet.UsedRange;

                    Console.WriteLine($"Start Parsing {name.ToUpper()}");

                    if (name == "EnumData")
                    {
                        filename = name + ".cs";
                        contents = string.Format(JsonFormat.enumFileFormat, ParseEnumData(name, range));
                    }
                    else if (name == "Round")
                    {
                        contents = string.Format(JsonFormat.contentsFormat, ParseRoundData(name, range));
                    }
                    // AA @ BB
                    // 데이터 내에서 추가 구분이 이루어지는 데이터들 (Tower, Enemey)
                    else if (name.Contains("@"))
                    {
                        // 0: data, 1: language
                        string data = ParseBasicData(name, range);
                        contents = string.Format(JsonFormat.jsonFormat, data);

                        string[] n = name.Split("@");
                        directory = $"/{n[0]}";
                        name = n[1];
                        filename = n[1] + ".json";
                    }
                    // 나머지 데이터들
                    else
                    {
                        string data = ParseIdData(name, range);
                        contents = string.Format(JsonFormat.jsonFormat, data);
                    }

                    File.WriteAllText(Path.Combine(Environment.CurrentDirectory + directory, filename), contents);
                }
                for (int i = 0; i < langContents.Count; i++)
                {
                    langContents[i] = langContents[i].Remove(langContents[i].Length - 2);
                    string content = string.Format(JsonFormat.jsonFormat, langContents[i]);
                    File.WriteAllText(Path.Combine(Environment.CurrentDirectory + "/Language/", langType[i] + ".json"), content);
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
            finally
            {
                workBook.Close(true);
                app.Quit();

                ReleaseObject(app);
                ReleaseObject(workBook);
            }
        }

        // 데이터의 정보가 Language만 있는 경우
        // Sprite를 불러오기 위해 저장하는 정보임.
        private static string ParseIdData(string dataName, Excel.Range range)
        {
            string contents = "";
            int count = 1;

            // 하나의 row는 하나의 데이터를 나타내고 있음.
            for (int row = 2; row <= range.Rows.Count; row++)
            {
                string idData = "";
                string nameData = "";
                string descData = "";
                for (int column = 1; column <= range.Columns.Count; column++)
                {
                    object o = (range.Cells[row, column] as Excel.Range).Value2;
                    string type = (range.Cells[1, column] as Excel.Range).Value2;
                    if (type == null || o == null) continue;

                    if (type.ToUpper() == "ID")
                    {
                        contents += o.ToString() + ",\n";
                        idData = o.ToString();
                    }

                    if (type.ToUpper() == "NAME")
                        nameData = o.ToString();

                    if (type.ToUpper() == "DESC")
                        descData = o.ToString();
                }
                ParseLanguage(idData, nameData, descData);
                count++;
            }
            // 마지막에 붙은 ,와 \n 제거
            if (contents != "")
                contents = contents.Remove(contents.Length - 2);
            Console.WriteLine($"Progress {dataName.ToUpper()} {count}");
            return contents;
        }

        private static string ParseBasicData(string dataName, Excel.Range range)
        {
            string contents = "";
            int count = 1;

            // 하나의 row는 하나의 데이터를 나타내고 있음.
            for (int row = 2; row <= range.Rows.Count; row++)
            {
                string data = "";
                string idData = "";
                string nameData = "";
                for (int column = 1; column <= range.Columns.Count; column++)
                {
                    object o = (range.Cells[row, column] as Excel.Range).Value2;
                    string type = (range.Cells[1, column] as Excel.Range).Value2;
                    if (type == null || o == null) continue;
                    string value = ParseValue(type.ToLower(), o);
                    if (value != null)
                    {
                        if (type.ToUpper() == "ID")
                            idData = o.ToString();

                        if (type.ToUpper() == "NAME")
                            nameData = o.ToString();

                        if (type.ToUpper() != "NAME") data += value + ",\n";
                    }
                }
                // BasicData의 경우 아직은 descData가 필요 없음.
                ParseLanguage(idData, nameData, "");

                if (data != "")
                {
                    // 마지막에 붙은 ,와 \n 제거
                    data = data.Remove(data.Length - 2);
                    if (contents != "") contents += ",\n";
                    contents += string.Format(JsonFormat.contentsFormat, data);
                }
                count++;
            }
            Console.WriteLine($"Progress {dataName.ToUpper()} {count}");
            return contents;
        }

        private static void ParseLanguage(string id, string n, string d)
        {
            // Array와 일반 string으로 구분
            string[] names = n.Split(",");
            string[] descs = null;
            if (string.IsNullOrEmpty(d) == false) descs = d.Split(",");
            if (n.Length > 1)
            {
                for (int i = 0; i < names.Length; i++)
                {
                    string str = ParseValue("id", id) + "," + ParseValue("name", names[i]);

                    // desc는 empty string인 경우도 있으므로 해당 경우에는 배제해주어야 함.
                    if (descs != null)
                        str += "," + ParseValue("desc", descs[i]);

                    langContents[i] += string.Format(JsonFormat.contentsFormat, str) + ",\n";
                }
            }
        }

        private static string ParseRoundData(string mapName, Excel.Range range)
        {
            string contents = "";
            string datas = "";
            //contents = ParseValue("mapName", mapName) + ",";
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
                    if (o.Contains("=")) data += o + ",";
                    else data += string.Format(JsonFormat.enumDataFormat, o, count++);
                }
                contents += string.Format(JsonFormat.enumFormat, type, data) + "\n";
            }

            string array = "";
            string dics = "";
            string init = "";
            for (int i = 0; i < types.Count; i++)
            {
                array += string.Format(JsonFormat.enumArrayFormat, types[i]) + "\n";
                dics += string.Format(JsonFormat.enumArrayStringFormat, types[i]) + "\n";
                init += string.Format(JsonFormat.enumArrayInitFormat, types[i]) + "\n";
            }

            array += dics;

            contents += string.Format(JsonFormat.enumArrayClassForamt, array, init);

            return contents;
        }

        private static string ParseValue(string type, object value)
        {
            if (value == null) return null;

            string s = value.ToString();
            int i;
            float[] fa;
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
            // ability class
            else if (TryParseAbility(s, out types, out values))
            {
                string content = "";
                for (int a = 0; a < types.Length; a++)
                {
                    content += string.Format(JsonFormat.abilityFormat, types[a], values[a]);
                    if (a < types.Length - 1) content += ",\n";
                }

                s = string.Format(JsonFormat.listFormat, type, content);
                return s;
            }
            // list
            else if (TryParseArrayFloat(s, out fa))
            {
                string content = "";
                for (int a = 0; a < fa.Length; a++)
                {
                    content += fa[a];
                    if (a < fa.Length - 1) content += ",";
                }
                s = string.Format(JsonFormat.listFormat, type, content);
                return s;
            }
            else if (TryParseColor(s, out values))
            {
                s = string.Format(JsonFormat.colorFormat, values[0], values[1], values[2], values[3]);
            }
            // vector2
            else if (TryParseVector2(s, out x, out y))
            {
                s = string.Format(JsonFormat.vector2Format, x, y);
            }
            // string
            else
            {
                // Array와 일반 string으로 구분
                string[] l = s.Split(",");
                if (l.Length > 1)
                {
                    string content = "";
                    for (int a = 0; a < l.Length; a++)
                    {
                        content += "\"" + l[a] + "\"";
                        if (a < l.Length - 1) content += ",";
                    }

                    s = string.Format(JsonFormat.listFormat, type, content);
                    return s;
                }
                else s = "\"" + s + "\"";
            }
            return string.Format(JsonFormat.valueFormat, type, s);
        }

        private static bool TryParseArrayFloat(string s, out float[] a)
        {
            string[] split = s.Split(";");
            a = new float[split.Length - 1];

            if (split.Length == 1) return false;

            for (int i = 0; i < a.Length; i++)
            {
                if (float.TryParse(split[i], out a[i]) == false) return false;
            }

            return true;
        }

        private static bool TryParseVector2(string s, out float x, out float y)
        {
            string[] xy = s.Split(",");
            x = 0.5f;
            y = 0.5f;
            if (xy.Length != 2) return false;

            if (float.TryParse(xy[0], out x) == false) return false;
            if (float.TryParse(xy[1], out y) == false) return false;

            return true;
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
                if (tv.Length != 2) return false;

                if (int.TryParse(tv[0], out types[i]) == false) return false;
                if (float.TryParse(tv[1], out values[i]) == false) return false;
            }

            return true;
        }
        private static bool TryParseColor(string s, out float[] color)
        {
            // (1,1,1,1) 의 형태
            s = s.Trim('(', ')');
            string[] rgba = s.Split(",");
            color = null;
            if (rgba.Length != 4) return false;

            color = new float[4];
            for (int i = 0; i < 4; i++)
            {
                if (float.TryParse(rgba[i], out color[i]) == false) return false;
                // 색의 값은 0~1 사이의 실수
                if (color[i] < 0) color[i] = 0;
                if (color[i] > 1) color[i] = 1;
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
