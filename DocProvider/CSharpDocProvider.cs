using DocCore.Extensions;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System.Collections.Generic;
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

        public IEnumerable<NamespaceDeclarationSyntax> Namespaces => root.Members.OfType<NamespaceDeclarationSyntax>();
        public IEnumerable<ClassDeclarationSyntax> GetClasses(NamespaceDeclarationSyntax ns) => ns.Members.OfType<ClassDeclarationSyntax>();

        public CommentDocs GetComment(CSharpSyntaxNode node)
        {
            var trivias = node.GetLeadingTrivia();
            var commentTrivia = trivias.Where(t => t.Kind() == SyntaxKind.SingleLineCommentTrivia);

            var formatedComment = commentTrivia
            .Select(c => Regex.Replace(c.ToString(), @"^//\s?", "")) // Remove leading slashes
            .Select(c => c + " ") // Add space to end of all lines
            .Select(c => Regex.Replace(c, @"^\s$", "\n")); // Replace the lines with only a space with a newline

            var fullComment = string.Join("", formatedComment);

            var comment = new CommentDocs() { Summary = fullComment };
            if (!string.IsNullOrEmpty(fullComment))
            {
                try
                {
                    comment = new Deserializer().Deserialize<CommentDocs>(fullComment);
                }
                catch (YamlException)
                { }
            }

            return comment;
        }


        public string GetMarkdownDocs(ClassDeclarationSyntax node)
        {
            var _namespace = node.Ancestors().OfType<NamespaceDeclarationSyntax>().Single();
            return (
$@"{node.Identifier}
======
##### Namespace: {_namespace.Name}

{GetComment(node).Summary}


{node.GetDeclaration()}


{GetClassConstructorList(node)}


");
        }

        private string GetClassConstructorList(ClassDeclarationSyntax node)
        {
            var constructors = node.Members.OfType<ConstructorDeclarationSyntax>();
            if (!constructors.Any())
                return null;


            var builder = new StringBuilder("# Constructors\n");

            foreach (var ctor in constructors)
            {
                var paramTypes = string.Join(", ", ctor.GetParameterTypes());

                builder.AppendLine($"{ctor.Identifier}({paramTypes})");
                builder.AppendLine("------");
                builder.AppendLine(GetComment(ctor).Summary);
                builder.AppendLine(ctor.GetDeclaration());
                builder.AppendLine(GetParameterTable(ctor));

            }

            return builder.ToString();
        }

        private static string GetParameterTable(BaseMethodDeclarationSyntax syntax)
        {
            if (!syntax.ParameterList.Parameters.Any())
                return "";

            var builder = new StringBuilder("### Parameters");
            builder.AppendLine("Name | Description");
            builder.AppendLine("--- | ---");
            foreach (var param in syntax.ParameterList.Parameters)
            {
                builder.AppendLine($"{param.Identifier} | Description");
            }
            return builder.ToString();
        }
    }
}