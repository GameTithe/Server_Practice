// See https://aka.ms/new-console-template for more information
//Console.WriteLine("Hello, World!");

class Progam
{
    int _answer;
    bool _complete;
     
    void A()
    {
        _answer = 123;
        Thread.MemoryBarrier();
        _complete = true;
        Thread.MemoryBarrier();
    }     
    void B()
    {
        Thread.MemoryBarrier();
        if(_complete)
        {
            Thread.MemoryBarrier();
            Console.WriteLine(_answer);
        }
    }
    static void Main(string[] args)
    {
    }
}