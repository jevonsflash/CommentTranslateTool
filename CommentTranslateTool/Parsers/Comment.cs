namespace Workshop.Parsers
{
    public class Comment
    {
        public string Origin { get; set; }
        public string Content { get; set; }
        public TextPositions Position { get; set; }
        public int Line { get; set; }
        public int MarginTop { get; set; }
    }
}
