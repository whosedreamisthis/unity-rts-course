using GameDevTV.Units;
using UnityEngine;

namespace GameDevTV.Commands
{
    [CreateAssetMenu(fileName = "Stop", menuName = "AI/Commands/Stop", order = 100)]
    public class StopCommand : ActionBase
    {
        public override bool CanHandle(CommandContext context)
        {
            return context.Commandable is AbstractUnit;
        }

        public override void Handle(CommandContext context)
        {
            AbstractUnit unit = (AbstractUnit)context.Commandable;

            unit.Stop();
        }
    }
}
