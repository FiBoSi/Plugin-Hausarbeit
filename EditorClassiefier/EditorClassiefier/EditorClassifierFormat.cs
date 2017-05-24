using System.ComponentModel.Composition;
using System.Windows.Media;
using Microsoft.VisualStudio.Text.Classification;
using Microsoft.VisualStudio.Utilities;

namespace EditorClassiefier
{
    internal sealed class EditorClassifierFormat : ClassificationFormatDefinition
    {

    }

    [Export(typeof(EditorFormatDefinition))]
    [Name("OrangeJuice")]
    [Order(Before = "Goldeneye")]
    internal sealed class MyFirstDefinition : ClassificationFormatDefinition
    {
        public MyFirstDefinition()
        {
            ForegroundColor = Colors.Orange;
        }
    }

    [Export(typeof(EditorFormatDefinition))]
    [Name("Goldeneye")]
    internal sealed class MySecondDefinition : ClassificationFormatDefinition
    {
        public MySecondDefinition()
        {
            ForegroundColor = Colors.DarkGoldenrod;
        }
    }
}
