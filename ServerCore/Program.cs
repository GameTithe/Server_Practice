using ServerCore;

class Progam
{

    static ThreadLocal<string> ThreadName = new ThreadLocal<string>(() => { return $"My name is {Thread.CurrentThread.ManagedThreadId}"; });
    //static string ThreadName;

    static void WhoAmI()
    {
        bool repeat = ThreadName.IsValueCreated;
        if(repeat)
        {
            Console.WriteLine(ThreadName.Value + "(repeat)");
        }
        else
        {
            Console.WriteLine(ThreadName.Value);
        }

    }

    static void Main(string[] args)
    {
        ThreadPool.SetMinThreads(1, 1);
        ThreadPool.SetMaxThreads(3, 3);

        Parallel.Invoke(WhoAmI, WhoAmI, WhoAmI, WhoAmI, WhoAmI, WhoAmI, WhoAmI);

        ThreadName.Dispose();

    }
}
