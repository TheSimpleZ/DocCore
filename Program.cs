using System;
using System.IO;
using System.Linq;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Text;
using DocCore.DocProvider;
using Buildalyzer;
using System.Threading.Tasks;

namespace DocCore
{
    class Program
    {
        static void Main(string[] args)
        {
            var projects = new AnalyzerManager(args[0]).Projects;

            foreach (var project in projects)
            {
                var projectName = Path.GetFileNameWithoutExtension(project.Key);
                var analyzer = project.Value;


                var result = analyzer.Build().Single();

                var files = result.SourceFiles;

                Parallel.ForEach(files, file =>
                {
                    using var stream = File.OpenRead(file);

                    var tree = CSharpSyntaxTree.ParseText(SourceText.From(stream), path: file);
                    var root = (CompilationUnitSyntax)tree.GetRoot();
                    var docProvider = new CSharpDocProvider(tree);


                    var classNodes = docProvider.Namespaces.SelectMany(docProvider.GetClasses);

                    Parallel.ForEach(classNodes, classNode =>
                    {
                        string dir = Path.Combine(Directory.GetCurrentDirectory(), "docs", projectName, docProvider.GetNamespace(classNode).Name.ToString());
                        if (!Directory.Exists(dir))  // if it doesn't exist, create
                            Directory.CreateDirectory(dir);

                        using var writer = File.CreateText(Path.Combine(dir, Path.GetFileNameWithoutExtension(file) + ".md"));

                        writer.Write(docProvider.GetMarkdownDocs(classNode));
                    });

                });

            }




        }
    }
}
