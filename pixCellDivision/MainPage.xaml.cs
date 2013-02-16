using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI;
using Windows.UI.Popups;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using Windows.UI.Xaml.Shapes;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace pixCellDivision
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        Rectangle selectedRectangle = null;

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

        Rectangle getNewRectangle(Rectangle old)
        {
            Rectangle newRect = new Rectangle();
            newRect.Fill = old.Fill;
            newRect.Stroke = new SolidColorBrush(Colors.Black);
            newRect.Tapped += new TappedEventHandler(Rectangle_Tapped);
            return newRect;
        }

        private void Vertical_Split(object sender, RoutedEventArgs e)
        {
            double newWidth = selectedRectangle.Width / 2;
            double newLeftMargin = selectedRectangle.Margin.Left + newWidth;

            Rectangle newRect = getNewRectangle(selectedRectangle);
            newRect.Width = newWidth;
            newRect.Height = selectedRectangle.Height;
            newRect.Margin = new Thickness(
                newLeftMargin,
                selectedRectangle.Margin.Top,
                selectedRectangle.Margin.Right,
                selectedRectangle.Margin.Bottom
            );
            DrawingCanvas.Children.Add(newRect);

            selectedRectangle.Width = newWidth;
        }

        private void Horizontal_Split(object sender, RoutedEventArgs e)
        {
            double newHeight = selectedRectangle.Height / 2;
            double newTopMargin = selectedRectangle.Margin.Top + newHeight;

            Rectangle newRect = getNewRectangle(selectedRectangle);
            newRect.Height = newHeight;
            newRect.Width = selectedRectangle.Width;
            newRect.Margin = new Thickness(
                selectedRectangle.Margin.Left,
                newTopMargin,
                selectedRectangle.Margin.Right,
                selectedRectangle.Margin.Bottom
            );
            DrawingCanvas.Children.Add(newRect);

            selectedRectangle.Height = newHeight;
        }

        private void Rectangle_Tapped(object sender, TappedRoutedEventArgs e)
        {
            Rectangle senderRect = sender as Rectangle;
            if (selectedRectangle != null)
                selectedRectangle.Stroke = new SolidColorBrush(Colors.Transparent);
            senderRect.Stroke = new SolidColorBrush(Colors.Blue);
            selectedRectangle = senderRect;
            if (!horizontalSplitButton.IsEnabled) horizontalSplitButton.IsEnabled = true;
            if (!verticalSplitButton.IsEnabled) verticalSplitButton.IsEnabled = true;
        }
    }
}
