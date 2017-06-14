using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Threading;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace VorlAnalyzer
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public class VorlAnalyzerAnalyzer : DiagnosticAnalyzer
    {
        public const string DiagnosticId = "VorlAnalyzer";

        // You can change these strings in the Resources.resx file. If you do not want your analyzer to be localize-able, you can use regular strings for Title and MessageFormat.
        // See https://github.com/dotnet/roslyn/blob/master/docs/analyzers/Localizing%20Analyzers.md for more on localization
        private static readonly LocalizableString Title = new LocalizableResourceString(nameof(Resources.AnalyzerTitle), Resources.ResourceManager, typeof(Resources));
        private static readonly LocalizableString MessageFormat = new LocalizableResourceString(nameof(Resources.AnalyzerMessageFormat), Resources.ResourceManager, typeof(Resources));
        private static readonly LocalizableString Description = new LocalizableResourceString(nameof(Resources.AnalyzerDescription), Resources.ResourceManager, typeof(Resources));
        private const string Category = "Naming";

        private static DiagnosticDescriptor Rule = new DiagnosticDescriptor(DiagnosticId, Title, MessageFormat, Category, DiagnosticSeverity.Warning, isEnabledByDefault: true, description: Description);

        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics { get { return ImmutableArray.Create(Rule); } }

        public override void Initialize(AnalysisContext context)
        {
            // TODO: Consider registering other actions that act on syntax instead of or in addition to symbols
            // See https://github.com/dotnet/roslyn/blob/master/docs/analyzers/Analyzer%20Actions%20Semantics.md for more information
            context.RegisterSymbolAction(AnalyzeSymbol, SymbolKind.NamedType);

            // eigene Methode registrieren
            context.RegisterSyntaxNodeAction(AnalyzeOpenNode, SyntaxKind.InvocationExpression);
        }

        // eigene Methode
        private static void AnalyzeOpenNode(SyntaxNodeAnalysisContext context)
        {
            // Syntaktische Analyse
            var invocationExpression = context.Node as InvocationExpressionSyntax;

            var memberExpression = invocationExpression.Expression as MemberAccessExpressionSyntax;

            if (memberExpression?.Name.ToString() != "Open")
                return;

            // Semantische Analyse
            var memberSymbol = context.SemanticModel.GetSymbolInfo(memberExpression).Symbol as IMethodSymbol;

            INamedTypeSymbol typeWithOpen = memberSymbol?.ContainingType;

            if (!(memberSymbol?.ContainingType.AllInterfaces.Any(x => x.Name.Equals("ICommunicationObject")) ?? false))
                return;

            // Schlieﬂen der Verbindung

            var blockExpression = invocationExpression.Parent;

            while (blockExpression != null && (!(blockExpression is BlockSyntax)))
                blockExpression = blockExpression.Parent;

            if (blockExpression != null)
            {
                var allInvocationsAfterOpen = blockExpression.ChildNodes().OfType<ExpressionStatementSyntax>().Where(x => (x.GetLocation().SourceSpan.Start > invocationExpression.GetLocation().SourceSpan.End) && x.Expression is InvocationExpressionSyntax).Select(x => x.Expression as InvocationExpressionSyntax);

                var allCloseExpressionAfterOpen = allInvocationsAfterOpen.Where(x => (x.Expression as MemberAccessExpressionSyntax)?.Name.ToString() == "Close");

                bool foundMatchingClose = false;

                foreach(var closeExpression in allCloseExpressionAfterOpen)
                {
                    var closeMemberExpression = closeExpression.Expression as MemberAccessExpressionSyntax;

                    if(closeMemberExpression != null)
                    {
                        var closeMemberSymbol = context.SemanticModel.GetSymbolInfo(closeMemberExpression).Symbol as IMethodSymbol;

                        if(closeMemberSymbol != null)
                        {
                            if(closeMemberSymbol.ContainingType == typeWithOpen && closeMemberExpression.ToString() == connectionIdentifier)
                            {
                                foundMatchingClose = true;
                                break;
                            }
                        }
                    }
                }

                if(!foundMatchingClose)
                {
                    var diagnostic = Diagnostic.Create(Rule, invocationExpression.GetLocation(), invocationExpression.ToString());

                    context.ReportDiagnostic(diagnostic);
                }

            }
        }

        private static void AnalyzeSymbol(SymbolAnalysisContext context)
        {
            // TODO: Replace the following code with your own analysis, generating Diagnostic objects for any issues you find
            var namedTypeSymbol = (INamedTypeSymbol)context.Symbol;

            // Find just those named type symbols with names containing lowercase letters.
            if (namedTypeSymbol.Name.ToCharArray().Any(char.IsLower))
            {
                // For all such symbols, produce a diagnostic.
                var diagnostic = Diagnostic.Create(Rule, namedTypeSymbol.Locations[0], namedTypeSymbol.Name);

                context.ReportDiagnostic(diagnostic);
            }
        }
    }
}
