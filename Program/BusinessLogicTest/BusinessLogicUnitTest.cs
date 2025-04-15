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
      DataLayerStartFixcure dataLayerFixcure = new();
      using (BusinessLogicImplementation newInstance = new(dataLayerFixcure))
      {
        int called = 0;
        int numberOfBalls2Create = 10;
        newInstance.Start(
          numberOfBalls2Create,
          (startingPosition, ball) => { called++; Assert.IsNotNull(startingPosition); Assert.IsNotNull(ball); },
          ball => { }
        );
        Assert.AreEqual<int>(1, called);
        Assert.IsTrue(dataLayerFixcure.StartCalled);
        Assert.AreEqual<int>(numberOfBalls2Create, dataLayerFixcure.NumberOfBallseCreated);
      }
    }

    #region testing instrumentation

    private class DataLayerConstructorFixture : Data.DataAbstractAPI
    {
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

      public override void Start(int numberOfBalls, Action<IVector, Data.IBall> ballCreationHandler, Action<Data.IBall> ballRemovalHandler)
      {
        throw new NotImplementedException();
      }
    }

    private class DataLayerDisposeFixture : Data.DataAbstractAPI
    {
      internal bool Disposed = false;

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

      public override void Start(int numberOfBalls, Action<IVector, Data.IBall> ballCreationHandler, Action<Data.IBall> ballRemovalHandler)
      {
        throw new NotImplementedException();
      }
    }

    private class DataLayerStartFixcure : Data.DataAbstractAPI
    {
      internal bool StartCalled = false;
      internal int NumberOfBallseCreated = -1;

      public override void Dispose()
      { }

      public override void Start(int numberOfBalls, Action<IVector, Data.IBall> ballCreationHandler, Action<Data.IBall> ballRemovalHandler)
      {
        StartCalled = true;
        NumberOfBallseCreated = numberOfBalls;
        ballCreationHandler(new DataVectorFixture(), new DataBallFixture());
      }

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

      private record DataVectorFixture : Data.IVector
      {
        public double x { get; init; }
        public double y { get; init; }
      }

      private class DataBallFixture : Data.IBall
      {
        public IVector Velocity { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public double Diameter { get => throw new NotImplementedException(); init => throw new NotImplementedException(); }

        public event EventHandler<IVector>? NewPositionNotification = null;
      }
    }

    #endregion testing instrumentation
  }
}