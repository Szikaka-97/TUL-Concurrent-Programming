//___________________________________+_______________________________________________________
//
//  Copyright 2024 Mariusz Postol LODZ POLAND.
//
//  To be in touch join the community by pressing the `Watch` button and to get started
//  comment using the discussion panel at
//  https://github.com/mpostol/TP/discussions/182
//__________________________________________________________________________________________

using Newtonsoft.Json.Linq;
using System;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Input;
using TP.ConcurrentProgramming.Presentation.ViewModel;

namespace TP.ConcurrentProgramming.PresentationView
{
    /// <summary>
    /// View implementation
    /// </summary>
    public partial class MainWindow : Window
    {
        private void ValidateNumberInTextbox(object sender, TextCompositionEventArgs e)
        {
            Regex regex = new Regex("[^0-9]+");
            e.Handled = regex.IsMatch(e.Text);
        }

        private void StartButtonClick(object sender, RoutedEventArgs e)
        {
            if (Int32.TryParse(BallCountInputBox.Text, out int ballsCount) && ballsCount > 0)
            {
                if (DataContext is MainWindowViewModel viewModel)
                {
                    BallCountInputBox.Focusable = false;

                    viewModel.InitializeObserver();
                    viewModel.Start(ballsCount);
                }
            }
            else
            {
                MessageBox.Show("Please pick a non-zero number of balls");
            }

        }
        private void StopButtonClick(object sender, RoutedEventArgs e)
        {
            if (DataContext is MainWindowViewModel viewModel)
            {
                viewModel.Stop();
            }
        }

        private void AddBallButtonClick(object sender, RoutedEventArgs e)
        {
            if (Int32.TryParse(BallCountInputBox.Text, out int ballsCount))
            {
                BallCountInputBox.Text = (ballsCount + 1).ToString();
            }
            else
            {
                BallCountInputBox.Text = "1";
            }

            UpdateBallCountButtons(sender, e);
        }
        
        private void RemoveBallButtonClick(object sender, RoutedEventArgs e)
        {
            if (Int32.TryParse(BallCountInputBox.Text, out int ballsCount) && ballsCount > 0)
            {
                BallCountInputBox.Text = (ballsCount - 1).ToString();
            }
            else
            {
                BallCountInputBox.Text = "0";
            }

            UpdateBallCountButtons(sender, e);
        }

        private void UpdateBallCountButtons(object sender, EventArgs e)
        {
            if (!Int32.TryParse(BallCountInputBox.Text, out int ballsCount)) return;

            RemoveBallButton.IsEnabled = ballsCount > 0;
        }

        public MainWindow()
        {
            InitializeComponent();
            MainWindowViewModel viewModel = (MainWindowViewModel)DataContext;
            double screenWidth = SystemParameters.PrimaryScreenWidth;
            double screenHeight = SystemParameters.PrimaryScreenHeight;
        }

        /// <summary>
        /// Raises the <seealso cref="System.Windows.Window.Closed"/> event.
        /// </summary>
        /// <param name="e">An <see cref="EventArgs"/> that contains the event data.</param>
        protected override void OnClosed(EventArgs e)
        {
            if (DataContext is MainWindowViewModel viewModel)
                viewModel.Dispose();
            base.OnClosed(e);
        }

    }
}