using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text.RegularExpressions;
using System.Xml.Serialization;

namespace AfterburnerDataHandler.SharedMemory.Afterburner
{
    public class MASM_Formatting
    {
        [XmlElement]
        public string DecimalSeparator
        {
            get { return decimalSeparator; }
            set
            {
                bool isValueChanged = decimalSeparator != value;
                decimalSeparator = value;
                if (isValueChanged) IsDirty = isValueChanged;
            }
        }

        [XmlElement]
        public string GlobalPrefix
        {
            get { return globalPrefix; }
            set
            {
                bool isValueChanged = globalPrefix != value;
                globalPrefix = value;
                if (isValueChanged) IsDirty = isValueChanged;
            }
        }

        [XmlElement]
        public string GlobalPostfix
        {
            get { return globalPostfix; }
            set
            {
                bool isValueChanged = globalPostfix != value;
                globalPostfix = value;
                if (isValueChanged) IsDirty = isValueChanged;
            }
        }

        [XmlElement]
        public MASM_FormattingItemsList FormattingItems
        {
            get { return formattingItems; }
        }

        [XmlIgnore]
        public bool IsDirty
        {
            get { return isDirty; }
            set { isDirty = value; if (value == true) OnParameterChanged(EventArgs.Empty); }
        }

        public event EventHandler<EventArgs> ParameterChanged;

        private string decimalSeparator = ".";
        private string globalPrefix = "";
        private string globalPostfix = "";
        private MASM_FormattingItemsList formattingItems = new MASM_FormattingItemsList();

        private readonly CultureInfo dataFormattingMode;
        private bool isDirty = false;

        public MASM_Formatting()
        {
            FormattingItems.ListChanged += FormattingItemsLisChanged;
            dataFormattingMode = (CultureInfo)CultureInfo.InvariantCulture.Clone();
            dataFormattingMode.NumberFormat.NumberGroupSeparator = "";
        }

        protected virtual void OnParameterChanged(EventArgs e)
        {
            ParameterChanged?.Invoke(this, e);
        }

        private void FormattingItemsLisChanged(object sender, EventArgs e)
        {
            IsDirty = true;
        }

        public string Format(MASM data)
        {
            MASM_FormattingItem item;
            string result = GlobalPrefix ?? "";
            string format;

            if (FormattingItems == null) return result;

            dataFormattingMode.NumberFormat.NumberDecimalSeparator = DecimalSeparator ?? ".";

            for (int i = 0; i < FormattingItems.Count; i++)
            {
                item = FormattingItems[i];

                if (item.enable == true)
                {
                    result += item.prefix ?? "";

                    if (item.mode == MASM_FormattingItem.ItemMode.Text)
                    {
                        result += item.property ?? "";
                    }
                    else if (item.mode == MASM_FormattingItem.ItemMode.Property)
                    {
                        if ((item.outFormat ?? "").Length < 1)
                        {
                            if ((int)item.targetData > 2) format = "{0}";
                            else format = "{0:0.###}";
                        }
                        else
                        {
                            format = item.outFormat;
                        }

                        switch (item.targetData)
                        {
                            case MASM_FormattingItem.TargetData.PropertyValue:
                                result += FormatString(format, ApplyModificators(data.FindProperty(item.property).data, item));
                                break;

                            case MASM_FormattingItem.TargetData.MinLimit:
                                result += FormatString(format, ApplyModificators(data.FindProperty(item.property).minLimit, item));
                                break;

                            case MASM_FormattingItem.TargetData.MaxLimit:
                                result += FormatString(format, ApplyModificators(data.FindProperty(item.property).maxLimit, item));
                                break;

                            case MASM_FormattingItem.TargetData.PropertyUnits:
                                result += FormatString(format, data.FindProperty(item.property).szSrcUnits ?? "");
                                break;

                            case MASM_FormattingItem.TargetData.PropertyFormat:
                                result += FormatString(format, data.FindProperty(item.property).szRecommendedFormat ?? "");
                                break;
                        }
                    }
                    else if (item.mode == MASM_FormattingItem.ItemMode.Time)
                    {
                        result += DateTime.Now.ToString(item.outFormat, dataFormattingMode);
                    }

                    result += item.postfix ?? "";
                }
            }

            result += GlobalPostfix;

            return Regex.Unescape(result ?? "");
        }

        private string FormatString(string format, object obj)
        {
            if (format == null || obj == null) return "";

            try
            {
                return String.Format(dataFormattingMode, format, obj);
            }
            catch
            {
                return String.Format(dataFormattingMode, "{0}", obj);
            }
        }

        private double ApplyModificators(double value, MASM_FormattingItem modificators)
        {
            if (Double.IsNaN(value) || Double.IsInfinity(value)) value = 0;

            switch (modificators.operationType)
            {
                case MASM_FormattingItem.OperationType.Add:
                    value += modificators.operationValue;
                    break;

                case MASM_FormattingItem.OperationType.Subtract:
                    value -= modificators.operationValue;
                    break;

                case MASM_FormattingItem.OperationType.Multiply:
                    value *= modificators.operationValue;
                    break;

                case MASM_FormattingItem.OperationType.Divide:
                    if (modificators.operationValue == 0 ||
                        Double.IsNaN(modificators.operationValue) ||
                        Double.IsInfinity(modificators.operationValue))
                    {
                        value = 0;
                    }
                    else value /= modificators.operationValue;
                    break;

                case MASM_FormattingItem.OperationType.Percent:
                    if (modificators.operationValue == 0 ||
                        Double.IsNaN(modificators.operationValue) ||
                        Double.IsInfinity(modificators.operationValue))
                    {
                        value = 0;
                    }
                    else
                    {
                        value = 100f / modificators.operationValue * value;
                    }
                    break;
            }

            switch (modificators.roundMode)
            {
                case MASM_FormattingItem.RoundingType.Round:
                    value = (float)Math.Round(value, MidpointRounding.ToEven);
                    break;

                case MASM_FormattingItem.RoundingType.RoundDown:
                    if (value > 0) value = (float)Math.Floor(value);
                    else value = (float)Math.Ceiling(value);
                    break;

                case MASM_FormattingItem.RoundingType.RoundUp:
                    if (value > 0) value = (float)Math.Ceiling(value);
                    else value = (float)Math.Floor(value);
                    break;
            }

            if (modificators.modulo == true) value = Math.Abs(value);

            if (Double.IsNaN(modificators.operationValue) ||
                Double.IsInfinity(modificators.operationValue) ||
                (value > -0.0000001f && value < 0.0000001f))
            {
                value = 0;
            }

            return value;
        }
    }
}