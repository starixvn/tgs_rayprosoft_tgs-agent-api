namespace TGS.Core.Infrastructures.Redis
{
    public class ScoredItem<T>
    {
        public ScoredItem(double score, T item)
        {
            Score = score;
            Item = item;
        }

        public T Item { get; set; }
        public double Score { get; set; }
    }
}
