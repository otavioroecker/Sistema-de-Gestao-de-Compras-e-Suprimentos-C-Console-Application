using System;
using System.Collections.Generic;

namespace GestaoCompraSuprimentosConsole
{
    public class CotacaoCRUD
    {
        public List<Cotacao> cotacoes;
        public Cotacao cotacao;
        public int posicao;
        private List<string> dados;
        private int coluna, linha, largura;
        private int larguraDados, colunaDados, linhaDados;
        private Tela tela;
        private RequisicaoCRUD requisicaoCRUD;
        private FornecedorCRUD fornecedorCRUD;

        public CotacaoCRUD(Tela tela, RequisicaoCRUD reqCrud, FornecedorCRUD fornCrud)
        {
            this.cotacoes = new List<Cotacao>();
            this.cotacao = new Cotacao();
            this.posicao = -1;

            this.dados = new List<string>()
            {
                "ID Cotacao    : ",
                "RC Id         : ",
                "Fornecedor Id : ",
                "Valor         : ",
                "Prazo (dias)  : ",
                "Justificativa : "
            };

            this.tela = tela;
            this.requisicaoCRUD = reqCrud;
            this.fornecedorCRUD = fornCrud;

            this.coluna = 6;
            this.linha = 4;
            this.largura = 68;

            this.larguraDados = largura - dados[0].Length - 2;
            this.colunaDados = coluna + dados[0].Length + 1;
            this.linhaDados = linha + 2;
        }

        public void ExecutarCRUD()
        {
            List<string> op = new List<string>() { "       Cotações       ", "1 - Registrar Cotação", "2 - Listar por RC", "0 - Sair" };
            while (true)
            {
                string opc = tela.MostrarMenu(op, coluna, linha);
                if (opc == "0") break;
                else if (opc == "1")
                {
                    tela.MontarJanela("Registrar Cotação", dados, coluna, linha, largura);
                    EntrarDados();
                    var req = requisicaoCRUD.ObterPorId(cotacao.RequisicaoId);
                    var forn = fornecedorCRUD.ObterPorId(cotacao.FornecedorId);
                    if (req == null) { tela.MostrarMensagem("RC não encontrada. Pressione uma tecla..."); Console.ReadKey(); continue; }
                    if (forn == null || !forn.DocumentacaoValida) { tela.MostrarMensagem("Fornecedor inválido ou documentação incompleta. Pressione uma tecla..."); Console.ReadKey(); continue; }
                    cotacao.Id = cotacoes.Count + 1;
                    cotacoes.Add(new Cotacao { Id = cotacao.Id, RequisicaoId = cotacao.RequisicaoId, FornecedorId = cotacao.FornecedorId, Valor = cotacao.Valor, PrazoDias = cotacao.PrazoDias, JustificativaEscolha = cotacao.JustificativaEscolha });
                    tela.MostrarMensagem("Cotação registrada. Pressione uma tecla...");
                    Console.ReadKey();
                }
                else if (opc == "2")
{
    // Só entra aqui se houver pelo menos uma cotação
    if (cotacoes == null || cotacoes.Count == 0)
    {
        tela.MostrarMensagem("Nenhuma cotação registrada. Pressione uma tecla...");
        Console.ReadKey();
        continue; // volta ao menu sem executar o resto
    }

    Console.Clear();
    tela.PrepararTela("Listagem de Cotações");
    Console.WriteLine();
    Console.WriteLine();
    Console.WriteLine();

    // Calcula o tamanho máximo de cada coluna dinamicamente
    int idWidth = Math.Max(2, cotacoes.Max(c => c.Id.ToString().Length));
    int rcWidth = Math.Max(2, cotacoes.Max(c => c.RequisicaoId.ToString().Length));
    int fornecedorWidth = Math.Max(12, cotacoes.Max(c => c.FornecedorId.ToString().Length));
    int valorWidth = Math.Max(8, cotacoes.Max(c => c.Valor.ToString("C").Length));
    int prazoWidth = Math.Max(5, cotacoes.Max(c => c.PrazoDias.ToString().Length));
    int justificativaWidth = 29; // largura fixa (limitada)

    // Cabeçalho
    Console.WriteLine(
        $"║ {"ID".PadRight(idWidth)} | {"RC".PadRight(rcWidth)} | {"Id F".PadRight(0)} | {"    Valor".PadRight(valorWidth + 1)} | {"Prazo".PadRight(prazoWidth)} | {"Justificativa".PadRight(justificativaWidth)}"
    );
    Console.WriteLine(
        $"║{"".PadRight(idWidth + rcWidth + fornecedorWidth + valorWidth + prazoWidth + justificativaWidth + 17, '═')}║"
    );

    // Linhas de dados
    foreach (var c in cotacoes)
    {
        var justificativa = string.IsNullOrEmpty(c.JustificativaEscolha)
            ? ""
            : (c.JustificativaEscolha.Length > justificativaWidth
                ? c.JustificativaEscolha.Substring(0, justificativaWidth - 3) + "..."
                : c.JustificativaEscolha);

        Console.WriteLine(
            $"║ {c.Id.ToString().PadRight(idWidth)} | {c.RequisicaoId.ToString().PadRight(rcWidth)} | {c.FornecedorId.ToString().PadRight(4)} | {c.Valor.ToString("C").PadLeft(valorWidth + 1)} | {c.PrazoDias.ToString().PadLeft(prazoWidth)} | {justificativa.PadRight(justificativaWidth)}"
        );
    }

    tela.MostrarMensagem("Pressione uma tecla para continuar...");
    Console.ReadKey();
}
            }
        }

