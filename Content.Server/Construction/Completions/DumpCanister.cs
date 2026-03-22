// OH14-Changes. Требуется для новых крафтов, за онову взят DestroyEntity.cs

using Content.Shared.Construction;
using JetBrains.Annotations;
using Content.Server.Destructible;
using Content.Server.Atmos.Piping.Unary.EntitySystems;

namespace Content.Server.Construction.Completions
{
    [UsedImplicitly]
    [DataDefinition]
    public sealed partial class DumpCanister : IGraphAction
    {
        public void PerformAction(EntityUid uid, EntityUid? userUid, IEntityManager entityManager)
        {
            entityManager.EntitySysManager.GetEntitySystem<GasCanisterSystem>().PurgeContents(uid);
        }
    }
}
