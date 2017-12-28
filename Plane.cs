using System;
using System.Diagnostics;
namespace Simple_LA
{
	public class Plane : LinearEquation
	{
		public Plane(double a, double b, double c, double k) : base(GetNormalVector(a, b, c), k)
		{
		}

		static private Vector GetNormalVector(double a, double b, double c)
		{
			double[,] vectorElements = new double[,] { { a }, { b }, { c } };
			return new Vector(vectorElements, Vector.DEFAULT_TOLERANCE);
		}
	}
}
