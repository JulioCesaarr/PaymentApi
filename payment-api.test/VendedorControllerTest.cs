using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using payment_api.Controller;
using payment_api.Models;
using payment_api.test.Helpers;
using Xunit;

namespace payment_api.test
{
    public class VendedorControllerTest
    {
        [Fact]
        public void CriaVendedor()
        {
            using (var context = new MoqDb().CreateDbContext())
            {
                var controller = new Vendedores(context);

                // Criar um novo vendedor para teste
                var novoVendedor = new Vendedor
                {
                    Cpf = "12345678900",
                    Nome = "Novo Vendedor",
                    Email = "novo.vendedor@example.com",
                    Telefone = "1234567890"
                };

                // Executar a ação de criação do vendedor
                var result = controller.CriaVendedor(novoVendedor);

                // Verificar se o resultado é OK (200)
                var okResult = Assert.IsType<OkResult>(result);
                Assert.Equal(200, okResult.StatusCode);

                // Verificar se o vendedor foi adicionado ao banco de dados
                var vendedorAdicionado = context.Vendedor.FirstOrDefault(v => v.Cpf == novoVendedor.Cpf);
                Assert.NotNull(vendedorAdicionado);
                Assert.Equal(novoVendedor.Nome, vendedorAdicionado.Nome);
                Assert.Equal(novoVendedor.Email, vendedorAdicionado.Email);
                Assert.Equal(novoVendedor.Telefone, vendedorAdicionado.Telefone);
            }
        }

        [Fact]
        public void BuscaVendedorESeIdInvalidoRetornaNotFound()
        {
            using (var context = new MoqDb().CreateDbContext())
            {
                var controller = new Vendedores(context);

                int idInexistente = 9999;

                var result = controller.BuscaVendedor(idInexistente);
                var vendedorInexistente = Assert.IsType<NotFoundResult>(result);
                Assert.Equal(404, vendedorInexistente.StatusCode);
            }
        }

        [Fact]
        public void BuscaVendedorPorId()
        {
            using (var context = new MoqDb().CreateDbContext())
            {
                var controller = new Vendedores(context);

                var novoVendedor = new Vendedor
                {
                    Cpf = "12345678900",
                    Nome = "Novo Vendedor",
                    Email = "novo.vendedor@example.com",
                    Telefone = "1234567890"
                };

                context.Vendedor.Add(novoVendedor);
                context.SaveChanges();

                var result = controller.BuscaVendedor(novoVendedor.Id);
                var encontrado = Assert.IsType<OkObjectResult>(result);
                Assert.NotNull(result);
                Assert.Equal(200, encontrado.StatusCode);
            }
        }

        [Fact]
        public void TestaAtualizaVendedorCorretamente()
        {
            using (var context = new MoqDb().CreateDbContext())
            {
                // Arrange
                var controller = new Vendedores(context);

                // Criar um novo vendedor para teste
                var novoVendedor = new Vendedor
                {
                    Cpf = "12345678900",
                    Nome = "Novo Vendedor",
                    Email = "novo.vendedor@example.com",
                    Telefone = "1234567890"
                };

                
                context.Vendedor.Add(novoVendedor);
                context.SaveChanges();

                
                var novosDadosVendedor = new Vendedor
                {
                    Id = novoVendedor.Id, // Mantém o mesmo ID
                    Cpf = "98765432100", // Novo CPF
                    Nome = "Vendedor Atualizado", // Novo Nome
                    Email = "vendedor.atualizado@example.com", // Novo Email
                    Telefone = "9876543210" // Novo Telefone
                };

                // Act
                var result = controller.AtualizaVendedor(novoVendedor.Id, novosDadosVendedor);

                // Assert
                var okResult = Assert.IsType<OkObjectResult>(result);
                Assert.Equal(200, okResult.StatusCode);

                // Verifica se o vendedor foi atualizado corretamente no banco de dados
                var vendedorAtualizado = context.Vendedor.Find(novoVendedor.Id);
                Assert.NotNull(vendedorAtualizado);
                Assert.Equal(novosDadosVendedor.Cpf, vendedorAtualizado.Cpf);
                Assert.Equal(novosDadosVendedor.Nome, vendedorAtualizado.Nome);
                Assert.Equal(novosDadosVendedor.Email, vendedorAtualizado.Email);
                Assert.Equal(novosDadosVendedor.Telefone, vendedorAtualizado.Telefone);
            }
        }

        [Fact]
        public void TestaAtualizaVendedorSemPassarTodosOsParametros()
        {
            using (var context = new MoqDb().CreateDbContext())
            {
                // Arrange
                var controller = new Vendedores(context);

                // Criar um novo vendedor para teste
                var novoVendedor = new Vendedor
                {
                    Cpf = "12345678900",
                    Nome = "Novo Vendedor",
                    Email = "novo.vendedor@example.com",
                    Telefone = "1234567890"
                };

                // Adiciona o novo vendedor ao contexto do banco de dados
                context.Vendedor.Add(novoVendedor);
                context.SaveChanges();

                // Novos dados do vendedor, faltando o CPF
                var novosDadosVendedor = new Vendedor
                {
                    Id = novoVendedor.Id, // Mantém o mesmo ID
                    Nome = "Vendedor Atualizado", // Novo Nome
                    Email = "vendedor.atualizado@example.com", // Novo Email
                    Telefone = "9876543210" // Novo Telefone
                };

                // Act
                var result = controller.AtualizaVendedor(novoVendedor.Id, novosDadosVendedor);

                // Assert
                var OkResult = Assert.IsType<OkObjectResult>(result);
                Assert.Equal(200, OkResult.StatusCode);

                // Verifica se o vendedor não foi atualizado no banco de dados
                var vendedorAtualizado = context.Vendedor.Find(novoVendedor.Id);
                Assert.NotNull(vendedorAtualizado);
                Assert.Equal(novoVendedor.Cpf, vendedorAtualizado.Cpf);
                Assert.Equal(novoVendedor.Nome, vendedorAtualizado.Nome);
                Assert.Equal(novoVendedor.Email, vendedorAtualizado.Email);
                Assert.Equal(novoVendedor.Telefone, vendedorAtualizado.Telefone);
            }
        }
        [Fact]
        public void TestaDeletarVendedorPorId(){
            using(var context = new MoqDb().CreateDbContext()){
                var controller = new Vendedores(context);

                int id = 1;

                var result = controller.DeletaVendedor(id);
                var NoContent = Assert.IsType<NoContentResult>(result);
                Assert.NotNull(result);
                Assert.Equal(204, NoContent.StatusCode);
            }
        }

        [Fact]
        public void TestaDeletarVendedorPorIdInexistente(){
            using(var context = new MoqDb().CreateDbContext()){
                var controller = new Vendedores(context);

                int id = 999;

                var result = controller.DeletaVendedor(id);
                var NotFound = Assert.IsType<NotFoundResult>(result);
                Assert.NotNull(result);
                Assert.Equal(404, NotFound.StatusCode);
            }
        }
    }
}