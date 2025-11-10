using System;

namespace GestaoCompraSuprimentosConsole
{
    public class Requisicao
    {
        public int Id;
        public string Departamento;
        public string Solicitante;
        public string ProdutoNome; 
        public decimal Quantidade;
        public string Justificativa;
        public decimal ValorEstimado;
        public string Status; // "Aberta", "Em Aprovação", "Aprovada", "Rejeitada", "OC Emitida"
        public int NivelAprovacao; 
        public int OcId; 
        public DateTime DataCriacao;
    }
}
