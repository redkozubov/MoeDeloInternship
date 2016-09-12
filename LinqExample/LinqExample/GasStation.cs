namespace LinqExample
{
    /// <summary>
    /// АЗС
    /// </summary>
    class GasStation
    {
        /// <summary>
        /// Компания - владелец АЗС
        /// </summary>
        public string Owner { get; set; }
        /// <summary>
        /// Улица, на которой расположена АЗС
        /// </summary>
        public string  Street { get; set; }
        /// <summary>
        /// Цена бензина, коп.
        /// </summary>
        public int GasPrice { get; set; }
        /// <summary>
        /// Марка бензина
        /// </summary>
        public int GasMark { get; set; }

        public GasStation(string own, string str, int prc, int mrk)
        {
            Owner = own;
            Street = str;
            GasPrice = prc;
            GasMark = mrk;
        }
    }
}