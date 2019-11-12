using System.Text;
using DocCore.Extensions;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace DocCore.DocProvider
{
    public class ConstructorDoc : MethodDoc
    {

        public ConstructorDoc(ConstructorDeclarationSyntax ctor) : base(ctor)
        {
            Ctor = ctor;
        }

        public ConstructorDeclarationSyntax Ctor { get; }

        public override string ToString()
        {
            return
$@"{Ctor.Identifier}({ParameterTypes})
------
{Comment.Summary}

```
{Declaration}
```

{ParameterTable}

";
        }
    }
}