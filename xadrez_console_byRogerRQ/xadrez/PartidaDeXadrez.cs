using System.Collections.Generic;
using tabuleiro;

namespace xadrez
{
    class PartidaDeXadrez
    {
        public Tabuleiro tab { get; private set; }

        public int turno;

        public Cor jogadorAtual;

        public bool terminada { get; set; }

        private HashSet<Peca> pecas;

        private HashSet<Peca> capturadas;

        public PartidaDeXadrez()
        {
            tab = new Tabuleiro(8, 8);
            turno = 1;
            jogadorAtual = Cor.Branca;
            terminada = false;
            pecas = new HashSet<Peca>();
            capturadas = new HashSet<Peca>();
            colocarPecas();
        }

        public void executaMovimento(Posicao origem,Posicao destino){
            Peca p = tab.retirarPeca(origem);
            p.incrementarQteMovimentos();
            Peca pecaCapturada = tab.retirarPeca(destino);
            tab.colocarPeca(p, destino);

            if(pecaCapturada != null) {
                capturadas.Add(pecaCapturada);
            }
        }

        public HashSet<Peca> pecasCapturadas(Cor cor)
        {
            HashSet<Peca> aux = new HashSet<Peca>();
            foreach(Peca x in capturadas)
            {
                if(x.cor == cor) {
                    aux.Add(x);
                }
            }
            return aux;
        }

        public HashSet<Peca> pecasEmJogo(Cor cor)
        {
            HashSet<Peca> aux = new HashSet<Peca>();
            foreach ( Peca x in pecas ) {
                if ( x.cor == cor ) {
                    aux.Add(x);
                }
            }
            aux.ExceptWith(pecasCapturadas(cor));
            return aux;
        }



        public void realizaJogada(Posicao origem,Posicao destino)
        {
            executaMovimento(origem,destino);
            turno++;
            mudaJogador();
        }

        private void mudaJogador()
        {
            if(jogadorAtual == Cor.Branca) {
                jogadorAtual = Cor.Preta;
            }
            else {
                jogadorAtual = Cor.Branca;
            }
        }

        public void validarPosicaoDeOrigem(Posicao pos )
        {
            if(tab.peca(pos) == null) {
                throw new TabuleiroException("Não existe peça na posição de origem escolhida!");
            }
            if(jogadorAtual !=tab.peca(pos).cor) {
                throw new TabuleiroException("A peça de origem escolhida não é sua");
            }
            if(!tab.peca(pos).existeMovimentosPossiveis()) {
                throw new TabuleiroException("Não há movimentos possiveis para a peça de origem escolhida");
            };
        }

        public void validarPosicaoDeDestino(Posicao origem,Posicao destino){
            if (!tab.peca(origem).podeMoverPara(destino)) {
                throw new TabuleiroException("Posicao de destino invalida");
            }
        }

        public void colocarNovaPeca(char coluna, int linha, Peca peca)
        {
            tab.colocarPeca(peca,new PosicaoXadrez(coluna,linha).toPosicao());
            pecas.Add(peca);
        }

        private void colocarPecas()  {
            colocarNovaPeca('d',1,new Torre(tab,Cor.Branca));
            colocarNovaPeca('e',1,new Torre(tab,Cor.Branca));
            colocarNovaPeca('f',1,new Torre(tab,Cor.Branca));
            colocarNovaPeca('c',1,new Torre(tab,Cor.Branca));

            colocarNovaPeca('d',8,new Torre(tab,Cor.Preta));
            colocarNovaPeca('e',8,new Torre(tab,Cor.Preta));
            colocarNovaPeca('f',8,new Torre(tab,Cor.Preta));
            colocarNovaPeca('c',8,new Torre(tab,Cor.Preta));
        }
    }
}
