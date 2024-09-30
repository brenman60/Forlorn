using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Dialogue/Node List", fileName = "Node List")]
public class DialogueNodes : MonoBehaviour
{
    public List<DialogueNode> nodes = new List<DialogueNode>();
}
