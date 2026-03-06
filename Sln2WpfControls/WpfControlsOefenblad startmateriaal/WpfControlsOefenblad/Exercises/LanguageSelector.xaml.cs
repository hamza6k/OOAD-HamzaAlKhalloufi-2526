using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using WpfControlsOefenblad.Helpers;

namespace WpfControlsOefenblad.Exercises;

[NavPage(Title = "Language Selector", Description = "ComboBox SelectionChanged event en ComboBoxItem", Order = 6, IsVisible = true)]
public partial class LanguageSelector : Page
{
    public LanguageSelector()
    {
        InitializeComponent();
    }
}
