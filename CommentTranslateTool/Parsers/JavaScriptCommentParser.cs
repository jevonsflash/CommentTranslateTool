using System;
using System.Collections.Generic;

namespace Workshop.Parsers
{
    public class JavaScriptCommentParser : CommentParser
    {
        public JavaScriptCommentParser()
        {
            Tags = new List<ParseTag>
            {
                new ParseTag()
                {
                    Start = "/*",
                    End = "*/",
                    Name = "multiline"
                },
                new ParseTag()
                {
                    Start = "//",
                    End = Environment.NewLine,
                    Name = "comment"
                },
                new ParseTag()
                {
                    Start = "//",
                    End = "",
                    Name = "singlelineend"
                },
            };
        }
    }
}
