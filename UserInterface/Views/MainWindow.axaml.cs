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
#if DEBUG
            this.AttachDevTools();
#endif
            WindowsTrayIcon icon = new WindowsTrayIcon();
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }
    }
}
