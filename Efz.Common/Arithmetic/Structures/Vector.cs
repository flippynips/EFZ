using System;

namespace Efz.Maths {
  
  // ----------------------------------------------------------------------------- Vector2 //
  
  /// <summary>
  /// Two points.
  /// </summary>
  public struct Vector2 : IEquatable<Vector2>, IComparable<Vector2> {
    
    /// <summary>
    /// Origin (0, 0).
    /// </summary>
    public static Vector2 Zero {
      get { return new Vector2(0, 0); }
    }
    /// <summary>
    /// A single unit vector.
    /// </summary>
    public static Vector2 One {
      get { return new Vector2(1.0, 1.0); }
    }
    /// <summary>
    /// Get a random unit vector.
    /// </summary>
    public static Vector2 Random {
      get { return new Vector2(Randomize.Double, Randomize.Double); }
    }
    /// <summary>
    /// Get a random unit vector between two vectors.
    /// </summary>
    public static Vector2 Range(Vector2 vectorA, Vector2 vectorB) {
      return new Vector2(Randomize.Range(vectorA.X, vectorB.X), Randomize.Range(vectorA.Y, vectorB.Y));
    }
    
    /// <summary>
    /// Magnitude of the vector squared.
    /// </summary>
    public double MagnitudeSquare {
      get {
        return X * X + Y * Y;
      }
    }
    /// <summary>
    /// Magnitude of the vector.
    /// </summary>
    public double Magnitude {
      get {
        return Math.Sqrt(X * X + Y * Y);
      }
    }
    /// <summary>
    /// The two dimensional angle of the vector.
    /// </summary>
    public double Angle {
      get {
        return Math.Acos(X/Y);
      }
    }
    /// <summary>
    /// The x dimension of the vector.
    /// </summary>
    public double X;
    /// <summary>
    /// The y dimension of the vector.
    /// </summary>
    public double Y;
    
    /// <summary>
    /// Construct a new vector with the supplied values.
    /// </summary>
    public Vector2(double x, double y) {
      X = x;
      Y = y;
    }
    
    /// <summary>
    /// Normalize the vector to a single unit.
    /// </summary>
    public Vector2 Normalize() {
      double invMagnitude = 1.0 / Math.Sqrt(X * X + Y * Y);
      return new Vector2(X * invMagnitude, Y * invMagnitude);
    }
    /// <summary>
    /// Retrieve the normalized vector to this one.
    /// </summary>
    public Vector2 Normal() {
      double invMagnitude = 1.0 / Math.Sqrt(X * X + Y * Y);
      return new Vector2(-Y * invMagnitude, X * invMagnitude);
    }
    /// <summary>
    /// The inverse vector to this one. Simply reverses the x and y dimensions.
    /// </summary>
    public Vector2 Inverse() {
      return new Vector2(Y, X);
    }
    
    /// <summary>
    /// Normalizes the vector if the unit length is greater than 1.
    /// </summary>
    public Vector2 Maximize() {
      double invMagnitude = Math.Sqrt(X * X + Y * Y);
      if(invMagnitude > 1) {
        invMagnitude = 1 / invMagnitude;
        return new Vector2(X * invMagnitude, Y * invMagnitude);
      }
      return new Vector2(X, Y);
    }
    
    /// <summary>
    /// Rotate the vector by the supplied radians.
    /// </summary>
    public Vector2 Rotate(double radians) {
      double c = Math.Cos(radians);
      double s = Math.Sin(radians);
      return new Vector2(X * c - Y * s, X * s + Y * c);
    }
    
    public override bool Equals(object obj) {
      return (obj is Vector2) && Equals((Vector2)obj);
    }
    
    public bool Equals(Vector2 other) {
      return Math.Abs(X - other.X) < Meth.Epsilon && Math.Abs(Y - other.Y) < Meth.Epsilon;
    }
    
    public int CompareTo(Vector2 other) {
      return (int)(MagnitudeSquare - other.MagnitudeSquare);
    }
    
    public override int GetHashCode() {
      return X.GetHashCode() ^ Y.GetHashCode();
    }
    
