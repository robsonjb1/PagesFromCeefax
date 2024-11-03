    public class List
    {
        public string RateDayChangeValue { get; set; }
        public string FromISO { get; set; }
        public string ToISO { get; set; }
        public string RateCurrent { get; set; }
        public string RateDayChangePercent { get; set; }
    }

    public class HLCurrencies
    {
        public List<List> list { get; set; }
    }

