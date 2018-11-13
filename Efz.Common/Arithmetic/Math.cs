using System;

using System.Linq;
using Efz.Maths;

namespace Efz {
  
  /// <summary>
  /// General methematical methods.
  /// </summary>
  static public class Meth {
    
    public const double Pi       = Math.PI;
    public const double PiTwo    = Math.PI * 2;
    public const double PiHalf   = Math.PI * .5;
    public const double PiQuater = Math.PI * .25;
    public const double Degrees2Radians = Math.PI / 180.0;
    public const double Epsilon  = 0.0001;
    public const double EpsilonInverse = 1/Epsilon;
    /// <summary>
    /// https://en.wikipedia.org/wiki/E_%28mathematical_constant%29
    /// </summary>
    public const double E = 2.71828182845904523536028747135266249775724709369995;
    
    public const string FormatDouble = "#####0.0000";
    
    public static float Smooth(float value, float target = 1f) {
      return ((float)Math.Cos(Pi + Pi * (value / target)) + 1) / 2f;
    }
    
    public static double Smooth(double value, double target = 1.0) {
      return (Math.Cos(Pi + Pi * (value / target)) + 1) / 2.0;
    }
    
    public static double Smooth(long value, long target = 1L) {
      return (Math.Cos(Pi + Pi * (value / (double)target)) + 1.0) / 2.0;
    }
    
    public static double Smooth(int value, int target = 1) {
      return (Math.Cos(Pi + Pi * (value / (double)target)) + 1.0) / 2.0;
    }
    
    /// <summary>
    /// Limit the value within the min and max values inclusive.
    /// </summary>
    public static long MinMax(long value, long min, long max) {
      if(value < min) return min;
      return value > max ? max : value;
    }
    
    /// <summary>
    /// Limit the value within the min and max values inclusive.
    /// </summary>
    public static int MinMax(int value, int min, int max) {
      if(value < min) return min;
      return value > max ? max : value;
    }
    
    /// <summary>
    /// Limit the value within the min and max values inclusive.
    /// </summary>
    public static float MinMax(float value, float min, float max) {
      if(value < min) return min;
      return value > max ? max : value;
    }
    
    /// <summary>
    /// Limit the value within the min and max values inclusive.
    /// </summary>
    public static double MinMax(double value, double min, double max) {
      if(value < min) return min;
      return value > max ? max : value;
    }
    
    /// <summary>
    /// Wrap a value between 0 and the specified max value inclusive.
    /// </summary>
    public static int Wrap(int value, int max) {
      if(value > max) return value % max;
      if(value < 0) return -value % max;
      return value;
    }
    
    /// <summary>
    /// Wrap a value between a minimum and maximum value inclusive.
    /// </summary>
    public static int Wrap(int value, int max, int min) {
      if(value > max) return min + value % (max - min);
      if(value < min) {
        value = value % (max - min);
        if(value < 0) return min - value;
        return min + value;
      }
      return value;
    }
    
    /// <summary>
    /// Wrap a value between 0 and the specified max value inclusive.
    /// </summary>
    public static long Wrap(long value, long max) {
      if(value > max) return value % max;
      if(value < 0) return -value % max;
      return value;
    }
    
    /// <summary>
    /// Wrap a value between a minimum and maximum value inclusive.
    /// </summary>
    public static long Wrap(long value, long max, long min) {
      if(value > max) return min + value % (max - min);
      if(value < min) {
        value = value % (max - min);
        if(value < 0) return min - value;
        return min + value;
      }
      return value;
    }
    
    /// <summary>
    /// Wrap a value between 0 and the specified max value inclusive.
    /// </summary>
    public static double Wrap(double value, double max) {
      if(value > max) return value % max;
      if(value < 0) return -value % max;
      return value;
    }
    
    /// <summary>
    /// Wrap a value between a minimum and maximum value inclusive.
    /// </summary>
    public static double Wrap(double value, double max, double min) {
      if(value > max) return min + value % (max - min);
      if(value < min) {
        value = value % (max - min);
        if(value < 0) return min - value;
        return min + value;
      }
      return value;
    }
    
    static public void Lerp(ref ushort value1, ushort value2, double delta) {
      value1 = (ushort)(value1 + (value2 - value1) * delta);
    }
    
    static public void Lerp(ref int value1, int value2, double delta) {
      value1 = (int)(value1 + (value2 - value1) * delta);
    }
    
