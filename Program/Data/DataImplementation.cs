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
using System.Collections.Concurrent;
using System.IO;
using System.Diagnostics;
using System.Threading;

namespace TP.ConcurrentProgramming.Data
{
  internal class DataImplementation : DataAbstractAPI
  {
    #region ctor

    public DataImplementation()
    {

    }
    public void StartOfLogging(string file = "Ball_stats.csv")
    {
      Console.WriteLine($"[DEBUG] Log file path: {Path.GetFullPath(_logPath)}");
      _logPath = file;
      _loggerRunning = true;
      _loggerTread = new Thread(LogWorker);
      _loggerTread.Start();
    }
    public void StopOfLogging()
    {
      _loggerRunning = false;
      if (_loggerTread != null)
      { 
        _loggerTread.Join();
      }
    }
    public void LogBallState(int ballId, Ball ball)
    {
      _logQueue.Enqueue(new BallLogData
      {
        BallId = ballId,
        Position = ball.Position,
        Velocity = ball.Velocity,
        Timestamp = DateTime.Now
      });
    }
    public void LogWorker()
    {
      using (var writer = new StreamWriter(_logPath))
      {
        while (true)
        {
          while (_logQueue.TryDequeue(out var logData))
          {
            writer.WriteLine($"{logData.BallId},{logData.Position.x},{logData.Position.y},{logData.Velocity.x},{logData.Velocity.y},{logData.Timestamp:O}");
          }

          if (!_loggerRunning)
            return;

          Thread.Sleep(70);
        }
      }
    }
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

      StartOfLogging();
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

      StopOfLogging();
    }

    public override void AddBall()
    {
      if (Disposed)
        throw new ObjectDisposedException(nameof(DataImplementation));

      Vector startingPosition = new(RandomGenerator.Next(10, TableSize - 10), RandomGenerator.Next(10, TableSize - 10));
      double bearing = RandomGenerator.NextDouble();
      Vector vel = new Vector(Math.Sin(bearing) * 10, Math.Cos(bearing) * 10);

      Ball newBall = new(startingPosition, vel);
      BallsList.Add(newBall);

      BallCreationHandler(newBall.Position, newBall);

      var cts = new CancellationTokenSource();
      BallThreads[newBall] = cts;

      int ballID = BallsList.Count;

      new Thread(() =>
      {
        while (!cts.Token.IsCancellationRequested)
        {
          newBall.Move((float) FrameTime / 1000);

          LogBallState(
              ballID,
              newBall
          );

          Thread.Sleep(FrameTime);
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
    private readonly ConcurrentQueue<BallLogData> _logQueue = new();
    private Thread _loggerTread;
    private bool _loggerRunning;
    private string _logPath = "Ball_stats.csv";
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