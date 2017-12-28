using System;
using System.Diagnostics;
namespace Simple_LA
{
	public class Vector : Matrix
	{
		public readonly int dimensions;
		public readonly bool horizontal;
		public readonly bool vertical;

		/// <param name="tolerance">Tolerance for floating point errors.</param>
		public Vector(double[,] elements, double tolerance) : base(elements, tolerance)
		{
			if (elements.GetLength(0) > 1 && elements.GetLength(1) == 1)
			{
				vertical = true;
				dimensions = rows;
			}
			else if (elements.GetLength(1) > 1 && elements.GetLength(0) == 1)
			{
				horizontal = true;
				dimensions = columns;
			}
			else
			{
				throw new Exception("Vector.Vector(double[,] elements, double tolerance):" + 
				                    "Vector must be either Nx1 (vertical) or 1xN (horizontal).");
			}
		}


		public Vector Add(Vector v2)
		{
			return new Vector(base.Add(v2).elements, Tolerance);
		}


		public Vector Subtract(Vector v2)
		{
			return new Vector(base.Subtract(v2).elements, Tolerance);
		}


		public new Vector ScalarMultiplication(double scalar)
		{
			return new Vector(base.ScalarMultiplication(scalar).elements, Tolerance);
		}


		public new Vector Transpose()
		{
			return new Vector(base.Transpose().elements, Tolerance);
		}



		/// <summary>
		/// M = Sqrt(V1^2 + V2^2 + ... + Vn^2)
		/// </summary>
		public double Magnitude()
		{
			double sumOfComponentsSquared = 0;
			for (int i = 0; i < rows; i++)
			{
				for (int j = 0; j < columns; j++)
				{
					sumOfComponentsSquared += (elements[i, j] * elements[i, j]);
				}
			}
			double magnitude = Math.Sqrt(sumOfComponentsSquared);

			return magnitude;
		}



		/// <summary>
		/// U(m1xn1) = V(m1xn1) / ||V(m1xn1)||
		/// </summary>
		public Vector Normalize()
		{
			if (IsZero())
			{
				throw new Exception("Vector.Normalize(): " +
					"cannot normalize a zero vector.");
			}

			double[,] result = new double[rows, columns];

			double magnitude = Magnitude();

			for (int i = 0; i < rows; i++)
			{
				for (int j = 0; j < columns; j++)
				{
					result[i, j] = elements[i, j] / magnitude;
				}
			}

			return new Vector(result, Tolerance);
		}



		/// <summary>
		/// Theta = arccos((V.W) / (||V||*||W||))
		/// Theta is in rad
		/// </summary>
		public double Angle(Vector v2)
		{
			if (!IsSameSize(v2))
			{
				throw new Exception("Vector.Angle(Vector v2): " +
					"vectors must be of same size.");
			}

			if (IsZero() || v2.IsZero())
			{
				throw new Exception("Vector.Angle(Vector v2): " +
					"cannot use a zero vector.");
			}

			return Math.Acos((DotProduct(v2)) / (Magnitude() * v2.Magnitude()));
		}



		/// <summary>
		/// V.W = V1*W1 + V2*W2 + ... + Vn*Wn
		/// Vectors must be the same size.
		/// </summary>
		public double DotProduct(Vector v2)
		{
			if (!IsSameSize(v2))
			{
				throw new Exception("Vector.DotProduct(Vector v2): " +
					"vectors must be of same size.");
			}

			double dotProduct = 0;

			for (int i = 0; i < rows; i++)
			{
				for (int j = 0; j < v2.columns; j++)
				{
					dotProduct += elements[i, j] * v2.elements[i, j];
				}
			}

			return dotProduct;
		}


		public bool IsParallel(Vector v2)
		{
			if (!IsSameSize(v2))
			{
				throw new Exception("Vector.IsParallel(Vector v2): " +
					"vectors must be of same size.");
			}

			if (IsZero() || v2.IsZero()) { return true; }

			if (IsEqual(v2)) { return true; }

			for (int i = 0; i < rows; i++)
			{
				for (int j = 0; j < columns; j++)
				{
					double scalar = v2.elements[0, 0] / elements[0, 0];
					if (Math.Abs(v2.elements[i, j] - (elements[i, j] * scalar)) > Tolerance) { return false; }
				}
			}

			return true;
		}



		public bool IsOrthogonal(Vector v2)
		{
			return Math.Abs(DotProduct(v2)) < Tolerance;
		}



