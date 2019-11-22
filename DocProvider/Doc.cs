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

            var fullComment = string.Join("\n", commentWithoutLeadingSlashes);

            var comment = new CommentDocs() { Summary = fullComment };
            if (!string.IsNullOrEmpty(fullComment))
            {
                try
                {
                    comment = new Deserializer().Deserialize<CommentDocs>(fullComment);
                }
                catch (YamlException e) when (fullComment.StartsWith("// Summary:"))
                {
                    Log.Error(e, "Could not parse yaml");
                }
                catch (YamlException)
                { }
            }

            return comment;
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