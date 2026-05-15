using RuStore.Review;
using UnityEngine;
using UnityEngine.UI;

namespace UI.GameplayMenu.Views
{
    public class ReviewScreen : MonoBehaviour
    {
        private const string CATALOG_APP = "https://www.rustore.ru/catalog/app/";

        [SerializeField] private Button _openReviewURLButton;

        private void OnEnable()
        {
            LaunchReviewFlow();
        }

        private void Start()
        {
            _openReviewURLButton.onClick.AddListener(OpenReviewInRuStore);
        }

        private void OnDestroy()
        {
            _openReviewURLButton.onClick.RemoveListener(OpenReviewInRuStore);
        }

        private void LaunchReviewFlow()
        {
            RuStoreReviewManager.Instance.LaunchReviewFlow(
                onFailure: (error) => {},
                onSuccess: () => {});
        }

        private void OpenReviewInRuStore()
        {
            var url = CATALOG_APP + Application.identifier;
            Application.OpenURL(url);
        }
    }
}