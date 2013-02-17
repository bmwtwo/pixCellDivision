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
        Rectangle selectedRectangle;    
        public MainPage()   
        {
            this.InitializeComponent();
            selectedRectangle = First_Rectangle;
            Windows.UI.Core.CoreWindow.GetForCurrentThread().KeyDown += Core_KeyDown;
        }

        /// <summary>
        /// Invoked when this page is about to be displayed in a Frame.
        /// </summary>
        /// <param name="e">Event data that describes how this page was reached.  The Parameter
        /// property is typically used to configure the page.</param>
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
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

        Rectangle getNewRectangle(Rectangle oldRect)
        {
            Rectangle newRect = new Rectangle();
            newRect.Fill = oldRect.Fill;
            newRect.Stroke = new SolidColorBrush(Colors.Black);
            newRect.Tapped += new TappedEventHandler(Rectangle_Tapped);
            return newRect;
        }

        private void Vertical_Split_Clicked(object sender, RoutedEventArgs e)
        {
            Ver_Split(selectedRectangle);
        }

        private void Horizontal_Split_Clicked(object sender, RoutedEventArgs e)
        {
            Hor_Split(selectedRectangle);
        }

        private void Rectangle_Tapped(object sender, TappedRoutedEventArgs e)
        {
            Rectangle senderRect = sender as Rectangle;
            selectedRectangle.Stroke = new SolidColorBrush(Colors.Transparent);
            senderRect.Stroke = new SolidColorBrush(Colors.Blue);
            selectedRectangle = senderRect;
        } 

        private void Core_KeyDown(Windows.UI.Core.CoreWindow sender, Windows.UI.Core.KeyEventArgs args)
        {
            System.Diagnostics.Debug.WriteLine(args.VirtualKey);
            if (args.VirtualKey == Windows.System.VirtualKey.H)
                Hor_Split(selectedRectangle);
            else if (args.VirtualKey == Windows.System.VirtualKey.V)
                Ver_Split(selectedRectangle);
        }

        private void Hor_Split(Rectangle oldRect)
        {
            double newHeight = oldRect.Height / 2;
            double newTopMargin = oldRect.Margin.Top + newHeight;

            Rectangle newRect = getNewRectangle(oldRect);
            newRect.Height = newHeight;
            newRect.Width = oldRect.Width;
            newRect.Margin = new Thickness(
                oldRect.Margin.Left,
                newTopMargin,
                oldRect.Margin.Right,
                oldRect.Margin.Bottom
            );
            DrawingCanvas.Children.Add(newRect);

            oldRect.Height = newHeight;
        }

        private void Ver_Split(Rectangle oldRect)
        {
            double newWidth = oldRect.Width / 2;
            double newLeftMargin = oldRect.Margin.Left + newWidth;

            Rectangle newRect = getNewRectangle(oldRect);
            newRect.Width = newWidth;
            newRect.Height = oldRect.Height;
            newRect.Margin = new Thickness(
                newLeftMargin,
                oldRect.Margin.Top,
                oldRect.Margin.Right,
                oldRect.Margin.Bottom
            );
            DrawingCanvas.Children.Add(newRect);

            oldRect.Width = newWidth;
        }

		private void onButtonClick(object sender, RoutedEventArgs e) 
		{
			Button senderButton = sender as Button;
			switch (senderButton.Name)
			{
				case "grayButton":
					selectedRectangle.Fill = new SolidColorBrush(Colors.Gray);	
					break;
				case "redButton":
					selectedRectangle.Fill = new SolidColorBrush(Colors.Red);	
					break;
				case "blueButton":
					selectedRectangle.Fill = new SolidColorBrush(Colors.Blue);	
					break;	
				case "greenButton":
					selectedRectangle.Fill = new SolidColorBrush(Colors.Green);	
					break;	
				case "brownButton":
					selectedRectangle.Fill = new SolidColorBrush(Colors.Brown);	
					break;	
				case "orangeButton":
					selectedRectangle.Fill = new SolidColorBrush(Colors.Orange);	
					break;	
				case "tealButton":
					selectedRectangle.Fill = new SolidColorBrush(Colors.Teal);	
					break;	
				case "magentaButton":
					selectedRectangle.Fill = new SolidColorBrush(Colors.Magenta);	
					break;	
				case "limeButton":
					selectedRectangle.Fill = new SolidColorBrush(Colors.Lime);	
					break;	
				case "purpleButton":
					selectedRectangle.Fill = new SolidColorBrush(Colors.Purple);	
					break;	
				case "pinkButton":
					selectedRectangle.Fill = new SolidColorBrush(Colors.Pink);	
					break;	
				case "cyanButton":
					selectedRectangle.Fill = new SolidColorBrush(Colors.Cyan);	
					break;	
				case "darkGreenButton":
					selectedRectangle.Fill = new SolidColorBrush(Colors.DarkGreen);	
					break;	
				case "blackButton":
					selectedRectangle.Fill = new SolidColorBrush(Colors.Black);	
					break;	
				case "whiteButton":
					selectedRectangle.Fill = new SolidColorBrush(Colors.White);	
					break;	
				default:
					break;
			}
		}
    }
}
