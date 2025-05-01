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
using System.Collections.Generic;
using System.ComponentModel;
using Newtonsoft.Json.Linq;

namespace TP.ConcurrentProgramming.Presentation.Model
{
  public interface IBall : INotifyPropertyChanged
  {
    double Top { get; }
    double Left { get; }
    double Diameter { get; }

    void UpdateScale();
  }

  public delegate void ScaleChangeHandler(double newScale);

  public abstract class ModelAbstractApi : IObservable<IBall>, IDisposable
  {
    public static ModelAbstractApi CreateModel()
    {
      return modelInstance.Value;
    }

    public abstract void Start(int numberOfBalls);

    public abstract void Stop();

    public abstract int CurrentBallsCount { get; set; }

    public abstract bool Running { get; }

    public abstract float Scale { get; set; }

    public abstract ObservableCollection<IBall> Balls { get; }

    #region IObservable

    public abstract IDisposable Subscribe(IObserver<IBall> observer);

    #endregion IObservable

    #region IDisposable

    public abstract void Dispose();

    #endregion IDisposable

    #region private

    private static Lazy<ModelAbstractApi> modelInstance = new Lazy<ModelAbstractApi>(() => new ModelImplementation());

    #endregion private
  }
}