using GameDevTV.Units;
using UnityEngine;

namespace GameDevTV.Commands
{
    public interface ICommand
    {
        bool CanHandle(AbstractCommandable commandable, RaycastHit hit);
        void Handle(AbstractCommandable commandable, RaycastHit hit);
    }
}
