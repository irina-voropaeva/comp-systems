using System;
using System.Collections.Generic;

namespace Core.Iterating 
{

    public class Iterator<TIterable>
    {
        public Iterator(List<TIterable> list, IterationOptions option)
            : this(list) => iterationOption = option;

        public Iterator(List<TIterable> list)
        {
            this._list = list;
            CurrentIndex = 0;
            Iteration = 1;
            CurrentElement = list[CurrentIndex];
            NextElement = list[CurrentIndex + 1];
            iterationOption = IterationOptions.ToCollectionEnd;
        }

        public bool MoveNext => CurrentIndex < CollectionLength;
        public bool CollectionEnd => CurrentIndex == CollectionLength;
        public int CollectionLength => _list.Count - 1;
        public TIterable CurrentElement { get; private set; }
        public TIterable NextElement { get; private set; }
        public int CurrentIndex { get; private set; }
        public int Iteration { get; private set; }

        private readonly List<TIterable> _list;
        private readonly IterationOptions iterationOption;

        public void MoveForward()
        {
            if (MoveNext)
            {
                CurrentIndex++;
                CurrentElement = _list[CurrentIndex];

                if (!CollectionEnd) NextElement = _list[CurrentIndex + 1];
                else NextElement = default;
            }
            else
            {
                if (iterationOption is IterationOptions.KeepIterating)
                {
                    ResetIndex();
                    Iteration++;
                }
                else CurrentElement = default;
            }
        }

        public void MoveBackward()
        {
            if (CurrentIndex > 0)
            {
                CurrentIndex--;
                NextElement = CurrentElement;
                CurrentElement = _list[CurrentIndex];
            }
        }

        public void MoveTo(int position)
        {
            if (position < 0 || position > CollectionLength) return;

            CurrentIndex = position;
            CurrentElement = _list[position];
            NextElement = _list[position + 1];
        }

        public TIterable LookAt(int position, Direction direction)
        {
            var lookAtPosition = direction is Direction.Forward ? CurrentIndex + position : CurrentIndex - position;

            return lookAtPosition < CollectionLength + 1 && lookAtPosition >= 0
                ? _list[lookAtPosition]
                : default;
        }

        public void ChangeCollection(Action<List<TIterable>> change)
        {
            try
            {
                change(_list);
                ChangeCurrentIndex();
            }
            catch (Exception)
            {
                throw;
            }
        }

        private void ChangeCurrentIndex()
        {
            var currentExist = _list.Contains(CurrentElement);

            if (currentExist)
            {
                CurrentIndex = _list.IndexOf(CurrentElement);
                NextElement = CollectionEnd ? default : _list[CurrentIndex + 1];
            }
            else ResetIndex();
        }

        private void ResetIndex()
        {
            CurrentIndex = 0;
            var hasElements = CollectionLength > 0;

            CurrentElement = hasElements ? _list[CurrentIndex] : default;
            NextElement = hasElements ? _list[CurrentIndex + 1] : default;
        }
    }
}