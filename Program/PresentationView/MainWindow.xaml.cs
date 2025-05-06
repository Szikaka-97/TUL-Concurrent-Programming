//___________________________________+_______________________________________________________
//
//  Copyright 2024 Mariusz Postol LODZ POLAND.
//
//  To be in touch join the community by pressing the `Watch` button and to get started
//  comment using the discussion panel at
//  https://github.com/mpostol/TP/discussions/182
//__________________________________________________________________________________________

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
    public MainWindow()
    {
      InitializeComponent();
      MainWindowViewModel viewModel = (MainWindowViewModel) DataContext;
    }

    // Workaround because we cannot bind events to a WPF app
    private void Window_SizeChanged(object sender, SizeChangedEventArgs e)
    {
      if (DataContext is MainWindowViewModel viewModel)
        viewModel.TableSizeChanged(sender, e);
    }

    private void Window_Closed(object sender, EventArgs e)
    {
      if (DataContext is MainWindowViewModel viewModel)
        viewModel.Dispose();
    }
  }
}