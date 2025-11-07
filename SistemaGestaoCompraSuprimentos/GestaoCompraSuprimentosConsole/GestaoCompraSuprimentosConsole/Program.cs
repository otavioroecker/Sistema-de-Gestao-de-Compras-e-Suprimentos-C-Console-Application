using System;
using System.Collections.Generic;

namespace GestaoCompraSuprimentosConsole
{
    class Program
    {
        static void Main(string[] args)
        {
            Tela tela = new Tela();

            // inicializa CRUDs
            FornecedorCRUD fornecedorCRUD = new FornecedorCRUD(tela);
            ProdutoCRUD produtoCRUD = new ProdutoCRUD(tela);
            RequisicaoCRUD requisicaoCRUD = new RequisicaoCRUD(tela);
            CotacaoCRUD cotacaoCRUD = new CotacaoCRUD(tela, requisicaoCRUD, fornecedorCRUD);
            OrdemCompraCRUD ordemCrud = new OrdemCompraCRUD(tela, requisicaoCRUD, fornecedorCRUD, cotacaoCRUD);

            List<string> opcoes = new List<string>()
            {
                "     Menu Compras     ",
                "1 - Requisições (RC)  ",
                "2 - Cotações          ",
                "3 - Fornecedores      ",
                "4 - Ordens de Compra  ",
                "5 - Produtos          ",
                "0 - Sair              "
            };

            while (true)
            {
                tela.PrepararTela("Sistema de Gestão de Compras e Suprimentos");
                string opc = tela.MostrarMenu(opcoes, 2, 3);

                if (opc == "0") break;
                else if (opc == "1") requisicaoCRUD.ExecutarCRUD();
                else if (opc == "2") cotacaoCRUD.ExecutarCRUD();
                else if (opc == "3") fornecedorCRUD.ExecutarCRUD();
                else if (opc == "4") ordemCrud.ExecutarCRUD();
                else if (opc == "5") produtoCRUD.ExecutarCRUD();
                else
                {
                    tela.MostrarMensagem("Opção inválida. Pressione uma tecla para continuar...");
                    Console.ReadKey();
                }
            }
        }
    }
}
