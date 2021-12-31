namespace SecretsAPI.Repos;
using Ro.SQLite.Data;
using System.Threading.Tasks;
using SecretsAPI.Models;
using System.Data;
using System.Collections.Generic;
using System.Security.Cryptography;

public interface IUsersRepo 
{
    Task<User> GetOne(string username);
    Task<Guid> Create(Login m);
    Task<int> Delete(Guid id);
    Task<bool> HasAccess(Login m);
    Task<int> ChangePassword(Guid userId, ChangePassword model);
}

public class UsersRepo : IUsersRepo
{
    private const string DATE_FORMAT = "yyyy-MM-dd HH:mm:ss.fff";   
    private IDbAsync Db { get; set; }
    public UsersRepo(IDbAsync db)
    {
        this.Db = db;
    }

    private User GetData(IDataReader dr) 
    {
        return new User()
        {
            Id = Guid.Parse(dr.GetString("Id")),
            Username = dr.GetString("Username"),
            DateCreated = DateTime.Parse(dr.GetString("DateCreated")),
            IsActive = dr.GetInt("IsActive") == 1
        };
    } 

    public Task<User> GetOne(string username)
    {
        string sql = "SELECT Id, Username, DateCreated, IsActive FROM Users WHERE Username=@Username;";
        var cmd = sql.ToCmd
        (
            "@Username".ToParam(DbType.String, username)
        );

        return this.Db.GetOneRow(cmd, GetData);
    }

    private void CreatePasswordHash(string password, out byte[] passwordHash, out byte[] passwordSalt)
    {
        using(var hmac = new HMACSHA512())
        {
            passwordSalt = hmac.Key;
            passwordHash = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));
        }
    }

    private bool VerifyPasswordHash(string password, byte[] hash, byte[] salt)
    {
        using(var hmac = new HMACSHA512(salt))
        {
            var computedHash = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));
            return computedHash.SequenceEqual(hash);
        }
    }

    public async Task<Guid> Create(Login m)
    {
        var id = Guid.NewGuid();
        CreatePasswordHash(m.Password, out byte[] hash, out byte[] salt);
        string sql = "INSERT INTO Users (Id,Username,PasswordHash,PasswordSalt,DateCreated,IsActive) VALUES (@Id,@Username,@PasswordHash,@PasswordSalt,@DateCreated,@IsActive);";
        var cmd = sql.ToCmd
        (
            "@Id".ToParam(DbType.String, id.ToString()),
            "@Username".ToParam(DbType.String, m.Username),
            "@PasswordHash".ToParam(DbType.Binary, hash),
            "@PasswordSalt".ToParam(DbType.Binary, salt),
            "@DateCreated".ToParam(DbType.String, DateTime.Now.ToString(DATE_FORMAT)),
            "@IsActive".ToParam(DbType.Int32, 0)
        );

        await this.Db.ExecuteNonQuery(cmd);
        return id;
    }

    public Task<int> Delete(Guid id)
    {
        string sql = "DELETE FROM Users WHERE Id=@Id;";
        var cmd = sql.ToCmd
        (
            "@Id".ToParam(DbType.String, id.ToString())
        );

        return this.Db.ExecuteNonQuery(cmd);
    }

    public async Task<bool> HasAccess(Login m)
    {
        var user = await GetOne(m.Username);
        if(user == null || user.IsActive == false)
        {
            return false;
        }
        string sql = "SELECT PasswordHash, PasswordSalt FROM Users WHERE Id=@Id;";
        var cmd = sql.ToCmd("@Id".ToParam(DbType.String, user.Id.ToString()));
        var tuple = await Db.GetOneRow(cmd, dr => {
            (byte[] hash, byte[] salt) t = 
            (
                (byte[])dr["PasswordHash"],
                (byte[])dr["PasswordSalt"]
            );
            
            return t;            
        });

        return VerifyPasswordHash(m.Password, tuple.hash, tuple.salt);
    }

    public Task<int> ChangePassword(Guid userId, ChangePassword model)
    {
        CreatePasswordHash(model.Password, out byte[] hash, out byte[] salt);   
        string sql = "UPDATE Users SET PasswordHash=@PasswordHash, PasswordSalt=@PasswordSalt WHERE UserId=@UserId;";
        var cmd = sql.ToCmd
        (
            "@UserId".ToParam(DbType.String, userId.ToString()),
            "@PasswordHash".ToParam(DbType.Binary, hash),
            "@PasswordSalt".ToParam(DbType.Binary, salt)
        );

        return this.Db.ExecuteNonQuery(cmd);
    }
}