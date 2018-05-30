$(document).ready(function () {
    $('#formcontacto').hide();
    $('#contentPolitics').hide();
    $('#EMECI').mask('99999-9999-9999');
    $('#Phone').mask('9999999999')
})

$('#EMECI').focus(function () {
    if (!$('#contentPolitcs').is(':visible')) $('#contentPolitics').show();
})

$('#politics').change(function () {
    if ($(this).is(':checked'))
        $('#contentPolitics').css({ 'background-color': '#F2F5A9', 'color': '', 'border': '' });
})

function NewFile()
{
    var form = $('#formcontacto');
    if (!$(form).is(':visible'))
    {
        form.show();
        $('#Name').focus();
        $('#contentPolitics').show();
        return false;
    }

    var isValid = true;
    var InputNewFile = ['Name', 'Phone', 'Email'];
    InputNewFile.map(function (i) {
        var v = true;
        ValidateForm.input = document.getElementById(i);
        switch (i)
        {
            case 'Name':
                v = ValidateForm.IsName();
                break;
            case 'Phone':
                v = ValidateForm.IsNumberAndSpaces();
                break;
            case 'Email':
                v = ValidateForm.IsEmail();
                break;
        }

        isValid = (isValid && v);
        var isValidPolitics = ValidatePolitics();
        isValid = (isValid && isValidPolitics);
    })

    return isValid;
}

function Renewal()
{
    var isValid = true;
    ValidateForm.input = document.getElementById('EMECI');
    isValid = (isValid && ValidateForm.IsEmpty());
    var isValidPolitics = ValidatePolitics();
    isValid = (isValid && isValidPolitics);

    return isValid;
}

function ValidatePolitics()
{
    var isValid = true;
    if (!$('#politics').is(':checked'))
    {
        isValid = false;
        ValidateForm.input = document.getElementById('contentPolitics');
        ValidateForm.Error();
    }

    return isValid;
}