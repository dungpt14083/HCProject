using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using TMPro;

public class Paging : MonoBehaviour
{
    [Header("Paging")]
    [SerializeField] private TMP_Text txtPreviousPage;
    [SerializeField] private TMP_Text txtCurrentPage;
    [SerializeField] private TMP_Text txtNextPage;
    [SerializeField] private TMP_Text txtNextPage_2;
    [SerializeField] private TMP_Text txtLastPage;

    [Header("Callback")]
    [SerializeField] private UnityEvent<int> OnPagingClicked;

    private int currentPage;
    public int CurrentPaging
    {
        get { return currentPage; }
    }
    private int lastPage;
    public void SetUpPaging(int inputLastPage, int inputCurrentPage = 0)
    {
        lastPage = inputLastPage;
        currentPage = inputCurrentPage;
        txtLastPage.text = lastPage.ToString();

        UpdatePagingNumber();
    }

    private void UpdatePagingNumber()
    {
        txtPreviousPage.transform.parent.gameObject.SetActive(true);
        txtCurrentPage.transform.parent.gameObject.SetActive(true);
        txtNextPage.transform.parent.gameObject.SetActive(true);
        txtNextPage_2.transform.parent.gameObject.SetActive(true);

        if (currentPage - 1 < 0)
        {
            txtPreviousPage.transform.parent.gameObject.SetActive(false);
        }
        if (currentPage == lastPage)
        {
            txtCurrentPage.transform.parent.gameObject.SetActive(false);
        }
        if (currentPage + 1 >= lastPage)
        {
            txtNextPage.transform.parent.gameObject.SetActive(false);
        }
        if(currentPage + 2 >= lastPage)
        {
            txtNextPage_2.transform.parent.gameObject.SetActive(false);
        }

        txtPreviousPage.text = (currentPage - 1).ToString();
        txtCurrentPage.text = currentPage.ToString();
        txtNextPage.text = (currentPage + 1).ToString();
        txtNextPage_2.text = (currentPage + 2).ToString();
    }

    public void btnPreviousPage_onClicked()
    {
        if(currentPage > 0)
        {
            currentPage--;
            UpdatePagingNumber();
            OnPagingClicked?.Invoke(currentPage);
        }
    }

    public void btnNextPage_onClicked()
    {
        if(currentPage < lastPage - 1)
        {
            currentPage++;
            UpdatePagingNumber();
            OnPagingClicked?.Invoke(currentPage);
        }
    }

    public void btnNextPage2_onClicked()
    {
        if (currentPage < lastPage - 2)
        {
            currentPage += 2;
            UpdatePagingNumber();
            OnPagingClicked?.Invoke(currentPage);
        }
    }

    public void btnLastPage_onClicked()
    {
        currentPage = lastPage;
        UpdatePagingNumber();
        OnPagingClicked?.Invoke(currentPage);
    }
}
