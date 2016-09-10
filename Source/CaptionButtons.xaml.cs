using System;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Interop;

namespace FramelessWPF
{
    public partial class CaptionButtons
    {
        private Window parent;

        public CaptionButtons()
        {
            InitializeComponent();
            Loaded += CaptionButtonsLoaded;
        }

        void CaptionButtonsLoaded(object sender, RoutedEventArgs e)
        {
            parent = GetTopParent();
        }

        private void CloseButtonClick(object sender, RoutedEventArgs e)
        {
            parent.Close();
        }

        private void RestoreButtonClick(object sender, RoutedEventArgs e)
        {
            parent.WindowState = parent.WindowState == WindowState.Maximized ? WindowState.Normal : WindowState.Maximized;
        }

        private void MinimizeButtonClick(object sender, RoutedEventArgs e)
        {
            parent.WindowState = WindowState.Minimized;
        }

        private Window GetTopParent()
        {
            return Window.GetWindow(this);
        }

        public Thickness MarginButton
        {
            get { return (Thickness)GetValue(MarginButtonProperty); }
            set { SetValue(MarginButtonProperty, value); }
        }

        public static readonly DependencyProperty MarginButtonProperty =
            DependencyProperty.Register("MarginButton", typeof(Thickness), typeof(MainWindow));

        public enum CaptionType
        {
            Full,
            ReduceClose
        }

        public CaptionType Type
        {
            get { return (CaptionType)GetValue(TypeProperty); }
            set { SetValue(TypeProperty, value); }
        }

        public static readonly DependencyProperty TypeProperty =
            DependencyProperty.Register("Type", typeof(CaptionType),
            typeof(MainWindow), new PropertyMetadata(CaptionType.Full));
    }

    internal static class NativeMethods
    {
        [DllImport("user32.dll")]
        internal static extern IntPtr DefWindowProc(IntPtr handle, int message, IntPtr wParam, IntPtr lParam);
    }

    public partial class MainWindow
    {
        protected override void OnSourceInitialized(EventArgs e)
        {
            base.OnSourceInitialized(e);
            var helper = new WindowInteropHelper(this);
            var source = HwndSource.FromHwnd(helper.Handle);
            if (source != null) source.AddHook(Hook);
        }

        private static IntPtr Hook(IntPtr handle, int message, IntPtr wParam, IntPtr lParam, ref bool handled)
        {
            if (message != 0x0086) return IntPtr.Zero;
            handled = true;
            return NativeMethods.DefWindowProc(handle, message, new IntPtr(1), lParam);
        }
    }
}
