using System.ComponentModel.Composition;
using Microsoft.VisualStudio.Text.Classification;
using Microsoft.VisualStudio.Utilities;

namespace EditorClassiefier
{
    internal static class EditorClassifierClassificationDefinition
    {
        [Export(typeof(ClassificationTypeDefinition))]
        [Name("OrangeJuice")]
        internal static ClassificationTypeDefinition myFirstDefinition = null;

        [Export(typeof(ClassificationTypeDefinition))]
        [Name("Goldeneye")]
        internal static ClassificationTypeDefinition mySecondDefinition = null;

    }
}
