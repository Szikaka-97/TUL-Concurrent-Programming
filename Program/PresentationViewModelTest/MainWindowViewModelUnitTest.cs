//__________________________________________________________________________________________
//
//  Copyright 2024 Mariusz Postol LODZ POLAND.
//
//  To be in touch join the community by pressing the `Watch` button and to get started
//  comment using the discussion panel at
//  https://github.com/mpostol/TP/discussions/182
//__________________________________________________________________________________________

using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Reactive;
using System.Reactive.Linq;
using TP.ConcurrentProgramming.Presentation.Model;
using ModelIBall = TP.ConcurrentProgramming.Presentation.Model.IBall;

namespace TP.ConcurrentProgramming.Presentation.ViewModel.Test
{
  [TestClass]
  public class MainWindowViewModelUnitTest
  {
    [TestMethod]
    public void ConstructorTest()
    {
      ModelNullFixture nullModelFixture = new();
      Assert.AreEqual<int>(0, nullModelFixture.Disposed);
      Assert.AreEqual<int>(0, nullModelFixture.Started);
      Assert.AreEqual<int>(0, nullModelFixture.Subscribed);
      using (MainWindowViewModel viewModel = new(nullModelFixture))
      {
        Random random = new Random();
        int numberOfBalls = random.Next(1, 10);
        viewModel.Start(numberOfBalls);
        Assert.IsNotNull(viewModel.Balls);
        Assert.AreEqual<int>(0, nullModelFixture.Disposed);
        Assert.AreEqual<int>(numberOfBalls, nullModelFixture.Started);
        Assert.AreEqual<int>(1, nullModelFixture.Subscribed);
      }
      Assert.AreEqual<int>(1, nullModelFixture.Disposed);
    }

    [TestMethod]
    public void BehaviorTestMethod()
    {
      ModelSimulatorFixture modelSimulator = new();
      MainWindowViewModel viewModel = new(modelSimulator);
      Assert.IsNotNull(viewModel.Balls);
      Assert.AreEqual<int>(0, viewModel.Balls.Count);
      Random random = new Random();
      int numberOfBalls = random.Next(1, 10);
      viewModel.Start(numberOfBalls);
      Assert.AreEqual<int>(numberOfBalls * 2, viewModel.Balls.Count);
      viewModel.Dispose();
      Assert.IsTrue(modelSimulator.Disposed);
      Assert.AreEqual<int>(0, viewModel.Balls.Count);
    }

        #region testing infrastructure

        private class ModelNullFixture : ModelAbstractApi
        {
            internal int Disposed = 0;
            internal int Started = 0;
            internal int Subscribed = 0;

            private ObservableCollection<IBall> fakeBalls = new();
            public override ObservableCollection<IBall> Balls => fakeBalls;

            private int ballCount = 0;
            public override int CurrentBallsCount
            {
                get => ballCount;
                set => ballCount = value;
            }

            public override bool Running => false;

            private double _scale = 1.0;
            public override double Scale
            {
                get => _scale;
                set => _scale = value;
            }

            public override double TableSize
            {
                get => 100.0;
                set { /* mo�na doda� logik� je�li potrzeba */ }
            }

            public override void Dispose() => Disposed++;

            public override void Start(int numberOfBalls) => Started = numberOfBalls;

            public override void Stop() { }

            public override IDisposable Subscribe(IObserver<IBall> observer)
            {
                Subscribed++;
                return new NullDisposable();
            }

            private class NullDisposable : IDisposable
            {
                public void Dispose() { }
            }
        }




        private class ModelSimulatorFixture : ModelAbstractApi
        {
            internal bool Disposed = false;

            private ObservableCollection<IBall> balls = new();
            public override ObservableCollection<IBall> Balls => balls;

            private int ballCount = 0;
            public override int CurrentBallsCount
            {
                get => ballCount;
                set => ballCount = value;
            }

            public override bool Running => true;

            private double _scale = 1.0;
            public override double Scale
            {
                get => _scale;
                set => _scale = value;
            }

            public override double TableSize
            {
                get => 200.0;
                set { /* opcjonalna logika */ }
            }

            public override void Start(int numberOfBalls)
            {
                balls.Clear();
                for (int i = 0; i < numberOfBalls; i++)
                {
                    var newBall = new ModelBall(0, 0);
                    balls.Add(newBall);
                    BallChanged?.Invoke(this, new BallChangeEventArgs { Ball = newBall });
                }
                ballCount = numberOfBalls;
            }

            public override void Stop()
            {
                balls.Clear();
                ballCount = 0;
            }

            public override void Dispose()
            {
                Disposed = true;
                Stop();
            }

            public override IDisposable? Subscribe(IObserver<IBall> observer)
            {
                eventObservable ??= Observable.FromEventPattern<BallChangeEventArgs>(
                    h => BallChanged += h,
                    h => BallChanged -= h
                );
                return eventObservable.Subscribe(
                    x => observer.OnNext(x.EventArgs.Ball),
                    ex => observer.OnError(ex),
                    () => observer.OnCompleted()
                );
            }

            public event EventHandler<BallChangeEventArgs>? BallChanged;
            private IObservable<EventPattern<BallChangeEventArgs>>? eventObservable;

            private class ModelBall : IBall
            {
                public ModelBall(double top, double left)
                {
                    Top = top;
                    Left = left;
                }

                public double Top { get; private set; }
                public double Left { get; private set; }
                public double Diameter => 10.0;
                public double Scale { get; set; } = 1.0;

                public void UpdateScale()
                {
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Scale)));
                }

                public event PropertyChangedEventHandler? PropertyChanged;
            }

            public class BallChangeEventArgs : EventArgs
            {
                public IBall Ball { get; set; } = default!;
            }
        }



        #endregion testing infrastructure
    }
}