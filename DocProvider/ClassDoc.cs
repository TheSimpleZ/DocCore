using System.Collections.Generic;
using System.Linq;
using DocCore.Extensions;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace DocCore.DocProvider
{
    public class ClassDoc : Doc
    {
        public ClassDoc(ClassDeclarationSyntax node) : base(node)
        {
            ClassName = node.Identifier.ToString();
            Namespace = GetNamespace(node).Name.ToString();
            Constructors = node.Members.OfType<ConstructorDeclarationSyntax>().Where(DocExtensions.IsPublic).Select(ctor => new ConstructorDoc(ctor));
            Methods = node.Members.OfType<MethodDeclarationSyntax>().Where(DocExtensions.IsPublic).Select(ctor => new MethodDoc(ctor));
            Properties = node.Members.OfType<PropertyDeclarationSyntax>().Where(DocExtensions.IsPublic).Select(ctor => new PropertyDoc(ctor));
        }



        public string Namespace { get; set; }
        public string ClassName { get; set; }


        public IEnumerable<ConstructorDoc> Constructors { get; set; } = Enumerable.Empty<ConstructorDoc>();
        public IEnumerable<PropertyDoc> Properties { get; set; } = Enumerable.Empty<PropertyDoc>();
        public IEnumerable<MethodDoc> Methods { get; set; } = Enumerable.Empty<MethodDoc>();

        private NamespaceDeclarationSyntax GetNamespace(ClassDeclarationSyntax node) => node.Ancestors().OfType<NamespaceDeclarationSyntax>().Single();


        public override string ToString()
        {
            return (
$@"{ClassName}
======
> Namespace: {Namespace}

{Comment.Summary}

```
{Declaration}
```

{(Constructors.Any() ? "## Constructors" : string.Empty)}

{string.Join("\n", Constructors)}

{(Properties.Any() ? "## Properties" : string.Empty)}

{string.Join("\n", Properties)}

{(Methods.Any() ? "## Methods" : string.Empty)}

{string.Join("\n", Methods)}

");
        }

    }
}