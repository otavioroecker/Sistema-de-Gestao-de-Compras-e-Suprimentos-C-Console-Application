using System;
using System.Collections.Generic;

namespace GestaoCompraSuprimentosConsole
{
    public class RecebimentoCRUD
    {
        private RequisicaoCRUD requisicaoCrud;
        private Tela tela;
        private List<Recebimento> recebimentos;

        public RecebimentoCRUD(Tela tela, RequisicaoCRUD requisicaoCrud)
        {
            this.tela = tela;
            this.requisicaoCrud = requisicaoCrud;
            this.recebimentos = new List<Recebimento>();
        }

        public void Executar()
        {
            tela.PrepararTela("Recebimento de Produtos");

            // ID da RC
            string entradaId = tela.Perguntar("Digite o ID da RC: ");
            if (!int.TryParse(entradaId, out int idRc) || idRc <= 0)
            {
                tela.MostrarMensagem("⚠ ID inválido. Pressione qualquer tecla...");
                Console.ReadKey();
                return;
            }

            var requisicao = requisicaoCrud.ObterPorId(idRc);
            if (requisicao == null)
            {
                tela.MostrarMensagem("⚠ RC não encontrada. Pressione qualquer tecla...");
                Console.ReadKey();
                return;
            }

            tela.MostrarMensagem($"RC encontrada: Produto {requisicao.ProdutoNome}, Quantidade {requisicao.Quantidade}, Valor {requisicao.ValorEstimado}, Solicitante {requisicao.Solicitante}");

            // Quantidade recebida
            string entradaQtd = tela.Perguntar("Quantidade recebida: ");
            if (!decimal.TryParse(entradaQtd, out decimal qtdRecebida) || qtdRecebida <= 0)
            {
                tela.MostrarMensagem("⚠ Quantidade inválida. Pressione qualquer tecla...");
                Console.ReadKey();
                return;
            }

            // Valor recebido
            string entradaValor = tela.Perguntar("Valor recebido: ");
            if (!decimal.TryParse(entradaValor, out decimal valorRecebido) || valorRecebido <= 0)
            {
                tela.MostrarMensagem("⚠ Valor inválido. Pressione qualquer tecla...");
                Console.ReadKey();
                return;
            }

            // Nome do responsável
            string nomeResp = tela.Perguntar("Nome do responsável (deve ser igual ao solicitante): ");
            if (string.IsNullOrWhiteSpace(nomeResp))
            {
                tela.MostrarMensagem("⚠ Nome do responsável não pode estar vazio. Pressione qualquer tecla...");
                Console.ReadKey();
                return;
            }

            // Verificação de igualdade com os dados da RC
            bool quantidadeOk = qtdRecebida == requisicao.Quantidade;
            bool valorOk = valorRecebido == requisicao.ValorEstimado;
            bool responsavelOk = nomeResp.Trim().Equals(requisicao.Solicitante.Trim(), StringComparison.OrdinalIgnoreCase);

            if (!quantidadeOk || !valorOk || !responsavelOk)
            {
                tela.MostrarMensagem("⚠ Atenção: Recebimento não confere com a RC!");
                if (!quantidadeOk)
                    tela.MostrarMensagem($"  - Quantidade da RC: {requisicao.Quantidade}, digitada: {qtdRecebida}");
                if (!valorOk)
                    tela.MostrarMensagem($"  - Valor da RC: {requisicao.ValorEstimado}, digitado: {valorRecebido}");
                if (!responsavelOk)
                    tela.MostrarMensagem($"  - Nome do responsável deve ser: {requisicao.Solicitante}, digitado: {nomeResp}");
                Console.ReadKey();
                return;
            }

            var rec = new Recebimento
            {
                RcId = requisicao.Id,
                QuantidadeRecebida = qtdRecebida,
                ValorRecebido = valorRecebido,
                Responsavel = nomeResp,
                DataRecebimento = DateTime.Now,
                Status = "Recebido"
            };

            recebimentos.Add(rec);
            requisicao.Status = "Recebido";

            tela.MostrarMensagem("✔ Recebimento registrado com sucesso! Pressione qualquer tecla...");
            Console.ReadKey();
        }

        public List<Recebimento> ListarRecebimentos()
        {
            return recebimentos;
        }
    }
}
