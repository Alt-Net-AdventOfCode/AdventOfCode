namespace AoC
{
    public interface ISolver
    {
        void SetupRun(Engine engine);
        object GetAnswer1(string data);
        object GetAnswer2(string data);
    }
}