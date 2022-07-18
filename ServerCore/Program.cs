// See https://aka.ms/new-console-template for more information
//Console.WriteLine("Hello, World!");

class Progam
{
    static void MainThread(object state)
    {
        for (int i = 0; i  < 5; i++)
            Console.WriteLine("Hello Thread!");
    }

    static void Main(string[] args)
    {
        //commit
        //background
        //인력사무소
        ThreadPool.SetMinThreads(1, 1);
        ThreadPool.SetMaxThreads(5, 5);

        for(int i = 0; i < 4; i++)
        {
            ThreadPool.QueueUserWorkItem((obj) => { while (true) { } });
        }

        ThreadPool.QueueUserWorkItem(MainThread);
        
        //Thread는 갯수제한 X 하지만 많다고 좋은 것은 X 최악의 방법이 될 수 있다
        //for (int i = 0; i < 1000; i++)
        //{
        //    Thread t = new Thread(MainThread);
        //    //t.Name = "Test Thread";
        //    t.IsBackground = true;
        //    t.Start();
        //}
        //Console.WriteLine("Waiting for Thread!");

        //t.Join();
        //Console.WriteLine("Hello Wolrd");
        while (true)
        {

        }
    }
}