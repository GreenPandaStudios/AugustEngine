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
        [SerializeField] bool useY = false;
        [SerializeField] uint gridSize = 500;
        private void Update()
        {
            if (transform.position.x < xBounds.x ||
                transform.position.y < yBounds.x ||
                transform.position.z < zBounds.x ||
                transform.position.x > xBounds.y ||
                transform.position.y > yBounds.y ||
                transform.position.z > zBounds.y )
            {

                var _griddedVec = new Vector3(
                    Mathf.RoundToInt(transform.position.x / gridSize) * gridSize,
                    useY ? Mathf.RoundToInt(transform.position.y / gridSize) * gridSize : 0,
                    Mathf.RoundToInt(transform.position.z / gridSize) * gridSize
                    );

                globalOffset.Value += _griddedVec;
                shiftByAmount?.Invoke(_griddedVec);
                TileGenerator.RequestUpdateTile?.Invoke();
            }
        }
    }
}