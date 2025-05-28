public interface IPooleable
{
    void GetObjectFromPool();
    void ReturnObjectToPool();
    void ResetToDefault();
    void Disable();
}
