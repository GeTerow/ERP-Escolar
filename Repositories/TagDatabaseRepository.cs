namespace TaskWeb.Repositories;

using System.Collections.Generic;
using TaskWeb.Models;
using Microsoft.Data.SqlClient;

public class TagDatabaseRepository : DbConnection, ITagRepository
{
    public TagDatabaseRepository(string? strConn) : base(strConn)
    {
        
    }

    public void Create(Tag tag)
    {
        throw new NotImplementedException();
    }

    public void Delete(int id)
    {
        throw new NotImplementedException();
    }

    public List<Tag> Read()
    {
        throw new NotImplementedException();
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