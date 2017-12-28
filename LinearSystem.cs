using System;
using System.Diagnostics;
namespace Simple_LA
{
	public class LinearSystem
	{
		private LinearEquation[] eqs;
		private Vector[] eqMatrix;
		private double[] coefficientMatrix;
		public readonly int dimensions;
		private Parametrization parametrization;

		public LinearSystem(LinearEquation[] eqs)
		{
			this.eqs = eqs;
			dimensions = eqs[0].normalVector.dimensions;
			eqMatrix = new Vector[eqs.Length];
			coefficientMatrix = new double[eqs.Length];
			for (int i = 0; i < eqs.Length; i++)
			{
				eqMatrix[i] = eqs[i].normalVector;
				coefficientMatrix[i] = eqs[i].k;
			}

		}



		/// <summary>
		/// True if at least one solution exists for the system.
		/// </summary>
		public bool GaussianElimination()
		{
			PrintSystem();
			ReducedRowEchelonForm();
			PrintSystem();


			if (!CheckSolutions())
			{
				return false;
			}

			return true;
		}



		/// <summary>
		/// Returns true if a solution exists for the system.
		/// </summary>
		private bool CheckSolutions()
		{
			if (!IsConsistent())
			{
				Debug.WriteLine("No solution exists for the system.");
				return false;
			}
			else if (InfinitSolutions())
			{
				Debug.WriteLine("Infinitely many solutions exist for the system.");
				parametrization = new Parametrization(eqMatrix, coefficientMatrix);
				parametrization.Print();
			}
			else
			{
				Debug.WriteLine("One solution exists for the system.");
			}

			return true;
		}


		private bool InfinitSolutions()
		{
			if (!IsConsistent())
			{
				return false;
			}

			for (int i = 0; i < eqMatrix.Length; i++)
			{
				bool pivotVariableFound = false;
				for (int variable = 0; variable < dimensions; variable++)
				{
					if (!eqMatrix[i].EqualsInt(0, variable))
					{
						if (!pivotVariableFound)
						{
							pivotVariableFound = true;
						}
						else
						{
							return true;
						}
					}
				}
			}

			return false;

		}

		private bool IsConsistent()
		{
			for (int i = 0; i < eqMatrix.Length; i++)
			{
				for (int variable = 0; variable < dimensions; variable++)
				{
					if (!eqMatrix[i].EqualsInt(0, variable))
					{
						break;
					}
					else if (variable == dimensions - 1)
					{
						if (coefficientMatrix[i] > 0 + Vector.DEFAULT_TOLERANCE
					  		|| coefficientMatrix[i] < 0 - Vector.DEFAULT_TOLERANCE)
						{
							return false;
						}
					}
				}
			}
			return true;
		}


		private void TriangularForm()
		{
			int row = 0;
			int variable = 0;
			while (variable < dimensions && row < eqMatrix.Length)
			{
				//FIND THE NEXT PIVOT VARIABLE
				FindNextPivotVariableRow(ref row, ref variable, true);

				//ELIMINATE PIVOT VARIABLE IN ALL ROWS BENEATH CURRENT
				EliminateVariableBelow(row, variable);

				//MOVE TO NEXT ROW AND VARIABLE
				row++;
				variable++;
			}
		}



		private void ReducedRowEchelonForm()
		{
			TriangularForm();

			int row = 0;
			int variable = 0;
			while (FindNextPivotVariable(ref row, ref variable))
			{
				//ELIMINATE PIVOT VARIABLE IN ALL ROWS ABOVE CURRENT
				EliminateVariableAbove(row, variable);

				//REDUCE THE CURRENT PIVOT VARIABLE TO ONE
				ReducePivotVariableToOne(row, variable);

				//MOVE TO NEXT ROW AND VARIABLE
				row++;
				variable++;
			}
		}


		private void ReducePivotVariableToOne(int row, int variable)
		{
			MultiplyRowByScalar(1 / eqMatrix[row].GetElement(variable), row);
		}



