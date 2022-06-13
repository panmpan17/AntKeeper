using System.Text;
using UnityEngine;
using TMPro;

public class CreditMenu : AbstractMenu
{
    [Header("Credits")]
    [SerializeField]
    private TextMeshProUGUI creditText;
    [SerializeField]
    [Multiline]
    private string format;
    [SerializeField]
    private Credit[] credits;


    protected override void Awake()
    {
        base.Awake();

        var builder = new StringBuilder();

        for (int i = 0; i < credits.Length; i++)
        {
            builder.Append(string.Format(format, credits[i].Name, credits[i].Occupation));

            if (i < credits.Length - 1)
            {
                builder.AppendLine();
                builder.AppendLine();
            }
        }

        creditText.text = builder.ToString();
    }


    [System.Serializable]
    public struct Credit
    {
        public string Name;
        [Multiline]
        public string Occupation;
    }
}
