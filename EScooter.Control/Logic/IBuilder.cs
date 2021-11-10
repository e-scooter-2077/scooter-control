namespace EScooter.Control.Logic
{
    public interface IBuilder<T>
    {
        public bool CanBuild();

        public T BuildWithDefaults();

        public T Build();
    }
}
