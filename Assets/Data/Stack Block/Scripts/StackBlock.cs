using System;
using Data.Game_Manager;
using Data.Materials.Scripts;
using UnityEngine;

namespace Data.Stack_Block.Scripts
{
    public class StackBlock : MonoBehaviour
    {
        [SerializeField] private float moveSpeed = 1f;
        [SerializeField] private float maxMoveSpeed = 2.3f;
        [SerializeField] private bool canMove;

        [SerializeField] private Spawner spawner;
        [SerializeField] private Transform placedBlocks;
        [SerializeField] private Transform previousBlockTransform;

        private Transform _startBlock;
        private MeshRenderer _meshRenderer;
        
        private Rigidbody _rigidbody;

        public static readonly Vector3 DefaultBlockScale = new Vector3(1, 0.2f, 1);

        public static Action OnBlockPlaced;
        public static Action OnBlockSliced;
        public static Action OnBlockPlacedPerfectly;

        private Directions _direction = Directions.Left;
        
        private int _moveDirection = 1;
        
        private const float MoveBackDistance = 1.5f;


        private void Awake()
        {
            _rigidbody = GetComponent<Rigidbody>();
            _meshRenderer = GetComponent<MeshRenderer>();

            _meshRenderer.material = ColorGenerator.GetBasedMaterialWithRandomColor(_meshRenderer.sharedMaterial);

            _startBlock = previousBlockTransform;
        }

        private void OnEnable()
        {
            GameManager.OnStart += () => canMove = true;
            GameManager.OnClicked += TryStopAndSplit;
            GameManager.OnRestart += Reset;
        }
        
        private void OnDisable()
        {
            GameManager.OnStart -= () => canMove = true;
            GameManager.OnClicked -= TryStopAndSplit;
            GameManager.OnRestart -= Reset;
        }

        private void Move()
        {
            ChangeMoveDirection();

            if (_direction == Directions.Left)
            {
                transform.position += transform.forward * (Time.deltaTime * moveSpeed * _moveDirection);
                return;
            }
            transform.position += transform.right * (Time.deltaTime * moveSpeed * _moveDirection);
        }
        
        private void Update()
        {
            if (canMove)
            {
                Move();
            }
        }

        private void ChangeMoveDirection()
        {
            if (GetHangover() > MoveBackDistance)
            {
               _moveDirection = -1;
            }

            if (GetHangover() < -MoveBackDistance)
            {
                _moveDirection = 1;
            }
        }
        
        private void TryStopAndSplit()
        {
            if (canMove)
            {
                float hangover = GetHangover();
                canMove = false;

                float max = _direction == Directions.Left
                    ? previousBlockTransform.localScale.z
                    : previousBlockTransform.localScale.x;

                if (Mathf.Abs(hangover) < 0.05f)
                {
                    hangover = 0;
                    if (moveSpeed < maxMoveSpeed)
                    {
                        moveSpeed += 0.1f;
                    }
                    OnBlockPlacedPerfectly.Invoke();
                }
                else
                {
                    if (moveSpeed > 1f)
                    {
                        moveSpeed -= 0.1f;  
                    }
                }

                if (Mathf.Abs(hangover) >= max)
                {
                    StopStacking();
                    return;
                }
                
                SplitBlock(hangover);
            }
        }

        private void SplitBlock(float hangover)
        {
            float direction = hangover > 0 ? 1f : -1f;
            
            if (_direction == Directions.Left)
            {
                SplitBlockByLeft(hangover , direction); 
            }
            else
            {
                SplitBlockByRight(hangover , direction); 
            }
            
            SpawnPreviousCube();
            SetUpStackBlock();
        }
        
