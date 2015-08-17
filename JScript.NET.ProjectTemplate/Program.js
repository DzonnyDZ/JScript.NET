import System;

package $safeprojectname${
    class Program{
        static function Main(args: String[])
        {
            Console.WriteLine("Hello World!");
            Console.WriteLine("Press any key to continue...");
            Console.ReadKey();
        }
    }
}

$safeprojectname$.Program.Main(Environment.GetCommandLineArgs());