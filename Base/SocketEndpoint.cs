using System.Runtime.CompilerServices;

namespace PI_AQP
{
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false)]
    public class SocketEndpoint : Attribute
    {
        public Type Classe { get; private set; } = default!;
        public void SetClass(Type type)
        {
            Classe = type;
        }

        public virtual string Action { get; set; }

        public SocketEndpoint([CallerMemberName] string methodName = null)
        {
            Console.WriteLine($"Meu metodo: {methodName}");
        }
    }
}