		/// <summary>
		/// Finds the index of the next pivot variable. Returns false if there are no more pivot variables.
		/// </summary>
		private bool FindNextPivotVariable(ref int row, ref int variable)
		{
			if (row < eqMatrix.Length && variable < dimensions)
			{
				int variableStart = variable;
				while (true)
				{
					if (!eqMatrix[row].EqualsInt(0, variable))
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
						if (row >= eqMatrix.Length)
						{
							break;
						}
					}
				}
			}

			return false;

		}



		/// <summary>
		/// Finds the row with the next pivot variable, and swaps with current row if swapRows == true.
		/// </summary>
		private void FindNextPivotVariableRow(ref int row, ref int variable, bool swapRows)
		{
			if (eqMatrix[row].EqualsInt(0, variable))
			{
				for (int j = 1, i = row + j; i < eqMatrix.Length && variable < dimensions; i++)
				{
					if (!eqMatrix[i].EqualsInt(0, variable))
					{
						if (swapRows)
						{
							SwapRows(row, i);
						}
						break;
					}
					else if (i == eqMatrix.Length - 1)
					{
						if (variable < dimensions - 1)
						{
							j++;
							i = row + j;
							variable++;
						}
						else
						{
							break;
						}
					}
				}
			}
		}


		/// <summary>
		/// All instances of variable in rows above the current will be reduced to zero.
		/// </summary>
		private void EliminateVariableAbove(int row, int variable)
		{
			for (int i = row - 1; i >= 0; i--)
			{
				EliminateVariableInRow(variable, row, i);
			}
		}


		/// <summary>
		/// Reduces the variable in row2 to zero by adding a multiple of row1 into row 2.
		/// </summary>
		private void EliminateVariableInRow(int variable, int row1, int row2)
		{
			if (!eqMatrix[row2].EqualsInt(0, variable) && !eqMatrix[row1].EqualsInt(0, variable))
			{
				double scalar = eqMatrix[row2].GetElement(variable) / eqMatrix[row1].GetElement(variable);
				AddMultipleOfRowIntoRow(-scalar, row1, row2);
			}
		}


		/// <summary>
		/// All instances of variable in rows below the current will be reduced to zero.
		/// </summary>
		private void EliminateVariableBelow(int row, int variable)
		{
			for (int i = row + 1; i < eqMatrix.Length; i++)
			{
				EliminateVariableInRow(variable, row, i);
			}
		}


		/// <summary>
		/// R1 <-> R2
		/// </summary>
		private void SwapRows(int row1, int row2)
		{
			Vector tmpEq = eqMatrix[row1];
			eqMatrix[row1] = eqMatrix[row2];
			eqMatrix[row2] = tmpEq;

			double tmpCoef = coefficientMatrix[row1];
			coefficientMatrix[row1] = coefficientMatrix[row2];
			coefficientMatrix[row2] = tmpCoef;
		}


		/// <summary>
		/// c * R -> R
		/// </summary>
		private void MultiplyRowByScalar(double scalar, int row)
		{
			eqMatrix[row] = eqMatrix[row].ScalarMultiplication(scalar);
			coefficientMatrix[row] *= scalar;
		}


		/// <summary>
		/// cR1 + R2 -> R2
		/// </summary>
		private void AddMultipleOfRowIntoRow(double scalar, int row1, int row2)
		{
			eqMatrix[row2] = eqMatrix[row2].Add(eqMatrix[row1].ScalarMultiplication(scalar));
			coefficientMatrix[row2] += (coefficientMatrix[row1] * scalar);
		}



		public void PrintSystem()
		{
			string tmp = "Linear System:\n";
			for (int i = 0; i < eqs.Length; i++)
			{
				for (int j = 0; j < eqMatrix[0].dimensions + 1; j++)
				{
					if (j < eqMatrix[0].dimensions)
					{
						tmp += " " + eqMatrix[i].GetElement(j) + " ";
					}
					else
					{
						tmp += " " + coefficientMatrix[i] + " ";
					}
				}
				tmp += "\n";
			}
			Debug.WriteLine(tmp);
		}
	}
}