    public override string ToString() {
      return Chars.BraceOpen + X.ToString(Meth.FormatDouble) + Chars.Comma + Y.ToString(Meth.FormatDouble) + Chars.BraceClose;
    }
    
    /// <summary>
    /// Get the scalar distance between this vector and the supplied vector.
    /// </summary>
    public double Distance(Vector2 vector) {
      return Math.Sqrt(Meth.Square(vector.X - X) + Meth.Square(vector.Y - Y));
    }
    /// <summary>
    /// Get the scalar distance between vectors squared.
    /// </summary>
    public double DistanceSquare(Vector2 vector) {
      return Meth.Square(vector.X - X) + Meth.Square(vector.Y - Y);
    }
    
    public bool Equals(Vector4 other) {
      return Math.Abs(X - other.X) < Meth.Epsilon &&
             Math.Abs(Y - other.Y) < Meth.Epsilon;
    }
    
    public static bool operator ==(Vector2 vectorA, Vector2 vectorB) {
      return Math.Abs(vectorA.X - vectorB.X) < Meth.Epsilon &&
             Math.Abs(vectorA.Y - vectorB.Y) < Meth.Epsilon;
    }
    
    public static bool operator !=(Vector2 vectorA, Vector2 vectorB) {
      return Math.Abs(vectorA.X - vectorB.X) > Meth.Epsilon ||
             Math.Abs(vectorA.Y - vectorB.Y) > Meth.Epsilon;
    }
    
    public static Vector2 operator +(Vector2 vectorA, Vector2 vectorB) {
      return new Vector2(vectorA.X + vectorB.X, vectorA.Y + vectorB.Y);
    }

    public static Vector2 operator -(Vector2 vectorA, Vector2 vectorB) {
      return new Vector2(vectorA.X - vectorB.X, vectorA.Y - vectorB.Y);
    }
    
    public static Vector2 operator *(Vector2 vectorA, Vector2 vectorB) {
      return new Vector2(vectorA.X * vectorB.X, vectorA.Y * vectorB.Y);
    }
    
    public static Vector2 operator /(Vector2 vectorA, Vector2 vectorB) {
      return new Vector2(vectorA.X / vectorB.X, vectorA.Y / vectorB.Y);
    }
    
    public static Vector2 operator *(Vector2 vector, double multiple) {
      return new Vector2(vector.X * multiple, vector.Y * multiple);
    }
    
    public static Vector2 operator *(double multiple, Vector2 vector) {
      return new Vector2(vector.X * multiple, vector.Y * multiple);
    }
    
    public static Vector2 operator /(Vector2 vector, double multiple) {
      return new Vector2(vector.X / multiple, vector.Y / multiple);
    }
    
    public static Vector2 operator /(Vector2 vector, int multiple) {
      return new Vector2(vector.X / multiple, vector.Y / multiple);
    }
    
    public static Vector2 operator %(Vector2 vectorA, Vector2 vectorB) {
      return new Vector2(vectorA.X % vectorB.X, vectorA.Y % vectorB.Y);
    }
    
    public static Vector2 operator %(Vector2 vectorA, int divisor) {
      return new Vector2(vectorA.X % divisor, vectorA.Y % divisor);
    }
    
    public static Vector2 operator -(Vector2 vector) {
      return new Vector2(-vector.X, -vector.Y);
    }
    
    public static bool operator <(Vector2 vectorA, Vector2 vectorB) {
      return vectorA.MagnitudeSquare < vectorB.MagnitudeSquare;
    }
    
    public static bool operator <=(Vector2 vectorA, Vector2 vectorB) {
      return vectorA.MagnitudeSquare <= vectorB.MagnitudeSquare;
    }
    
    public static bool operator >(Vector2 vectorA, Vector2 vectorB) {
      return vectorA.MagnitudeSquare > vectorB.MagnitudeSquare;
    }
    
    public static bool operator >=(Vector2 vectorA, Vector2 vectorB) {
      return vectorA.MagnitudeSquare >= vectorB.MagnitudeSquare;
    }
    
  }
  
  // ----------------------------------------------------------------------------- Vector3 //
  
  public struct Vector3 : IEquatable<Vector3>, IComparable<Vector3> {
    
