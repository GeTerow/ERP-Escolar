namespace TaskWeb.Services;

using Microsoft.Extensions.DependencyInjection;
using TaskWeb.Models;
using TaskWeb.Repositories;

public static class DatabaseSeeder
{
    public static void Seed(IServiceProvider services)
    {
        using var scope = services.CreateScope();
        var seeder = new Seeder(
            scope.ServiceProvider.GetRequiredService<ITurnoRepository>(),
            scope.ServiceProvider.GetRequiredService<ISlotAulaRepository>(),
            scope.ServiceProvider.GetRequiredService<IProfessorRepository>(),
            scope.ServiceProvider.GetRequiredService<IDisponibilidadeProfessorRepository>());
        seeder.Run();
    }

    private sealed class Seeder
    {
        private readonly ITurnoRepository _turnoRepository;
        private readonly ISlotAulaRepository _slotRepository;
        private readonly IProfessorRepository _professorRepository;
        private readonly IDisponibilidadeProfessorRepository _disponibilidadeRepository;

        private readonly SlotTemplate[] _morningSlots = new[]
        {
            new SlotTemplate(1, new TimeSpan(7, 30, 0), new TimeSpan(8, 20, 0), false),
            new SlotTemplate(2, new TimeSpan(8, 20, 0), new TimeSpan(9, 10, 0), false),
            new SlotTemplate(3, new TimeSpan(9, 10, 0), new TimeSpan(10, 0, 0), false),
            new SlotTemplate(4, new TimeSpan(10, 0, 0), new TimeSpan(10, 20, 0), true),
            new SlotTemplate(5, new TimeSpan(10, 20, 0), new TimeSpan(11, 10, 0), false),
            new SlotTemplate(6, new TimeSpan(11, 10, 0), new TimeSpan(12, 0, 0), false),
            new SlotTemplate(7, new TimeSpan(12, 0, 0), new TimeSpan(12, 30, 0), false)
        };

        private readonly SlotTemplate[] _afternoonSlots = new[]
        {
            new SlotTemplate(1, new TimeSpan(13, 0, 0), new TimeSpan(13, 50, 0), false),
            new SlotTemplate(2, new TimeSpan(13, 50, 0), new TimeSpan(14, 40, 0), false),
            new SlotTemplate(3, new TimeSpan(14, 40, 0), new TimeSpan(15, 30, 0), false),
            new SlotTemplate(4, new TimeSpan(15, 30, 0), new TimeSpan(15, 50, 0), true),
            new SlotTemplate(5, new TimeSpan(15, 50, 0), new TimeSpan(16, 40, 0), false),
            new SlotTemplate(6, new TimeSpan(16, 40, 0), new TimeSpan(17, 30, 0), false),
            new SlotTemplate(7, new TimeSpan(17, 30, 0), new TimeSpan(18, 0, 0), false)
        };

        public Seeder(
            ITurnoRepository turnoRepository,
            ISlotAulaRepository slotRepository,
            IProfessorRepository professorRepository,
            IDisponibilidadeProfessorRepository disponibilidadeRepository)
        {
            _turnoRepository = turnoRepository;
            _slotRepository = slotRepository;
            _professorRepository = professorRepository;
            _disponibilidadeRepository = disponibilidadeRepository;
        }

        public void Run()
        {
            var turnos = EnsureTurnos();
            var slotsPorTurno = EnsureSlots(turnos);
            EnsureDisponibilidade(slotsPorTurno);
        }

        private List<Turno> EnsureTurnos()
        {
            var turnos = _turnoRepository.ReadAll();
            if (turnos.Count == 0)
            {
                _turnoRepository.Create(new Turno
                {
                    Nome = "Manha",
                    HoraInicio = new TimeSpan(7, 30, 0),
                    HoraFim = new TimeSpan(12, 30, 0)
                });
                _turnoRepository.Create(new Turno
                {
                    Nome = "Tarde",
                    HoraInicio = new TimeSpan(13, 0, 0),
                    HoraFim = new TimeSpan(18, 0, 0)
                });
                turnos = _turnoRepository.ReadAll();
            }

            return turnos;
        }

        private Dictionary<int, List<SlotAula>> EnsureSlots(List<Turno> turnos)
        {
            var slots = _slotRepository.ReadAll();
            if (slots.Count == 0)
            {
                foreach (var turno in turnos)
                {
                    var templates = GetTemplatesForTurno(turno.Nome);
                    foreach (var template in templates)
                    {
                        _slotRepository.Create(new SlotAula
                        {
                            TurnoId = turno.TurnoId,
                            Sequencia = template.Sequencia,
                            HoraInicio = template.Inicio,
                            HoraFim = template.Fim,
                            EhIntervalo = template.EhIntervalo
                        });
                    }
                }

                slots = _slotRepository.ReadAll();
            }

            return slots
                .GroupBy(s => s.TurnoId)
                .ToDictionary(
                    g => g.Key,
                    g => g.Where(s => !s.EhIntervalo)
                          .OrderBy(s => s.Sequencia)
                          .ToList());
        }

        private void EnsureDisponibilidade(Dictionary<int, List<SlotAula>> slotsPorTurno)
        {
            if (_disponibilidadeRepository.ReadAll().Count > 0)
            {
                return;
            }

            var professores = _professorRepository.ReadAll();
            if (professores.Count == 0 || slotsPorTurno.Count == 0)
            {
                return;
            }

            var turnoIds = slotsPorTurno.Keys.OrderBy(id => id).ToArray();
            int cursor = 0;
            foreach (var professor in professores)
            {
                var turnoId = turnoIds[cursor % turnoIds.Length];
                cursor++;

                if (!slotsPorTurno.TryGetValue(turnoId, out var slots) || slots.Count == 0)
                {
                    continue;
                }

                for (int dia = 1; dia <= 5; dia++)
                {
                    foreach (var slot in slots)
                    {
                        _disponibilidadeRepository.Create(new DisponibilidadeProfessor
                        {
                            ProfessorId = professor.ProfessorId,
                            DiaSemana = dia,
                            SlotAulaId = slot.SlotAulaId
                        });
                    }
                }
            }
        }

        private SlotTemplate[] GetTemplatesForTurno(string nomeTurno)
        {
            return nomeTurno?.Trim().Equals("Tarde", StringComparison.OrdinalIgnoreCase) == true
                ? _afternoonSlots
                : _morningSlots;
        }
    }

    private record SlotTemplate(int Sequencia, TimeSpan Inicio, TimeSpan Fim, bool EhIntervalo);
}
