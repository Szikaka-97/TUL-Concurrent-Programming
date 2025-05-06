using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using TP.ConcurrentProgramming.Data;

namespace TP.ConcurrentProgramming.BusinessLogic
{
  class BusinessVector : IVector
  {
    #region IVector

    /// <summary>
    /// The X component of the vector.
    /// </summary>
    public double x { get; init; }
    /// <summary>
    /// The Y component of the vector.
    /// </summary>
    public double y { get; init; }

    #endregion IVector

    /// <summary>
    /// Creates new instance of <seealso cref="BusinessVector"/> and initialize all properties
    /// </summary>
    public BusinessVector(double XComponent, double YComponent)
    {
      x = XComponent;
      y = YComponent;
    }

    public IVector Add(IVector other)
    {
      return new BusinessVector(this.x + other.x, this.y + other.y);
    }

    public IVector Subtract(IVector other)
    {
      return new BusinessVector(this.x - other.x, this.y - other.y);
    }

    public IVector Multiply(float scale)
    {
      return new BusinessVector(this.x * scale, this.y * scale);
    }

    public IVector Divide(float scale)
    {
      return new BusinessVector(this.x / scale, this.y / scale);
    }
    double Length()
    {
      return Math.Sqrt(this.x * this.x + this.y * this.y);
    }

    public IVector Normalize()
    {
      return this.Divide((float)Length());
    }
  }
}
