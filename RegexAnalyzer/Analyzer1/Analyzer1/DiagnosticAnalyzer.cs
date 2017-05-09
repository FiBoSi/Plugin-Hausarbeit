using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Threading;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace Analyzer1
{
    //Test test hallo?
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public class Analyzer1Analyzer : DiagnosticAnalyzer 
    {
        public const string DiagnosticId = "TriviaAnalyzer";
        internal static readonly LocalizableString Title = "TriviaAnalyzer Title";
        internal static readonly LocalizableString MessageFormat = "TriviaAnalyzer '{0}'";
        internal const string Category = "TriviaAnalyzer Category";

        internal static DiagnosticDescriptor Rule = new DiagnosticDescriptor(DiagnosticId, Title, MessageFormat, Category, DiagnosticSeverity.Warning, true);

        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics { get { return ImmutableArray.Create(Rule); } }

        public override void Initialize(AnalysisContext context)
        {
            context.RegisterSyntaxTreeAction(this.HandleSyntaxTree);
        }

        private void HandleSyntaxTree(SyntaxTreeAnalysisContext context)
        {
            SyntaxNode root = context.Tree.GetCompilationUnitRoot(context.CancellationToken);
            var commentNodes = from node in root.DescendantTrivia() where node.IsKind(SyntaxKind.MultiLineCommentTrivia) || node.IsKind(SyntaxKind.SingleLineCommentTrivia) select node;

            if (!commentNodes.Any())
            {
                return;
            }
            foreach (var node in commentNodes)
            {
                string commentText = "";
                switch (node.Kind())
                {
                    case SyntaxKind.SingleLineCommentTrivia:
                        commentText = node.ToString().TrimStart('/');
                        break;
                    case SyntaxKind.MultiLineCommentTrivia:
                        var nodeText = node.ToString();
                        commentText = nodeText.Substring(2, nodeText.Length - 4);
                        break;
                }

                if(commentText.Contains("Todo"))
                {
                    var diagnostic = Diagnostic.Create(Rule, node.GetLocation());
                    context.ReportDiagnostic(diagnostic);
                }

                //if (!string.IsNullOrEmpty(commentText))
                //{

                //    var diagnostic = Diagnostic.Create(Rule, node.GetLocation());
                //    context.ReportDiagnostic(diagnostic);
                //}
            }
        }
    }
}
