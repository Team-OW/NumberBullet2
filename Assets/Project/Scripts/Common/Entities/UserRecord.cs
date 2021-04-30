using System;
using Treevel.Common.Managers;
using Treevel.Common.Patterns.Singleton;
using Treevel.Common.Utils;
using Treevel.Modules.MenuSelectScene.Settings;
using UniRx;
using UnityEngine;

namespace Treevel.Common.Entities
{
    public class UserRecord : SingletonObjectBase<UserRecord>
    {
        /// <summary>
        /// 起動日数
        /// </summary>
        private ReactiveProperty<int> _startupDays;

        public IObservable<int> StartupDaysObservable => _startupDays;

        /// <summary>
        /// 最終起動日に応じて，起動日数を更新する
        /// </summary>
        public void UpdateStartupDays()
        {
            var lastStartupDate = LastStartupDate;

            if (lastStartupDate is DateTime date) {
                if (date < DateTime.Today) {
                    // 起動日数を加算する
                    _startupDays.Value++;
                }
            }

            ScheduleManager.AddEvent(this, "UpdateStartupDays", DateTime.Today.AddDays(1));
        }

        /// <summary>
        /// 最終起動日
        /// </summary>
        private DateTime? _lastStartupDate;

        public DateTime? LastStartupDate {
            get => _lastStartupDate;
            set {
                _lastStartupDate = value;

                if (_lastStartupDate is DateTime date) {
                    PlayerPrefsUtility.SetDateTime(Constants.PlayerPrefsKeys.LAST_STARTUP_DATE, date);
                }
            }
        }

        private void Awake()
        {
            _startupDays = new ReactiveProperty<int>(PlayerPrefs.GetInt(Constants.PlayerPrefsKeys.STARTUP_DAYS, Default.STARTUP_DAYS));
            _lastStartupDate = PlayerPrefsUtility.GetDateTime(Constants.PlayerPrefsKeys.LAST_STARTUP_DATE);

            _startupDays
                .Subscribe(startupDays => PlayerPrefs.SetInt(Constants.PlayerPrefsKeys.STARTUP_DAYS, startupDays))
                .AddTo(this);

            ResetController.DataReset.Subscribe(_ => {
                // 最終起動日だけはリセットしない
                PlayerPrefs.DeleteKey(Constants.PlayerPrefsKeys.FAILURE_REASONS_COUNT);
                PlayerPrefs.DeleteKey(Constants.PlayerPrefsKeys.STARTUP_DAYS);

                _startupDays.Value = Default.STARTUP_DAYS;
            }).AddTo(this);
        }
    }
}