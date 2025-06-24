#:sdk Microsoft.NET.Sdk
#:property LangVersion preview

#region Null assignment
// This code demonstrates the use of null assignment and null-conditional operators in C# 14
Customer? customer = null;
customer?.OrderId = "12345";
Console.WriteLine($"orderId in customer null:{customer?.OrderId}");
#endregion


#region Extension methods example
int[] list = [ 1, 3, 5, 7, 9 ];
var x1 = list.ValuesLessThan(7);
var x2 = MyExtensions.ValuesLessThan(list,  7);
Console.WriteLine($"values < 7: {string.Join(", ", x1)}");
Console.WriteLine($"values < 7: {string.Join(", ", x2)}");
var x3 = list.ValuesGreaterThan(0);
Console.WriteLine($"values > 0: {string.Join(", ", x3)}");
var x4 = list.ValuesGreaterThan(3);
Console.WriteLine($"values > 3: {string.Join(", ", x4)}");
#endregion


class Customer
{
    public string? OrderId { get; set; }
    public string? Name { get; set; }
}

#region Simple class Field
class SimpleField
{
    public string Message
    {
        get;
        set => field = value ?? throw new ArgumentNullException(nameof(value));
    }
}
#endregion

#region Extensions
public static class MyExtensions
{
    public static IEnumerable<int> ValuesLessThan(this IEnumerable<int> source, int threshold)
            => source.Where(x => x < threshold);

    extension(IEnumerable<int> source)
    {
        public IEnumerable<int> ValuesGreaterThan(int threshold)
            => source.Where(x => x > threshold);

        public IEnumerable<int> ValuesGreaterThanZero
            => source.ValuesGreaterThan(0);
    }
}
#endregion