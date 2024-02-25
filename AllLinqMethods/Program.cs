using Dumpify; // to print the results in a tabular format

Console.WriteLine("All Linq Methods ");

IEnumerable<string> stringValues = new List<string>() { "Anish", "Jiya", "Jeeba" };
IEnumerable<object> mixedValues = new List<object>() { "Anish", 1, 2, 3, "Jiya", "Jeeba" };
IEnumerable<int> intValues = new List<int>() { 1, 2, 1, 3, 4, 5 };


// WhereMethod();
// OfTypeMethod();
// SkipAndTake(stringValues, intValues);
ChunkAndCast(mixedValues, intValues);

//SelectAndSelectMany(stringValues);


Console.ReadLine();

void OfTypeMethod()
{
    mixedValues.OfType<string>().Dump(); // returns IEnumerable<T>
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


void SkipAndTake(IEnumerable<string> enumerable, IEnumerable<int> listOfInts)
{
    enumerable.Skip(2).Dump("Skip"); // Will skip the first 2 items and return (IEnumerable<T>) the rest -> "Jeeba"   
    enumerable.SkipLast(2)
        .Dump("SkipLast"); //Returns A NEW enumerable collection and skips items from the end based on the count - (IEnumerable<T>) -> │ "Anish"    
    enumerable.Take(1)
        .Dump("Take"); //Returns a specified number of contiguous elements from the start of a sequence - (IEnumerable<T>) -> "Anish"  
    enumerable.TakeLast(1)
        .Dump("TakeLast"); // Returns a new enumerable collection The number of elements to take from the end of the collection.  -> "Jeeba"  


    enumerable.SkipWhile(i => i == "Anish")
        .Dump("SkipWhile"); //Skip items as long as predicate returns true ->  "Jiya" "Jeeba" once the predicate returns false, it just returns the rest of the collection
    enumerable.TakeWhile(i => i == "Anish")
        .Dump("TakeWhile"); //Take items as long as predicate returns true -> "Anish"  

    listOfInts.SkipWhile(i => i == 1)
        .Dump(""); // here it returns  2, 1, 3, 4, 5  because the first item matches the predicate and the second one doesnt, so it returns all the items from the second element
    listOfInts.SkipWhile(i => i == 2)
        .Dump(""); // here it returns the entire list as the first element itself doesnt match the predicate

    listOfInts.TakeWhile(i => i == 2)
        .Dump("Take"); // doesnt return anything as the first item itself doesnt match the predicate, it keeps returning items until it gets a false 

    listOfInts.TakeWhile(i => i <= 1)
        .Dump("Take"); // returns 1 as the 2nd item doesnt match the predicate, so the rest of the items are ignored
}

void SelectAndSelectMany(IEnumerable<string> stringValues1)
{
    stringValues1.Select(i => i.ToUpper()).Dump();
    stringValues1.Select((value, index) => $"Index is{index}: Value=>{value.ToUpper()}")
        .Dump(); //  "Index is0: Value=>ANISH" 


//[[1, 2, 3], [4, 5, 6]]
    IEnumerable<List<int>> nestedList = new List<List<int>> { new List<int>() { 1, 2, 3 }, new List<int>() { 4, 5, 6 } };

// Projects each element of a sequence to an IEnumerable<out T> and flattens the resulting sequences into one sequence
// The return is an IEnumerable that contains a flattened list

    nestedList.SelectMany(i => i).Dump("Plain Select");  

// nestedList.SelectMany(i => i.ToString()).Dump("Incorrect ToString Select");  This wont work as we will use toString on the flattened array and not its item
// So we need to chain it with a select that will read the items from the flattened list

    nestedList.SelectMany(i => i)
        .Select(i => i.ToString()).Dump();

// Check the output to get better understanding
    nestedList.SelectMany((x,i)=> x.Select(v=> $"index: {i}, value {v}"))
        .Dump();

    // "index: 0, value 1"   
    // "index: 0, value 2"   
    // "index: 0, value 3"   
    // "index: 1, value 4"   
    // "index: 1, value 5"   
    // "index: 1, value 6"   
}

void ChunkAndCast(IEnumerable<object> objects, IEnumerable<int> ints)
{
    objects.Cast<string>().Dump("Cast");

    ints.Chunk(4).Dump("Chunk"); // splits the List into a chunk of n(items specified). In this case 2 (Opposite of SelectMany)
}

#region Immediate Execution
// https://learn.microsoft.com/en-us/dotnet/framework/data/adonet/ef/language-reference/query-execution#deferred-query-execution

#endregion


#region Deffered Execution


#endregion
class Sample
{
    public int Age { get; set; }
}