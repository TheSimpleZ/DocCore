using System;
using System.IO;
using System.Linq;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Text;
using DocCore.DocProvider;

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
            var docProvider = new CSharpDocProvider(tree);

            var classNode = docProvider.Namespaces.SelectMany(docProvider.GetClasses).First();

            Console.WriteLine(docProvider.GetMarkdownDocs(classNode));

            using var writer = File.CreateText($"{Directory.GetCurrentDirectory()}/doc.md");

            writer.Write(docProvider.GetMarkdownDocs(classNode));

        }
    }
}