    /// <summary>
    /// Origin (0, 0).
    /// </summary>
    public static Vector3 Zero {
      get { return new Vector3(0, 0, 0); }
    }
    /// <summary>
    /// A single unit vector.
    /// </summary>
    public static Vector3 One {
      get { return new Vector3(1.0, 1.0, 1.0); }
    }
    /// <summary>
    /// Get a random unit vector.
    /// </summary>
    public static Vector3 Random {
      get { return new Vector3(Randomize.Double, Randomize.Double, Randomize.Double); }
    }
    /// <summary>
    /// Get a random unit vector between two vectors.
    /// </summary>
    public static Vector3 Range(Vector3 vectorA, Vector3 vectorB) {
      return new Vector3(Randomize.Range(vectorA.X, vectorB.X), Randomize.Range(vectorA.Y, vectorB.Y), Randomize.Range(vectorA.Z, vectorB.Z));
    }
    
    public double MagnitudeSquare {
      get {
        return X * X + Y * Y + Z * Z;
      }
    }

    public double Magnitude {
      get {
        return Math.Sqrt(X * X + Y * Y + Z * Z);
      }
    }

    public double X;
    public double Y;
    public double Z;

    public Vector3(double x, double y, double z = 0.0) {
      X = x;
      Y = y;
      Z = z;
    }
    
    public override string ToString() {
      return Chars.BraceOpen +
        X.ToString(Meth.FormatDouble) + Chars.Comma +
        Y.ToString(Meth.FormatDouble) + Chars.Comma +
        Z.ToString(Meth.FormatDouble) +
        Chars.BraceClose;
    }
    
    public override bool Equals(object obj) {
      return (obj is Vector3) && Equals((Vector3)obj);
    }
    
    public bool Equals(Vector3 vector) {
      return Math.Abs(X - vector.X) < Meth.Epsilon &&
             Math.Abs(Y - vector.Y) < Meth.Epsilon &&
             Math.Abs(Z - vector.Z) < Meth.Epsilon;
    }
    
    public int CompareTo(Vector3 other) {
      return (int)(MagnitudeSquare - other.MagnitudeSquare);
    }
    
    public override int GetHashCode() {
      return X.GetHashCode() ^ Y.GetHashCode() ^ Z.GetHashCode();
    }
    
    public static bool operator ==(Vector3 vectorA, Vector3 vectorB) {
      return Math.Abs(vectorA.X - vectorB.X) < Meth.Epsilon &&
             Math.Abs(vectorA.Y - vectorB.Y) < Meth.Epsilon &&
             Math.Abs(vectorA.Z - vectorB.Z) < Meth.Epsilon;
    }
    
    public static bool operator !=(Vector3 vectorA, Vector3 vectorB) {
      return Math.Abs(vectorA.X - vectorB.X) > Meth.Epsilon ||
             Math.Abs(vectorA.Y - vectorB.Y) > Meth.Epsilon ||
             Math.Abs(vectorA.Z - vectorB.Z) > Meth.Epsilon;
    }
    
    /// <summary>
    /// Get the scalar distance between this vector and the supplied vector.
    /// </summary>
    public double Distance(Vector3 vector) {
      return Math.Sqrt(Meth.Square(vector.X - X) + Meth.Square(vector.Y - Y) + Meth.Square(vector.Z - Z));
    }
    /// <summary>
    /// Get the scalar distance between vectors squared.
    /// </summary>
    public double DistanceSquare(Vector3 vector) {
      return Meth.Square(vector.X - X) + Meth.Square(vector.Y - Y) + Meth.Square(vector.Z - Z);
    }
    
    public static Vector3 operator +(Vector3 vectorA, Vector3 vectorB) {
      return new Vector3(vectorA.X + vectorB.X, vectorA.Y + vectorB.Y, vectorA.Z + vectorB.Z);
    }
    
    public static Vector3 operator -(Vector3 vectorA, Vector3 vectorB) {
      return new Vector3(vectorA.X - vectorB.X, vectorA.Y - vectorB.Y, vectorA.Z - vectorB.Z);
    }
    
    public static Vector3 operator *(Vector3 vectorA, Vector3 vectorB) {
      return new Vector3(vectorA.X * vectorB.X, vectorA.Y * vectorB.Y, vectorA.Z * vectorB.Z);
    }
    
