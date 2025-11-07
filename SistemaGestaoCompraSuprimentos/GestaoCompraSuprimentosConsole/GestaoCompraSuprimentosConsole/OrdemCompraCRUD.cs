using System;
using System.Collections.Generic;

namespace GestaoCompraSuprimentosConsole
{
    public class OrdemCompraCRUD
    {
        public List<OrdemCompra> ordens;
        public OrdemCompra ordem;
        public int posicao;
        private List<string> dados;
        private int coluna, linha, largura;
        private int larguraDados, colunaDados, linhaDados;
        private Tela tela;
        private RequisicaoCRUD requisicaoCRUD;
        private FornecedorCRUD fornecedorCRUD;
        private CotacaoCRUD cotacaoCRUD;

        public OrdemCompraCRUD(Tela tela, RequisicaoCRUD reqCrud, FornecedorCRUD fornCrud, CotacaoCRUD cotCrud)
        {
            this.ordens = new List<OrdemCompra>();
            this.ordem = new OrdemCompra();
            this.posicao = -1;

            this.dados = new List<string>() { "ID OC    : ", "RC Id    : ", "Fornecedor: ", "Valor    : " };

            this.tela = tela;
            this.requisicaoCRUD = reqCrud;
            this.fornecedorCRUD = fornCrud;
            this.cotacaoCRUD = cotCrud;

            this.coluna = 6;
            this.linha = 4;
            this.largura = 68;

            this.larguraDados = largura - dados[0].Length - 2;
            this.colunaDados = coluna + dados[0].Length + 1;
            this.linhaDados = linha + 2;
        }

        public void ExecutarCRUD()
        {
            List<string> op = new List<string>() { " Ordem de Compra ", "1 - Emitir OC a partir de RC", "2 - Listar OC", "0 - Sair" };
            while (true)
            {
                string opc = tela.MostrarMenu(op, coluna, linha);
                if (opc == "0") break;
                else if (opc == "1")
                {
                    Console.Clear();
                    tela.MontarJanela("Emitir Ordem de Compra", dados, coluna, linha, largura);
                    Console.SetCursorPosition(colunaDados, linhaDados + 1);
                    int idReq;
                    if (!int.TryParse(Console.ReadLine(), out idReq)) { tela.MostrarMensagem("Id inválido. Pressione uma tecla..."); Console.ReadKey(); continue; }
                    var req = requisicaoCRUD.ObterPorId(idReq);
                    if (req == null) { tela.MostrarMensagem("RC não encontrada. Pressione uma tecla..."); Console.ReadKey(); continue; }
                    var cotacoes = cotacaoCRUD.ListarPorRequisicao(idReq);
                    if (cotacoes.Count == 0) { tela.MostrarMensagem("Sem cotações para essa RC. Pressione uma tecla..."); Console.ReadKey(); continue; }

                    Console.Clear();
                    tela.PrepararTela("Escolha a cotação (ID) para emitir OC");
                    foreach (var c in cotacoes) Console.WriteLine($"{c.Id} - Forn {c.FornecedorId} - Valor {c.Valor:C} - Prazo {c.PrazoDias}d - Obs: {c.JustificativaEscolha}");
                    tela.MostrarMensagem("Digite a ID da cotação escolhida:");
                    int idCot;
                    if (!int.TryParse(Console.ReadLine(), out idCot)) continue;
                    var chosen = cotacoes.Find(x => x.Id == idCot);
                    if (chosen == null) { tela.MostrarMensagem("Cotação inválida. Pressione uma tecla..."); Console.ReadKey(); continue; }
                    var forn = fornecedorCRUD.ObterPorId(chosen.FornecedorId);
                    if (forn == null || !forn.DocumentacaoValida) { tela.MostrarMensagem("Fornecedor inválido/documentação incompleta. Pressione uma tecla..."); Console.ReadKey(); continue; }

                    OrdemCompra oc = new OrdemCompra
                    {
                        Id = ordens.Count + 1,
                        RequisicaoId = req.Id,
                        FornecedorId = chosen.FornecedorId,
                        Valor = chosen.Valor,
                        Status = "Emitida",
                        DataEmissao = DateTime.Now
                    };
                    ordens.Add(oc);
                    req.Status = "OC Emitida";
                    req.OcId = oc.Id;

                    tela.MostrarMensagem("OC emitida com sucesso! Pressione uma tecla...");
                    Console.ReadKey();
                }
                else if (opc == "2")
                {
                    Console.Clear();
                    tela.PrepararTela("Listagem de OC");
                    Console.WriteLine();
                    foreach (var o in ordens) Console.WriteLine($"OC {o.Id} | RC {o.RequisicaoId} | Forn {o.FornecedorId} | Valor {o.Valor:C} | {o.Status}");
                    tela.MostrarMensagem("Pressione uma tecla para continuar...");
                    Console.ReadKey();
                }
            }
        }

        public OrdemCompra ObterPorId(int id)
        {
            foreach (var o in ordens) if (o.Id == id) return o;
            return null;
        }
    }
}
