using System;
using System.Collections.Generic;
using UnityEngine;
using Weapon.Data;
using Random = UnityEngine.Random;

namespace Weapon
{
    public class EnhancementManager : MonoBehaviour
    {
        //todo migrate to data manager
        private const string ENHANCEMENT_CSV_FILE = "enhancement_dataset";
        private Camera _camera;

        [SerializeField] private Canvas cards;
        [SerializeField] private EnhanceCardUI cardPrefab;

        private List<EnhancementDataEntry> _dataEntries;
        private int _headcount = 5;

        private void Awake()
        {
            //todo get headcount from gameManager??
            LoadDataSet();
            _camera = Camera.main;
        }

        private void Start()
        {
            CreateCards();
        }

        private void LoadDataSet()
        {
            //Inversion of control -> dataManager
            _dataEntries = CsvReader.ReadCsvFromResources<EnhancementDataEntry>(ENHANCEMENT_CSV_FILE, 1);
        }

        public void CreateCards()
        {
            int repeat = 0;
            int cnt = 0;
            bool[] selected = new bool[_dataEntries.Count];
            while (repeat < 10000 && cnt < _headcount + 2)
            {
                repeat++;
                int idx = Random.Range(0, _dataEntries.Count);
                if (selected[idx])
                {
                    continue;
                }

                cnt++;
                selected[idx] = true;
            }

            cnt = 0;
            float width = 0f;
            for (int i = 0; i < _dataEntries.Count; i++)
            {
                if (selected[i])
                {
                    EnhanceCardUI card = Instantiate(cardPrefab, cards.transform, false);
                    card.SetEnhancementData(_dataEntries[i].ToEnhancementData());
                    if (width == 0)
                    {
                        width = card.GetComponent<RectTransform>().rect.width;
                    }

                    ArrangeCard(card, width, cnt++);
                }
            }
        }

        private void ArrangeCard(EnhanceCardUI card, float cardWidth, int index)
        {
            //todo animate card arrangement
            Vector3 spacing = new Vector3(cardWidth + 50f, 0f);
            Vector3 centerPosition = _camera.ViewportToScreenPoint(new Vector3(0.5f, 0.5f));
            Vector3 startPosition = centerPosition - (spacing) * ((_headcount + 2) / 2f);
            startPosition.x += spacing.x / 2f;
            card.transform.position = startPosition + (spacing * index);
        }

        public void SelectCard(int playerIndex)
        {
            //on click event callback 
        }
    }
}