//____________________________________________________________________________________________________________________________________
//
//  Copyright (C) 2024, Mariusz Postol LODZ POLAND.
//
//  To be in touch join the community by pressing the `Watch` button and get started commenting using the discussion panel at
//
//  https://github.com/mpostol/TP/discussions/182
//
//_____________________________________________________________________________________________________________________________________

using TP.ConcurrentProgramming.Data;

namespace TP.ConcurrentProgramming.BusinessLogic
{
  internal class Ball : IBall
  {
    public Ball(Data.IBall ball)
    {
      ball.NewPositionNotification += RaisePositionChangeEvent;

      this.dataBall = ball;
      Position = new Position(0, 0);
    }

    #region IBall

    public event EventHandler<IPosition>? NewPositionNotification;

    #endregion IBall

    #region internal

    internal Data.IBall dataBall;
    internal IPosition Position;
    public double Radius => this.dataBall.Diameter / 2;
    public IVector Velocity
    {
      get => this.dataBall.Velocity;
      set => this.dataBall.Velocity = value;
    }

    #endregion internal

    #region private

    private void RaisePositionChangeEvent(object? sender, Data.IVector e)
    {
      BusinessVector delta = new BusinessVector(e.x - Position.x, e.y - Position.y);

      Position = new Position(e.x, e.y);

      var nextCollision = BusinessLogicAbstractAPI.GetBusinessLogicLayer().ComputeCollision(new BallMovement(this, delta));

      if (nextCollision != null)
      {
        dataBall.NotifyCollision(nextCollision);
      }

      NewPositionNotification?.Invoke(this, Position);
    }

    #endregion private
  }
}