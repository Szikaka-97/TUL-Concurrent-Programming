//____________________________________________________________________________________________________________________________________
//
//  Copyright (C) 2024, Mariusz Postol LODZ POLAND.
//
//  To be in touch join the community by pressing the `Watch` button and get started commenting using the discussion panel at
//
//  https://github.com/mpostol/TP/discussions/182
//
//_____________________________________________________________________________________________________________________________________

using System;
using System.Diagnostics;

namespace TP.ConcurrentProgramming.Data
{
  internal class DataImplementation : DataAbstractAPI
  {
    #region ctor

    public DataImplementation()
    {
      MoveTimer = new Timer(Move, null, TimeSpan.Zero, TimeSpan.FromMilliseconds(50));
    }

    #endregion ctor

    #region DataAbstractAPI

    public override void Start(int numberOfBalls, Action<IVector, IBall> ballCreationHandler, Action<IBall> ballRemovalHandler)
    {
      if (Disposed)
        throw new ObjectDisposedException(nameof(DataImplementation));
      if (ballCreationHandler == null)
        throw new ArgumentNullException(nameof(ballCreationHandler));
      if (ballRemovalHandler == null)
        throw new ArgumentNullException(nameof(ballRemovalHandler));

      BallCreationHandler = ballCreationHandler;
      BallRemovalHandler = ballRemovalHandler;

      for (int i = 0; i < numberOfBalls; i++)
      {
        AddBall();
      }
    }

    public override void EndSimulation()
    {
      if (Disposed)
        throw new ObjectDisposedException(nameof(DataImplementation));
      BallsList.Clear();
    }

    public override void AddBall()
    {
      if (Disposed)
        throw new ObjectDisposedException(nameof(DataImplementation));

      Vector startingPosition = new(RandomGenerator.Next(10, TableSize - 10), RandomGenerator.Next(10, TableSize - 10));
      Ball newBall = new(startingPosition, startingPosition);
      BallCreationHandler(startingPosition, newBall);
      BallsList.Add(newBall);
    }

    public override void RemoveBall()
    {
      if (Disposed)
        throw new ObjectDisposedException(nameof(DataImplementation));

      IBall removed = BallsList.Last();
      BallsList.RemoveAt(BallsList.Count - 1);

      BallRemovalHandler(removed);
    }

    #endregion DataAbstractAPI

    #region IDisposable

    protected virtual void Dispose(bool disposing)
    {
      if (!Disposed)
      {
        if (disposing)
        {
          MoveTimer.Dispose();
          BallsList.Clear();
        }
        Disposed = true;
      }
      else
        throw new ObjectDisposedException(nameof(DataImplementation));
    }

    public override void Dispose()
    {
      // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
      Dispose(disposing: true);
      GC.SuppressFinalize(this);
    }

    #endregion IDisposable

    #region private

    //private bool disposedValue;
    private bool Disposed = false;

    private readonly Timer MoveTimer;
    private Random RandomGenerator = new();
    private List<Ball> BallsList = [];
    private Action<IVector, IBall> BallCreationHandler;
    private Action<IBall> BallRemovalHandler;
    private int TableSize = 100;

    private void Move(object? x)
    {
      foreach (Ball item in BallsList)
      {
        double bearing = RandomGenerator.NextDouble() * 2 * Math.PI;
        double dist = RandomGenerator.NextDouble();

        var movement = new Vector(
          Math.Cos(bearing) * dist,
          Math.Sin(bearing) * dist
        );

        movement = new Vector(
          Math.Clamp(movement.x, item.Radius - item.Position.x, TableSize - item.Position.x - item.Radius),
          Math.Clamp(movement.y, item.Radius - item.Position.y, TableSize - item.Position.y - item.Radius)
        );

        
        item.Move(movement);
      }
    }

    #endregion private

    #region TestingInfrastructure

    [Conditional("DEBUG")]
    internal void CheckBallsList(Action<IEnumerable<IBall>> returnBallsList)
    {
      returnBallsList(BallsList);
    }

    [Conditional("DEBUG")]
    internal void CheckNumberOfBalls(Action<int> returnNumberOfBalls)
    {
      returnNumberOfBalls(BallsList.Count);
    }

    [Conditional("DEBUG")]
    internal void CheckObjectDisposed(Action<bool> returnInstanceDisposed)
    {
      returnInstanceDisposed(Disposed);
    }

    #endregion TestingInfrastructure
  }
}