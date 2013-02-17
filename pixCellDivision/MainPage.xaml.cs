using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI;
using Windows.UI.ApplicationSettings;
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
        SolidColorBrush DEFAULT_STROKE  = new SolidColorBrush(Colors.LightGray);
        SolidColorBrush SELECTED_STROKE = new SolidColorBrush(Colors.Black);

        Boolean needrec = false;
        Boolean needsplit = false;
        Boolean needcolor = false;
        String[] macro;
        Boolean rec_state = false;
        Rectangle[] rec_children;
        Boolean recorded = false;
        Boolean runningmac = false;
        Rectangle[] running_array;
        int child_index;

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

            ZoomIn.Text = "" + '\uE12E';
            ZoomOut.Text = "" + '\uE1A4';
            HorizontalSplit.Text = "" + '\uE0CE';
            VerticalSplit.Text = "" + '\uE0CD';
            UndoIcon.Text = "" + '\uE10E';
            RecordIcon.Text = "" + '\uE116';
            StopIcon.Text = "" + '\uE002';
            PlayIcon.Text = "" + '\uE102';

            // add settings options
            SettingsPane.GetForCurrentView().CommandsRequested += onSettingsCommandsRequested;
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
            newRect.Stroke = DEFAULT_STROKE;
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
            if (needsplit || needcolor)
                return;
            Rectangle senderRect = sender as Rectangle;
            selectedRectangle.Stroke = DEFAULT_STROKE;
            senderRect.Stroke = SELECTED_STROKE;
            selectedRectangle = senderRect;

            if (rec_state)
            {
                int srcindex = 0;
                for (int i = 0; i <= child_index; i++)
                    if (selectedRectangle.Equals(rec_children[i]))
                        srcindex = i;
                macro[child_index] += srcindex + "";
                needrec = false;
                needsplit = true;
                RecInst.Text = "Split the rectangle.";
            }   
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
            if (needcolor || needrec)
                return;
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
            if (rec_state)
            {
                needcolor = true;
                needsplit = false;
                rec_children[child_index + 1] = newRect;
                macro[child_index] += "h";
                RecInst.Text = "Pick a color.";
            }
            if (runningmac)
            {
                running_array[child_index + 1] = newRect;
                child_index++;
            }
        }

        private void Ver_Split(Rectangle oldRect)
        {
            if (needrec || needcolor)
                return;
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
            if (rec_state)
            {
                needsplit = false;
                needcolor = true;
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
            if (needrec == true || needsplit == true)
                return;
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

            if (rec_state)
            {
                needcolor = false;
                needrec = true;
                macro[child_index] += senderButton.Content;
                child_index++;
                RecInst.Text = "Pick a rectangle.";
            }


            SolidColorBrush brush = new SolidColorBrush(newColor);
            undoIsColor.Push(true);
            undoColorBrush.Push(selectedRectangle.Fill as SolidColorBrush);
            undoColorRect.Push(selectedRectangle);
            if (UndoButton.IsEnabled == false)
                UndoButton.IsEnabled = true;

            selectedRectangle.Fill = brush;
		}

        private void recMacro(object sender, RoutedEventArgs e)
        {
            needrec = true;
            macro = new String[10];
            rec_children = new Rectangle[11];
            rec_children[0] = selectedRectangle;
            child_index = 0;
            rec_state = true;
            RecInst.Text = "The Macro has started. Please add commands in the following order: pick, split, color.";
            RecInst.Visibility = Visibility.Visible;
        }

        private void stopMacro(object sender, RoutedEventArgs e)
        {
            if (!recorded)
            {
                RecInst.Text = "You have not recorded a macro. There is nothing to stop.";
                RecInst.Visibility = Visibility.Visible;
            }
            if (rec_state)
            {
                rec_state = false;
                RecInst.Text = "Your macro has stopped recording.";
                recorded = true;
                needcolor = false;
                needrec = false;
                needsplit = false;
            }

        }

        private void PlayMacro(object sender, RoutedEventArgs e)
        {
            if (!recorded)
            {
                RecInst.Text = "You have not recorded a macro to play yet.";
                RecInst.Visibility = Visibility.Visible;
                return;
            }
            runningmac = true;
            Rectangle current;
            rec_state = false;
            running_array = new Rectangle[11];
            running_array[0] = selectedRectangle;
            int times = child_index;
            child_index = 0;
            for (int i = 0; i < 10; i++)
            {
                if (macro[i] == null)
                    break;
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
                DrawingCanvas.Visibility = Windows.UI.Xaml.Visibility.Visible;
            }
            runningmac = false;
            child_index = times;
        }



        private void Undo_Click(object sender, RoutedEventArgs e)
        {
            undo();
        }

        private void undo()
        {
            if (needsplit || needrec || needcolor)
                return;
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
                    First_Rectangle.Stroke = SELECTED_STROKE;
                }
                DrawingCanvas.Children.Remove(rectToDelete);
            }

            if (undoIsColor.Count == 0) UndoButton.IsEnabled = false;

        }

        // settings menu stuff below this point
        void onEmailCommand(IUICommand command)
        {
            Uri uri = new Uri("mailto:bmwtwo@princeton.edu?subject=pixCellDivision%20Feedback");
            Windows.System.Launcher.LaunchUriAsync(uri);
        }

        void onSettingsCommandsRequested(SettingsPane settingsPane, SettingsPaneCommandsRequestedEventArgs eventArgs)
        {
            UICommandInvokedHandler handler = new UICommandInvokedHandler(onEmailCommand);
            SettingsCommand emailCommand = new SettingsCommand("helpPage", "Tips & Feedback", handler);
            eventArgs.Request.ApplicationCommands.Add(emailCommand);
        }
    }
}
