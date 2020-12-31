namespace MangaReader.Avalonia.Platform.Win.Interop
{
    /// <summary>
    /// The notify icon version that is used. The higher
    /// the version, the more capabilities are available.
    /// </summary>
    public enum NotifyIconVersion
    {
        /// <summary>
        /// Extended tooltip support, which is available for Vista and later.
        /// Detailed information about what the different versions do, can be found <a href="https://docs.microsoft.com/en-us/windows/win32/api/shellapi/nf-shellapi-shell_notifyicona">here</a>
        /// </summary>
        Vista = 0x4
    }
}
