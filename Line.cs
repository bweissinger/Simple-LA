using System;
using System.Diagnostics;
namespace Simple_LA
{
	public class Line : LinearEquation
	{
		public Line(double a, double b, double k) : base(GetNormalVector(a, b), k)
		{
		}

		static private Vector GetNormalVector(double a, double b)
		{
			double[,] vectorElements = new double[,] { { a }, { b } };
			return new Vector(vectorElements, Vector.DEFAULT_TOLERANCE);
		}
	}
}
