namespace Sp
{
    class Village
    {
        public string Name { get; set; }
        public string Number { get; set; }
        public string URL { get; set; }
        public string CategoryType { get; set; }
        public Town Town { get; set; }
        public string GetFullName() {
            return Town.GetFullName() + Name;
        }


    }

    
}
