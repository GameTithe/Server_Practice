class SpinLock
{
    volatile int _locked = 0;

    public void Acquire()
    {
        while (true)
        {
            //int original = Interlocked.Exchange(ref _locked, 1);

            //if (original == 0)
            //    return;

            //CAS Compare-And-Swap
            int expected = 0;
            int desire = 1;
            if( Interlocked.CompareExchange(ref _locked, desire , expected) == expected)
                break;

            //쉬다올게!

            //Thread.Sleep(1); // 무조건 휴식 => 무조건 1ms정도 쉬고 싶어요
            //Thread.Sleep(0); // 조건부 양보 => 나보다 우선순위가 낮은 애들한테는 양보 불가
            Thread.Yield(); // 관대한 양보 => 관대하게 양보할 테니, 지금 실행이 가능한 쓰레드가 있으면 실행하세요 => 실행 가능한 애가 없으면 자신에게 시간을 사용
        }
    }

    public void Release()
    {
        _locked = 0;
    }
}

class Progam
{
    static int _num = 0;
    static SpinLock _lock = new SpinLock();

    static void Thread_1()
    {
        for (int i = 0; i < 1000000; i++)
        {
            _lock.Acquire();
            _num++;
            _lock.Release();

        }
    }   
    static void Thread_2()
    {
        for (int i = 0; i < 1000000; i++)
        {
            _lock.Acquire();
            _num--;
            _lock.Release();

        }
    }
    static void Main(string[] args)
    {
        Task t1 = new Task(Thread_1);   
        Task t2 = new Task(Thread_2);

        t1.Start();
        t2.Start();

        Task.WaitAll(t1, t2);

        Console.WriteLine(_num);
    }
}