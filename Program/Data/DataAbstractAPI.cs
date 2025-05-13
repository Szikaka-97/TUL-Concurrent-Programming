//____________________________________________________________________________________________________________________________________
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
  public abstract class DataAbstractAPI : IDisposable
  {
    #region Layer Factory

    public static DataAbstractAPI GetDataLayer()
    {
      return modelInstance.Value;
    }

    #endregion Layer Factory

    #region public API

    public abstract void Start(int numberOfBalls, int frameTime, Action<IVector, IBall> ballCreationHandler, Action<IBall> ballRemovalHandler);

    public abstract void EndSimulation();
    public abstract void AddBall();
    public abstract void RemoveBall();

    public abstract SimulationParameters SimulationParameters { get; }

    #endregion public API

    #region IDisposable

    public abstract void Dispose();

    #endregion IDisposable

    #region private

    private static Lazy<DataAbstractAPI> modelInstance = new Lazy<DataAbstractAPI>(() => new DataImplementation());

    #endregion private
  }

  public interface IVector
  {
    /// <summary>
    /// The X component of the vector.
    /// </summary>
    double x { get; init; }

    /// <summary>
    /// The y component of the vector.
    /// </summary>
    double y { get; init; }

    IVector Add(IVector other);
    IVector Subtract(IVector other);
    IVector Multiply(float scale);
    IVector Divide(float scale);

    public static IVector operator +(IVector a, IVector b) => a.Add(b);
    public static IVector operator -(IVector a, IVector b) => a.Subtract(b);
    public static IVector operator *(IVector vec, float scale) => vec.Multiply(scale);
    public static IVector operator /(IVector vec, float scale) => vec.Divide(scale);

    public static double Dot(IVector a, IVector b)
    {
      return a.x * b.x + a.y * b.y;
    }

    public static IVector Reflect(IVector d, IVector n)
    {
      return d - (n * 2 * (float) Dot(d, n));
    }

    public double Length()
    {
      return Math.Sqrt(this.x * this.x + this.y * this.y);
    }

    public IVector Normalize()
    {
      return this.Divide((float)Length());
    }
  }

  public interface IBall
  {
    event EventHandler<IVector> NewPositionNotification;

    IVector Velocity { get; set; }

    double Diameter { get; init; }
  }

  public record SimulationParameters(int frameTime, float tableSize);

  public record CollisionEvent(int time, IVector normal, IBall? otherBall);
}