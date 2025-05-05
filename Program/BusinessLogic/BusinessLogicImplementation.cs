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

      layerBelow.Start(numberOfBalls, (startingPosition, databall) => upperLayerBallCreationHandler(new Position(startingPosition.x, startingPosition.x), new Ball(databall)), databall => upperLayerBallRemovalHandler(new Ball(databall)));
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
      var currentVel = movement.movingBall.Velocity;

      if (ball.Position.x + ball.Radius >= 100 || ball.Position.x - ball.Radius <= 0)
      {
        currentVel = new BusinessVector(-currentVel.x, currentVel.y);
      }
      if (ball.Position.y + ball.Radius >= 100 || ball.Position.y - ball.Radius <= 0)
      {
        currentVel = new BusinessVector(currentVel.x, -currentVel.y);
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