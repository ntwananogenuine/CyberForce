using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace POE
    {
        public class CyberTask
        {
            public int Id { get; set; }
            public string Title { get; set; }
            public string Description { get; set; }
            public string Reminder { get; set; }
            public bool IsCompleted { get; set; }
            public DateTime CreatedAt { get; set; }

            public override string ToString()
            {
                string status = IsCompleted ? "[Done]" : "[Pending]";
                string reminder = string.IsNullOrWhiteSpace(Reminder) ? "None" : Reminder;
                return $"{status} {Title} — {Description} (Reminder: {reminder})";
            }
        }

        public class TaskDatabase
        {
            private const string ConnectionString =
                "Server=127.0.0.1;Database=cyberforce_db;Uid=root;Pwd=FLAMEZ2026#;";

            public static void Initialise()
            {
                using (var con = new MySqlConnection("Server=127.0.0.1;Port=3306;Uid=root;Pwd=FLAMEZ2026#;"))
                {
                    con.Open();
                    var cmd = con.CreateCommand();
                    cmd.CommandText =
                        "CREATE DATABASE IF NOT EXISTS cyberforce_db CHARACTER SET utf8mb4;";
                    cmd.ExecuteNonQuery();
                }

                using (var con = new MySqlConnection(ConnectionString))
                {
                    con.Open();
                    var cmd = con.CreateCommand();
                    cmd.CommandText = @"
                    CREATE TABLE IF NOT EXISTS tasks (
                        id          INT AUTO_INCREMENT PRIMARY KEY,
                        title       VARCHAR(200) NOT NULL,
                        description TEXT,
                        reminder    VARCHAR(100),
                        is_completed TINYINT(1) DEFAULT 0,
                        created_at  DATETIME DEFAULT CURRENT_TIMESTAMP
                    );";
                    cmd.ExecuteNonQuery();
                }
            }

            public static int AddTask(string title, string description, string reminder)
            {
                using (var con = new MySqlConnection(ConnectionString))
                {
                    con.Open();
                    var cmd = con.CreateCommand();
                    cmd.CommandText =
                        "INSERT INTO tasks (title, description, reminder) " +
                        "VALUES (@t, @d, @r); SELECT LAST_INSERT_ID();";
                    cmd.Parameters.AddWithValue("@t", title ?? "");
                    cmd.Parameters.AddWithValue("@d", description ?? "");
                    cmd.Parameters.AddWithValue("@r", reminder ?? "");
                    return Convert.ToInt32(cmd.ExecuteScalar());
                }
            }

            public static List<CyberTask> GetAllTasks()
            {
                var list = new List<CyberTask>();
                using (var con = new MySqlConnection(ConnectionString))
                {
                    con.Open();
                    var cmd = con.CreateCommand();
                    cmd.CommandText =
                        "SELECT id, title, description, reminder, is_completed, created_at " +
                        "FROM tasks ORDER BY id DESC;";
                    using (var r = cmd.ExecuteReader())
                    {
                        while (r.Read())
                        {
                            list.Add(new CyberTask
                            {
                                Id = r.GetInt32(0),
                                Title = r.GetString(1),
                                Description = r.GetString(2),
                                Reminder = r.IsDBNull(3) ? "" : r.GetString(3),
                                IsCompleted = r.GetInt32(4) == 1,
                                CreatedAt = r.GetDateTime(5)
                            });
                        }
                    }
                }
                return list;
            }

            public static bool CompleteTask(int id)
            {
                using (var con = new MySqlConnection(ConnectionString))
                {
                    con.Open();
                    var cmd = con.CreateCommand();
                    cmd.CommandText = "UPDATE tasks SET is_completed = 1 WHERE id = @id;";
                    cmd.Parameters.AddWithValue("@id", id);
                    return cmd.ExecuteNonQuery() > 0;
                }
            }

            public static bool DeleteTask(int id)
            {
                using (var con = new MySqlConnection(ConnectionString))
                {
                    con.Open();
                    var cmd = con.CreateCommand();
                    cmd.CommandText = "DELETE FROM tasks WHERE id = @id;";
                    cmd.Parameters.AddWithValue("@id", id);
                    return cmd.ExecuteNonQuery() > 0;
                }
            }
        }
    }

