using System;
using System.Collections.Generic;

namespace GestaoCompraSuprimentosConsole
{
    public class FornecedorCRUD
    {
        private List<Fornecedor> fornecedores;
        private Fornecedor fornecedor;
        private int posicao;
        private List<string> dados = new List<string>();
        private int coluna, linha, largura;
        private int larguraDados, colunaDados, linhaDados;
        private Tela tela;

        public FornecedorCRUD(Tela tela)
        {
            this.fornecedores = new List<Fornecedor>();
            this.fornecedor = new Fornecedor();
            this.posicao = -1;

            this.dados.Add("ID        : ");
            this.dados.Add("Nome      : ");
            this.dados.Add("Documento : ");
            this.dados.Add("Contato   : ");
            this.dados.Add("Email     : ");
            this.dados.Add("Telefone  : ");
            this.dados.Add("Doc Valida (S/N):");

            this.tela = tela;
            this.coluna = 2;
            this.linha = 4;
            this.largura = 68;

            this.larguraDados = this.largura - dados[0].Length - 2;
            this.colunaDados = this.coluna + dados[0].Length + 1;
            this.linhaDados = this.linha + 2;

            // dados de teste
            fornecedores.Add(new Fornecedor(1, "Alpha Fornecedores", "12.345.678/0001-90", "João", "joao@alpha.com", "11-9999-0001", true));
            fornecedores.Add(new Fornecedor(2, "Beta Suprimentos", "98.765.432/0001-55", "Maria", "maria@beta.com", "11-9888-0002", true));
        }

        public void ExecutarCRUD()
        {
            this.tela.MontarJanela("Cadastro de Fornecedores", dados, coluna, linha, largura);
            EntrarDados(1);
            bool achou = ProcurarCodigo();
            if (!achou)
            {
                string resp = tela.Perguntar("Fornecedor não encontrado. Deseja cadastrar (S/N) : ");
                if (resp.ToLower() == "s")
                {
                    EntrarDados(2);
                    resp = tela.Perguntar("Confirma cadastro (S/N) : ");
                    if (resp.ToLower() == "s")
                    {
                        fornecedor.Id = fornecedores.Count + 1;
                        fornecedores.Add(new Fornecedor(fornecedor.Id, fornecedor.Nome, fornecedor.Documento, fornecedor.Contato, fornecedor.Email, fornecedor.Telefone, fornecedor.DocumentacaoValida));
                        tela.MostrarMensagem("Fornecedor cadastrado. Pressione uma tecla...");
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
                    tela.MontarJanela("Alteração de Fornecedor", dados, coluna, linha + dados.Count + 2, largura);
                    tela.MostrarMensagem("Informe novos dados");
                    EntrarDados(2, true);
                    resp = tela.Perguntar("Confirma alteração (S/N) : ");
                    if (resp.ToLower() == "s")
                    {
                        fornecedores[posicao].Nome = fornecedor.Nome;
                        fornecedores[posicao].Documento = fornecedor.Documento;
                        fornecedores[posicao].Contato = fornecedor.Contato;
                        fornecedores[posicao].Email = fornecedor.Email;
                        fornecedores[posicao].Telefone = fornecedor.Telefone;
                        fornecedores[posicao].DocumentacaoValida = fornecedor.DocumentacaoValida;
                        tela.MostrarMensagem("Fornecedor alterado. Pressione uma tecla...");
                        Console.ReadKey();
                    }
                }
                else if (resp.ToLower() == "e")
                {
                    resp = tela.Perguntar("Confirma exclusão (S/N) : ");
                    if (resp.ToLower() == "s")
                    {
                        fornecedores.RemoveAt(posicao);
                        tela.MostrarMensagem("Fornecedor excluído. Pressione uma tecla...");
                        Console.ReadKey();
                    }
                }
            }
        }

        public void EntrarDados(int qual, bool alteracao = false)
        {
            if (qual == 1)
            {
                Console.SetCursorPosition(colunaDados, linhaDados);
                int id;
                if (int.TryParse(Console.ReadLine(), out id)) fornecedor.Id = id;
            }
            else
            {
                int desloc = alteracao ? dados.Count + 2 : 0;
                Console.SetCursorPosition(colunaDados, linhaDados + desloc + 1);
                fornecedor.Nome = Console.ReadLine();
                Console.SetCursorPosition(colunaDados, linhaDados + desloc + 2);
                fornecedor.Documento = Console.ReadLine();
                Console.SetCursorPosition(colunaDados, linhaDados + desloc + 3);
                fornecedor.Contato = Console.ReadLine();
                Console.SetCursorPosition(colunaDados, linhaDados + desloc + 4);
                fornecedor.Email = Console.ReadLine();
                Console.SetCursorPosition(colunaDados, linhaDados + desloc + 5);
                fornecedor.Telefone = Console.ReadLine();
                Console.SetCursorPosition(colunaDados + 5, linhaDados + desloc + 6);
                string s = Console.ReadLine();
                fornecedor.DocumentacaoValida = (s?.ToLower() == "s" || s?.ToLower() == "y" || s?.ToLower() == "sim");
            }
        }

        public bool ProcurarCodigo()
        {
            bool achei = false;
            for (int i = 0; i < fornecedores.Count; i++)
            {
                if (fornecedor.Id == fornecedores[i].Id)
                {
                    achei = true;
                    posicao = i;
                    break;
                }
            }
            return achei;
        }

        public void MostrarDados()
        {
            tela.MostrarMensagem(colunaDados, linhaDados + 1, fornecedores[posicao].Nome);
            tela.MostrarMensagem(colunaDados, linhaDados + 2, fornecedores[posicao].Documento);
            tela.MostrarMensagem(colunaDados, linhaDados + 3, fornecedores[posicao].Contato);
            tela.MostrarMensagem(colunaDados, linhaDados + 4, fornecedores[posicao].Email);
            tela.MostrarMensagem(colunaDados, linhaDados + 5, fornecedores[posicao].Telefone);
            tela.MostrarMensagem(colunaDados + 6, linhaDados + 6, fornecedores[posicao].DocumentacaoValida ? "Sim  " : "Não  ");
        }

        public Fornecedor ObterPorId(int id)
        {
            foreach (var f in fornecedores) if (f.Id == id) return f;
            return null;
        }
    }
}
