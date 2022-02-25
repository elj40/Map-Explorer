using UnityEngine;

public class Setup : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        Application.targetFrameRate = 60;
    }

    public static void printArray(Vector3[] array) {
    string s = "[";
    foreach(Vector3 v in array) {
        s += "(x: " + v.x.ToString() + ", y: " + v.y.ToString() + ", z:" + v.z.ToString() + "),";
    }
    s += "]";
    Debug.Log(s);
    }

    public static void printArray(Vector2[] array) {
    string s = "[";
    foreach(Vector2 v in array) {
        s += "(x: " + v.x.ToString() + ", y: " + v.y.ToString() + "),";
    }
    s += "]";
    Debug.Log(s);
    }

    public static void printArray(int[] array) {
        string s = "[";
        foreach(float v in array) {
            s += v.ToString() + ", ";
        }
        s += "]";
        Debug.Log(s);
    }

    public static void print(Vector3 v, string description) {
        Debug.Log(description + "(x: " + v.x.ToString() + ", y: " + v.y.ToString() + ", z:" + v.z.ToString() + "),");
    }

    public static void print(Vector2 v, string description) {
        Debug.Log(description + "(x: " + v.x.ToString() + ", y: " + v.y.ToString() + "),");
    }
}

