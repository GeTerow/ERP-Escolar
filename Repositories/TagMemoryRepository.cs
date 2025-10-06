namespace TaskWeb.Repositories;

using System.Collections.Generic;
using System.Collections.Immutable;
using TaskWeb.Models;

public class TagMemoryRepository : ITagRepository
{
    private List<Tag> lista = new List<Tag>();

    public void Create(Tag tag)
    {
        tag.TagId = Math.Abs((int)DateTimeOffset.Now.ToUnixTimeMilliseconds());
        lista.Add(tag);
    }

    public void Delete(int id)
    {
        throw new NotImplementedException();
    }

    public List<Tag> Read()
    {
        return lista;
    }

    public Tag Read(int id)
    {
        throw new NotImplementedException();
    }

    public void Update(Tag tag)
    {
        throw new NotImplementedException();
    }
}