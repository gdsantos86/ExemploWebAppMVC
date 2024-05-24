using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using ExemploWebAppMVC.Context;
using ExemploWebAppMVC.Models;

namespace ExemploWebAppMVC.Controllers
{
    public class FuncionariosController : Controller
    {
        private readonly AppDbContext _context;

        public FuncionariosController(AppDbContext context)
        {
            _context = context;
        }

        // GET: Funcionarios
        public async Task<IActionResult> Index(string ordemDeClassificacao, string cargoIdPesq, string empresaIdPesq)
        {
            // Preenche os selects na view
            ViewBag.Cargos = new SelectList(await _context.Cargos.ToListAsync(), "Id", "Nome", Convert.ToInt32(cargoIdPesq));
            ViewBag.Empresas = new SelectList(await _context.Empresas.ToListAsync(), "Id", "Nome", Convert.ToInt32(empresaIdPesq));

            var funcionarios = _context.Funcionarios
                .Include(f => f.Cargo)
                .Include(f => f.Empresa)
                .AsQueryable();

            funcionarios = Filtrar(funcionarios, cargoIdPesq, empresaIdPesq);

            funcionarios = Classificar(funcionarios, ordemDeClassificacao);

            return View(await funcionarios.AsNoTracking().ToListAsync());
        }

        // GET: Funcionarios/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var funcionario = await _context.Funcionarios
                .Include(f => f.Empresa)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (funcionario == null)
            {
                return NotFound();
            }

            return View(funcionario);
        }

        // GET: Funcionarios/Create
        public IActionResult Create()
        {
            ViewData["EmpresaId"] = new SelectList(_context.Empresas, "Id", "Nome");
            ViewData["CargoId"] = new SelectList(_context.Cargos, "Id", "Nome");
            return View();
        }

        // POST: Funcionarios/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Nome,Idade,CargoId,EmpresaId")] Funcionario funcionario)
        {
            if (ModelState.IsValid)
            {
                _context.Add(funcionario);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["EmpresaId"] = new SelectList(_context.Empresas, "Id", "Nome", funcionario.EmpresaId);
            ViewData["CargoId"] = new SelectList(_context.Cargos, "Id", "Nome", funcionario.CargoId);
            return View(funcionario);
        }

        // GET: Funcionarios/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var funcionario = await _context.Funcionarios.FindAsync(id);
            if (funcionario == null)
            {
                return NotFound();
            }
            ViewData["EmpresaId"] = new SelectList(_context.Empresas, "Id", "Nome", funcionario.EmpresaId);
            ViewData["CargoId"] = new SelectList(_context.Cargos, "Id", "Nome", funcionario.CargoId);
            return View(funcionario);
        }

        // POST: Funcionarios/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Nome,Idade,Cargo,EmpresaId")] Funcionario funcionario)
        {
            if (id != funcionario.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(funcionario);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!FuncionarioExists(funcionario.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            ViewData["EmpresaId"] = new SelectList(_context.Empresas, "Id", "Nome", funcionario.EmpresaId);
            ViewData["CargoId"] = new SelectList(_context.Cargos, "Id", "Nome", funcionario.CargoId);
            return View(funcionario);
        }

        // GET: Funcionarios/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var funcionario = await _context.Funcionarios
                .Include(f => f.Empresa)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (funcionario == null)
            {
                return NotFound();
            }

            return View(funcionario);
        }

        // POST: Funcionarios/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var funcionario = await _context.Funcionarios.FindAsync(id);
            if (funcionario != null)
            {
                _context.Funcionarios.Remove(funcionario);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool FuncionarioExists(int id)
        {
            return _context.Funcionarios.Any(e => e.Id == id);
        }


        private IQueryable<Funcionario> Filtrar(IQueryable<Funcionario> funcionarios, string cargoIdPesq, string empresaIdPesq)
        {
            ViewBag.CargoIdCorrente = cargoIdPesq;
            ViewBag.EmpresaIdCorrente = empresaIdPesq;


            if (!String.IsNullOrEmpty(cargoIdPesq))
            {
                funcionarios = funcionarios.Where(f => f.CargoId == Convert.ToInt32(cargoIdPesq));
            }

            if (!String.IsNullOrEmpty(empresaIdPesq))
            {
                funcionarios = funcionarios.Where(f => f.EmpresaId == Convert.ToInt32(empresaIdPesq));
            }


            return funcionarios;
        }


        private IQueryable<Funcionario> Classificar(IQueryable<Funcionario> funcionarios, string ordemDeClassificacao)
        {
            ViewBag.NomeClassifParam = ordemDeClassificacao == "Nome" ? "nome_dec" : "Nome";
            ViewBag.IdadeClassifParam = ordemDeClassificacao == "Idade" ? "idade_dec" : "Idade";

            funcionarios = ordemDeClassificacao switch
            {
                "Nome" => funcionarios.OrderBy(f => f.Nome),
                "nome_dec" => funcionarios.OrderByDescending(f => f.Nome),
                "Idade" => funcionarios.OrderBy(s => s.Idade),
                "idade_dec" => funcionarios.OrderByDescending(s => s.Idade),
                _ => funcionarios.OrderBy(s => s.Id),
            };
            return funcionarios;
        }

    }
}
