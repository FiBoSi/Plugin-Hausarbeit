using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Composition;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.CodeActions;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Rename;
using Microsoft.CodeAnalysis.Text;

namespace VorlAnalyzer
{
    [ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(VorlAnalyzerCodeFixProvider)), Shared]
    public class VorlAnalyzerCodeFixProvider : CodeFixProvider
    {
        private const string title = "Add Close-Statement";

        public sealed override ImmutableArray<string> FixableDiagnosticIds
        {
            get { return ImmutableArray.Create(VorlAnalyzerAnalyzer.DiagnosticId); }
        }

        public sealed override FixAllProvider GetFixAllProvider()
        {
            // See https://github.com/dotnet/roslyn/blob/master/docs/analyzers/FixAllProvider.md for more information on Fix All Providers
            return WellKnownFixAllProviders.BatchFixer;
        }

        public sealed override async Task RegisterCodeFixesAsync(CodeFixContext context)
        {
            var root = await context.Document.GetSyntaxRootAsync(context.CancellationToken).ConfigureAwait(false);

            // TODO: Replace the following code with your own analysis, generating a CodeAction for each fix to suggest
            var diagnostic = context.Diagnostics.First();
            var diagnosticSpan = diagnostic.Location.SourceSpan;

            // Find the type declaration identified by the diagnostic.
            //var declaration = root.FindToken(diagnosticSpan.Start).Parent.AncestorsAndSelf().OfType<TypeDeclarationSyntax>().First();

            var open = root.FindToken(diagnosticSpan.Start).Parent.AncestorsAndSelf().OfType<InvocationExpressionSyntax>().First();

            var block = findParentBlock(open);

            // Register a code action that will invoke the fix.
            context.RegisterCodeFix(
                CodeAction.Create(
                    title,
                    c => AddClosingStatement(context.Document, open, block, c),
                    equivalenceKey: title),
                diagnostic);
        }

        private BlockSyntax findParentBlock(ExpressionSyntax expression)
        {
            var current = expression.Parent;

            while (current != null && (!(current is BlockSyntax)))
                current = current.Parent;

            return current as BlockSyntax;
        }

        private async Task<Document> AddClosingStatement(Document document, InvocationExpressionSyntax open, BlockSyntax block, CancellationToken c)
        {
            // Root vom Syntaxbaum abholen
            var root = await document.GetSyntaxRootAsync(c);

            // Member Expression auslesen, zb client.open()
            var memberExpression = open.Expression as MemberAccessExpressionSyntax;

            // identifier als String abholen
            var connectionIdentifier = memberExpression.Expression.ToString();

            var closeStatement = SyntaxFactory.ExpressionStatement(
                SyntaxFactory.InvocationExpression(
                    SyntaxFactory.MemberAccessExpression(
                        SyntaxKind.SimpleMemberAccessExpression,
                        SyntaxFactory.IdentifierName(connectionIdentifier),
                        SyntaxFactory.IdentifierName("close"))));

            var newRoot = root.InsertNodesAfter(block.ChildNodes().Last(), new SyntaxNode[] { closeStatement });

            var newDocument = document.WithSyntaxRoot(newRoot);

            return newDocument;
        }
    }
}