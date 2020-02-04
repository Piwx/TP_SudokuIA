using System.Linq;
using GeneticSharp.Domain;
using GeneticSharp.Domain.Chromosomes;
using GeneticSharp.Domain.Crossovers;
using GeneticSharp.Domain.Mutations;
using GeneticSharp.Domain.Populations;
using GeneticSharp.Domain.Selections;
using GeneticSharp.Domain.Terminations;
using NoyauTP;


namespace Solver_algo_genetic
{
    public static class SudokuGeneticHelper
    {
		//Function called by the Solver
		public static Sudoku EvolveSudokuSolution( Sudoku sudokuBoard, int populationSize, int generationNb)
        {
            //Create a SudokuCellsChromosome from the current board
			var sudokuChromosome = new SudokuCellsChromosome(sudokuBoard); //Sudoku made of Genes
			var fitness = new SudokuFitness(sudokuBoard);
			var selection = new EliteSelection();
			var crossover = new UniformCrossover();
			var mutation = new UniformMutation();

			var population = new Population(populationSize, populationSize, sudokuChromosome);
			var ga = new GeneticAlgorithm(population, fitness, selection, crossover, mutation)
			{
				Termination = new OrTermination(new ITermination[]
				{
					new FitnessThresholdTermination(0),
					new GenerationNumberTermination(generationNb)
				})
			};

			ga.Start();

            //Choose the best version tested (the actual solution)
			var bestIndividual = ((SudokuCellsChromosome)ga.Population.BestChromosome);
			return bestIndividual.GetSudokus()[0];
			
		}
	}
}
