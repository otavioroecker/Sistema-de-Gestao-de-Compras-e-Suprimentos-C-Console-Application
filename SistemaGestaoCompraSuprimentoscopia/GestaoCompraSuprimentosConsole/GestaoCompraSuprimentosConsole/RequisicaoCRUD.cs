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
                "Produto Nome    : ", 
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

            budgetPorDept["TI"] = 15000m;
            budgetPorDept["Compras"] = 50000m;
            budgetPorDept["Financeiro"] = 20000m;

            requisicoes.Add(new Requisicao
            {
                Id = 1,
                Departamento = "TI",
                Solicitante = "Carlos",
                ProdutoNome = "Notebook Dell", 
                Quantidade = 1,
                Justificativa = "Substituição",
                ValorEstimado = 3500m,
                Status = "Pendente de Aprovação",
                NivelAprovacao = 0,
                DataCriacao = DateTime.Now.AddDays(-2)
            });
        }

        public void ExecutarCRUD()
        {
            tela.MontarJanela("Requisições de Compra (RC)", dados, coluna, linha, largura);
            tela.MostrarMensagem("Procure uma RC pelo ID");

            EntrarDadosId(requisicao);
            bool achou = ProcurarCodigo();

            if (!achou)
            {
                string resp = tela.Perguntar("RC não encontrada. Deseja criar (S/N) : ");
                if (resp.ToLower() == "s")
                {
                    var novaReq = new Requisicao();

                    bool ok = EntrarDadosCampos(novaReq);
                    if (!ok) return;

                    resp = tela.Perguntar("Enviar para aprovação (S/N) : ");
                    if (resp.ToLower() == "s")
                    {
                        novaReq.Status = "Pendente de Aprovação";
                        novaReq.DataCriacao = DateTime.Now;
                        novaReq.NivelAprovacao = 0;
                        novaReq.Id = requisicoes.Count + 1;
                        requisicoes.Add(novaReq);

                        tela.MostrarMensagem("RC criada com sucesso. Pressione uma tecla...");
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
                    tela.MostrarMensagem(aprovado
                        ? "Aprovação processada. Pressione uma tecla..."
                        : "Não foi possível aprovar. Pressione uma tecla...");
                    Console.ReadKey();
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
                        tela.MostrarMensagem("OC Emitida com sucesso. Pressione uma tecla...");
                    }
                    else
                    {
                        tela.MostrarMensagem("Não é possível emitir OC: aprovação/budget insuficiente.");
                    }
                    Console.ReadKey();
                }
            }
        }


        private void EntrarDadosId(Requisicao req)
        {
            while (true)
            {
                Console.SetCursorPosition(colunaDados, linhaDados);
                string entrada = Console.ReadLine()?.Trim() ?? "";

                if (int.TryParse(entrada, out int id) && id > 0)
                {
                    req.Id = id;
                    break;
                }
                else
                {
                    tela.MostrarMensagem("⚠ Digite um número válido para o ID da RC.");
                    Console.ReadKey(true);
                    tela.MostrarMensagem(" ");
                }
            }
        }

        private bool EntrarDadosCampos(Requisicao req)
        {
            int desloc = 1;

            // Departamento
            while (true)
            {
                Console.SetCursorPosition(colunaDados, linhaDados + desloc++);
                string entrada = Console.ReadLine()?.Trim() ?? "";

                if (!string.IsNullOrEmpty(entrada))
                {
                    req.Departamento = entrada;
                    break;
                }
                else
                {
                    tela.MostrarMensagem("⚠ Departamento é obrigatório.");
                    Console.ReadKey(true);
                    tela.MostrarMensagem(" ");
                }
            }

            // Solicitante
            while (true)
            {
                Console.SetCursorPosition(colunaDados, linhaDados + desloc++);
                string entrada = Console.ReadLine()?.Trim() ?? "";

                if (!string.IsNullOrEmpty(entrada))
                {
                    req.Solicitante = entrada;
                    break;
                }
                else
                {
                    tela.MostrarMensagem("⚠ Solicitante é obrigatório.");
                    Console.ReadKey(true);
                    tela.MostrarMensagem(" ");
                }
            }

            // Produto Nome
            while (true)
            {
                Console.SetCursorPosition(colunaDados, linhaDados + desloc++);
                string entrada = Console.ReadLine()?.Trim() ?? "";

                if (!string.IsNullOrEmpty(entrada))
                {
                    req.ProdutoNome = entrada;
                    break;
                }
                else
                {
                    tela.MostrarMensagem("⚠ Produto Nome é obrigatório.");
                    Console.ReadKey(true);
                    tela.MostrarMensagem(" ");
                }
            }

            // Quantidade
            while (true)
            {
                Console.SetCursorPosition(colunaDados, linhaDados + desloc++);
                string entrada = Console.ReadLine()?.Trim() ?? "";

                if (decimal.TryParse(entrada, out decimal quant) && quant > 0)
                {
                    req.Quantidade = quant;
                    break;
                }
                else
                {
                    tela.MostrarMensagem("⚠ Digite uma quantidade válida (> 0).");
                    Console.ReadKey(true);
                    tela.MostrarMensagem(" ");
                }
            }

            // Justificativa
            while (true)
            {
                Console.SetCursorPosition(colunaDados, linhaDados + desloc++);
                string entrada = Console.ReadLine()?.Trim() ?? "";

                if (!string.IsNullOrEmpty(entrada))
                {
                    req.Justificativa = entrada;
                    break;
                }
                else
                {
                    tela.MostrarMensagem("⚠ Justificativa é obrigatória.");
                    Console.ReadKey(true);
                    tela.MostrarMensagem(" ");
                }
            }

            // Valor Estimado
            while (true)
            {
                Console.SetCursorPosition(colunaDados, linhaDados + desloc++);
                string entrada = Console.ReadLine()?.Trim() ?? "";

                if (decimal.TryParse(entrada, out decimal valor) && valor > 0)
                {
                    req.ValorEstimado = valor;
                    break;
                }
                else
                {
                    tela.MostrarMensagem("⚠ Digite um valor válido (> 0).");
                    Console.ReadKey(true);
                    tela.MostrarMensagem(" ");
                }
            }

            return true;
        }


        private bool PodeEmitirOC(Requisicao r)
        {
            if (r.Status == "Rejeitada") return false;
            if (!budgetPorDept.ContainsKey(r.Departamento)) return false;

            decimal available = budgetPorDept[r.Departamento];
            if (r.ValorEstimado > available) return false;

            if (r.ValorEstimado < 1000m && r.NivelAprovacao >= 1) return true;
            if (r.ValorEstimado <= 10000m && r.NivelAprovacao >= 2) return true;
            if (r.ValorEstimado > 10000m && r.NivelAprovacao >= 3) return true;

            return false;
        }

        private bool AprovarRequisicao(Requisicao r)
        {
            r.NivelAprovacao++;
            if (PodeEmitirOC(r))
            {
                budgetPorDept[r.Departamento] -= r.ValorEstimado;
                r.Status = "Aprovada";
                return true;
            }
            r.Status = "Pendente de Aprovação";
            return false;
        }

        public bool ProcurarCodigo()
        {
            for (int i = 0; i < requisicoes.Count; i++)
            {
                if (requisicao.Id == requisicoes[i].Id)
                {
                    posicao = i;
                    return true;
                }
            }
            return false;
        }

        public void MostrarDados()
        {
            var r = requisicoes[posicao];
            tela.MostrarMensagem(colunaDados, linhaDados + 1, r.Departamento);
            tela.MostrarMensagem(colunaDados, linhaDados + 2, r.Solicitante);
            tela.MostrarMensagem(colunaDados, linhaDados + 3, r.ProdutoNome); 
            tela.MostrarMensagem(colunaDados, linhaDados + 4, r.Quantidade.ToString());
            tela.MostrarMensagem(colunaDados, linhaDados + 5, r.Justificativa);
            tela.MostrarMensagem(colunaDados, linhaDados + 6, r.ValorEstimado.ToString("C"));
            tela.MostrarMensagem(colunaDados, linhaDados + 10, $"Status: {r.Status} | Nível: {r.NivelAprovacao}");
        }

        public Requisicao ObterPorId(int id)
        {
            foreach (var r in requisicoes)
                if (r.Id == id) return r;
            return null;
        }
    }
}
