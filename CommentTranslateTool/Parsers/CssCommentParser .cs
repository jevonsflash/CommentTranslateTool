using System.Collections.Generic;

namespace Workshop.Parsers
{
    public class CssCommentParser : CommentParser
    {
        public CssCommentParser()
        {
            Tags = new List<ParseTag>
            {
                //Multi line comment
                new ParseTag(){
                    Start = "/*",
                    End = "*/",
                    Name = "comment"
                }
            };
        }
    }
}
