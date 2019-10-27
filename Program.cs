using System;
using System.IO;
using System.Linq;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Text;

namespace DocCore
{
    class Program
    {
        static void Main(string[] args)
        {
            var path = @"ResourceExtensions.cs";
            using var stream = File.OpenRead(path);

            var tree = CSharpSyntaxTree.ParseText(SourceText.From(stream), path: path);
            var root = (CompilationUnitSyntax)tree.GetRoot();
            var namespaceNode = (NamespaceDeclarationSyntax)(root.Members.First());

            var classNode = (ClassDeclarationSyntax)(namespaceNode.Members.First());

            var trivias = classNode.GetLeadingTrivia();
            var xmlCommentTrivia = trivias.FirstOrDefault(t => t.Kind() == SyntaxKind.SingleLineCommentTrivia);
            var xml = xmlCommentTrivia.GetStructure();
            Console.WriteLine(xml);
        }
    }
}
