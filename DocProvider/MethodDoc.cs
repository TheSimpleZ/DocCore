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


        protected string ParameterTable
        {
            get
            {
                if (!Method.ParameterList.Parameters.Any())
                    return "";

                var builder = new StringBuilder("### Parameters\n");
                builder.AppendLine("Name | Description");
                builder.AppendLine("--- | ---");
                foreach (var param in Method.ParameterList.Parameters)
                {
                    builder.AppendLine($"{param.Identifier} | {Comment.Parameters?[param.Identifier.ToString()]}");
                }
                return builder.ToString();
            }
        }

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