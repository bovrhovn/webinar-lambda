using Lambada.Base;

namespace Lambada.Generators.Options
{
    public class GeneratorOptions : WebOptions
    {
        public string DefaultEmailTo { get; set; }
        public string DefaultEmailFrom { get; set; }
        public string DefaultSalt { get; set; }
    }
}