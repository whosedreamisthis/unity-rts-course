using UnityEngine;

namespace GameDevTV.UI
{
    public interface IUIElement<T>
    {
        void EnableFor(T item);
        void Disable();
        // Start is called once before the first execution of Update after the MonoBehaviour is created
    }

    public interface IUIElement<T1, T2>
    {
        void EnableFor(T1 item, T2 callback);
        void Disable();
        // Start is called once before the first execution of Update after the MonoBehaviour is created
    }
}