    public static Vector3 operator /(Vector3 vectorA, Vector3 vectorB) {
      return new Vector3(vectorA.X / vectorB.X, vectorA.Y / vectorB.Y, vectorA.Z / vectorB.Z);
    }
    
    public static Vector3 operator *(Vector3 vector, double multiple) {
      return new Vector3(vector.X * multiple, vector.Y * multiple, vector.Z * multiple);
    }
    
    public static Vector3 operator *(double multiple, Vector3 vector) {
      return new Vector3(vector.X * multiple, vector.Y * multiple, vector.Z * multiple);
    }
    
    public static Vector3 operator /(Vector3 vector, double multiple) {
      return new Vector3(vector.X / multiple, vector.Y / multiple, vector.Z / multiple);
    }
    
    public static Vector3 operator /(Vector3 vector, int multiple) {
      return new Vector3(vector.X / multiple, vector.Y / multiple, vector.Z / multiple);
    }
    
    public static Vector3 operator %(Vector3 vectorA, Vector3 vectorB) {
      return new Vector3(vectorA.X % vectorB.X, vectorA.Y % vectorB.Y, vectorA.Z % vectorB.Z);
    }
    
    public static Vector3 operator %(Vector3 vectorA, int divisor) {
      return new Vector3(vectorA.X % divisor, vectorA.Y % divisor, vectorA.Z % divisor);
    }
    
    public static Vector3 operator -(Vector3 vector) {
      return new Vector3(-vector.X, -vector.Y, -vector.Z);
    }
    
    public static bool operator <(Vector3 vectorA, Vector3 vectorB) {
      return vectorA.MagnitudeSquare < vectorB.MagnitudeSquare;
    }
    
    public static bool operator <=(Vector3 vectorA, Vector3 vectorB) {
      return vectorA.MagnitudeSquare <= vectorB.MagnitudeSquare;
    }
    
    public static bool operator >(Vector3 vectorA, Vector3 vectorB) {
      return vectorA.MagnitudeSquare > vectorB.MagnitudeSquare;
    }
    
    public static bool operator >=(Vector3 vectorA, Vector3 vectorB) {
      return vectorA.MagnitudeSquare >= vectorB.MagnitudeSquare;
    }
    
  }
  
  // ----------------------------------------------------------------------------- Vector3 //
  
  public struct Vector4 : IEquatable<Vector4>, IComparable<Vector4> {
    static public Vector4 Zero {
      get {
        return new Vector4(0, 0);
      }
    }

    static public Vector4 One {
      get {
        return new Vector4(1, 1, 1, 1);
      }
    }
    
    public double MagnitudeSquare {
      get {
        return X * X + Y * Y + Z * Z + W * W;
      }
    }

    public double Magnitude {
      get {
        return Math.Sqrt(X * X + Y * Y + Z * Z + W * W);
      }
    }

    public double X;
    public double Y;
    public double Z;
    public double W;

    public Vector4(double x, double y, double z = 0, double w = 0) {
      X = x;
      Y = y;
      Z = z;
      W = w;
    }
    
    public override string ToString() {
      return Chars.BraceOpen +
        X.ToString(Meth.FormatDouble) + Chars.Comma +
        Y.ToString(Meth.FormatDouble) + Chars.Comma +
        Z.ToString(Meth.FormatDouble) + Chars.Comma +
        W.ToString(Meth.FormatDouble) +
        Chars.BraceClose;
    }
    
    public override int GetHashCode() {
      return X.GetHashCode() ^ Y.GetHashCode() ^ Z.GetHashCode() ^ W.GetHashCode();
    }
    
    public override bool Equals(object obj) {
      return (obj is Vector4) && Equals((Vector4)obj);
    }

    public bool Equals(Vector4 other) {
      return Math.Abs(X - other.X) < Meth.Epsilon &&
             Math.Abs(Y - other.Y) < Meth.Epsilon &&
             Math.Abs(Z - other.Z) < Meth.Epsilon &&
             Math.Abs(W - other.W) < Meth.Epsilon;
    }
    
