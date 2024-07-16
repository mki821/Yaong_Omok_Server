using System.Data;
using System.Data.SQLite;
using System.Security.Cryptography;
using System.Text;

namespace Yaong_Omok_Server {
    public class Database {
        private static string dataSource = $@"Data Source=D:\Github\Yaong_Omok_Server\Yaong_Omok_Server\database.db";
        private static SQLiteDataAdapter? adapter;

        public static List<UserInfo> SelectAll(string table) {
            List<UserInfo> userInfo = [];

            try {
                DataSet ds = new DataSet();

                string sql = $"SELECT * FROM {table}";
                adapter = new SQLiteDataAdapter(sql, dataSource);
                adapter.Fill(ds, table);

                DataTable? dt = ds.Tables[table];

                foreach(DataRow dr in dt.Rows) {
                    UserInfo user = new() {
                        nickname = dr["NickName"].ToString(),
                        password = dr["Password"].ToString(),
                        point = int.Parse(dr["Point"].ToString())
                    };
                    userInfo.Add(user);
                }
            }
            catch (Exception ex) {
                Console.WriteLine(ex.ToString());
            }

            return userInfo;
        }

        public static UserInfo? SelectByNickname(string table, string nickname) {
            UserInfo? userInfo = null;

            try {
                DataSet ds = new DataSet();

                string sql = $"SELECT * FROM {table} WHERE Nickname='{nickname}'";
                adapter = new SQLiteDataAdapter(sql, dataSource);
                adapter.Fill(ds, table);

                if(ds.Tables[table].Rows.Count == 0) return null;
                    
                DataRow dr = ds.Tables[table].Rows[0];

                userInfo = new() {
                    nickname = dr["NickName"].ToString(),
                    password = dr["Password"].ToString(),
                    point = int.Parse(dr["Point"].ToString())
                };
            }
            catch(Exception ex) {
                Console.WriteLine(ex.ToString());
            }

            return userInfo;
        }

        public static void Insert(string table, string value) {
            try {
                using(SQLiteConnection connection = new SQLiteConnection(dataSource)) {
                    connection.Open();

                    string sql = $"INSERT INTO {table} VALUES ({value})";
                    SQLiteCommand cmd = new SQLiteCommand(sql, connection);
                    cmd.ExecuteNonQuery();
                }
            }
            catch(Exception ex) {
                Console.WriteLine(ex.ToString());
            }
        }

        public void Update(string table, string value, string nickname) {
            try {
                using(SQLiteConnection connection = new SQLiteConnection(dataSource)) {
                    connection.Open();
                    string sql = $"UPDATE {table} SET {value} WHERE Nickname='{nickname}'";
                    SQLiteCommand cmd = new SQLiteCommand(sql, connection);
                    cmd.ExecuteNonQuery();
                }
            }
            catch(Exception ex) {
                Console.WriteLine(ex.ToString());
            }
        }

        public static string SHA256HASH(string password) {
            SHA256 sha = new SHA256Managed();
            byte[] hash = sha.ComputeHash(Encoding.ASCII.GetBytes(password));
            StringBuilder builder = new StringBuilder();

            foreach(byte b in hash) {
                builder.AppendFormat("{0:x2}", b);
            }

            return builder.ToString();
        }
    }
}
