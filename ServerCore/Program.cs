class SessionManager
{ 
    static object _lock = new object();

    public static void TestSession()
    {
        lock(_lock)
        {

        }
    }

    public static void Test()
    {
        lock(_lock)
        {
            UserManager.TestUser();
        }
    }
}

class UserManager
{
    static object _lock = new object();

    public static void Test()
    {
        lock(_lock)
        {
            SessionManager.TestSession();
        }
    }

    public static void TestUser()
    { 
        lock (_lock)
        {
            
        }
    }
}
class Progam
{
    static int number = 0;
    static object _obj = new object();

    static void Thread_1()
    {
        // atomic = 원자성

        for (int i = 0; i < 10000; i++)
        {
            // 상호배제 Mutual Exclusive 

            SessionManager.Test();
            
        }
        
    }

    // 데드락 DeadLock
    static void Thread_2()
    {
        for (int i = 0; i < 10000; i++)
        {
            UserManager.Test();
            
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