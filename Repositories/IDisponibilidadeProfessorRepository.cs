namespace TaskWeb.Repositories;

using TaskWeb.Models;

public interface IDisponibilidadeProfessorRepository
{
    List<DisponibilidadeProfessor> ReadAll();
    List<DisponibilidadeProfessor> ReadByProfessor(int professorId);
    List<DisponibilidadeProfessor> ReadByProfessorAndDia(int professorId, int diaSemana);
    void Create(DisponibilidadeProfessor disponibilidade);
    void Delete(int id);
    void DeleteByProfessor(int professorId);
}
