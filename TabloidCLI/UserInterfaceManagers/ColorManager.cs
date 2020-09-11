using System;

namespace TabloidCLI.UserInterfaceManagers
{
    class ColorManager : IUserInterfaceManager
    {
        private readonly IUserInterfaceManager _parentUI;

        public ColorManager(IUserInterfaceManager parentUI)
        {
            _parentUI = parentUI;
        }

        public IUserInterfaceManager Execute()
        {
            Console.Clear();
            Console.WriteLine("Color Menu");
            Console.WriteLine($"Current color is {Console.BackgroundColor}");
            Console.WriteLine("Choose a new background color from below:");

            Console.WriteLine(" 1) Black");
            Console.WriteLine(" 2) Blue");
            Console.WriteLine(" 3) Red");
            Console.WriteLine(" 4) Green");
            Console.WriteLine(" 5) Yellow");
            Console.WriteLine(" 0) Return to Main Menu");

            Console.Write("> ");
            string choice = Console.ReadLine();
            switch (choice)
            {
                case "1":
                    Console.BackgroundColor = ConsoleColor.Black;
                    return this;
                case "2":
                    Console.BackgroundColor = ConsoleColor.Blue;
                    return this;
                case "3":
                    Console.BackgroundColor = ConsoleColor.DarkRed;
                    return this;
                case "4":
                    Console.BackgroundColor = ConsoleColor.DarkGreen;
                    return this;
                case "5":
                    Console.BackgroundColor = ConsoleColor.DarkYellow;
                    return this;
                case "0":
                    Console.Clear();
                    return _parentUI;
                default:
                    Console.WriteLine("Invalid Selection");
                    return this;
            }
        }
    }
}
