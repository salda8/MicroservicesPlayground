namespace Identity.MongoDb.Sample
{
    public class MongoDbSettings 
    {
        public string ConnectionString { get; set; }
        public string Database { get; set; }

        public override string ToString() {
            return $"{ConnectionString}/{Database}";

        }
    }
}