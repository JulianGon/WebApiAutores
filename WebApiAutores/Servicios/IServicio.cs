namespace WebApiAutores.Servicios
{
    public interface IServicio
    {
        Guid ObtenerScoped();
        Guid ObtenerSingleton();
        Guid ObtenerTransient();
        void RealizarTarea();
    }
    public class ServicioA : IServicio
    {
        private readonly ILogger<ServicioA> logger;
        private readonly ServicioSingleton servicioSingleton;
        private readonly ServicioTransient servicioTransient;
        private readonly ServicioScoped servicioScoped;

        public ServicioA(ILogger<ServicioA> logger, ServicioSingleton servicioSingleton, ServicioTransient servicioTransient, ServicioScoped servicioScoped)
        {
            this.logger = logger;
            this.servicioSingleton = servicioSingleton;
            this.servicioTransient = servicioTransient;
            this.servicioScoped = servicioScoped;
        }

        public Guid ObtenerTransient() { return servicioTransient.guid; }
        public Guid ObtenerScoped() { return servicioScoped.guid; }
        public Guid ObtenerSingleton() { return servicioSingleton.guid; }

        public void RealizarTarea()
        {
            throw new NotImplementedException();
        }
    }

    public class ServicioB : IServicio
    {
        public Guid ObtenerScoped()
        {
            throw new NotImplementedException();
        }

        public Guid ObtenerSingleton()
        {
            throw new NotImplementedException();
        }

        public Guid ObtenerTransient()
        {
            throw new NotImplementedException();
        }

        public void RealizarTarea()
        {
            throw new NotImplementedException();
        }
    }

    public class ServicioTransient 
    {
        public Guid guid = Guid.NewGuid();
    }
    public class ServicioScoped
    {
        public Guid guid = Guid.NewGuid();
    }
    public class ServicioSingleton
    {
        public Guid guid = Guid.NewGuid();
    }


}
