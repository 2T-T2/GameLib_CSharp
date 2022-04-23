using System;
using System.Drawing;

namespace GameLib.Util
{
	/// <summary>二次元ベクトルを表すクラス</summary>
	public class Vector {
		/// <summary>x成分</summary>
		public double x;
		/// <summary>y成分</summary>
		public double y;
		/// <summary>コンストラクタ</summary>
		/// <summary>x成分</summary>
		/// <summary>y成分</summary>
		public Vector( double x, double y ) {
			this.x = x;
			this.y = y;
		}
		/// <summary>ベクトルを足す</summary>
		/// <param name="vec">このオブジェクトに足されるベクトル</param>
		/// <returns>void型</returns>
		public Vector add( Vector vec ) { return new Vector(this.x + vec.x, this.y + vec.y); }
		/// <summary>ベクトルを引く</summary>
		/// <param name="vec">このオブジェクトから引かれるベクトル</param>
		/// <returns>void型</returns>
		public Vector sub( Vector vec ) { return this.add( vec.mul(-1) ); }
		/// <summary>ベクトルをかける</summary>
		/// <param name="n">このオブジェクトにかけられる数値</param>
		/// <returns>void型</returns>
		public Vector mul( double n )   { return new Vector(this.x * n, this.y * n); }
		/// <summary>このベクトルの長さを取得</summary>
		/// <returns>double型。ベクトルの長さ</returns>
		public double getLength()  { return Math.Sqrt( ((this.x * this.x) + (this.y * this.y)) ); }
		/// <summary>このベクトルの角度を取得</summary>
		/// <returns>double型。このベクトルの角度。度数</returns>
		public double getDegree()  { return this.getRadian() * (180/Math.PI); }
		/// <summary>このベクトルの角度を取得</summary>
		/// <returns>double型。このベクトルの角度。Radian度数</returns>
		public double getRadian()  { return Math.Atan2( this.y, this.x ); }

		/// <summary>オペレーターオーバーロード</summary>
		public static Vector operator+ ( Vector vec1, Vector vec2 ) { return vec1.add( vec2 ); }
		/// <summary>オペレーターオーバーロード</summary>
		public static Vector operator- ( Vector vec1, Vector vec2 ) { return vec1.sub( vec2 ); }
		/// <summary>オペレーターオーバーロード</summary>
		public static Vector operator* ( Vector vec1, double d ) { return vec1.mul( d ); }
		/// <summary>オペレーターオーバーロード</summary>
		public static Vector operator/ ( Vector vec1, double d ) { return vec1.mul( 1/d ); }
	}

	/// <summary>位置ベクトルを表すクラス</summary>
	public class PositionVector {
		/// <summary>この位置ベクトルの原点座標</summary>
		public Point point;
		/// <summary>この位置ベクトルのベクトル</summary>
		public Vector vector;

		/// <summary>コンストラクタ</summary>
		/// <param name="p">この位置ベクトルの原点座標</param>
		/// <param name="vec">この位置ベクトルのベクトル</param>
		public PositionVector( Point p, Vector vec ) {
			this.point = p;
			this.vector = vec;
		}
		/// <summary>コンストラクタ</summary>
		/// <param name="start">この位置ベクトルの原点座標</param>
		/// <param name="end">この位置ベクトルの終点座標</param>
		public PositionVector( Point start, Point end ) {
			this.point = start;
			this.vector = new Vector( end.X - start.X, end.Y - start.Y );
		}
		/// <summary>この位置ベルトルの原点X座標を取得する</summary>
		/// <returns>double型。この位置ベルトルの原点X座標</returns>
		public double getBeginX() { return this.point.X; }
		/// <summary>この位置ベルトルの原点y座標を取得する</summary>
		/// <returns>double型。この位置ベルトルの原点Y座標</returns>
		public double getBeginY() { return this.point.Y; }
		/// <summary>この位置ベルトルの終点X座標を取得する</summary>
		/// <returns>double型。この位置ベルトルの終点X座標</returns>
		public double getEndX() { return this.point.X + this.vector.x; }
		/// <summary>この位置ベルトルの終点y座標を取得する</summary>
		/// <returns>double型。この位置ベルトルの終点y座標</returns>
		public double getEndY() { return this.point.Y + this.vector.y; }
	}
}