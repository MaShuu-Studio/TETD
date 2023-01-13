using System;
using System.Collections.Generic;
using System.Text;

namespace Excel_To_Json
{
    class JsonFormat
    {
        // {0} 콘텐츠 목록 (contentsFormat)
        public static string jsonFormat =
@"
{{
    ""list"": 
    [
        {0}
    ]
}}
";
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
    }
}
