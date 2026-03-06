using System.Windows;
using System.Windows.Controls;
using WpfControlsOefenblad.Helpers;

namespace WpfControlsOefenblad.Exercises;

/// <summary>
/// Interaction logic for KlikMij.xaml
/// </summary>
[NavPage(Title="Klik mij", Description="Eenvoudige Button met Click-event", Order=1, IsVisible=true)]
public partial class KlikMij : Page
{
    public KlikMij()
    {
        InitializeComponent();
    }
}
