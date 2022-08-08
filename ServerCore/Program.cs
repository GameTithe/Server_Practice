class Progam
{
    // 1.근성
    // 2.양보
    // 3.갑질

    //상호배제
    //Monitor
    static object _lock = new object();
    static SpinLock _lock2 = new SpinLock();
    //직접 만든다

    // [ ] [ ] [ ] // [ ] [ ] 

    class Reward
    {

    }
    //RWLock ReaderWriteLock
    static ReaderWriterLockSlim _lock3 = new ReaderWriterLockSlim();

    //99.999%
    static Reward GetReward(int id)
    {
        _lock3.EnterReadLock();

        _lock3.ExitReadLock();

      
        return null;
    }

    //0.001%
    void AddReward(Reward reward)
    {
        _lock3.EnterWriteLock();

        _lock3.ExitWriteLock();
    }

    static void Main(string[] args)
    {
        lock (_lock)
        {
        }

        bool lockTaken = false;

        try
        {
            _lock2.Enter(ref lockTaken);
        }
        finally
        {
            if (lockTaken)
                _lock2.Exit();
        }
    }
}