		/// <summary>
		/// Projection of elements onto v2.
		/// </summary>
		public Vector Projection(Vector v2)
		{
			if (!IsSameSize(v2))
			{
				throw new Exception("Vector.Projection(Vector v2): " +
					"vectors must be of same size.");
			}

			if (IsZero() || v2.IsZero())
			{
				throw new Exception("Vector.Projection(vector v2): " +
					"cannot use a zero vector.");
			}

			Vector unitVector = v2.Normalize();

			return unitVector.ScalarMultiplication(DotProduct(unitVector));
		}



		/// <summary>
		/// Finds component of vector orthogonal to v2.
		/// </summary>
		public Vector OrthogonalComponent(Vector v2)
		{
			if (!IsSameSize(v2))
			{
				throw new Exception("Vector.OrthogonalComponent(Vector v2): " +
					"vectors must be of same size.");
			}

			return Subtract(Projection(v2));
		}



		/// <summary>
		/// Finds component of vector parallel to v2.
		/// </summary>
		public Vector ParallelComponent(Vector v2)
		{
			if (!IsSameSize(v2))
			{
				throw new Exception("Vector.ParallelComponent(Vector v2): " +
					"vectors must be of same size.");
			}

			return Subtract(OrthogonalComponent(v2));
		}



		/// <summary>
		/// V1xV2, vectors must be 3 dimensional column vectors.
		/// </summary>
		public Vector CrossProduct(Vector v2)
		{
			if (!Is3Dimensional() || !v2.Is3Dimensional())
			{
				throw new Exception("Vector.CrossProduct(Vector v2): " +
					"vectors must be 3 dimensional.");
			}

			double[,] crossProduct = new double[,]
			{
				{(elements[1, 0]*v2.elements[2, 0]) - (v2.elements[1, 0]*elements[2,0])},
				{0 - ((elements[0, 0]*v2.elements[2, 0]) - (v2.elements[0, 0]*elements[2, 0]))},
				{(elements[0, 0]*v2.elements[1, 0]) - (v2.elements[0, 0]*elements[1, 0])}
			};

			return new Vector(crossProduct, Tolerance);
		}



		public bool Is3Dimensional()
		{
			if (rows == 3 && columns == 1) { return true; }
			return false;
		}



		/// <summary>
		/// Checks if the double at element[i] is within Tolerance to be considered equal to int equalTo
		/// </summary>
		public bool EqualsInt(int equalTo, int i)
		{
			int j;
			if (horizontal)
			{
				j = i;
				i = 0;
			}
			else
			{
				j = 0;
			}
			if (elements[i, j] < equalTo + Tolerance && elements[i, j] > equalTo - Tolerance)
			{
				return true;
			}
			return false;
		}




		/// <summary>
		/// Checks if the double at element[i] is within Tolerance to be considered equal to double equalTo
		/// </summary>
		public bool EqualsDouble(double equalTo, int i)
		{
			int j;
			if (horizontal)
			{
				j = i;
				i = 0;
			}
			else
			{
				j = 0;
			}
			if (elements[i, j] < equalTo + Tolerance && elements[i, j] > equalTo - Tolerance)
			{
				return true;
			}
			return false;
		}


		/// <summary>
		/// ||V1xV2|| / 2
		/// </summary>
		public double AreaOfTriangle(Vector v2)
		{
			if (!Is3Dimensional() || !v2.Is3Dimensional())
			{
				throw new Exception("Vector.AreaOfTriangle(Vector v2): " +
					"vectors must be 3 dimensional.");
			}

			return 0.5 * AreaOfParallelogram(v2);
		}



		/// <summary>
		/// ||V1xV2||
		/// </summary>
		public double AreaOfParallelogram(Vector v2)
		{
			if (!Is3Dimensional() || !v2.Is3Dimensional())
			{
				throw new Exception("Vector.AreaOfParallelogram(Vector v2): " +
					"vectors must be 3 dimensional.");
			}

			return CrossProduct(v2).Magnitude();
		}



		/// <summary>
		/// Returns a n-length vector between points p1 and p2 in n dimensions. Vector points from p1 to p2.
		/// </summary>
		public static Vector GetVectorFromPoints(double[] p1, double[] p2)
		{
			if (p1.Length != p2.Length)
			{
				throw new Exception("Vector.GetVectorFromPoints(double[] p1, double[] p2): " + 
				                    "points must be in the same number of dimensions.");
			}

			double[,] elements = new double[p1.Length, 1];

			for (int i = 0; i < p1.Length; i++)
			{
				elements[i, 0] = p2[i] - p1[i];
			}

			return new Vector(elements, Vector.DEFAULT_TOLERANCE);

		}

		public double GetElement(int position)
		{
			return elements[position, 0];
		}
	}
}
