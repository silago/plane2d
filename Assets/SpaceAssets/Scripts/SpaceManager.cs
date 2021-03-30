#region
using UnityEngine;
#endregion
public enum ScrollDirection { LeftToRight, RightToLeft, DownToUp, UpToDown }

public class SpaceManager : MonoBehaviour
{

    public static SpaceManager instance;
    //Set the direction that the screen or the camera is moving
    public ScrollDirection scrollDirection = ScrollDirection.LeftToRight;
    private ScrollDirection direction;

    private void Start()
    {
        direction = scrollDirection;
        instance = this;
    }

    private void Update()
    {
        //Prevent that the variable could be changed in execution mode (removing this could cause bugs)
        if (direction != scrollDirection) scrollDirection = direction;
    }
}