        private void SplitBlockByLeft(float hangover , float direction)
        {
            float newZSize = previousBlockTransform.transform.localScale.z - Mathf.Abs(hangover);
            float fallingBlockSize = transform.localScale.z - newZSize;

            float newZPosition = previousBlockTransform.transform.position.z + (hangover / 2);
            transform.localScale = new Vector3(transform.localScale.x, transform.localScale.y, newZSize);
            transform.position = new Vector3(transform.position.x, transform.position.y, newZPosition);

            float cubeEdge = transform.position.z + (newZSize / 2f * direction);
            float fallingBlockZPosition = cubeEdge + fallingBlockSize / 2f * direction;

            if (hangover != 0)
            {
                SpawnDropCube(fallingBlockZPosition, fallingBlockSize);
            }
        }
        
        private void SplitBlockByRight(float hangover , float direction)
        {
            float newXSize = previousBlockTransform.transform.localScale.x - Mathf.Abs(hangover);
            float fallingBlockSize = transform.localScale.x - newXSize;

            float newXPosition = previousBlockTransform.transform.position.x + (hangover / 2);
            transform.localScale = new Vector3(newXSize, transform.localScale.y, transform.localScale.z);
            transform.position = new Vector3(newXPosition, transform.position.y, transform.position.z);

            float cubeEdge = transform.position.x + (newXSize / 2f * direction);
            float fallingBlockXPosition = cubeEdge + fallingBlockSize / 2f * direction;
            
            if (hangover != 0)
            {
                SpawnDropCube(fallingBlockXPosition, fallingBlockSize);
            }
        }


        private float GetHangover()
        {
            if (_direction == Directions.Left)
            {
                return transform.position.z - previousBlockTransform.transform.position.z;
            }
            return transform.position.x - previousBlockTransform.transform.position.x;
        }
        

        private void SpawnPreviousCube()
        {
            var cube = CreateCube();

            cube.transform.position = transform.position;
            cube.transform.localScale = transform.localScale;

            previousBlockTransform = cube.transform;
        }
        
        private GameObject CreateCube()
        {
            var cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
            cube.transform.SetParent(placedBlocks.transform);

            cube.GetComponent<MeshRenderer>().material = _meshRenderer.material;

            return cube;
        }
        
        private void SpawnDropCube(float fallingBlockPosition, float fallingBlockSize)
        { 
            OnBlockSliced.Invoke();
            
            var cube = CreateCube();

            if (_direction == Directions.Left)
            {
                cube.transform.localScale = new Vector3(transform.localScale.x, transform.localScale.y, fallingBlockSize);
                cube.transform.position = new Vector3(transform.position.x, transform.position.y, fallingBlockPosition);
            }
            else
            {
                cube.transform.localScale = new Vector3(fallingBlockSize, transform.localScale.y, transform.localScale.z);
                cube.transform.position = new Vector3(fallingBlockPosition, transform.position.y, transform.position.z); 
            }

            cube.AddComponent<Rigidbody>();
            Destroy(cube.gameObject, 2f);
        }

        private void SetUpStackBlock()
        {
            ChangeDirection();
            var spawnPosition = spawner.GetSpawnPosition(_direction, transform.position, previousBlockTransform.position);

            transform.position = new Vector3(spawnPosition.x,
                transform.position.y + previousBlockTransform.localScale.y, spawnPosition.z);

            canMove = true;
            
            _meshRenderer.material = ColorGenerator.GetBasedMaterialWithRandomColor(_meshRenderer.sharedMaterial);
            
            OnBlockPlaced.Invoke();
        }

        private void ChangeDirection()
        {
            if (_direction == Directions.Left)
            {
                _direction = Directions.Right;
                return; 
            }
            _direction = Directions.Left;
        }


        private void StopStacking()
        {
            _rigidbody.isKinematic = false;

            canMove = false;
            GameManager.OnFinished.Invoke();
        }

        private void Reset()
        {
            _direction = Directions.Left;
            transform.localScale = DefaultBlockScale;
            _rigidbody.isKinematic = true;

            moveSpeed = 1f;
            
            transform.position = spawner.GetDefaultPosition();
            previousBlockTransform = _startBlock;
            
            _meshRenderer.material = ColorGenerator.GetBasedMaterialWithRandomColor(_meshRenderer.sharedMaterial);
        }
    }
}
