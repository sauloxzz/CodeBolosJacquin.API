using CodeBolosJacquin.API.Context;
using CodeBolosJacquin.API.Domains;
using CodeBolosJacquin.API.Interfaces;
using CodeBolosJacquin.API.ViewModels;
using Microsoft.EntityFrameworkCore;

namespace CodeBolosJacquin.API.Repositories
{
    public class BoloRepository : IBoloRepository
    {
        //Injetando BolosJacquinContext com um metodo construtor
        private readonly BolosJacquinContext _context;
        public BoloRepository(BolosJacquinContext context)
        {
            _context = context;
        }





        public async Task<bool> AtualizarAsync(int id, BoloRequestViewModel boloAtulizado)
        {
            var bolo = await _context.Bolos
                .Include(b => b.Categoria)
                .Include(b => b.BoloImagens)
                .FirstOrDefaultAsync(b => b.Id == id);

            if (bolo == null)
                throw new KeyNotFoundException($"Bolo não encontrado.");

            bolo.Nome = boloAtulizado.Nome ?? bolo.Nome;
            bolo.Descricao = boloAtulizado.Descricao ?? bolo.Descricao;
            bolo.Preco = boloAtulizado.Preco;
            bolo.Peso = boloAtulizado.Peso;


            bolo.Categoria.Clear();
            var categorias = await obterCategoriasAsync(boloAtulizado.Categorias);
            foreach (var categoria in categorias)
            {
                bolo.Categoria.Add(categoria);
            }

            if (bolo.BoloImagens.Any())
            {
                _context.BoloImagens.RemoveRange(bolo.BoloImagens);
                bolo.BoloImagens.Clear();
            }

            if (boloAtulizado.Imagens != null)
            {
                foreach (var imagem in boloAtulizado.Imagens)
                {
                    bolo.BoloImagens.Add(new BoloImagen
                    {
                        CaminhoImagem = imagem,
                        BoloId = bolo.Id
                    });
                }
            } 
            await _context.SaveChangesAsync();

            return true;
        }



        public async Task<BoloResponseViewModel?> BuscarPorIdAsync(int id)
        {
            var Bolo = await _context.Bolos
                .Include(b => b.Categoria)
                .Include(b => b.BoloImagens)
                .FirstOrDefaultAsync(b => b.Id == id);
            
            return Bolo == null ? null  : MapToResponse(Bolo);
        }



        public Task<BoloResponseViewModel> CadastrarAsync(BoloRequestViewModel bolo)
        {
            throw new NotImplementedException();
        }



        public async Task<IEnumerable<BoloResponseViewModel>> ListarTodosAsync()
        {
           var Bolos = await _context.Bolos.Include(b => b.Categoria).Include(b => b.BoloImagens).ToListAsync();

            return Bolos.Select(MapToResponse);
        }



        public Task<bool> RemoverAsync(int id)
        {
            throw new NotImplementedException();
        }




        private static BoloResponseViewModel MapToResponse(Bolo bolo)
        {
            return new BoloResponseViewModel
            {
                id = bolo.Id,
                Nome = bolo.Nome,
                Descricao = bolo.Descricao,
                Preco = bolo.Preco,
                Peso = bolo.Peso,
                Categorias = bolo.Categoria.Select(c => c.Nome).ToList(),
                Imagens = bolo.BoloImagens.Select(i => i.CaminhoImagem).ToList()
            };
        }


        private async Task <List<Categoria>> obterCategoriasAsync (IEnumerable<string>? categorias)
        {
            var lista = new List<Categoria>();
            if (categorias == null)
                return lista;

            foreach (var nome in categorias
                .Where(n => !string.IsNullOrWhiteSpace(n))
                .Select (n => n.Trim())
                .Distinct(StringComparer.OrdinalIgnoreCase)
            )
            {
                var categoria = await _context.Categorias.FirstOrDefaultAsync(c => c.Nome == nome);
                if (categoria == null)
                {
                    categoria = new Categoria
                    {
                        Nome = nome,
                    };
                    _context.Categorias.Add(categoria);
                }

                lista.Add(categoria);
            }

            return lista;

        }

        
    }
}
