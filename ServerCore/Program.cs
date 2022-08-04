class Progam
{
    static int number = 0;

    static void Thread_1()
    {
        // atomic = 원자성

        for (int i = 0; i < 100000; i++)
        {
            // All or Nothing  Interloacked들이 동시다발적으로 일어난다고해도 승자는 생긴다 
            int afterValue = Interlocked.Increment(ref number);
            /*
            number++;

            int temp = number; // 0
            temp += 1; // 1
            number = temp; // 1
            */

        }
    }
    static void Thread_2()
    {
        for (int i = 0; i < 100000; i++)
        { 
            Interlocked.Decrement(ref number);
            /*
            number--;

            int temp = number; // 0
            temp += 1; // -1
            number = temp; // -1
            */
        }
    }   
    static void Main(string[] args)
    {
        Task t1 = new Task(Thread_1);
        Task t2 = new Task(Thread_2);

        t1.Start();
        t2.Start();

        Task.WaitAll(t1, t2);

        Console.WriteLine(number);
    }
}