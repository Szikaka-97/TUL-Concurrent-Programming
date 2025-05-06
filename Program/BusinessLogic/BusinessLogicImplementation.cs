//____________________________________________________________________________________________________________________________________
//
//  Copyright (C) 2024, Mariusz Postol LODZ POLAND.
//
//  To be in touch join the community by pressing the `Watch` button and get started commenting using the discussion panel at
//
//  https://github.com/mpostol/TP/discussions/182
//
//_____________________________________________________________________________________________________________________________________

using System.Collections.Concurrent;
using System.Diagnostics;
using UnderneathLayerAPI = TP.ConcurrentProgramming.Data.DataAbstractAPI;



namespace TP.ConcurrentProgramming.BusinessLogic
{
  internal class BusinessLogicImplementation : BusinessLogicAbstractAPI
  {
    #region ctor

    public BusinessLogicImplementation() : this(null)
    { }

    internal BusinessLogicImplementation(UnderneathLayerAPI? underneathLayer)
    {
      layerBelow = underneathLayer == null ? UnderneathLayerAPI.GetDataLayer() : underneathLayer;
    }

    #endregion ctor

    #region BusinessLogicAbstractAPI

    public override void Dispose()
    {
      if (Disposed)
        throw new ObjectDisposedException(nameof(BusinessLogicImplementation));
      layerBelow.Dispose();
      Disposed = true;
    }

    public override void Start(int numberOfBalls, Action<IPosition, IBall> upperLayerBallCreationHandler, Action<IBall> upperLayerBallRemovalHandler)
    {
      if (Disposed)
        throw new ObjectDisposedException(nameof(BusinessLogicImplementation));
      if (upperLayerBallCreationHandler == null)
        throw new ArgumentNullException(nameof(upperLayerBallCreationHandler));

      ballCreationHandler = upperLayerBallCreationHandler;

      layerBelow.Start(numberOfBalls,
        (startingPosition, databall) =>
        {
          Ball newBall = new Ball(databall);
          lock (ballListLock)
          {
            balls.Add(newBall);
          }
          upperLayerBallCreationHandler(new Position(startingPosition.x, startingPosition.y), newBall);
        },
        databall =>
        {
          Ball toRemove;
          lock (ballListLock)
          {
            toRemove = balls.FirstOrDefault(b => b.Equals(databall))!;
            balls.Remove(toRemove);
          }
          upperLayerBallRemovalHandler(toRemove);
        });
    }

    public override void Stop()
    {
      layerBelow.EndSimulation();
    }
    public override void AddBall()
    {
      layerBelow.AddBall();
    }
    public override void RemoveBall()
    {
      layerBelow.RemoveBall();
    }

    internal override Data.IVector ComputeCollision(BallMovement movement)
    {
      var ball = movement.movingBall;
      var currentVel = ball.Velocity;

      if (ball.Position.x + ball.Radius >= 100 || ball.Position.x - ball.Radius <= 0)
      {
        currentVel = new BusinessVector(-currentVel.x, currentVel.y);
      }
      if (ball.Position.y + ball.Radius >= 100 || ball.Position.y - ball.Radius <= 0)
      {
        currentVel = new BusinessVector(currentVel.x, -currentVel.y);
      }

      lock (ballListLock)
      {
        foreach (var other in balls)
        {
          if (other == ball) continue;

          double dx = ball.Position.x - other.Position.x;
          double dy = ball.Position.y - other.Position.y;
          double dist = Math.Sqrt(dx * dx + dy * dy);
          double minDist = ball.Radius + other.Radius;

          if (dist < minDist && dist > 0)
          {
            currentVel = new BusinessVector(-currentVel.x, -currentVel.y);
            other.Velocity = new BusinessVector(-other.Velocity.x, -other.Velocity.y);

            break; 
          }
        }
      }

      return currentVel;
    }


    internal void QueueMovement(BallMovement movement)
    {

    }

    #endregion BusinessLogicAbstractAPI

    #region private

    private bool Disposed = false;

    private readonly UnderneathLayerAPI layerBelow;

    private Action<IPosition, IBall> ballCreationHandler;

    private ConcurrentQueue<BallMovement> queuedMovements;

    private readonly List<Ball> balls = new List<Ball>();
    private readonly object ballListLock = new object(); 


    #endregion private

    #region TestingInfrastructure

    [Conditional("DEBUG")]
    internal void CheckObjectDisposed(Action<bool> returnInstanceDisposed)
    {
      returnInstanceDisposed(Disposed);
    }

    #endregion TestingInfrastructure
  }
}