using System;
using System.Collections.Generic;

namespace GestaoCompraSuprimentosConsole
{
    public class ProdutoCRUD
    {
        public List<Produto> produtos;
        private Produto produto;
        private int posicao;
        private List<string> dados = new List<string>();
        private int coluna, linha, largura;
        private int larguraDados, colunaDados, linhaDados;
        private Tela tela;

        public ProdutoCRUD(Tela tela)
        {
            this.produtos = new List<Produto>();
            this.produto = new Produto();
            this.posicao = -1;

            dados.Add("Código   : ");
            dados.Add("Descrição: ");
            dados.Add("Categoria: ");
            dados.Add("Unidade  : ");
            dados.Add("Valor Est: ");

            this.tela = tela;
            this.coluna = 6;
            this.linha = 3;
            this.largura = 68;

            this.larguraDados = largura - dados[0].Length - 2;
            this.colunaDados = coluna + dados[0].Length + 1;
            this.linhaDados = linha + 2;

            // exemplo
            produtos.Add(new Produto("P001", "Notebook 14\"", "TI", "UN", 3500m));
            produtos.Add(new Produto("P002", "Mouse Óptico", "Acessórios", "UN", 45m));
        }

        public void ExecutarCRUD()
        {
            this.tela.MontarJanela("Cadastro de Produtos", dados, coluna, linha, largura);
            EntrarDados(1);
            bool achou = ProcurarCodigo();
            if (!achou)
            {
                string resp = tela.Perguntar("Produto não encontrado. Deseja cadastrar (S/N) : ");
                if (resp.ToLower() == "s")
                {
                    EntrarDados(2);
                    resp = tela.Perguntar("Confirma cadastro (S/N) : ");
                    if (resp.ToLower() == "s")
                    {
                        produtos.Add(new Produto(produto.Codigo, produto.Descricao, produto.Categoria, produto.Unidade, produto.ValorEstimado));
                        tela.MostrarMensagem("Produto cadastrado. Pressione uma tecla...");
                        Console.ReadKey();
                    }
                }
            }
            else
            {
                MostrarDados();
                string resp = tela.Perguntar("Deseja alterar, excluir ou voltar (A/E/V) : ");
                if (resp.ToLower() == "a")
                {
                    tela.MontarJanela("Alteração de Produto", dados, coluna, linha + dados.Count + 2, largura);
                    tela.MostrarMensagem("Informe novos dados");
                    EntrarDados(2, true);
                    resp = tela.Perguntar("Confirma alteração (S/N) : ");
                    if (resp.ToLower() == "s")
                    {
                        produtos[posicao].Descricao = produto.Descricao;
                        produtos[posicao].Categoria = produto.Categoria;
                        produtos[posicao].Unidade = produto.Unidade;
                        produtos[posicao].ValorEstimado = produto.ValorEstimado;
                        tela.MostrarMensagem("Produto alterado. Pressione uma tecla...");
                        Console.ReadKey();
                    }
                }
                else if (resp.ToLower() == "e")
                {
                    resp = tela.Perguntar("Confirma exclusão (S/N) : ");
                    if (resp.ToLower() == "s")
                    {
                        produtos.RemoveAt(posicao);
                        tela.MostrarMensagem("Produto excluído. Pressione uma tecla...");
                        Console.ReadKey();
                    }
                }
            }
        }

        public void EntrarDados(int qual, bool alteracao = false)
        {
            if (qual == 1)
            {
                SafeCursor();
                Console.SetCursorPosition(colunaDados, linhaDados);
                produto.Codigo = Console.ReadLine();
            }
            else
            {
                int desloc = alteracao ? dados.Count + 2 : 0;
                Console.SetCursorPosition(colunaDados, linhaDados + desloc + 1);
                produto.Descricao = Console.ReadLine();
                Console.SetCursorPosition(colunaDados, linhaDados + desloc + 2);
                produto.Categoria = Console.ReadLine();
                Console.SetCursorPosition(colunaDados, linhaDados + desloc + 3);
                produto.Unidade = Console.ReadLine();
                Console.SetCursorPosition(colunaDados, linhaDados + desloc + 4);
                decimal v;
                if (decimal.TryParse(Console.ReadLine(), out v)) produto.ValorEstimado = v;
            }
        }

        public bool ProcurarCodigo()
        {
            bool achei = false;
            for (int i = 0; i < produtos.Count; i++)
            {
                if (produto.Codigo == produtos[i].Codigo)
                {
                    achei = true;
                    posicao = i;
                    produto = produtos[i];
                    break;
                }
            }
            return achei;
        }

        public void MostrarDados()
        {
            tela.MostrarMensagem(colunaDados, linhaDados + 1, produtos[posicao].Descricao);
            tela.MostrarMensagem(colunaDados, linhaDados + 2, produtos[posicao].Categoria);
            tela.MostrarMensagem(colunaDados, linhaDados + 3, produtos[posicao].Unidade);
            tela.MostrarMensagem(colunaDados, linhaDados + 4, produtos[posicao].ValorEstimado.ToString("C"));
        }

        public Produto ObterPorCodigo(string codigo)
        {
            foreach (var p in produtos) if (p.Codigo == codigo) return p;
            return null;
        }

        private void SafeCursor()
        {
            if (Console.BufferWidth < 80) Console.BufferWidth = 80;
            if (Console.BufferHeight < 25) Console.BufferHeight = 25;
        }
    }
}
