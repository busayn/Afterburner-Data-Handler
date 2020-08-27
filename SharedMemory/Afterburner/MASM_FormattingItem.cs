using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AfterburnerDataHandler.SharedMemory.Afterburner
{
    public struct MASM_FormattingItem
    {
        public enum ItemMode
        {
            Text = 0,
            Property = 1,
            Time = 2
        }

        public enum TargetData
        {
            PropertyValue = 0,
            MinLimit = 1,
            MaxLimit = 2,
            PropertyUnits = 3,
            PropertyFormat = 4
        }

        public enum OperationType
        {
            None = 0,
            Add = 1,
            Subtract = 2,
            Multiply = 3,
            Divide = 4,
            Percent = 5
        }

        public enum RoundingType
        {
            None = 0,
            Round = 1,
            RoundDown = 2,
            RoundUp = 3
        }

        public bool enable;
        public ItemMode mode;
        public string property;
        public string prefix;
        public string postfix;
        public TargetData targetData;
        public OperationType operationType;
        public float operationValue;
        public RoundingType roundMode;
        public bool modulo;
        public string outFormat;

        public MASM_FormattingItem(ItemMode mode)
        {
            this.enable = true;
            this.mode = mode;
            this.property = "";
            this.prefix = "";
            this.postfix = "";
            this.targetData = TargetData.PropertyValue;
            this.operationType = OperationType.None;
            this.operationValue = 0;
            this.roundMode = RoundingType.None;
            this.modulo = false;
            this.outFormat = "";
        }

        public static bool operator == (MASM_FormattingItem a, MASM_FormattingItem b)
        {
            return a.enable == b.enable &&
                   a.mode == b.mode &&
                   a.property == b.property &&
                   a.prefix == b.prefix &&
                   a.postfix == b.postfix &&
                   a.targetData == b.targetData &&
                   a.operationType == b.operationType &&
                   a.operationValue == b.operationValue &&
                   a.roundMode == b.roundMode &&
                   a.modulo == b.modulo &&
                   a.outFormat == b.outFormat;
        }

        public static bool operator != (MASM_FormattingItem a, MASM_FormattingItem b)
        {
            return !(a == b);
        }

        public override bool Equals(object obj) { return base.Equals(obj); }
        public override int GetHashCode() { return base.GetHashCode(); }
    }
}
