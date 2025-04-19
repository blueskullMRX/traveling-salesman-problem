using System;

public class Program
{
    public static void Main(string[] args)
    {
        Tabusearch tabusearch = new Tabusearch();

        tabusearch.initialsolution = new int[5];
        tabusearch.initialsolution = tabusearch.generaterandomsolution();
        tabusearch.distanceMatrix = new int[][]
        {
            new int[] { 0, 355, 70, 258 ,240 },
            new int[] { 355, 0, 280, 88,600 },
            new int[] { 70, 280, 0,166,310 },
            new int[] { 258, 88, 166, 0,496 },
            new int[] { 240, 600, 310, 496,0 }
        };
        
        tabusearch.tabulist = new List<(int, int)>(); 

        int bestDistance = tabusearch.TabuSearch(15); 
        Console.WriteLine("Best Distance: " + bestDistance);
        Console.WriteLine("Best Solution: " + string.Join(" -> ", tabusearch.bestsolution));
    }
}

public class Tabusearch
{
    public List<(int, int)> tabulist; // Tabu list to store recently swapped city pairs
    public int[][] distanceMatrix;
    public int[] initialsolution;
    public int[] bestsolution;
    public int[][] swapcombination;

    public int calculatetripdistance(int[] solution)
    {
        int distance = 0;
        for (int i = 0; i < solution.Length - 1; i++)
        {
            distance += distanceMatrix[solution[i]][solution[i + 1]];
        }
        distance += distanceMatrix[solution[solution.Length - 1]][solution[0]]; // Return to the first city
        return distance;
    }

    public void swap(int[] solution, int i, int j)
    {
        int temp = solution[i];
        solution[i] = solution[j];
        solution[j] = temp;
    }

    public int TabuSearch(int maxIterations)
    {
        int[] currentSolution = new int[initialsolution.Length];
        Array.Copy(initialsolution, currentSolution, initialsolution.Length);
        bestsolution = new int[initialsolution.Length];
        Array.Copy(initialsolution, bestsolution, initialsolution.Length);

        int bestDistance = calculatetripdistance(currentSolution);
        Console.WriteLine("Initial solution: " + string.Join(" -> ", currentSolution));
        Console.WriteLine("Initial distance: " + bestDistance);

        Random random = new Random();
        for (int iteration = 0; iteration < maxIterations; iteration++)
        {
            int[] bestNeighbor = null;
            int bestNeighborDistance = int.MaxValue;
            int bestI = -1, bestJ = -1;

            // Generate neighbors by swapping cities
            for (int i = 0; i < currentSolution.Length - 1; i++)
            {
                for (int j = i + 1; j < currentSolution.Length; j++)
                {
                    if (tabulist.Contains((i, j)) || tabulist.Contains((j, i)))
                        continue; // Skip swaps in the tabu list

                    int[] neighbor = new int[currentSolution.Length];
                    Array.Copy(currentSolution, neighbor, currentSolution.Length);
                    swap(neighbor, i, j);

                    int neighborDistance = calculatetripdistance(neighbor);
                    if (neighborDistance < bestNeighborDistance)
                    {
                        bestNeighbor = neighbor;
                        bestNeighborDistance = neighborDistance;
                        bestI = i;
                        bestJ = j;
                    }
                }
            }

            if (bestNeighbor != null)
            {
                // Debug information
                Console.WriteLine($"Iteration {iteration + 1}: Swapping positions {bestI} and {bestJ} with values {currentSolution[bestI]} and {currentSolution[bestJ]}");

                Array.Copy(bestNeighbor, currentSolution, currentSolution.Length);

                // Add the swap to the tabu list (using positions, not values)
                tabulist.Add((bestI, bestJ));
                if (tabulist.Count > 10) // Limit the size of the tabu list
                {
                    tabulist.RemoveAt(0);
                }

                // Update the best solution if the neighbor is better
                if (bestNeighborDistance < bestDistance)
                {
                    bestDistance = bestNeighborDistance;
                    Array.Copy(bestNeighbor, bestsolution, bestsolution.Length);
                    Console.WriteLine("Found better solution!");
                }
            }
            else
            {
                // If we can't find a non-tabu move, perform a random move to escape local optima
                int i = random.Next(0, currentSolution.Length);
                int j = random.Next(0, currentSolution.Length);
                while (i == j) j = random.Next(0, currentSolution.Length);

                Console.WriteLine($"Iteration {iteration + 1}: No non-tabu moves, performing random swap of {i} and {j}");
                swap(currentSolution, i, j);
                tabulist.Add((i, j));
                if (tabulist.Count > 10) tabulist.RemoveAt(0);

                int newDistance = calculatetripdistance(currentSolution);
                if (newDistance < bestDistance)
                {
                    bestDistance = newDistance;
                    Array.Copy(currentSolution, bestsolution, currentSolution.Length);
                    Console.WriteLine("Random move found better solution!");
                }
            }

            // Add Console.WriteLine statements to log the best distance and solution for each iteration
            Console.WriteLine($"Current solution: {string.Join(" -> ", currentSolution)}");
            Console.WriteLine($"Current distance: {calculatetripdistance(currentSolution)}");
            Console.WriteLine($"Best Distance: {bestDistance}");
            Console.WriteLine($"Best Solution: {string.Join(" -> ", bestsolution)}");
            Console.WriteLine($"Tabu list contains {tabulist.Count} entries");
            Console.WriteLine("-------------------");
        }

        return bestDistance;
    }

    public int[] generaterandomsolution()
    {
        Random random = new Random();
        int[] solution = new int[initialsolution.Length];
        for (int i = 0; i < initialsolution.Length; i++)
        {
            solution[i] = i;
        }
        for (int i = 0; i < solution.Length; i++)


        {
            int j = random.Next(i, solution.Length);
            swap(solution, i, j);
        }
        return solution;
    }

}
