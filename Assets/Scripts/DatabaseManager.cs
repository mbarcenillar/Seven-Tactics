using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using Mono.Data.SqliteClient;
using UnityEngine;

public class DatabaseManager : MonoBehaviour
{
    private static DatabaseManager instance;

    public string dbPath { get; private set; }
    public bool IsReady { get; private set; } = false;
    
    private void Awake()
    {
        // Asegurar que solo exista una instancia
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject); // Hacer que este objeto persista entre escenas
            InitializeDatabase();
        }
        else
        {
            Destroy(gameObject); // Eliminar instancias adicionales
        }
    }

    private void InitializeDatabase()
    {
        dbPath = Path.Combine(Application.persistentDataPath, "GameDatabase.db");
        Debug.Log("Ruta de la base de datos: " + dbPath);

        if (!File.Exists(dbPath))
        {
            Debug.Log("Creando nueva base de datos...");
            CreateDatabase();
        }
        else
        {
            Debug.Log("Base de datos ya existente.");
        }

        IsReady = true;
    }

    //Crear base de datos
    private void CreateDatabase()
    {
        using (var connection = new SqliteConnection($"URI = file:{dbPath}")) 
        {
            connection.Open();
            using (var command = connection.CreateCommand())
            {
                //Crear la tabla de equipos
                command.CommandText = @"
                    CREATE TABLE IF NOT EXISTS Teams (
                        Id INTEGER PRIMARY KEY AUTOINCREMENT,
                        Name TEXT NOT NULL
                    );";
                command.ExecuteNonQuery();

                //Crear la tabla de jugadores
                command.CommandText = @"
                    CREATE TABLE IF NOT EXISTS Players (
                        Id INTEGER PRIMARY KEY AUTOINCREMENT,
                        Name TEXT,
                        Surname TEXT,
                        Age INTENGER,
                        Attack INTEGER,
                        Defense INTEGER,
                        Stamina INTEGER,
                        TeamId INTEGER,
                        FOREIGN KEY(TeamId) REFERENCES Teams(Id)
                    );";
                command.ExecuteNonQuery();

                //Crear la tabla de entrenadores
                command.CommandText = @"
                    CREATE TABLE IF NOT EXISTS Coaches (
                        Id INTEGER PRIMARY KEY AUTOINCREMENT,
                        Name TEXT,
                        Surname TEXT,
                        Age INTEGER,
                        TeamId INTEGER,
                        FOREIGN KEY(TeamId) REFERENCES Teams(Id)
                    );";
                command.ExecuteNonQuery();

                //Crear la tabla partidas guardadas
                command.CommandText = @"
                    CREATE TABLE IF NOT EXISTS SavedGames (
                        GameId INTEGER PRIMARY KEY AUTOINCREMENT,
                        GameName TEXT NOT NULL
                    );";
                command.ExecuteNonQuery();
            }
            connection.Close();
        }
        Debug.Log("Base de datos creada en : " +  dbPath);
    }

    //A�adir datos la BBDD mediante archivos CSV
    /*private void LoadDataFromCSV()
    {
        //A�adir jugadores
        string PlayersPath = Path.Combine(Application.streamingAssetsPath, "players.csv");
        if (File.Exists(PlayersPath))
        {
            var lines = File.ReadAllLines(PlayersPath);
            foreach (var line in lines.Skip(1))
            {
                var data = line.Split(",");
                AddPlayer(data[0], data[1], int.Parse(data[2]), int.Parse(data[3]), int.Parse(data[4]), int.Parse(data[5]), int.Parse(data[6]));
            }
        }

        //A�adir equipos
        string TeamsPath = Path.Combine(Application.streamingAssetsPath, "teams.csv");
        if (File.Exists(TeamsPath))
        {
            var lines = File.ReadAllLines(TeamsPath);
            foreach(var line in lines.Skip(1))
            { 
                var data = line.Split(",");
                AddTeam(data[0]);
            }
        }
        Debug.Log("Datos iniciales cargados desde CSV.");

        //A�adir entrenadores
        string CoachesPath = Path.Combine(Application.streamingAssetsPath, "coaches.csv");
        if (File.Exists(CoachesPath))
        {
            var lines = File.ReadAllLines(CoachesPath);
            foreach (var line in lines.Skip(1))
            {
                var data = line.Split(",");
                AddCoach(data[0], data[1], int.Parse(data[2]), int.Parse(data[3]));
            }
        }
    }*/

    private void Start()
    {
        //Ruta de la base de datos
        dbPath = Path.Combine(Application.persistentDataPath, "GameDatabase.db");

        //Verificar si existe
        if (!File.Exists(dbPath))
        {
            Debug.Log("Creando nueva base de datos...");
            Debug.Log("Ruta de la base de datos: " + dbPath);
            CreateDatabase();
            VerifyTables();
        }
        else
        {
            Debug.Log("Base de datos ya existente.");
            VerifyTables();
            //LoadDataFromCSV();
        }
    }

    //A�adir jugador en la tabla
    public void AddPlayer(string name, string surname, int age, int attack, int defense, int stamina, int teamId)
    {
        using (var connection = new SqliteConnection($"URI = file:{dbPath}"))
        {
            connection.Open();
            using (var command = connection.CreateCommand())
            {
                command.CommandText = @"
                    INSERT INTO Players (Name, Surname, Age, Attack, Defense, Stamina, TeamId) 
                    VALUES (@Name, @Surname, @Age, @Attack, @Defense, @Stamina, @TeamId);";
                command.Parameters.Add(new SqliteParameter("@Name", name));
                command.Parameters.Add(new SqliteParameter("@Surname", surname));
                command.Parameters.Add(new SqliteParameter("@Age", age));
                command.Parameters.Add(new SqliteParameter("@Attack", attack));
                command.Parameters.Add(new SqliteParameter("@Defense", defense));
                command.Parameters.Add(new SqliteParameter("@Stamina", stamina));
                command.Parameters.Add(new SqliteParameter("@TeamId", teamId));
                command.ExecuteNonQuery();
            }
            connection.Close();
        }
        Debug.Log($"Jugador añadido: {name} {surname}");
    }

    //A�adir entrenador en la tabla
    public void AddCoach(string name, string surname, int age, int teamId)
    {
        using (var connection = new SqliteConnection($"URI = file:{dbPath}"))
        {
            connection.Open();
            using (var command = connection.CreateCommand())
            {
                command.CommandText = @"
                    INSERT INTO Coaches (Name, Surname, Age, TeamId)
                    VALUES (@Name, @Surname, @Age, @TeamId);";
                command.Parameters.Add(new SqliteParameter("@Name", name));
                command.Parameters.Add(new SqliteParameter("@Surname", surname));
                command.Parameters.Add(new SqliteParameter("@Age", age));
                command.Parameters.Add(new SqliteParameter("@TeamId", teamId));
                command.ExecuteNonQuery();
            }
            connection.Close();
        }
        Debug.Log($"Entrenador a�adido: {name} {surname}");
    }

    //A�adir equipo a la tabla
    public void AddTeam(string name)
    {
        using (var connection = new SqliteConnection($"URI = file:{dbPath}"))
        {
            connection.Open();
            using (var command = connection.CreateCommand())
            {
                command.CommandText = @"
                    INSERT INTO Teams (Name)
                    VALUES (@Name);";
                command.Parameters.Add(new SqliteParameter("@Name", name));
                command.ExecuteNonQuery();
            }
            connection.Close();
        }
        Debug.Log($"Equipo añadido: {name}");
    }

    //Consultar tabla jugadores
    public void ReadPlayers()
    {
        using (var connection = new SqliteConnection($"URI = file:{dbPath}"))
        {
            connection.Open();
            using (var command = connection.CreateCommand())
            {
                command.CommandText = "SELECT * FROM Players;";
                using (IDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        Debug.Log($"Jugador: {reader["Name"]} {reader["Surname"]}, Edad: {reader["Age"]}, Ataque: {reader["Attack"]}, Defensa: {reader["Defense"]}, Resistencia: {reader["Stamina"]}, Equipo: {reader["TeamId"]}");
                    }
                }
            }
            connection.Close();
        }
    }

    //Consultar tabla entrenadores
    public void ReadCoaches()
    {
        using (var connection = new SqliteConnection($"URI = file:{dbPath}"))
        {
            connection.Open();
            using (var command = connection.CreateCommand())
            {
                command.CommandText = "SELECT * FROM Coaches;";
                using (IDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        Debug.Log($"Entrenador: {reader["Name"]} {reader["Surname"]}, Edad: {reader["Age"]}, Equipo: {reader["TeamId"]}");
                    }
                }
            }
            connection.Close();
        }
    }

    //Consultar tabla equipos y crear tupla
    public List<(int TeamId, string TeamName)> ReadTeams()
    {
        var teams = new List<(int TeamId, string TeamName)>();

        using (var connection = new SqliteConnection($"URI = file:{dbPath}"))
        {
            connection.Open();
            using (var command = connection.CreateCommand())
            {
                command.CommandText = "SELECT * FROM Teams;";
                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        int teamId = reader.GetInt32(0);
                        string teamName = reader.GetString(1);
                        teams.Add((teamId, teamName));
                    }
                }
            }
        }
        return teams;
    }

    //Buscar equipor Id
    public (int TeamId, string TeamName)? GetTeamById (int teamId)
    {
        using (var connection = new SqliteConnection($"URI=file:{dbPath}"))
        {
            connection.Open();
            using (var command = connection.CreateCommand())
            {
                command.CommandText = "SELECT TeamId, TeamName FROM Teams WHERE TeamId = @TeamId;";
                command.Parameters.Add(new SqliteParameter("@TeamId", teamId));

                using (var reader = command.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        int id = reader.GetInt32(0);
                        string name = reader.GetString(1);
                        return (id, name);
                    }
                }
            }
        }
        Debug.LogWarning($"No se encontró un equipo con el ID {teamId}.");
        return null;
    }

    //Buscar equipo por nombre
    public (int TeamId, string TeamName)? GetTeamByName(string name)
    {
        using (var connection = new SqliteConnection($"URI=file:{dbPath}"))
        {
            connection.Open();
            using (var command = connection.CreateCommand())
            {
                command.CommandText = "SELECT Id, Name FROM Teams WHERE Name LIKE @Name;";
                command.Parameters.Add(new SqliteParameter("@Name", $"%{name}%"));

                using (var reader = command.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        int teamId = reader.GetInt32(0);
                        string teamName = reader.GetString(1);
                        return(teamId, teamName);
                    }
                }
            }
        }
        return null;
    }

    //Añadir partidas a la tabla
    public void AddSavedGame(string gameName)
    {
        using (var connection = new SqliteConnection($"URI=file:{dbPath}"))
        {
            connection.Open();
            using (var command = connection.CreateCommand())
            {
                command.CommandText = @"
                    INSERT INTO SavedGames(GameName)
                    VALUES (@GameName);";
                command.Parameters.Add(new SqliteParameter("@GameName", gameName));

                command.ExecuteNonQuery();
            }
        }
    }

    //Eliminar partidas guardadas
    public void DeleteSavedGame(int gameId)
    {
        using (var connection = new SqliteConnection($"URI=file:{dbPath}"))
        {
            connection.Open();
            using (var command = connection.CreateCommand())
            {
                command.CommandText = "DELETE FROM SavedGames WHERE GameId = @GameId;";
                command.Parameters.Add(new SqliteParameter("@GameId", gameId));

                command.ExecuteNonQuery();
            }
        }
    }

    //Consultar tabla partidas guardadas
    public List<(int GameId, string GameName)> GetAllSavedGames()
    {
        var savedGames = new List<(int, string)>();
        using (var connection = new SqliteConnection($"URI=file:{dbPath}"))
        {
            connection.Open();
            using (var command = connection.CreateCommand())
            {
                command.CommandText = "SELECT GameId, GameName FROM SavedGames;";
                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        int gameId = reader.GetInt32(0);
                        string gameName = reader.GetString(1);
                        Debug.Log($"Partida guardada: ID = {gameId}, Nombre = {gameName}");
                        savedGames.Add((gameId, gameName));
                    }
                }
            }
        }
        return savedGames;
    } 

    //Cargar partida guardada
    public string LoadGameById(int gameId)
    {
        using (var connection = new SqliteConnection($"URI=file:{dbPath}"))
        {
            connection.Open();
            using (var command = connection.CreateCommand())
            {
                command.CommandText = @"
                    SELECT GameName
                    FROM SavedGames
                    WHERE GameId = @GameId;";

                command.Parameters.Add(new SqliteParameter("@GameId", gameId));
                using (var reader = command.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        return (reader.GetString(0));
                    }
                }
            }
        }
        return null; // Si no se encuentra la partida
    }

    private void VerifyTables()
{
    using (var connection = new SqliteConnection($"URI=file:{dbPath}"))
    {
        connection.Open();
        using (var command = connection.CreateCommand())
        {
            command.CommandText = "SELECT name FROM sqlite_master WHERE type='table';";
            using (var reader = command.ExecuteReader())
            {
                while (reader.Read())
                {
                    Debug.Log($"Tabla existente: {reader.GetString(0)}");
                }
            }
        }
    }
}
}
