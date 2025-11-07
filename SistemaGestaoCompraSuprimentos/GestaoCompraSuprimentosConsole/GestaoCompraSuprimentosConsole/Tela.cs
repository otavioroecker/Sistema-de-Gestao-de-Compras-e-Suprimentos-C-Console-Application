using System;
using System.Collections.Generic;

namespace GestaoCompraSuprimentosConsole
{
    public class Tela
    {
        private int largura;
        private int altura;
        private int colunaInicial;
        private int linhaInicial;
        private bool telaCheia;

        public Tela()
        {
            this.largura = 80;
            this.altura = 25;
            this.colunaInicial = 0;
            this.linhaInicial = 0;
            this.telaCheia = true;
            Console.CursorVisible = true;
        }

        public Tela(int coluna, int linha, int largura, int altura)
        {
            this.largura = largura;
            this.altura = altura;
            this.colunaInicial = coluna;
            this.linhaInicial = linha;
            this.telaCheia = false;
            Console.CursorVisible = true;
        }

        public void PrepararTela(string titulo = "")
        {
            Console.Clear();
            this.MontarMoldura(this.colunaInicial, this.linhaInicial, this.colunaInicial + this.largura, this.linhaInicial + this.altura);

            if (this.telaCheia)
            {
                this.MontarMoldura(this.colunaInicial, this.linhaInicial, this.colunaInicial + this.largura, this.linhaInicial + 2);
                this.MontarMoldura(this.colunaInicial, this.linhaInicial + this.altura - 2, this.colunaInicial + this.largura, this.linhaInicial + this.altura);
            }

            this.Centralizar(this.colunaInicial, this.colunaInicial + this.largura, this.linhaInicial + 1, titulo);
        }

        public string MostrarMenu(List<string> ops, int ci, int li)
        {
            int cf = ci + ops[0].Length + 1;
            int lf = li + ops.Count + 2;
            this.MontarMoldura(ci, li, cf, lf);
            int linha = li + 1;
            for (int i = 0; i < ops.Count; i++)
            {
                SafeSetCursor(ci + 1, linha);
                Console.Write(ops[i]);
                linha++;
            }
            SafeSetCursor(ci + 1, linha);
            Console.Write("Opção : ");
            return Console.ReadLine() ?? "";
        }

        public void Centralizar(int ci, int cf, int lin, string msg)
        {
            int col = (cf - ci - msg.Length) / 2 + ci;
            SafeSetCursor(col, lin);
            Console.Write(msg);
        }

        public void ApagarArea(int ci, int li, int cf, int lf)
        {
            for (int coluna = ci; coluna <= cf; coluna++)
            {
                for (int linha = li; linha <= lf; linha++)
                {
                    SafeSetCursor(coluna, linha);
                    Console.Write(" ");
                }
            }
        }

        public void MontarMoldura(int ci, int li, int cf, int lf)
        {
            // garante limites
            if (ci < 0) ci = 0;
            if (li < 0) li = 0;
            if (cf >= Console.BufferWidth) cf = Console.BufferWidth - 1;
            if (lf >= Console.BufferHeight) lf = Console.BufferHeight - 1;

            ApagarArea(ci, li, cf, lf);

            for (int col = ci; col < cf; col++)
            {
                SafeSetCursor(col, li);
                Console.Write("═");
                SafeSetCursor(col, lf);
                Console.Write("═");
            }

            for (int lin = li; lin < lf; lin++)
            {
                SafeSetCursor(ci, lin);
                Console.Write("║");
                SafeSetCursor(cf, lin);
                Console.Write("║");
            }

            SafeSetCursor(ci, li); Console.Write("╔");
            SafeSetCursor(ci, lf); Console.Write("╚");
            SafeSetCursor(cf, li); Console.Write("╗");
            SafeSetCursor(cf, lf); Console.Write("╝");
        }

        public void MontarJanela(string titulo, List<string> dados, int coluna, int linha, int largura)
        {
            int cf = coluna + largura;
            int lf = linha + dados.Count + 2;
            this.MontarMoldura(coluna, linha, cf, lf);
            this.Centralizar(coluna, cf, linha + 1, titulo);
            linha += 2;
            for (int i = 0; i < dados.Count; i++)
            {
                SafeSetCursor(coluna + 1, linha);
                Console.Write(dados[i]);
                linha++;
            }
        }

        public int MostrarNavegacao<T>(string titulo, List<T> itens, Func<T, List<string>> formatarDados, int coluna, int linha, int largura)
        {
            if (itens == null || itens.Count == 0)
            {
                MostrarMensagem("Nenhum item encontrado. Pressione uma tecla...");
                Console.ReadKey();
                return -1;
            }

            int indice = 0;
            ConsoleKey tecla;

            do
            {
                Console.Clear();
                MontarJanela(titulo, formatarDados(itens[indice]), coluna, linha, largura);

                // Exibe número do item atual
                int total = itens.Count;
                Centralizar(coluna, coluna + largura, linha + 2 + formatarDados(itens[indice]).Count, $"[{indice + 1}/{total}]");

                MostrarMensagem("← / → para navegar | [ESC / V / 0] para sair");

                tecla = Console.ReadKey(true).Key;

                if (tecla == ConsoleKey.RightArrow && indice < itens.Count - 1)
                    indice++;
                else if (tecla == ConsoleKey.LeftArrow && indice > 0)
                    indice--;

            } while (tecla != ConsoleKey.Escape && tecla != ConsoleKey.V && tecla != ConsoleKey.D0);

            return indice; // retorna o índice do último item visualizado
        }

        public void MostrarMensagem(string msg)
        {
            this.ApagarArea(this.colunaInicial + 1, this.linhaInicial + this.altura - 1, this.colunaInicial + this.largura - 1, this.linhaInicial + this.altura - 1);
            int coluna = (this.largura - msg.Length) / 2;
            if (coluna < 0) coluna = 0;
            SafeSetCursor(coluna, this.linhaInicial + this.altura - 1);
            Console.Write(msg);
        }

        public void MostrarMensagem(int coluna, int linha, string msg)
        {
            SafeSetCursor(coluna, linha);
            Console.Write(msg);
        }

        public string Perguntar(string pergunta)
        {
            this.MostrarMensagem(pergunta);
            return Console.ReadLine() ?? "";
        }

        // Evita ArgumentOutOfRangeException
        private void SafeSetCursor(int x, int y)
        {
            try
            {
                if (x < 0) x = 0;
                if (y < 0) y = 0;
                if (x >= Console.BufferWidth) x = Console.BufferWidth - 1;
                if (y >= Console.BufferHeight) y = Console.BufferHeight - 1;
                Console.SetCursorPosition(x, y);
            }
            catch
            {
                // ignore
            }
        }
    }
}
