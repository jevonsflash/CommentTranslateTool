using System.Collections.Generic;

namespace Workshop.Parsers
{
    public class XamlCommentParser : CommentParser
    {
        public XamlCommentParser()
        {
            Tags = new List<ParseTag>
            {
                //Multi line comment
                new ParseTag(){
                    Start = "<!--",
                    End = "-->",
                    Name = "multiline"
                }
            };
        }
    }
}
