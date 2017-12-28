using System;
namespace Simple_LA
{
	public class LinearEquation
	{
		public readonly int dimensions;
		public readonly Vector normalVector;
		public readonly double k;

		public LinearEquation(Vector normalVector, double k)
		{
			this.normalVector = normalVector;
			this.k = k;
			dimensions = normalVector.dimensions;
		}

		public bool IsParallel(LinearEquation eq2)
		{
			return normalVector.IsParallel(eq2.normalVector);
		}

		public bool IsEqual(LinearEquation eq2)
		{
			double[] pt1 = new double[dimensions];
			double[] pt2 = new double[dimensions];

			for (int i = 0; i < dimensions - 1; i++)
			{
				pt2[i] -= normalVector.GetElement(i);
			}
			pt1[dimensions - 1] = eq2.k / eq2.normalVector.GetElement(dimensions - 1);
			pt2[dimensions - 1] = k / normalVector.GetElement(dimensions - 1);


			return Vector.GetVectorFromPoints(pt1, pt2).IsOrthogonal(normalVector);
		}

		public void Print()
		{
			string tmp = "";
			for (int i = 0; i < dimensions; i++)
			{
				if (i != 0)
				{
					tmp += " + ";
				}
				tmp += normalVector.GetElement(i) + "v" + (i + 1);
			}
			tmp += " = " + k;
		}
	}
}
