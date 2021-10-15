using System.Collections.Generic;

namespace Workshop.Parsers
{
    public class RazorCommentParser : CommentParser
    {
        public RazorCommentParser()
        {
            Tags = new List<ParseTag>
            {
                //Multi line comment
                new ParseTag(){
                    Start = "@*",
                    End = "*@",
                    Name = "multiline2"
                }
            };
        }
    }
}
