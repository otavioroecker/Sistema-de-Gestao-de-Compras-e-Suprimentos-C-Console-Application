using System;
using System.Collections.Generic;

namespace GestaoCompraSuprimentosConsole
{
    public class RequisicaoCRUD
    {
        public List<Requisicao> requisicoes;
        public Requisicao requisicao;
        public int posicao;
        private List<string> dados;
        private int coluna, linha, largura;
        private int larguraDados, colunaDados, linhaDados;
        private Tela tela;

        // budgets por departamento (exemplo)
        private Dictionary<string, decimal> budgetPorDept = new Dictionary<string, decimal>();

        public RequisicaoCRUD(Tela tela)
        {
            this.requisicoes = new List<Requisicao>();
            this.requisicao = new Requisicao();
            this.posicao = -1;

            this.dados = new List<string>()
            {
                "ID              : ",
                "Departamento    : ",
                "Solicitante     : ",
                "Produto Codigo  : ",
                "Quantidade      : ",
                "Justificativa   : ",
                "Valor Estimado  : "
            };

            this.tela = tela;
            this.coluna = 2;
            this.linha = 4;
            this.largura = 76;

            this.larguraDados = this.largura - dados[0].Length - 2;
            this.colunaDados = this.coluna + dados[0].Length + 1;
            this.linhaDados = this.linha + 2;

            // budgets de exemplo
            budgetPorDept["TI"] = 15000m;
            budgetPorDept["Compras"] = 50000m;
            budgetPorDept["Financeiro"] = 20000m;

            // exemplo de requisição
            requisicoes.Add(new Requisicao { Id = 1, Departamento = "TI", Solicitante = "Carlos", ProdutoCodigo = "P001", Quantidade = 1, Justificativa = "Substituição", ValorEstimado = 3500m, Status = "Aberta", NivelAprovacao = 0, DataCriacao = DateTime.Now.AddDays(-2) });
        }

        public void ExecutarCRUD()
        {
            tela.MontarJanela("Requisições de Compra (RC)", dados, coluna, linha, largura);
            tela.MostrarMensagem("Procure uma RC pelo ID");
            EntrarDados(1);
            bool achou = ProcurarCodigo();
            if (!achou)
            {
                string resp = tela.Perguntar("RC não encontrada. Deseja criar (S/N) : ");
                if (resp.ToLower() == "s")
                {
                    bool ok = EntrarDados(2);
                    if (!ok) return;
                    resp = tela.Perguntar("Enviar para aprovação (S/N) : ");
                    if (resp.ToLower() == "s")
                    {
                        requisicao.Status = "Aberta";
                        requisicao.DataCriacao = DateTime.Now;
                        requisicao.NivelAprovacao = 0;
                        requisicao.Id = requisicoes.Count + 1;
                        requisicoes.Add(requisicao);
                        tela.MostrarMensagem("RC criada. Pressione uma tecla...");
                        Console.ReadKey();
                    }
                }
            }
            else
            {
                MostrarDados();
                string resp = tela.Perguntar("Aprovar/Rejeitar/Emitir OC/Voltar (A/R/O/V) : ");
                if (resp.ToLower() == "a")
                {
                    bool aprovado = AprovarRequisicao(requisicoes[posicao]);
                    if (aprovado)
                    {
                        tela.MostrarMensagem("Aprovação processada. Pressione uma tecla...");
                        Console.ReadKey();
                    }
                }
                else if (resp.ToLower() == "r")
                {
                    requisicoes[posicao].Status = "Rejeitada";
                    tela.MostrarMensagem("RC rejeitada. Pressione uma tecla...");
                    Console.ReadKey();
                }
                else if (resp.ToLower() == "o")
                {
                    if (PodeEmitirOC(requisicoes[posicao]))
                    {
                        requisicoes[posicao].Status = "OC Emitida";
                        tela.MostrarMensagem("RC marcada como OC Emitida. Use o módulo OC para criar o documento. Pressione uma tecla...");
                        Console.ReadKey();
                    }
                    else
                    {
                        tela.MostrarMensagem("Não é possível emitir OC: aprovação/budget insuficiente. Pressione uma tecla...");
                        Console.ReadKey();
                    }
                }
            }
        }

