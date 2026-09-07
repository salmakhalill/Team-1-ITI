$(document).ready(function () {

    var $form = $(".dynamic-form");

    function reindexRows() {
        $('#items-tbody .item-row').each(function (index) {
            $(this).find('.product-select').attr('name', `Items[${index}].ProductId`);
            $(this).find('.qty-input').attr('name', `Items[${index}].Quantity`);
            $(this).find('.cost-input').attr('name', `Items[${index}].UnitCost`);
        });

        $form.removeData('validator');
        $form.removeData('unobtrusiveValidation');
        $.validator.unobtrusive.parse($form);
    }

    function calculateTotals() {
        let grandTotal = 0;
        $('#items-tbody .item-row').each(function () {
            let qty = parseFloat($(this).find('.qty-input').val()) || 0;
            let cost = parseFloat($(this).find('.cost-input').val()) || 0;
            let lineTotal = qty * cost;
            grandTotal += lineTotal;
            $(this).find('.line-total').text('$' + lineTotal.toFixed(2));
        });
        $('#grand-total').text('$' + grandTotal.toFixed(2));
    }

    function checkDuplicates(selectElem) {
        let currentVal = $(selectElem).val();
        if (!currentVal) return;

        let count = 0;
        $('#items-tbody .product-select').each(function () {
            if ($(this).val() === currentVal) count++;
        });

        if (count > 1) {
            alert('This item is already in the list. Please increase the quantity instead.');
            $(selectElem).val('');
        }
    }

    $('#items-tbody').on('input', '.calc-input', calculateTotals);

    $('#items-tbody').on('change', '.product-select', function () {
        checkDuplicates(this);
    });

    $('#items-tbody').on('click', '.btn-remove', function () {
        if ($('#items-tbody .item-row').length > 1) {
            $(this).closest('tr').remove();
            reindexRows();
            calculateTotals();
        } else {
            alert('The document must have at least one item.');
        }
    });

    $('#btn-add-item').click(function () {
        let newRow = $('#row-template').html();
        $('#items-tbody').append(newRow);

        if (typeof lucide !== 'undefined') {
            lucide.createIcons();
        }

        reindexRows();
        calculateTotals();
    });

    calculateTotals();

<<<<<<< HEAD
    $form.on('blur', 'input, select, textarea', function () {
=======
    $form.on('blur', 'input, select', function () {
>>>>>>> origin/develop
        $(this).valid();
    });
});