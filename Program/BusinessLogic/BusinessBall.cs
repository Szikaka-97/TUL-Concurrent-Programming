//____________________________________________________________________________________________________________________________________
//
//  Copyright (C) 2024, Mariusz Postol LODZ POLAND.
//
//  To be in touch join the community by pressing the `Watch` button and get started commenting using the discussion panel at
//
//  https://github.com/mpostol/TP/discussions/182
//
//_____________________________________________________________________________________________________________________________________

namespace TP.ConcurrentProgramming.BusinessLogic
{
  internal class Ball : IBall
  {
    public Ball(Data.IBall ball)
    {
      ball.NewPositionNotification += RaisePositionChangeEvent;

      this.dataBall = ball;
      this.position = new Position(0, 0);
    }

    #region IBall

    public event EventHandler<IPosition>? NewPositionNotification;

    #endregion IBall

    #region private

    private Data.IBall dataBall;

    private IPosition position;

    private double Radius => this.dataBall.Diameter / 2;

    private void RaisePositionChangeEvent(object? sender, Data.IVector e)
    {
      this.position = new Position(e.x, e.y);

      if (this.position.x + Radius >= 100 || this.position.x - Radius <= 0)
      {
        this.dataBall.Velocity = new BusinessVector(-this.dataBall.Velocity.x, this.dataBall.Velocity.y);
      }
      if (this.position.y + Radius >= 100 || this.position.y - Radius <= 0)
      {
        this.dataBall.Velocity = new BusinessVector(this.dataBall.Velocity.x, -this.dataBall.Velocity.y);
      }

      NewPositionNotification?.Invoke(this, this.position);
    }

    #endregion private
  }
}