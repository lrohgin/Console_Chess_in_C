﻿using System.Collections.Generic;
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

        public bool xeque { get; private set; }

        public PartidaDeXadrez()
        {
            tab = new Tabuleiro(8, 8);
            turno = 1;
            jogadorAtual = Cor.Branca;
            terminada = false;
            xeque = false;
            pecas = new HashSet<Peca>();
            capturadas = new HashSet<Peca>();
            colocarPecas();
        }

        public Peca executaMovimento(Posicao origem,Posicao destino){
            Peca p = tab.retirarPeca(origem);
            p.incrementarQteMovimentos();
            Peca pecaCapturada = tab.retirarPeca(destino);
            tab.colocarPeca(p, destino);

            if(pecaCapturada != null) {
                capturadas.Add(pecaCapturada);
            }

            //#jogadaespecial roque pequeno

            if(p is Rei && destino.coluna == origem.coluna +2) {
                Posicao origemT = new Posicao(origem.linha,origem.coluna + 3);
                Posicao destinoT = new Posicao(origem.linha,origem.coluna + 1);
                Peca T = tab.retirarPeca(origemT);
                T.incrementarQteMovimentos();
                tab.colocarPeca(T,destinoT);
            }

            //#jogadaespecial roque grande

            if ( p is Rei && destino.coluna == origem.coluna - 2 ) {
                Posicao origemT = new Posicao(origem.linha,origem.coluna - 4);
                Posicao destinoT = new Posicao(origem.linha,origem.coluna - 1);
                Peca T = tab.retirarPeca(origemT);
                T.incrementarQteMovimentos();
                tab.colocarPeca(T,destinoT);
            }

            return pecaCapturada;
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

        private Cor adversaria(Cor cor )
        {
            if(cor == Cor.Branca ) {
                return Cor.Preta;
            }
            else {
                return Cor.Branca;
            }
        }

        private Peca rei(Cor cor)
        {
            foreach ( Peca x in pecasEmJogo(cor) ) {
                if(x is Rei) {
                    return x;
                }
            }
            return null;
        }

        public bool estaEmXeque(Cor cor)
        {
            Peca R = rei(cor);
            if (R == null ) {
                throw new TabuleiroException("Nao tem rei da cor" + cor + " no tabuleiro");
            }

            foreach(Peca x in pecasEmJogo(adversaria(cor))) {
                bool[,] mat = x.movimentosPossiveis();
                if(mat[R.posicao.linha,R.posicao.coluna]) {
                    return true;
                }  
            }
            return false;
        }

        public bool testeXequemate(Cor cor )
        {
            if(!estaEmXeque(cor)) {
                return false;
            }
            foreach(Peca x in pecasEmJogo(cor)) {
                bool[,] mat = x.movimentosPossiveis();
                for(int i=0 ;i<tab.linhas ;i++) {
                    for(int j=0 ; j<tab.colunas ;j++ ) {
                        if(mat[i,j]) {
                            Posicao origem = x.posicao;
                            Posicao destino = new Posicao(i,j);
                            Peca pecaCapturada = executaMovimento(origem,destino);
                            bool testeXeque = estaEmXeque(cor);
                            desfazMovimento(origem,destino,pecaCapturada);
                            if(!testeXeque) {
                                return false;
                            }
                        } 
                
                    }
                }
            }
            return true;
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

        public void desfazMovimento(Posicao origem,Posicao destino,Peca pecaCapturada)
        {
            Peca p = tab.retirarPeca(destino);
            p.decrementarQteMovimentos();
            if(pecaCapturada != null) {
                tab.colocarPeca(pecaCapturada,destino);
                capturadas.Remove(pecaCapturada);
            }
            tab.colocarPeca(p,origem);

            //#jogadaespecial roque pequeno

            if ( p is Rei && destino.coluna == origem.coluna + 2 ) {
                Posicao origemT = new Posicao(origem.linha,origem.coluna + 3);
                Posicao destinoT = new Posicao(origem.linha,origem.coluna + 1);
                Peca T = tab.retirarPeca(destinoT);
                T.decrementarQteMovimentos();
                tab.colocarPeca(T,origemT);
            }

            //#jogadaespecial roque grande

            if ( p is Rei && destino.coluna == origem.coluna - 2 ) {
                Posicao origemT = new Posicao(origem.linha,origem.coluna - 4);
                Posicao destinoT = new Posicao(origem.linha,origem.coluna - 1);
                Peca T = tab.retirarPeca(destinoT);
                T.decrementarQteMovimentos();
                tab.colocarPeca(T,origemT);
            }


        }



        public void realizaJogada(Posicao origem,Posicao destino)
        {
            Peca pecaCapturada = executaMovimento(origem,destino);

            if ( estaEmXeque(jogadorAtual) ) {
                desfazMovimento(origem,destino,pecaCapturada);
                throw new TabuleiroException("Você não pode se colocar em xeque");
            } 

            if(estaEmXeque(adversaria(jogadorAtual))) {
                xeque = true;
            } else {
                xeque = false;
            }

            if(testeXequemate(adversaria(jogadorAtual))) {
                terminada = true;
            } else {
                turno++;
                mudaJogador();
            }      
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
            if (!tab.peca(origem).movimentoPossivel(destino)) {
                throw new TabuleiroException("Posicao de destino invalida");
            }
        }

        public void colocarNovaPeca(char coluna, int linha, Peca peca)
        {
            tab.colocarPeca(peca,new PosicaoXadrez(coluna,linha).toPosicao());
            pecas.Add(peca);
        }

        

        private void colocarPecas()  {
            colocarNovaPeca('a',1,new Torre(tab,Cor.Branca));
            colocarNovaPeca('b',1,new Cavalo(tab,Cor.Branca));
            colocarNovaPeca('c',1,new Bispo(tab,Cor.Branca));
            colocarNovaPeca('d',1,new Dama(tab,Cor.Branca));
            colocarNovaPeca('e',1,new Rei(tab,Cor.Branca,this));
            colocarNovaPeca('f',1,new Bispo(tab,Cor.Branca));
            colocarNovaPeca('g',1,new Cavalo(tab,Cor.Branca));
            colocarNovaPeca('h',1,new Torre(tab,Cor.Branca));
            colocarNovaPeca('a',2,new Peao(tab,Cor.Branca,this));
            colocarNovaPeca('b',2,new Peao(tab,Cor.Branca,this));
            colocarNovaPeca('c',2,new Peao(tab,Cor.Branca,this));
            colocarNovaPeca('d',2,new Peao(tab,Cor.Branca,this));
            colocarNovaPeca('e',2,new Peao(tab,Cor.Branca,this));
            colocarNovaPeca('f',2,new Peao(tab,Cor.Branca,this));
            colocarNovaPeca('g',2,new Peao(tab,Cor.Branca,this));
            colocarNovaPeca('h',2,new Peao(tab,Cor.Branca,this));

            colocarNovaPeca('a',8,new Torre(tab,Cor.Preta));
            colocarNovaPeca('b',8,new Cavalo(tab,Cor.Preta));
            colocarNovaPeca('c',8,new Bispo(tab,Cor.Preta));
            colocarNovaPeca('d',8,new Dama(tab,Cor.Preta));
            colocarNovaPeca('e',8,new Rei(tab,Cor.Preta,this));
            colocarNovaPeca('f',8,new Bispo(tab,Cor.Preta));
            colocarNovaPeca('g',8,new Cavalo(tab,Cor.Preta));
            colocarNovaPeca('h',8,new Torre(tab,Cor.Preta));
            colocarNovaPeca('a',7,new Peao(tab,Cor.Preta,this));
            colocarNovaPeca('b',7,new Peao(tab,Cor.Preta,this));
            colocarNovaPeca('c',7,new Peao(tab,Cor.Preta,this));
            colocarNovaPeca('d',7,new Peao(tab,Cor.Preta,this));
            colocarNovaPeca('e',7,new Peao(tab,Cor.Preta,this));
            colocarNovaPeca('f',7,new Peao(tab,Cor.Preta,this));
            colocarNovaPeca('g',7,new Peao(tab,Cor.Preta,this));
            colocarNovaPeca('h',7,new Peao(tab,Cor.Preta,this));

        }
    }
}
