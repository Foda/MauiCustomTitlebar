#if WINDOWS
using Microsoft.Maui.Platform;
using Microsoft.UI.Input;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Media;
using Application = Microsoft.Maui.Controls.Application;

namespace TitlebarSample
{
    public partial class App : Application
    {
        private List<FrameworkElement> _titlebarInteractive = new();
        private InputNonClientPointerSource _appWindowPointerSrc;
        private TitlebarViewModel _titlebarViewModel;

        public static MauiWinUIWindow MainWindow { get; private set; }

        public App(TitlebarViewModel titlebarViewModel)
        {
            _titlebarViewModel = titlebarViewModel;

            MainPage = new AppShell();
            MainPage.Loaded += MainPage_Loaded;
        }

        void MainPage_Loaded(object? sender, EventArgs _)
        {
            MainPage.Loaded -= MainPage_Loaded;
            SetTitlebarInteractive();
        }

        private void SetTitlebarInteractive()
        {
            MainWindow = Current.Windows[0].Handler.PlatformView as MauiWinUIWindow;
            if (MainWindow != null)
            {
                var mauiContext = Current.Windows[0].Handler.MauiContext;
                var customTitlebar = new TitlebarContent(_titlebarViewModel);
                var platformView = customTitlebar.ToPlatform(mauiContext);

                _titlebarInteractive = GetPassthroughElements(customTitlebar, mauiContext);
                _appWindowPointerSrc =
                    InputNonClientPointerSource.GetForWindowId(MainWindow.AppWindow.Id);

                // Wait for the titlebar to be loaded before setting the drag regions
                platformView.Loaded += CustomTitlebar_Loaded;
                void CustomTitlebar_Loaded(object? sender, RoutedEventArgs e)
                {
                    platformView.Loaded -= CustomTitlebar_Loaded;

                    // Workaround: this will force the content to resize and react to the titlebar height change
                    Current.Windows[0].Width += 1;
                }

                var contentControl = FindChildElementByName(MainWindow.Content, "CustomTitlebarContent") as Microsoft.UI.Xaml.Controls.ContentControl;
                contentControl.Content = platformView;
                contentControl.SizeChanged += (s, e) => UpdateDragRegions();
            }
        }

        /// <summary>
        /// Update the drag regions to include any interactive elements
        /// </summary>
        private void UpdateDragRegions()
        {
            var rectArray = new Windows.Graphics.RectInt32[_titlebarInteractive.Count];
            for (int i = 0; i < _titlebarInteractive.Count; i++)
            {
                var control = _titlebarInteractive[i];
                double scaleAdjustment = control.XamlRoot.RasterizationScale;

                var transform = control.TransformToVisual(null);
                var bounds = transform.TransformBounds(
                    new Windows.Foundation.Rect(0, 0, control.ActualWidth, control.ActualHeight));

                var controlRect = GetRect(bounds, scaleAdjustment);
                rectArray[i] = controlRect;
            }

            _appWindowPointerSrc.SetRegionRects(NonClientRegionKind.Passthrough, rectArray);
        }

        #region Utilities

        /// <summary>
        /// Get all the passthrough elements in the visual tree based on the InputTransparent property
        /// </summary>
        /// <param name="layoutParent"></param>
        /// <param name="mauiContext"></param>
        /// <returns></returns>
        private static List<FrameworkElement> GetPassthroughElements(IVisualTreeElement layoutParent, IMauiContext mauiContext)
        {
            var passthroughElements = new List<FrameworkElement>();
            foreach (var p in layoutParent.GetVisualTreeDescendants())
            {
                if (p is IContentView)
                    continue;

                var asView = p as IView;
                if (mauiContext != null &&
                    asView != null &&
                    !asView.InputTransparent)
                {
                    var childView = asView.ToHandler(mauiContext).PlatformView;
                    if (childView != null)
                    {
                        passthroughElements.Add(childView);
                    }
                }
            }

            return passthroughElements;
        }

        /// <summary>
        /// Find a child element by name in the visual tree
        /// </summary>
        /// <param name="root"></param>
        /// <param name="name"></param>
        /// <returns></returns>
        private static DependencyObject? FindChildElementByName(DependencyObject root, string name)
        {
            int children = VisualTreeHelper.GetChildrenCount(root);
            for (int i = 0; i < children; i++)
            {
                DependencyObject childObject = VisualTreeHelper.GetChild(root, i);
                var fe = childObject as FrameworkElement;
                if (fe != null && fe.Name == name)
                {
                    return childObject;
                }
                else
                {
                    DependencyObject childInSubtree = FindChildElementByName(childObject, name);
                    if (childInSubtree != null)
                    {
                        return childInSubtree;
                    }
                }
            }
            return null;
        }

        private static Windows.Graphics.RectInt32 GetRect(Windows.Foundation.Rect bounds, double scale)
        {
            return new Windows.Graphics.RectInt32(
                _X: (int)Math.Round(bounds.X * scale),
                _Y: (int)Math.Round(bounds.Y * scale),
                _Width: (int)Math.Round(bounds.Width * scale),
                _Height: (int)Math.Round(bounds.Height * scale)
            );
        }

        #endregion
    }
}
#endif