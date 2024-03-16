using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.VisualBasic;
using payment_api.Controller;
using payment_api.Models;
using payment_api.test.Helpers;
using Xunit;
using Xunit.Sdk;

namespace payment_api.test
{
    public class VendasControllerTest
    {
        [Fact]
        public void TestaVendaESeVendedorForNuloRetornaBadRequest()
        {
            // Arrange
            using (var context = new MoqDb().CreateDbContext())
            {
                var controller = new Vendas(context);

                var vendaInput = new VendaInputModel
                {
                    VendedorId = 100, // ID de um vendedor que não existe
                    Item = "Batata",
                    Data = DateTime.Now,
                    Status = "Aguardando pagamento"
                };

                // Act
                var result = controller.RealizaVenda(vendaInput);

                // Assert
                Assert.NotNull(result);
                var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
                Assert.Equal("Vendedor não encontrado", badRequestResult.Value);
            }
        }

        [Fact]
        public void TestaSeItemDaVendaÉNuloESeForRetornaMensagemDeErro()
        {
            //Arrange
            using (var context = new MoqDb().CreateDbContext())
            {
                var controller = new Vendas(context);

                var novoVendedor = new Vendedor
                {
                    Cpf = "12345678900",
                    Nome = "Novo Vendedor",
                    Email = "novo.vendedor@example.com",
                    Telefone = "1234567890"
                };
                context.Vendedor.Add(novoVendedor);
                context.SaveChanges();

                var vendaInput = new VendaInputModel
                {
                    VendedorId = novoVendedor.Id,
                    Item = "",
                    Data = DateTime.Now,
                    Status = "Aguardando pagamento"
                };

                // Act
                var result = controller.RealizaVenda(vendaInput);

                // Assert
                Assert.NotNull(result);
                var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
                var errorMessage = badRequestResult.Value.ToString();
                Assert.Equal("O campo Item não pode conter apenas espaços em branco.", errorMessage);
            }
        }

        [Fact]
        public void RealizaVendaComSucessoERetornaOk()
        {
            using (var context = new MoqDb().CreateDbContext())
            {
                var controller = new Vendas(context);

                var novoVendedor = new Vendedor
                {
                    Cpf = "12345678900",
                    Nome = "Novo Vendedor",
                    Email = "novo.vendedor@example.com",
                    Telefone = "1234567890"
                };
                context.Vendedor.Add(novoVendedor);
                context.SaveChanges();

                var vendaInput = new VendaInputModel
                {
                    VendedorId = novoVendedor.Id,
                    Item = "Batata",
                    Data = DateTime.Now,
                    Status = "Aguardando pagamento"
                };

                var result = controller.RealizaVenda(vendaInput);

                Assert.NotNull(result);
                var criadoCorretamente = Assert.IsType<CreatedAtActionResult>(result);
                Assert.Equal(201, criadoCorretamente.StatusCode);
            }
        }

        [Fact]
        public void TestaBuscaVendaPorId()
        {
            using (var context = new MoqDb().CreateDbContext())
            {
                // Given
                var controller = new Vendas(context);

                var novoVendedor = new Vendedor
                {
                    Cpf = "12345678900",
                    Nome = "Novo Vendedor",
                    Email = "novo.vendedor@example.com",
                    Telefone = "1234567890"
                };
                context.Vendedor.Add(novoVendedor);
                context.SaveChanges();

                var vendaInput = new VendaInputModel
                {
                    VendedorId = novoVendedor.Id,
                    Item = "Batata",
                    Data = DateTime.Now,
                    Status = "Aguardando pagamento"
                };

                var resultVenda = controller.RealizaVenda(vendaInput) as CreatedAtActionResult;
                var vendaCriada = resultVenda.Value as Venda;
                // When
                var result = controller.BuscaVendaPorId(vendaCriada.Id);
                // Then
                Assert.IsType<OkObjectResult>(result);
                var okResult = result as OkObjectResult;
                Assert.IsType<Venda>(okResult.Value);
                var vendaRetornada = okResult.Value as Venda;
                Assert.Equal(vendaCriada.Id, vendaRetornada.Id);
            }
        }

        [Fact]
        public void TestaBuscaVendaERetornaSeIdDaVendaNotFound()
        {
            using (var context = new MoqDb().CreateDbContext())
            {
                var controller = new Vendas(context);

                int idInexistente = -1;

                var result = controller.BuscaVendaPorId(idInexistente);

                var notFoundResult = Assert.IsType<NotFoundObjectResult>(result);
                Assert.Equal("Venda não encontrada", notFoundResult.Value);
            }
        }