        private void EntrarDados()
{
    int idReq, idForn, prazo;
    decimal val;
    string justificativa;

    // ----------- ID da Requisição -----------
    while (true)
    {
        Console.SetCursorPosition(colunaDados, linhaDados + 1);
        string entrada = Console.ReadLine()?.Trim() ?? "";

        if (int.TryParse(entrada, out idReq) && idReq > 0)
            break;
        else
        {
            tela.MostrarMensagem("⚠ Digite um número inteiro válido para o ID da Requisição.");
            Console.ReadKey(true);
            tela.MostrarMensagem(" "); // limpa mensagem
        }
    }

    // ----------- ID do Fornecedor -----------
    while (true)
    {
        Console.SetCursorPosition(colunaDados, linhaDados + 2);
        string entrada = Console.ReadLine()?.Trim() ?? "";

        if (int.TryParse(entrada, out idForn) && idForn > 0)
            break;
        else
        {
            tela.MostrarMensagem("⚠ Digite um número inteiro válido para o ID do Fornecedor.");
            Console.ReadKey(true);
            tela.MostrarMensagem(" ");
        }
    }

    // ----------- Valor -----------
    while (true)
    {
        Console.SetCursorPosition(colunaDados, linhaDados + 3);
        string entrada = Console.ReadLine()?.Trim() ?? "";

        if (decimal.TryParse(entrada, out val) && val > 0)
            break;
        else
        {
            tela.MostrarMensagem("⚠ Digite um valor numérico positivo.");
            Console.ReadKey(true);
            tela.MostrarMensagem(" ");
        }
    }

    // ----------- Prazo (dias) -----------
    while (true)
    {
        Console.SetCursorPosition(colunaDados, linhaDados + 4);
        string entrada = Console.ReadLine()?.Trim() ?? "";

        if (int.TryParse(entrada, out prazo) && prazo >= 0)
            break;
        else
        {
            tela.MostrarMensagem("⚠ Digite um número inteiro para o prazo.");
            Console.ReadKey(true);
            tela.MostrarMensagem(" ");
        }
    }

    // ----------- Justificativa -----------
    while (true)
    {
        Console.SetCursorPosition(colunaDados, linhaDados + 5);
        string entrada = Console.ReadLine()?.Trim() ?? "";

        if (!string.IsNullOrEmpty(entrada))
        {
            justificativa = entrada;
            break;
        }
        else
        {
            tela.MostrarMensagem("⚠ Justificativa é obrigatória.");
            Console.ReadKey(true);
            tela.MostrarMensagem(" ");
        }
    }

    // --- Preenche os dados ---
    cotacao.RequisicaoId = idReq;
    cotacao.FornecedorId = idForn;
    cotacao.Valor = val;
    cotacao.PrazoDias = prazo;
    cotacao.JustificativaEscolha = justificativa;
}


        public List<Cotacao> ListarPorRequisicao(int requisicaoId)
        {
            var lista = new List<Cotacao>();
            foreach (var c in cotacoes) if (c.RequisicaoId == requisicaoId) lista.Add(c);
            return lista;
        }
    }
}
