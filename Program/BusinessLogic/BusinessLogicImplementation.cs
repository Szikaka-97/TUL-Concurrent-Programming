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
using TP.ConcurrentProgramming.Data;
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

      layerBelow.Start(numberOfBalls, 20,
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

    internal override CollisionEvent? ComputeCollision(BallMovement movement)
    {
      lock (balls)
      {
        var ball = movement.ball;
        var currentVel = ball.Velocity;

        double timeToCollision = 0;
        IVector collisionNormal;
        Ball? collidingBall = null;

        var xBound = currentVel.x > 0 ? layerBelow.SimulationParameters.tableSize - ball.Radius : ball.Radius;
        var yBound = currentVel.y > 0 ? layerBelow.SimulationParameters.tableSize - ball.Radius : ball.Radius;

        var xNormal = currentVel.x > 0 ? -1 : 1;
        var yNormal = currentVel.y > 0 ? -1 : 1;

        timeToCollision = (xBound - ball.Position.x) / currentVel.x;
        collisionNormal = new BusinessVector(xNormal, 0);

        var tempTime = (yBound - ball.Position.y) / currentVel.y;
        if (tempTime < timeToCollision)
        {
          timeToCollision = tempTime;
          collisionNormal = new BusinessVector(0, yNormal);
        }

        foreach (var candidate in balls)
        {
          if (candidate == ball) continue;

          double r = ball.Radius + candidate.Radius;

          double
            ax = ball.Position.x,
            ay = ball.Position.y,
            bx = candidate.Position.x,
            by = candidate.Position.y,
            vax = currentVel.x,
            vay = currentVel.y,
            vbx = candidate.Velocity.x,
            vby = candidate.Velocity.y;

          // Yeah, make sense of that
          double
            a = vax * vax - 2 * vax * vbx + vbx * vbx + vay * vay - 2 * vay * vby + vby * vby,
            b = 2 * (ax * vax - bx * vax - ax * vbx + bx * vbx + ay * vay - by * vay - ay * vby + by * vby),
            c = ax * ax - 2 * ax * bx + bx * bx + ay * ay - 2 * ay * by + by * by - r * r;

          double d = Math.Sqrt(b * b - 4 * a * c);

          tempTime = (-b - d) / (2 * a);

          if (tempTime < 0)
          {
            tempTime = (-b + d) / (2 * a);
          }

          if (tempTime > 0 && tempTime < timeToCollision)
          {
            timeToCollision = tempTime;

            collidingBall = candidate;

            var nextPosA = new Position(ball.Position.x + ball.Velocity.x * timeToCollision, ball.Position.y + ball.Velocity.y * timeToCollision);
            var nextPosB = new Position(candidate.Position.x + candidate.Velocity.x * timeToCollision, candidate.Position.y + candidate.Velocity.y * timeToCollision);

            collisionNormal = new BusinessVector(nextPosA.x - nextPosB.x, nextPosA.y - nextPosB.y).Normalize();
          }
        }

        if (timeToCollision < 1)
        {
          return new CollisionEvent((int) Math.Ceiling(timeToCollision * layerBelow.SimulationParameters.frameTime), collisionNormal, collidingBall?.dataBall);
        }
        else
        {
          return null;
        }
      }
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