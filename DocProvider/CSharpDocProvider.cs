using DocCore.Extensions;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using YamlDotNet.Core;
using YamlDotNet.Serialization;

namespace DocCore.DocProvider
{
    public class CSharpDocProvider
    {
        private readonly CompilationUnitSyntax root;
        public CSharpDocProvider(SyntaxTree tree)
        {
            root = (CompilationUnitSyntax)tree.GetRoot();
        }

        private IEnumerable<NamespaceDeclarationSyntax> Namespaces => root.Members.OfType<NamespaceDeclarationSyntax>();
        private IEnumerable<ClassDeclarationSyntax> GetClasses(NamespaceDeclarationSyntax ns) => ns.Members.OfType<ClassDeclarationSyntax>();



        public IEnumerable<(string @namespace, string content)> GetMarkdownDocs() => Namespaces.SelectMany(n =>
                                                                                GetClasses(n)
                                                                                    .Where(DocExtensions.IsPublic)
                                                                                    .Select(c => GetMarkdownDoc(n, c)));

        private (string @namespace, string content) GetMarkdownDoc(NamespaceDeclarationSyntax @namespace, ClassDeclarationSyntax node)
        {

            return (@namespace.Name.ToString(), new ClassDoc(node).ToString());
        }
    }
}