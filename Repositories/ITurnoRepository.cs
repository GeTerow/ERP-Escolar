namespace TaskWeb.Repositories;

using TaskWeb.Models;

public interface ITurnoRepository
{
    List<Turno> ReadAll();
    Turno? Read(int id);
    void Create(Turno turno);
    void Update(Turno turno);
    void Delete(int id);
}
