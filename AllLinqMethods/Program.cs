using Dumpify; // to print the results in a tabular format

Console.WriteLine("All Linq Methods ");

IEnumerable<string> stringValues = new List<string>() { "Anish", "Jiya", "Jeeba" };
IEnumerable<object> mixedValues = new List<object>() { "Anish", 1, 2, 3, "Jiya", "Jeeba" };


WhereMethod();

OfTypeMethod();

void OfTypeMethod()
{
    mixedValues.OfType<string>().Dump();
    mixedValues.OfType<int>().Dump();

    #region Source Code

    // this is how this method works .. Loop through each item and check if its of type TResult
    // private static IEnumerable<TResult> OfTypeIterator<TResult>(IEnumerable source)
    // {
    //     foreach (object? obj in source)
    //     {
    //         if (obj is TResult result)
    //         {
    //             yield return result;
    //         }
    //     }
    // }

    #endregion
}

void WhereMethod()
{
    stringValues.Where(i => !string.IsNullOrWhiteSpace(i)).Dump();
}

Console.ReadLine();