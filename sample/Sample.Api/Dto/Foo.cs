namespace Sample.Api.Dto
{
    public class Foo
    {
        public int Id { get; set; }
        public string Name { get; set; }

        public override string ToString()
        {
            return $"{nameof(Id)}: {Id}, {nameof(Name)}: {Name}";
        }
    }
}