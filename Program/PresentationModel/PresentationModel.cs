//____________________________________________________________________________________________________________________________________
//
//  Copyright (C) 2024, Mariusz Postol LODZ POLAND.
//
//  To be in touch join the community by pressing the `Watch` button and get started commenting using the discussion panel at
//
//  https://github.com/mpostol/TP/discussions/182
//_____________________________________________________________________________________________________________________________________

using System;
using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics;
using System.Reactive;
using System.Reactive.Linq;
using UnderneathLayerAPI = TP.ConcurrentProgramming.BusinessLogic.BusinessLogicAbstractAPI;

namespace TP.ConcurrentProgramming.Presentation.Model
{
  /// <summary>
  /// Class Model - implements the <see cref="ModelAbstractApi" />
  /// </summary>
  internal class ModelImplementation : ModelAbstractApi
  {
    internal ModelImplementation() : this(null)
    { }

    internal ModelImplementation(UnderneathLayerAPI underneathLayer)
    {
      layerBelow = underneathLayer == null ? UnderneathLayerAPI.GetBusinessLogicLayer() : underneathLayer;
      eventObservable = Observable.FromEventPattern<BallChangeEventArgs>(this, "BallChanged");
    }

    #region ModelAbstractApi

    public override void Dispose()
    {
      if (Disposed)
        throw new ObjectDisposedException(nameof(Model));
      layerBelow.Dispose();
      Balls.Clear();
      Disposed = true;
    }

    public override IDisposable Subscribe(IObserver<IBall> observer)
    {
      return eventObservable.Subscribe(x => observer.OnNext(x.EventArgs.Ball), ex => observer.OnError(ex), () => observer.OnCompleted());
    }

    public override void Start(int numberOfBalls)
    {
      _Running = true;

      layerBelow.Start(numberOfBalls, BallAdditionHandler, (ball) => { });
    }

    public override void Stop()
    {
      layerBelow.Stop();

      Balls.Clear();
    }

    public override int CurrentBallsCount { get; set; }

    public override bool Running => _Running;

    public override float Scale
    {
      get
      {
        return _Scale;
      }
      set
      {
        _Scale = value;

        foreach (var ball in Balls)
        {
          ball.Scale = _Scale;
        }
      }
    }

    public override ObservableCollection<IBall> Balls { get; } = new();

    #endregion ModelAbstractApi

    #region API

    public event EventHandler<BallChangeEventArgs> BallChanged;

    #endregion API

    #region private

    private bool Disposed = false;
    private bool _Running = false;
    private float _Scale = 1;
    private readonly IObservable<EventPattern<BallChangeEventArgs>> eventObservable = null;
    private readonly UnderneathLayerAPI layerBelow = null;

    private void BallAdditionHandler(BusinessLogic.IPosition position, BusinessLogic.IBall ball)
    {
      ModelBall newBall = new ModelBall(position.x, position.y, ball) { Diameter = 5, Scale = _Scale };
      BallChanged.Invoke(this, new BallChangeEventArgs() { Ball = newBall });
    }

    #endregion private

    #region TestingInfrastructure

    [Conditional("DEBUG")]
    internal void CheckObjectDisposed(Action<bool> returnInstanceDisposed)
    {
      returnInstanceDisposed(Disposed);
    }

    [Conditional("DEBUG")]
    internal void CheckUnderneathLayerAPI(Action<UnderneathLayerAPI> returnNumberOfBalls)
    {
      returnNumberOfBalls(layerBelow);
    }

    [Conditional("DEBUG")]
    internal void CheckBallChangedEvent(Action<bool> returnBallChangedIsNull)
    {
      returnBallChangedIsNull(BallChanged == null);
    }

    #endregion TestingInfrastructure
  }

  public class BallChangeEventArgs : EventArgs
  {
    public IBall Ball { get; init; }
  }
}