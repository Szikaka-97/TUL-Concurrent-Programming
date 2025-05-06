//____________________________________________________________________________________________________________________________________
//
//  Copyright (C) 2024, Mariusz Postol LODZ POLAND.
//
//  To be in touch join the community by pressing the `Watch` button and get started commenting using the discussion panel at
//
//  https://github.com/mpostol/TP/discussions/182
//
//_____________________________________________________________________________________________________________________________________

namespace TP.ConcurrentProgramming.BusinessLogic.Test
{
  [TestClass]
  public class BallUnitTest
  {
        [TestMethod]
        public void MoveTestMethod()
        {
            DataBallFixture dataBallFixture = new DataBallFixture();
            Ball newInstance = new(dataBallFixture);

            newInstance.NewPositionNotification += (sender, position) =>
            {
                Assert.IsNotNull(sender);
                Assert.IsNotNull(position);
            };
        }


        #region testing instrumentation

        private class DataBallFixture : Data.IBall
    {
      public Data.IVector Velocity { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
      public double Diameter { get => throw new NotImplementedException(); init => throw new NotImplementedException(); }

      public event EventHandler<Data.IVector>? NewPositionNotification;

            public void NotifyCollision(Data.CollisionEvent collision)
            {
                throw new NotImplementedException();
            }

            internal void Move()
      {
        NewPositionNotification?.Invoke(this, new VectorFixture(0.0, 0.0));
      }
    }

    private class VectorFixture : Data.IVector
    {
      internal VectorFixture(double X, double Y)
      {
        x = X; y = Y;
      }

      public double x { get; init; }
      public double y { get; init; }

            public Data.IVector Add(Data.IVector other)
            {
                throw new NotImplementedException();
            }

            public Data.IVector Divide(float scale)
            {
                throw new NotImplementedException();
            }

            public Data.IVector Multiply(float scale)
            {
                throw new NotImplementedException();
            }

            public Data.IVector Subtract(Data.IVector other)
            {
                throw new NotImplementedException();
            }
        }

    #endregion testing instrumentation
  }
}