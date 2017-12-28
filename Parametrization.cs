using System;
using System.Diagnostics;
using System.Collections.Generic;
namespace Simple_LA
{
	public class Parametrization
	{
		public readonly int dimensions;
		private Vector basePoint;
		private int[] directionVectorsNumbers;
		private Vector[] directionVectors;

		public Parametrization(Vector[] reducedRowEchelon, double[] coefficientMatrix)
		{
			dimensions = reducedRowEchelon[0].dimensions;
			SetBasePoint(reducedRowEchelon, coefficientMatrix);
			SetDirectionVectors(reducedRowEchelon);
		}


		private void SetBasePoint(Vector[] reducedRowEchelon, double[] coefficientMatrix)
		{
			double[,] tmp = new Double[dimensions,1];
			int row = 0;
			int variable = 0;
			while (FindNextPivotVariable(ref row, ref variable, reducedRowEchelon))
			{
				tmp[variable,0] = coefficientMatrix[row];

				//MOVE TO NEXT ROW AND VARIABLE
				row++;
				variable++;
			}
			basePoint = new Vector(tmp, Vector.DEFAULT_TOLERANCE);
		}



		private void SetDirectionVectors(Vector[] reducedRowEchelon)
		{
			reducedRowEchelon = RearrangeReducedRowEchelon(reducedRowEchelon);
			List<Vector> tmp = new List<Vector>();
			List<int> tmpVarNums = new List<int>();
			for (int i = 0; i < dimensions; i++)
			{
				if (basePoint.EqualsInt(0, i))
				{
					double[,] tmpElements = new double[dimensions,1];
					for (int j = 0; j < dimensions; j++)
					{
						tmpElements[j, 0] = -reducedRowEchelon[j].GetElement(i);
					}
					tmpElements[i, 0] = 1;
					tmp.Add(new Vector(tmpElements, Vector.DEFAULT_TOLERANCE));
					tmpVarNums.Add(i + 1);
				}
			}
			directionVectors = tmp.ToArray();
			directionVectorsNumbers = tmpVarNums.ToArray();
		}



		private Vector[] RearrangeReducedRowEchelon(Vector[] reducedRowEchelon)
		{
			List<Vector> tmp = new List<Vector>();
			int i = 0;
			int row = 0;
			int variable = 0;
			while (FindNextPivotVariable(ref row, ref variable, reducedRowEchelon))
			{
				if (row == i)
				{
					tmp.Add(reducedRowEchelon[row]);
				}
				else
				{
					for (; i < row; i++)
					{
						tmp.Add(new Vector(new double[dimensions, 1], Vector.DEFAULT_TOLERANCE));
					}
					tmp.Add(reducedRowEchelon[row]);
					i = variable;
				}

				//MOVE TO NEXT ROW AND VARIABLE
				i++;
				row++;
				variable++;
			}
			if (tmp.Count < dimensions)
			{
				for (i = tmp.Count; i < dimensions; i++)
				{
					tmp.Add(new Vector(new double[dimensions, 1], Vector.DEFAULT_TOLERANCE));
				}
			}

			return tmp.ToArray();

		}


		private void SetDirectionVectorsNumbers()
		{
			for (int i = 0, j = 0; i < directionVectorsNumbers.Length; i++)
			{
				for (int k = j; j < basePoint.dimensions; j++)
				{
					if (basePoint.EqualsInt(0, k))
					{
						directionVectorsNumbers[i] = k + 1;
						j = k;
						break;
					}
				}
			}
		}



		/// <summary>
		/// Finds the index of the next pivot variable. Returns false if there are no more pivot variables.
		/// </summary>
		private bool FindNextPivotVariable(ref int row, ref int variable, Vector[] reducedRowEchelon)
		{
			int dimensions = reducedRowEchelon[0].dimensions;
			if (row < reducedRowEchelon.Length && variable < dimensions)
			{
				int variableStart = variable;
				while (true)
				{
					if (!reducedRowEchelon[row].EqualsInt(0, variable))
					{
						return true;
					}
					else if (variable < dimensions - 1)
					{
						variable++;
					}
					else
					{
						row++;
						variable = variableStart;
						if (row >= reducedRowEchelon.Length)
						{
							break;
						}
					}
				}
			}

			return false;

		}


		public void Print()
		{
			for (int i = 0; i < dimensions; i++)
			{
				String tmp = "";
				tmp += "v" + (i + 1) + " =";
				bool firstVar = true;
				if (!basePoint.EqualsInt(0, i))
				{
					tmp += String.Format(" " + basePoint.GetElement(i));
					firstVar = false;
				}
				for (int j = 0; j < directionVectors.Length; j++)
				{
					if (!directionVectors[j].EqualsInt(-1, i) &&
						!directionVectors[j].EqualsInt(0, i) &&
						!directionVectors[j].EqualsInt(1, i))
					{
						if (!firstVar)
						{
							if (directionVectors[j].GetElement(i) < 0)
							{
								tmp += " - ";
							}
							else
							{
								tmp += " + ";
							}
						}
						else if (directionVectors[j].GetElement(i) < 0)
						{
							tmp += " " + Math.Abs(directionVectors[j].GetElement(i));
						}
						else
						{
							tmp += " " + directionVectors[j].GetElement(i);
						}
					}
					else
					{
						if (!firstVar && !directionVectors[j].EqualsInt(0, i))
						{
							if (directionVectors[j].GetElement(i) < 0)
							{
								tmp += " - ";
							}
							else
							{
								tmp += " + ";
							}
						}
						else if (firstVar && !directionVectors[j].EqualsInt(0, i))
						{
							tmp += " ";
						}
						tmp += "v" + directionVectorsNumbers[j];
					}
				}
				Debug.WriteLine(tmp);
			}
		}


		public void PrintAsVectors()
		{
			String tmp = "";
			for (int i = 0; i < dimensions; i++)
			{
				tmp += String.Format("|v{0:000}|", (i + 1));
				tmp += String.Format("|{0:0.000000}| ", basePoint.GetElement(i));
				bool halfWay = false;
				if ((dimensions % 2 == 1 && (i == (dimensions - 1) / 2)) || (i == dimensions / 2))
				{
					halfWay = true;
					tmp += " = ";
				}
				else
				{
					halfWay = false;
					tmp += "   ";
				}
				for (int j = 0; j < directionVectors.Length; j++)
				{
					if (halfWay)
					{
						tmp += String.Format("v{0:000}", directionVectorsNumbers[j]);
						tmp += String.Format("|{0:0.000000}| ", directionVectors[j].GetElement(i));
						tmp += " + ";
					}
					else
					{
						tmp += "    ";
						tmp += String.Format("|{0:0.000000}| ", directionVectors[j].GetElement(i));
						tmp += "   ";
					}
				}
				tmp += "\n";
			}
			Debug.WriteLine(tmp);
		}
	}
}
