﻿using JNogueira.Bufunfa.Dominio.Resources;
using System.ComponentModel.DataAnnotations;

namespace JNogueira.Bufunfa.Api.ViewModels
{
    // View model utilizado para o alterar o cartão de crédito
    public class AlterarCartaoCreditoViewModel
    {
        /// <summary>
        /// Id do cartão
        /// </summary>
        [Required(ErrorMessageResourceType = typeof(Mensagem), ErrorMessageResourceName = "Id_Invalido")]
        public int IdCartao { get; set; }

        /// <summary>
        /// Nome do cartão
        /// </summary>
        [Required(ErrorMessageResourceType = typeof(CartaoCreditoMensagem), ErrorMessageResourceName = "Nome_Obrigatorio_Nao_Informado")]
        [MaxLength(100, ErrorMessageResourceType = typeof(CartaoCreditoMensagem), ErrorMessageResourceName = "Nome_Tamanho_Maximo_Excedido")]
        public string Nome { get; set; }

        /// <summary>
        /// Valor do limite do cartão
        /// </summary>
        [Required(ErrorMessageResourceType = typeof(CartaoCreditoMensagem), ErrorMessageResourceName = "Valor_Limite_Invalido")]
        public decimal? ValorLimite { get; set; }

        /// <summary>
        /// Dia do vencimento da fatura do cartão
        /// </summary>
        [Required(ErrorMessageResourceType = typeof(CartaoCreditoMensagem), ErrorMessageResourceName = "Dia_Vencimento_Fatura_Invalido")]
        public int? DiaVencimentoFatura { get; set; }
    }
}
