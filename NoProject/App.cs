#:package Humanizer@2.*

using Humanizer;

var date = new DateTime(2024, 1, 1);

Console.WriteLine($"Hola mundo desde {date.Humanize()}"); // Outputs: "Hola mundo desde hace x años/meses"