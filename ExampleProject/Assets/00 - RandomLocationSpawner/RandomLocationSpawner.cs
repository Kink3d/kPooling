using UnityEngine;
using kTools.Pooling;

public class RandomLocationSpawner : MonoBehaviour
{
#region Fields
    float m_CurrentTime;
#endregion

#region Properties
    [Tooltip("Source object to use for instances.")]
    public GameObject source;

    [Tooltip("Locations to spawn instances.")]
    public Transform[] locations;

    [Tooltip("Amount of instances to create.")]
    public int instanceCount = 1;

    [Tooltip("Spawn speed in spawns per second.")]
    public float speed = 1.0f;
#endregion

#region Initialization
    void Start()
    {
        // Create a Pool of the object "source"
        // Since we only need one Pool the key can also be object "source"
        PoolingSystem.CreatePool(source, source, instanceCount);
    }
#endregion

#region Update
    void Update()
    {
        // Simple timer
        // If currentTime < 1 add to the timer and dont spawn
        if(m_CurrentTime < 1.0f)
        {
            // Multiply deltaTime by speed and increment
            m_CurrentTime += (Time.deltaTime * speed);
            return;
        }
        else
        {
            // Get an instance from the Pool
            GameObject instance;
            if(PoolingSystem.TryGetInstance(source, out instance))
            {
                // Position at a random index from locations array
                int randomIndex = Random.Range(0, locations.Length);
                instance.transform.position = locations[randomIndex].position;
            }

            // Reset timer
            m_CurrentTime = 0.0f;
        }
    }
#endregion

#region UI
    // Called by "Instances" InputField in Game UI
    public void SetInstances(string stringValue)
    {
        // Parse input string as integer
        int value;
        if(!int.TryParse(stringValue, out value))
            return;

        // Avoid redundant work
        if(value == instanceCount)
            return;

        // Set value
        instanceCount = value;

        // Recreate Pool
        PoolingSystem.DestroyPool<GameObject>(source);
        PoolingSystem.CreatePool(source, source, instanceCount);
    }

    // Called by "Speed" InputField in Game UI
    public void SetSpeed(string stringValue)
    {
        // Parse input string as integer
        float value;
        if(!float.TryParse(stringValue, out value))
            return;
        
        // Avoid redundant work
        if(value == speed)
            return;
        
        // Set value
        speed = value;
    }
#endregion
}
