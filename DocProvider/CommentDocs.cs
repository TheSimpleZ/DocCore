using System.Collections.Generic;
namespace DocCore.DocProvider
{
    public class CommentDocs
    {
        public string Summary { get; set; }
        public IDictionary<string, string> Parameters { get; set; }
    }
}
