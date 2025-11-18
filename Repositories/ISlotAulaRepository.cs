namespace TaskWeb.Repositories;

using TaskWeb.Models;

public interface ISlotAulaRepository
{
    List<SlotAula> ReadAll();
    List<SlotAula> ReadByTurno(int turnoId);
    SlotAula? Read(int id);
    void Create(SlotAula slot);
    void Update(SlotAula slot);
    void Delete(int id);
}
