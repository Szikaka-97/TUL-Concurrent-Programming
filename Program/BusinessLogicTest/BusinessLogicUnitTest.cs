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

namespace TP.ConcurrentProgramming.BusinessLogic.Test
{
  [TestClass]
  public class BusinessLogicImplementationUnitTest
  {
    [TestMethod]
    public void ConstructorTestMethod()
    {
      using (BusinessLogicImplementation newInstance = new(new DataLayerConstructorFixture()))
      {
        bool newInstanceDisposed = true;
        newInstance.CheckObjectDisposed(x => newInstanceDisposed = x);
        Assert.IsFalse(newInstanceDisposed);
      }
    }

    [TestMethod]
    public void DisposeTestMethod()
    {
      DataLayerDisposeFixture dataLayerFixcure = new DataLayerDisposeFixture();
      BusinessLogicImplementation newInstance = new(dataLayerFixcure);
      Assert.IsFalse(dataLayerFixcure.Disposed);
      bool newInstanceDisposed = true;
      newInstance.CheckObjectDisposed(x => newInstanceDisposed = x);
      Assert.IsFalse(newInstanceDisposed);
      newInstance.Dispose();
      newInstance.CheckObjectDisposed(x => newInstanceDisposed = x);
      Assert.IsTrue(newInstanceDisposed);
      Assert.ThrowsException<ObjectDisposedException>(() => newInstance.Dispose());
      Assert.ThrowsException<ObjectDisposedException>(() => newInstance.Start(0, (position, ball) => { }, ball => { }));
      Assert.IsTrue(dataLayerFixcure.Disposed);
    }

     [TestMethod]
    public void StartTestMethod()
    {
   
        int createdBallCount = 0;
        int removedBallCount = 0;
        int numberOfBalls = 5;

        BusinessLogicImplementation logic = new();

            logic.Start(
            numberOfBalls,
            (position, ball) =>
            {
                Assert.IsNotNull(position);
                Assert.IsNotNull(ball);
                createdBallCount++;
            },
            ball =>
            {
                removedBallCount++;
            }
        );

        Assert.AreEqual(numberOfBalls, createdBallCount);
        Assert.AreEqual(0, removedBallCount); 
    }




        #region testing instrumentation

        private class DataLayerConstructorFixture : Data.DataAbstractAPI
    {
            public override SimulationParameters SimulationParameters => throw new NotImplementedException();

            public override void AddBall()
      {
        throw new NotImplementedException();
      }

      public override void Dispose()
      { }

      public override void EndSimulation()
      {
        throw new NotImplementedException();
      }

      public override void RemoveBall()
      {
        throw new NotImplementedException();
      }



            public override void Start(int numberOfBalls, int frameTime, Action<IVector, Data.IBall> ballCreationHandler, Action<Data.IBall> ballRemovalHandler)
            {
                throw new NotImplementedException();
            }
        }

    private class DataLayerDisposeFixture : Data.DataAbstractAPI
    {
      internal bool Disposed = false;

            public override SimulationParameters SimulationParameters => throw new NotImplementedException();

            public override void AddBall()
      {
        throw new NotImplementedException();
      }

      public override void Dispose()
      {
        Disposed = true;
      }

      public override void EndSimulation()
      {
        throw new NotImplementedException();
      }

      public override void RemoveBall()
      {
        throw new NotImplementedException();
      }

            public override void Start(int numberOfBalls, int frameTime, Action<IVector, Data.IBall> ballCreationHandler, Action<Data.IBall> ballRemovalHandler)
            {
                throw new NotImplementedException();
            }
        }

    private class DataLayerStartFixcure : Data.DataAbstractAPI
    {
      internal bool StartCalled = false;
      internal int NumberOfBallseCreated = -1;

            public override SimulationParameters SimulationParameters => throw new NotImplementedException();

            public override void Dispose()
      { }



      public override void AddBall()
      {
        throw new NotImplementedException();
      }

      public override void RemoveBall()
      {
        throw new NotImplementedException();
      }

      public override void EndSimulation()
      {
        throw new NotImplementedException();
      }

            public override void Start(int numberOfBalls, int frameTime, Action<IVector, Data.IBall> ballCreationHandler, Action<Data.IBall> ballRemovalHandler)
            {
                throw new NotImplementedException();
            }

            private record DataVectorFixture : Data.IVector
      {
        public double x { get; init; }
        public double y { get; init; }

                public IVector Add(IVector other)
                {
                    throw new NotImplementedException();
                }

                public IVector Divide(float scale)
                {
                    throw new NotImplementedException();
                }

                public IVector Multiply(float scale)
                {
                    throw new NotImplementedException();
                }

                public IVector Subtract(IVector other)
                {
                    throw new NotImplementedException();
                }
            }

      private class DataBallFixture : Data.IBall
      {
        public IVector Velocity { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public double Diameter { get => throw new NotImplementedException(); init => throw new NotImplementedException(); }

        public event EventHandler<IVector>? NewPositionNotification = null;

                public void NotifyCollision(CollisionEvent collision)
                {
                    throw new NotImplementedException();
                }
            }
    }

    #endregion testing instrumentation
  }
}