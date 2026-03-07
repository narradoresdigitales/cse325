// See https://aka.ms/new-console-template for more information

var greeting = "Hello, World!";
var currentDate = DateTime.Today;
var christmasDay = new DateTime(currentDate.Year, 12, 25);

Console.WriteLine($" {greeting} ");
Console.WriteLine($"The current date and time is {currentDate}.");

// Calculate the number of days until Christmas
if (currentDate >= christmasDay)
{
    christmasDay = christmasDay.AddYears(1); 
}

var daysUntilChristmas = (christmasDay - currentDate).Days;
Console.WriteLine($"The number of days until Christmas is {daysUntilChristmas}.");
