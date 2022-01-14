using Npgsql;
using MonsterTradingCard.Models.Message;
using MESSAGEREPO = MonsterTradingCard.DAL.IMessageRepository;
using System;
using System.Collections.Generic;
using System.Data;

namespace MonsterTradingCard.DAL.DatabaseMessageRepository
{
    class DatabaseMessageRepository : MESSAGEREPO.IMessageRepository
    {
        private const string CreateTableCommand = "CREATE TABLE IF NOT EXISTS messages (id SERIAL PRIMARY KEY, content VARCHAR, username VARCHAR, CONSTRAINT fk_username FOREIGN KEY (username) REFERENCES users(username) ON DELETE CASCADE)";

        private const string InsertMessageCommand = "INSERT INTO messages(content, username) VALUES (@content, @username) RETURNING id";
        private const string DeleteMessageCommand = "DELETE FROM messages WHERE id=@id AND username=@username";
        private const string UpdateMessageCommand = "UPDATE messages SET content=@content WHERE id=@id AND username=@username";
        private const string SelectMessageByIdCommand = "SELECT id, content FROM messages WHERE id=@id AND username=@username";
        private const string SelectMessagesCommand = "SELECT id, content FROM messages WHERE username=@username";

        private readonly NpgsqlConnection _connection;

        public DatabaseMessageRepository(NpgsqlConnection connection)
        {
            _connection = connection;
            EnsureTables();
        }

        public void DeleteMessage(string username, int messageId)
        {
            using var cmd = new NpgsqlCommand(DeleteMessageCommand, _connection);
            cmd.Parameters.AddWithValue("id", messageId);
            cmd.Parameters.AddWithValue("username", username);
            cmd.ExecuteNonQuery();
        }

        public Message GetMessageById(string username, int messageId)
        {
            Message message = null;
            using (var cmd = new NpgsqlCommand(SelectMessageByIdCommand, _connection))
            {
                cmd.Parameters.AddWithValue("id", messageId);
                cmd.Parameters.AddWithValue("username", username);

                // take the first row, if any
                using var reader = cmd.ExecuteReader();
                if (reader.Read())
                {
                    message = ReadMessage(reader);
                }
            }
            return message;
        }

        public IEnumerable<Message> GetMessages(string username)
        {
            var messages = new List<Message>();

            using (var cmd = new NpgsqlCommand(SelectMessagesCommand, _connection))
            {
                cmd.Parameters.AddWithValue("username", username);

                // take the first row, if any
                using var reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    var message = ReadMessage(reader);
                    messages.Add(message);
                }
            }
            return messages;
        }

        public void InsertMessage(string username, Message message)
        {
            using var cmd = new NpgsqlCommand(InsertMessageCommand, _connection);
            cmd.Parameters.AddWithValue("content", message.Content);
            cmd.Parameters.AddWithValue("username", username);
            var result = cmd.ExecuteScalar();

            message.Id = Convert.ToInt32(result);
        }

        public void UpdateMessage(string username, Message message)
        {
            using var cmd = new NpgsqlCommand(UpdateMessageCommand, _connection);
            cmd.Parameters.AddWithValue("id", message.Id);
            cmd.Parameters.AddWithValue("content", message.Content);
            cmd.Parameters.AddWithValue("username", username);
            cmd.ExecuteNonQuery();
        }

        private void EnsureTables()
        {
            using var cmd = new NpgsqlCommand(CreateTableCommand, _connection);
            cmd.ExecuteNonQuery();
        }

        private Message ReadMessage(IDataRecord record)
        {
            var message = new Message
            {
                Id = Convert.ToInt32(record["id"]),
                Content = Convert.ToString(record["content"])
            };

            return message;
        }
    }
}
