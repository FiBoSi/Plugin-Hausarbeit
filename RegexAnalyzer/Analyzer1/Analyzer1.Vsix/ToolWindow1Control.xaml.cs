//------------------------------------------------------------------------------
// <copyright file="ToolWindow1Control.xaml.cs" company="Company">
//     Copyright (c) Company.  All rights reserved.
// </copyright>
//------------------------------------------------------------------------------

namespace Analyzer1.Vsix
{
    using System.Diagnostics.CodeAnalysis;
    using System.Windows;
    using System.Windows.Controls;
    using Analyzer1;
    using System.Collections.Generic;
    using ClassLibrary1;
    using Microsoft.CodeAnalysis.Text;


    /// <summary>
    /// Interaction logic for ToolWindow1Control.
    /// </summary>
    public partial class ToolWindow1Control : UserControl
    {

        private List<TodoItem> items = new List<TodoItem>();

        /// <summary>
        /// Initializes a new instance of the <see cref="ToolWindow1Control"/> class.
        /// </summary>
        public ToolWindow1Control()
        {
            this.InitializeComponent();
        }

        /// <summary>
        /// Handles click on the button by displaying a message box.
        /// </summary>
        /// <param name="sender">The event sender.</param>
        /// <param name="e">The event args.</param>
        [SuppressMessage("Microsoft.Globalization", "CA1300:SpecifyMessageBoxOptions", Justification = "Sample code")]
        [SuppressMessage("StyleCop.CSharp.NamingRules", "SA1300:ElementMustBeginWithUpperCaseLetter", Justification = "Default event handler naming pattern")]
        private void button1_Click(object sender, RoutedEventArgs e)
        {
            //MessageBox.Show(
            //    string.Format(System.Globalization.CultureInfo.CurrentUICulture, "Invoked '{0}'", this.ToString()),
            //    "ToolWindow1");


            //FillTodoList();

            // Listbox mit gefundenen Todos füllen
            listBox.ItemsSource = Class1.foundTodos;

            //Analyzer1Analyzer.DoShit(items[0]);
        }

        private void button2_Click(object sender, RoutedEventArgs e)
        {
            items[1].todoLocation.SourceTree.WithChangedText(items[1].todoLocation.SourceTree.GetText().WithChanges(new Microsoft.CodeAnalysis.Text.TextChange(items[1].todoLocation.SourceSpan, items[1].text + " FOUND!")));

        }

        // gefundene Todos in klasseneigener liste speichern
        public void FillTodoList()
        {
            items = Class1.foundTodos;
        }
    }
}