using System;
using System.Collections;
using UnityEngine;

namespace UIStrategy
{
    public interface IShowStrategy
    {
        IEnumerator Execute(Action onComplete);
    }

    public interface IHideStrategy
    {
        IEnumerator Execute();
    }

    // ShowStrategy

    public class ShowNoneStrategy : IShowStrategy
    {
        private readonly UIView view;

        public ShowNoneStrategy(UIView view)
        {
            this.view = view;
        }

        public IEnumerator Execute(Action onComplete)
        {
            onComplete?.Invoke();
            yield break;
        }
    }

    public class ShowFadeInStrategy : IShowStrategy
    {
        private readonly UIView view;

        public ShowFadeInStrategy(UIView view)
        {
            this.view = view;
        }

        public IEnumerator Execute(Action onComplete)
        {
            yield return view.FadeIn();
            onComplete?.Invoke();
        }
    }

    public class ShowSlideInStrategy : IShowStrategy
    {
        private readonly UIView view;
        private readonly SlideDirection direction;

        public ShowSlideInStrategy(UIView view, SlideDirection direction)
        {
            this.view = view;
            this.direction = direction;
        }

        public IEnumerator Execute(Action onComplete)
        {
            yield return view.SlideIn(direction);
            onComplete?.Invoke();
        }
    }

    public class ShowPopUpStrategy : IShowStrategy
    {
        private readonly UIView view;

        public ShowPopUpStrategy(UIView view)
        {
            this.view = view;
        }

        public IEnumerator Execute(Action onComplete)
        {
            yield return view.PopUp();
            onComplete?.Invoke();
        }
    }

    public class ShowSlideFadeInStrategy : IShowStrategy
    {
        private readonly UIView view;
        private readonly SlideDirection direction;
        private readonly CanvasGroup canvasGroup;

        public ShowSlideFadeInStrategy(UIView view, SlideDirection direction, CanvasGroup canvasGroup)
        {
            this.view = view;
            this.direction = direction;
            this.canvasGroup = canvasGroup;
        }

        public IEnumerator Execute(Action onComplete)
        {
            view.StartCoroutine(view.SlideIn(direction));
            view.StartCoroutine(view.FadeIn());
            yield return new WaitForSeconds(view.uiShowTime);
            onComplete?.Invoke();
        }
    }

    // HideStrategy

    public class HideNoneStrategy : IHideStrategy
    {
        private readonly UIView view;

        public HideNoneStrategy(UIView view)
        {
            this.view = view;
        }

        public IEnumerator Execute()
        {
            return view.HideNone();
        }
    }

    public class HideFadeOutStrategy : IHideStrategy
    {
        private readonly UIView view;

        public HideFadeOutStrategy(UIView view)
        {
            this.view = view;
        }

        public IEnumerator Execute()
        {
            return view.FadeOut();
        }
    }

    public class HideSlideOutStrategy : IHideStrategy
    {
        private readonly UIView view;
        private readonly SlideDirection direction;

        public HideSlideOutStrategy(UIView view, SlideDirection direction)
        {
            this.view = view;
            this.direction = direction;
        }

        public IEnumerator Execute()
        {
            return view.SlideOut(direction);
        }
    }

    public class HideCollapseStrategy : IHideStrategy
    {
        private readonly UIView view;

        public HideCollapseStrategy(UIView view)
        {
            this.view = view;
        }

        public IEnumerator Execute()
        {
            return view.Collapse();
        }
    }

    public class HideSlideFadeOutStrategy : IHideStrategy
    {
        private readonly UIView view;
        private readonly SlideDirection direction;
        private readonly CanvasGroup canvasGroup;

        public HideSlideFadeOutStrategy(UIView view, SlideDirection direction, CanvasGroup canvasGroup)
        {
            this.view = view;
            this.direction = direction;
            this.canvasGroup = canvasGroup;
        }

        public IEnumerator Execute()
        {
            view.StartCoroutine(view.SlideOut(direction));
            view.StartCoroutine(view.FadeOut());
            yield return new WaitForSeconds(view.uiHideTime);
        }
    }

    // StrategyFactory

    public static class ShowStrategyFactory
    {
        public static IShowStrategy Create(UIAppearance appearance, UIView view)
        {
            switch (appearance)
            {
                case UIAppearance.None:
                    return new ShowNoneStrategy(view);
                case UIAppearance.FadeIn:
                    return new ShowFadeInStrategy(view);
                case UIAppearance.SlideIn:
                    return new ShowSlideInStrategy(view, view.slideInDirection);
                case UIAppearance.PopUp:
                    return new ShowPopUpStrategy(view);
                case UIAppearance.SlideFadeIn:
                    return new ShowSlideFadeInStrategy(view, view.slideInDirection, view.canvasGroup);
                default:
                    throw new System.ArgumentException("Invalid UIAppearance");
            }
        }
    }

    public static class HideStrategyFactory
    {
        public static IHideStrategy Create(UIHideAppearance appearance, UIView view)
        {
            switch (appearance)
            {
                case UIHideAppearance.None:
                    return new HideNoneStrategy(view);
                case UIHideAppearance.FadeOut:
                    return new HideFadeOutStrategy(view);
                case UIHideAppearance.SlideOut:
                    return new HideSlideOutStrategy(view, view.slideOutDirection);
                case UIHideAppearance.Collapse:
                    return new HideCollapseStrategy(view);
                case UIHideAppearance.SlideFadeOut:
                    return new HideSlideFadeOutStrategy(view, view.slideOutDirection, view.canvasGroup);
                default:
                    throw new System.ArgumentException("Invalid UIHideAppearance");
            }
        }
    }
}
