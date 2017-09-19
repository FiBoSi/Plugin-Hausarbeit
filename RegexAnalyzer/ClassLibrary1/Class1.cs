using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;
using Microsoft.CodeAnalysis.CSharp;
using System.Composition;
using System.Collections.Immutable;

namespace ClassLibrary1
{

    public interface IDiasgnosticAnalyzerContract
    {
        //
        // Zusammenfassung:
        //     Returns a set of descriptors for the diagnostics that this analyzer is capable
        //     of producing.
        ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics { get; }

        bool Equals(object obj);
        int GetHashCode();
        //
        // Zusammenfassung:
        //     Called once at session start to register actions in the analysis context.
        //
        // Parameter:
        //   context:
        void Initialize(AnalysisContext context);
        string ToString();
    }

    [Export(typeof(IDiasgnosticAnalyzerContract))]
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    class TodoAnalyzer : IDiasgnosticAnalyzerContract
    {
        // Sammlung von zu erkennenden todo-Schreibweisen
        private string[] todoList = { "todo", "Todo", "TODO" };

        public const string DiagnosticId = "TriviaAnalyzer";
        internal static readonly LocalizableString Title = "TriviaAnalyzer Title";
        internal static readonly LocalizableString MessageFormat = "TriviaAnalyzer";
        internal const string Category = "TriviaAnalyzer Category";

        internal static DiagnosticDescriptor Rule = new DiagnosticDescriptor(DiagnosticId, Title, MessageFormat, Category, DiagnosticSeverity.Info, true);

        public ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics { get { return ImmutableArray.Create(Rule); } }

        // Zugriff auf Syntax-Baum
        public void Initialize(AnalysisContext context)
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
                if (MyOwnContains(commentText, todoList))
                {
                    // erzeugen einer Diagnostic an der Location des nodes
                    var diagnostic = Diagnostic.Create(Rule, node.GetLocation());
                    context.ReportDiagnostic(diagnostic);

                    // speichert gefundene Todos in TodoItem-Liste von Class1
                    // dort können sie vom ToolWindow gefunden werden
                    Class1.foundTodos.Add(new TodoItem() { text = commentText, todoItemID = counter, todoLocation = node.GetLocation() });
                    counter++;
                }
            }
        }

        // prüft ob im übergebenen string str strings s aus dem string[] p zu finden sind
        private bool MyOwnContains(string str, params string[] p)
        {
            return p.Any(s => str.Contains(s)); // sollte prüfen ob die strings damit beginnen
        }
    }

    public interface ICodeFixProviderContract
    {

    }

    [Export(typeof(ICodeFixProviderContract))]
    class TodoCodeFixProvider : ICodeFixProviderContract
    {

    }

    public static class Class1
    {
        public static List<TodoItem> foundTodos = new List<TodoItem>();
    }

    // als interface bereitstellen
    public interface ITodoItem
    {
        string text { get; set; }
        int todoItemID { get; set; }
        Location todoLocation { get; set; }
    }

    [Export(typeof(ITodoItem))]
    public class TodoItem : ITodoItem
    {
        public string text
        {
            get
            {
                throw new NotImplementedException();
            }

            set
            {
                text = value;
            }
        }

        public int todoItemID
        {
            get
            {
                throw new NotImplementedException();
            }

            set
            {
                todoItemID = value;
            }
        }

        public Location todoLocation
        {
            get
            {
                throw new NotImplementedException();
            }

            set
            {
                todoLocation = value;
            }
        }
    }
}
