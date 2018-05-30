var ValidateForm =
    {
        input: null,
        IsEmail: function ()
        {
            var reg = /^(([^<>()[\]\\.,;:\s@\"]+(\.[^<>()[\]\\.,;:\s@\"]+)*)|(\".+\"))@((\[[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}\])|(([a-zA-Z\-0-9]+\.)+[a-zA-Z]{2,}))$/igm;
            if (reg.test(this.input.value.trim()))
            {
                this.Success();
                return true;
            } else
            {
                this.Error();
                return false;
            }
        },
        IsEmpty: function()
        {
            if (this.input.value.trim() != '')
            {
                this.Success();
                return true;
            } else
            {
                this.Error();
                return false;
            }
        },
        IsName: function()
        {
            if (this.input.value.trim() != '' && this.input.value.split(' ').length -1 >= 1)
            {
                this.Success();
                return true;
            } else
            {
                this.Error();
                return false;
            }
        },
        IsNumberAndSpaces: function()
        {
            if (/^\d+$/.test(this.input.value))
            {
                this.Success();
                return true;
            } else
            {
                this.Error();
                return false;
            }
        },
        Success: function ()
        {
            this.input.style.backgroundColor = '';
            this.input.style.color = '';
            this.input.style.border = '';
        },
        Error: function ()
        {
            this.input.style.backgroundColor = '#F2DEDE';
            this.input.style.color = '#B94A48';
            this.input.style.border = '1px solid';
        }
    };