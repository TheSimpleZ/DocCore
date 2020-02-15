using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using DocCore.Extensions;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Serilog;
using YamlDotNet.Core;
using YamlDotNet.Serialization;

namespace DocCore.DocProvider
{
    public abstract class Doc
    {

        protected Doc(CSharpSyntaxNode node)
        {
            Node = node;
            Comment = GetComment(node);
            Declaration = node.GetDeclaration();
        }

        public CSharpSyntaxNode Node { get; }
        public CommentDocs Comment { get; }

        public string Declaration { get; }




        private static CommentDocs GetComment(CSharpSyntaxNode node)
        {
            var trivias = node.GetLeadingTrivia();
            var commentTrivia = trivias.Where(t => t.Kind() == SyntaxKind.SingleLineCommentTrivia);

            var commentWithoutLeadingSlashes = commentTrivia.Select(c => Regex.Replace(c.ToString(), @"^//\s?", ""));

            if (TryDeserializeYaml(string.Join("\n", commentWithoutLeadingSlashes), out var comment))
            {
                return comment;
            }
            else
            {
                var fullComment = commentWithoutLeadingSlashes
                                    .DefaultIfEmpty(string.Empty)
                                    .Aggregate((a, b) => AdjustNewlinesAndDots(a, b));

                if (!string.IsNullOrWhiteSpace(fullComment) && !fullComment.EndsWith("."))
                    fullComment = fullComment + ".";

                return new CommentDocs() { Summary = fullComment };
            }

        }

        private static bool TryDeserializeYaml(string yamlString, out CommentDocs comment)
        {
            comment = default;

            if (string.IsNullOrWhiteSpace(yamlString))
                return false;

            try
            {
                comment = new Deserializer().Deserialize<CommentDocs>(yamlString);
                return true;
            }
            catch (YamlException e) when (yamlString.StartsWith("// Summary:"))
            {
                Log.Error(e, "Could not parse yaml");
            }
            catch (YamlException)
            { }

            return false;
        }

        // Add newlines and dots depending on what the first string (a) ends with.
        private static string AdjustNewlinesAndDots(string a, string b)
        {
            return string.IsNullOrWhiteSpace(a)
                    ? a + b
                    : a.EndsWith(".")
                        ? $"{a}\n{b}"
                        : $"{a}.\n{b}";
        }

        protected string ParameterTable
        {
            get
            {

                var builder = new StringBuilder();
                if (Comment.Parameters.Any())
                {
                    builder.AppendLine("### Parameters\n");
                    builder.AppendLine("Name | Description");
                    builder.AppendLine("--- | ---");
                    foreach (var (paramName, description) in Comment.Parameters)
                    {
                        builder.AppendLine($"{paramName} | {description}");
                    }
                }
                return builder.ToString();
            }
        }
    }
}