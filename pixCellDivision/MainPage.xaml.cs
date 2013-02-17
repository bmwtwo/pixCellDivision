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

        // record undo type
        Stack<Boolean> undoIsColor            = new Stack<Boolean>();
        // record details of color undos
        Stack<Rectangle> undoColorRect        = new Stack<Rectangle>();
        Stack<SolidColorBrush> undoColorBrush = new Stack<SolidColorBrush>();
        // record details of splitting undos
        Stack<Rectangle> undoResize           = new Stack<Rectangle>();
        Stack<Rectangle> undoDelete           = new Stack<Rectangle>();

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
            if (args.VirtualKey == Windows.System.VirtualKey.H)
                Hor_Split(selectedRectangle);
            else if (args.VirtualKey == Windows.System.VirtualKey.V)
                Ver_Split(selectedRectangle);
            else if (args.VirtualKey == Windows.System.VirtualKey.U || args.VirtualKey == Windows.System.VirtualKey.Z)
                undo();
        }

        private void performCommonSplitTasks(Rectangle oldRect, Rectangle newRect)
        {
            DrawingCanvas.Children.Add(newRect);

            undoIsColor.Push(false);
            undoResize.Push(oldRect);
            undoDelete.Push(newRect);
            if (UndoButton.IsEnabled != true)
                UndoButton.IsEnabled = true;
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
            performCommonSplitTasks(oldRect, newRect);

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
            performCommonSplitTasks(oldRect, newRect);

            oldRect.Width = newWidth;
        }

		private void onButtonClick(object sender, RoutedEventArgs e) 
		{
            Color newColor;
			Button senderButton = sender as Button;
			switch (senderButton.Name)
			{
				case "grayButton":
					newColor = Colors.Gray;	
					break;
				case "redButton":
					newColor = Colors.Red;	
					break;
				case "blueButton":
					newColor = Colors.Blue;	
					break;	
				case "greenButton":
					newColor = Colors.Green;	
					break;	
				case "brownButton":
					newColor = Colors.Brown;	
					break;	
				case "orangeButton":
					newColor = Colors.Orange;	
					break;	
				case "tealButton":
					newColor = Colors.Teal;	
					break;	
				case "magentaButton":
					newColor = Colors.Magenta;	
					break;	
				case "limeButton":
					newColor = Colors.Lime;	
					break;	
				case "purpleButton":
					newColor = Colors.Purple;	
					break;	
				case "pinkButton":
					newColor = Colors.Pink;	
					break;	
				case "cyanButton":
					newColor = Colors.Cyan;	
					break;	
				case "darkGreenButton":
					newColor = Colors.DarkGreen;	
					break;	
				case "blackButton":
					newColor = Colors.Black;	
					break;	
				case "whiteButton":
					newColor = Colors.White;	
					break;	
				default:
					break;
			}

            SolidColorBrush brush = new SolidColorBrush(newColor);
            undoIsColor.Push(true);
            undoColorBrush.Push(selectedRectangle.Fill as SolidColorBrush);
            undoColorRect.Push(selectedRectangle);
            if (UndoButton.IsEnabled == false)
                UndoButton.IsEnabled = true;

            selectedRectangle.Fill = brush;
		}

        private void Undo_Click(object sender, RoutedEventArgs e)
        {
            undo();
        }

        private void undo()
        {
            if (undoIsColor.Count == 0) return; // can't undo

            if (undoIsColor.Pop() == true)
            {
                MessageDialog d = new MessageDialog("here");
                undoColorRect.Pop().Fill = undoColorBrush.Pop();
            }
            else { // undo a split
                Rectangle rectToDelete = undoDelete.Pop();
                Rectangle rectToResize = undoResize.Pop();
                // if the squares have the same left margin, undoDelete must be directly below undoResize
                if (rectToDelete.Margin.Left == rectToResize.Margin.Left)
                    rectToResize.Height *= 2;
                else
                    rectToResize.Width *= 2;

                if (rectToDelete == selectedRectangle)
                {
                    selectedRectangle = First_Rectangle;
                    First_Rectangle.Stroke = new SolidColorBrush(Colors.Blue);
                }
                DrawingCanvas.Children.Remove(rectToDelete);
            }

            if (undoIsColor.Count == 0) UndoButton.IsEnabled = false;
        }
    }
}
