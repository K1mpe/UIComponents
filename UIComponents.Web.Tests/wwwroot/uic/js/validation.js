uic.validation = uic.validation || {
    //return true if this string is recognised as color
    isColor: function (value) {
        let words = ['inherit', 'initial', 'unset', 'revert', 'revert-layer', 'currentcolor'];
        let style = new Option().style;
        style.color = value;

        // return '' != style.color && !words.includes(value.toLowerCase());
        return CSS.supports('color', value) && !words.includes(value.toLowerCase());
    },

    //This block sets events to validate on changes
    init: {
        validateColor: function (element) {
            element = $(element);
            $(document).keyup(() => uic.validation.validateColor(element));
            element.blur(() => uic.validation.validateColor(element));
        },
        validateDateTime: function (element) {
            element = $(element);
            element.keyup(() => uic.validation.validateDateTime(element));
            element.blur(() => uic.validation.validateDateTime(element));
            element.change(() => uic.validation.validateDateTime(element));
        },
        validateText: function (element) {
            element = $(element);
            element.keyup(() => uic.validation.validateText(element));
            element.blur(() => uic.validation.validateText(element));
        },
        validateNumber: function (element) {
            element = $(element);
            element.keyup(() => uic.validation.validateNumber(element));
            element.blur(() => uic.validation.validateNumber(element));
        },
        validateSelect: function (element) {
            element = $(element);
            element.change(() => uic.validation.validateSelect(element));
            element.blur(() => uic.validation.validateSelect(element));

        }
    },
    validateColor: function (element) {
        let el = $(element);
        let currentValue = uic.getValue(el);
        let id = el.attr('id');
        let validationSpan = this._getValidationSpan(el.attr('name'), el);
        if (!validationSpan.length)
            return;
        validationSpan.text("");


        if (el.attr('required') != undefined) 
            this._validateRequired(el, validationSpan, currentValue);

        if (el.attr('data-validation-valid-color') != undefined)
            this._validateIsValidColor(el, validationSpan, currentValue);
    },
    validateDateTime: function (element) {
        let el = $(element);
        let currentValue = uic.getValue(el);
        let id = el.attr('id');
        let validationSpan = this._getValidationSpan(el.attr('name'), el);
        if (!validationSpan.length)
            return;
        validationSpan.text("");

        if (el.attr('required') != undefined)
            this._validateRequired(el, validationSpan, currentValue);
        if (el.attr('min'))
            this._validateMinDateTime(el, validationSpan, currentValue);
        if (el.attr('max'))
            this._validateMaxDateTime(el, validationSpan, currentValue);
    },
    validateText: function (element) {
        let el = $(element);
        let currentValue = uic.getValue(el);
        let id = el.attr('id');
        let validationSpan = this._getValidationSpan(el.attr('name'), el);
        if (!validationSpan.length)
            return;
        validationSpan.text("");

        if (el.attr('required') != undefined)
            this._validateRequired(el, validationSpan, currentValue);
        if (el.attr('minlength'))
            this._validateMinLength(el, validationSpan, currentValue);
        if (el.attr('maxlength'))
            this._validateMaxLength(el, validationSpan, currentValue);
    },
    validateNumber: function (element) {
        let el = $(element);
        let currentValue = uic.getValue(el);
        let id = el.attr('id');
        let validationSpan = this._getValidationSpan(el.attr('name'), el);
        if (!validationSpan.length)
            return;
        validationSpan.text("");

        if (el.attr('required') != undefined)
            this._validateRequired(el, validationSpan, currentValue);
        if (el.attr('min'))
            this._validateMinValue(el, validationSpan, currentValue);
        if (el.attr('max'))
            this._validateMaxValue(el, validationSpan, currentValue);
    },
    validateSelect: function (element) {
        let el = $(element);
        let currentValue = uic.getValue(el);
        let id = el.attr('id');
        let validationSpan = this._getValidationSpan(el.attr('name'), el);
        if (!validationSpan.length)
            return;
        validationSpan.text("");

        if (el.attr('required') != undefined)
            this._validateRequired(el, validationSpan, currentValue);
        
    },

    //gets the translated propertyname from a element
    _getPropertyName: function (el) {
        if (el.attr('propertyname'))
            return el.attr('propertyname');

        let id = el.attr('id');
        if (id != undefined) {
            let label = $(`label[for="${id}"]`);
            if (label.length) {
                let text = Array.from(label[0].childNodes)
                    .filter(node => node.nodeType === Node.TEXT_NODE)
                    .map(node => node.textContent.trim())
                    .join('');
                if (!text.length)
                    text = label.text();
                return text.trim();
            }
        }
        return '{Unknown property}';
    },
    _getValidationSpan: function (propertyName, el) {
        let form = $(el).closest('form');
        let spanElement = $();
        if(form.length)
            spanElement = form.find(`span.field-validation-valid[data-valmsg-for="${propertyName}"]`);
        if (!spanElement.length)
            spanElement = $(`span.field-validation-valid[data-valmsg-for="${propertyName}"]`);
        if (!spanElement.length)
            spanElement = $(`span.field-validation-valid[data-valmsg-for$=".${propertyName}"]`);
        if (!spanElement.length) {
            try {
                //Find any span for this element and add the class text-danger
                let spanElement = $(`span[for=${propertyName}]`);
                spanElement.removeClass();
                spanElement.addClass("text-danger");
            } catch {
                //May crash silently if propertyName contains []
            }

        }
        return spanElement;
    },

    _validateIsValidColor: function (el, span, value) {
        if (span.length == 0)
            return;
        if (value == null || value == undefined || value == '')
            return;

        if (!this.isColor(value)) {
            let propName = this._getPropertyName(el);
            let validationText = this._validationMessages.invalidColor;
            let text = uic.translation.replacePlaceholders(validationText, {
                PropertyName: propName,
                PropertyValue: value
            });
            span.text(text)
        }
    },
    _validateMaxDateTime: function (el, span, value) {
        if (span.length == 0)
            return;
        value = new moment(value);
        let maxDate = new moment(el.attr('max'));
        if (value > maxDate) {
            let propName = this._getPropertyName(el);
            let validationText = this._validationMessages.maxValue;
            let text = uic.translation.replacePlaceholders(validationText, {
                ComparisonValue: maxDate.format(),
                PropertyName: propName,
                PropertyValue: value
            });
            span.text(text)
        }
    },
    _validateMaxLength: function (el, span, value) {
        if (span.length == 0)
            return;
        let maxLength = el.attr('maxlength');
        if (value == null || value.length == 0 || maxLength == undefined)
            return;
        let length = value.length;
        if (length > +maxLength) {
            let propName = this._getPropertyName(el);
            let validationText = this._validationMessages.maxLength;
            let text = uic.translation.replacePlaceholders(validationText, {
                ComparisonValue: maxLength,
                PropertyName: propName,
                PropertyValue: value
            });
            span.text(text)
        }
    },
    _validateMaxValue: function (el, span, value) {
        if (span.length == 0)
            return;
        let maxVal = el.attr('max');
        if (value == null || value.length == 0 || maxVal == undefined)
            return;
        if (+value > +maxVal) {
            let propName = this._getPropertyName(el);
            let validationText = this._validationMessages.maxValue;
            let text = uic.translation.replacePlaceholders(validationText, {
                ComparisonValue: maxVal,
                PropertyName: propName,
                PropertyValue: value
            });
            span.text(text)
        }
    },
    _validateMinDateTime: function (el, span, value) {
        if (span.length == 0)
            return;
        value = new moment(value);
        let minDate = new moment(el.attr('min'));
        if (+value < +minDate) {
            let propName = this._getPropertyName(el);
            let validationText = this._validationMessages.minValue;
            let text = uic.translation.replacePlaceholders(validationText, {
                ComparisonValue: minDate.format(),
                PropertyName: propName,
                PropertyValue: value
            });
            span.text(text)
        }
    },
    _validateMinLength: function (el, span, value) {
        if (span.length == 0)
            return;
        let minLength = el.attr('minlength');
        if (value == null || value.length == 0 || minLength == undefined)
            return;
        let length = value.length;
        if (length < +minLength) {
            let propName = this._getPropertyName(el);
            let validationText = this._validationMessages.minLength;
            let text = uic.translation.replacePlaceholders(validationText, {
                ComparisonValue: minLength,
                PropertyName: propName,
                PropertyValue: value
            });
            span.text(text)
        }
    },
    _validateMinValue: function (el, span, value) {
        if (span.length == 0)
            return;
        let minVal = el.attr('min');
        if (value == null || value.length == 0 || minVal == undefined)
            return;
        if (+minVal > +value) {
            let propName = this._getPropertyName(el);
            let validationText = this._validationMessages.minValue;
            let text = uic.translation.replacePlaceholders(validationText, {
                ComparisonValue: minVal,
                PropertyName: propName,
                PropertyValue: value
            });
            span.text(text)
        }
    },
    _validateRequired: function (el, span, value) {
        if (span.length == 0)
            return;
        if (value == null || value == undefined || value == '') {
            let propName = this._getPropertyName(el);
            let validationText = this._validationMessages.required;
            let text = uic.translation.replacePlaceholders(validationText, {
                PropertyName: propName,
                PropertyValue: value
            });
            span.text(text)
        }
    },


    _validationMessages: {
        invalidColor: null,
        maxLength: null,
        maxValue: null,
        minLength: null,
        minValue: null,
        required: null,
    }
}