using Microsoft.AspNetCore.Mvc;
using TrilhaApiDesafio.Context;
using TrilhaApiDesafio.Models;

namespace TrilhaApiDesafio.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class TarefaController : ControllerBase
    {
        private readonly OrganizadorContext _context;

        public TarefaController(OrganizadorContext context)
        {
            _context = context;
        }

        [HttpGet("{id}")]
        public IActionResult ObterPorId(int id)
        {
            var tarefaEncontrada = _context.Tarefas.Find(id);
            if (tarefaEncontrada == null)
                return NotFound();
            return Ok(tarefaEncontrada);
        }

        [HttpGet("ObterTodos")]
        public IActionResult ObterTodos()
        {
            var tarefas = _context.Tarefas.ToList();
            return Ok(tarefas);
        }

        [HttpGet("proximasTarefasPendentes")]
        public IActionResult ProximasTarefasPendentes()
        {                                
            DateTime exatamenteAgora = DateTime.Now;
            var tarefas = _context.Tarefas.Where(x => x.Data > exatamenteAgora)
                                            .Where(x => x.Status == EnumStatusTarefa.Pendente)
                                            .OrderBy(x => x.Data);
            return Ok(tarefas);
        }

        [HttpGet("ProximasTarefasFinalizadas")]
        public IActionResult ProximasTarefasFinalizadas()
        {                                
            DateTime exatamenteAgora = DateTime.Now;
            var tarefas = _context.Tarefas.Where(x => x.Data > exatamenteAgora)
                                            .Where(x => x.Status == EnumStatusTarefa.Finalizado)
                                            .OrderBy(x => x.Data);
            return Ok(tarefas);
        }

        [HttpGet("Resumo")]
        public IActionResult ResumoTarefas()
        {                                
            DateTime exatamenteAgora = DateTime.Now;
            int QtdTarefasPendentesPassado = _context.Tarefas.Where(x => x.Data < exatamenteAgora)
                                            .Where(x => x.Status == EnumStatusTarefa.Pendente)
                                            .Count();
            int QtdTarefasPendentesFuturo = _context.Tarefas.Where(x => x.Data >= exatamenteAgora)
                                            .Where(x => x.Status == EnumStatusTarefa.Pendente)
                                            .Count();
            int QtdTarefasFinalizadasPassado = _context.Tarefas.Where(x => x.Data < exatamenteAgora)
                                            .Where(x => x.Status == EnumStatusTarefa.Finalizado)
                                            .Count();
            int QtdTarefasFinalizadasFuturo = _context.Tarefas.Where(x => x.Data >= exatamenteAgora)
                                            .Where(x => x.Status == EnumStatusTarefa.Finalizado)
                                            .Count();

            
            var resumo = new { TarefasPendentes =  new { Total = QtdTarefasPendentesPassado + QtdTarefasPendentesFuturo, Passado = QtdTarefasPendentesPassado, Futuro = QtdTarefasPendentesFuturo },
                                TarefasFinalizadas = new { Total = QtdTarefasFinalizadasPassado + QtdTarefasFinalizadasFuturo, Passado = QtdTarefasFinalizadasPassado, Futuro = QtdTarefasFinalizadasFuturo }
                            };                                            
            return Ok(resumo);
        }

        [HttpGet("ObterPorTitulo")]
        public IActionResult ObterPorTitulo(string titulo)
        {
            var tarefa = _context.Tarefas.Where(x => x.Titulo == titulo);
            return Ok(tarefa);
        }

        [HttpGet("ObterPorData")]
        public IActionResult ObterPorData(DateTime data)
        {
            var tarefa = _context.Tarefas.Where(x => x.Data.Date == data.Date);
            return Ok(tarefa);
        }

        [HttpGet("ObterPorStatus")]
        public IActionResult ObterPorStatus(EnumStatusTarefa status)
        {
            var tarefa = _context.Tarefas.Where(x => x.Status == status);
            return Ok(tarefa);
        }

        [HttpPost]
        public IActionResult Criar(Tarefa tarefa)
        {
            if (tarefa.Data == DateTime.MinValue)
                return BadRequest(new { Erro = "A data da tarefa não pode ser vazia" });

            _context.Add(tarefa);
            _context.SaveChanges();
            return CreatedAtAction(nameof(ObterPorId), new { id = tarefa.Id }, tarefa);
        }

        [HttpPut("{id}")]
        public IActionResult Atualizar(int id, Tarefa tarefa)
        {
            var tarefaBanco = _context.Tarefas.Find(id);

            if (tarefaBanco == null)
                return NotFound();

            if (tarefa.Data == DateTime.MinValue)
                return BadRequest(new { Erro = "A data da tarefa não pode ser vazia" });

            tarefaBanco.Titulo = tarefa.Titulo;
            tarefaBanco.Descricao = tarefa.Descricao;
            tarefaBanco.Data = tarefa.Data;
            tarefaBanco.Status = tarefa.Status;
            _context.Update(tarefaBanco);
            _context.SaveChanges();
            return Ok(tarefaBanco);
        }

        [HttpDelete("{id}")]
        public IActionResult Deletar(int id)
        {
            var tarefaBanco = _context.Tarefas.Find(id);

            if (tarefaBanco == null)
                return NotFound();

            _context.Remove(tarefaBanco);
            _context.SaveChanges();
            return NoContent();
        }
    }
}
