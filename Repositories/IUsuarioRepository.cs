namespace TaskWeb.Repositories;

using TaskWeb.Models;

public interface IUsuarioRepository
{
    Usuario? Login(LoginViewModel model);
    
    List<Usuario> ReadAll();
    Usuario? Read(int id);
    void Create(Usuario usuario);
    void Update(Usuario usuario);
    void Delete(int id);
}