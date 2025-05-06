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
using System.Threading;

namespace TP.ConcurrentProgramming.Data
{
  internal class DataImplementation : DataAbstractAPI
  {
    #region ctor

    public DataImplementation()
    { }

    #endregion ctor

    #region DataAbstractAPI

    public override void Start(int numberOfBalls, int frameTime, Action<IVector, IBall> ballCreationHandler, Action<IBall> ballRemovalHandler)
    {
      if (Disposed)
        throw new ObjectDisposedException(nameof(DataImplementation));
      if (ballCreationHandler == null)
        throw new ArgumentNullException(nameof(ballCreationHandler));
      if (ballRemovalHandler == null)
        throw new ArgumentNullException(nameof(ballRemovalHandler));

      BallCreationHandler = ballCreationHandler;
      BallRemovalHandler = ballRemovalHandler;

      FrameTime = frameTime;

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
      double bearing = RandomGenerator.NextDouble();
      Vector vel = new Vector(Math.Sin(bearing), Math.Cos(bearing));

      Ball newBall = new(startingPosition, vel);
      BallsList.Add(newBall);

      BallCreationHandler(newBall.Position, newBall);

      var cts = new CancellationTokenSource();
      BallThreads[newBall] = cts;

      new Thread( () =>
      {
        int localFrameTime = FrameTime;

        while (!cts.Token.IsCancellationRequested)
        {
          float deltaTime = (float) localFrameTime / FrameTime;

          // Handle collision with another ball that might've occured
          if (newBall.NextCollision != null)
          {
            localFrameTime = newBall.NextCollision.time;
            newBall.Velocity = IVector.Reflect(newBall.Velocity, newBall.NextCollision.normal);
            
            newBall.ClearCollision();
          }

          newBall.Move(deltaTime);

          localFrameTime = FrameTime;

          if (newBall.NextCollision != null)
          {
            localFrameTime = newBall.NextCollision.time;
            newBall.Velocity = IVector.Reflect(newBall.Velocity, newBall.NextCollision.normal);

            if (newBall.NextCollision.otherBall != null)
            {
              var otherCollisionEvent = new CollisionEvent(newBall.NextCollision.time, newBall.NextCollision.normal * -1, newBall);

              newBall.NextCollision.otherBall.NotifyCollision(otherCollisionEvent);
            }

            newBall.ClearCollision();
          }

          Thread.Sleep(localFrameTime);
        }
      }).Start();
    }

    public override void RemoveBall()
    {
      if (Disposed)
        throw new ObjectDisposedException(nameof(DataImplementation));
      if (BallsList.Count == 0)
        return;

      Ball removed = BallsList.Last();
      BallsList.Remove(removed);
      BallRemovalHandler(removed);

      if (BallThreads.TryGetValue(removed, out var cts))
      {
        cts.Cancel();
        BallThreads.Remove(removed);
      }
    }

    public override SimulationParameters SimulationParameters => new SimulationParameters(FrameTime, TableSize);

    #endregion DataAbstractAPI

    #region IDisposable

    protected virtual void Dispose(bool disposing)
    {
      if (!Disposed)
      {
        if (disposing)
        {
          foreach (var cts in BallThreads.Values)
            cts.Cancel();

          BallThreads.Clear();

          BallsList.Clear();
        }
        
        Disposed = true;
      }
      else throw new ObjectDisposedException(nameof(DataImplementation));
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

    private Random RandomGenerator = new();
    private List<Ball> BallsList = [];
    private Action<IVector, IBall> BallCreationHandler;
    private Action<IBall> BallRemovalHandler;
    private int TableSize = 100;
    private int FrameTime = 0;
    private readonly Dictionary<Ball, CancellationTokenSource> BallThreads = new();
    
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

        internal void Start(int v, Action<object, object> value1, Action<IVector> value2)
        {
            throw new NotImplementedException();
        }

        #endregion TestingInfrastructure
    }
}