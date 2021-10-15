using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ICSharpCode.AvalonEdit.Highlighting;
using Workshop.Parsers;

namespace Workshop.Model
{
    public class ParserProvider
    {

        public string Name { get; set; }
        public string Type { get; set; }
        public Type ParserType { get; set; }

        public IHighlightingDefinition HighlightingDefinition => HighlightingManager.Instance.HighlightingDefinitions.FirstOrDefault(c => c.Name == this.Type);
        
        public ICommentParser CreateCommentParser()
        {
            var instance = Activator.CreateInstance(ParserType) as ICommentParser;
            return instance;
        }
    }
}
