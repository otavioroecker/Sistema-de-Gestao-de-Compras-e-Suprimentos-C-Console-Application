using System;

namespace GestaoCompraSuprimentosConsole
{
    public class OrdemCompra
    {
        public int Id;
        public int RequisicaoId;
        public int FornecedorId;
        public decimal Valor;
        public string Status; // "Emitida", "Recebida"
        public DateTime DataEmissao;
        public DateTime? DataRecebimento;
    }
}
