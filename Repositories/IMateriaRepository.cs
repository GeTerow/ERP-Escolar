namespace TaskWeb.Repositories;

using TaskWeb.Models;

public interface IMateriaRepository
{
    List<Materia> ReadAll();
    List<Materia> ReadByTurma(int turmaId);
    Materia? Read(int id);
    void Create(Materia materia);
    void Update(Materia materia);
    void Delete(int id);
}
