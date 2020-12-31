using System.Runtime.InteropServices;
using System.Windows.Input;
using MangaReader.Avalonia.Platform.Win.Interop;

namespace MangaReader.Avalonia.Platform.Win
{
  public class WindowsTrayIcon : ITrayIcon
  {
    public ICommand DoubleClickCommand { get; set; }

    public ICommand BalloonClickedCommand { get; set; }

    private TaskBarIcon taskBarIcon;

    private object lastBalloonState;

    public void SetIcon()
    {
      if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
      {
        var iconSource = "MangaReader.Avalonia.Assets.main.ico";
        var icon = TaskBarIcon.ToIcon(iconSource);
        taskBarIcon = new TaskBarIcon(icon);
        taskBarIcon.MouseEventHandler += TaskBarIconOnMouseEventHandler;
      }
    }

    public void ShowBalloon(string text, object state)
    {
      this.lastBalloonState = state;
      taskBarIcon?.ShowBalloonTip(nameof(MangaReader), text, BalloonFlags.Info);
    }

    private void TaskBarIconOnMouseEventHandler(object sender, MouseEvent e)
    {
      if (e == MouseEvent.IconDoubleClick)
      {
        var command = this.DoubleClickCommand;
        if (command != null && command.CanExecute(null))
        {
          command.Execute(null);
        }
      }

      if (e == MouseEvent.BalloonToolTipClicked)
      {
        var command = this.BalloonClickedCommand;
        if (command != null && command.CanExecute(lastBalloonState))
        {
          command.Execute(lastBalloonState);
        }
      }
    }

    public void Dispose()
    {
      if (taskBarIcon != null)
        taskBarIcon.MouseEventHandler -= TaskBarIconOnMouseEventHandler;
      taskBarIcon?.Dispose();
    }
  }
}