    static public void Lerp(ref float value1, float value2, float delta) {
      value1 = value1 + (value2 - value1) * delta;
    }
    
    static public void Lerp(ref double value1, double value2, double delta) {
      value1 = value1 + (value2 - value1) * delta;
    }
    
    static public void Lerp(ref Vector2 value1, ref Vector2 value2, double delta) {
      Lerp(ref value1.X, value2.X, delta);
      Lerp(ref value1.Y, value2.Y, delta);
    }
    
    static public void Lerp(ref Vector3 value1, ref Vector3 value2, double delta) {
      Lerp(ref value1.X, value2.X, delta);
      Lerp(ref value1.Y, value2.Y, delta);
      Lerp(ref value1.Z, value2.Z, delta);
    }
    
    static public Vector2 Cross(Vector2 point, double a) {
      return new Vector2(-a * point.X, a * point.Y);
    }

    static public Vector2 Cross(double a, Vector2 point) {
      return new Vector2(a * point.X, -a * point.Y);
    }
    
    static public double Cross(Vector2 a, Vector2 b) {
      return a.X * b.Y - a.Y * b.X;
    }
    
    static public double Dot(Vector2 a, Vector2 b) {
      return a.X * b.X + a.Y * b.Y;
    }
    
    static public Vector2 Min(Vector2 a, Vector2 b) {
      return new Vector2(Math.Min(a.X, b.X), Math.Min(a.Y, b.Y));
    }
    
    static public Vector2 Max(Vector2 a, Vector2 b) {
      return new Vector2(Math.Max(a.X, b.X), Math.Max(a.Y, b.Y));
    }
    
    /// <summary>
    /// Returns the intersection point of the line A with line B
    /// Ref http://paulbourke.net/geometry/lineline2d/
    /// </summary>
    public static Vector2 LineLineIntersection(Vector2 lineA1, Vector2 lineA2, Vector2 lineB1, Vector2 lineB2){
      double s = ((lineB2.X - lineB1.X) * (lineA1.Y - lineB1.Y) - (lineB2.Y - lineB1.Y) * (lineA1.X - lineB1.X))
               / ((lineB2.Y - lineB1.Y) * (lineA2.X - lineA1.X) - (lineB2.X - lineB1.X) * (lineA2.Y - lineA1.Y));
      return new Vector2(lineA1.X + s * (lineA2.X - lineA1.X), lineA1.Y + s * (lineA2.Y - lineA1.Y));
    }
    
    /// <summary>
    /// Returns the intersection point of a line from the origin to point A with line B
    /// Ref http://paulbourke.net/geometry/lineline2d/
    /// </summary>
    static public Vector2 LineLineIntersection(Vector2 pointA, Vector2 lineB1, Vector2 lineB2){
      double s = ((lineB2.X - lineB1.X) * -lineB1.Y - (lineB2.Y - lineB1.Y) * -lineB1.X)
               / ((lineB2.Y - lineB1.Y) * pointA.X - (lineB2.X - lineB1.X) * pointA.Y);
      return new Vector2(s * pointA.X, s * pointA.Y);
    }
    
    /// <summary>
    /// Return the closest line circle intersection to point.
    /// </summary>
    static public Vector2 LineCircleClosestIntersection(Vector2 lineA, Vector2 lineB, Vector2 circlePosition, double circleRadius, Vector2 point) {
      Vector2 intersectionA;
      Vector2 intersectionB;
      byte intersections = LineCircleIntersections(lineA, lineB, circlePosition, circleRadius, out intersectionA, out intersectionB);
      
      if (intersections == 1) {
        return intersectionA;
      }
      
      if (intersections == 2) {
        if (intersectionA.DistanceSquare(point) < intersectionB.DistanceSquare(point)) {
          return intersectionA;
        }
        return intersectionB;
      }

      return Vector2.Zero;// no intersections at all
    }
    
    /// <summary>
    /// Return the closest line circle intersection to point.
    /// </summary>
    static public Vector2 LineCircleFurthestIntersection(Vector2 lineA, Vector2 lineB, Vector2 circlePosition, double circleRadius, Vector2 point) {
      Vector2 intersectionA;
      Vector2 intersectionB;
      byte intersections = LineCircleIntersections(lineA, lineB, circlePosition, circleRadius, out intersectionA, out intersectionB);
      
      if (intersections == 1) {
        return intersectionA;
      }
      
      if (intersections == 2) {
        if (intersectionA.DistanceSquare(point) > intersectionB.DistanceSquare(point)) {
          return intersectionA;
        }
        return intersectionB;
      }

      return Vector2.Zero;// no intersections at all
    }
    
