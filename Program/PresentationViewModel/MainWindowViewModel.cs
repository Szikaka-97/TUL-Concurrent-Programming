//__________________________________________________________________________________________
//
//  Copyright 2024 Mariusz Postol LODZ POLAND.
//
//  To be in touch join the community by pressing the `Watch` button and to get started
//  comment using the discussion panel at
//  https://github.com/mpostol/TP/discussions/182
//__________________________________________________________________________________________

using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics.Tracing;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Data;
using System.Windows.Input;
using Newtonsoft.Json.Converters;
using TP.ConcurrentProgramming.Presentation.Model;
using TP.ConcurrentProgramming.Presentation.ViewModel.MVVMLight;
using TP.ConcurrentProgramming.PresentationViewModel;
using ModelIBall = TP.ConcurrentProgramming.Presentation.Model.IBall;

namespace TP.ConcurrentProgramming.Presentation.ViewModel
{
  public class MainWindowViewModel : ViewModelBase, IDisposable
  {
    public ICommand AddBall { get; init; }
    public ICommand RemoveBall { get; init; }
    public ICommand StartSimulation { get; init; }

    public string BallCount
    {
      get
      {
        return ModelLayer?.CurrentBallsCount.ToString();
      }
      set
      {
        if (Int32.TryParse(value, out var count))
        {
          ModelLayer.CurrentBallsCount = count;

          RaisePropertyChanged("BallCount");
        }
      }
    }

    #region ctor

    public MainWindowViewModel() : this(null)
    { }

    internal MainWindowViewModel(ModelAbstractApi modelLayerAPI)
    {
      ModelLayer = modelLayerAPI == null ? ModelAbstractApi.CreateModel() : modelLayerAPI;
      Observer = ModelLayer.Subscribe<ModelIBall>(x => Balls.Add(x));

      AddBall = new BindedCommand(ExecuteAddBall, CanAddBall);
      RemoveBall = new BindedCommand(ExecuteRemoveBall, CanRemoveBall);
      StartSimulation = new BindedCommand(ExecuteStartSimulation, CanStartSimulation);
    }

    #endregion ctor

    #region public API

    public void Start(int numberOfBalls)
    {
      if (Disposed)
        throw new ObjectDisposedException(nameof(MainWindowViewModel));
      ModelLayer.Start(numberOfBalls);
      Observer.Dispose();
    }

    public void Stop()
    {
      if (Disposed)
        throw new ObjectDisposedException(nameof(MainWindowViewModel));

      ModelLayer.Stop();
    }

    public void ExecuteAddBall(object? param)
    {
      ModelLayer.CurrentBallsCount++;

      RaisePropertyChanged("BallCount");
    }

    public bool CanAddBall(object? param)
    {
      return ModelLayer.CurrentBallsCount < 20 && !ModelLayer.Running;
    }

    public void ExecuteRemoveBall(object? param)
    {
      ModelLayer.CurrentBallsCount--;

      RaisePropertyChanged("BallCount");
    }

    public bool CanRemoveBall(object? param)
    {
      return ModelLayer.CurrentBallsCount > 0 && !ModelLayer.Running;
    }

    public void ExecuteStartSimulation(object? param)
    {
      Start(ModelLayer.CurrentBallsCount);
    }

    public bool CanStartSimulation(object? param)
    {
      return ModelLayer != null && ModelLayer.CurrentBallsCount > 0 && !ModelLayer.Running;
    }

    public double TableSize {
      get => ModelLayer.TableSize;
      set 
      {
        ModelLayer.TableSize = value;
        RaisePropertyChanged(nameof(TableSize));
      }
    }

    public void TableSizeChanged(object sender, SizeChangedEventArgs e)
    {
      double minSize = Math.Min(e.NewSize.Width - 8, e.NewSize.Height - 89);

      TableSize = (float) minSize;
    }

    public ObservableCollection<ModelIBall> Balls => ModelLayer.Balls;

    public void InitializeObserver()
    {
      if (Disposed)
        throw new ObjectDisposedException(nameof(MainWindowViewModel));
      Observer = ModelLayer.Subscribe<ModelIBall>(Balls.Add);
    }
    #endregion public API

    #region IDisposable

    protected virtual void Dispose(bool disposing)
    {
      if (!Disposed)
      {
        if (disposing)
        {
          Observer.Dispose();
          ModelLayer.Dispose();
        }

        // TODO: free unmanaged resources (unmanaged objects) and override finalizer
        // TODO: set large fields to null
        Disposed = true;
      }
    }

    public void Dispose()
    {
      if (Disposed)
        throw new ObjectDisposedException(nameof(MainWindowViewModel));
      Dispose(disposing: true);
      GC.SuppressFinalize(this);
    }

    #endregion IDisposable

    #region private

    private IDisposable Observer = null;
    private ModelAbstractApi ModelLayer;
    private bool Disposed = false;

    #endregion private
  }
}