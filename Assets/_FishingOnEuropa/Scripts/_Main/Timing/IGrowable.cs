namespace Europa.GameEvents
{
    public interface IGrowable
    {
        /// <summary>
        /// Called when this object grows. Growth is handled by the <see cref="UnobservedTimePassageHandler"/>.
        /// From <see cref="GrowthObjectBase.OnUnobservedGrowth(int)"/>.
        /// </summary>
        /// <param name="wasObserved">Is true if the object is loaded when a daily event fires,
        /// false if the object has just been loaded in after daily events occurred during the object's inactivity.</param>
        /// <param name="cycles">For when <paramref name="wasObserved"/> is false, this is the amount of times the growth occurred in the background.
        /// Eg: if an apple tree is unloaded for 3 mornings, we'll need to make it grow for 3 stages when it is loaded in.</param>
        protected void OnGrowth(bool wasObserved = true, int cycles = 1);

        /// <summary>
        /// Retrieves the environmental location of this growable.
        /// When this location is loaded, the growables in this location will grow if any daily events have passed.
        /// </summary>
        /// <returns>The growth zone of the growable object.</returns>
        protected string GetGrowthZone();

        /// <summary>
        /// Registers this growable with the growth handler.
        /// </summary>
        public void RegisterGrowable()
        {
            // Implementation needed
        }
    }


}

