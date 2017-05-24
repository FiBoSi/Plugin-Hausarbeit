
using System;
using System.ComponentModel.Composition;
using Microsoft.VisualStudio.Text;
using Microsoft.VisualStudio.Text.Classification;
using Microsoft.VisualStudio.Utilities;

namespace EditorClassiefier
{
    [Export(typeof(IClassifierProvider))]
    [ContentType("text")]
    internal class EditorClassifierProvider : IClassifierProvider
    {
        [Import]
        private IClassificationTypeRegistryService classificationRegistry;

        public IClassifier GetClassifier(ITextBuffer textBuffer)
        {
            return textBuffer.Properties.GetOrCreateSingletonProperty<EditorClassifier>(creator: () => new EditorClassifier(this.classificationRegistry));
        }
    }
}
