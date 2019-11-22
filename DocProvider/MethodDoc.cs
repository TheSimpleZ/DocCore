using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace DocCore.DocProvider
{
    public class MethodDoc : Doc
    {

        public MethodDoc(BaseMethodDeclarationSyntax method) : base(method)
        {
            Method = method;
        }

        public BaseMethodDeclarationSyntax Method { get; }

        public string ParameterTypes => string.Join(", ", Method.ParameterList.Parameters.Select(p => p.Type.ToString()));

        public override string ToString()
        {
            var returnString = string.IsNullOrEmpty(Comment.Returns) ? "" : $"**Returns:** {Comment.Returns}";
            return
$@"{(Method as MethodDeclarationSyntax)?.Identifier.ToString() ?? "Anon"}({ParameterTypes})
------
{Comment.Summary}

```
{Declaration}
```

{ParameterTable}
{returnString}

";
        }
    }
}