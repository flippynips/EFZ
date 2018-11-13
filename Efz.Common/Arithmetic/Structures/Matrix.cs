using System;

namespace Efz.Maths {
  
  public struct Matrix2 {
    public double     m00;
    public double     m01;
    public double     m10;
    public double     m11;
    
    public Matrix2(double _radians) {
      double c = Math.Cos(_radians);
      double s = Math.Sin(_radians);

      m00 = c; m01 = -s;
      m10 = s; m11 =  c;
    }

    public Matrix2(double _m00, double _m01, double _m10, double _m11) {
      m00 = _m00; m01 = _m01;
      m10 = _m10; m11 = _m11;
    }
    
    public void Rotate(double _radians) {
      double c = Math.Cos(_radians);
      double s = Math.Sin(_radians);
      
      m00 = c; m01 = -s;
      m10 = s; m11 =  c;
    }
    
    public void Scale(Vector2 _size) {
      m00 *= _size.X;
      m11 *= _size.Y;
    }
    
    public void Sheer(Vector2 _sheer) {
      m01 *= _sheer.X;
      m10 *= _sheer.Y;
    }
    
    public Matrix2 Abs() {
      return new Matrix2(Math.Abs(m00), Math.Abs(m01), Math.Abs(m10), Math.Abs(m11));
    }

    public Vector2 AxisX() {
      return new Vector2(m00, m10);
    }

    public Vector2 AxisY() {
      return new Vector2(m01, m11);
    }
    
    public Matrix2 Transpose() {
      return new Matrix2(m00, m10, m01, m11);
    }
    
    public override int GetHashCode() {
      return (int)this.m00 ^ (int)this.m01 ^ (int)this.m10 ^ (int)this.m11;
    }
    
    static public Vector2 operator *(Matrix2 _matrix, Vector2 _point) {
      return new Vector2(_matrix.m00 * _point.X + _matrix.m01 * _point.Y, _matrix.m10 * _point.X + _matrix.m11 * _point.Y);
    }
    
    static public Matrix2 operator *(Matrix2 _matrixX, Matrix2 _matrixY) {
      // [00 01]  [00 01]
      // [10 11]  [10 11]
      return new Matrix2(
        _matrixX.m00 * _matrixY.m00 + _matrixX.m01 * _matrixY.m10,
        _matrixX.m00 * _matrixY.m01 + _matrixX.m01 * _matrixY.m11,
        _matrixX.m10 * _matrixY.m00 + _matrixX.m11 * _matrixY.m10,
        _matrixX.m10 * _matrixY.m01 + _matrixX.m11 * _matrixY.m11
      );
    }
  }

}

