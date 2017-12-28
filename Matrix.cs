using System;
using System.Diagnostics;
namespace Simple_LA
{
	public class Matrix
	{
		protected readonly double Tolerance;
		public const double DEFAULT_TOLERANCE = 0.000000001;
		public double[,] elements { get; private set;}
		public readonly int rows;
		public readonly int columns;



		/// <param name="tolerance">Tolerance for floating point errors.</param>
		public Matrix(double[,] elements, double tolerance) 
		{
			this.elements = elements;
			Tolerance = tolerance;
			rows = elements.GetLength(0);
			columns = elements.GetLength(1);
		}



		/// <summary>
		/// (m1xn1)*(m2xn2)=(m1xn2). # of columns in matrix1 must equal # of rows in matrix2.
		/// </summary>
		public Matrix Multiply(Matrix m2)
		{
			if (columns != m2.rows)
			{
				throw new Exception("Matrix.Multiply(Matrix m2): " +
							   "number of columns in this matrix does not match number of rows in m2.");
			}

			double[,] result = new double[rows, m2.columns];


			for (int i = 0; i < rows; i++)
			{
				Vector row = GetRow(i);
				for (int j = 0; j < m2.columns; j++)
				{
					Vector column = m2.GetColumn(j);
					result[i, j] = row.DotProduct(column.Transpose());
				}
			}

			return new Matrix(result, Tolerance);
		}


		public Matrix ScalarMultiplication(double scalar)
		{

			double[,] result = new double[rows, columns];

			for (int i = 0; i < rows; i++)
			{
				for (int j = 0; j < columns; j++)
				{
					result[i, j] = scalar * elements[i, j];
				}
			}

			return new Matrix(result, Tolerance);
		}



		/// <summary>
		/// Matrices must be the same size.
		/// </summary>
		public Matrix Add(Matrix m2)
		{
			if (!IsSameSize(m2))
			{
				throw new Exception("Matrix.Add(Matrix m2): " +
									"matrices must be the same size.");
			}

			double[,] result = new double[rows, columns];

			for (int i = 0; i < rows; i++)
			{
				for (int j = 0; j < columns; j++)
				{
					result[i, j] = elements[i, j] + m2.elements[i, j];
				}
			}

			return new Matrix(result, Tolerance);
		}



		/// <summary>
		/// this - m2 = m3 
		/// Matrices must be the same size.
		/// </summary>
		public Matrix Subtract(Matrix m2)
		{
			if (!IsSameSize(m2))
			{
				throw new Exception("Matrix.Subtract(Matrix m2): " +
					"matrices must be the same size.");
			}

			double[,] result = new double[rows, columns];

			for (int i = 0; i < rows; i++)
			{
				for (int j = 0; j < columns; j++)
				{
					result[i, j] = elements[i, j] - m2.elements[i, j];
				}
			}

			return new Matrix(result, Tolerance);
		}


		/// <summary>
		/// (m1xn1)T=(n1xm1)
		/// </summary>
		public Matrix Transpose()
		{
			double[,] mT = new double[columns, rows];

			for (int i = 0; i < rows; i++)
			{
				for (int j = 0; j < columns; j++)
				{
					mT[j, i] = elements[i, j];
				}
			}

			return new Matrix(mT, Tolerance);
		}



		/// <summary>
		/// True if entire matrix is zero.
		/// </summary>
		public bool IsZero()
		{
			for (int i = 0; i < rows; i++)
			{
				for (int j = 0; j < columns; j++)
				{
					if (Math.Abs(elements[i, j]) > Tolerance) { return false; }
				}
			}

			return true;
		}


		/// <summary>
		/// Checks if the double at element[i, j] is within Tolerance to be considered equal to int equalTo
		/// </summary>
		public bool EqualsInt(int equalTo, int i, int j)
		{
			if (elements[i, j] < equalTo + Tolerance && elements[i, j] > equalTo - Tolerance)
			{
				return true;
			}
			return false;
		}



		/// <summary>
		/// Checks if the double at element[i, j] is within Tolerance to be considered equal to double equalTo
		/// </summary>
		public bool EqualsDouble(int equalTo, int i, int j)
		{
			if (elements[i, j] < equalTo + Tolerance && elements[i, j] > equalTo - Tolerance)
			{
				return true;
			}
			return false;
		}




		public bool IsEqual(Matrix m2)
		{
			if (!IsSameSize(m2)){ return false; }

			for (int i = 0; i < rows; i++)
			{
				for (int j = 0; j < columns; j++)
				{
					if (Math.Abs(elements[i, j]) - Math.Abs(m2.elements[i, j]) > Tolerance) { return false; }
				}
			}

			return true;
		}


		public bool IsSameSize(Matrix m2)
		{
			if (rows != m2.rows || columns != m2.columns){ return false; }
			return true;
		}



		public bool NewElements(Matrix matrix)
		{
			if (IsSameSize(matrix))
			{
				elements = matrix.elements;
				return true;
			}

			return false;
		}


		public void PrintElements()
		{
			string values = "";
			for (int i = 0; i < rows; i++)
			{
				for (int j = 0; j < columns; j++)
				{
					values += elements[i, j].ToString() + " ";
				}
				values += "\n";
			}
			Debug.WriteLine(values);
		}


		public void PrintSize()
		{
			Debug.WriteLine(rows + "x" + columns);
		}



		public Vector GetRow(int rowIndex)
		{
			if (rowIndex < 0 || rowIndex >= rows) 
			{ 
				return null; 
			}

			double[,] rowElements = new double[1,columns];

			for (int j = 0; j < columns; j++)
			{
				rowElements[0, j] = elements[rowIndex, j];
			}

			return new Vector(rowElements, Tolerance);
		}




		public Vector GetColumn(int columnIndex)
		{
			if (columnIndex < 0 || columnIndex >= columns)
			{
				return null;
			}

			double[,] columnElements = new double[rows, 1];

			for (int i = 0; i < rows; i++)
			{
				columnElements[i, 0] = elements[i, columnIndex];
			}

			return new Vector(columnElements, Tolerance);
		}
	}
}
