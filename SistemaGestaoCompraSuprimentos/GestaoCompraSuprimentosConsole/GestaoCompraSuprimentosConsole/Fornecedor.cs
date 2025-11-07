using System;

namespace GestaoCompraSuprimentosConsole
{
    public class Fornecedor
    {
        public int Id;
        public string Nome;
        public string Documento; // CNPJ
        public string Contato;
        public string Email;
        public string Telefone;
        public bool DocumentacaoValida;

        public Fornecedor() { }

        public Fornecedor(int id, string nome, string doc, string contato, string email, string tel, bool docVal)
        {
            Id = id; Nome = nome; Documento = doc; Contato = contato; Email = email; Telefone = tel; DocumentacaoValida = docVal;
        }
    }
}
