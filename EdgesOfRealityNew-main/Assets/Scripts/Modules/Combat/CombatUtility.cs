using Metroidvania.Entities;
using UnityEngine;

namespace Metroidvania.Combat
{
    public static class CombatUtility
    {
        public static void CastEntityBoxHit(Vector2 position, Vector2 size, Collider2D[] hits, LayerMask targetLayer, float damage, Vector2 knockbackForce)
        {
            var contactFilter = new ContactFilter2D();
            contactFilter.SetLayerMask(targetLayer);
            int hitsCount = Physics2D.OverlapBox(position, size, 0, contactFilter, hits);

            if (hitsCount == 0)
                return;

            EntityHitData hitData = new EntityHitData(damage, knockbackForce);

            for (int i = 0; i < hitsCount; i++)
            {
                Collider2D hit = hits[i];
                if (hit.TryGetComponent<IEntityHittable>(out IEntityHittable hittableTarget))
                    hittableTarget.OnTakeHit(hitData);
            }
        }

        public static Vector2 FromFacingDirection(Vector2 knockbackForce, float facingDirection)
        {
            return new Vector2(knockbackForce.x * Mathf.Sign(facingDirection), knockbackForce.y);
        }
    }
}
