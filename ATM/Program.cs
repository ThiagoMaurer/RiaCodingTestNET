public class ATM
{
    private static readonly int[] Denominations = { 100, 50, 10 };

    public static void Main(string[] args)
    {
        while (true)
        {
            string? input = null;

            Console.WriteLine("Enter the payout amount (or 'q' to quit):");

            while (string.IsNullOrEmpty(input))
                input = Console.ReadLine();

            if (input.ToLower() == "q")
                break;

            if (!int.TryParse(input, out int amount))
            {
                Console.WriteLine("Invalid input. Please enter a valid integer amount.");
                continue;
            }

            List<string> combinations = GetCombinations(amount, 0);

            Console.WriteLine();
            if (combinations.Count == 0)
            {
                Console.WriteLine("No combinations found.");
            }
            else
            {
                if (combinations.Count == 1)
                    Console.WriteLine($"There is {combinations.Count} possible combination to reach {input} EUR. Check it below: ");
                else
                    Console.WriteLine($"There are {combinations.Count} possible combinations to reach {input} EUR. Check them below: ");

                combinations.ForEach(x => Console.WriteLine(" > " + x));
            }

            Console.WriteLine();
            Console.WriteLine();
        }

        Console.WriteLine("Program ended. Press any key to exit.");
        Console.ReadKey();
    }

    private static List<string> GetCombinations(int amount, int denominationsIndex)
    {
        List<string> result = new List<string>();

        if (denominationsIndex >= Denominations.Length)
        {
            return result;
        }

        int currentDenomination = Denominations[denominationsIndex];
        int maxCount = amount / currentDenomination;

        for (int count = 1; count <= maxCount; count++)
        {
            int total = count * currentDenomination;

            if (total == amount)
            {
                result.Add("(" + count + " x " + currentDenomination + ")");
            }
            else if (total < amount)
            {
                List<string> subCombinations = GetCombinations(amount - total, denominationsIndex + 1);
                foreach (string subCombo in subCombinations)
                {
                    result.Add("(" + count + " x " + currentDenomination + ") " + subCombo);
                }
            }
        }

        List<string> combinationsWithoutCurrentDenomination = GetCombinations(amount, denominationsIndex + 1);
        result.AddRange(combinationsWithoutCurrentDenomination);

        return result;
    }
}