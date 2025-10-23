namespace TaskWeb.Repositories;

using TaskWeb.Models;

public interface IAlunoRepository
{
    List<Aluno> ReadAll();
    Aluno? Read(int id);
    void Create(Aluno aluno);
    void Update(Aluno aluno);
    void Delete(int id);
}
