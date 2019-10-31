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

        public IEnumerable<NamespaceDeclarationSyntax> Namespaces => root.Members.OfType<NamespaceDeclarationSyntax>();
        public IEnumerable<ClassDeclarationSyntax> GetClasses(NamespaceDeclarationSyntax ns) => ns.Members.OfType<ClassDeclarationSyntax>();

        public static CommentDocs GetComment(CSharpSyntaxNode node)
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
                catch (YamlException)
                { }
            }

            return comment;
        }


        public string GetMarkdownDocs(ClassDeclarationSyntax node)
        {
            var _namespace = GetNamespace(node);
            return (
$@"{node.Identifier}
======
##### Namespace: {_namespace.Name}

{GetComment(node).Summary}


{node.GetDeclaration()}


{GetClassConstructorList(node)}

{GetClassPropertyList(node)}

{GetClassMethodList(node)}


");
        }

        public NamespaceDeclarationSyntax GetNamespace(ClassDeclarationSyntax node) => node.Ancestors().OfType<NamespaceDeclarationSyntax>().Single();

        private string GetClassConstructorList(ClassDeclarationSyntax node)
        {
            var constructors = node.Members.OfType<ConstructorDeclarationSyntax>();
            if (!constructors.Any())
                return null;


            var builder = new StringBuilder("# Constructors\n");

            foreach (var ctor in constructors)
            {
                var paramTypes = string.Join(", ", ctor.GetParameterTypes());
                var commentDocs = GetComment(ctor);
                builder.AppendLine($"{ctor.Identifier}({paramTypes})");
                builder.AppendLine("------");
                builder.AppendLine(commentDocs.Summary);
                builder.AppendLine(ctor.GetDeclaration());
                builder.AppendLine(GetParameterTable(ctor));
                if (!string.IsNullOrEmpty(commentDocs.Returns))
                    builder.AppendLine($"**Returns:** {commentDocs.Returns}");
                builder.AppendLine();

            }

            return builder.ToString();
        }

        private string GetClassPropertyList(ClassDeclarationSyntax node)
        {
            var properties = node.Members.OfType<PropertyDeclarationSyntax>();
            if (!properties.Any())
                return null;


            var builder = new StringBuilder("# Properties\n");

            foreach (var ctor in properties)
            {
                var commentDocs = GetComment(ctor);
                builder.AppendLine($"{ctor.Identifier}");
                builder.AppendLine("------");
                builder.AppendLine(commentDocs.Summary);
                builder.AppendLine(ctor.GetDeclaration());
                builder.AppendLine();

            }

            return builder.ToString();
        }

        private string GetClassMethodList(ClassDeclarationSyntax node)
        {
            var methods = node.Members.OfType<MethodDeclarationSyntax>();
            if (!methods.Any())
                return null;


            var builder = new StringBuilder("# Methods\n");

            foreach (var method in methods)
            {
                var paramTypes = string.Join(", ", method.GetParameterTypes());
                var commentDocs = GetComment(method);
                builder.AppendLine($"{method.Identifier}({paramTypes})");
                builder.AppendLine("------");
                builder.AppendLine(commentDocs.Summary);
                builder.AppendLine(method.GetDeclaration());
                builder.AppendLine(GetParameterTable(method));
                if (!string.IsNullOrEmpty(commentDocs.Returns))
                    builder.AppendLine($"**Returns:** {commentDocs.Returns}");
                builder.AppendLine();

            }

            return builder.ToString();
        }

        private static string GetParameterTable(BaseMethodDeclarationSyntax syntax)
        {
            if (!syntax.ParameterList.Parameters.Any())
                return "";

            var builder = new StringBuilder("### Parameters\n");
            builder.AppendLine("Name | Description");
            builder.AppendLine("--- | ---");
            foreach (var param in syntax.ParameterList.Parameters)
            {
                builder.AppendLine($"{param.Identifier} | {GetComment(syntax)?.Parameters?[param.Identifier.ToString()]}");
            }
            return builder.ToString();
        }
    }
}