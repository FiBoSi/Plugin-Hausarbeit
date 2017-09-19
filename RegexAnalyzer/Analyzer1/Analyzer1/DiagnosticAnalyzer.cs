using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Threading;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;
using ClassLibrary1;
using System.Windows.Documents;


namespace Analyzer1
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public class Analyzer1Analyzer : DiagnosticAnalyzer 
    {
        // Sammlung von zu erkennenden todo-Schreibweisen
        private string[] todoList = { "todo", "Todo", "TODO" };

        public const string DiagnosticId = "TriviaAnalyzer";
        internal static readonly LocalizableString Title = "TriviaAnalyzer Title";
        internal static readonly LocalizableString MessageFormat = "TriviaAnalyzer";
        internal const string Category = "TriviaAnalyzer Category";

        internal static DiagnosticDescriptor Rule = new DiagnosticDescriptor(DiagnosticId, Title, MessageFormat, Category, DiagnosticSeverity.Info, true);

        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics { get { return ImmutableArray.Create(Rule); } }

        // Zugriff auf Syntax-Baum
        public override void Initialize(AnalysisContext context)
        {
            context.RegisterSyntaxTreeAction(this.HandleSyntaxTree);
        }
        
        private void HandleSyntaxTree(SyntaxTreeAnalysisContext context)
        {
            //contextStored = context;

            SyntaxNode root = context.Tree.GetCompilationUnitRoot(context.CancellationToken);

            // Sammeln aller Single- und MultiLineCommentTrivia
            var commentNodes = from node in root.DescendantTrivia() where node.IsKind(SyntaxKind.MultiLineCommentTrivia) || node.IsKind(SyntaxKind.SingleLineCommentTrivia) select node;

            if (!commentNodes.Any())
            {
                return;
            }

            int counter = 0;
            foreach (var node in commentNodes)
            {
                string commentText = "";
                // Ablegen des node-Inhalts in commentText als String
                switch (node.Kind())
                {
                    case SyntaxKind.SingleLineCommentTrivia:
                        commentText = node.ToString().TrimStart('/');
                        break;
                    case SyntaxKind.MultiLineCommentTrivia:
                        var nodeText = node.ToString().TrimStart('/');
                        commentText = nodeText.Substring(0, nodeText.Length - 4);
                        break;
                }
                
                // Prüfen ob commentText eine bekannte todo-Schreibweise enthält
                if(MyContains.MyOwnContains(commentText, todoList))
                {
                    // erzeugen einer Diagnostic an der Location des nodes
                    var diagnostic = Diagnostic.Create(Rule, node.GetLocation());
                    context.ReportDiagnostic(diagnostic);
                    
                    // speichert gefundene Todos in TodoItem-Liste von Class1
                    // dort können sie vom ToolWindow gefunden werden
                    //Class1.foundTodos.Add(new TodoItem() { text = commentText , todoItemID = counter , todoLocation = node.GetLocation() });
                    //counter++;
                }
            }
        }
    }
}


