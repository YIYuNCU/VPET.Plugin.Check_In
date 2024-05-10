using LinePutScript;
using LinePutScript.Converter;

namespace VPET.Evian.Check_In
{
    public class Setting : Line
    {
        public Setting(ILine line) : base(line)
        {
        }
        public Setting()
        {
        }
        /// <summary>
        /// 启用Check_In
        /// </summary>
        [Line]
        public bool Enable { get; set; } = true;
        
    }
}
