namespace SecretsAPI.Repos;
using Ro.SQLite.Data;
using System.Threading.Tasks;
using SecretsAPI.Models;
using System.Data;
using System.Collections.Generic;

public interface ISecretsRepo 
{
    Task<Secret> GetOne(Guid userId, Guid id);
    Task<IEnumerable<Secret>> GetAll(Guid userId);
    Task<int> Create(Secret secret);
    Task<int> Update(Secret secret);
    Task<int> Delete(Guid userId, Guid id); 
}

public class SecretsRepo: ISecretsRepo 
{
    private const string DATE_FORMAT = "yyyy-MM-dd HH:mm:ss.fff";   
    private IDbAsync Db { get; set; }
    public SecretsRepo(IDbAsync db)
    {
        this.Db = db;
    }

    private Secret GetData(IDataReader dr) 
    {
        return new Secret()
        {
            Id = Guid.Parse(dr.GetString("Id")),
            UserId = Guid.Parse(dr.GetString("UserId")),
            Content = dr.GetString("Content"),
            DateCreated = DateTime.Parse(dr.GetString("DateCreated")),
            DateModified = DateTime.Parse(dr.GetString("DateModified"))
        };
    } 

    public Task<Secret> GetOne(Guid userId, Guid id)
    {
        string sql = "SELECT Id, UserId, Content, DateCreated, DateModified FROM Secrets WHERE Id=@Id and UserId=@UserId;";
        var cmd = sql.ToCmd
        (
            "@Id".ToParam(DbType.String, id.ToString()),
            "@UserId".ToParam(DbType.String, userId.ToString())
        );

        return this.Db.GetOneRow(cmd, GetData);
    }
    
    public Task<IEnumerable<Secret>> GetAll(Guid userId)
    {
        string sql = "SELECT Id, UserId, Content, DateCreated, DateModified FROM Secrets WHERE UserId=@UserId;";
        var cmd = sql.ToCmd
        (
            "@UserId".ToParam(DbType.String, userId.ToString())
        );

        return this.Db.GetRows(cmd, GetData);
    }


    public Task<int> Create(Secret s)
    {
        var now = DateTime.Now;
        string sql = "INSERT INTO Secrets (Id,UserId,Content,DateCreated,DateModified) VALUES (@Id,@UserId,@Content,@DateCreated,@DateModified);";
        var cmd = sql.ToCmd
        (
            "@Id".ToParam(DbType.String, s.Id.ToString()),
            "@UserId".ToParam(DbType.String, s.UserId.ToString()),
            "@Content".ToParam(DbType.String, s.Content),
            "@DateCreated".ToParam(DbType.String, now.ToString(DATE_FORMAT)),
            "@DateModified".ToParam(DbType.String, now.ToString(DATE_FORMAT))
        );

        return this.Db.ExecuteNonQuery(cmd);
    }

    public Task<int> Update(Secret s)
    {
        string sql = "UPDATE Secrets Content=@Content, DateModified=@DateModified WHERE Id=@Id AND UserId=@UserId;";
        var cmd = sql.ToCmd
        (
            "@Id".ToParam(DbType.String, s.Id.ToString()),
            "@UserId".ToParam(DbType.String, s.UserId.ToString()),
            "@Content".ToParam(DbType.String, s.Content),
            "@DateModified".ToParam(DbType.String, DateTime.Now.ToString(DATE_FORMAT))
        );

        return this.Db.ExecuteNonQuery(cmd);
    }

    public Task<int> Delete(Guid userId, Guid id)
    {
        string sql = "DELETE FROM Secrets WHERE Id=@Id AND UserId=@UserId;";
        var cmd = sql.ToCmd
        (
            "@Id".ToParam(DbType.String, id.ToString()),
            "@UserId".ToParam(DbType.String, userId.ToString())
        );

        return this.Db.ExecuteNonQuery(cmd);
    }
}