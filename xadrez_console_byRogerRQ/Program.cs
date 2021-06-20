using System;
using tabuleiro;
using xadrez;

namespace xadrez_console_byRogerRQ
{
    class Program
    {
        static void Main(string[] args)
        {
            PosicaoXadrez pos = new PosicaoXadrez('h', 0);
            Console.WriteLine(pos);

            Console.WriteLine(pos.toPosicao());
        }
    }
}
