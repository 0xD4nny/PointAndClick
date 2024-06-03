namespace PointAndClick0xDanny
{
    public class Entry(int score, int missHits, int accuracy, int level, int gameTime, string playerName, DateTime entryCreateTime)
    {
        public int Score = score, MissHits = missHits, Accuracy = accuracy, Level = level, GameTime = gameTime;
        public string PlayerName = playerName;
        public DateTime EntryCreateTime = entryCreateTime;
    }
}
