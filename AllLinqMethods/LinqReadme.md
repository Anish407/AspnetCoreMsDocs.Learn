### https://learn.microsoft.com/en-us/dotnet/framework/data/adonet/ef/language-reference/query-execution#deferred-query-execution

## Query Execution
<p>After a LINQ query is created by a user, it is converted to a command tree. A command tree is a representation of a query that is compatible with the Entity Framework. The command tree is then executed against the data source. At query execution time, all query expressions (that is, all components of the query) are evaluated, including those expressions that are used in result materialization.

At what point query expressions are executed can vary. LINQ queries are always executed when the query variable is iterated over, not when the query variable is created. This is called deferred execution. You can also force a query to execute immediately, which is useful for caching query results. This is described later in this topic.

When a LINQ to Entities query is executed, some expressions in the query might be executed on the server and some parts might be executed locally on the client. Client-side evaluation of an expression takes place before the query is executed on the server. If an expression is evaluated on the client, the result of that evaluation is substituted for the expression in the query, and the query is then executed on the server. Because queries are executed on the data source, the data source configuration overrides the behavior specified in the client. For example, null value handling and numerical precision depend on the server settings. Any exceptions thrown during query
execution on the server are passed directly up to the client.</p>

## Deferred query execution
<p>In a query that returns a sequence of values, the query variable itself never
holds the query results and only stores the query commands.
Execution of the query is deferred until the query variable is iterated over in a foreach or
For Each loop. This is known as deferred execution; that is, query execution occurs some time after the query is constructed.
**This means that you can execute a query as frequently as you want to. This is useful when, for example, you have a database that is being updated by other applications. In your application, you can create a query to retrieve the latest information and repeatedly execute the query, returning the updated information every time.**

#### If we call .ToList twice on a deffered execution operator, then it will go to the database twice, if that is not intended, then call Save the result of .ToList and use it in the later steps.

Deferred execution enables multiple queries to be combined or a query to be extended. When a query is extended, it is modified to include the new operations, and the eventual execution will reflect the changes.</p>

## Immediate Query Execution
In contrast to the deferred execution of queries that produce a sequence of values,
queries that return a singleton value are executed immediately. Some examples of singleton queries are Average, Count, First, and Max. These execute immediately because the query must produce a sequence to calculate the singleton result. You can also force immediate execution. This is useful when you want to cache the results of a query. To force immediate execution of a query that does not produce a singleton value, you can call the ToList method, 
\the ToDictionary method, or the ToArray method on a query or query variable. 



### Literals and Parameters
Local variables, such as the orderID variable in the following example, are evaluated on the client.
~~~  
int orderID = 51987;
IQueryable<SalesOrderHeader> salesInfo =
     from s in context.SalesOrderHeaders
     where s.SalesOrderID == orderID
     select s;
~~~

Method parameters are also evaluated on the client. The orderID parameter passed into the MethodParameterExample method, below, is an example.
~~~
public static void MethodParameterExample(int orderID)
{
    using (AdventureWorksEntities context = new AdventureWorksEntities())
    {

        IQueryable<SalesOrderHeader> salesInfo =
            from s in context.SalesOrderHeaders
            where s.SalesOrderID == orderID
            select s;

        foreach (SalesOrderHeader sale in salesInfo)
        {
            Console.WriteLine("OrderID: {0}, Total due: {1}", sale.SalesOrderID, sale.TotalDue);
        }
    }
}
~~~
