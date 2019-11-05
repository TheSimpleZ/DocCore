using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using DocCore.Extensions;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using YamlDotNet.Core;
using YamlDotNet.Serialization;

namespace DocCore.DocProvider
{
    public class ClassDoc : Doc
    {
        public ClassDoc(ClassDeclarationSyntax node) : base(node)
        {
            ClassName = node.Identifier.ToString();
            Namespace = GetNamespace(node).Name.ToString();
            Constructors = node.Members.OfType<ConstructorDeclarationSyntax>().Where(DocExtensions.IsPublic).Select(ctor => new DocComment(ctor));
            Methods = node.Members.OfType<MethodDeclarationSyntax>().Where(DocExtensions.IsPublic).Select(ctor => new MethodDoc(ctor));
            Properties = node.Members.OfType<PropertyDeclarationSyntax>().Where(DocExtensions.IsPublic).Select(ctor => new PropertyDoc(ctor));
        }



        public string Namespace { get; set; }
        public string ClassName { get; set; }


        public IEnumerable<DocComment> Constructors { get; set; } = Enumerable.Empty<DocComment>();
        public IEnumerable<PropertyDoc> Properties { get; set; } = Enumerable.Empty<PropertyDoc>();
        public IEnumerable<MethodDoc> Methods { get; set; } = Enumerable.Empty<MethodDoc>();

        private NamespaceDeclarationSyntax GetNamespace(ClassDeclarationSyntax node) => node.Ancestors().OfType<NamespaceDeclarationSyntax>().Single();


        public override string ToString()
        {
            return (
$@"{ClassName}
======
##### Namespace: {Namespace}

{Comment.Summary}


{Declaration}


{string.Join("\n", Constructors)}

{string.Join("\n", Properties)}

{string.Join("\n", Methods)}
");
        }

    }
}