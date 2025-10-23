namespace TaskWeb.Repositories;

using TaskWeb.Models;

public interface IGradeRepository
{
    List<GradeHorario> ReadByTurma(int turmaId);
}
