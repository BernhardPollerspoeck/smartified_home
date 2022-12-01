// See https://aka.ms/new-console-template for more information
using System.Text;

Console.WriteLine("Hello, World!");

var data = File.ReadAllBytes("test.png");

var enc = Convert.ToBase64String(data);

Console.ReadLine();