    /// <summary>
    /// Return the closest line circle intersection to (0, 0).
    /// </summary>
    static public Vector2 LineCircleClosestIntersection(Vector2 line, Vector2 circlePosition, double circleRadius) {
      Vector2 intersectionA;
      Vector2 intersectionB;
      int intersections = LineCircleIntersections(line, circlePosition, circleRadius, out intersectionA, out intersectionB);
      
      if (intersections == 1) {
        return intersectionA;
      }
      
      if (intersections == 2) {
        if (intersectionA.MagnitudeSquare < intersectionB.MagnitudeSquare) {
          return intersectionA;
        }
        return intersectionB;
      }
      // No Intersections.
      return Vector2.Zero;
    }
    
    /// <summary>
    /// Return the furthest line circle intersection from (0, 0).
    /// </summary>
    static public Vector2 LineCircleFurthestIntersection(Vector2 line, Vector2 circlePosition, double circleRadius) {
      Vector2 intersectionA;
      Vector2 intersectionB;
      int intersections = LineCircleIntersections(line, circlePosition, circleRadius, out intersectionA, out intersectionB);
      
      if (intersections == 1) {
        return intersectionA;
      }
      
      if (intersections == 2) {
        if (intersectionA.MagnitudeSquare > intersectionB.MagnitudeSquare) {
          return intersectionA;
        }
        return intersectionB;
      }
      // No Intersections.
      return Vector2.Zero;
    }

    /// <summary>
    /// Find the points of intersection between a line and a circle.
    /// </summary>
    static public byte LineCircleIntersections(Vector2 lineA, Vector2 lineB, Vector2 circlePosition, double circleRadius, out Vector2 intersectionA, out Vector2 intersectionB) {
      double dx, dy, A, B, C, det, t;
      
      dx = lineB.X - lineA.X;
      dy = lineB.Y - lineA.Y;
      
      A = dx * dx + dy * dy;
      B = 2 * (dx * (lineA.X - circlePosition.X) + dy * (lineA.Y - circlePosition.Y));
      C = (lineA.X - circlePosition.X) * (lineA.X - circlePosition.X) + (lineA.Y - circlePosition.Y) * (lineA.Y - circlePosition.Y) - circleRadius * circleRadius;

      det = B * B - 4 * A * C;
      if ((A <= Epsilon) || (det < 0)) {
        // no solution
        intersectionA = intersectionB = Vector2.Zero;
        return 0;
      }
      if (Math.Abs(det) < Meth.Epsilon) {
        // One solution.
        t = -B / (2 * A);
        intersectionA = new Vector2(lineA.X + t * dx, lineA.Y + t * dy);
        intersectionB = Vector2.Zero;
        return 1;
      }
      // Two solutions.
      t = ((-B + Math.Sqrt(det)) / (2 * A));
      intersectionA = new Vector2(lineA.X + t * dx, lineA.Y + t * dy);
      t = ((-B - Math.Sqrt(det)) / (2 * A));
      intersectionB = new Vector2(lineA.X + t * dx, lineA.Y + t * dy);
      return 2;
    }
    
    /// <summary>
    /// Find the points of intersection between a line through the origin and a circle.
    /// </summary>
    static public byte LineCircleIntersections(Vector2 line, Vector2 circlePosition, double circleRadius, out Vector2 intersectionA, out Vector2 intersectionB) {
      double dx, dy, A, B, C, det, t;
      
      dx = line.X;
      dy = line.Y;
      
      A = dx * dx + dy * dy;
      B = 2 * (dx * -circlePosition.X + dy * -circlePosition.Y);
      C = (-circlePosition.X) * -circlePosition.X + circlePosition.Y * circlePosition.Y - circleRadius * circleRadius;

      det = B * B - 4 * A * C;
      if ((A <= Epsilon) || (det < 0)) {
        intersectionA = intersectionB = Vector2.Zero;
        return 0;
      }
      if (det == 0) {
        // One solution.
        t = -B / (2 * A);
        intersectionA = new Vector2(t * dx, t * dy);
        intersectionB = Vector2.Zero;
        return 1;
      }
      // Two solutions.
      t = ((-B + Math.Sqrt(det)) / (2 * A));
      intersectionA = new Vector2(t * dx, t * dy);
      t = ((-B - Math.Sqrt(det)) / (2 * A));
      intersectionB = new Vector2(t * dx, t * dy);
      return 2;
    }
    
