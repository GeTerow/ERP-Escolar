namespace TaskWeb.Repositories;

using TaskWeb.Models;

public interface IGradeRepository
{
    List<GradeHorario> ReadByTurma(int turmaId);
    List<GradeHorario> ReadByProfessor(int professorId);
    GradeHorario? Read(int id);
    void Create(GradeHorario horario);
    void Update(GradeHorario horario);
    void Delete(int id);
    void DeleteByTurma(int turmaId);
}

