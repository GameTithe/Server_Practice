class Progam
{
    static int number = 0;
    static object _obj = new object();

    static void Thread_1()
    {
        // atomic = 원자성

        for (int i = 0; i < 100000; i++)
        {
            // 상호배제 Mutual Exclusive 

            lock(_obj)
            {
                number++;
            }

            //try
            //{

            //    Monitor.Enter(_obj); // 문을 잠구는 행위
            //    number++;

            //    return;
            //}
            //finally
            //{
            //    Monitor.Exit(_obj);
                 
            //}

        }
        
    }

    // 데드락 DeadLock
    static void Thread_2()
    {
        for (int i = 0; i < 100000; i++)
        {
            lock(_obj)
            {
                number--;
            }
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