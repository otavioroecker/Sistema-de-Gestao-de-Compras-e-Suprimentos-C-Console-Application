namespace GestaoCompraSuprimentosConsole
{
    public class Produto
    {
        public string Codigo;
        public string Descricao;
        public string Categoria;
        public string Unidade;
        public decimal ValorEstimado;

        public Produto() { }
        public Produto(string codigo, string descricao, string categoria, string unidade, decimal valor)
        {
            Codigo = codigo; Descricao = descricao; Categoria = categoria; Unidade = unidade; ValorEstimado = valor;
        }
    }
}
