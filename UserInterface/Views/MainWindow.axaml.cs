using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using MangaReader.Avalonia.Platform.Win;

namespace UserInterface.Views
{
    public class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();

            WindowsTrayIcon icon = new WindowsTrayIcon();
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }
    }
}
