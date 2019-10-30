using System.Collections.Generic;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace DocCore.Extensions
{
    public static class DocExtensions
    {
        public static string GetDeclaration(this SyntaxNode node) => "```\n" + node.ToString().Split('\n').FirstOrDefault() + "\n```";

        public static IEnumerable<string> GetParameterTypes(this BaseMethodDeclarationSyntax node) => node.ParameterList.Parameters.Select(p => p.Type.ToString());

    }
}