    /// <summary>
    /// For speed; does not cover infinite circle intersections, or 1 intersection. During program use this is
    /// rarely needed, and easily added.
    /// </summary>
    public static byte CircleCircleIntersections(Vector2 circleAPosition, double circleARadius, Vector2 circleBPosition, double circleBRadius, out Vector2 intersectionA, out Vector2 intersectionB) {
      // If the circles are closer than both their radius combined.
      double distance = circleAPosition.DistanceSquare(circleBPosition);
      if(distance < (circleARadius + circleBRadius) * (circleARadius + circleBRadius)) {
        // Normalized vector in direction of collision center, from circleA.
        Vector2 normal = circleBPosition - circleAPosition.Normalize();
        
        // The middle of the collision area.
        double midpointDistance = (circleARadius * circleARadius - circleBRadius * circleBRadius + distance) / System.Math.Sqrt(distance);
        Vector2 midpoint = circleAPosition + normal * midpointDistance;
        
        // Distance between intersection points / 2. (a^2 + b^2 = c^2)
        double width = Math.Sqrt((circleARadius * circleARadius) - midpointDistance * midpointDistance);
        // Normal of the collision. Already normalized.
        normal = new Vector2(-normal.Y, normal.X);
        
        // The intersections are the midpoint plus the normal multiplied by the width of the collision points.
        intersectionA = midpoint + normal * width;
        intersectionB = midpoint - normal * width;
        return 2;
      }
      intersectionA = intersectionB = Vector2.Zero;
      return 0;
    }
    
    /// <summary>
    /// Get the closest intersection between two circles to a point.
    /// </summary>
    public static Vector2 CircleCircleClosestIntersection(Vector2 circleAPosition, double circleARadius, Vector2 circleBPosition, double circleBRadius, Vector2 point) {
      Vector2 intersectionA;
      Vector2 intersectionB;
      
      byte intersections = CircleCircleIntersections(circleAPosition, circleARadius, circleBPosition, circleBRadius, out intersectionA, out intersectionB);
      
      if(intersections == 2) {
        if(intersectionA.DistanceSquare(point) < intersectionB.DistanceSquare(point)) {
          return intersectionA;
        }
        return intersectionB;
      }
      return Vector2.Zero;
    }
    
    /// <summary>
    /// Returns if the point is 'left' of an arc.
    /// </summary>
    public static bool LeftOfArc(bool convex, Vector2 arc1, Vector2 arc2, Vector2 arcCenter, double arcRadius, Vector2 point) {
      if(convex) {
        return (arc2.X - arc1.X) * (point.Y - arc1.Y) - (arc2.Y - arc1.Y) * (point.X - arc1.X) < 0 &&
        arcCenter.DistanceSquare(point) > arcRadius * arcRadius;
      }
      return (arc2.X - arc1.X) * (point.Y - arc1.Y) - (arc2.Y - arc1.Y) * (point.X - arc1.X) < 0 ||
             arcCenter.DistanceSquare(point) > arcRadius * arcRadius;
    }
    
    // Returns if the point is 'left' of a line
    public static bool LeftOf(Vector2 line1, Vector2 line2, Vector2 point) {
      return (line2.X - line1.X) * (point.Y - line1.Y) - (line2.Y - line1.Y) * (point.X - line1.X) < 0;
    }
    
    /// <summary>
    /// Returns a shortened vector:
    /// p * (1 - f) + q * f
    /// </summary>
    public static Vector2 Interpolate(Vector2 vector1, Vector2 vector2, double delta) {
      return new Vector2(vector1.X * (1.0 - delta) + vector2.X * delta, vector1.Y * (1.0 - delta) + vector2.Y * delta);
    }
    
    /// <summary>
    /// Square the value.
    /// </summary>
    static public double Square(double value) {
      return value * value;
    }
    
    /// <summary>
    /// Round a double value to the closest long.
    /// </summary>
    static public long Round(double value) {
      return (long)(value + .5);
    }
    
    /// <summary>
    /// Determines whether value A is greater than value B with a bias relative to the two values.
    /// </summary>
    static public bool BiasGreaterThan(double valueA, double valueB) {
      return valueA >= valueB * 0.95 + valueA * 0.01;
    }
    
  }
}