        [Fact]
        public void TestaAtualizaVendaParaUmValorInválido()
        {
            using (var context = new MoqDb().CreateDbContext())
            {
                // Arrange
                var controller = new Vendas(context);

                // Criar uma nova venda para teste
                var novaVenda = new Venda
                {
                    Vendedor = new Vendedor { Id = 9, Nome = "Vendedor Teste" },
                    Data = DateTime.Now,
                    Status = "Aguardando pagamento",
                    Item = "Item de teste"
                };

                context.Vendas.Add(novaVenda);
                context.SaveChanges();

                // Novo status inválido
                string novoStatusInvalido = "Status'";

                // Act
                var result = controller.AtualizaVenda(novoStatusInvalido, novaVenda.Id);

                // Assert
                var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
                Assert.Equal("Transição de status inválida", badRequestResult.Value);
            }
        }
        [Fact]
        public void TestaAtualizaVendaParaUmValorVálido()
        {
            using (var context = new MoqDb().CreateDbContext())
            {
                // Arrange
                var controller = new Vendas(context);

                // Criar uma nova venda para teste
                var novaVenda = new Venda
                {
                    Vendedor = new Vendedor { Id = 20, Nome = "Vendedor Teste" },
                    Data = DateTime.Now,
                    Status = "Aguardando pagamento",
                    Item = "Item de teste"
                };

                context.Vendas.Add(novaVenda);
                context.SaveChanges();

                // Novo status válido
                string novoStatusValido = "Pagamento Aprovado";

                // Act
                var result = controller.AtualizaVenda(novoStatusValido, novaVenda.Id);

                // Assert
                var tudoCerto = Assert.IsType<OkObjectResult>(result);
                Assert.Equal(200, tudoCerto.StatusCode);
            }
        }

        [Fact]
        public void TestaAtualizaVendaDeAprovadoParaTransporte()
        {
            using (var context = new MoqDb().CreateDbContext())
            {
                // Arrange
                var controller = new Vendas(context);

                var novaVenda = new Venda
                {
                    Vendedor = new Vendedor { Id = 14, Nome = "Vendedor Teste" },
                    Data = DateTime.Now,
                    Status = "Pagamento Aprovado",
                    Item = "Item de teste"
                };

                context.Vendas.Add(novaVenda);
                context.SaveChanges();

                // Novo status válido
                string novoStatusValido = "Enviado para Transportadora";

                // Act
                var result = controller.AtualizaVenda(novoStatusValido, novaVenda.Id);

                // Assert
                var tudoCerto = Assert.IsType<OkObjectResult>(result);
                Assert.Equal(200, tudoCerto.StatusCode);
            }
        }

        [Fact]
        public void TesteDeletarVendar()
        {
            using (var context = new MoqDb().CreateDbContext())
            {
                var controller = new Vendas(context);

                var novoVendedor = new Vendedor
                {
                    Cpf = "12345678900",
                    Nome = "Novo Vendedor",
                    Email = "novo.vendedor@example.com",
                    Telefone = "1234567890"
                };
                context.Vendedor.Add(novoVendedor);
                context.SaveChanges();

                var vendaInput = new VendaInputModel
                {
                    VendedorId = novoVendedor.Id,
                    Item = "Batata",
                    Data = DateTime.Now,
                    Status = "Aguardando pagamento"
                };

                var resultVenda = controller.RealizaVenda(vendaInput) as CreatedAtActionResult;
                var vendaCriada = resultVenda.Value as Venda;

                var result = controller.DeletaVenda(vendaCriada.Id);

                var noContentResult = Assert.IsType<NoContentResult>(result);
                Assert.Equal(204, noContentResult.StatusCode);
            }
        }
        [Fact]
        public void TesteDeletarVendarIdInexistente()
        {
            using (var context = new MoqDb().CreateDbContext())
            {
                var controller = new Vendas(context);

                // Defina um ID que não exista no banco de dados
                int idInexistente = 9999;

                // Executar a ação de deleção com o ID inexistente
                var result = controller.DeletaVenda(idInexistente);

                // Verificar se o resultado é NotFound (404)
                var notFoundResult = Assert.IsType<NotFoundObjectResult>(result);
                Assert.Equal(404, notFoundResult.StatusCode);
            }
        }

    }
}