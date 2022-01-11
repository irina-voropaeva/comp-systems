using System;
using System.Collections.Generic;

namespace Core
{

    public enum IterationOptions
    {
        ToCollectionEnd,
        KeepIterating
    }

    public enum Direction
    {
        Forward,
        Backward
    }

    public class Iterator<TIterable>
    {
        public Iterator(List<TIterable> collection, IterationOptions option)
            : this(collection) => iterationOption = option;

        public Iterator(List<TIterable> collection)
        {
            this.collection = collection;
            CurrentIndex = 0;
            Iteration = 1;
            CurrentElement = collection[CurrentIndex];
            NextElement = collection[CurrentIndex + 1];
            iterationOption = IterationOptions.ToCollectionEnd;
        }

        public bool MoveNext => CurrentIndex < CollectionLength;
        public bool CollectionEnd => CurrentIndex == CollectionLength;
        public int CollectionLength => collection.Count - 1;
        public TIterable CurrentElement { get; private set; }
        public TIterable NextElement { get; private set; }
        public int CurrentIndex { get; private set; }
        public int Iteration { get; private set; }

        private readonly List<TIterable> collection;
        private readonly IterationOptions iterationOption;

        public void MoveForward()
        {
            if (MoveNext)
            {
                CurrentIndex++;
                CurrentElement = collection[CurrentIndex];

                if (!CollectionEnd) NextElement = collection[CurrentIndex + 1];
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
                CurrentElement = collection[CurrentIndex];
            }
        }

        public void MoveTo(int position)
        {
            if (position < 0 || position > CollectionLength) return;

            CurrentIndex = position;
            CurrentElement = collection[position];
            NextElement = collection[position + 1];
        }

        public TIterable LookAt(int position, Direction direction)
        {
            var lookAtPosition = direction is Direction.Forward ? CurrentIndex + position : CurrentIndex - position;

            return lookAtPosition < CollectionLength + 1 && lookAtPosition >= 0
                ? collection[lookAtPosition]
                : default;
        }

        public void ChangeCollection(Action<List<TIterable>> change)
        {
            try
            {
                change(collection);
                ChangeCurrentIndex();
            }
            catch (Exception)
            {
                throw;
            }
        }

        private void ChangeCurrentIndex()
        {
            var currentExist = collection.Contains(CurrentElement);

            if (currentExist)
            {
                CurrentIndex = collection.IndexOf(CurrentElement);
                NextElement = CollectionEnd ? default : collection[CurrentIndex + 1];
            }
            else ResetIndex();
        }

        private void ResetIndex()
        {
            CurrentIndex = 0;
            var hasElements = CollectionLength > 0;

            CurrentElement = hasElements ? collection[CurrentIndex] : default;
            NextElement = hasElements ? collection[CurrentIndex + 1] : default;
        }
    }
}