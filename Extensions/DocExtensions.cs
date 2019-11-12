using System.Collections.Generic;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace DocCore.Extensions
{
    public static class DocExtensions
    {
        public static string GetDeclaration(this SyntaxNode node) => string.Join("\n",
        string.Concat(node.ToString().TakeWhile(c => c != '{'))
        .Split('\n')
        .Select(l => l.Trim())).Trim();


        public static bool IsPublic(dynamic classNode)
        {
            SyntaxTokenList modifiers = classNode.Modifiers;
            return modifiers.Any(modifier => modifier.Kind() == SyntaxKind.PublicKeyword || modifier.Kind() == SyntaxKind.ProtectedKeyword);
        }

    }
}