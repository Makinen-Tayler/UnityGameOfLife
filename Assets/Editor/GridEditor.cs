
using UnityEngine;
using UnityEditor;

[CanEditMultipleObjects]
[CustomEditor(typeof(GridManager))]
public class GridEditor : UnityEditor.Editor
{
    public override void OnInspectorGUI() {
        base.OnInspectorGUI();

        GridManager gm = (GridManager)target;

        GUILayout.BeginHorizontal();

            if(GUILayout.Button("Show Grid")) {
                    gm.ShowGrid();
            }

            if(GUILayout.Button("Clear Grid")) {
                gm.ClearGrid();
            }

        GUILayout.EndHorizontal();




        //gm.camSize = EditorGUILayout.Slider("Cam Zoom:", gm.camSize, -100f, 100f);
        //gm.cam.transform.position = new Vector3((float)gm.width / 2 - 0.5f, (float)gm.height / 2 - 0.5f, gm.camZoom);


        //gm.width = EditorGUILayout.Slider("Width:", gm.width, 1f, 200f);
        //gm.height = EditorGUILayout.Slider("Height:", gm.height, 1f, 200f);

        //gm.cellSize = EditorGUILayout.Slider("Cell Size:", gm.cellSize, 1f, .5f);
        

    }
}


