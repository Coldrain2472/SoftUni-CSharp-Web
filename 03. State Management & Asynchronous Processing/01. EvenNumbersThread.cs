 int start = int.Parse(Console.ReadLine());
 int end = int.Parse(Console.ReadLine());

 Thread evenNumbers = new Thread(() => PrintEvenNumbers(start, end));

 evenNumbers.Start();
 evenNumbers.Join();
 
 Console.WriteLine("Thread finished work");

 static void PrintEvenNumbers(int start, int end)
 {
     for (int i = start; i <= end; i++)
     {
         if (i % 2 == 0)
         {
             Console.WriteLine(i);
         }
     }
 }