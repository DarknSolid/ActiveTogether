
namespace EntityLib.Entities.AbstractClasses
{
    public interface DateAndIntegerId : IntegerId
    {
        public DateTime DateTime { get; set; }
    }
}
