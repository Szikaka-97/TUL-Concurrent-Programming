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
using System.Windows;
using TP.ConcurrentProgramming.Presentation.ViewModel;

namespace TP.ConcurrentProgramming.PresentationView
{
    /// <summary>
    /// View implementation
    /// </summary>
    public partial class MainWindow : Window
    {
        private int ballsValues = 0;

        private void BallSelector_Changed(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            if (BallSelector.SelectedItem is string value && value != "Liczba kulek")
            {
                MessageBox.Show($"Wybrano: {value}");
                ballsValues = int.Parse(value);
            }
            else
            {
                MessageBox.Show("Niepoprawna wartosc liczba wyswietlonych kulek bedzie = 0");
                ballsValues = 0;
            }
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            if (ballsValues != 0)
            {
                if (DataContext is MainWindowViewModel viewModel)
                {
                    viewModel.InitializeObserver();
                    viewModel.Start(ballsValues);
                }
                BallSelector.IsEnabled = false;  
            }
            else
            {
                MessageBox.Show("Nic sie nie wyswietli gdyz nie wybrano wartosci");
            }

        }
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            if (DataContext is MainWindowViewModel viewModel)
            {
                viewModel.Balls.Clear();
                MessageBox.Show("Usunolem wszystkie kulki");
            }
            BallSelector.IsEnabled = true;

        }

        public MainWindow()
        {
            InitializeComponent();
            MainWindowViewModel viewModel = (MainWindowViewModel)DataContext;
            double screenWidth = SystemParameters.PrimaryScreenWidth;
            double screenHeight = SystemParameters.PrimaryScreenHeight;
            BallSelector.Items.Add("Liczba kulek");
            for (int i = 1; i <= 20; i++)
            {
                BallSelector.Items.Add(i.ToString());
            }
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