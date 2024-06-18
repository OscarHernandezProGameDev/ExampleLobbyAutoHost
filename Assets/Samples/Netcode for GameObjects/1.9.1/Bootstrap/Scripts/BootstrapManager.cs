using Unity.Netcode;
using Unity.Netcode.Transports.UTP;
using UnityEngine;
using UnityEngine.UIElements;

namespace Unity.Netcode.Samples
{
    /// <summary>
    /// Class to display helper buttons and status labels on the GUI, as well as buttons to start host/client/server.
    /// Once a connection has been established to the server, the local player can be teleported to random positions via a GUI button.
    /// </summary>
    public class BootstrapManager : MonoBehaviour
    {
        private GUIStyle myStyleButton;
        private ApplicationData applicationData;

        private void Awake()
        {
#if !UNITY_WEBGL
            applicationData = new ApplicationData();
#endif
            myStyleButton = new GUIStyle();
            myStyleButton.fontSize = 40;
            myStyleButton.fontStyle = FontStyle.Bold;
            //myStyleButton.normal.textColor = Color.white;
        }
        void Start()
        {
#if !UNITY_WEBGL
            var transport = NetworkManager.Singleton.gameObject.GetComponent<UnityTransport>();
            var ip = ApplicationData.IP();
            var port = (ushort)ApplicationData.Port();

            Debug.Log($"IP: {ip}, Port: {port}");

            if (!string.IsNullOrEmpty(ip) && port > 0)
            {
                transport.ConnectionData.Address = ip;
                transport.ConnectionData.Port = port;
            }

            Debug.Log($"Connecting to {transport.ConnectionData.Address}:{transport.ConnectionData.Port}");

            if (SystemInfo.graphicsDeviceType == UnityEngine.Rendering.GraphicsDeviceType.Null)
                StartServer();
#endif
        }

        private void OnGUI()
        {
            GUILayout.BeginArea(new Rect(10, 10, 300, 300));

            var networkManager = NetworkManager.Singleton;
            if (!networkManager.IsClient && !networkManager.IsServer)
            {
                if (GUILayout.Button("Host", myStyleButton))
                {
                    networkManager.StartHost();
                }

                if (GUILayout.Button("Client", myStyleButton))
                {
                    networkManager.StartClient();
                }

                if (GUILayout.Button("Server", myStyleButton))
                {
                    StartServer();
                }
            }
            else
            {
                GUILayout.Label($"Mode: {(networkManager.IsHost ? "Host" : networkManager.IsServer ? "Server" : "Client")}");

                // "Random Teleport" button will only be shown to clients
                if (networkManager.IsClient)
                {
                    if (GUILayout.Button("Random Teleport", myStyleButton))
                    {
                        if (networkManager.LocalClient != null)
                        {
                            // Get `BootstrapPlayer` component from the player's `PlayerObject`
                            if (networkManager.LocalClient.PlayerObject.TryGetComponent(out BootstrapPlayer bootstrapPlayer))
                            {
                                // Invoke a `ServerRpc` from client-side to teleport player to a random position on the server-side
                                bootstrapPlayer.RandomTeleportServerRpc();
                            }
                        }
                    }
                }
            }

            GUILayout.EndArea();
        }

        private void StartServer()
        {
            NetworkManager.Singleton.StartServer();

            //FindAnyObjectByType<CubeMovement>()?.InitMovement();
        }
    }
}
