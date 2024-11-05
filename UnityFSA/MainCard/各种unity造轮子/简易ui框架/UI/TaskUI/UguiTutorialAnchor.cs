using UnityEngine;

namespace XMeta.Foundation.UI.TaskUI
{

    public class UguiTutorialAnchor : MonoBehaviour
    {
        public string anchorName;
        public int sortingOrder = 0;

        public MonoBehaviour anchorNameInterface;

        public string AnchorName
        {
            get
            {
                if (anchorNameInterface is IAnchorName i)
                {
                    return i.AnchorName;
                }

                return anchorName;
            }
        }




        public void OnEnable()
        {
            AllUguiTutorialAnchor.Add(this);
        }

        public void OnDisable()
        {
            AllUguiTutorialAnchor.Remove(this);
        }
    }
}