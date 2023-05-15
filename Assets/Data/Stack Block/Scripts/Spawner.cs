using System;
using Data.Materials.Scripts;
using UnityEngine;

namespace Data.Stack_Block.Scripts
{
    public class Spawner : MonoBehaviour
    {
        [SerializeField] private Transform leftSpawnPosition;
        [SerializeField] private Transform rightSpawnPosition;

        public Vector3 GetSpawnPosition(Directions direction , Vector3 blockPosition , Vector3 previousBlockPosition)
        {
            float x = direction == Directions.Right ? rightSpawnPosition.transform.position.x : previousBlockPosition.x;
            float z = direction == Directions.Left ? rightSpawnPosition.transform.position.z : previousBlockPosition.z;
            
            if (direction == Directions.Left)
            {
                 x = previousBlockPosition.x;
                 z = leftSpawnPosition.transform.position.z;
            }
            return new Vector3(x, leftSpawnPosition.position.y , z);
        }
        
        public Vector3 GetDefaultPosition()
        {
            return leftSpawnPosition.position;
        }
        
        private void OnDrawGizmos()
        {
            Gizmos.color = Color.green;
            Gizmos.DrawWireCube(leftSpawnPosition.position , StackBlock.DefaultBlockScale);
            Gizmos.DrawWireCube(rightSpawnPosition.position , StackBlock.DefaultBlockScale);
        }
    }
}
