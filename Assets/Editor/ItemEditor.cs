using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

public class ItemEditor : EditorWindow
{
    [SerializeField] private int m_SelectedIndex = -1;
    private VisualElement m_RightPane;

    [MenuItem("Tools/Item Editor")]
    public static void ShowMyEditor()
    {
        // This method is called when the user selects the menu item in the Editor
        EditorWindow wnd = GetWindow<ItemEditor>();
        wnd.titleContent = new GUIContent("Item Editor");

        // Limit size of the window
        wnd.minSize = new Vector2(450, 200);
        wnd.maxSize = new Vector2(1920, 720);
    }

    protected void OnEnable()
    {
        var data = EditorPrefs.GetString("ItemData", JsonUtility.ToJson(this, false));
        JsonUtility.FromJsonOverwrite(data, this);
    }
    protected void OnDisable()
    {
        var data = JsonUtility.ToJson(this, false);
        EditorPrefs.SetString("ItemData", data);
    }

    public void CreateGUI()
    {
        // Get a list of all sprites in the project
        var allObjectGuids = AssetDatabase.FindAssets("t:Sprite", new string[] { "Assets/Items" });
        var allObjects = new List<Sprite>();
        foreach (var guid in allObjectGuids)
        {
            allObjects.Add(AssetDatabase.LoadAssetAtPath<Sprite>(AssetDatabase.GUIDToAssetPath(guid)));
        }

        // Create a two-pane view with the left pane being fixed with
        var splitView = new TwoPaneSplitView(0, 250, TwoPaneSplitViewOrientation.Horizontal);

        // Add the panel to the visual tree by adding it as a child to the root element
        rootVisualElement.Add(splitView);

        // A TwoPaneSplitView always needs exactly two child elements
        var leftPane = new ListView();
        splitView.Add(leftPane);
        m_RightPane = new ScrollView(ScrollViewMode.VerticalAndHorizontal);
        splitView.Add(m_RightPane);

        // Initialize the list view with all sprites' names
        leftPane.makeItem = () => new Label();
        leftPane.bindItem = (item, index) => { (item as Label).text = allObjects[index].name; };
        leftPane.itemsSource = allObjects;

        // React to the user's selection
        leftPane.onSelectionChange += OnSpriteSelectionChange;

        // Restore the selection index from before the hot reload
        leftPane.selectedIndex = m_SelectedIndex;

        // Store the selection index when the selection changes
        leftPane.onSelectionChange += (items) => { m_SelectedIndex = leftPane.selectedIndex; };
    }

    private void OnSpriteSelectionChange(IEnumerable<object> selectedItems)
    {
        // Clear all previous content from the pane
        m_RightPane.Clear();

        // Get the selected sprite
        var selectedSprite = selectedItems.First() as Sprite;
        if (selectedSprite == null)
            return;

        var name = new TextField("Name");
        var hp = new TextField("Hp");
        var atk = new TextField("Atk");
        var money = new TextField("Money");
        var coolTime = new TextField("CoolTime");

        // Add a new Image control and display the sprite
        var spriteImage = new Image();
        spriteImage.scaleMode = ScaleMode.ScaleToFit;
        spriteImage.sprite = selectedSprite;

        // Add the Image control to the right-hand pane
        m_RightPane.Add(spriteImage);
        m_RightPane.Add(name);
        m_RightPane.Add(hp);
        m_RightPane.Add(atk);
        m_RightPane.Add(money);
        m_RightPane.Add(coolTime);
    }
}