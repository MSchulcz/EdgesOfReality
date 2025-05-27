using DG.Tweening;
using Metroidvania.UI;
using UnityEngine;
using UnityEngine.UI;

namespace Metroidvania
{
    public class CharacterStatusBar : Singleton<CharacterStatusBar>
    {
        [SerializeField] private SliderValueAnimator m_lifeSlider;
        [SerializeField] private SliderValueAnimator m_staminaSlider;

        private CharacterAttribute<float> _life;
        private CharacterAttribute<float> _stamina;

        public void ConnectLife(CharacterAttribute<float> life)
        {
            _life = life;

            life.OnValueChanged += LifeChanged;
            life.OnLevelChanged += LifeLevelChanged;
        }

        public void SetLife(float life)
        {
            m_lifeSlider.slider.DOKill();
            m_lifeSlider.slider.value = life;
        }

        private void LifeChanged(float value)
        {
            m_lifeSlider.Animate(value);
        }

        private void LifeLevelChanged(int level)
        {
            m_lifeSlider.slider.maxValue = _life.maxValue;
        }

        public void ConnectStamina(CharacterAttribute<float> stamina)
        {
            _stamina = stamina;

            stamina.OnValueChanged += StaminaChanged;
            stamina.OnLevelChanged += StaminaLevelChanged;
        }

        public void SetStamina(float stamina)
        {
            m_staminaSlider.slider.DOKill();
            m_staminaSlider.slider.value = stamina;
        }

        private void StaminaChanged(float value)
        {
            m_staminaSlider.Animate(value);
        }

        private void StaminaLevelChanged(int level)
        {
            m_staminaSlider.slider.maxValue = _stamina.maxValue;
        }
    }
}