        public bool EntrarDados(int qual, bool alteracao = false)
        {
            if (qual == 1)
            {
                Console.SetCursorPosition(colunaDados, linhaDados);
                int id;
                if (int.TryParse(Console.ReadLine(), out id)) requisicao.Id = id;
            }
            else
            {
                int desloc = alteracao ? dados.Count + 2 : 0;
                Console.SetCursorPosition(colunaDados, linhaDados + desloc + 1);
                requisicao.Departamento = Console.ReadLine();
                Console.SetCursorPosition(colunaDados, linhaDados + desloc + 2);
                requisicao.Solicitante = Console.ReadLine();
                Console.SetCursorPosition(colunaDados, linhaDados + desloc + 3);
                requisicao.ProdutoCodigo = Console.ReadLine();
                Console.SetCursorPosition(colunaDados, linhaDados + desloc + 4);
                decimal quant;
                if (decimal.TryParse(Console.ReadLine(), out quant)) requisicao.Quantidade = quant;
                Console.SetCursorPosition(colunaDados, linhaDados + desloc + 5);
                requisicao.Justificativa = Console.ReadLine();
                Console.SetCursorPosition(colunaDados, linhaDados + desloc + 6);
                decimal valor;
                if (decimal.TryParse(Console.ReadLine(), out valor)) requisicao.ValorEstimado = valor;
            }
            return true;
        }

        private bool PodeEmitirOC(Requisicao r)
        {
            if (r.Status == "Rejeitada") return false;
            if (!budgetPorDept.ContainsKey(r.Departamento)) return false;
            decimal available = budgetPorDept[r.Departamento];
            if (r.ValorEstimado > available) return false;

            // alçadas
            if (r.ValorEstimado < 1000m && r.NivelAprovacao >= 1) return true;
            if (r.ValorEstimado >= 1000m && r.ValorEstimado <= 10000m && r.NivelAprovacao >= 2) return true;
            if (r.ValorEstimado > 10000m && r.NivelAprovacao >= 3) return true;
            return false;
        }

        private bool AprovarRequisicao(Requisicao r)
        {
            r.NivelAprovacao++;
            if (PodeEmitirOC(r))
            {
                if (budgetPorDept.ContainsKey(r.Departamento))
                {
                    budgetPorDept[r.Departamento] -= r.ValorEstimado;
                }
                r.Status = "Aprovada";
                return true;
            }
            else
            {
                r.Status = "Em Aprovação";
                return true;
            }
        }

        public bool ProcurarCodigo()
        {
            bool achei = false;
            for (int i = 0; i < requisicoes.Count; i++)
            {
                if (requisicao.Id == requisicoes[i].Id)
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
            var r = requisicoes[posicao];
            tela.MostrarMensagem(colunaDados, linhaDados + 1, r.Departamento);
            tela.MostrarMensagem(colunaDados, linhaDados + 2, r.Solicitante);
            tela.MostrarMensagem(colunaDados, linhaDados + 3, r.ProdutoCodigo);
            tela.MostrarMensagem(colunaDados, linhaDados + 4, r.Quantidade.ToString());
            tela.MostrarMensagem(colunaDados, linhaDados + 5, r.Justificativa);
            tela.MostrarMensagem(colunaDados, linhaDados + 6, r.ValorEstimado.ToString("C"));
            tela.MostrarMensagem(colunaDados + 6, linhaDados + 10, $"Status: {r.Status} | Niv: {r.NivelAprovacao}");
        }

        public Requisicao ObterPorId(int id)
        {
            foreach (var r in requisicoes) if (r.Id == id) return r;
            return null;
        }
    }
}
