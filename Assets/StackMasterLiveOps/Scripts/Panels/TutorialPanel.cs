using UnityEngine;

public class TutorialPanel : MonoBehaviour
{
    [SerializeField] private GameObject _stepOneObj;
    [SerializeField] private GameObject _stepTwoObj;
    [SerializeField] private GameObject _stepThreeObj;
    [SerializeField] private GameObject _stepFourObj;


    public void Show()
    {
        gameObject.SetActive(true);
    }

    public void StepOne()
    {
        _stepOneObj.gameObject.SetActive(true);
        _stepTwoObj.gameObject.SetActive(false);
        _stepThreeObj.gameObject.SetActive(false);
        _stepFourObj.gameObject.SetActive(false);
    }
    public void StepTwo()
    {
        _stepOneObj.gameObject.SetActive(false);
        _stepTwoObj.gameObject.SetActive(true);
        _stepThreeObj.gameObject.SetActive(false);
        _stepFourObj.gameObject.SetActive(false);
    }
    public void StepThree()
    {
        _stepOneObj.gameObject.SetActive(false);
        _stepTwoObj.gameObject.SetActive(false);
        _stepThreeObj.gameObject.SetActive(true);
        _stepFourObj.gameObject.SetActive(false);
    }
    public void StepFour()
    {
        _stepOneObj.gameObject.SetActive(false);
        _stepTwoObj.gameObject.SetActive(false);
        _stepThreeObj.gameObject.SetActive(false);
        _stepFourObj.gameObject.SetActive(true);
    }
    public void JokerClicked()
    {
        _stepThreeObj.gameObject.SetActive(false);
    }
    public void Hide()
    {
        gameObject.SetActive(false);
    }
}
