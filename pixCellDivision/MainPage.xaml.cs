using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Popups;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace pixCellDivision
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        public MainPage()
        {
            this.InitializeComponent();
        }

        /// <summary>
        /// Invoked when this page is about to be displayed in a Frame.
        /// </summary>
        /// <param name="e">Event data that describes how this page was reached.  The Parameter
        /// property is typically used to configure the page.</param>
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
        }

        private async void ScrollViewer_ManipulationStarted_1(object sender, ManipulationStartedRoutedEventArgs e)
        {
            /*MessageDialog dialog = new MessageDialog("test", "test2");
            await dialog.ShowAsync();*/
        }

        private void Zoom_Out(object sender, RoutedEventArgs e)
        {
            float newZoomFactor = Math.Max(1.0f, DrawingContainer.ZoomFactor - 1);
            DrawingContainer.ZoomToFactor(newZoomFactor);
        }

        private void Zoom_In(object sender, RoutedEventArgs e)
        {
            DrawingContainer.ZoomToFactor(DrawingContainer.ZoomFactor + 1);
        }
    }
}
