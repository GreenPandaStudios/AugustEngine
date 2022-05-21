namespace AugustEngine.Procedural
{
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;

    public class InfiniteWorldTrigger : InfiniteWorldTransform
    {
        [SerializeField] Vector2 xBounds = new Vector2(-500,500);
        [SerializeField] Vector2 yBounds = new Vector2(-500, 500);
        [SerializeField] Vector2 zBounds = new Vector2(-500, 500);
        private void Update()
        {
            if (transform.position.x < xBounds.x ||
                transform.position.y < yBounds.x ||
                transform.position.z < zBounds.x ||
                transform.position.x > xBounds.y ||
                transform.position.y > yBounds.y ||
                transform.position.z > zBounds.y )
            {                
                globalOffset.Value += transform.position;
                shiftByAmount?.Invoke(transform.position);
                TileGenerator.RequestUpdateTile?.Invoke();
            }
        }
    }
}