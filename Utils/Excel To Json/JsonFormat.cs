using System;
using System.Collections.Generic;
using System.Text;

namespace Excel_To_Json
{
    class JsonFormat
    {
        // {0} 콘텐츠 목록 (contentsFormat)
        public static string jsonFormat =
@"{{
    ""list"": 
    [
        {0}
    ]
}}";
        // {0} 데이터 내용(vauleFormat)
        public static string contentsFormat =
@"
{{
    {0}
}}";
        // {0} 타입 이름
        // {1} 타입 값
        public static string valueFormat =
@"""{0}"": {1}";

        // {0 - 2} x, y, z
        public static string vector2Format =
@"{{ ""x"": {0}, ""y"": {1} }}";

        public static string vector3Format =
@"{{ ""x"": {0}, ""y"": {1}, ""z"": {2} }}";

        // {0} list 이름
        // {1} 콘텐츠 목록 (contentsFormat)
        public static string listFormat =
@"    ""{0}"": [ {1} ]";
        // {0} abliity Type (enum)
        // {1} ability Value (float)
        public static string abilityFormat =
@"{{""type"": {0}, ""value"": {1}}}";
        // {0} 각 enum
        // {1} 각 enum의 Array
        public static string enumFileFormat =
@"using System;
using System.Collections.Generic;

namespace EnumData
{{
{0}
}}
";
        // {0} Type 이름
        // {1} 데이터
        public static string enumFormat =
@"  public enum {0} {{ {1}}}";
        // {0} 데이터 이름
        // {1} 데이터 번호
        public static string enumDataFormat =
@"{0} = {1}, ";

        // {0} 데이터 내용
        // {1} Init 초기화 내용
        public static string enumArrayClassForamt =
@"
    public static class EnumArray
    {{
        {0}

        public static void Init()
        {{
            {1}
        }}
    }}";

        // {0} Type 이름
        public static string enumArrayFormat =
@"public static {0}[] {0}s {{ get; private set; }} = ({0}[])Enum.GetValues(typeof({0}));";
        // {0} Type 이름
        public static string enumArrayStringFormat =
@"public static Dictionary<{0},string> {0}Strings {{ get; private set; }}";
        // {0} Type 이름
        public static string enumArrayInitFormat =
@"
            {0}Strings = new Dictionary<{0}, string>();
            for (int i = 0; i < {0}s.Length; i++)
            {{
                {0} type = {0}s[i];
                {0}Strings.Add(type, type.ToString()); 
            }}";
    }
}
