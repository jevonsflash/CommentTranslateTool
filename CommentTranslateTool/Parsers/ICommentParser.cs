using System.Collections.Generic;

namespace Workshop.Parsers
{
    public enum TextPositions
    {
        Bottom,
        Right
    }

    public interface ICommentParser
    {
        IEnumerable<CommentRegion> GetCommentRegions(string text, int startFrom = 0);
        Comment GetComment(string commentText);
        TextPositions GetPositions(Comment comment);
    }

    public class CommentRegion
    {
        public int Start { get; set; }
        public int Length { get; set; }
        public int End { get { return Start + Length; } }
        public ParseTag Tag { get; set; }

    }
}