    public int CompareTo(Vector4 other) {
      return (int)(MagnitudeSquare - other.MagnitudeSquare);
    }
    
    public static bool operator ==(Vector4 vectorA, Vector4 vectorB) {
      return Math.Abs(vectorA.X - vectorB.X) < Meth.Epsilon &&
             Math.Abs(vectorA.Y - vectorB.Y) < Meth.Epsilon &&
             Math.Abs(vectorA.Z - vectorB.Z) < Meth.Epsilon &&
             Math.Abs(vectorA.W - vectorB.W) < Meth.Epsilon;
    }
    
    public static bool operator !=(Vector4 vectorA, Vector4 vectorB) {
      return Math.Abs(vectorA.X - vectorB.X) > Meth.Epsilon ||
             Math.Abs(vectorA.Y - vectorB.Y) > Meth.Epsilon ||
             Math.Abs(vectorA.Z - vectorB.Z) > Meth.Epsilon ||
             Math.Abs(vectorA.W - vectorB.W) > Meth.Epsilon;
    }
    
    static public Vector4 operator +(Vector4 vectorA, Vector4 vectorB) {
      return new Vector4(vectorA.X + vectorA.X, vectorA.Y + vectorA.Y, vectorA.Z + vectorA.Z, vectorA.W + vectorB.W);
    }
    
    static public Vector4 operator -(Vector4 vectorA, Vector4 vectorB) {
      return new Vector4(vectorA.X - vectorB.X, vectorA.Y - vectorB.Y, vectorA.Z - vectorB.Z);
    }
    
    static public Vector4 operator *(Vector4 vectorA, Vector4 vectorB) {
      return new Vector4(vectorA.X * vectorB.X, vectorA.Y * vectorB.Y, vectorA.Z * vectorB.Z);
    }
    
    static public Vector4 operator /(Vector4 vectorA, Vector4 vectorB) {
      return new Vector4(vectorA.X / vectorB.X, vectorA.Y / vectorB.Y, vectorA.Z / vectorB.Z);
    }
    
    public static Vector4 operator *(Vector4 vector, double multiple) {
      return new Vector4(vector.X * multiple, vector.Y * multiple, vector.Z * multiple, vector.W * multiple);
    }
    
    public static Vector4 operator *(double multiple, Vector4 vector) {
      return new Vector4(vector.X * multiple, vector.Y * multiple, vector.Z * multiple, vector.W * multiple);
    }
    
    public static Vector4 operator /(Vector4 vector, double multiple) {
      return new Vector4(vector.X / multiple, vector.Y / multiple, vector.Z / multiple, vector.W / multiple);
    }
    
    public static Vector4 operator /(Vector4 vector, int multiple) {
      return new Vector4(vector.X / multiple, vector.Y / multiple, vector.Z / multiple, vector.W / multiple);
    }
    
    public static Vector4 operator %(Vector4 vectorA, Vector4 vectorB) {
      return new Vector4(vectorA.X % vectorB.X, vectorA.Y % vectorB.Y, vectorA.Z % vectorB.Z, vectorA.W % vectorB.W);
    }
    
    public static Vector4 operator %(Vector4 vectorA, int divisor) {
      return new Vector4(vectorA.X % divisor, vectorA.Y % divisor, vectorA.Z % divisor, vectorA.W % divisor);
    }
    
    public static Vector4 operator -(Vector4 vector) {
      return new Vector4(-vector.X, -vector.Y, -vector.Z, -vector.W);
    }
    
    public static bool operator <(Vector4 vectorA, Vector4 vectorB) {
      return vectorA.MagnitudeSquare < vectorB.MagnitudeSquare;
    }
    
    public static bool operator <=(Vector4 vectorA, Vector4 vectorB) {
      return vectorA.MagnitudeSquare <= vectorB.MagnitudeSquare;
    }
    
    public static bool operator >(Vector4 vectorA, Vector4 vectorB) {
      return vectorA.MagnitudeSquare > vectorB.MagnitudeSquare;
    }
    
    public static bool operator >=(Vector4 vectorA, Vector4 vectorB) {
      return vectorA.MagnitudeSquare >= vectorB.MagnitudeSquare;
    }
    
  }

}

