using Npgsql;
using System.Configuration;

namespace SP2
{
    public static class DB
    {
        public static NpgsqlConnection Connection = new NpgsqlConnection(ConfigurationManager.ConnectionStrings["users"].ConnectionString);
    }

}
