using System;
using System.Collections.Generic;
using Microsoft.VisualStudio.Text;
using Microsoft.VisualStudio.Text.Classification;

namespace EditorClassiefier
{
    /// <summary>
    /// Classifier that classifies all text as an instance of the "EditorClassifier" classification type.
    /// </summary>
    internal class EditorClassifier : IClassifier
    {
        IClassificationTypeRegistryService _classificationTypeRegistry;

        internal EditorClassifier(IClassificationTypeRegistryService registry)
        {
            _classificationTypeRegistry = registry;
        }

        public event EventHandler<ClassificationChangedEventArgs> ClassificationChanged;

        public IList<ClassificationSpan> GetClassificationSpans(SnapshotSpan span)
        {
            ITextSnapshot snapshot = span.Snapshot;

            List<ClassificationSpan> spans = new List<ClassificationSpan>();

            if(snapshot.Length == 0)
            {
                return spans;
            }

            int startnumber = span.Start.GetContainingLine().LineNumber;
            int endnumber = (span.End - 1).GetContainingLine().LineNumber;

            for(int i = startnumber; i <= endnumber; i++)
            {
                ITextSnapshotLine line = snapshot.GetLineFromLineNumber(i);

                IClassificationType type = null;

                string text = line.Snapshot.GetText(line.Start, line.Length);

                if(text.Contains("@@"))
                {
                    type = _classificationTypeRegistry.GetClassificationType("Goldeneye");
                }

                if (text.Contains("Todo "))
                {
                    type = _classificationTypeRegistry.GetClassificationType("OrangeJuice");
                }

                if (type != null)
                {
                    spans.Add(new ClassificationSpan(line.Extent, type));
                }
            }

            return spans;
        }
    }
}
