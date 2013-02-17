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
        String[] macro;
        Boolean rec_state = false;
        Rectangle[] rec_children;
        Boolean recorded = false;
        Boolean runningmac = false;
        Rectangle[] running_array;
        int child_index;
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

            if (rec_state)
            {
                int srcindex = 0;
                for (int i = 0; i < child_index; i++)
                    if (selectedRectangle.Equals(rec_children[i]))
                        srcindex = i;
                macro[child_index] += srcindex + "";
                RecInst.Text = "Split the rectangle.";
            }   
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
            if (rec_state)
            {
                rec_children[child_index + 1] = newRect;
                macro[child_index] += "h";
                RecInst.Text = "Pick a color.";
            }
            if (runningmac)
                running_array[child_index + 1] = newRect;
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
            if (rec_state)
            {
                rec_children[child_index + 1] = newRect;
                macro[child_index] += "v";
                RecInst.Text = "Pick a color.";
            }
            if (runningmac)
            {
                running_array[child_index + 1] = newRect;
                child_index++;
            }
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
            if (rec_state)
            {
                macro[child_index] += senderButton.Content;
                child_index++;
                RecInst.Text = "Pick a rectangle.";
            }
		}

        private void recMacro(object sender, RoutedEventArgs e)
        {
            macro = new String[10];
            rec_children = new Rectangle[10];
            child_index = 0;
            rec_state = true;
            RecInst.Text = "The Macro has started: Please do things in the following order. Pick, Split, Color.";
            RecInst.Visibility = Visibility.Visible;
        }

        private void stopMacro(object sender, RoutedEventArgs e)
        {
            if (!recorded)
                RecInst.Text = "You have not recorded a macro. There is nothing to stop.";
            if (rec_state)
                rec_state = false;
            RecInst.Text = "Your macro has stopped recording.";
            recorded = true;
        }

        private void PlayMacro(object sender, RoutedEventArgs e)
        {
            if (!recorded)
                RecInst.Text = "You have not recorded a macro to play yet.";
            runningmac = true;
            Rectangle current;
            running_array = new Rectangle[10];
            running_array[0] = selectedRectangle;
            int times = child_index;
            for (int i = 0; i < times; i++)
            {
                child_index = 0;
                current = running_array[int.Parse(macro[i][0] + "")];
                switch (macro[i][1])
                {
                    case 'h': Hor_Split(current); break;
                    case 'v': Ver_Split(current); break;
                }
                switch (macro[i].Substring(2, 2))
                {
                    case "Gr": current.Fill = new SolidColorBrush(Colors.Gray); break;
                    case "Re": current.Fill = new SolidColorBrush(Colors.Red); break;
                    case "Bl": current.Fill = new SolidColorBrush(Colors.Blue); break;
                    case "Gn": current.Fill = new SolidColorBrush(Colors.Green); break;
                    case "Br": current.Fill = new SolidColorBrush(Colors.Brown); break;
                    case "Or": current.Fill = new SolidColorBrush(Colors.Orange); break;
                    case "Te": current.Fill = new SolidColorBrush(Colors.Teal); break;
                    case "Ma": current.Fill = new SolidColorBrush(Colors.Magenta); break;
                    case "Li": current.Fill = new SolidColorBrush(Colors.Lime); break;
                    case "Pu": current.Fill = new SolidColorBrush(Colors.Purple); break;
                    case "Pi": current.Fill = new SolidColorBrush(Colors.Pink); break;
                    case "Cy": current.Fill = new SolidColorBrush(Colors.Cyan); break;
                    case "Dg": current.Fill = new SolidColorBrush(Colors.DarkGreen); break;
                    case "Bk": current.Fill = new SolidColorBrush(Colors.Black); break;
                    case "Wh": current.Fill = new SolidColorBrush(Colors.White); break;
                }
            }
            runningmac = false;
            child_index = times;
        }
    }
}
