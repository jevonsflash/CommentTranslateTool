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
        public static List<ParserProvider> GetAllProviders()
        {
            var ParserProviders = new List<ParserProvider>();
            ParserProviders.Add(new ParserProvider()
            {
                Name = "C#",
                Type = "C#",
                FileExtension = ".cs",
                ParserType = typeof(CSharpCommentParser)

            });
            ParserProviders.Add(new ParserProvider()
            {
                Name = "C++",
                Type = "C++",
                FileExtension = ".cpp",
                ParserType = typeof(CppCommentParser)

            });
            ParserProviders.Add(new ParserProvider()
            {
                Name = "JavaScript",
                Type = "JavaScript",
                FileExtension = ".js|.ts",
                ParserType = typeof(JavaScriptCommentParser)

            });
            ParserProviders.Add(new ParserProvider()
            {
                Name = "CSS",
                Type = "CSS",
                FileExtension = ".css",
                ParserType = typeof(CssCommentParser)

            });
            ParserProviders.Add(new ParserProvider()
            {
                Name = "Python",
                Type = "Python",
                FileExtension = ".py",
                ParserType = typeof(PythonCommentParser)

            });
            ParserProviders.Add(new ParserProvider()
            {
                Name = "VB",
                Type = "VB",
                FileExtension = ".vb",
                ParserType = typeof(VBCommentParser)

            });
            ParserProviders.Add(new ParserProvider()
            {
                Name = "HTML",
                Type = "HTML",
                FileExtension = ".html",
                ParserType = typeof(HtmlCommentParser)

            });
            ParserProviders.Add(new ParserProvider()
            {
                Name = "Xaml",
                Type = "XML",
                FileExtension = ".xaml",

                ParserType = typeof(XamlCommentParser)

            });
            ParserProviders.Add(new ParserProvider()
            {
                Name = "XML",
                Type = "XML",
                FileExtension = ".xml|.xsl|.xslt|.xsd|.manifest|.config|.addin|" +
                                ".xshd|.wxs|.wxi|.wxl|.proj|.csproj|.vbproj|.ilproj|" +
                                ".booproj|.build|.xfrm|.targets|.xpt|" +
                                ".xft|.map|.wsdl|.disco|.ps1xml|.nuspec",
                ParserType = typeof(XmlCommentParser)

            });
            return ParserProviders;

        }
        public string Name { get; set; }
        public string Type { get; set; }
        public Type ParserType { get; set; }
        public string FileExtension { get; set; }

        public List<string> FileExtensionList => FileExtension.Split('|').ToList();
        public IHighlightingDefinition HighlightingDefinition => HighlightingManager.Instance.HighlightingDefinitions.FirstOrDefault(c => c.Name == this.Type);

        public ICommentParser CreateCommentParser()
        {
            var instance = Activator.CreateInstance(ParserType) as ICommentParser;
            return instance;
        }
    }
}
