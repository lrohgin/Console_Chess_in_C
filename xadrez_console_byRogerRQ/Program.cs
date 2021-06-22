using System;
using tabuleiro;
using xadrez;

namespace xadrez_console_byRogerRQ
{
    class Program
    {
        static void Main(string[] args)
        {
            try {

            PartidaDeXadrez partida = new PartidaDeXadrez();
            
                while(!partida.terminada) {
                    Console.Clear();
                    Tela.imprimirTabuleiro(partida.tab);

                    Console.WriteLine();
                    Console.Write("Origem: ");
                    Posicao origem = Tela.lerPosicaoXadrez().toPosicao();
                    
                    bool[,] posicoesPossiveis = partida.tab.peca(origem).movimentosPossiveis();
                    Console.Clear();
                    Tela.imprimirTabuleiro(partida.tab, posicoesPossiveis);

                    Console.WriteLine();
                    Console.Write("Destino: ");
                    Posicao destino = Tela.lerPosicaoXadrez().toPosicao();

                    partida.executaMovimento(origem, destino);
                }

            
            Console.ReadLine();
            }
            catch(TabuleiroException e) {
                Console.WriteLine(e.Message);
            }
        }
    }
}
