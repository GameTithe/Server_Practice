// See https://aka.ms/new-console-template for more information
//Console.WriteLine("Hello, World!");

class Progam
{
    //메모리 배리어
    // A) 코드 재배치 억제
    // B) 가시성

    static volatile int x = 0;
    static volatile int y = 0;
    static volatile int r1 = 0;
    static volatile int r2 = 0;

    static void Thread_1()
    {
        y = 1;

        // ------------------------
        Thread.MemoryBarrier();

        r1 = x;
    }

    static void Thread_2()
    {
        x = 1;
        // ------------------------
        Thread.MemoryBarrier();
        r2 = y;
    }

    static void Main(string[] args)
    {
        int count = 0;
        while(true)
        {
            count++;
            x = y = r1 = r2 = 0;
            Task t1 = new Task(Thread_1);
            Task t2 = new Task(Thread_2);

            t1.Start();
            t2.Start();

            Task.WaitAll(t1, t2);

            if (r1 == 0 && r2 == 0)
                break;
        }

        Console.WriteLine($"{count} 번 만에 빠져나옴");
    }
}