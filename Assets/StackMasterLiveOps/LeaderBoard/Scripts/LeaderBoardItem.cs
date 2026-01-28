using TMPro;
using UnityEngine;
using UnityEngine.UI;
public class LeaderBoardItem : MonoBehaviour
{

    [SerializeField] Image _avtar_Img;
    [SerializeField] TextMeshProUGUI _rank_Label;
    [SerializeField] TextMeshProUGUI _score_Lable;
    [SerializeField] TextMeshProUGUI _playerName_Label;

    public int Rank { get; private set; }
    public int Score { get; private set; }
    public string Name { get; private set; }
    public Sprite Sprite;

    public void SetItem(int rank, int score, string name, Sprite sprite = null)
    {
        if (sprite != null)
        {
            Sprite = sprite;
        }
        if (_avtar_Img != null)
        {
            _avtar_Img.sprite = sprite;
        }
        Rank = rank;
        Score = score;
        Name = name;

        if (_rank_Label != null)
        {
            _rank_Label.text = rank.ToString();
        }
        _score_Lable.text = score.ToString();
        _playerName_Label.text = name;
    }
    public void SetRank(int rank)
    {
        if (_rank_Label != null)
        {
            _rank_Label.text = rank.ToString();
        }
        Rank = rank;
    }
    public void SetScroe(int score)
    {
        _score_Lable.text = score.ToString();
    }
}
