using SQLite;

namespace SolexMobile.Data
{
    public interface ISQLite
    {
        SQLiteConnection GetConnection();
    }
}
