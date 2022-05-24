namespace AugustEngine.Procedural
{
    /// <summary>
    /// A data structure for returning weight-based selections randomly and 
    /// deterministicly based on seeded values
    /// <seealso cref="NoiseGeneration.SquirrelNoise(int, long)"/>
    /// </summary>
    [System.Serializable]
    
    public class PopulationTable
    {
      
        /// <summary>
        /// What a <see cref="PopulationTable"/> selects from
        /// </summary>
        [System.Serializable]

        public class TableOption
        {
            private object obj = null;
            public object Obj { get => obj; }
            private uint weight = 0;
            public uint Weight { get => weight; }
            /// <summary>
            /// <see cref="TableOption"/> constructor
            /// </summary>
            /// <param name="obj">The object to return</param>
            /// <param name="weight">The weighted chance of selecting this object</param>
            public TableOption(object obj, uint weight)
            {

                this.obj = obj;
                //must have a weight of at least 1
                this.weight = weight == 0 ? 1 : weight;
            }
        }
        private TableOption[] options = new TableOption[0];
        private long sum;
        /// <summary>
        /// Population table consturctor
        /// </summary>
        /// <param name="option">All possible options to select from</param>
        public PopulationTable(params TableOption[] option )
        {
            options = option;
            sum = 0;
            for (int i = 0; i < option.Length; i++)
            {
                sum += option[i].Weight;
            }
        }

        public object Select(uint seed = 0)
        {
            if (options == null || options.Length == 0) return null;

            //seed if needed
            if (seed == 0)
            {
                seed = (uint)System.DateTime.Now.Millisecond;
            }
            long selection = NoiseGeneration.SquirrelNoise((int) seed, seed);
            selection = selection % sum;
            //go throught each option until our pointer exceeds the selection
            long pointer = 0;
            for (int i = 0; i < options.Length; i++)
            {
                if (pointer >= selection) return options[i];
                
                pointer += options[i].Weight;          
            }


            //we shouldn't get here
            return options[options.Length - 1];
        }
    }
}

