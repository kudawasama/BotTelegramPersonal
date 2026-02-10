using System;
using System.IO;

namespace BotTelegram.Data
{
    public static class MemoryStore
    {
        private static readonly string FilePath = "data/memories.txt";

        static MemoryStore()
        {
            Directory.CreateDirectory("data");

            if (!File.Exists(FilePath))
                File.Create(FilePath).Dispose();
        }

        public static void Save(long chatId, string text)
        {
            string line = $"{DateTime.Now:yyyy-MM-dd HH:mm:ss} | {chatId} | {text}";
            File.AppendAllText(FilePath, line + Environment.NewLine);
        }
    }
}
