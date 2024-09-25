namespace Telemetric.Shared;

public class Diagnostics
{
    public class Product
    {
        public class Activity
        {
            public const string Order = "product.order";
        }

        public class Tags
        {
            public const string Id = "product.id";
            public const string Price = "product.price";
        }
    }
}
