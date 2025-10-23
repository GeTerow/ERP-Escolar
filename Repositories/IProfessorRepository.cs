namespace TaskWeb.Repositories;

using TaskWeb.Models;

public interface IProfessorRepository
{
    List<Professor> ReadAll();
    Professor? Read(int id);
    void Create(Professor professor);
    void Update(Professor professor);
    void Delete(int id);
}
