using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace DocCore.DocProvider
{
    public class PropertyDoc : Doc
    {
        public PropertyDoc(PropertyDeclarationSyntax prop) : base(prop)
        {
            Prop = prop;
        }

        public PropertyDeclarationSyntax Prop { get; }

        public override string ToString()
        {
            return
$@"{Prop.Identifier}
------
{Comment.Summary}
{Declaration}

";
        }
    }
}