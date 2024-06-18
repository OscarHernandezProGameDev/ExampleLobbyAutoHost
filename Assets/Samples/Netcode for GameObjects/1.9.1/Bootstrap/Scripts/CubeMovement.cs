using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Unity.Netcode.Samples
{
    public class CubeMovement : NetworkBehaviour
    {
        public float speed;
        public float xMin;
        public float xMax;
        [SerializeField] private float direction = 1;

        public override void OnNetworkSpawn()
        {
            if (IsServer)
            {
                InitMovement();
            }
        }

        public override void OnNetworkDespawn()
        {
            if (IsServer)
            {
                StopCoroutine(MovementCoroutine());
            }
        }

        public void InitMovement()
        {
            StartCoroutine(MovementCoroutine());
        }

        IEnumerator MovementCoroutine()
        {
            while (true)
            {
                var x = transform.position.x + direction * speed * Time.deltaTime;

                if (direction > 0 && x > xMax)
                {
                    direction = -1;
                }
                else if (direction < 0 && x < xMin)
                {
                    direction = 1;
                }

                transform.position = new Vector3(x, transform.position.y, transform.position.z);

                yield return null;
            }
        }
    }
}
