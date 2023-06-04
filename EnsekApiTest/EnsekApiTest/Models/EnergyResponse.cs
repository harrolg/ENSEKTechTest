namespace EnsekApiTest.Models
{
    public class EnergyResponse
    {
        public Electric electric { get; set; }
        public Gas gas { get; set; }
        public Nuclear nuclear { get; set; }
        public Oil oil { get; set; }
    }

    public class Electric
    {
        public int energy_id { get; set; }
        public float price_per_unit { get; set; }
        public int quantity_of_units { get; set; }
        public string unit_type { get; set; }
    }

    public class Gas
    {
        public int energy_id { get; set; }
        public float price_per_unit { get; set; }
        public int quantity_of_units { get; set; }
        public string unit_type { get; set; }
    }

    public class Nuclear
    {
        public int energy_id { get; set; }
        public float price_per_unit { get; set; }
        public int quantity_of_units { get; set; }
        public string unit_type { get; set; }
    }

    public class Oil
    {
        public int energy_id { get; set; }
        public float price_per_unit { get; set; }
        public int quantity_of_units { get; set; }
        public string unit_type { get; set; }
    } 
}
