using UnityEngine;
     
public class BuildSidebar : MonoBehaviour
{
    public TowerSpawnButton towerSpawnButton;
    public FauxGravityBody[] fauxGravityBodys;
    protected virtual void Start()
    {

        for (int i = 0; i <3; i++)
        {
            TowerSpawnButton button = Instantiate(towerSpawnButton, transform);
            button.InitializeButton(fauxGravityBodys[i]);
            button.buttonTapped += OnButtonTapped;
            button.draggedOff += OnButtonDraggedOff;

        }

    }

    void OnButtonTapped(FauxGravityBody fauxGravityBody)
    {
        var gameUI = GameUIManager.instance;
        if (gameUI.isBuilding)
        {
            gameUI.CancelGhostPlacement();
        }
        gameUI.SetToBuildMode(fauxGravityBody);
    }

    void OnButtonDraggedOff(FauxGravityBody fauxGravityBody)
    {
        if (!GameUIManager.instance.isBuilding)
        {
            GameUIManager.instance.SetToDragMode(fauxGravityBody);
        }
    }
}
