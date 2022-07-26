﻿// See https://aka.ms/new-console-template for more information
//Console.WriteLine("Hello, World!");

class Progam
{
    volatile static bool _stop = false;

    static void ThreadMain()
    {
        Console.WriteLine("쓰레드 시작!");

        while(_stop == false)
        {
            //누군가가 stop 신호를 해주기를 기다린다
        }

        Console.WriteLine("쓰레드 종료!");
    }   

    static void Main(string[] args)
    {
        Task t = new Task(ThreadMain);
        t.Start();

        Thread.Sleep(1000);

        _stop = true;

        Console.WriteLine("Stop 호출");
        Console.WriteLine("종료 대기 중");

        //Thread q ;  q.Join()  같은 의미
        t.Wait();

        Console.WriteLine("종료 성공");
    }
}