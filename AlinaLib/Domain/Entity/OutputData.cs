namespace AlinaLib.Domain.Entity
{
    public class OutputData
    {
        public List<Record> Records { get; set; }

        #region .ctors

        public OutputData(IEnumerable<Record> records)
        {
            Records = new List<Record>(records);
        }
        #endregion
    }
}
