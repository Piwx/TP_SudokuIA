﻿using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using GeneticSharp.Domain.Chromosomes;
using GeneticSharp.Domain.Fitnesses;
using NoyauTP;

namespace Solver_algo_genetic
{
    /// <summary>
    /// Evaluates a sudoku chromosome for completion by counting duplicates in rows, columns, boxes, and differences from the target mask
    /// </summary>
    public class SudokuFitness : IFitness
    {
		/// <summary>
		/// The target Sudoku Mask to solve.
		/// </summary>
		private readonly Sudoku _targetSudokuBoard;

		public SudokuFitness(Sudoku targetSudokuBoard)
		{
			_targetSudokuBoard = targetSudokuBoard;
		}

		/// <summary>
		/// Evaluates a chromosome according to the IFitness interface. Simply reroutes to a typed version.
		/// </summary>
		/// <param name="chromosome"></param>
		/// <returns></returns>
		public double Evaluate(IChromosome chromosome)
		{
			return Evaluate((SudokuCellsChromosome)chromosome);
		}

		/// <summary>
		/// Evaluates a ISudokuChromosome by summing over the fitnesses of its corresponding Sudoku boards.
		/// </summary>
		/// <param name="chromosome">a Chromosome that can build Sudokus</param>
		/// <returns>the chromosome's fitness</returns>
		public double Evaluate(SudokuCellsChromosome chromosome)
		{
			List<double> scores = new List<double>();

			var sudokus = chromosome.GetSudokus();
			foreach (var sudoku in sudokus)
			{
				scores.Add(Evaluate(sudoku));
			}

			return scores.Sum();
		}

		/// <summary>
		/// Evaluates a single Sudoku board by counting the duplicates in rows, boxes
		/// and the digits differing from the target mask.
		/// </summary>
		/// <param name="testSudokuBoard">the board to evaluate</param>
		/// <returns>the number of mistakes the Sudoku contains.</returns>
		public double Evaluate(Sudoku testSudokuBoard)
		{
			// We use a large lambda expression to count duplicates in rows, columns and boxes
			var targetCells = _targetSudokuBoard.Cells;
			var cells = testSudokuBoard.Cells.Select((c, i) => new { index = i, cell = c }).ToList();
			var toTest = cells.GroupBy(x => x.index / 9).Select(g => g.Select(c => c.cell)) // rows
			  .Concat(cells.GroupBy(x => x.index % 9).Select(g => g.Select(c => c.cell))) //columns
			  .Concat(cells.GroupBy(x => x.index / 27 * 27 + x.index % 9 / 3 * 3).Select(g => g.Select(c => c.cell))); //boxes
			var toReturn = -toTest.Sum(test => test.GroupBy(x => x).Select(g => g.Count() - 1).Sum()); // Summing over duplicates
			toReturn -= cells.Count(x => targetCells[x.index] > 0 && targetCells[x.index] != x.cell); // Mask
			return toReturn;
		}



	}



}