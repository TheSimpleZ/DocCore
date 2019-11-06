using System.IO;
using System.Linq;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.Text;
using DocCore.DocProvider;
using Buildalyzer;
using System.Threading.Tasks;
using Serilog;
using System.Collections.Generic;
using McMaster.Extensions.CommandLineUtils;
using DocCore.Extensions;

namespace DocCore
{
    class Program
    {

        public static int Main(string[] args)
        => CommandLineApplication.Execute<Program>(args);

        [Option(Description = "The sln file. Defaults to current directory")]
        public string Sln { get; }

        [Option(Description = "The output folder. Defaults to ./docs")]
        public string Output { get; } = Path.Combine(Directory.GetCurrentDirectory(), "docs");

        [Option(Description = "List of glob patterns to exclude projects")]
        public string[] Exclude { get; } = new string[] { };



        private void OnExecute()
        {

            Log.Logger = new LoggerConfiguration().WriteTo.ColoredConsole().CreateLogger();

            var slnPath = Sln ?? Directory.GetFiles(Directory.GetCurrentDirectory()).First(f => f.EndsWith(".sln"));
            Log.Information("Loading projects in {SolutionPath}", slnPath);
            var sln = new AnalyzerManager(slnPath).Projects.Where(proj => !Exclude.Any(e => proj.Key.Like(e)));

            var buildResults = BuildProjects(sln);

            Log.Information("{ProjectCount} projects loaded", buildResults.Count());


            Parallel.ForEach(buildResults, buildResult =>
            {
                var projectName = Path.GetFileNameWithoutExtension(buildResult.projectPath);

                Log.Information("Getting files in {ProjectName}", projectName);
                var files = buildResult.project.SourceFiles;

                Parallel.ForEach(files, file =>
                {
                    var filename = Path.GetFileNameWithoutExtension(file);

                    Log.Information("Generating docs for {FileName}", filename);
                    var docs = GetDocProvider(file).GetMarkdownDocs();

                    Parallel.ForEach(docs, doc =>
                    {
                        string dir = projectName != doc.@namespace ? Path.Combine(Output, projectName, doc.@namespace.Replace($"{projectName}.", "")) : Path.Combine(Output, projectName);
                        if (!Directory.Exists(dir))
                            Directory.CreateDirectory(dir);

                        using var writer = File.CreateText(Path.Combine(dir, filename + ".md"));

                        writer.Write(doc.content);
                    });
                });
            });
        }

        private static List<(string projectPath, AnalyzerResult project)> BuildProjects(IEnumerable<KeyValuePair<string, ProjectAnalyzer>> sln)
        {
            var buildResults = new List<(string projectPath, AnalyzerResult project)>();
            foreach (var (projectPath, projectAnalyzer) in sln)
            {
                Log.Information("Building {ProjectName}", Path.GetFileNameWithoutExtension(projectPath));
                buildResults.Add((projectPath, projectAnalyzer.Build().Single()));
            }

            return buildResults;
        }

        private static CSharpDocProvider GetDocProvider(string file)
        {
            using var stream = File.OpenRead(file);
            var tree = CSharpSyntaxTree.ParseText(SourceText.From(stream), path: file);
            return new CSharpDocProvider(tree);
        }
    }
}
