﻿//____________________________________________________________________________________________________________________________________
//
//  Copyright (C) 2024, Mariusz Postol LODZ POLAND.
//
//  To be in touch join the community by pressing the `Watch` button and get started commenting using the discussion panel at
//
//  https://github.com/mpostol/TP/discussions/182
//
//_____________________________________________________________________________________________________________________________________

namespace TP.ConcurrentProgramming.Data
{
  internal class Ball : IBall
  {
    #region ctor

    internal Ball(Vector initialPosition, Vector initialVelocity, double initialDiameter = 10)
    {
      PositionBackingField = initialPosition;
      Velocity = initialVelocity;
      DiameterBackingField = initialDiameter;
    }

    #endregion ctor

    #region IBall

    public event EventHandler<IVector>? NewPositionNotification;

    public IVector Velocity { get; set; }

    public double Diameter
    {
      get => DiameterBackingField;
      init => DiameterBackingField = value;
    }

    public double Radius
    {
      get => DiameterBackingField / 2;
      init => DiameterBackingField = value * 2;
    }

    #endregion IBall

    #region internal

    internal Vector Position
    {
      get => PositionBackingField;
    }

    #endregion internal

    #region private

    private Vector PositionBackingField;
    private double DiameterBackingField;

    private void RaiseNewPositionChangeNotification()
    {
      NewPositionNotification?.Invoke(this, PositionBackingField);
    }

    internal void Move(Vector delta)
    {
      PositionBackingField = PositionBackingField + delta;

      RaiseNewPositionChangeNotification();
    }

    #endregion private
  }
}