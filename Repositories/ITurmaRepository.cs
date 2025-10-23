namespace TaskWeb.Repositories;

using TaskWeb.Models;

public interface ITurmaRepository
{
    List<Turma> ReadAll();
    Turma? Read(int id);
    void Create(Turma turma);
    void Update(Turma turma);
    void Delete(int id);
}
